using Microsoft.AspNetCore.Html;
using www.Models;

namespace www.ViewModels
{
    public class EfficiencyScheduleViewModel
    {
        public string? Bootcamp { get; set; }
        public string? Category { get; set; }
        public string? DateRange { get; set; }
        public string? TimeFrame { get; set; }
    }
}
