using PFM.DataAccess.Entities;
using PFM.Helpers.Analytics;
using PFM.Helpers.PageSort;

namespace PFM.DataAccess.Repositories.Abstraction
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetAll();
        Task<Transaction> GetByIdAsync(string id);
        Task<PagedSortedList<Transaction>> GetTransactionsAsync(PagerSorter pagerSorter);
        Task ImportTransactions(List<Transaction> entities , int batchSize);
        Task<bool> ExistsAsync(string id);
        void Update(Transaction entity);
        Task<List<Transaction>> GetTransactionsAnalyticsAsync(AnalyticsQuery analyticsQuery);
        Task UpdateRange(List<Transaction> entities);
    }
}
