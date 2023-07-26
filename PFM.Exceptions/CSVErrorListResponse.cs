namespace PFM.Exceptions
{
    public class CSVErrorListResponse : ErrorResponse
    {
        public List<CSVRowError> Errors { get; set; }

        public CSVErrorListResponse() : base()
        {

        }
    }
}
