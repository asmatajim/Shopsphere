using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Data;
using ShopSphere.Models;
using ShopSphere.Utility;

namespace ShopSphere.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order anOrder)
        {
            if (!ModelState.IsValid)
            {
                return View(anOrder);
            }

            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();

            if (!cart.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return View(anOrder);
            }

            anOrder.OrderDetails = new List<OrderDetails>();

            foreach (var item in cart)
            {
                anOrder.OrderDetails.Add(new OrderDetails
                {
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity
                });
            }

            anOrder.OrderNo = GetOrderNo();
            anOrder.OrderDate = DateTime.Now;
            anOrder.UserName = User.Identity.Name;

            _db.Orders.Add(anOrder);
            await _db.SaveChangesAsync();

            HttpContext.Session.Remove("cart");
            TempData["order"] = "Order Placed Successfully!";
            return RedirectToAction("Index", "Home");

        }

        public string GetOrderNo()
        {
            int rowCount = _db.Orders.Count() + 1;
            return rowCount.ToString("000");
        }
        public IActionResult OrderHistory()
        {
            var username = User.Identity.Name;

            var orders = _db.Orders
                .Where(o => o.UserName == username)
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }
    }
}
