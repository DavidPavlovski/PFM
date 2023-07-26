namespace PFM.Exceptions
{
    public class FileErrorResponse : ErrorResponse
    {
        public string CsvHeaders { get; set; }

        public FileErrorResponse() : base()
        {
            Description = "Error occured while processing your request";
        }
    }
}
