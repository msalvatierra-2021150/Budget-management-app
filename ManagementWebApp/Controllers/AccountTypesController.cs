using ManagementWebApp.Models;
using ManagementWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManagementWebApp.Controllers
{
    public class AccountTypesController : Controller
    {
        private readonly IAccountTypesRepository accountTypesRepository;

        public IUserService userService;

        public AccountTypesController(IAccountTypesRepository accountTypesRepository, IUserService userService)
        {
            this.accountTypesRepository = accountTypesRepository;
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountTypes accountTypes)
        {
            if (!ModelState.IsValid)
            {
                return View(accountTypes);
            }

            var existance = await accountTypesRepository.VerifyExistance(accountTypes);
            if (existance)
            {
                ModelState.AddModelError("Name", "This account type already exists");
                return View(accountTypes);
            }
            accountTypes.UserId = userService.getUserId();
            accountTypes.OrderNum = 1;
            await accountTypesRepository.Create(accountTypes);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerifyExistance(AccountTypes accountTypes)
        {
            accountTypes.UserId = userService.getUserId();
            accountTypes.OrderNum = 1;
            var accountTypeExists = await accountTypesRepository.VerifyExistance(accountTypes);
            if (accountTypeExists)
            {
                return Json($"Account type {accountTypes.Name} already exists");
            }
            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int userId = 1)
        {
            var accountTypes = await accountTypesRepository.GetAll(userId);
            return View(accountTypes);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = userService.getUserId();
            var accountTypeExistance = accountTypesRepository.GetById(id, userId);
            if (accountTypeExistance is null)
            {
                return RedirectToAction("NotFound");
            }
            var accountType = await accountTypesRepository.GetById(id, userId);
            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountTypes accountTypes)
        {
            if (!ModelState.IsValid)
            {
                return View(accountTypes);
            }

            var userId = userService.getUserId();
            var accountTypeExistance = accountTypesRepository.GetById(accountTypes.Id, userId);
            if (accountTypeExistance is null)
            {
                return RedirectToAction("NotFound");
            }

            accountTypes.UserId = userService.getUserId();
            await accountTypesRepository.Edit(accountTypes);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = userService.getUserId();
            var accountType = await accountTypesRepository.GetById(id, userId);
            if (accountType is null)
            {
                return RedirectToAction("NotFound");
            }
            return View(accountType);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAction(int id)
        {
            var userId = userService.getUserId();
            var accountType = await accountTypesRepository.GetById(id, userId);
            if (accountType is null)
            {
                return RedirectToAction("NotFound");
            }
            await accountTypesRepository.Delete(id, userId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Order([FromBody] int[] ids)
        {
            var userId = userService.getUserId();
            var accountTypes = await accountTypesRepository.GetAll(userId);
            var accountTypesIdsFromBackend = accountTypes.Select(accountType => accountType.Id);

            var accountTypesIdsNotBelongingToUser = ids.Except(accountTypesIdsFromBackend).ToList();
           
            if (accountTypesIdsNotBelongingToUser.Count > 0)
            {
                return Forbid();
            }

            var orderedAccountTypes = ids.Select((id, index) =>
                new AccountTypes() { Id = id, OrderNum = index + 1 }).AsEnumerable();
            
            await accountTypesRepository.Order(orderedAccountTypes);
            
            return Ok();
        }
    }
}