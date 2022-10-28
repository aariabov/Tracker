using Tracker.Common.Progress;

namespace Tracker.Web.Sandbox;

public class ProgressableTestService
{
    private readonly Progress _progress;

    public ProgressableTestService(Progress progress)
    {
        _progress = progress;
    }

    public async Task RunJob(ClientSocketRm socket, int taskId)
    {
        await Helpers.SomeJob(_progress, total: 100, socket, frequency: 5, taskId);
    }
}

public class ProgressableParamsTestService
{
    private readonly Progress _progress;

    public ProgressableParamsTestService(Progress progress)
    {
        _progress = progress;
    }

    public async Task RunJob(ClientSocketRm socket, TestJobParams model, int taskId)
    {
        await Helpers.SomeJob(_progress, total: 100, socket, frequency: 5, taskId);
    }
}

internal static class Helpers
{
    internal static async Task SomeJob(Progress progress, int total, ClientSocketRm socket, int frequency, int taskId)
    {
        for (var i = 0; i <= total; i++)
        {
            //SIMULATING SOME TASK
            Thread.Sleep(50);

            progress.NotifyClient(i, total, socket, frequency, taskId);
        }
    }
}

    