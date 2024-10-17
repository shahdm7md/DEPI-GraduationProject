using Microsoft.AspNetCore.Identity;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DEPI_Project.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class CartItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }

    [ForeignKey("Cart")]
    public int CartId { get; set; }
    public Cart Cart { get; set; }
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    public Product Product { get; set; }
}
