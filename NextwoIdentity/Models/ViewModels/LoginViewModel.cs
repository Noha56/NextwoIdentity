using System.ComponentModel.DataAnnotations;

namespace NextwoIdentity.Models.ViewModels
{
    public class LoginViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Enter Email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
