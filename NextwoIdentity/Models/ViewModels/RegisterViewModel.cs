using System.ComponentModel.DataAnnotations;

namespace NextwoIdentity.Models.ViewModels
{
    public class RegisterViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage ="Enter Email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]   
        public string? Password { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Conform and Password not match")]
        public string? ConfirmPassword { get; set; }
        public string? Phone { get; set; }
    }
}
