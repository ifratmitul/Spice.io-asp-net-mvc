﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }
        public MenuItemController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Categories = _dbContext.Category,
                MenuItem = new Models.MenuItem()
            };
        }

        public async Task<IActionResult> Index()
        {
            var menuItem = await _dbContext.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(menuItem);
        }

        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            //if (!ModelState.IsValid)
            //{
            //    return View(MenuItemVM);
            //}
            _dbContext.MenuItem.Add(MenuItemVM.MenuItem);
            //await _dbContext.SaveChangesAsync();
            //Image upload start
            string webRootPath = _webHostEnvironment.WebRootPath;
            var file = HttpContext.Request.Form.Files;
            var menuItemfromDb = await _dbContext.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);


            if (file.Count > 0)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(file[0].FileName);
                using (FileStream? fileStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                };

                MenuItemVM.MenuItem.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                MenuItemVM.MenuItem.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }
            //image upload end

            //Issue available in creating here....

            //_dbContext.MenuItem.Add(MenuItemVM.MenuItem);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            MenuItemVM.MenuItem = await _dbContext.MenuItem
                                                   .Include(m => m.Category)
                                                   .Include(m => m.SubCategory)
                                                   .Where(m => m.Id == id).SingleOrDefaultAsync();

            MenuItemVM.SubCategories = await _dbContext.SubCategory
                                                       .Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId)
                                                       .ToListAsync();

            if (MenuItemVM.MenuItem == null) return NotFound();
            return View(MenuItemVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            //if (!ModelState.IsValid)
            //{
            //    return View(MenuItemVM);
            //}
            _dbContext.MenuItem.Add(MenuItemVM.MenuItem);
            //await _dbContext.SaveChangesAsync();
            //Image upload start
            string webRootPath = _webHostEnvironment.WebRootPath;
            var file = HttpContext.Request.Form.Files;
            var menuItemfromDb = await _dbContext.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);


            if (file.Count > 0)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(file[0].FileName);
                using (FileStream? fileStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                };

                MenuItemVM.MenuItem.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                MenuItemVM.MenuItem.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }
            //image upload end

            //Issue available in creating here....

            //_dbContext.MenuItem.Add(MenuItemVM.MenuItem);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

    }
}
