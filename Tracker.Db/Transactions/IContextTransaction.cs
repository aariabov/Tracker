namespace Tracker.Db.Transactions;

public interface IContextTransaction : IDisposable
{
    Task CommitAsync();
    Task RollbackAsync();

}