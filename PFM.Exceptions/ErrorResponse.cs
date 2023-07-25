namespace PFM.Exceptions
{
    public class ErrorResponse
    {
        public string Description { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public ErrorResponse()
        {
            Description = "Error occured while processing your request";
        }
    }
}
