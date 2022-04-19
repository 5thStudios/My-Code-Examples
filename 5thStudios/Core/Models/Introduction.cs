using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Introduction
    {
        public string FullName { get; set; }
        public string CareerTitle { get; set; }
        public string IntroImgUrl { get; set; }



        public Introduction()
        {
            //FullName = "Jim Fifth";
            //CareerTitle = "Full Stack Umbraco Expert";
            //IntroImgUrl = "/media/1062/ag_jimfifth_portrait.png?crop=0,0.1079393398751123,0.10793933987511231,0&cropmode=percentage&width=804&height=990&rnd=131949609840000000";
        }
    }
}
