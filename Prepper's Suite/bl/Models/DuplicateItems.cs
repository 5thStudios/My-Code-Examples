using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models
{
    public class DuplicateItems
    {
        public EF.Category Category { get; set; }
        public EF.Location Location { get; set; }
        public string ItemName { get; set; }
        public int Count { get; set; }
        public bool IsCompleted { get; set; }
        public List<bl.EF.Item> LstItems { get; set; }


        public DuplicateItems()
        {
            LstItems = new List<EF.Item>();
        }
    }
}
