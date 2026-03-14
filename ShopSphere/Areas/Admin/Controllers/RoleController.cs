using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopSphere.Areas.Admin.Models;
using ShopSphere.Data;
using ShopSphere.Models;

namespace ShopSphere.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public RoleController(
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles;
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name)
        {
            IdentityRole role = new IdentityRole
            {
                Name = name
            };

            var isExist = await _roleManager.RoleExistsAsync(role.Name);

            if (isExist)
            {
                ViewBag.mgs = "This role already exists.";
                ViewBag.name = name;
                return View();
            }

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                TempData["save"] = "Role has been saved successfully";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            ViewBag.id = role.Id;
            ViewBag.name = role.Name;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string name)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            role.Name = name;

            var isExist = await _roleManager.RoleExistsAsync(role.Name);

            if (isExist)
            {
                ViewBag.mgs = "This role already exists.";
                ViewBag.name = name;
                return View();
            }

            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                TempData["save"] = "Role has been updated successfully";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            ViewBag.id = role.Id;
            ViewBag.name = role.Name;

            return View();
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                TempData["delete"] = "Role has been deleted successfully";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }
        public IActionResult Assign()
        {
            ViewData["UserId"] = new SelectList(
                _db.Users
                .Where(u => u.LockoutEnd == null || u.LockoutEnd < DateTime.Now)
                .ToList(),
                "Id",
                "UserName");

            ViewData["RoleId"] = new SelectList(
                _roleManager.Roles.ToList(),
                "Name",
                "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(RoleUserVm roleUser)
        {
            var user = _db.Users
                .FirstOrDefault(u => u.Id == roleUser.UserId);

            if (user == null)
                return NotFound();

            var isCheckRoleAssign = await _userManager.IsInRoleAsync(user, roleUser.RoleId);

            if (isCheckRoleAssign)
            {
                ViewBag.mgs = "This user already has this role.";

                ViewData["UserId"] = new SelectList(
                    _db.Users
                    .Where(u => u.LockoutEnd == null || u.LockoutEnd < DateTime.Now)
                    .ToList(),
                    "Id",
                    "UserName");

                ViewData["RoleId"] = new SelectList(
                    _roleManager.Roles.ToList(),
                    "Name",
                    "Name");

                return View();
            }

            var role = await _userManager.AddToRoleAsync(user, roleUser.RoleId);

            if (role.Succeeded)
            {
                TempData["save"] = "User role assigned successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public IActionResult AssignUserRole()
        {
            var result =
                from ur in _db.UserRoles
                join r in _db.Roles on ur.RoleId equals r.Id
                join u in _db.Users on ur.UserId equals u.Id
                select new UserRoleMapping
                {
                    UserId = ur.UserId,
                    RoleId = ur.RoleId,
                    UserName = u.UserName,
                    RoleName = r.Name
                };

            ViewBag.UserRoles = result.ToList();

            return View();
        }
    }
}