using System.Data;

namespace Tracker.Db.Transactions;

public class TransactionManager : ITransactionManager
{
    private readonly AppDbContext _db;

    public TransactionManager(AppDbContext db)
    {
        _db = db;
    }

    public IContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        return new ContextTransaction(_db, isolationLevel);
    }
}
