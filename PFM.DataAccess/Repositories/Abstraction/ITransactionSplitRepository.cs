using PFM.DataAccess.Entities;

namespace PFM.DataAccess.Repositories.Abstraction
{
    public interface ITransactionSplitRepository
    {
        void AddRange(List<TransactionSplit> entities);
        void DeleteRange(List<TransactionSplit> entities);
    }
}
