using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models
{
    public class PaginationContent
    {
        public Uri baseUri { get; set; }
        public NameValueCollection queryString { get; set; }
        public string baseUrl { get; set; }
        public Pagination Pagination { get; set; }

        //public Int64 TotalResults { get; set; }
        //public int CurrentPg { get; set; }
        //public int TotalPgs { get; set; }

    }
}
