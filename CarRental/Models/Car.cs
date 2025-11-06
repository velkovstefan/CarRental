using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarRental.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Please Enter Brand")]
        public String Brand { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int Year { get; set; }

        public String Image { get; set; }

        public int NumberOfRentals { get; set; }
        [Required]
        public int Price { get; set; }


        [Required]
        public int Power { get; set; }
        [Required]
        public int Consumption { get; set; }
      
        public string FuelType { get; set; }
        [Required]
        public int NumberOfSeats { get; set; }
        
        public int NumberOfDoors{ get; set; }
        
        public string CarType { get; set; } //Sedan,Coupe,Cabriolet
        [Required]
        public int Capacity { get; set; }

        // ne e required bidejkji elektricni avtomobili nemaat menuvac
        public string Transmission {  get; set; }

        public int NumderOfTimesRented { get; set; }


    }
}