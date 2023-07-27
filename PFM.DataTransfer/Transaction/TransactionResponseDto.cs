using PFM.Enums;

namespace PFM.DataTransfer.Transaction
{
    public class TransactionResponseDto
    {
        public string Id { get; set; }
        public string BeneficiaryName { get; set; }
        public DateTime Date { get; set; }
        public Direction Direction { get; set; }
        public double Ammount { get; set; }
        public string Description { get; set; }
        public string Curency { get; set; }
        public int? Mcc { get; set; }
        public TransactionKind Kind { get; set; }
        public string? CatCode { get; set; }
        public List<TransactionSplitDto> TransactionSplits { get; set; }
    }
}
