using System.ComponentModel.DataAnnotations;

namespace ManagementWebApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(maximumLength: 50, ErrorMessage = "It can be longer than {1} characters" +
            "")]
        public string Name { get; set; }
        [Display(Name = "Operation Type")]
        public TransactionType TransactionTypeId { get; set; }
        public int UserId { get; set; }
    }
}
