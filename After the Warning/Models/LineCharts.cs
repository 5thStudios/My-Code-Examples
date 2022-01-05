using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;
using ContentModels = Umbraco.Web.PublishedModels;


namespace Models
{
    public class LineCharts
    {
        public List<string> LstAgeRange { get; set; }
        public string jsonAgeRange { get; set; }
        public List<ChartDataset> LstChartData { get; set; }
        public int TotalEntries { get; set; }

        public List<LineChart> LstAgeStats_Heavenly { get; set; }
        public List<LineChart> LstAgeStats_Hellish { get; set; }
        public List<LineChart> LstAgeStats_Purgatorial { get; set; }
        public List<LineChart> LstAgeStats_Unknown { get; set; }

        public LineCharts()
        {
            LstAgeRange = new List<string>();
            LstChartData = new List<ChartDataset>();
            LstAgeStats_Heavenly = new List<LineChart>();
            LstAgeStats_Hellish = new List<LineChart>();
            LstAgeStats_Purgatorial = new List<LineChart>();
            LstAgeStats_Unknown = new List<LineChart>();
        }
    }
}