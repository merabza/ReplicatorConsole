using AppCliTools.CliParametersEdit.Counters;
using AppCliTools.CliParametersEdit.Generators;
using DatabaseTools.DbTools;
using DatabaseTools.DbTools.Models;
using LanguageExt;
using Microsoft.Extensions.Logging;
using OneOf;
using ParametersManagement.LibDatabaseParameters;
using ParametersManagement.LibParameters;
using ReplicatorConsole.Counters;
using ReplicatorShared.Data.Models;
using ReplicatorShared.Data.Steps;
using SystemTools.SystemToolsShared;
using SystemTools.SystemToolsShared.Errors;
using ToolsManagement.DatabasesManagement;

namespace ReplicatorConsole.Generators;

internal sealed class StandardJobsSchemaGenerator
{
    private readonly string _appName;
    private readonly string _databaseServerConnectionName;
    private readonly ILogger _logger;
    private readonly string? _parametersFileName;
    private readonly IParametersManager _parametersManager;
    private readonly bool _useConsole;

    // ReSharper disable once ConvertToPrimaryConstructor
    public StandardJobsSchemaGenerator(string appName, bool useConsole, ILogger logger,
        IParametersManager parametersManager, string databaseServerConnectionName, string? parametersFileName)
    {
        _appName = appName;
        _useConsole = useConsole;
        _logger = logger;
        _parametersManager = parametersManager;
        _databaseServerConnectionName = databaseServerConnectionName;
        _parametersFileName = parametersFileName;
    }

    internal async ValueTask Generate(CancellationToken cancellationToken = default)
    {
        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        OneOf<IDatabaseManager, Error[]> createDatabaseManagerResult =
            await DatabaseManagersFactory.CreateDatabaseManager(_appName, _logger, true, _databaseServerConnectionName,
                new DatabaseServerConnections(parameters.DatabaseServerConnections), cancellationToken);

        if (createDatabaseManagerResult.IsT1)
        {
            Error.PrintErrorsOnConsole(createDatabaseManagerResult.AsT1);
        }

        IDatabaseManager? dac = createDatabaseManagerResult.AsT0;
        Option<Error[]> testConnectionResult = await dac.TestConnection(null, cancellationToken);
        if (testConnectionResult.IsSome)
        {
            StShared.WriteErrorLine("Can not connect to server. Generation process stopped", true, _logger);
            return;
        }

        //შემოწმდეს არსებობს თუ არა ყოველდღიური შედულე 
        //და თუ არ არსებობს, დაემატოს ღამის 4 საათზე.
        string scheduleDailyName = CreateJobScheduleDaily(parameters);

        //შემოწმდეს არსებობს თუ არა სტარტაპის შედულე 
        //და თუ არ არსებობს, დაემატოს.
        string scheduleAtStartName = CreateJobScheduleAtStart(parameters);

        //შემოწმდეს არსებობს თუ არა შედულე რომელიც პროცესს გაუშვებს 
        //საათში ერთხელ ოღონდ შუა საათში
        //და თუ არ არსებობს, დაემატოს.
        string scheduleHourlyName = CreateJobScheduleHourly(parameters);

        StandardSmartSchemas.Generate(_parametersManager);

        //თუ არ არსებობს დაემატოს ჭკვიანი სქემები: DailyStandard, Reduce, Hourly
        //var smartSchemaDailyStandardName = standardSmartSchemas.SmartSchemaDailyStandardName; //DailyStandard
        //var smartSchemaReduceName = standardSmartSchemas.SmartSchemaReduceName; //Reduce
        //var smartSchemaHourlyName = standardSmartSchemas.SmartSchemaHourlyName; //Hourly

        StandardArchiversGenerator.Generate(_useConsole, _parametersManager);

        //თუ არ არსებობს შეიქმნას zipClass არქივატორი
        //var archiverZipClassName = standardArchiversGenerator.ArchiverZipClassName; //ZipClass
        //string archiverZipName = standardArchiversGenerator.ArchiverZipName; //Zip
        //string archiverRarName = standardArchiversGenerator.ArchiverRarName; //Rar

        //1. დადგინდეს SQL სერვერი ლოკალურია თუ მოშორებული.
        OneOf<bool, Error[]> isServerLocalResult = await dac.IsServerLocal(cancellationToken);
        bool isServerLocal = false;
        if (isServerLocalResult.IsT0)
        {
            isServerLocal = isServerLocalResult.AsT0;
        }

        string fullBuFileStorageName = RegisterFileStorage(EBackupType.Full);
        string trLogBuFileStorageName = RegisterFileStorage(EBackupType.TrLog);

        OneOf<DbServerInfo, Error[]> getDatabaseServerInfoResult = await dac.GetDatabaseServerInfo(cancellationToken);
        if (getDatabaseServerInfoResult.IsT1)
        {
            Error.PrintErrorsOnConsole(getDatabaseServerInfoResult.AsT1);
            StShared.WriteErrorLine("dbServerInfo does not created. Generation process stopped", true, _logger);
            return;
        }

        DbServerInfo? dbServerInfo = getDatabaseServerInfoResult.AsT0;

        //დასაშვებია თუ არა სერვერის მხარეს ბექაპირებისას კომპრესია
        bool isServerAllowsCompression = dbServerInfo.AllowsCompression;

        var uploadFileStorageCruderNameCounter =
            new FileStorageCruderNameCounter(_logger, _parametersManager, "Upload FileStorage", null);
        string? uploadFileStorageName = await uploadFileStorageCruderNameCounter.Count(cancellationToken);

        var stepNamePrefixCounter =
            new StepNamePrefixCounter(_appName, _logger, _parametersManager, _databaseServerConnectionName);
        string stepNamePrefix = stepNamePrefixCounter.Count();

        const string dateMask = DateMaskKeeper.DateMask;

        //დავიანგარიშოთ ლოკალურად სად უნდა იყოს სრული ბექაპების ფაილები
        //LocalPathCounter databaseFullBackupsLocalPathCounter =
        //    LocalPathCounterFactory.CreateDatabaseBackupsLocalPathCounter(parameters,
        //        _parametersManager.ParametersFileName,
        //        EBackupType.Full);
        //string databaseFullBackupsLocalPath = databaseFullBackupsLocalPathCounter.Count(null);

        string? databaseFullBackupsLocalPath =
            parameters.CountLocalPath(null, _parametersFileName, $"Database{EBackupType.Full}Backups");

        //ბაზების სრული ბექაპი
        CreateBackupStep(EBackupType.Full, isServerAllowsCompression, true, stepNamePrefix, dateMask,
            StandardSmartSchemas.DailyStandardSmartSchemaName, StandardSmartSchemas.ReduceSmartSchemaName,
            isServerLocal ? fullBuFileStorageName : null, databaseFullBackupsLocalPath,
            isServerAllowsCompression ? null : nameof(EArchiveType.ZipClass), uploadFileStorageName, scheduleDailyName,
            scheduleAtStartName, parameters);

        //დავიანგარიშოთ ლოკალურად სად უნდა იყოს ტრანზაქშენ ლოგების ბექაპების ფაილები
        //LocalPathCounter databaseTrLogBackupsLocalPathCounter =
        //    LocalPathCounterFactory.CreateDatabaseBackupsLocalPathCounter(parameters,
        //        _parametersManager.ParametersFileName,
        //        EBackupType.TrLog);
        //string databaseTrLogBackupsLocalPath = databaseTrLogBackupsLocalPathCounter.Count(null);

        string? databaseTrLogBackupsLocalPath =
            parameters.CountLocalPath(null, _parametersFileName, $"Database{EBackupType.TrLog}Backups");

        //ტრანზაქშენ ლოგების ბექაპი, საათობრივად, მხოლოდ იმ ბაზებისათვის, რომელთათვისაც დასაშვებია ტრანზაქშენ ლოგებით ბექაპი
        CreateBackupStep(EBackupType.TrLog, false, false, stepNamePrefix, dateMask,
            StandardSmartSchemas.HourlySmartSchemaName, StandardSmartSchemas.HourlySmartSchemaName,
            isServerLocal ? trLogBuFileStorageName : null, databaseTrLogBackupsLocalPath, null, uploadFileStorageName,
            scheduleHourlyName, scheduleAtStartName, parameters);

        //8.1. პროცედურების გადაკომპილირება, 
        CreateMaintenanceStep(EMultiDatabaseActionType.RecompileProcedures, stepNamePrefix, scheduleDailyName,
            scheduleAtStartName, parameters);

        //8.2. სტატისტიკის გადაანგარიშება
        CreateMaintenanceStep(EMultiDatabaseActionType.UpdateStatistics, stepNamePrefix, scheduleDailyName,
            scheduleAtStartName, parameters);

        //8.3. ბაზების მთლიანობის შემოწმება
        CreateMaintenanceStep(EMultiDatabaseActionType.CheckRepairDataBase, stepNamePrefix, scheduleDailyName,
            scheduleAtStartName, parameters);
    }

    private string RegisterFileStorage(EBackupType backupType)
    {
        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        var fileStorageGenerator = new FileStorageGenerator(_parametersManager);

        string? fullPath = parameters.CountLocalPath(null, _parametersFileName, $"Database{backupType}Backups");

        if (string.IsNullOrWhiteSpace(fullPath))
        {
            throw new Exception("fullPath does not counted");
        }

        var dir = new DirectoryInfo(fullPath);
        fileStorageGenerator.GenerateForLocalPath(dir.Name, fullPath);
        return dir.Name;
    }

    private static void CreateScheduleByJobStep(string jobStepName, string scheduleName,
        ReplicatorParameters parameters)
    {
        if (parameters.JobsBySchedules.Any(a => a.JobStepName == jobStepName && a.ScheduleName == scheduleName))
        {
            return;
        }

        int nextSequentialNumber = parameters.JobsBySchedules.Where(w => w.ScheduleName == scheduleName)
            .DefaultIfEmpty().Max(m => m?.SequentialNumber ?? 0) + 1;

        parameters.JobsBySchedules.Add(new JobStepBySchedule(jobStepName, scheduleName, nextSequentialNumber));
    }

    private void CreateMaintenanceStep(EMultiDatabaseActionType multiDatabaseActionType, string stepNamePrefix,
        string scheduleNameFirst, string scheduleNameSecond, ReplicatorParameters parameters)
    {
        string stepName = $"{stepNamePrefix} - {multiDatabaseActionType} for all databases";

        if (parameters.MultiDatabaseProcessSteps.ContainsKey(stepName))
        {
            return;
        }

        var multiDatabaseProcessStep = new MultiDatabaseProcessStep
        {
            ActionType = multiDatabaseActionType, //გასაშვები პროცესის ტიპი
            DatabaseServerConnectionName =
                _databaseServerConnectionName, //ბაზასთან დაკავშირების პარამეტრების ჩანაწერის სახელი
            DatabaseSet = EDatabaseSet.AllDatabases, //ბაზების სიმრავლე, რომლისთვისაც უნდა გაეშვას ეს პროცესი.

            //JobStep
            //SequentialNumber = GetNextJobStepSequentialNumber(),
            ProcLineId = 1,
            DelayMinutesBeforeStep = 0,
            DelayMinutesAfterStep = 0,
            HoleStartTime = new TimeSpan(0, 0, 0),
            HoleEndTime = new TimeSpan(23, 59, 59),
            Enabled = true,
            PeriodType = EPeriodType.Day,
            FreqInterval = 1,
            StartAt = DateTime.Today
        };

        parameters.MultiDatabaseProcessSteps.Add(stepName, multiDatabaseProcessStep);
        CreateScheduleByJobStep(stepName, scheduleNameFirst, parameters);
        CreateScheduleByJobStep(stepName, scheduleNameSecond, parameters);
    }

    private void CreateBackupStep(EBackupType backupType, bool useCompression, bool useVerification,
        string stepNamePrefix, string dateMask, string mainSmartSchemaName, string reduceSmartSchemaName,
        string? databaseFileStorageName, string? databaseBackupsLocalPath, string? archiverName,
        string? uploadFileStorageName, string scheduleNameFirst, string scheduleNameSecond,
        ReplicatorParameters parameters)
    {
        string backupStepName = $"{stepNamePrefix} {backupType} Backup";

        if (parameters.DatabaseBackupSteps.ContainsKey(backupStepName))
        {
            return;
        }

        var backupNameMiddlePartCounter = new BackupNameMiddlePartCounter(backupType);
        var backupFileExtensionCounter = new BackupFileExtensionCounter(backupType);
        var procLineCounter = new ProcLineCounter(_appName, _logger, _parametersManager, _databaseServerConnectionName,
            databaseFileStorageName, uploadFileStorageName);
        var smartSchemaNameCounter =
            new SmartSchemaNameCounter(_parametersManager, databaseFileStorageName, uploadFileStorageName);

        var databaseBackupStep = new DatabaseBackupStep
        {
            DatabaseServerConnectionName = _databaseServerConnectionName,
            DatabaseSet = EDatabaseSet.AllDatabases,
            DatabaseBackupParameters = new DatabaseParameters
            {
                BackupType = backupType,
                //ბექაპირება
                Compress = useCompression,
                Verify = useVerification,
                //ბექაპის ფაილის სახელის დარქმევა
                BackupNamePrefix = $"{stepNamePrefix}_",
                BackupNameMiddlePart = backupNameMiddlePartCounter.Count(),
                DateMask = dateMask,
                BackupFileExtension = backupFileExtensionCounter.Count()
            },
            //DbServerSideBackupPath = databaseBackupsLocalPath,

            //ბაზის სერვერის მხარე
            SmartSchemaName = smartSchemaNameCounter.Count(ESmartSchemaCase.DatabaseServerSide, mainSmartSchemaName,
                reduceSmartSchemaName),
            FileStorageName = databaseFileStorageName,
            //ჩამოტვირთვა და ლოკალური მხარე
            DownloadProcLineId = procLineCounter.Count(EProcLineCase.Download),
            LocalPath = databaseBackupsLocalPath, //ლოკალური ფოლდერი ბაზების ბექაპების შესანახად
            LocalSmartSchemaName =
                smartSchemaNameCounter.Count(ESmartSchemaCase.Local, mainSmartSchemaName, reduceSmartSchemaName),
            //დაკუმშვა
            ArchiverName = archiverName,
            CompressProcLineId = procLineCounter.Count(EProcLineCase.Archive),
            //ატვირთვა რეზერვაციისათვის
            UploadFileStorageName = uploadFileStorageName, //გარე ფაილსაცავის სახელი
            UploadProcLineId = procLineCounter.Count(EProcLineCase.Upload),
            UploadSmartSchemaName = smartSchemaNameCounter.Count(ESmartSchemaCase.UploadServerSide, mainSmartSchemaName,
                reduceSmartSchemaName),

            //JobStep
            ProcLineId = procLineCounter.Count(EProcLineCase.Backup),
            DelayMinutesBeforeStep = 0,
            DelayMinutesAfterStep = 0,
            HoleStartTime = new TimeSpan(0, 0, 0),
            HoleEndTime = new TimeSpan(23, 59, 59),
            Enabled = true,
            PeriodType = backupType == EBackupType.Full ? EPeriodType.Day : EPeriodType.Hour,
            FreqInterval = 1,
            StartAt = DateTime.Today
        };

        parameters.DatabaseBackupSteps.Add(backupStepName, databaseBackupStep);
        CreateScheduleByJobStep(backupStepName, scheduleNameFirst, parameters);
        CreateScheduleByJobStep(backupStepName, scheduleNameSecond, parameters);
    }

    private static string CreateJobScheduleHourly(ReplicatorParameters parameters)
    {
        List<KeyValuePair<string, JobSchedule>> jsdKvp = parameters.JobSchedules.Where(w => w.Value is
        {
            Enabled: true, ScheduleType: EScheduleType.Daily,
            DailyFrequencyType: EDailyFrequency.OccursManyTimes
        }).ToList();

        if (jsdKvp.Any())
        {
            return jsdKvp[0].Key;
        }

        var jobScheduleHourly = new JobSchedule
        {
            Enabled = true,
            ScheduleType = EScheduleType.Daily,
            FreqInterval = 1,
            DurationStartDate = DateTime.Today,
            DurationEndDate = DateTime.MaxValue,
            DailyFrequencyType = EDailyFrequency.OccursManyTimes,
            ActiveStartDayTime = new TimeSpan(0, 30, 0),
            FreqSubDayType = EEveryMeasure.Hour,
            FreqSubDayInterval = 1,
            ActiveEndDayTime = new TimeSpan(23, 59, 59)
        };

        string name = CreateNewName(["Hourly"], parameters.JobSchedules.Keys.ToList());
        parameters.JobSchedules.Add(name, jobScheduleHourly);
        return name;
    }

    private static string CreateJobScheduleDaily(ReplicatorParameters parameters)
    {
        List<KeyValuePair<string, JobSchedule>> jsdKvp = parameters.JobSchedules.Where(w => w.Value is
        {
            Enabled: true, ScheduleType: EScheduleType.Daily, DailyFrequencyType: EDailyFrequency.OccursOnce
        }).ToList();

        if (jsdKvp.Any())
        {
            return jsdKvp[0].Key;
        }

        var jobScheduleDaily = new JobSchedule
        {
            Enabled = true,
            ScheduleType = EScheduleType.Daily,
            FreqInterval = 1,
            DurationStartDate = DateTime.Today,
            DurationEndDate = DateTime.MaxValue,
            DailyFrequencyType = EDailyFrequency.OccursOnce,
            ActiveStartDayTime = new TimeSpan(4, 0, 0)
        };

        string name = CreateNewName(["Daily", "DailyAt4"], [.. parameters.JobSchedules.Keys]);
        parameters.JobSchedules.Add(name, jobScheduleDaily);

        return name;
    }

    private static string CreateJobScheduleAtStart(ReplicatorParameters parameters)
    {
        const string atStartName = "AtStart";
        List<KeyValuePair<string, JobSchedule>> jsdKvp = parameters.JobSchedules
            .Where(w => w.Value is { Enabled: true, ScheduleType: EScheduleType.AtStart }).ToList();

        if (jsdKvp.Count != 0)
        {
            return jsdKvp[0].Key;
        }

        var jobScheduleAtStart = new JobSchedule
        {
            Enabled = true,
            ScheduleType = EScheduleType.AtStart,
            DurationStartDate = DateTime.Today,
            DurationEndDate = DateTime.MaxValue
            //JobStepNames = new List<string>()
        };

        parameters.JobSchedules.Add(CreateNewName([atStartName], [.. parameters.JobSchedules.Keys]),
            jobScheduleAtStart);

        return atStartName;
    }

    private static string CreateNewName(string[] templates, List<string> reservedNames)
    {
        int ver = 1;
        while (true)
        {
            foreach (string temp in templates.Select(template => ver == 1 ? template : $"{template}{ver}"))
            {
                if (!reservedNames.Contains(temp))
                {
                    return temp;
                }
            }

            ver++;
        }
    }
}
