using System;
using System.Collections.Generic;


namespace bl.Models.api
{
    public class PurchaseRecord
    {
        public Guid? OriginalId { get; set; }
        public string Txn_id { get; set; }
        public DateTime Created { get; set; }
        public string PayerEmail { get; set; }
        public string ItemName { get; set; }
        public string ItemNumber { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; }
        public bool Internal { get; set; }



        public PurchaseRecord() { }
    }
}