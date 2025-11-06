using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarRental.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Grade { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int CarId { get; set; }
        public string Comment { get; set; }
    }
}