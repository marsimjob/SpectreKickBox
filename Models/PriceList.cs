using System.ComponentModel.DataAnnotations;

namespace SpectreKickBox.Models
{
    public partial class PriceList
    {
        [Key]
        public int PriceID { get; set; } // Changed from PriceListID to PriceID
        public string Label { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
