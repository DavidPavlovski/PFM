using PFM.DataAccess.Entities;
using PFM.Helpers.Analytics;
using PFM.Helpers.PageSort;

namespace PFM.DataAccess.Repositories.Abstraction
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetTransactionsAnalyticsAsync(AnalyticsQuery analyticsQuery);
        Task<PagedSortedList<Transaction>> GetTransactionsAsync(PagerSorter pagerSorter);
        Task ImportTransactions(List<Transaction> entities, int batchSize);
        Task UpdateRange(List<Transaction> entities);
        Task<Transaction> GetByIdAsync(string id);
        IEnumerable<Transaction> GetAll();
        Task<bool> ExistsAsync(string id);
        void Update(Transaction entity);
    }
}
