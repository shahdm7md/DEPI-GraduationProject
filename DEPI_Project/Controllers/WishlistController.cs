using DEPI_Project.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DEPI_Project.Controllers
{
	public class WishlistController : Controller
	{
		private readonly ApplicationDbContext _context;
		public WishlistController(ApplicationDbContext context)
		{
			_context = context;

		}
		[Authorize]
		public IActionResult Index()
		{

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" }); // Return null if user is not authenticated
            }

            var list = _context.Wishlist
                .Where(x => x.UserId == userId).Include(x => x.User)
                .Include(ci => ci.Product)
                .ToList();

            if (list == null)
            {
                return RedirectToAction("Home", "Index");
            }

            return View(list);
           
            


        }
		[HttpPost]
		[Authorize]
		public async Task<IActionResult> AddWishes([FromBody] int productId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
			{
				return Json(new { success = false, message = "Please log in first." });
			}

			var existingItem = _context.Wishlist.FirstOrDefault(x => x.ProductId == productId && x.UserId == userId);
			if (existingItem != null)
			{
				return Json(new { success = false, message = "Product already in wishlist." });
			}

			var wishlistItem = new Wishlist
			{
				UserId = userId,
				ProductId = productId,
				CreatedAt = DateTime.Now
			};
			_context.Wishlist.Add(wishlistItem);
			await _context.SaveChangesAsync();

			return Json(new { success = true });
		}

		[HttpPost]
		[Authorize]
		public IActionResult RemoveItem([FromBody] int productId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var wishlistItem = _context.Wishlist.FirstOrDefault(item => item.ProductId == productId && item.UserId == userId);

			if (wishlistItem != null)
			{
				_context.Wishlist.Remove(wishlistItem);
				_context.SaveChanges();
				return Json(new { success = true });
			}

			return Json(new { success = false, message = "Item not found in wishlist." });
		}

	}
}
