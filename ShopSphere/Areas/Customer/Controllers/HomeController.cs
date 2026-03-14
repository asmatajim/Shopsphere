using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Data;
using ShopSphere.Models;
using ShopSphere.Utility;
using System.Diagnostics;
using X.PagedList;
using X.PagedList.Extensions;

namespace ShopSphere.Areas.Customer.Controllers
{


    [Area("Customer")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(string search, int? productType, int? tag, int? page)
        {
            int pageSize = 9;
            int pageNumber = page ?? 1;

            var products = _db.Products
                .Include(c => c.ProductTypes)
                .Include(c => c.SpecialTag)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search));
            }

            if (productType != null)
            {
                products = products.Where(p => p.ProductTypeId == productType);
            }

            if (tag != null)
            {
                products = products.Where(p => p.SpecialTagId == tag);
            }

            ViewBag.ProductTypes = _db.ProductTypes.ToList();
            ViewBag.Tags = _db.SpecialTags.ToList();

            return View(products.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
        public IActionResult Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var product = _db.Products
                .Include(c => c.ProductTypes)
                .FirstOrDefault(c => c.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }


        [HttpPost]
        [ActionName("Detail")]
        public IActionResult ProductDetail(int? id)
        {
            if (id == null)
                return NotFound();

            var product = _db.Products
                .Include(c => c.ProductTypes)
                .FirstOrDefault(c => c.Id == id);

            if (product == null)
                return NotFound();

            List<Products> products = HttpContext.Session.Get<List<Products>>("products") ?? new List<Products>();

            var existingProduct = products.FirstOrDefault(p => p.Id == id);

            if (existingProduct == null)
            {
                products.Add(product);
            }

            HttpContext.Session.Set("products", products);

            return RedirectToAction(nameof(Index));
        }

        [ActionName("Remove")]
        public IActionResult RemoveToCart(int? id)
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");

            if (products != null)
            {
                var product = products.FirstOrDefault(c => c.Id == id);

                if (product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove(int? id)
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");

            if (products != null)
            {
                var product = products.FirstOrDefault(c => c.Id == id);

                if (product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCartItem(int id)
        {
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("cart");

            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.Product.Id == id);

                if (item != null)
                {
                    cart.Remove(item);
                    HttpContext.Session.Set("cart", cart);
                }
            }

            return RedirectToAction(nameof(Cart));
        }
        public IActionResult Cart()
        {
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();

            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int id)
        {
               var product = _db.Products
                   .Include(p => p.ProductTypes)
                   .Include(p => p.SpecialTag)
                   .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();

            var item = cart.FirstOrDefault(c => c.Product.Id == id);

            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    Product = product,
                    Quantity = 1
                });
            }

            HttpContext.Session.Set("cart", cart);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult IncreaseQty(int id)
        {
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("cart");

            var item = cart.FirstOrDefault(c => c.Product.Id == id);

            if (item != null)
            {
                item.Quantity++;
            }

            HttpContext.Session.Set("cart", cart);

            return RedirectToAction("Cart");
        }
        public IActionResult DecreaseQty(int id)
        {
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("cart");

            var item = cart.FirstOrDefault(c => c.Product.Id == id);

            if (item != null)
            {
                item.Quantity--;

                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
            }

            HttpContext.Session.Set("cart", cart);

            return RedirectToAction("Cart");
        }
    }
}