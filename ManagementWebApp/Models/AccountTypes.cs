using ManagementWebApp.Validation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManagementWebApp.Models
{
    public class AccountTypes
    {
        public int Id { get; set; }
        [Required(ErrorMessage="{0} is a required field")]
        [StringLength(50, ErrorMessage="The {0} field must be between {2} and {1} characters long", MinimumLength=2)]
        [Display(Name="Account Type Name:")]
        [FirstLetterMayuscule]
        [Remote(action: "VerifyExistance", controller: "AccountTypes")]
        public string Name{ get; set; }
        public int UserId { get; set; }
        public int OrderNum { get; set;}
    }
}
