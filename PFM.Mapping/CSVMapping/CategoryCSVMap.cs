using CsvHelper.Configuration;
using PFM.DataAccess.Entities;

namespace PFM.Mapping.CSVMapping
{
    public class CategoryCSVMap : ClassMap<Category>
    {
        public CategoryCSVMap() : base()
        {
            Map(x => x.Code).Name("code").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    throw new ArgumentException($"Category code is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'code'");
                }
                return true;
            });

            Map(x => x.ParentCode).Name("parent-code");

            Map(x => x.Name).Name("name").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    if (string.IsNullOrEmpty(field.Field))
                    {
                        throw new ArgumentException($"Name is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'name'");
                    }
                }
                return true;
            });
        }
    }
}
