namespace SpectreKickBox.Models
{
    public partial class Newsletter
    {
        public int NewsletterID { get; set; }
        public string NewsTitle { get; set; } = null!;
        public string NewsContent { get; set; } = null!;
        public int PostedByAccountID { get; set; }
        public bool IsActive { get; set; }
        public int PostYear { get; set; }
        public int PostWeek { get; set; }
        public string NewsType { get; set; } = null!;
        public virtual Account PostedByAccount { get; set; } = null!;
    }
}
