using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BestCarRental.Models
{
    public class CarDto
    {
        [Required, MaxLength(100)]
        public string Brand { get; set; } = "";
        [Required, MaxLength(100)]
        public string CarModel { get; set; } = "";
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; } = "";
        [Required]
        public IFormFile? ImageFile { get; set; }

        [MaxLength(100)]
        public string Category { get; set; } = "";
       
    }
}
