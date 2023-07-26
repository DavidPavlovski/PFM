using CsvHelper.Configuration;

namespace PFM.Helpers.CSVParser
{
    public class CustomClassMap<T> : ClassMap<T>
    {
        public List<string> Errors { get; set; }
        public CustomClassMap()
        {
            Errors = new List<string>();
        }
    }
}
