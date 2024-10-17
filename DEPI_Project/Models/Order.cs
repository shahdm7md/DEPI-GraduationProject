using Microsoft.AspNetCore.Identity;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DEPI_Project.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class Order
{
    public int Id { get; set; }
    public double TotalPrice { get; set; }
    public string Status { get; set; }
    public string ShippingAddress { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey("ApplicationUser")]
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    // Navigation properties
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}
