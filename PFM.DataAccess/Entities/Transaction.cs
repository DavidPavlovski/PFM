using CsvHelper.Configuration.Attributes;
using PFM.Enums;
using System.ComponentModel;

namespace PFM.DataAccess.Entities
{
    public class Transaction
    {
        public string Id { get; set; }
        public string BeneficiaryName { get; set; }
        public DateTime Date { get; set; }
        public Direction Direction { get; set; }
        public double Ammount { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public int? Mcc { get; set; }
        public TransactionKind Kind { get; set; }
        public string? CatCode { get; set; }
        public Category Category { get; set; }
       
    }
}
