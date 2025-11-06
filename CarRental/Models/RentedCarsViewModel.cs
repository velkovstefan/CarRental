using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarRental.Models
{
    public class RentedCarsViewModel
    {
        public RentedCarsViewModel(Car car, bool isRented, DateTime dateStart, DateTime dateEnd, int rentId) { 
            Car = car;
            IsRented = isRented;
            DateStart = dateStart;
            DateEnd = dateEnd;
            RentId = rentId;
        }
        public Car Car { get; set; }
        public int RentId { get; set; }
        public bool IsRented { get; set; }
        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }
    }
}