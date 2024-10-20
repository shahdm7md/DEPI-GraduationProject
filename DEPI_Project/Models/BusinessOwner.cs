using Microsoft.AspNetCore.Identity;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DEPI_Project.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
public class BusinessOwner 
{
    
    public string Id { get; set; }
    public string BusinessName { get; set; }
    public string BusinessDescription { get; set; }
    public DateTime CreatedAt { get; set; }
    [ForeignKey("User")]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
	public virtual ICollection<Product> Products { get; set; } 
	public virtual ICollection<Follow> Follows { get; set; }

}
