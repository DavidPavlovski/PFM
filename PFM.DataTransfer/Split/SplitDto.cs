using System.ComponentModel.DataAnnotations;

namespace PFM.DataTransfer.Split
{
    public class SplitDto
    {
        [Required]
        public string CatCode { get; set; }
        [Required]
        
        public double Ammount { get; set; }
    }
}
