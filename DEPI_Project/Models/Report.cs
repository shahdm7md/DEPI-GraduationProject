using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string ItemType { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }

        [ForeignKey("ReportedBy")]
        public string ReportedById { get; set; }
        public ApplicationUser ReportedBy { get; set; }
        [ForeignKey("Product")]
        public int ReportedItemId { get; set; }
        public Product Product { get; set; }
    }

}
