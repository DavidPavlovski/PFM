using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
