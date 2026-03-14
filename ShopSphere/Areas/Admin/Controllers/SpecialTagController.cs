using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Data;
using ShopSphere.Models;

namespace ShopSphere.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SpecialTagController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SpecialTagController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var specialTags = _db.SpecialTags.ToList();
            return View(specialTags);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTag specialTag)
        {
            if (ModelState.IsValid)
            {
                var exists = await _db.SpecialTags
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == specialTag.Name.ToLower());

                if (exists != null)
                {
                    ModelState.AddModelError("", "Special tag already exists!");
                    return View(specialTag);
                }

                _db.SpecialTags.Add(specialTag);
                await _db.SaveChangesAsync();

                TempData["success"] = "Special Tag added successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(specialTag);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = _db.SpecialTags.Find(id);

            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SpecialTag specialTag)
        {
            if (ModelState.IsValid)
            {
                var exists = await _db.SpecialTags
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == specialTag.Name.ToLower() && x.Id != specialTag.Id);

                if (exists != null)
                {
                    TempData["error"] = "Special Tag already exists!";
                    return View(specialTag);
                }

                _db.SpecialTags.Update(specialTag);
                await _db.SaveChangesAsync();

                TempData["success"] = "Special Tag updated successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(specialTag);
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = _db.SpecialTags.Find(id);

            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = _db.SpecialTags.Find(id);

            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = await _db.SpecialTags.FindAsync(id);

            if (specialTag == null)
                return NotFound();

            _db.SpecialTags.Remove(specialTag);
            await _db.SaveChangesAsync();

            TempData["delete"] = "Special Tag deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
