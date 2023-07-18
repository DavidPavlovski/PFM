using PFM.DataTransfer.Analitycs;
using PFM.Helpers.Analytics;

namespace PFM.Services.Abstraction
{
    public interface IAnalyticsService
    {
        Task<AnalyticsListDto> GetTransactionAnalyticsAsync(AnalyticsQuery analyticsQuery);
    }
}
