using AppCliTools.CliMenu;
using ParametersManagement.LibParameters;
using ReplicatorShared.Data.Models;

namespace ReplicatorConsole.MenuCommands;

public sealed class SelectScheduleNamesCommand : CliMenuCommand
{
    private readonly IParametersManager _parametersManager;
    private readonly string _scheduleName;
    private readonly bool _selected;
    private readonly string _stepName;

    // ReSharper disable once ConvertToPrimaryConstructor
    public SelectScheduleNamesCommand(IParametersManager parametersManager, string stepName, string scheduleName,
        bool selected) : base($"{(selected ? "√" : "×")} {scheduleName}", EMenuAction.Reload, EMenuAction.Reload, null,
        false, EStatusView.Table, true)
    {
        _parametersManager = parametersManager;
        _stepName = stepName;
        _scheduleName = scheduleName;
        _selected = selected;
    }

    protected override async ValueTask<bool> RunBody(CancellationToken cancellationToken = default)
    {
        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        if (_selected)
        {
            //წავშალოთ
            JobStepBySchedule? jobStepBySchedule =
                parameters.JobsBySchedules.SingleOrDefault(s =>
                    s.JobStepName == _stepName && s.ScheduleName == _scheduleName);
            if (jobStepBySchedule != null)
            {
                parameters.JobsBySchedules.Remove(jobStepBySchedule);
            }
        }
        else
        {
            //ჩავამატოთ
            JobStepBySchedule? jobStepBySchedule =
                parameters.JobsBySchedules.SingleOrDefault(s =>
                    s.JobStepName == _stepName && s.ScheduleName == _scheduleName);
            if (jobStepBySchedule == null)
            {
                var newJobStepBySchedule = new JobStepBySchedule(_stepName, _scheduleName,
                    parameters.JobsBySchedules.Where(w => w.ScheduleName == _scheduleName).DefaultIfEmpty()
                        .Max(m => m?.SequentialNumber ?? 0) + 1);
                parameters.JobsBySchedules.Add(newJobStepBySchedule);
            }
        }

        ReNumSequences();
        await _parametersManager.Save(parameters, "Schedule Updated", null, cancellationToken);
        return true;
    }

    private void ReNumSequences()
    {
        var parameters = (ReplicatorParameters)_parametersManager.Parameters;

        int sn = 1;
        foreach (JobStepBySchedule jobStepBySchedule in parameters.JobsBySchedules
                     .Where(w => w.ScheduleName == _scheduleName).OrderBy(o => o.SequentialNumber)
                     .ThenBy(tb => tb.JobStepName))
        {
            jobStepBySchedule.SequentialNumber = sn;
            sn++;
        }
    }
}
