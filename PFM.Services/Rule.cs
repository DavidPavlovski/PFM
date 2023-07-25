namespace PFM.Services
{
    public class Rule
    {
        public string CatCode { get; set; }
        public string ParentCode { get; set; }
        public string Name { get; set; }
        public List<string> Keywords { get; set; }
        public List<int> Mcc { get; set; }
    }
}
