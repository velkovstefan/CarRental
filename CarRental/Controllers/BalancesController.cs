using CarRental.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;

namespace CarRental.Controllers
{
    public class BalancesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Balances
        public Balance Getbalance()
        {
            string userId = User.Identity.GetUserId();
            if (userId == null) {
                return null;
            }

            Balance balance = new Balance();
            bool isOwner = User.IsInRole("Owner");
           
            if (isOwner)
            {
                balance = db.balance.FirstOrDefault(b => b.Id == 1);
            }
            else
            {
                if (userId != null)
                {
                    balance = db.balance.FirstOrDefault(b => b.UserId == userId);
                }
            }


            return balance;
        }

       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BalanceExists(int id)
        {
            return db.balance.Count(e => e.Id == id) > 0;
        }
    }
}