using PFM.Enums;

namespace PFM.Helpers.PageSort
{
    public class PagedSortedList<T>
    {
        public TransactionKind? TransactionKind { get; set; } = null;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public int? Page { get; set; } = 0;
        public int? PageSize { get; set; } = 10;
        public string SortBy { get; set; } = string.Empty;
        public SortDirection SortOrder { get; set; } = SortDirection.Asc;
        public int TotalCount { get; set; }
        public List<T> Items { get; set; }
        public PagedSortedList()
        {

        }
        public PagedSortedList(PagerSorter pagerSorter, List<T> items, int totalCount)
        {
            TransactionKind = pagerSorter.TransactionKind;
            StartDate = pagerSorter.StartDate;
            EndDate = pagerSorter.EndDate;
            Page = pagerSorter.Page;
            PageSize = pagerSorter.PageSize;
            SortBy = pagerSorter.SortBy;
            SortOrder = pagerSorter.SortOrder;
            Items = items;
            TotalCount = totalCount;
        }
    }
}
