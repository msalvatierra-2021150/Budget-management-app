using ManagementWebApp.Models;
using ManagementWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManagementWebApp.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IUserService userService;

        public CategoriesController(ICategoryRepository categoryRepository, IUserService userService) {
            this.categoryRepository = categoryRepository;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var id = userService.getUserId();
            var category = await categoryRepository.Get(id);
            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            var id = userService.getUserId();
            category.UserId = id;
            await categoryRepository.Create(category);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = userService.getUserId();
            var category = await categoryRepository.getById(id, userId);
            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category categoryToEdit)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryToEdit);
            }
            var userId = userService.getUserId();
            var category = await categoryRepository.getById(categoryToEdit.Id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await categoryRepository.EditCategory(categoryToEdit);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = userService.getUserId();
            var category = await categoryRepository.getById(id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAction(int id)
        {
            var userId = userService.getUserId();
            var category = categoryRepository.getById(id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await categoryRepository.Delete(id, userId);
            return RedirectToAction("Index");
        }
    }
}
