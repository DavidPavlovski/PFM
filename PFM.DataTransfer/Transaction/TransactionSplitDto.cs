using PFM.DataTransfer.Split;
using System.ComponentModel.DataAnnotations;

namespace PFM.DataTransfer.Transaction
{
    public class TransactionSplitDto
    {
        [Required]
        public List<SplitDto> Splits { get; set; }
    }
}
