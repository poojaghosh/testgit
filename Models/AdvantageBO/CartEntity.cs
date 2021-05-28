using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCPAdvantageCart.Models.AdvantageBO
{
    public class CartEntity
    {
        public long Id { get; set; }
        public Nullable<long> UserId { get; set; }
        public string UserEmail { get; set; }
        public string ProductSKU { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> UnitRate { get; set; }
        public Nullable<decimal> UnitTotalCost { get; set; }
        public Nullable<decimal> MSRP { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}