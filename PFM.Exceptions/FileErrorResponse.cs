namespace PFM.Exceptions
{
    public class FileErrorResponse
    {
        public string Description { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string CsvHeaders { get; set; }

        public FileErrorResponse()
        {
            Description = "Error occured while processing your request";
        }
    }
}
