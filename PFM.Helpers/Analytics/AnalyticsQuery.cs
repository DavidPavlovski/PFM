using PFM.Enums;

namespace PFM.Helpers.Analytics
{
    public class AnalyticsQuery
    {
        public string? CatCode { get; set; } = null;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public Direction? Direction { get; set; } = null;
    }
}
