using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRental.Models
{
    public class CarsViewModel
    {
        public CarsViewModel(Car car, bool isRented) { 
            Car = car;
            IsRented = isRented;
        }
        public Car Car { get; set; }
        public bool IsRented { get; set; }
        
    }
}