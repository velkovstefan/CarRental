using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;
using static CarRental.Controllers.CarsController;

namespace CarRental.Models
{
    public class Transaction
    {
        public Transaction()
        {

        }
        public Transaction(string reason, int amount, DateTime date, string userIdFrom, string userIdTo)
        {
            Reason = reason;
            Amount = amount;
            Date = date;
            UserIdFrom = userIdFrom;
            UserIdTo = userIdTo;
        }

        [Key]
        public int Id { get; set; }

        public string Reason { get; set; }

        public int Amount { get; set; }

        public DateTime Date { get; set; }

        public string UserIdFrom { get; set; }
        public string UserIdTo { get; set; }
        
    }
}