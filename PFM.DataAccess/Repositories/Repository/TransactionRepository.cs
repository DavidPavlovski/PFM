using Microsoft.EntityFrameworkCore;
using PFM.DataAccess.DbContextOption;
using PFM.DataAccess.Entities;
using PFM.DataAccess.Repositories.Abstraction;
using PFM.Helpers.Analytics;
using PFM.Helpers.Extensions;
using PFM.Helpers.PageSort;
using System.Linq;

namespace PFM.DataAccess.Repositories.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        readonly ApplicationDbContext _dbContext;

        public TransactionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _dbContext.Transactions.AnyAsync(x => x.Id == id);
        }

        public IEnumerable<Transaction> GetAll()
        {
            return _dbContext.Transactions;
        }

        public async Task<Transaction> GetByIdAsync(string id)
        {
            return await _dbContext
                            .Transactions
                            .Include(x => x.TransactionSplits)
                            .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Transaction>> GetTransactionsAnalyticsAsync(AnalyticsQuery analyticsQuery)
        {
            var query = _dbContext.Transactions.Where(x => x.CatCode != null).AsQueryable();
            if (analyticsQuery.CatCode is not null)
            {
                query = query.Where(x => analyticsQuery.CatCode.Contains(x.CatCode));
            }
            if (analyticsQuery.Direction is not null)
            {
                query = query.Where(x => x.Direction == analyticsQuery.Direction);
            }
            if (analyticsQuery.StartDate is not null)
            {
                query = query.Where(x => x.Date >= analyticsQuery.StartDate);
            }
            if (analyticsQuery.EndDate is not null)
            {
                query = query.Where(x => x.Date <= analyticsQuery.EndDate);
            }
            return await query.ToListAsync();
        }

        public async Task<PagedSortedList<Transaction>> GetTransactionsAsync(PagerSorter pagerSorter)
        {
            var query = _dbContext
                            .Transactions
                            .Include(x => x.Category)
                            .Include(x => x.TransactionSplits)
                            .AsQueryable();

            if (pagerSorter.TransactionKind is not null)
            {
                query = query.Where(x => x.Kind == pagerSorter.TransactionKind);
            }

            if (pagerSorter.StartDate is not null)
            {
                query = query.Where(x => x.Date >= pagerSorter.StartDate);
            }

            if (pagerSorter.EndDate is not null)
            {
                query = query.Where(x => x.Date <= pagerSorter.EndDate);
            }

            if (!string.IsNullOrEmpty(pagerSorter.SortBy))
            {
                query = pagerSorter.SortBy switch
                {
                    "beneficiaryName" => pagerSorter.SortOrder == SortDirection.Asc ? query.OrderBy(x => x.BeneficiaryName) : query.OrderByDescending(x => x.BeneficiaryName),
                    "date" => pagerSorter.SortOrder == SortDirection.Asc ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date),
                    "direction" => pagerSorter.SortOrder == SortDirection.Asc ? query.OrderBy(x => x.Direction) : query.OrderByDescending(x => x.Direction),
                    "ammount" => pagerSorter.SortOrder == SortDirection.Asc ? query.OrderBy(x => x.Ammount) : query.OrderByDescending(x => x.Ammount),
                    "description" => pagerSorter.SortOrder == SortDirection.Asc ? query.OrderBy(x => x.Description) : query.OrderByDescending(x => x.Description),
                    "currency" => pagerSorter.SortOrder == SortDirection.Asc ? query.OrderBy(x => x.Currency) : query.OrderByDescending(x => x.Currency),
                    "kind" => pagerSorter.SortOrder == SortDirection.Asc ? query.OrderBy(x => x.Kind) : query.OrderByDescending(x => x.Kind),
                    "catCode" => pagerSorter.SortOrder == SortDirection.Asc ? query.OrderBy(x => x.CatCode) : query.OrderByDescending(x => x.CatCode),
                    _ => pagerSorter.SortOrder == SortDirection.Asc ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date),
                };
            }
            else
            {
                query = query.OrderBy(x => x.Date);
            }
            var totalCount = query.Count();
            var items = await query.ApplyPaging(pagerSorter).ToListAsync();
            return new(pagerSorter, items, totalCount);
        }

        public void ImportTransactions(List<Transaction> entities)
        {
            foreach (var entity in entities)
            {
                if (_dbContext.Transactions.Contains(entity))
                {
                    _dbContext.Transactions.Update(entity);
                }
                else
                {
                    _dbContext.Transactions.Add(entity);
                }
            }
        }

        public void Update(Transaction entity)
        {
            _dbContext.Transactions.Update(entity);
        }

        public void UpdateRange(List<Transaction> entities)
        {
            _dbContext.Transactions.UpdateRange(entities);
        }
    }
}
