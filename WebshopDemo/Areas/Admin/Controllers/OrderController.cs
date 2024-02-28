using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel;
using Webshop.Models;
using WebshopDemo.Data;

namespace WebshopDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("Admin/Order/Index")]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Order.ToListAsync();

            return View(orders);
        }

        [Route("Admin/Order/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Order.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            //var orderProducts = await _context.OrderProduct.Where(p => p.OrderId == id).ToListAsync();

            //order.OrderProducts = orderProducts;

            return View(order);
        }

        [Route("Admin/Order/Create")]
        public async Task<IActionResult> Create() 
        {
            ViewBag.Users = await _context.Users.Select(user =>
                new SelectListItem 
                { 
                    Value = user.Id.ToString(),
                    Text = user.FirstName + " " + user.LastName
                }
            ).ToListAsync();

            return View();
        }

        [Route("Admin/Order/Create/{order}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateCreated,Total,UserId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return  RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        [Route("Admin/Order/Create/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null) 
            {
                return NotFound();
            }

            return View(order);
        }

        [Route("Admin/Order/Edit/{id}/{order}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateCreated,Total,UserId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Order.Any(o => o.Id == id))
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
            return View(order);
        }

        [Route("Admin/Order/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Order.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Route("Admin/Order/DeleteConfirmed/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
