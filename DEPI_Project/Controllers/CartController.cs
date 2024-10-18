using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    // Temporary cart data
    private static Cart cart = new Cart
    {
        Id = 1,
        CartItems = new List<CartItem>
        {
            new CartItem
            {
                Product = new Product { Id = 1, Name = "Sample Product 1", Price = 50.0 },
                Quantity = 1,
                Price = 50.0
            },
            new CartItem
            {
                Product = new Product { Id = 2, Name = "Sample Product 2", Price = 100.0 },
                Quantity = 1,
                Price = 100.0
            }
        }
    };

    public IActionResult Index()
    {
        // Calculate the total price based on quantity and price of each item
        cart.TotalPrice = cart.CartItems.Sum(item => item.Quantity * item.Product.Price);
        return View(cart);
    }


    [HttpPost]
    public IActionResult IncreaseQuantity(int productId)
    {
        var cartItem = cart.CartItems.FirstOrDefault(item => item.Product.Id == productId);
        if (cartItem != null)
        {
            cartItem.Quantity++;
            cartItem.Price = cartItem.Quantity * cartItem.Product.Price;
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DecreaseQuantity(int productId)
    {
        var cartItem = cart.CartItems.FirstOrDefault(item => item.Product.Id == productId);
        if (cartItem != null && cartItem.Quantity > 1)
        {
            cartItem.Quantity--;
            cartItem.Price = cartItem.Quantity * cartItem.Product.Price;
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult RemoveItem(int productId)
    {
        var cartItem = cart.CartItems.FirstOrDefault(item => item.Product.Id == productId);
        if (cartItem != null)
        {
            cart.CartItems.Remove(cartItem);
        }

        return RedirectToAction("Index");
    }


    [HttpGet]
    public IActionResult Checkout()
    {
        // Ensure the cart has items
        if (!cart.CartItems.Any())
        {
            return RedirectToAction("Index"); // Redirect to the cart if empty
        }

        return View(cart);
    }

    [HttpPost]
    public IActionResult PlaceOrder(string firstName, string lastName, string email, string address, string city, string country, string zipCode, string telephone, bool createAccount, bool differentAddress, string orderNotes, string paymentMethod, bool termsAccepted)
    {
        if (!termsAccepted)
        {
            ModelState.AddModelError("", "You must accept the terms and conditions.");
            return RedirectToAction("Checkout"); // Re-render the checkout page
        }

        // Here you can implement your order processing logic
        // e.g., save to database, send confirmation email, etc.

        // Clear the cart after placing the order
        cart.CartItems.Clear();

        return RedirectToAction("OrderConfirmation"); // Redirect to a confirmation page
    }

    public IActionResult OrderConfirmation()
    {
        return View(); // Create a view to show order confirmation
    }
}