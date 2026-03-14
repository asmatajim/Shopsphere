using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Data;
using ShopSphere.Models;

namespace ShopSphere.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductTypesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var productTypes = _db.ProductTypes.ToList();
            return View(productTypes);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes model)
        {
            if (ModelState.IsValid)
            {
                var exists = await _db.ProductTypes
                    .FirstOrDefaultAsync(x => x.ProductType.ToLower() == model.ProductType.ToLower());

                if (exists != null)
                {
                    ModelState.AddModelError("", "Product Type already exists!");
                    return View(model);
                }
                _db.ProductTypes.Add(model);
                await _db.SaveChangesAsync();

                TempData["success"] = "Product Type added successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var model = _db.ProductTypes.Find(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductTypes model)
        {
            if (ModelState.IsValid)
            {
                var exists = await _db.ProductTypes
                    .FirstOrDefaultAsync(x => x.ProductType.ToLower() == model.ProductType.ToLower() && x.Id != model.Id);

                if (exists != null)
                {
                    TempData["error"] = "Product Type already exists!";
                    return View(model);
                }

                _db.ProductTypes.Update(model);
                await _db.SaveChangesAsync();

                TempData["success"] = "Product Type updated successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var model = _db.ProductTypes.Find(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var model = _db.ProductTypes.Find(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            var model = await _db.ProductTypes.FindAsync(id);

            if (model == null)
                return NotFound();

            _db.ProductTypes.Remove(model);
            await _db.SaveChangesAsync();

            TempData["delete"] = "Product Type deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
