using DEPI_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEPI_Project.Data;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

public class CartController : Controller
{
	private readonly ApplicationDbContext _context;
	private readonly UserManager<ApplicationUser> _userManager;

	public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
	{
		_context = context;
		_userManager = userManager;
	}

	private Cart GetCart()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrEmpty(userId))
		{
			return null; // Return null if user is not authenticated
		}

		var cart = _context.Carts
			.Include(c => c.CartItems)
			.ThenInclude(ci => ci.Product)
			.FirstOrDefault(c => c.ApplicationUserId == userId);

		if (cart == null)
		{
			cart = new Cart { CartItems = new List<CartItem>(), ApplicationUserId = userId };
			_context.Carts.Add(cart);
			_context.SaveChanges();
		}

		return cart;
	}
	public IActionResult Index()
	{
		// Check if the user is authenticated
		if (!User.Identity.IsAuthenticated)
		{
            // Redirect to the login page if the user is not logged in
            return Redirect("https://localhost:44345/Identity/Account/Login");
        }

		// Get the cart for the authenticated user
		var cart = GetCart();

		// If the cart is null, create a new one
		if (cart == null)
		{
			cart = new Cart
			{
				ApplicationUserId = User.Identity.Name, // Assuming the cart is associated with a user
				CartItems = new List<CartItem>()
			};
			// Save the new cart (you might want to save it to your database here)
		}

		// Update the total price of the cart
		UpdateCartTotalPrice(cart);

		// Pass the cart to the layout or view
		ViewData["Cart"] = cart;

		return View(cart);
	}


	[HttpPost]
	public IActionResult IncreaseQuantity(int productId)
	{
		var cart = GetCart();
		if (cart == null) return RedirectToAction("Login", "Account"); // Check for user authentication

		var cartItem = cart.CartItems.FirstOrDefault(item => item.Product.Id == productId);
		if (cartItem != null)
		{
			cartItem.Quantity++;
			cartItem.Price = cartItem.Quantity * cartItem.Product.Price; // Update price based on new quantity
		}

		_context.SaveChanges(); // Save changes to the database
		return RedirectToAction("Index");
	}

	[HttpPost]
	public IActionResult DecreaseQuantity(int productId)
	{
		var cart = GetCart();
		if (cart == null) return RedirectToAction("Login", "Account");

		var cartItem = cart.CartItems.FirstOrDefault(item => item.Product.Id == productId);
		if (cartItem != null && cartItem.Quantity > 1)
		{
			cartItem.Quantity--;
			cartItem.Price = cartItem.Quantity * cartItem.Product.Price; // Update price based on new quantity
		}

		_context.SaveChanges(); // Save changes to the database
		return RedirectToAction("Index");
	}

	[HttpPost]
	public IActionResult RemoveItem(int productId)
	{
		var cart = GetCart();
		if (cart == null) return RedirectToAction("Login", "Account");

		var cartItem = cart.CartItems.FirstOrDefault(item => item.Product.Id == productId);
		if (cartItem != null)
		{
			cart.CartItems.Remove(cartItem); // Remove item from cart
		}

		_context.SaveChanges(); // Save changes to the database
		return RedirectToAction("Index");
	}

	[HttpGet]
	public IActionResult Checkout()
	{
		var cart = GetCart();
		if (cart == null || !cart.CartItems.Any())
		{
			return RedirectToAction("Index"); // Redirect to cart if empty or user is not authenticated
		}

		UpdateCartTotalPrice(cart); // Update total price before checkout
		return View(cart);
	}

	[HttpPost]
	public async Task<IActionResult> Checkout(string firstName, string lastName, string email, string address, string city, string country, string zipCode, string telephone, string paymentMethod)
	{
		var cart = GetCart();
		if (cart == null || !cart.CartItems.Any())
		{
			return RedirectToAction("Index"); // Redirect to cart if empty or user is not authenticated
		}

		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrEmpty(userId))
		{
			return RedirectToAction("Login", "Account");
		}

		// Create a new order
		var order = new Order
		{
			TotalPrice = cart.TotalPrice,
			Status = "Pending",
			ShippingAddress = address,
			PaymentMethod = paymentMethod,
			CreatedAt = DateTime.Now,
			ApplicationUserId = userId
		};

		_context.Orders.Add(order);
		await _context.SaveChangesAsync();

		foreach (var cartItem in cart.CartItems)
		{
			var orderItem = new OrderItem
			{
				Quantity = cartItem.Quantity,
				Price = cartItem.Product.Price,
				ProductId = cartItem.Product.Id,
				OrderId = order.Id
			};

			_context.OrderItems.Add(orderItem);
		}

		var shippingDetails = new ShippingDetails
		{
			Address = address,
			City = city,
			PostalCode = zipCode,
			Country = country,
			CreatedAt = DateTime.Now,
			OrderId = order.Id
		};

		_context.ShippingDetails.Add(shippingDetails);
		await _context.SaveChangesAsync();

		// Clear the cart after successful checkout
		 if (cart != null)
    {
        cart.CartItems.Clear(); // Remove all items in the cart
        _context.Carts.Remove(cart); // Remove the cart itself
        await _context.SaveChangesAsync(); // Save changes to the database
    }

		return RedirectToAction("OrderConfirmation");
	}

	public IActionResult OrderConfirmation()
	{
		return View(); // Create a view to show order confirmation
	}

	private void UpdateCartTotalPrice(Cart cart)
	{
		cart.TotalPrice = cart.CartItems.Sum(item => item.Quantity * item.Product.Price);
	}
}
