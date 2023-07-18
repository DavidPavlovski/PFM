using PFM.DataAccess.Entities;
using PFM.Helpers.PageSort;

namespace PFM.DataAccess.Repositories.Abstraction
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetByIdAsync(string id);
        Task<PagedSortedList<Transaction>> GetTransactionsAsync(PagerSorter pagerSorter);
        void ImportTransactions(List<Transaction> entities);
        Task<bool> ExistsAsync(string id);
        void Update(Transaction entity);
    }
}
