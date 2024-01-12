using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagementWebApp.Models
{
    public class AccountsCreationViewModel : Accounts
    {
        public IEnumerable<SelectListItem>? AccountTypes { get; set; }
    }
}
