using System.Net;

namespace PFM.Exceptions
{
    public class CustomException : Exception
    {
        public string? Description { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<CSVRowError> Errors { get; set; }

        public CustomException(string message) : base(message)
        {

        }
    }
}