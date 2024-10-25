using DEPI_Project.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;

namespace DEPI_Project.Controllers
{
	public class ShowOrdersController : Controller
	{
		private readonly ApplicationDbContext _context;
		public ShowOrdersController(ApplicationDbContext context)
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
			var orders = _context.Orders.Where(ord=>ord.ApplicationUserId==userId).Include(user=>user.ApplicationUser).Include(item=>item.OrderItems).ToList();
			if (orders == null)
			{
				return RedirectToAction("Home", "Index");
			}

			return View(orders);
		}
        public IActionResult ItemesOrder(int id)
		{
			var items = _context.OrderItems.Where(x=>x.OrderId == id).Include(pro=>pro.Product).ToList();
			if (items == null)
			{
				var order = _context.Orders.Where(ord => ord.Id == id).FirstOrDefault();
				_context.Orders.Remove(order);

                return RedirectToAction( "Index");
			}
			return View(items);
		}

    }
}
