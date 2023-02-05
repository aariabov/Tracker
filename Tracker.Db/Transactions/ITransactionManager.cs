using System.Data;

namespace Tracker.Db.Transactions;

public interface ITransactionManager
{
    IContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
}
