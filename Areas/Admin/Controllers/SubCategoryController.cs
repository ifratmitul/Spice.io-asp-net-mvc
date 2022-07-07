using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dbContext.SubCategory.Include(c => c.Category).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                Categories = await _dbContext.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _dbContext.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (!ModelState.IsValid) //Issue in the model
            {
                var doesSubCategoryExists = _dbContext.SubCategory
                                                      .Include(sb => sb.Category)
                                                      .Where(sb => sb.Name == model.SubCategory.Name && sb.Category.Id == model.SubCategory.CategoryId);

                if (doesSubCategoryExists.Count() > 0)
                {
                    //error
                    StatusMessage = $"Error: Sub Category exists Under {doesSubCategoryExists.First().Category.Name} category, Please use another category";
                }
                else
                {
                    _dbContext.SubCategory.Add(model.SubCategory);
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                StatusMessage = "Error: Invalid Model";
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                Categories = await _dbContext.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _dbContext.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage,
            };

            return View(modelVM);
        }
        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            subCategories = await (from subCategory in _dbContext.SubCategory
                                   where subCategory.CategoryId == id
                                   select subCategory
                             ).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        public async Task<IActionResult> Edit(int id)
        {

            if (id == null) return NotFound();

            var subCategory = await _dbContext.SubCategory.SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null) return NotFound();

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                Categories = await _dbContext.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _dbContext.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoryAndCategoryViewModel model)
        {
            if (!ModelState.IsValid) //Issue in the modelstate
            {
                var doesSubCategoryExists = _dbContext.SubCategory
                                                      .Include(sb => sb.Category)
                                                      .Where(sb => sb.Name == model.SubCategory.Name && sb.Category.Id == model.SubCategory.CategoryId);

                if (doesSubCategoryExists.Count() > 0)
                {
                    //error
                    StatusMessage = $"Error: Sub Category exists Under {doesSubCategoryExists.First().Category.Name} category, Please use another category";
                }
                else
                {
                    SubCategory sb = await _dbContext.SubCategory.FindAsync(id);
                    sb.Name = model.SubCategory.Name;

                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                StatusMessage = "Error: Invalid Model";
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                Categories = await _dbContext.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _dbContext.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage,
            };

            return View(modelVM);
        }
    }
}
