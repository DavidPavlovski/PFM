namespace PFM.DataTransfer.Analitycs
{
    public class AnalyticsListDto
    {
        public List<AnalyticsDto> Groups { get; set; }

        public AnalyticsListDto()
        {
            Groups = new();
        }
    }
}
