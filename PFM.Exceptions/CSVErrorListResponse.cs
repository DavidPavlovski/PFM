using System.Net;

namespace PFM.Exceptions
{
    public class CSVErrorListResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public List<CSVRowError> Errors { get; set; }
    }
}
