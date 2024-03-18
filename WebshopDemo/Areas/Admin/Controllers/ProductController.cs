using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebshopDemo.Models;
using WebshopDemo.Data;

namespace WebshopDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
                _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Product.ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Product
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(o => o.Id == id);

            var categories = await _context.Category.ToListAsync();

            foreach (var productCategory in product.ProductCategories)
            {
                productCategory.CategoryName = categories.FirstOrDefault(c => c.Id == productCategory.CategoryId)?.Name;
            }

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Category.Select(category =>
                new SelectListItem
                { 
                    Value = category.Id.ToString(),
                    Text = category.Name
                }
                ).ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                foreach (var categoryId in product.Categories)
                {
                    var productCategory = new ProductCategory
                    {
                        CategoryId = categoryId
                    };

                    product.ProductCategories.Add(productCategory); 
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Product
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.Category.Select(category =>
                new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name,
                    Selected  = product.ProductCategories.Select(c => c.CategoryId).Contains(category.Id)
                }
                ).ToListAsync();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var productDb = await _context.Product
                        .Include(p => p.ProductCategories)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    productDb?.ProductCategories.Clear();
                    productDb.Name = product.Name;
                    productDb.Description = product.Description;
                    productDb.Price = product.Price;
                    productDb.Quantity = product.Quantity;

                    foreach (var categoryId in product.Categories)
                    {
                        var productCategory = new ProductCategory
                        {
                            CategoryId = categoryId
                        };

                        productDb.ProductCategories.Add(productCategory);
                    }

                    _context.Update(productDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Product.Any(o => o.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Product.FirstOrDefaultAsync(o => o.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
