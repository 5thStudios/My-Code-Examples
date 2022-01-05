using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace bl.EF
{
    public partial class Item
    {
        public Nullable<int> Quantity { get; set; }
        public string Barcode { get; set; }
        public string JsonData { get; set; }



        public Item()
        {
            //Set default values
            Quantity = 1;
        }
    }
}
