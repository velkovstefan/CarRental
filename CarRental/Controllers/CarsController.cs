using CarRental.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using static CarRental.Controllers.CarsController;

namespace CarRental.Controllers
{
    public class CarsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static List<string> carTypes = new List<string>
            {
                "Cabriolet/Roadster","Estate Car", "Off-road Vehicle/Pickup Truck/SUV","Sedan","SmallCar", "SportsCar/Coupe", "Van/Minibus","Other"
            };
        private static List<string> fuelTypes = new List<string>
            {
                "Petrol","Diesel", "Electric"
            };
        private static List<int> numDoors = new List<int>
            {
                2,4
            };
        private static List<string> transmission = new List<string>
            {
                "Manual","Automatic"
            };
        public class Data
        {
            public int carId { get; set; }
            public int grade { get; set; }
            public string comment { get; set; }
        }
        public class RentData
        {
            public int carId { get; set; }
            public string start { get; set; }
            public string end { get; set; }
        }
        public class GetDateData
        {
            public int carId { get; set; }
            
        }
        public class ServiceData
        {
            public int carId { get; set; }
            public string typeOfService { get; set; }
        }
        public class JsonMessage
        {
            public JsonMessage(string message)
            {
                this.message = message;
            }

            public string message { get; set; }
        }
        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("Something went wrong!");
            return char.ToUpper(input[0]) + input.Substring(1); 
        }
        // GET: Cars
        public ActionResult Index()
        {
          
                var data = db.Database
                                  .SqlQuery<CarBrands>("SELECT * FROM CarBrands").ToList();
                ViewBag.CarBrands = data;
          

            List<CarsViewModel> viewModel = new List<CarsViewModel>();
            DateTime date = DateTime.Today;
            List<Car> listCars = new List<Car>();

            if (Request.QueryString.Count==0) {

                listCars = db.cars.OrderByDescending(p => p.NumderOfTimesRented).ToList();

            }
            else
            {
                var query = db.cars.OrderByDescending(p => p.NumderOfTimesRented).AsQueryable();


                foreach (string key in Request.QueryString.AllKeys)
                {
                    string val = Request.QueryString[key];
                    if (string.IsNullOrEmpty(val)) continue;

                    string tmpKey = key.Replace("min", "").Replace("max", "");
                    tmpKey = char.ToUpper(tmpKey[0]) + tmpKey.Substring(1);
                    tmpKey = tmpKey.Replace("NumDoors", "NumberOfDoors");
                    tmpKey = tmpKey.Replace("NumSeats", "NumberOfSeats");

                    var prop = typeof(Car).GetProperties()
                        .FirstOrDefault(p => p.Name.Equals(tmpKey));

                    if (prop == null) continue;

                    var parameter = Expression.Parameter(typeof(Car));
                    var property = Expression.Property(parameter, prop.Name);
                    Expression predicate = null;

                   
                    if (key.Contains("min") && int.TryParse(val, out int min))
                    {
                        var constant = Expression.Constant(min);
                        predicate = Expression.GreaterThanOrEqual(property, constant);
                    }
                    else if (key.Contains("max") && int.TryParse(val, out int max))
                    {
                        var constant = Expression.Constant(max);
                        predicate = Expression.LessThanOrEqual(property, constant);
                    }
                   
                    else if (prop.PropertyType == typeof(string))
                    {
                        var constant = Expression.Constant(val);

                            predicate = Expression.Equal(property, constant);
                        
                    }
                    else
                    {
                        var convertedValue = Convert.ChangeType(val, prop.PropertyType);
                        var constant = Expression.Constant(convertedValue);
                        predicate = Expression.Equal(property, constant);
                    }

                    if (predicate != null)
                    {
                        var lambda = Expression.Lambda<Func<Car, bool>>(predicate, parameter);
                        // parametar => predicate(parametar.property, constant)
                        query = query.Where(lambda);
                    }
                }
                
                listCars = query.OrderByDescending(p => p.NumderOfTimesRented).ToList();

            }

            bool onlyRentable = Request.QueryString["onlyRentable"] != null && Request.QueryString["onlyRentable"] == "on";
            foreach (var item in listCars)
            {
                CarsViewModel element = new CarsViewModel(item, true);
                List<RentedCars> isRented = db.rentals.Where(a => (a.CarId == item.Id) && (DateTime.Compare(a.DateStart, date) <= 0) && (DateTime.Compare(a.DateEnd, date) >= 0)).ToList();
                if (isRented.Count == 0 || isRented == null)
                {
                    element.IsRented = false;
                }
                if ( onlyRentable && element.IsRented)
                {
                    continue;
                }
                viewModel.Add(element);
            }

            ViewBag.CarTypes = carTypes;
            ViewBag.FuelTypes = fuelTypes;
            ViewBag.NumDoors = numDoors;
            ViewBag.Transmissions = transmission;
            return View(viewModel);
        }
        [Authorize]
        public ActionResult MyRentedCars()
        {
            string userId = User.Identity.GetUserId();
           

            List<RentedCarsViewModel> viewModel = new List<RentedCarsViewModel>();

            DateTime date = DateTime.Today;
            List<RentedCars> Rented = db.rentals.Where(a => (a.UserId == userId)).OrderByDescending(p => p.DateStart).ToList();

            foreach (var item in Rented)
            {
                var car = new Car();
                car = db.cars.Find(item.CarId);
                if (car != null)
                {
                    RentedCarsViewModel element = new RentedCarsViewModel(car, false, item.DateStart, item.DateEnd, item.Id); 
                    
                 if ((DateTime.Compare(item.DateStart, date) <= 0) && (DateTime.Compare(item.DateEnd, date) >= 0))
                {
                    element.IsRented = true;
                    
                }
                    viewModel.Add(element);

                }
              
                
            }

            return View(viewModel);
        }

        [Authorize(Roles = "Owner")]
        public ActionResult ServiceCars()
        {
            //string userId = User.Identity.GetUserId();


            List<RentedCarsViewModel> viewModel = new List<RentedCarsViewModel>();

            DateTime date = DateTime.Today;
           

            var owner = db.Users.FirstOrDefault(u => u.Email=="user@test.com");
            List<RentedCars> Rented = db.rentals.Where(a => (a.UserId == owner.Id)).OrderByDescending(p => p.DateStart).ToList();

            foreach (var item in Rented)
            {
                var car = new Car();
                car = db.cars.Find(item.CarId);
                if (car != null)
                {
                    RentedCarsViewModel element = new RentedCarsViewModel(car, false, item.DateStart, item.DateEnd, item.Id);

                    if ((DateTime.Compare(item.DateStart, date) <= 0) && (DateTime.Compare(item.DateEnd, date) >= 0))
                    {
                        element.IsRented = true;

                    }
                    viewModel.Add(element);

                }


            }

            return View(viewModel);
        }
        // GET: Cars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.cars.Find(id);

            

            if (car == null)
            {
                return HttpNotFound();
            }
            List<Review> reviews = db.reviews.Where(a => a.CarId == id).OrderByDescending(p => p.Id).ToList();

            DetailsViewModel model = new DetailsViewModel(car, reviews);
            
            return View(model);
        }
        int DaysBetween(DateTime d1, DateTime d2)
        {
            TimeSpan span = d2.Subtract(d1);
            return (int)span.TotalDays;
        }
        public JsonResult RentServiceFunction(RentData data)
        {
            string userId = User.Identity.GetUserId();

            if (userId == null)
            {
                string m = "Please log in for this action.";

                return Json(new JsonMessage(m));
            }
          

            DateTime Start = DateTime.Parse(data.start);
            DateTime End = DateTime.Parse(data.end);
            if (DateTime.Compare(Start, End) > 0)
            {
                string m = "Dates were incorect.";

                return Json(new JsonMessage(m));
            }
            int count = db.rentals.Where(x => x.CarId == data.carId && (
            (Start <= x.DateStart && End >= x.DateStart) ||
            (Start <= x.DateEnd && End >= x.DateEnd) ||
            (Start >= x.DateStart && End <= x.DateEnd)
        )).Count();

            if (count > 0)
            {
                return Json(new JsonMessage("Car is not avalible"));
            }
            RentedCars rental = new RentedCars();
            rental.CarId = data.carId;
            rental.UserId = userId;
            rental.DateStart = DateTime.Parse(data.start);
            rental.DateEnd = DateTime.Parse(data.end);

            int diffOfDates = DaysBetween(rental.DateStart, rental.DateEnd) + 1;
            Car car = db.cars.Find(rental.CarId);
            int price = diffOfDates * car.Price;

            var row = db.balance.FirstOrDefault(b => b.UserId == userId);
            var rowOwner = db.balance.FirstOrDefault(b => b.Id == 1);
            if (car.NumberOfRentals == 0)
            {
                Console.WriteLine("Car needs to go to service..");
                return Json(new JsonMessage("Car needs to go to service."));
            }
            if (row != null && row.MoneyBalance >= price)
            {
                
                    row.MoneyBalance -= price;
                    rowOwner.MoneyBalance += price;

                    car.NumberOfRentals -= 1;
                    car.NumderOfTimesRented += 1;

                    DateTime time = DateTime.Now;
                    Transaction t = new Transaction("Rented Car", price, time, userId, rowOwner.UserId);

                    db.transactions.Add(t);
                    db.SaveChanges();
                
              
                
                Console.WriteLine("Updated successfully.");
            }
            else
            {
                Console.WriteLine("Not enough balance or user not found.");

                return Json(new JsonMessage("Not enough balance or user not found."));
            }

            db.rentals.Add(rental);
            db.SaveChanges();

            string mess = "Successfully rented car.";

            return Json(new JsonMessage(mess));
        }

        [HttpPost]
        public JsonResult RentCar(RentData data)
        {
            if (data == null || data.start == null || data.start == "" || data.end == null || data.end == "")
            {
                return Json(new JsonMessage("Please fill out all fields."));
            }

            DateTime d1 = DateTime.ParseExact(data.start, "dd/MM/yyyy", null);
            DateTime d2 = DateTime.ParseExact(data.end, "dd/MM/yyyy", null);

            data.start = d1.ToString("MM/dd/yyyy h:mm tt");
            data.end = d2.ToString("MM/dd/yyyy h:mm tt");

            var json = RentServiceFunction(data);


            return json;
        }


        [HttpPost]
        [Authorize(Roles = "Owner,Admin")]
        public JsonResult ServiceCar(ServiceData data)
        {
            string userId = User.Identity.GetUserId();
   

            if (userId == null)
            {
                string m = "Please log in for this action.";

                return Json(new JsonMessage(m));
            }
            if (data == null || data.carId == null)
            {
                return Json(new JsonMessage("Unable to process data."));
            }
            int i = 0;
            int c = 10;
            int price = 200;
            Car car = db.cars.Find(data.carId);
            if (car.NumberOfRentals == 10)
            {
                return Json(new JsonMessage("Your car doesn't need servicing"));
            }

            if (car.NumberOfRentals <= 2&&data.typeOfService != "major")
            {
                return Json(new JsonMessage("You must do major service on this car"));
            }
            if(data.typeOfService == "major")
            {
                i = 2;//it incluses today
                //c = 10;
                price = 500;
            }
            var rowOwner = db.balance.FirstOrDefault(b => b.Id == 1);

            RentedCars renatlData = new RentedCars();

            var today = DateTime.Today;

            renatlData.CarId = data.carId;
            renatlData.DateStart = today;
            renatlData.DateEnd = today.AddDays(i);

            int count = db.rentals.Where(x => x.CarId == data.carId &&(
            (renatlData.DateStart <= x.DateStart && renatlData.DateEnd >= x.DateStart) ||
            (renatlData.DateStart <= x.DateEnd && renatlData.DateEnd >= x.DateEnd) ||
            (renatlData.DateStart >= x.DateStart && renatlData.DateEnd <= x.DateEnd)
        )).Count();

            if (count > 0)
            {
                return Json(new JsonMessage("Car is not avalible"));
            }
            renatlData.UserId = rowOwner.UserId;

            car.NumberOfRentals = c;
            rowOwner.MoneyBalance -= price;

           
            DateTime time = DateTime.Now;
            string s = FirstCharToUpper(data.typeOfService);
            Transaction t = new Transaction(s+" service on car", price, time, rowOwner.UserId, "");
           

            db.rentals.Add(renatlData);
            db.transactions.Add(t);
            db.SaveChanges();

            
            
            string mess = "Successfully serviced car. Car servicing will take "+(i+1)+ " day" + (i == 0 ? "" : "s");

            return Json(new JsonMessage(mess));

        }

            [HttpPost]
        public JsonResult AddReview(Data data)
        {

            string userId = User.Identity.GetUserId();
            

            if (userId == null)
            {
                string m = "Please log in for this action.";
                
                return Json(new JsonMessage(m));
            }
            if (User.IsInRole("Owner"))
            {
                string m = "Owners can't submit review.";

                return Json(new JsonMessage(m));
            }
            if (data.comment == null || data.comment=="")
            {
                string m = "Please fill out all fields.";

                return Json(new JsonMessage(m));
            }
            int count = db.rentals.Count(x=>x.CarId==data.carId&&x.UserId==userId&&x.DateEnd<=DateTime.Today);
            if (count <= 0)
            {
                string m = "Only users that previously rented this car can leave a review.";

                return Json(new JsonMessage(m));
            }
            Review review = new Review();
            review.CarId = data.carId;
            review.Grade = data.grade;
            review.Comment = data.comment;
            review.UserId = userId;
            review.UserName = User.Identity.GetUserName();
            db.reviews.Add(review);
            db.SaveChanges();

            return Json(review);
        }


        
       [HttpPost]
        public JsonResult CarDates(GetDateData data)
        {
            if (data == null || data.carId == null)
            {
                return Json(new JsonMessage("Unable to process data."));
            }
            List<RentedCars> list = db.rentals.Where(x => x.CarId == data.carId).ToList();
            var dates = new List<string>();
            DateTime date = DateTime.Today;
            foreach (var x in list)
            {
            for (var dt = x.DateStart; dt <= x.DateEnd; dt = dt.AddDays(1))
                 {

                    if (dt >= date)
                    {
                        dates.Add(dt.ToString("dd/MM/yyyy"));
                    }
                    
                }
            }
            
            return Json(dates);
        }
        // GET: Cars/Create
        [Authorize(Roles = "Owner,Admin")]
        public ActionResult Create()
        {
            ViewBag.CarTypes = carTypes;
            ViewBag.FuelTypes = fuelTypes;
            ViewBag.NumDoors = numDoors;
            ViewBag.Transmissions = transmission;
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Brand,Model,Year,Image,NumberOfRentals,Price,Power,Consumption,FuelType,NumberOfSeats,NumberOfDoors,CarType,Capacity,Transmission")] Car car)
        {
           
            if (ModelState.IsValid)
            {
                if (car.FuelType == "Electric")
                {
                    car.Transmission = "No transmission";
                }
                car.NumberOfRentals = 10;
                db.cars.Add(car);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.CarTypes = carTypes;
            ViewBag.FuelTypes = fuelTypes;
            ViewBag.NumDoors = numDoors;
            ViewBag.Transmissions = transmission;
            return View(car);
        }

        // GET: Cars/Edit/5
        [Authorize(Roles = "Owner,Admin")]
        public ActionResult Edit(int? id)
        {
            ViewBag.FuelTypes = fuelTypes;
            ViewBag.CarTypes = carTypes;
            ViewBag.NumDoors = numDoors;
            ViewBag.Transmissions = transmission;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Brand,NumberOfRentals,Model,Year,Image,Price,Power,Consumption,FuelType,NumberOfSeats,NumberOfDoors,CarType,Capacity,Transmission,NumderOfTimesRented")] Car car)
        {
            ViewBag.FuelTypes = fuelTypes;
            ViewBag.CarTypes = carTypes;
            ViewBag.NumDoors = numDoors;
            ViewBag.Transmissions = transmission;
            if (ModelState.IsValid)
            {
                if (car.FuelType == "Electric")
                {
                    car.Transmission = "No transmission";
                }
                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(car);
        }

        // GET: Cars/Delete/5
        [Authorize(Roles = "Owner, Admin")]
        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Car car = db.cars.Find(id);
            db.cars.Remove(car);
            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }

        [HttpPost]
        public ActionResult DeleteRental(int id)
        {
            RentedCars rental = db.rentals.Find(id);
            db.rentals.Remove(rental);
            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
