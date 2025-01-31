using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BestCarRental.Models
{
    public class Car
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Brand { get; set; } = "";
        [MaxLength(100)]
        public string CarModel { get; set; } = "";
        [Precision(16,2)]
        public decimal Price { get; set; }
        [MaxLength(100)]
        public string Description { get; set; } = "";
        [Required,MaxLength(100)]
        public string ImageFileName { get; set; } = "";
        public DateTime AddedAt { get; set; }

        [MaxLength(100)]
        public string Category { get; set; } = "";

    }
}
