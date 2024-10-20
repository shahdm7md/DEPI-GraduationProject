using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEPI_Project.Data;
using Microsoft.AspNetCore.Identity;

namespace DEPI_Project.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Method to get the user's cart or create a new one
        private Cart GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return null; // Return null if user is not authenticated
            }

            // Attempt to find the user's cart
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.ApplicationUserId == userId);

            if (cart == null)
            {
                // Create a new cart if one doesn't exist
                cart = new Cart
                {
                    ApplicationUserId = userId,
                    CartItems = new List<CartItem>(),
                    CreatedAt = DateTime.UtcNow,
                    TotalPrice = 0.0
                };

                // Add the new cart to the database
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            return cart;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)  // Include the user who posted the comment
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Handle posting a comment
        [HttpPost]
        public async Task<IActionResult> PostComment(int productId, string commentText)
        {
            // Ensure the comment text is not empty
            if (string.IsNullOrWhiteSpace(commentText))
            {
                ModelState.AddModelError("", "Comment cannot be empty.");
                return RedirectToAction("Details", new { id = productId });
            }

            // Get the logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not authenticated
            }

            // Find the product by ID
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound(); // Return 404 if the product doesn't exist
            }

            // Create a new comment
            var comment = new Comment
            {
                CommentText = commentText,
                CreatedAt = DateTime.Now,
                UserId = userId,
                ProductId = productId
            };

            // Add the comment to the database
            _context.comments.Add(comment);
            await _context.SaveChangesAsync();
            

            // Redirect back to the product details page to see the new comment
            return RedirectToAction("Details", new { id = productId });
        }
        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            // Get the product by id
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            // Get the user's cart or create a new one if the user is authenticated
            var cart = GetCart();

            // Check if the cart is null (not authenticated)
            if (cart == null)
            {
                // Redirect to login if user is not authenticated
                return Redirect("https://localhost:44345/Identity/Account/Login");
            }

            // Check if the product is already in the cart
            var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == product.Id);
            if (cartItem == null)
            {
                // Add new cart item if it's not already in the cart
                cartItem = new CartItem
                {
                    ProductId = product.Id,
                    Quantity = 1,
                    Price = product.Price,
                    CartId = cart.Id
                };
                cart.CartItems.Add(cartItem);
            }
            else
            {
                // If the product is already in the cart, increase the quantity
                cartItem.Quantity++;
            }

            // Update the total price of the cart
            cart.TotalPrice = cart.CartItems.Sum(i => i.Quantity * i.Price);

            // Save changes to the database
            _context.SaveChanges();

            // Redirect to the product details page
            return RedirectToAction("Details", new { id = product.Id });
        }


    }
}



