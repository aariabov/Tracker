using Tracker.Instructions.Common.Progress;

namespace Tracker.Common.Progress;

public class ProgressRm
{
    public ClientSocketRm SocketInfo { get; set; }
    public int TaskId { get; set; }
}

public class ProgressRm<T> : ProgressRm
{
    public T Pars { get; set; }
}
