using PFM.DataAccess.Repositories.Abstraction;
using PFM.DataTransfer.Analitycs;
using PFM.Helpers.Analytics;
using PFM.Services.Abstraction;

namespace PFM.Services.Implementation
{
    public class AnalyticsService : IAnalyticsService
    {
        readonly ITransactionRepository _transactionRepository;

        public AnalyticsService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<AnalyticsListDto> GetTransactionAnalyticsAsync(AnalyticsQuery analyticsQuery)
        {
            var transactions = await _transactionRepository.GetTransactionsAnalyticsAsync(analyticsQuery);
            var groups = transactions.GroupBy(a => a.CatCode);
            var analyticsList = new AnalyticsListDto();
            foreach (var group in groups)
            {
                var analytic = new AnalyticsDto
                {
                    CatCode = group.Key,
                    Ammount = Math.Round(group.Sum(x => x.Ammount), 2),
                    Count = group.Count()
                };
                analyticsList.Groups.Add(analytic);
            }
            return analyticsList;
        }
    }
}
