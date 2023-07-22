using PFM.Enums;

namespace PFM.Helpers.PageSort
{
    public class PagedSortedList<T>
    {
        public int? Page { get; set; } = 0;
        public int? PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
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
        }
    }
}
