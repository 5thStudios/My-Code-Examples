namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Original_PurchaseRecords
    {
        public int id { get; set; }

        public Guid? originalId { get; set; }

        [StringLength(50)]
        public string txn_id { get; set; }

        public DateTime? created { get; set; }

        [StringLength(150)]
        public string payerEmail { get; set; }

        public int? memberId { get; set; }

        [StringLength(100)]
        public string itemName { get; set; }

        public int? itemNumber { get; set; }

        public decimal? price { get; set; }

        public decimal? discount { get; set; }

        public bool internalPurchase { get; set; }

        public bool isAddedToSite { get; set; }
    }
}
