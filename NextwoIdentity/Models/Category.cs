using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NextwoIdentity.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        [Required]
        public string? Name { get; set; }

    }
}
