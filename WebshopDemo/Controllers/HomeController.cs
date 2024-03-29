using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebshopDemo.Models;
using WebshopDemo.Data;
using WebshopDemo.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Authorization;

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
            return View(nameof(Index), message);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Product(int? categoryId)
        {
            var products = await _context.Product.ToListAsync();
            ViewBag.CurrentCategory = "Filter category";

            if (categoryId != null) 
            {
                var productIds = await _context.ProductCategory
                    .Where(p => p.CategoryId == categoryId)
                    .Select(p => p.ProductId)
                    .ToListAsync();

                products = products.Where(p => productIds.Contains(p.Id)).ToList();

                var currentCategory = await _context.Category.FirstOrDefaultAsync(c => c.Id == categoryId);

                ViewBag.CurrentCategory = currentCategory?.Name;
            }

            ViewBag.Categories = await _context.Category.Select(c => 
                new SelectListItem 
                { 
                    Value = c.Id.ToString(),
                    Text = c.Name,
                }).ToListAsync();

            return View(products);
        }

        [Authorize(Roles = "User, Admin")]
        public IActionResult Order(List<string> errors, Order test)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName) ?? new List<CartItem>();

            var order = HttpContext.Session.GetObjectFromJson<Order>("orderData") ?? new Order();

            if (cart.Count == 0) 
            {
                return RedirectToAction(nameof(Index));
            }

            decimal sum = 0;

            ViewBag.TotalPrice = cart.Sum(item => sum + item.GetTotal());

            ViewBag.Errors = errors;

            ViewBag.CurrentUserId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;

            var data = new
            {
                OrderData = order,
                CartItems = cart,
            };

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User, Admin")]
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
                        Price = cartItem.Product.Price
                    };

                    orderProducts.Add(orderProduct);
                }
                
                order.OrderProducts = orderProducts;
                order.DateCreated = DateTime.Now;

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

                HttpContext.Session.SetObjectAsJson("orderData", order);
                return RedirectToAction(nameof(Order), new { errors = modelErrors, test = order });
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
