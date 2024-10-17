using Microsoft.AspNetCore.Identity;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DEPI_Project.Models;
public class ApplicationUser : IdentityUser
{
    public string UserType { get; set; }
    public string? ProfilePicture { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    
    public virtual ICollection<Follow> Follows { get; set; }
    public virtual ICollection<Order> Orders { get; set; }
    public virtual ICollection<Cart> Carts { get; set; }
    public virtual ICollection<Wishlist> Wishlists { get; set; }
}
