using AppCliTools.CliMenu;
using AppCliTools.CliParameters;
using AppCliTools.CliParameters.FieldEditors;
using Microsoft.Extensions.Logging;
using ParametersManagement.LibParameters;
using ReplicatorConsole.FieldEditors;
using ReplicatorConsole.MenuCommands;
using ReplicatorShared.Data.Models;
using ReplicatorShared.Data.Steps;
using SystemTools.BackgroundTasks;
using SystemTools.SystemToolsShared;

namespace ReplicatorConsole.StepCruders;

public /*open*/ class StepCruder<TStep> : ParCruder<TStep> where TStep : JobStep, new()
{
    private readonly string _appName;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private readonly string? _parametersFileName;
    private readonly IProcesses _processes;

    protected StepCruder(string appName, ILogger logger, IHttpClientFactory httpClientFactory, IProcesses processes,
        ParametersManager parametersManager, Dictionary<string, TStep> currentValuesDictionary, string crudName,
        string crudNamePlural) : base(parametersManager, currentValuesDictionary, crudName, crudNamePlural)
    {
        _parametersFileName = parametersManager.ParametersFileName;
        _appName = appName;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _processes = processes;
        //რიგითი ნომერი უნდა დგინდება არსებულ ნომრებში მაქსიმუმს 1-ით მეტი, ან არსებული ნაბიჯების რაოდენობაზე 1-ით მეტი. (მაქსიმუმი ამ 2 რიცხვს შორის)
        FieldEditors.Add(new IntFieldEditor(nameof(JobStep.ProcLineId), 1));
        FieldEditors.Add(new IntFieldEditor(nameof(JobStep.DelayMinutesBeforeStep)));
        FieldEditors.Add(new IntFieldEditor(nameof(JobStep.DelayMinutesAfterStep)));
        FieldEditors.Add(new TimeSpanFieldEditor(nameof(JobStep.HoleStartTime), new TimeSpan(0, 0, 0)));
        FieldEditors.Add(new TimeSpanFieldEditor(nameof(JobStep.HoleEndTime), new TimeSpan(23, 59, 59)));
        FieldEditors.Add(new EnumFieldEditor<EPeriodType>(nameof(JobStep.PeriodType), EPeriodType.Day));
        FieldEditors.Add(new IntFieldEditor(nameof(JobStep.FreqInterval), 1));
        FieldEditors.Add(new DateTimeFieldEditor(nameof(JobStep.StartAt), DateTime.Today));
        FieldEditors.Add(new BoolFieldEditor(nameof(JobStep.Enabled), true));
    }

    //public საჭიროა Replicator პროექტისათვის
    public override void FillDetailsSubMenu(CliMenuSet itemSubMenuSet, string itemName)
    {
        base.FillDetailsSubMenu(itemSubMenuSet, itemName);

        var jobStep = (JobStep?)GetItemByName(itemName);

        if (jobStep is null)
        {
            StShared.WriteErrorLine("jobStep does not found. step does not started", true, _logger);
            return;
        }

        var runThisStepNowCommand = new RunThisStepNowCommand(_appName, _logger, _httpClientFactory, _processes,
            ParametersManager, jobStep, _parametersFileName);
        itemSubMenuSet.AddMenuItem(runThisStepNowCommand);

        var parameters = (ReplicatorParameters)ParametersManager.Parameters;

        //if (parameters == null)
        //    return;

        List<string> scheduleNamesList = parameters.JobsBySchedules.Where(w => w.JobStepName == itemName)
            .Select(s => s.ScheduleName).ToList();
        foreach (KeyValuePair<string, JobSchedule> kvp in parameters.JobSchedules)
        {
            itemSubMenuSet.AddMenuItem(new SelectScheduleNamesCommand(ParametersManager, itemName, kvp.Key,
                scheduleNamesList.Contains(kvp.Key)));
        }
    }

    public override bool ContainsRecordWithKey(string recordKey)
    {
        var parameters = (ReplicatorParameters)ParametersManager.Parameters;
        Dictionary<string, JobStep> steps = parameters.GetSteps();
        return steps.ContainsKey(recordKey);
    }
}
