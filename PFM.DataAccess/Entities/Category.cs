namespace PFM.DataAccess.Entities
{
    public class Category
    {
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string Name { get; set; }
        public List<Transaction> Transactions { get; set; }

        public void Update(Category category)
        {
            ParentCode = category.ParentCode;
            Name = category.Name;
        }
    }
}
