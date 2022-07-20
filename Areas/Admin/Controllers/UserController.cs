using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Utility;
using System.Security.Claims;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public UserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)this.User.Identity;
            Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View(await _dbContext.ApplicationUser.Where(u => u.Id != claim.Value).ToListAsync());
        }

        public async Task<IActionResult> Lock(string id)
        {
            if (id == null) return NotFound();

            var applicationUser = await _dbContext.ApplicationUser.Where(m => m.Id == id).FirstOrDefaultAsync();
            if (applicationUser == null) return NotFound();

            applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Unlock(string id)
        {
            if (id == null) return NotFound();

            var applicationUser = await _dbContext.ApplicationUser.Where(m => m.Id == id).FirstOrDefaultAsync();
            if (applicationUser == null) return NotFound();

            applicationUser.LockoutEnd = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
