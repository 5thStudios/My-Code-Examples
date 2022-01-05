using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using bl.EF;

namespace ScrumTests.Models
{
    public class AllScrumData
    {
        public List<ScrumColumn> LstScrumColumns { get; set; }
        public List<ScrumCard> LstPredefinedScrumCards { get; set; }
        public ScrumCard VirtualScrumCard { get; set; }
        public bool AddCard { get; set; }
        public int DeleteCardId { get; set; }



        public AllScrumData()
        {
            LstScrumColumns = new List<ScrumColumn>();
            LstPredefinedScrumCards = new List<ScrumCard>();
            VirtualScrumCard = new ScrumCard();
            DeleteCardId = -1;
        }

    }
}