namespace PFM.Helpers.PageSort
{
    public class PagedSortedList<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string SortBy { get; set; }
        public SortDirection SortOrder { get; set; }
        public List<T> Items { get; set; }
        public PagedSortedList()
        {

        }
        public PagedSortedList(PagerSorter pagerSorter, List<T> items, int totalCount)
        {
            Page = pagerSorter.Page;
            PageSize = pagerSorter.PageSize;
            Items = items;
            TotalCount = totalCount;
            SortOrder = pagerSorter.SortOrder;
            SortBy = pagerSorter.SortBy;
            TotalPages = (int)Math.Ceiling((double)TotalCount / PageSize);
        }
    }
}
