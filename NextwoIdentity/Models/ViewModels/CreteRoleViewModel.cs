using System.ComponentModel.DataAnnotations;

namespace NextwoIdentity.Models.ViewModels
{
    public class CreteRoleViewModel
    {
        [Required]
        public string? RoleName { get; set; }
    }
}
