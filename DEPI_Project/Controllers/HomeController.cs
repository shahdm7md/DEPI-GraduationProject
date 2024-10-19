using DEPI_Project.Data;
using DEPI_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEPI_Project.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _context;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
		{
			_logger = logger;
			_context = context;
		}

		// Search Action ·›· —… «·„‰ Ã«  Õ”» «·›∆… Ê«·»ÕÀ
		public async Task<IActionResult> Search(int? categoryId, string query)
		{
			// ›· —… «·„‰ Ã«  »‰«¡ ⁄·Ï «·›∆… Ê«·ﬂ·„… «·„› «ÕÌ…
			var products = _context.Products.AsQueryable();

			if (categoryId.HasValue)
			{
				products = products.Where(p => p.CategoryId == categoryId);
			}

			if (!string.IsNullOrWhiteSpace(query))
			{
				products = products.Where(p => p.Name.Contains(query));
			}

			var result = await products.Include(p => p.Category).ToListAsync();

			// ≈⁄«œ… «·‰ «∆Ã ≈·Ï View «·»ÕÀ
			ViewBag.Categories = await _context.Categories.ToListAsync();
			return View("Index", result);  // ⁄—÷ «·‰ «∆Ã ›Ì ‰›” View «·‹ Index
		}

		// Index Action ·⁄—÷ «·’›Õ… «·—∆Ì”Ì… Ê«·„‰ Ã« 
		public async Task<IActionResult> Index()
		{
			var categories = await _context.Categories.ToListAsync(); // Ã·» «·›∆« 
			ViewBag.Categories = categories; //  „—Ì— «·›∆«  ≈·Ï «·‹ View

			var products = await _context.Products.Include(p => p.Category).ToListAsync(); // Ã·» «·„‰ Ã« 
			return View(products);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
