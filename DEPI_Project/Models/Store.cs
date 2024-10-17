using Microsoft.AspNetCore.Identity;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DEPI_Project.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class Store
{
    public int Id { get; set; }
    public string StoreName { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey("BusinessOwner")]
    public string BusinessOwnerId { get; set; }
    public BusinessOwner BusinessOwner { get; set; }

    // Navigation properties
    public virtual ICollection<Product> Products { get; set; }
}
