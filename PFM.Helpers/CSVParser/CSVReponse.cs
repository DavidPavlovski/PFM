using PFM.Exceptions;

namespace PFM.Helpers.CSVParser
{
    public class CSVReponse<T>
    {
        public List<T> Items { get; set; }
        public List<CSVRowError> Errors { get; set; }
    }
}
