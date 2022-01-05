using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace bl.EF
{
    public partial class ScrumCard
    {
        public string CompletionDateString { get; set; }


        public List<ScrumChecklist> LstScrumChecklist { get; set; }
        public string ChecklistStatus { get; set; }
        public int TotalChecklistItems { get; set; }
        public int CompletedChecklistItems { get; set; }


        public List<ScrumActivity> LstScrumActivity { get; set; }
        public string ActivityStatus { get; set; }




        public ScrumCard(Boolean InitializeLists)
        {
            if (InitializeLists)
            {
                LstScrumChecklist = new List<ScrumChecklist>();
                LstScrumActivity = new List<ScrumActivity>();
            }
        }
        public ScrumCard(int _CardId, int _SortId, int _ColumnId, string _CardName)
        {
            SortId = _SortId;
            CardId = _CardId;
            StatusId = _ColumnId;
            Title = _CardName.Trim();
            LstScrumChecklist = new List<ScrumChecklist>();
            LstScrumActivity = new List<ScrumActivity>();
        }
    }
}
