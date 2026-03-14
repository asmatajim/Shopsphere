using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Data;
using ShopSphere.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShopSphere.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _he;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment he)
        {
            _db = db;
            _he = he;
        }

        public IActionResult Index()
        {
            var products = _db.Products
                .Include(c => c.ProductTypes)
                .Include(c => c.SpecialTag)
                .ToList();

            return View(products);
        }

        [HttpPost]
        public IActionResult Index(string searchName, decimal? lowAmount, decimal? largeAmount)
        {
            var products = _db.Products
                .Include(c => c.ProductTypes)
                .Include(c => c.SpecialTag)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                products = products.Where(p => p.Name.Contains(searchName));
            }

            if (lowAmount != null)
            {
                products = products.Where(p => p.Price >= lowAmount);
            }

            if (largeAmount != null)
            {
                products = products.Where(p => p.Price <= largeAmount);
            }

            return View(products.ToList());
        }
        public IActionResult Create()
        {
            ViewData["productTypeId"] = new SelectList(_db.ProductTypes, "Id", "ProductType");
            ViewData["TagId"] = new SelectList(_db.SpecialTags, "Id", "Name");
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Create(Products product, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var searchProduct = _db.Products
                    .FirstOrDefault(c => c.Name.ToLower().Trim() == product.Name.ToLower().Trim());

                if (searchProduct != null)
                {
                    ViewBag.ProductTypes = new SelectList(_db.ProductTypes, "Id", "ProductType");
                    ViewBag.SpecialTags = new SelectList(_db.SpecialTags, "Id", "Name");
                    ModelState.AddModelError("", "This product is already added!");
                    return View(product);
                }

                if (image != null)
                {
                    string uploadFolder = Path.Combine(_he.WebRootPath, "Images");

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;

                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    product.Image = "Images/" + uniqueFileName;
                }
                else
                {
                    product.Image = "Images/noimage.PNG";
                }

                _db.Products.Add(product);
                await _db.SaveChangesAsync();

                TempData["success"] = "Product Added Successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        public IActionResult Edit(int? id)
        {
            ViewData["productTypeId"] = new SelectList(_db.ProductTypes, "Id", "ProductType");
            ViewData["TagId"] = new SelectList(_db.SpecialTags, "Id", "Name");

            if (id == null)
                return NotFound();

            var product = _db.Products
                .Include(c => c.ProductTypes)
                .Include(c => c.SpecialTag)
                .FirstOrDefault(c => c.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }



        [HttpPost]
        public async Task<IActionResult> Edit(Products products, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string uploadFolder = Path.Combine(_he.WebRootPath, "Images");

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;

                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    products.Image = "Images/" + uniqueFileName;
                }

                _db.Products.Update(products);
                await _db.SaveChangesAsync();

                TempData["success"] = " Product Updated Successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(products);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = _db.Products
                .Include(c => c.ProductTypes)
                .Include(c => c.SpecialTag)
                .FirstOrDefault(c => c.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = _db.Products
                .Include(c => c.SpecialTag)
                .Include(c => c.ProductTypes)
                .FirstOrDefault(c => c.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null)
                return NotFound();

            var product = _db.Products.FirstOrDefault(c => c.Id == id);

            if (product == null)
                return NotFound();

            var imagePath = Path.Combine(_he.WebRootPath, product.Image);

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            TempData["delete"] = " Product Deleted Successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}