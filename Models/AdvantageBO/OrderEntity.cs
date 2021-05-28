using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlockChainPayment.Models.AdvantageBO
{
    public class OrderEntity
    {
        public long Id { get; set; }
        public string OrderId { get; set; }
        public Nullable<long> UserId { get; set; }
        public string UserEmail { get; set; }
        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<decimal> TotalCost { get; set; }
        public string PaidTransactionVia { get; set; }
        public string PaidTransactionId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}