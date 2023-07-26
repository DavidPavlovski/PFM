using PFM.DataAccess.Entities;
using PFM.Helpers.CSVParser;

namespace PFM.Mapping.CSVMapping
{
    public class CategoryCSVMap : CustomClassMap<Category>
    {
        public CategoryCSVMap() : base()
        {
            Map(x => x.Code).Name("code").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    Errors.Add($"Category code is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'code'");
                }
                return true;
            }).Default(string.Empty, useOnConversionFailure: true);

            Map(x => x.ParentCode).Name("parent-code");

            Map(x => x.Name).Name("name").Validate(field =>
            {

                if (string.IsNullOrEmpty(field.Field))
                {
                    Errors.Add($"Name is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'name'");
                }

                return true;
            }).Default(string.Empty, useOnConversionFailure: true);
        }
    }
}
