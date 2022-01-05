using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using bl.EF;

namespace ScrumTests.Models
{
    public class ScrumColumn
    {
        public string ColumnColor { get; set; }  //***Delete this.  apply in css only

        public string ColumnName { get; set; }
        public int ColumnId { get; set; }
        public List<ScrumCard> LstScrumCards { get; set; }



        public ScrumColumn() {
            LstScrumCards = new List<ScrumCard>();
        }
    }
}