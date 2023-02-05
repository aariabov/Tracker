namespace Tracker.Db.UnitOfWorks;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
