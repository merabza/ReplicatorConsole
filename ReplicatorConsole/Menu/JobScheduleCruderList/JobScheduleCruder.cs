using AppCliTools.CliMenu;
using AppCliTools.CliParameters;
using AppCliTools.CliParameters.FieldEditors;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorConsole.FieldEditors;
using ReplicatorConsole.MenuCommands;
using ReplicatorShared.Data.Models;
using SystemTools.BackgroundTasks;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.Menu.JobScheduleCruderList;

public sealed class JobScheduleCruder : ParCruder<JobSchedule>
{
    private readonly string _appName;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private readonly string? _parametersFileName;
    private readonly IProcesses _processes;

    public JobScheduleCruder(string appName, ILogger logger, IHttpClientFactory httpClientFactory,
        IParametersManager parametersManager, Dictionary<string, JobSchedule> currentValuesDictionary,
        IProcesses processes) : base(parametersManager, currentValuesDictionary, "Job Schedule", "Job Schedules")
    {
        _parametersFileName = parametersManager.ParametersFileName;
        _appName = appName;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _processes = processes;

        FieldEditors.Add(new EnumFieldEditor<EScheduleType>(nameof(JobSchedule.ScheduleType), EScheduleType.Daily));
        FieldEditors.Add(new DateTimeFieldEditor(nameof(JobSchedule.RunOnceDateTime), DateTime.Today.AddDays(1)));
        FieldEditors.Add(new IntFieldEditor(nameof(JobSchedule.FreqInterval), 1));
        FieldEditors.Add(new EnumFieldEditor<EDailyFrequency>(nameof(JobSchedule.DailyFrequencyType),
            EDailyFrequency.OccursOnce));
        FieldEditors.Add(new EnumFieldEditor<EEveryMeasure>(nameof(JobSchedule.FreqSubDayType), EEveryMeasure.Hour));
        FieldEditors.Add(new IntFieldEditor(nameof(JobSchedule.FreqSubDayInterval), 1));
        FieldEditors.Add(new TimeSpanFieldEditor(nameof(JobSchedule.ActiveStartDayTime), new TimeSpan(0, 0, 0)));
        FieldEditors.Add(new TimeSpanFieldEditor(nameof(JobSchedule.ActiveEndDayTime), new TimeSpan(23, 59, 59)));
        FieldEditors.Add(new DateFieldEditor(nameof(JobSchedule.DurationStartDate), DateTime.Today));
        FieldEditors.Add(new DateFieldEditor(nameof(JobSchedule.DurationEndDate), DateTime.MaxValue.Date));
    }

    protected override void CheckFieldsEnables(ItemData itemData, string? lastEditedFieldName = null)
    {
        var jobSchedule = (JobSchedule)itemData;

        bool enableDuration = jobSchedule.ScheduleType != EScheduleType.Once;
        EnableFieldByName(nameof(JobSchedule.DurationStartDate), enableDuration);
        EnableFieldByName(nameof(JobSchedule.DurationEndDate), enableDuration);

        bool enableOnce = jobSchedule.ScheduleType == EScheduleType.Once;
        EnableFieldByName(nameof(JobSchedule.RunOnceDateTime), enableOnce);

        bool enableDaily = jobSchedule.ScheduleType == EScheduleType.Daily;
        EnableFieldByName(nameof(JobSchedule.FreqInterval), enableDaily);
        EnableFieldByName(nameof(JobSchedule.DailyFrequencyType), enableDaily);
        EnableFieldByName(nameof(JobSchedule.ActiveStartDayTime), enableDaily);

        bool enableDailyOccursManyTimes =
            enableDaily && jobSchedule.DailyFrequencyType == EDailyFrequency.OccursManyTimes;
        EnableFieldByName(nameof(JobSchedule.FreqSubDayType), enableDailyOccursManyTimes);
        EnableFieldByName(nameof(JobSchedule.FreqSubDayInterval), enableDailyOccursManyTimes);
        EnableFieldByName(nameof(JobSchedule.ActiveEndDayTime), enableDailyOccursManyTimes);
    }

    //public საჭიროა Replicator პროექტისათვის
    public override void FillDetailsSubMenu(CliMenuSet itemSubMenuSet, string itemName)
    {
        base.FillDetailsSubMenu(itemSubMenuSet, itemName);

        var runAllStepsNowCommand = new RunAllStepsNowCommand(_appName, _logger, _httpClientFactory, _processes,
            ParametersManager, itemName, _parametersFileName);
        itemSubMenuSet.AddMenuItem(runAllStepsNowCommand);
    }
}
