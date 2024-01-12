using ManagementWebApp.Validation;
using System.ComponentModel.DataAnnotations;

namespace ManagementWebApp.Models
{
    public class Accounts
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 50)]
        [FirstLetterMayuscule]
        public string Name { get; set; }
        [Display(Name = "Account Type")]
        public int AccountTypeId { get; set; }
        public decimal Balance { get; set; }
        [StringLength(maximumLength: 10000)]
        public string? Description { get; set; }
        public string? AccountType { get; set; }
    }
}
