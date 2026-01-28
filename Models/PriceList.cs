namespace SpectreKickBox.Models
{
    public partial class PriceList
    {
        public int PriceListID { get; set; }
        public string Label { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
