namespace PFM.Exceptions
{
    public class CSVRowError
    {
        public int Row { get; set; }
        public List<string> Errors { get; set; }
        public CSVRowError()
        {
            Errors = new List<string>();
        }
    }
}
