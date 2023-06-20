namespace Tracker.Analytics;

public static class Helpers
{
    public static string GetString(this ExecStatus status)
    {
        switch (status)
        {
            case ExecStatus.InWork:
                return "В работе";
            case ExecStatus.InWorkOverdue:
                return "В работе просрочено";
            case ExecStatus.Completed:
                return "Выполнено в срок";
            case ExecStatus.CompletedOverdue:
                return "Выполнено с нарушением срока";
            default:
                throw new Exception("Unknown ExecStatus");
        }
    }
}
