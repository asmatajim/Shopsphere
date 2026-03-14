using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopSphere.Data;
using ShopSphere.Models;
using System.Linq;

namespace ShopSphere.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public UserController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.ApplicationUsers.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, user.PasswordHash);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    TempData["save"] = "User has been created successfully";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(user);
        }

        public IActionResult Edit(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);

            if (userInfo == null)
                return NotFound();

            userInfo.FirstName = user.FirstName;
            userInfo.LastName = user.LastName;

            var result = await _userManager.UpdateAsync(userInfo);

            if (result.Succeeded)
            {
                TempData["save"] = "User has been updated successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(userInfo);
        }

        public IActionResult Details(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        public IActionResult Lockout(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult Lockout(ApplicationUser user)
        {
            var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);

            if (userInfo == null)
                return NotFound();

            userInfo.LockoutEnd = DateTime.Now.AddYears(100);

            int rowAffected = _db.SaveChanges();

            if (rowAffected > 0)
            {
                TempData["save"] = "User has been lockout successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(userInfo);
        }

        public IActionResult Active(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult Active(ApplicationUser user)
        {
            var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);

            if (userInfo == null)
                return NotFound();

            userInfo.LockoutEnd = DateTime.Now.AddDays(-1);

            int rowAffected = _db.SaveChanges();

            if (rowAffected > 0)
            {
                TempData["save"] = "User has been active successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(userInfo);
        }

        public IActionResult Delete(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult Delete(ApplicationUser user)
        {
            var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);

            if (userInfo == null)
                return NotFound();

            _db.ApplicationUsers.Remove(userInfo);

            int rowAffected = _db.SaveChanges();

            if (rowAffected > 0)
            {
                TempData["save"] = "User has been deleted successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(userInfo);
        }
    }
}