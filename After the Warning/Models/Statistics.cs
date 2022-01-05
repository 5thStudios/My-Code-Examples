using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;

namespace Models
{
    public class Statistics
    {
        public StatsByAge statsByAge { get; set; }



        public Statistics(UmbracoHelper Umbraco, Boolean useTestData = false)
        {
            statsByAge = new StatsByAge(Umbraco, useTestData);
        }
    }
}