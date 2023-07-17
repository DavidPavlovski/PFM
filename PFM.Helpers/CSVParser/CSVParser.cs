using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;

namespace PFM.Helpers.CSVParser
{
    public static class CSVParser
    {
        public static List<T> ParseCSV<T, TMap>(IFormFile file )  where TMap : ClassMap<T>
        {
            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TMap>();
            var records = csv.GetRecords<T>();
            var entities = new List<T>();
            foreach (var record in records)
            {
                entities.Add(record);
            }
            return entities;
        }
    }
}
