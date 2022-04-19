using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Strings;

namespace Core.Models
{
    public class Thankyou
    {
        public IHtmlEncodedString ResponseText { get; set; }
        public string SignatureImgUrl { get; set; }
        public List<String> LstMyAttributes { get; set; }



        public Thankyou()
        {
            //ResponseText = "Thank you so very much for reaching out to me. I will get back to you as soon as possible.";
            //FullName = "Jim Fifth";
            //SignatureImgUrl = "/media/1075/signature_white.png?crop=0.23131117293931031,0.27543771497139069,0.25864868690165921,0.30117340239694074&cropmode=percentage&width=200&height=107&rnd=132024877660000000";

            LstMyAttributes = new List<string>();
            //LstMyAttributes.Add("Always learning");
            //LstMyAttributes.Add("never quiting");
            //LstMyAttributes.Add("teachable");
            //LstMyAttributes.Add("hard working");
            //LstMyAttributes.Add("humble");
            //LstMyAttributes.Add("prayerful");
        }
    }
}