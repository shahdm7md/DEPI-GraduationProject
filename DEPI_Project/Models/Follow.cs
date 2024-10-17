using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DEPI_Project.Models;
public class Follow
{
    public int Id { get; set; }
   
    
    public DateTime CreatedAt { get; set; }

    [ForeignKey("Follower")]
    public string FollowerId { get; set; }
    public ApplicationUser Follower { get; set; }
    [ForeignKey("Following")]
    public string FollowingId { get; set; }
    public BusinessOwner Following { get; set; }
}
