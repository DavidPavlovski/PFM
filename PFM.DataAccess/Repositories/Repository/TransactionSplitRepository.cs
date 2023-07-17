using PFM.DataAccess.DbContextOption;
using PFM.DataAccess.Entities;
using PFM.DataAccess.Repositories.Abstraction;

namespace PFM.DataAccess.Repositories.Repository
{
    public class TransactionSplitRepository : ITransactionSplitRepository
    {
        readonly ApplicationDbContext _dbContext;
        public TransactionSplitRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void AddRange(List<TransactionSplit> entities)
        {
            _dbContext.TransactionsSplits.AddRange(entities);
        }

        public void DeleteRange(List<TransactionSplit> entities)
        {
            _dbContext.TransactionsSplits.RemoveRange(entities);
        }
    }
}
