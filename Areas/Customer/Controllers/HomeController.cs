using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVm = new IndexViewModel
            {
                MenuItem = await _dbContext.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _dbContext.Category.ToListAsync(),
                Coupon = await _dbContext.Coupons.Where(c => c.IsActive).ToListAsync()
            };

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                var crt = _dbContext.ShoppingCarts.Where(u => u.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(SD.CartSessionKey, crt);
            }

            return View(IndexVm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var menuItemfromDb = await _dbContext.MenuItem.Include(m => m.SubCategory).Include(m => m.Category).Where(m => m.Id == id).FirstOrDefaultAsync();
            ShoppingCart cart = new ShoppingCart()
            {
                MenuItem = menuItemfromDb,
                MenuItemId = menuItemfromDb.Id
            };

            return View(cart);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cart)
        {
            cart.Id = 0;
            if (cart.MenuItemId != null)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claim.Value;

                var cartFromDb = await _dbContext.ShoppingCarts
                                                          .Where(c => c.ApplicationUserId == cart.ApplicationUserId && c.MenuItemId == cart.MenuItemId)
                                                          .FirstOrDefaultAsync();
                if (cartFromDb == null)
                {
                    await _dbContext.ShoppingCarts.AddAsync(cart);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + cart.Count;
                }
                await _dbContext.SaveChangesAsync();
                var count = _dbContext.ShoppingCarts
                                       .Where(c => c.ApplicationUserId == cart.ApplicationUserId && c.MenuItemId == cart.MenuItemId)
                                       .ToList().Count();

                HttpContext.Session.SetInt32(SD.CartSessionKey, count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var menuItemfromDb = await _dbContext.MenuItem.Where(m => m.Id == cart.MenuItemId)
                                                              .Include(m => m.SubCategory).Include(m => m.Category)
                                                              .FirstOrDefaultAsync();
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    MenuItem = menuItemfromDb,
                    MenuItemId = menuItemfromDb.Id
                };

                return View(shoppingCart);
            }
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}