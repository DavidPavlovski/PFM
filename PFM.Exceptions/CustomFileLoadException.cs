namespace PFM.Exceptions
{
    public class CustomFileLoadException : Exception
    {
        public string CsvHeaders { get; set; }
        public CustomFileLoadException(string message) : base(message)
        {

        }
    }
}
