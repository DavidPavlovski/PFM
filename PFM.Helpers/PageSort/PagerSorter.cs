using PFM.Enums;

namespace PFM.Helpers.PageSort
{
    public class PagerSorter
    {
        public TransactionKind? TransactionKind { get; set; } = null;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "date";
        public SortDirection SortOrder { get; set; } = SortDirection.Asc;
    }
}
