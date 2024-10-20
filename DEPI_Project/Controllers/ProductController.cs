using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DEPI_Project.Data;
using DEPI_Project.Models;
using DEPI_Project.Models.ViewsModels;
using System.IO;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly List<string> _allowedExtensions = new() { ".png", ".jpg", ".jpeg", ".gif" };
    private const long _maxAllowedPosterSize = 1048576; // 1 MB

    public ProductController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // عرض كل المنتجات أو حسب الـ Category
    public IActionResult Index(int? categoryId)
    {
        var products = _context.Products
            .Where(p => !categoryId.HasValue || p.CategoryId == categoryId)
            .ToList();

        ViewBag.SelectedCategory = categoryId;
        ViewBag.Categories = _context.Categories.ToList();
        return View(products);
    }

    // عرض المنتجات الخاصة بالـ Store للبيزنس أونر أو كل المنتجات للأدمن
    [Authorize(Roles = "Admin,BusinessOwner")]
    public async Task<IActionResult> MyStoreProducts()
    {
        var user = await _userManager.GetUserAsync(User);
        var products = new List<Product>();

        if (User.IsInRole("Admin"))
        {
            products = _context.Products.ToList();
        }
        else if (User.IsInRole("BusinessOwner"))
        {
            var businessOwner = _context.BusinessOwners.FirstOrDefault(b => b.UserId == user.Id);
            if (businessOwner != null)
            {
                products = _context.Products
                    .Where(p => p.Store.BusinessOwnerId == businessOwner.Id)
                    .ToList();
            }
        }

        return View(products);
    }

    // عرض صفحة إضافة منتج جديد
    [Authorize(Roles = "Admin,BusinessOwner")]
    public IActionResult AddProduct()
    {
        return View(new ProductViewModel());
    }

    // معالجة إضافة منتج جديد
    [Authorize(Roles = "Admin,BusinessOwner")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProduct(ProductViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        var businessOwner = _context.BusinessOwners.FirstOrDefault(b => b.UserId == user.Id);

        if (businessOwner == null && !User.IsInRole("Admin"))
        {
            return Unauthorized();
        }

        var store = _context.Stores.FirstOrDefault(s => s.BusinessOwnerId == businessOwner.Id);
        if (store == null) return NotFound("Store not found.");

        var files = Request.Form.Files;
        if (!files.Any())
        {
            ModelState.AddModelError("Prod_Image", "Please select a product image.");
            return View(model);
        }

        var file = files.First();
        if (!_allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
        {
            ModelState.AddModelError("Prod_Image", "Invalid image format.");
            return View(model);
        }

        if (file.Length > _maxAllowedPosterSize)
        {
            ModelState.AddModelError("Prod_Image", "Image size exceeds 1 MB.");
            return View(model);
        }

        // حفظ الصورة في wwwroot/images
        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        var product = new Product
        {
            Name = model.Prod_Name,
            Description = model.Prod_Description,
            Price = (double)model.Prod_Price,
            ImageUrl = "/images/" + uniqueFileName, // حفظ المسار النسبي للصورة
            CreatedAt = DateTime.Now,
            StoreId = store.Id
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyStoreProducts));
    }

    // تعديل منتج
    [Authorize(Roles = "Admin,BusinessOwner")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return BadRequest();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (User.IsInRole("BusinessOwner") && product.Store.BusinessOwner.UserId != user.Id)
        {
            return Unauthorized();
        }

        var viewModel = new ProductViewModel
        {
            Id = product.Id,
            Prod_Name = product.Name,
            Prod_Price = (decimal)product.Price,
            Prod_Description = product.Description
        };

        return View("AddProduct", viewModel);
    }

    // معالجة تعديل المنتج
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductViewModel model)
    {
        if (!ModelState.IsValid) return View("AddProduct", model);

        var product = await _context.Products.FindAsync(model.Id);
        if (product == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (User.IsInRole("BusinessOwner") && product.Store.BusinessOwner.UserId != user.Id)
        {
            return Unauthorized();
        }

        var files = Request.Form.Files;
        if (files.Any())
        {
            var file = files.First();
            if (!_allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                ModelState.AddModelError("Prod_Image", "Invalid image format.");
                return View("AddProduct", model);
            }

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            product.ImageUrl = "/images/" + uniqueFileName;
        }

        product.Name = model.Prod_Name;
        product.Price = (double)model.Prod_Price;
        product.Description = model.Prod_Description;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyStoreProducts));
    }

    // حذف منتج
    [Authorize(Roles = "Admin,BusinessOwner")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (User.IsInRole("BusinessOwner") && product.Store.BusinessOwner.UserId != user.Id)
        {
            return Unauthorized();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyStoreProducts));
    }
}
