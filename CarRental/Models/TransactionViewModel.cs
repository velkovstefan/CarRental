using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRental.Models
{
    public class TransactionViewModel
    {
        public TransactionViewModel() { }

        public TransactionViewModel(Transaction transaction, string UserNameTo, string UserNameFrom) {

            Transaction = transaction;
            this.UserNameTo = UserNameTo;
            this.UserNameFrom = UserNameFrom;
        }

        public Transaction Transaction { get; set; }
        public string UserNameTo { get; set; }
        public string UserNameFrom { get; set; }
    }
}