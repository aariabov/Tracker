using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tracker.Db.Transactions;

public class ContextTransaction : IContextTransaction
{
    private readonly IDbContextTransaction _transaction;
    
    public ContextTransaction(AppDbContext db, IsolationLevel isolationLevel)
    {
        _transaction = db.Database.BeginTransaction(isolationLevel);
    }

    public async Task CommitAsync()
    {
        await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }
}