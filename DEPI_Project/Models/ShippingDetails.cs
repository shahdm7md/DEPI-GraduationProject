using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models
{
    public class ShippingDetails
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }

}
