using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DEPI_Project.Data;
using DEPI_Project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

public class ProductController : Controller
{

    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly List<string> _allowedExtensions = new() { ".png", ".jpg", ".jpeg", ".gif" };
   // private object _environment;
    private const long _maxAllowedPosterSize = 1048576; // 1 MB

    public ProductController(ApplicationDbContext context,  UserManager<ApplicationUser> userManager)
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

    [Authorize(Roles = "BusinessOwner,Admin")]
    public async Task<IActionResult> MyStoreProducts()
    {
        var user = await _userManager.GetUserAsync(User);
        var businessOwner = await _context.BusinessOwners.FirstOrDefaultAsync(b => b.UserId == user.Id);

        if (businessOwner == null)
        {
            return NotFound("Business owner not found.");
        }

        var products = await _context.Products
            .Where(p => p.BusinessOwnerId == businessOwner.Id)
            .Include(p => p.Category) // لجلب بيانات الفئة
            .ToListAsync();

        // تحويل قائمة Product إلى ProductViewModel
        var productViewModels = products.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = (decimal)p.Price,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            ImageUrl = p.ImageUrl,
            DiscountPercentage = p.DiscountPercentage,
            DiscountStartDate = p.DiscountStartDate,
            DiscountEndDate = p.DiscountEndDate
        }).ToList();

        return View(productViewModels);
    }


    // عرض صفحة إضافة منتج جديد
    [Authorize(Roles = "BusinessOwner,Admin")]
	public IActionResult AddProduct()
	{
		ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
		return View(new ProductViewModel());
	}

    [Authorize(Roles = "BusinessOwner,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProduct(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var businessOwner = await _context.BusinessOwners
            .FirstOrDefaultAsync(bo => bo.UserId == userId);

        if (businessOwner == null)
            return Unauthorized("You are not a Business Owner.");

        // تحقق مما إذا كان هناك صورة مرفوعة
        if (model.ImageFile != null)
        {
            // تحقق من الامتداد
            var extension = Path.GetExtension(model.ImageFile.FileName);
            if (!_allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("ImageFile", "Invalid image type.");
                ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
                return View(model);
            }

            // تحقق من حجم الصورة
            if (model.ImageFile.Length > _maxAllowedPosterSize)
            {
                ModelState.AddModelError("ImageFile", "Image size should not exceed 1 MB.");
                ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
                return View(model);
            }

            // مسار حفظ الصورة
            var imagePath = Path.Combine("wwwroot/images/products", model.ImageFile.FileName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            // تعيين مسار الصورة
            model.ImageUrl = $"/images/products/{model.ImageFile.FileName}";
        }

        // إنشاء كائن المنتج الجديد
        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Price = (double)model.Price,
            CategoryId = model.CategoryId,
            CreatedAt = DateTime.UtcNow,
            BusinessOwnerId = businessOwner.Id,
            ImageUrl = model.ImageUrl,
            DiscountPercentage = model.DiscountPercentage,
            DiscountStartDate = model.DiscountStartDate,
            DiscountEndDate = model.DiscountEndDate
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyStoreProducts));
    }



    // عرض صفحة تعديل منتج
    [Authorize(Roles = "BusinessOwner,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound("Product not found.");
        }

        var model = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = (decimal)product.Price,
            CategoryId = product.CategoryId,
            CategoryName = _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId)?.Name, // إذا كنت بحاجة لاسم الفئة
            ImageUrl = product.ImageUrl,
            DiscountPercentage = product.DiscountPercentage,
            DiscountStartDate = product.DiscountStartDate,
            DiscountEndDate = product.DiscountEndDate
        };

        ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
        return View(model);
    }


    // معالجة تعديل منتج
    [Authorize(Roles = "BusinessOwner,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        var product = await _context.Products.FindAsync(model.Id);
        if (product == null)
        {
            return NotFound("Product not found.");
        }

        // تحديث خصائص المنتج
        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = (double)model.Price;
        product.CategoryId = model.CategoryId;
        product.DiscountPercentage = model.DiscountPercentage;
        product.DiscountStartDate = model.DiscountStartDate;
        product.DiscountEndDate = model.DiscountEndDate;

        // إذا كان هناك صورة مرفوعة
        if (model.ImageFile != null)
        {
            var extension = Path.GetExtension(model.ImageFile.FileName);
            if (!_allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("ImageFile", "Invalid image type.");
                ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            if (model.ImageFile.Length > _maxAllowedPosterSize)
            {
                ModelState.AddModelError("ImageFile", "Image size should not exceed 1 MB.");
                ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            // مسار حفظ الصورة
            var imagePath = Path.Combine("wwwroot/images/products", model.ImageFile.FileName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            // تعيين مسار الصورة
            product.ImageUrl = $"/images/products/{model.ImageFile.FileName}";
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyStoreProducts));
    }



    [Authorize(Roles = "BusinessOwner,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.BusinessOwner.UserId == userId);

        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyStoreProducts));
    }

    [Authorize(Roles = "BusinessOwner,Admin")]
    public async Task<IActionResult> DeleteConfirmation(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound("Product not found.");
        }

        var model = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = (decimal)product.Price,
            CategoryId = product.CategoryId,
            CategoryName = _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId)?.Name,
            ImageUrl = product.ImageUrl
        };

        return View(model);
    }


    // دالة للحصول على معرف البيزنس أونر الحالي
    private async Task<string> GetCurrentBusinessOwnerId()
	{
		var user = await _userManager.GetUserAsync(User);
		var businessOwner = await _context.BusinessOwners.FirstOrDefaultAsync(b => b.UserId == user.Id);
		return businessOwner?.Id;
	}

}
