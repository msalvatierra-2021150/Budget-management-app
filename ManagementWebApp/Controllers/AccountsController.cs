using AutoMapper;
using ManagementWebApp.Models;
using ManagementWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementWebApp.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountTypesRepository accountTypesRepository;
        private readonly IUserService userService;
        private readonly IAccountRepository accountRepository;
        private readonly IMapper mapper;

        public AccountsController(
            IAccountTypesRepository accountTypesRepository, 
            IUserService userService,
            IAccountRepository accountRepository,
            IMapper mapper
            ) {
            this.accountTypesRepository = accountTypesRepository;
            this.userService = userService;
            this.accountRepository = accountRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = userService.getUserId();
            var accountTypes = await accountTypesRepository.GetAll(userId);
            var model = new AccountsCreationViewModel();
            model.AccountTypes = await GetAccountTypes(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountsCreationViewModel account)
        {
            var userId = userService.getUserId();
            var accountType = await accountTypesRepository.GetById(account.AccountTypeId, userId);
            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Shared");
            }


            if (!ModelState.IsValid)
            {
                account.AccountTypes = await GetAccountTypes(userId);
                return View(account);
            }

            await accountRepository.Create(account);
            return RedirectToAction("Index");
        }

        public async Task<IEnumerable<SelectListItem>> GetAccountTypes(int userId)
        {
            var accountTypes = await accountTypesRepository.GetAll(userId);
            return accountTypes.Select(accountType =>
                new SelectListItem(
                    accountType.Name,
                    accountType.Id.ToString()
                    )
                );
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = userService.getUserId();
            var accounts = await accountRepository.GetAll(userId);
            var model = accounts
                .GroupBy(account => account.AccountType)
                .Select(groupedAccounts => new IndexAccountsViewModel
                {
                    //Key its the AccountType
                    AccountType = groupedAccounts.Key,
                    Accounts = groupedAccounts.AsEnumerable()
                }).ToList();
            //If we have to accounts of the same type, we group them
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = userService.getUserId();
            var account = await accountRepository.GetById(id, userId);
            if (account is null)
            {
                return RedirectToAction("Shared", "NotFound");
            }
            //Maps and assigns all data fro Accounts to AccountsCreationViewModel
            var model = mapper.Map<AccountsCreationViewModel>(account);
            model.AccountTypes = await GetAccountTypes(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountsCreationViewModel accountToEdit)
        {
            var userId = userService.getUserId();
            var account = await accountRepository.GetById(accountToEdit.Id, userId);
            if (account is null)
            {
                return RedirectToAction("Shared", "NotFound");
            }

            var accountType = await accountTypesRepository.GetById(accountToEdit.AccountTypeId, userId);
            if (accountType is null)
            {
                return RedirectToAction("Shared", "NotFound");
            }

            await accountRepository.Edit(accountToEdit);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = userService.getUserId();
            var account = await accountRepository.GetById(id, userId);
            if (account is null)
            {
                return RedirectToAction("Shared", "NotFound");
            }
            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAction(int id)
        {
            var userId = userService.getUserId();
            var account = await accountRepository.GetById(id, userId);
            if (account is null)
            {
                return RedirectToAction("Shared", "NotFound");
            }
            await accountRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
