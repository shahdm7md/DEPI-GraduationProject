using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DEPI_Project.Models;
public class OrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public Order Order { get; set; }
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    public Product Product { get; set; }
}
