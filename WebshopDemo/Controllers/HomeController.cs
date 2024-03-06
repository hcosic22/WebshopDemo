using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebshopDemo.Models;
using WebshopDemo.Data;
using WebshopDemo.Extensions;
using WebshopDemo.Models;

namespace WebshopDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;

        public const string SessionKeyName = "cart";

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string message)
        {
            return View(message);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Product(int? categoryId)
        {
            var productIds = await _context.ProductCategory.Where(p => p.Id == categoryId).Select(p => p.ProductId).ToListAsync();

            var products = await _context.Product.Where(p => productIds.Contains(p.Id)).ToListAsync();

            ViewBag.Categories = await _context.Category.Select(c => 
                new SelectListItem 
                { 
                    Value = c.Id.ToString(),
                    Text = c.Name,
                }).ToListAsync();

            return View(products);
        }

        public IActionResult Order(List<string> errors)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName) ?? new List<CartItem>();

            if (cart.Count == 0) 
            {
                return RedirectToAction(nameof(Index));
            }

            decimal sum = 0;

            ViewBag.TotalPrice = cart.Sum(item => sum + item.GetTotal());

            ViewBag.Errors = errors;

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrder(Order order)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName) ?? new List<CartItem>();

            if (cart.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var modelErrors = new List<string>();
            if (ModelState.IsValid) 
            {
                List<OrderProduct> orderProducts = new List<OrderProduct>();

                foreach (var cartItem in cart)
                {
                    OrderProduct orderProduct = new OrderProduct
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.Product.Id,
                        Quantity = cartItem.Quantity,
                        Total = cartItem.GetTotal(),
                    };

                    orderProducts.Add(orderProduct);
                }
                
                order.OrderProducts = orderProducts;

                _context.Order.Add(order);
                _context.SaveChanges();

                HttpContext.Session.SetObjectAsJson(SessionKeyName, "");

                return RedirectToAction(nameof(Index), new { message = "Thank you for your order" });
            }
            else
            {
                foreach (var modelState in ModelState.Values) 
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        modelErrors.Add(modelError.ErrorMessage);
                    }
                }

                return RedirectToAction(nameof(Order), new { errors = modelErrors });
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
