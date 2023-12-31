﻿using CsvHelper;
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
        private const int DEFAULT_ROW_ERRORS = 15;
        public CSVParser(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public CSVReponse<TEntity> ParseCSV<TEntity, TMap>(IFormFile file) where TMap : CustomClassMap<TEntity>
        {
            var rowErrorsCount = _configuration.GetValue<int?>("variables:CSVErrorRows") ?? DEFAULT_ROW_ERRORS;

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var map = csv.Context.RegisterClassMap<TMap>();

            var items = new List<TEntity>();
            var errors = new List<CSVRowError>();

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
                        errors.Add(rowError);
                        if (errors.Count == rowErrorsCount)
                        {
                            break;
                        }
                        continue;
                    }
                    items.Add(item);
                }
                catch (CsvHelperException csvEx)
                {
                    return default;
                }
            }
            var csvResponse = new CSVReponse<TEntity>
            {
                Items = items,
                Errors = errors
            };
            return csvResponse;
        }
    }
}