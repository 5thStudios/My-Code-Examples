using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class MasterClass
    {
        public About About { get; set; }
        public Contact Contact { get; set; }
        public Introduction Introduction { get; set; }
        public Portfolio Portfolio { get; set; }
        public Skills Skills { get; set; }
        public SidePnl SidePnl { get; set; }
        public Thankyou Thankyou { get; set; }



        public MasterClass()
        {
            //About = new About();
            //Contact = new Contact();
            //Introduction = new Introduction();
            //Portfolio = new Portfolio();
            //Skills = new Skills();
            //SidePnl = new SidePnl();
            //Thankyou = new Thankyou();
        }
    }
}
