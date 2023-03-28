using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextwoIdentity.Models
{
    public class Product
    {
      
        public Guid Id { get; set; }
        [Required]
        public string? Name { get; set; }

        [Required]
        public double Price { get; set; }

        public DateTime AddedDate { get; set; }

        [Required]
        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }

        public Category? Category { get; set; }

    }
}
