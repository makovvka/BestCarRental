using BestCarRental.Entities;
using BestCarRental.Models;
using Microsoft.AspNetCore.Mvc;

namespace BestCarRental.Controllers
{
    public class CarsController : Controller
    {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment environment;

        public CarsController(AppDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
 
        public IActionResult Index(string searchString)
        {
            var cars = context.Cars.OrderByDescending(p => p.Id).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(n => n.Brand.Contains(searchString)).ToList();
            }

            return View(cars);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CarDto cardto)
        {
            if (cardto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(cardto);
            }

            // save the image file 

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(cardto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/cars/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                cardto.ImageFile.CopyTo(stream);
            }

            Car car = new Car()
            {
                Brand = cardto.Brand,
                CarModel = cardto.CarModel,
                Price = cardto.Price,
                Description = cardto.Description,
                Category = cardto.Category,
                ImageFileName = newFileName,
                AddedAt = DateTime.Now,
            };

            context.Cars.Add(car);
            context.SaveChanges();

            return RedirectToAction("Index", "Cars");
        }

        public ActionResult Edit(int id)
        {
            var car = context.Cars.Find(id);

            if (car == null)
            {
                return RedirectToAction("Index", "Cars");
            }
            var cardto = new CarDto()
            {
                Brand = car.Brand,
                CarModel = car.CarModel,
                Price = car.Price,
                Category = car.Category,
                Description = car.Description,
            };

            ViewData["Id"] = car.Id;
            ViewData["ImageFileName"] = car.ImageFileName;
            ViewData["AddedAt"] = car.AddedAt.ToString("MM/dd/yyyy");

            return View(cardto);
        }

        [HttpPost]
        public ActionResult Edit(int id, CarDto carDto)
        {
            var car = context.Cars.Find(id);
            if (car == null)
            {
                return RedirectToAction("Index", "Cars");
            }


            if (!ModelState.IsValid)
            {
                ViewData["Id"] = car.Id;
                ViewData["ImageFileName"] = car.ImageFileName;
                ViewData["AddedAt"] = car.AddedAt.ToString("MM/dd/yyyy");
                return View(car);
            }

            // update the image file if we have a new image 
            string newFileName = car.ImageFileName;
            if (carDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(carDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/cars/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    carDto.ImageFile.CopyTo(stream);
                }

                // delete old pic

                string oldImageFullPath = environment.WebRootPath + "/cars/" + car.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
            }

            // update the car in DB

            car.Brand = carDto.Brand;
            car.CarModel = carDto.CarModel;
            car.Description = carDto.Description;
            car.Category = carDto.Category;
            car.ImageFileName = newFileName;
            car.Price = carDto.Price;

            context.SaveChanges();
            return RedirectToAction("Index", "Cars");
        }

        public IActionResult Delete(int id) 
        {
            var car = context.Cars.Find(id);
            if (car == null)
            {
                return RedirectToAction("Index", "Cars");
            }

            string imageFullPath = environment.WebRootPath + "/cars/" + car.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            context.Cars.Remove(car);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Cars");
        }
    }

}
