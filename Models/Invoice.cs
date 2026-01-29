using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpectreKickBox.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceID { get; set; }

        [Required]
        public int AccountID { get; set; }

        [Required]
        public int MembershipPlanID { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; } = DateTime.Now; // Default to current date

        // Navigation properties (optional, for EF relationships)
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }

        [ForeignKey("MembershipPlanID")]
        public virtual MembershipPlan MembershipPlan { get; set; }
    }
}
