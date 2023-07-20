using CsvHelper;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PFM.Exceptions;
using System.Globalization;
using System.Text;

namespace PFM.Helpers.CSVParser
{
    public class CSVParser : ICSVParser
    {
        readonly IConfiguration _configuration;

        public CSVParser(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public CSVReponse<TEntity> ParseCSV<TEntity, TMap>(IFormFile file) where TMap : CustomClassMap<TEntity>
        {
            var rowErrorsCount = _configuration.GetValue<int>("variables:CSVErrorRows");
            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var map = csv.Context.RegisterClassMap<TMap>();

            var items = new List<TEntity>();
            var rowErrors = new List<CSVRowError>();
            while (csv.Read())
            {
                try
                {
                    map.Errors.Clear();
                    var item = csv.GetRecord<TEntity>();
                    if (map.Errors.Any())
                    {
                        var rowError = new CSVRowError
                        {
                            Row = csv.Context.Parser.RawRow,
                            Errors = map.Errors.Distinct().Select(x => x).ToList()
                        };
                        rowErrors.Add(rowError);
                        if (rowErrors.Count == rowErrorsCount)
                        {
                            break;
                        }
                        continue;
                    }
                    items.Add(item);
                }
                catch (CsvHelperException csvEx)
                {
                    var rowError = new CSVRowError
                    {
                        Row = csv.Context.Parser.RawRow,
                        Errors = map.Errors.Distinct().Select(x => x).ToList()
                    };
                    rowErrors.Add(rowError);
                }
            }
            return new CSVReponse<TEntity>
            {
                Items = items,
                Errors = rowErrors
            };
        }
    }
}