using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public partial class PurchaseRecord
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string ExtensionRequest { get; set; }


        public PurchaseRecord() { }
    }
}
