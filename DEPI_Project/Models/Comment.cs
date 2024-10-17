using Microsoft.AspNetCore.Identity;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DEPI_Project.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class Comment
{
    public int Id { get; set; }
    public string CommentText { get; set; }
    public DateTime CreatedAt { get; set; }
    [ForeignKey("User")]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    public Product Product { get; set; }
}
