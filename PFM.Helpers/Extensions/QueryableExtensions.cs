using PFM.Helpers.PageSort;

namespace PFM.Helpers.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, PagerSorter pagerSorter)
        {
            if (pagerSorter.Page <= 0)
                pagerSorter.Page = 0;

            if (pagerSorter.PageSize <= 0 || pagerSorter.PageSize > 10)
                pagerSorter.PageSize = 10;

            return query.Skip(pagerSorter.Page * pagerSorter.PageSize).Take(pagerSorter.PageSize);
        }
    }

}
