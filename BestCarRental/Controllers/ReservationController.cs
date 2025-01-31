using System.Configuration;
using System.Data;
using BestCarRental.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BestCarRental.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;

        private readonly string _connectionString;
        public ReservationController(AppDbContext context, IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ReservationView()
        {
            var user = _context.UserAccounts.FirstOrDefault(u => u.Email == HttpContext.User.Identity.Name);
            var currentDate = DateTime.Now;
            var reservations = _context.Reservations
                .Include(r => r.Car)
                .Where(r => r.UserAccountId == user.Id)
                .Select(r => new
                {
                    r.Id,
                    carModel = r.Car.CarModel,
                    r.CarId,
                    r.StartDate,
                    r.EndDate,
                })
                .ToList();
            return View(reservations);
        }

        public async Task<IActionResult> AddReservation(int id)
        {
            var cars = await _context.Cars.FirstOrDefaultAsync(s => s.Id == id);
        
        Console.WriteLine(id);
            if (cars == null)
            {
                return NotFound();
            }
            Reservation reservation = new Reservation();
            reservation.CarId = cars.Id;
            Console.WriteLine(reservation.CarId);

            return View(reservation);

        }

        [HttpPost]
        public IActionResult AddReservation(Reservation reservation)
        {
         
            UserAccount user = _context.UserAccounts
                .Where(ua => ua.Email == HttpContext.User.Identity.Name)
                .FirstOrDefault();

    
            DateTime startDate = reservation.StartDate != DateTime.MinValue
                                 ? reservation.StartDate
                                 : DateTime.Now;
            DateTime endDate = reservation.EndDate != DateTime.MinValue
                                 ? reservation.EndDate
                                 : DateTime.Now.AddDays(7);

           
            bool isAvailable = CheckIfCarIsAvailable(reservation.CarId, startDate, endDate);

            if (!isAvailable)
            {
                
                ModelState.AddModelError("", "Car is not available for the selected dates.");
                ViewBag.Message = "Reservation unavailable";
                return View(reservation);
            }

            
            Reservation res = new Reservation
            {
                CarId = reservation.CarId,
                StartDate = startDate,
                EndDate = endDate,
                UserAccountId = user?.Id ?? 0 
            };

            try
            {
                _context.Reservations.Add(res);
                _context.SaveChanges();
                ModelState.Clear();
                ViewBag.Message =
                    $"Reservation for car {res.CarId} from {res.StartDate} to {res.EndDate} was successfully added.";
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the reservation. Please try again.");
            }

            return View(reservation);
        }
        private bool CheckIfCarIsAvailable(int carId, DateTime startDate, DateTime endDate)
        {
            
            
            string connectionString = _context.Database.GetDbConnection().ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("CheckCarAvailability", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.AddWithValue("@CarId", carId);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    connection.Open();

                    // The stored procedure returns 0 or 1
                    int result = (int)command.ExecuteScalar();
                    return (result == 1);
                }
            }
        }



    }
}
