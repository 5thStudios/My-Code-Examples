using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models
{
    public class Pagination
    {
        public int pageNo { get; set; }
        public int previous { get; set; }
        public int next { get; set; }
        public int itemsPerPage { get; set; }
        public int totalPages { get; set; }
        public int totalItems { get; set; }
        public int itemsToSkip { get; set; }
    }
}
