using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BestCarRental.Models;

namespace BestCarRental.Entities
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserAccountId { get; set; }

        [ForeignKey(nameof(UserAccountId))]
        public UserAccount UserAccount { get; set; } // Navigation property for UserAccount

        [Required]
        public int CarId { get; set; }

        [ForeignKey(nameof(CarId))]
        public Car Car { get; set; } // Navigation property for Car

        [Required]
        public DateTime StartDate { get; set; } // Rental start date

        [Required]
        public DateTime EndDate { get; set; } // Rental end date
    }
}
