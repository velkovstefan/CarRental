using CarRental.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace CarRental.Controllers
{
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Transactions
        [Authorize]
         public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.UserId = userId;
            bool isOwner = User.IsInRole("Owner");
            List<TransactionViewModel> transactions = new List<TransactionViewModel>();
            List<Transaction> list = null;
            var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

             var owners = db.Users.Where(u => u.Roles.Any(r => r.RoleId == "0")).ToList();
            List<String> listOwners = new List<String>();
            foreach (var user in owners) { 
            listOwners.Add(user.Id);
            }
             ViewBag.Owners = listOwners;

            if (isOwner)
            {
                list = db.transactions.OrderByDescending(p => p.Id).ToList();
            }
            else
            {
                list = db.transactions.OrderByDescending(p => p.Id).Where(t => t.UserIdTo.Equals(userId) || t.UserIdFrom.Equals(userId)).ToList();
            }
                
            foreach (var item in list)
            {
            

                var userFrom =  UserManager.FindById(item.UserIdFrom);
                var userTo = UserManager.FindById(item.UserIdTo);
                
                if(item.Reason == "Added Balance")
                {
                    transactions.Add(new TransactionViewModel(item, userTo.UserName.ToString(), ""));
                    continue;
                }
                else if (item.Reason == "Major service on car" || item.Reason == "Minor service on car")
                { 
                    transactions.Add(new TransactionViewModel(item, "Service", userFrom.UserName.ToString()));
                    continue;
                }

                    if (userTo != null && userFrom!=null)
                {
                    transactions.Add(new TransactionViewModel(item, userTo.UserName.ToString(), userFrom.UserName.ToString()));
                }
                
            }
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }


        // GET: Transactions/Delete/5
        [Authorize(Roles = "Owner, Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [Authorize(Roles="Owner, Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.transactions.Find(id);
            db.transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult AddBalance()
        {
            Balance model = new Balance();
            model.UserId = User.Identity.GetUserId();
            
            ViewBag.MoneyBalance = 0;
            bool isOwner = User.IsInRole("Owner");
            Balance balance;
            if (isOwner)
            {
                balance = db.balance.FirstOrDefault(b => b.Id == 1);
            }
            else
            {
                balance = db.balance.FirstOrDefault(b => b.UserId == model.UserId);
            }
                 
            if (balance != null)
            {
                ViewBag.MoneyBalance = balance.MoneyBalance;
            }

            return View(model);

        }
        [Authorize]
        [HttpPost]
        public ActionResult AddBalance(Balance model)
        {
            if (ModelState.IsValid)
            {
                string userId = model.UserId;
                

                if (model.MoneyBalance <= 0)
                {
                    ViewBag.MoneyBalance = 0;
                    return RedirectToAction("AddBalance");
                   
                }
                bool isOwner = User.IsInRole("Owner");
                Balance balance;
                if (isOwner)
                {
                    balance = db.balance.FirstOrDefault(b => b.Id == 1);
                }
                else
                {
                    balance = db.balance.FirstOrDefault(b => b.UserId == model.UserId);
                }

                if (balance == null)
                {
                    Balance newBalance = new Balance();
                    newBalance.UserId = userId;
                    newBalance.MoneyBalance = model.MoneyBalance;
                    ViewBag.MoneyBalance = newBalance.MoneyBalance;
                    db.balance.Add(newBalance);
                }
                else
                {
                    balance.MoneyBalance += model.MoneyBalance;
                    ViewBag.MoneyBalance = balance.MoneyBalance;
                }
                db.transactions.Add(new Transaction("Added Balance", model.MoneyBalance, DateTime.Now, "", userId));
                db.SaveChanges();
            }
            model.MoneyBalance = 0;
            return View(model);

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
