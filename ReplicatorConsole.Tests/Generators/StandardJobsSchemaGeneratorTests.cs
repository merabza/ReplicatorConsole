using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DatabaseTools.DbTools;
using DatabaseTools.DbTools.Models;
using Microsoft.Extensions.Logging;
using Moq;
using ParametersManagement.LibDatabaseParameters;
using ParametersManagement.LibFileParameters.Models;
using ParametersManagement.LibParameters;
using ReplicatorConsole.Generators;
using ReplicatorShared.Data.Models;
using ReplicatorShared.Data.Steps;
using SystemTools.SystemToolsShared;
using Xunit;

namespace ReplicatorConsole.Tests.Generators;

public sealed class StandardJobsSchemaGeneratorTests
{
    private const string ConnectionName = "Conn";
    private const string FoldersSetName = "HDD";
    private const string StepNamePrefix = "Srv";
    private const string DateMask = "_yyyy_MM_dd_HHmmss_fffffff";
    private const string DailySchedule = "Daily";
    private const string HourlySchedule = "Hourly";
    private const string AtStartSchedule = "AtStart";
    private const string BackupStorageName = "Bak";
    private const string BackupFolder = @"E:\BAK";
    private const string UploadStorageName = "Up";
    private const string LocalPath = @"C:\Local\Backups";
    private const string ServerBackupDirectory = @"F:\SqlBackup";

    private static readonly string[] DailyTemplates = ["Daily", "DailyAt4"];

    private readonly Mock<ILogger> _logger = new();
    private readonly ReplicatorParameters _parameters = new();

    // ---------- CreateNewName ----------

    [Fact]
    public void CreateNewName_WhenFirstTemplateIsFree_ReturnsIt()
    {
        // Arrange
        List<string> reservedNames = [];

        // Act
        string result = StandardJobsSchemaGenerator.CreateNewName(DailyTemplates, reservedNames);

        // Assert
        Assert.Equal("Daily", result);
    }

    [Fact]
    public void CreateNewName_WhenFirstTemplateIsTaken_ReturnsNextTemplate()
    {
        // Arrange
        List<string> reservedNames = ["Daily"];

        // Act
        string result = StandardJobsSchemaGenerator.CreateNewName(DailyTemplates, reservedNames);

        // Assert
        Assert.Equal("DailyAt4", result);
    }

    [Fact]
    public void CreateNewName_WhenAllTemplatesAreTaken_AddsVersionNumber()
    {
        // Arrange
        List<string> reservedNames = ["Daily", "DailyAt4", "Daily2"];

        // Act
        string result = StandardJobsSchemaGenerator.CreateNewName(DailyTemplates, reservedNames);

        // Assert
        Assert.Equal("DailyAt42", result);
    }

    // ---------- Schedules ----------

    [Fact]
    public void CreateJobScheduleDaily_WhenNoDailySchedule_AddsScheduleAtFourOClock()
    {
        // Act
        string result = StandardJobsSchemaGenerator.CreateJobScheduleDaily(_parameters);

        // Assert
        Assert.Equal(DailySchedule, result);
        JobSchedule schedule = _parameters.JobSchedules[DailySchedule];
        Assert.True(schedule.Enabled);
        Assert.Equal(EScheduleType.Daily, schedule.ScheduleType);
        Assert.Equal(1, schedule.FreqInterval);
        Assert.Equal(DateTime.Today, schedule.DurationStartDate);
        Assert.Equal(DateTime.MaxValue, schedule.DurationEndDate);
        Assert.Equal(EDailyFrequency.OccursOnce, schedule.DailyFrequencyType);
        Assert.Equal(new TimeSpan(4, 0, 0), schedule.ActiveStartDayTime);
    }

    [Fact]
    public void CreateJobScheduleDaily_WhenEnabledDailyScheduleExists_ReturnsItsName()
    {
        // Arrange
        _parameters.JobSchedules.Add("Night", DailyOnceSchedule(true));

        // Act
        string result = StandardJobsSchemaGenerator.CreateJobScheduleDaily(_parameters);

        // Assert
        Assert.Equal("Night", result);
        Assert.Single(_parameters.JobSchedules);
    }

    [Fact]
    public void CreateJobScheduleDaily_WhenOnlyDisabledOrOtherSchedulesExist_AddsNewScheduleWithFreeName()
    {
        // Arrange
        _parameters.JobSchedules.Add("Night", DailyOnceSchedule(false));
        _parameters.JobSchedules.Add(DailySchedule,
            new JobSchedule { Enabled = true, ScheduleType = EScheduleType.AtStart });
        _parameters.JobSchedules.Add("Often",
            new JobSchedule
            {
                Enabled = true,
                ScheduleType = EScheduleType.Daily,
                DailyFrequencyType = EDailyFrequency.OccursManyTimes
            });

        // Act
        string result = StandardJobsSchemaGenerator.CreateJobScheduleDaily(_parameters);

        // Assert
        Assert.Equal("DailyAt4", result);
        Assert.Equal(4, _parameters.JobSchedules.Count);
    }

    [Fact]
    public void CreateJobScheduleHourly_WhenNoHourlySchedule_AddsScheduleEveryHour()
    {
        // Act
        string result = StandardJobsSchemaGenerator.CreateJobScheduleHourly(_parameters);

        // Assert
        Assert.Equal(HourlySchedule, result);
        JobSchedule schedule = _parameters.JobSchedules[HourlySchedule];
        Assert.True(schedule.Enabled);
        Assert.Equal(EScheduleType.Daily, schedule.ScheduleType);
        Assert.Equal(1, schedule.FreqInterval);
        Assert.Equal(DateTime.Today, schedule.DurationStartDate);
        Assert.Equal(DateTime.MaxValue, schedule.DurationEndDate);
        Assert.Equal(EDailyFrequency.OccursManyTimes, schedule.DailyFrequencyType);
        Assert.Equal(new TimeSpan(0, 30, 0), schedule.ActiveStartDayTime);
        Assert.Equal(EEveryMeasure.Hour, schedule.FreqSubDayType);
        Assert.Equal(1, schedule.FreqSubDayInterval);
        Assert.Equal(new TimeSpan(23, 59, 59), schedule.ActiveEndDayTime);
    }

    [Fact]
    public void CreateJobScheduleHourly_WhenEnabledRepeatingScheduleExists_ReturnsItsName()
    {
        // Arrange
        _parameters.JobSchedules.Add("EveryHour",
            new JobSchedule
            {
                Enabled = true,
                ScheduleType = EScheduleType.Daily,
                DailyFrequencyType = EDailyFrequency.OccursManyTimes
            });

        // Act
        string result = StandardJobsSchemaGenerator.CreateJobScheduleHourly(_parameters);

        // Assert
        Assert.Equal("EveryHour", result);
        Assert.Single(_parameters.JobSchedules);
    }

    [Fact]
    public void CreateJobScheduleHourly_WhenRepeatingScheduleIsDisabled_AddsNewSchedule()
    {
        // Arrange
        _parameters.JobSchedules.Add("EveryHour",
            new JobSchedule
            {
                Enabled = false,
                ScheduleType = EScheduleType.Daily,
                DailyFrequencyType = EDailyFrequency.OccursManyTimes
            });

        // Act
        string result = StandardJobsSchemaGenerator.CreateJobScheduleHourly(_parameters);

        // Assert
        Assert.Equal(HourlySchedule, result);
        Assert.Equal(2, _parameters.JobSchedules.Count);
    }

    [Fact]
    public void CreateJobScheduleAtStart_WhenNoAtStartSchedule_AddsIt()
    {
        // Act
        string result = StandardJobsSchemaGenerator.CreateJobScheduleAtStart(_parameters);

        // Assert
        Assert.Equal(AtStartSchedule, result);
        JobSchedule schedule = _parameters.JobSchedules[AtStartSchedule];
        Assert.True(schedule.Enabled);
        Assert.Equal(EScheduleType.AtStart, schedule.ScheduleType);
        Assert.Equal(DateTime.Today, schedule.DurationStartDate);
        Assert.Equal(DateTime.MaxValue, schedule.DurationEndDate);
    }

    [Fact]
    public void CreateJobScheduleAtStart_WhenEnabledAtStartScheduleExists_ReturnsItsName()
    {
        // Arrange
        _parameters.JobSchedules.Add("OnBoot",
            new JobSchedule { Enabled = true, ScheduleType = EScheduleType.AtStart });

        // Act
        string result = StandardJobsSchemaGenerator.CreateJobScheduleAtStart(_parameters);

        // Assert
        Assert.Equal("OnBoot", result);
        Assert.Single(_parameters.JobSchedules);
    }

    [Fact]
    public void CreateJobScheduleAtStart_WhenAtStartNameIsTakenByDisabledSchedule_ReturnsNameOfCreatedSchedule()
    {
        // Arrange
        _parameters.JobSchedules.Add(AtStartSchedule,
            new JobSchedule { Enabled = false, ScheduleType = EScheduleType.AtStart });

        // Act
        string result = StandardJobsSchemaGenerator.CreateJobScheduleAtStart(_parameters);

        // Assert
        Assert.Equal("AtStart2", result);
        JobSchedule createdSchedule = _parameters.JobSchedules[result];
        Assert.True(createdSchedule.Enabled);
        Assert.Equal(EScheduleType.AtStart, createdSchedule.ScheduleType);
    }

    [Fact]
    public void CreateJobScheduleAtStart_WhenAtStartScheduleIsDisabled_AddsNewSchedule()
    {
        // Arrange
        _parameters.JobSchedules.Add("OnBoot",
            new JobSchedule { Enabled = false, ScheduleType = EScheduleType.AtStart });

        // Act
        string result = StandardJobsSchemaGenerator.CreateJobScheduleAtStart(_parameters);

        // Assert
        Assert.Equal(AtStartSchedule, result);
        Assert.True(_parameters.JobSchedules[AtStartSchedule].Enabled);
    }

    // ---------- Step to schedule binding ----------

    [Fact]
    public void CreateScheduleByJobStep_WhenBindingIsNew_AddsItAfterLastStepOfSchedule()
    {
        // Arrange
        _parameters.JobsBySchedules.Add(new JobStepBySchedule("A", DailySchedule, 1));
        _parameters.JobsBySchedules.Add(new JobStepBySchedule("B", DailySchedule, 3));
        _parameters.JobsBySchedules.Add(new JobStepBySchedule("C", AtStartSchedule, 10));

        // Act
        StandardJobsSchemaGenerator.CreateScheduleByJobStep("D", DailySchedule, _parameters);

        // Assert
        JobStepBySchedule binding = _parameters.JobsBySchedules.Single(s => s.JobStepName == "D");
        Assert.Equal(DailySchedule, binding.ScheduleName);
        Assert.Equal(4, binding.SequentialNumber);
    }

    [Fact]
    public void CreateScheduleByJobStep_WhenScheduleHasNoSteps_StartsNumberingFromOne()
    {
        // Arrange
        _parameters.JobsBySchedules.Add(new JobStepBySchedule("A", AtStartSchedule, 5));

        // Act
        StandardJobsSchemaGenerator.CreateScheduleByJobStep("A", DailySchedule, _parameters);

        // Assert
        JobStepBySchedule binding = _parameters.JobsBySchedules.Single(s => s.ScheduleName == DailySchedule);
        Assert.Equal("A", binding.JobStepName);
        Assert.Equal(1, binding.SequentialNumber);
    }

    [Fact]
    public void CreateScheduleByJobStep_WhenBindingExists_DoesNotDuplicateIt()
    {
        // Arrange
        _parameters.JobsBySchedules.Add(new JobStepBySchedule("A", DailySchedule, 1));

        // Act
        StandardJobsSchemaGenerator.CreateScheduleByJobStep("A", DailySchedule, _parameters);

        // Assert
        Assert.Single(_parameters.JobsBySchedules);
    }

    // ---------- Maintenance steps ----------

    [Fact]
    public void CreateMaintenanceStep_WhenStepIsNew_AddsDailyStepForAllDatabases()
    {
        // Arrange
        StandardJobsSchemaGenerator sut = CreateSut(ConnectionName);

        // Act
        sut.CreateMaintenanceStep(EMultiDatabaseActionType.UpdateStatistics, StepNamePrefix, DailySchedule,
            AtStartSchedule, _parameters);

        // Assert
        const string stepName = "Srv - UpdateStatistics for all databases";
        MultiDatabaseProcessStep step = _parameters.MultiDatabaseProcessSteps[stepName];
        Assert.Equal(EMultiDatabaseActionType.UpdateStatistics, step.ActionType);
        Assert.Equal(ConnectionName, step.DatabaseServerConnectionName);
        Assert.Equal(EDatabaseSet.AllDatabases, step.DatabaseSet);
        AssertDailyJobStepDefaults(step, EPeriodType.Day);
        AssertBoundToSchedules(stepName);
    }

    [Fact]
    public void CreateMaintenanceStep_WhenStepExists_KeepsItUnchanged()
    {
        // Arrange
        const string stepName = "Srv - RecompileProcedures for all databases";
        var existingStep = new MultiDatabaseProcessStep { ProcLineId = 7 };
        _parameters.MultiDatabaseProcessSteps.Add(stepName, existingStep);
        StandardJobsSchemaGenerator sut = CreateSut(ConnectionName);

        // Act
        sut.CreateMaintenanceStep(EMultiDatabaseActionType.RecompileProcedures, StepNamePrefix, DailySchedule,
            AtStartSchedule, _parameters);

        // Assert
        Assert.Same(existingStep, _parameters.MultiDatabaseProcessSteps[stepName]);
        Assert.Empty(_parameters.JobsBySchedules);
    }

    // ---------- Backup steps ----------

    [Fact]
    public void CreateBackupStep_ForFullBackup_CreatesStepPointingToFoldersSetAndFileStorage()
    {
        // Arrange
        AddFileStorages();
        StandardJobsSchemaGenerator sut = CreateSut(string.Empty);

        // Act
        sut.CreateBackupStep(EBackupType.Full, true, true, StepNamePrefix, DateMask, "DailyStandard", "Reduce",
            FoldersSetName, BackupStorageName, LocalPath, null, UploadStorageName, DailySchedule, AtStartSchedule,
            _parameters);

        // Assert
        const string stepName = "Srv Full Backup";
        DatabaseBackupStep step = _parameters.DatabaseBackupSteps[stepName];
        Assert.Equal(string.Empty, step.DatabaseServerConnectionName);
        Assert.Equal(EDatabaseSet.AllDatabases, step.DatabaseSet);
        DatabaseParameters backupParameters = step.DatabaseBackupParameters!;
        Assert.Equal(EBackupType.Full, backupParameters.BackupType);
        Assert.True(backupParameters.Compress);
        Assert.True(backupParameters.Verify);
        Assert.Equal("Srv_", backupParameters.BackupNamePrefix);
        Assert.Equal("_Backup_Full", backupParameters.BackupNameMiddlePart);
        Assert.Equal(DateMask, backupParameters.DateMask);
        Assert.Equal("bak", backupParameters.BackupFileExtension);
        Assert.Equal("Reduce", step.SmartSchemaName);
        Assert.Equal(BackupStorageName, step.FileStorageName);
        Assert.Equal(FoldersSetName, step.DbServerFoldersSetName);
        Assert.Equal(1, step.DownloadProcLineId);
        Assert.Equal(LocalPath, step.LocalPath);
        Assert.Equal("Reduce", step.LocalSmartSchemaName);
        Assert.Null(step.ArchiverName);
        Assert.Equal(1, step.CompressProcLineId);
        Assert.Equal(UploadStorageName, step.UploadFileStorageName);
        Assert.Equal(4, step.UploadProcLineId);
        Assert.Equal("DailyStandard", step.UploadSmartSchemaName);
        AssertDailyJobStepDefaults(step, EPeriodType.Day);
        AssertBoundToSchedules(stepName);
    }

    [Fact]
    public void CreateBackupStep_ForTransactionLogBackup_CreatesHourlyStep()
    {
        // Arrange
        AddFileStorages();
        StandardJobsSchemaGenerator sut = CreateSut(string.Empty);

        // Act
        sut.CreateBackupStep(EBackupType.TrLog, false, false, StepNamePrefix, DateMask, HourlySchedule, HourlySchedule,
            FoldersSetName, BackupStorageName, LocalPath, "ZipClass", UploadStorageName, HourlySchedule,
            AtStartSchedule, _parameters);

        // Assert
        DatabaseBackupStep step = _parameters.DatabaseBackupSteps["Srv TrLog Backup"];
        DatabaseParameters backupParameters = step.DatabaseBackupParameters!;
        Assert.Equal(EBackupType.TrLog, backupParameters.BackupType);
        Assert.False(backupParameters.Compress);
        Assert.False(backupParameters.Verify);
        Assert.Equal("_Backup_TrLog", backupParameters.BackupNameMiddlePart);
        Assert.Equal("trn", backupParameters.BackupFileExtension);
        Assert.Equal(HourlySchedule, step.SmartSchemaName);
        Assert.Equal(HourlySchedule, step.LocalSmartSchemaName);
        Assert.Equal(HourlySchedule, step.UploadSmartSchemaName);
        Assert.Equal("ZipClass", step.ArchiverName);
        Assert.Equal(FoldersSetName, step.DbServerFoldersSetName);
        Assert.Equal(EPeriodType.Hour, step.PeriodType);
    }

    [Fact]
    public void CreateBackupStep_WhenStepExists_KeepsItUnchanged()
    {
        // Arrange
        var existingStep = new DatabaseBackupStep { FileStorageName = "Old" };
        _parameters.DatabaseBackupSteps.Add("Srv Full Backup", existingStep);
        StandardJobsSchemaGenerator sut = CreateSut(string.Empty);

        // Act
        sut.CreateBackupStep(EBackupType.Full, true, true, StepNamePrefix, DateMask, "DailyStandard", "Reduce",
            FoldersSetName, BackupStorageName, LocalPath, null, UploadStorageName, DailySchedule, AtStartSchedule,
            _parameters);

        // Assert
        Assert.Same(existingStep, _parameters.DatabaseBackupSteps["Srv Full Backup"]);
        Assert.Empty(_parameters.JobsBySchedules);
    }

    // ---------- File storage registration ----------

    [Fact]
    public void RegisterFileStorage_WhenStorageForFolderExists_ReusesIt()
    {
        // Arrange
        _parameters.FileStorages.Add("NoPath", new FileStorageData());
        _parameters.FileStorages.Add(BackupStorageName, new FileStorageData { FileStoragePath = BackupFolder });

        // Act
        string result = CreateSut(ConnectionName).RegisterFileStorage(@"e:\bak\");

        // Assert
        Assert.Equal(BackupStorageName, result);
        Assert.Equal(2, _parameters.FileStorages.Count);
    }

    [Fact]
    public void RegisterFileStorage_WhenOnlyRemoteStoragesExist_CreatesStorageNamedAfterFolder()
    {
        // Arrange
        _parameters.FileStorages.Add("Remote", new FileStorageData { FileStoragePath = "ftp://example.invalid/BAK" });

        // Act
        string result = CreateSut(ConnectionName).RegisterFileStorage(BackupFolder);

        // Assert
        Assert.Equal("BAK", result);
        Assert.Equal(BackupFolder, _parameters.FileStorages["BAK"].FileStoragePath);
    }

    [Fact]
    public void RegisterFileStorage_WhenFolderNameIsTakenByAnotherStorage_CreatesUniqueName()
    {
        // Arrange
        _parameters.FileStorages.Add("BAK", new FileStorageData { FileStoragePath = @"D:\Other\BAK" });

        // Act
        string result = CreateSut(ConnectionName).RegisterFileStorage(BackupFolder);

        // Assert
        Assert.Equal("BAK2", result);
        Assert.Equal(BackupFolder, _parameters.FileStorages["BAK2"].FileStoragePath);
        Assert.Equal(@"D:\Other\BAK", _parameters.FileStorages["BAK"].FileStoragePath);
    }

    // ---------- Database server folders set selection ----------

    [Fact]
    public void SelectDbServerFoldersSet_WhenConnectionHasNoFoldersSets_CreatesDefaultFromServerInfo()
    {
        // Arrange
        DatabaseServerConnectionData connection = AddConnection(null);

        // Act
        (string Name, string BackupFolder)? result = CreateSut(ConnectionName).SelectDbServerFoldersSet(ServerInfo());

        // Assert
        Assert.Equal(("Default", ServerBackupDirectory), result);
        DatabaseFoldersSet defaultSet = connection.DatabaseFoldersSets!["Default"];
        Assert.Equal(@"F:\Data", defaultSet.Data);
        Assert.Equal(@"F:\Log", defaultSet.DataLog);
    }

    [Fact]
    public void SelectDbServerFoldersSet_WhenFoldersSetsAreEmpty_CreatesDefaultFromServerInfo()
    {
        // Arrange
        AddConnection([]);

        // Act
        (string Name, string BackupFolder)? result = CreateSut(ConnectionName).SelectDbServerFoldersSet(ServerInfo());

        // Assert
        Assert.Equal(("Default", ServerBackupDirectory), result);
    }

    [Fact]
    public void SelectDbServerFoldersSet_WhenOnlyOneSetHasBackupFolder_ReturnsItWithoutAsking()
    {
        // Arrange
        DatabaseServerConnectionData connection = AddConnection(new Dictionary<string, DatabaseFoldersSet>
        {
            ["NoBackup"] = new() { Backup = " ", Data = @"D:\Data" },
            [FoldersSetName] = new() { Backup = BackupFolder }
        });

        // Act
        (string Name, string BackupFolder)? result = CreateSut(ConnectionName).SelectDbServerFoldersSet(ServerInfo());

        // Assert
        Assert.Equal((FoldersSetName, BackupFolder), result);
        Assert.Equal(2, connection.DatabaseFoldersSets!.Count);
    }

    [Fact]
    public void SelectDbServerFoldersSet_WhenNoSetHasBackupFolder_ReturnsNull()
    {
        // Arrange
        AddConnection(new Dictionary<string, DatabaseFoldersSet> { ["NoBackup"] = new() { Data = @"D:\Data" } });

        // Act
        (string Name, string BackupFolder)? result = CreateSut(ConnectionName).SelectDbServerFoldersSet(ServerInfo());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SelectDbServerFoldersSet_WhenSeveralSetsHaveBackupFolder_AsksUser()
    {
        // Arrange
        AddConnection(new Dictionary<string, DatabaseFoldersSet>
        {
            ["Default"] = new() { Backup = @"D:\Backups" }, [FoldersSetName] = new() { Backup = BackupFolder }
        });
        StandardJobsSchemaGenerator sut = CreateSut(ConnectionName);
        Exception? exception = null;

        // Act
        string output = CaptureConsoleOutput(() =>
            exception = Record.Exception(() => sut.SelectDbServerFoldersSet(ServerInfo())));

        // Assert
        //test host console is not interactive, so the menu input fails instead of waiting for a key
        Assert.True(exception is InvalidOperationException or IOException, exception?.ToString());
        Assert.Contains("Select Database server folders set", output, StringComparison.Ordinal);
    }

    // ---------- Generate ----------

    [Fact]
    public async Task Generate_WhenDatabaseManagerCannotBeCreated_StopsWithoutChangingParameters()
    {
        // Arrange
        _parameters.DatabaseServerConnections.Add(ConnectionName,
            new DatabaseServerConnectionData { DatabaseServerProvider = EDatabaseProvider.None });
        StandardJobsSchemaGenerator sut = CreateSut(ConnectionName);
        TextWriter originalOutput = Console.Out;
        await using var output = new StringWriter(CultureInfo.InvariantCulture);
        Console.SetOut(output);
        Exception? exception;

        // Act
        try
        {
            exception = await Record.ExceptionAsync(() => sut.Generate(CancellationToken.None).AsTask());
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        // Assert
        Assert.Null(exception);
        Assert.Contains("[ERROR] Database manager does not created. Generation process stopped", output.ToString(),
            StringComparison.Ordinal);
        Assert.Empty(_parameters.JobSchedules);
        Assert.Empty(_parameters.SmartSchemas);
        Assert.Empty(_parameters.DatabaseBackupSteps);
    }

    [Fact]
    public async Task Generate_WhenServerIsNotReachable_StopsWithoutChangingParameters()
    {
        // Arrange
        _parameters.DatabaseServerConnections.Add(ConnectionName,
            new DatabaseServerConnectionData
            {
                DatabaseServerProvider = EDatabaseProvider.SqlServer,
                ServerAddress = "localhost,1",
                WindowsNtIntegratedSecurity = true,
                TrustServerCertificate = true
            });

        StandardJobsSchemaGenerator sut = CreateSut(ConnectionName);
        TextWriter originalOutput = Console.Out;
        await using var output = new StringWriter(CultureInfo.InvariantCulture);
        Console.SetOut(output);

        // Act
        try
        {
            await sut.Generate(CancellationToken.None);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        // Assert
        Assert.Contains("[ERROR] Can not connect to server. Generation process stopped", output.ToString(),
            StringComparison.Ordinal);
        Assert.Empty(_parameters.JobSchedules);
        Assert.Empty(_parameters.SmartSchemas);
        Assert.Empty(_parameters.DatabaseBackupSteps);
        _logger.Verify(
            l => l.Log(LogLevel.Error, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString() == "Can not connect to server. Generation process stopped"),
                It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    // ---------- Helpers ----------

    private StandardJobsSchemaGenerator CreateSut(string databaseServerConnectionName)
    {
        return new StandardJobsSchemaGenerator("GeneratorTests", true, _logger.Object,
            new ParametersManager(null, _parameters), databaseServerConnectionName, null);
    }

    private static string CaptureConsoleOutput(Action action)
    {
        TextWriter originalOutput = Console.Out;
        using var output = new StringWriter(CultureInfo.InvariantCulture);
        Console.SetOut(output);
        try
        {
            action();
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        return output.ToString();
    }

    private static JobSchedule DailyOnceSchedule(bool enabled)
    {
        return new JobSchedule
        {
            Enabled = enabled, ScheduleType = EScheduleType.Daily, DailyFrequencyType = EDailyFrequency.OccursOnce
        };
    }

    private static DbServerInfo ServerInfo()
    {
        return new DbServerInfo("16.0", "MSSQLSERVER", ServerBackupDirectory, @"F:\Data", @"F:\Log", true,
            StepNamePrefix);
    }

    private DatabaseServerConnectionData AddConnection(Dictionary<string, DatabaseFoldersSet>? foldersSets)
    {
        var connection = new DatabaseServerConnectionData
        {
            DatabaseServerProvider = EDatabaseProvider.SqlServer, DatabaseFoldersSets = foldersSets
        };
        _parameters.DatabaseServerConnections.Add(ConnectionName, connection);
        return connection;
    }

    private void AddFileStorages()
    {
        _parameters.FileStorages.Add(BackupStorageName, new FileStorageData { FileStoragePath = BackupFolder });
        _parameters.FileStorages.Add(UploadStorageName,
            new FileStorageData { FileStoragePath = "ftp://example.invalid/backups" });
    }

    private static void AssertDailyJobStepDefaults(JobStep step, EPeriodType periodType)
    {
        Assert.Equal(1, step.ProcLineId);
        Assert.Equal(0, step.DelayMinutesBeforeStep);
        Assert.Equal(0, step.DelayMinutesAfterStep);
        Assert.Equal(TimeSpan.Zero, step.HoleStartTime);
        Assert.Equal(new TimeSpan(23, 59, 59), step.HoleEndTime);
        Assert.True(step.Enabled);
        Assert.Equal(periodType, step.PeriodType);
        Assert.Equal(1, step.FreqInterval);
        Assert.Equal(DateTime.Today, step.StartAt);
    }

    private void AssertBoundToSchedules(string stepName)
    {
        string[] expectedSchedules = [DailySchedule, AtStartSchedule];
        Assert.Equal(expectedSchedules,
            _parameters.JobsBySchedules.Where(w => w.JobStepName == stepName).Select(s => s.ScheduleName));
    }
}
