using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRental.Models
{
    public class DetailsViewModel
    {
        public DetailsViewModel(Car car, List<Review> reviews) {
            Car = car;
            Reviews = reviews;
        }

        public Car Car { get; set; }
        public List<Review> Reviews { get; set; }
    }
}