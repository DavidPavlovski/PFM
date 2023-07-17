using CsvHelper.Configuration;
using PFM.DataAccess.Entities;

namespace PFM.Mapping.CSVMapping
{
    public class TransactionCSVMap : ClassMap<Transaction>
    {
        public TransactionCSVMap() : base()
        {
            Map(x => x.Id).Index(0);
            Map(x => x.BeneficiaryName).Index(1);
            Map(x => x.Date).Index(2);
            Map(x => x.Direction).Index(3);
            Map(x => x.Ammount).Index(4);
            Map(x => x.Description).Index(5);
            Map(x => x.Currency).Index(6);
            Map(x => x.Mcc).Index(7);
            Map(x => x.Kind).Index(8);
        }
    }
}
