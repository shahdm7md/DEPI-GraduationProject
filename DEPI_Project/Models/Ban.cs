using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models
{
    public class Ban
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string Duration { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [ForeignKey("BannedBy")]
        public string BannedById { get; set; }
        public ApplicationUser BannedBy { get; set; }
    }

}
