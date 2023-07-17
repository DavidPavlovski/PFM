namespace PFM.DataAccess.Entities
{
    public class TransactionSplit
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CatCode { get; set; }
        public double Ammount { get; set; }
        public Transaction Transaction { get; set; }
    }
}
