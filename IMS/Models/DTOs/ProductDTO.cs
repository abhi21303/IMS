using System.ComponentModel.DataAnnotations;

namespace IMS.Models.DTOs
{
    public class ProductDTO
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        [Required(ErrorMessage = "Please select a category.")]
        public int? CategoryId { get; set; }
        public int Price { get; set; }
        public virtual Category? Category { get; set; }

    }
}
 