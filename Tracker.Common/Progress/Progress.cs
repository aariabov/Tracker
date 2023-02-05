using Microsoft.AspNetCore.SignalR;

namespace Tracker.Common.Progress;

public class Progress
{
    private readonly IHubContext<ProgressHub> _hub;

    public Progress(IHubContext<ProgressHub> hub)
    {
        _hub = hub;
    }

    public void NotifyClient(int processed, int total, ClientSocketRm socket, int frequency, int taskId)
    {
        var percentage = (processed * 100) / total;
        if (percentage % frequency == 0)
        {
            SendToClient(processed, total, socket, taskId).ConfigureAwait(false);
        }
    }

    private async Task SendToClient(int processed, int total, ClientSocketRm socket, int taskId)
    {
        var progressInfo = new ProgressInfo(processed, total, taskId);
        await _hub.Clients.Client(socket.ConnectionId).SendAsync(socket.MethodName, progressInfo);
    }

    private class ProgressInfo
    {
        public int Processed { get; }
        public int Total { get; }
        public int TaskId { get; }

        public ProgressInfo(int processed, int total, int taskId)
        {
            Processed = processed;
            Total = total;
            TaskId = taskId;
        }
    }
}
