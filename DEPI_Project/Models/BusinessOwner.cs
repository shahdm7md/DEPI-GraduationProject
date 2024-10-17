using Microsoft.AspNetCore.Identity;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DEPI_Project.Models;
public class BusinessOwner : IdentityUser
{

    public string BusinessName { get; set; }
    public string BusinessDescription { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<Follow> Follows { get; set; }

}
