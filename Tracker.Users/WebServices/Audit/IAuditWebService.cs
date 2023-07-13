namespace Tracker.Users;

public interface IAuditWebService
{
    Task<int> CreateLogAsync(LogModel body);

    Task DeleteLogAsync(DeleteLogModel body);
}
