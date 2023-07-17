using CsvHelper.Configuration;
using PFM.DataAccess.Entities;

namespace PFM.Mapping.CSVMapping
{
    public class CategoryCSVMap : ClassMap<Category>
    {
        public CategoryCSVMap() : base()
        {
            Map(x => x.Code).Index(0);
            Map(x => x.ParentCode).Index(1);
            Map(x => x.Name).Index(2);
        }
    }
}
