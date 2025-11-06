using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarRental.Models
{
    public class RentedCars
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int CarId { get; set; }

        [Required]
        public DateTime DateStart{ get; set; }

        [Required]
        public DateTime DateEnd { get; set; }
        

    }
}