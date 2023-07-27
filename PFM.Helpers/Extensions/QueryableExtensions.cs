using PFM.Helpers.PageSort;

namespace PFM.Helpers.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, PagerSorter pagerSorter)
        {
            return query.Skip(pagerSorter.Page * pagerSorter.PageSize).Take(pagerSorter.PageSize);
        }
    }

}
