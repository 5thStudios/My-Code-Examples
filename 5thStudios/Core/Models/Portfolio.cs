using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Portfolio
    {
        public List<Project> LstProjects { get; set; }




        public Portfolio()
        {
            LstProjects = new List<Project>();

            //Test project 01
            Project project = new Project();

            //project.ProjectLink = new Link("NAHUvision", "https://videos.nahu.org/");
            //project.ScreenshotUrl = "/media/1155/01.jpg?crop=0.13026315789473683,0,0.14605263157894743,0&cropmode=percentage&width=700&height=475&rnd=132349649980000000";
            //project.Summary = "<p>The newest addition to the NAHU Umbraco website family, this project was very exciting to build.  Preparing for launch within the next few weeks, this site is designed to provide their users with a library of all company videos stored on Vimeo.  Videos are organized by categories allowing for easy filtering and allows management to designate if a video is public or private, thus requiring users to login before viewing.  Once a video has been completed, management can download a full list of all members and the videos they had fully watched.</p>";
       
            //project.LstCarouselShots.Add(new Link("", "/media/1155/01.jpg?crop=0.11933366560785157,0,0.11352134119881689,0.029212881340802136&cropmode=percentage&width=700&height=435&rnd=132349649980000000", "img 01"));
            //project.LstCarouselShots.Add(new Link("", "/media/1156/02.jpg?crop=0,0.15089285714285716,0.0000000000000001263187085796,0&cropmode=percentage&width=700&height=435&rnd=132349650200000000", "img 02"));

            //LstProjects.Add(project);


            ////Test project 01
            //project = new Project();

            //project.ProjectLink = new Link("NAHU", "https://nahu.org/");
            //project.ScreenshotUrl = "/media/1161/01.jpg?crop=0.012879025464115951,0,0.26267579217936177,0&cropmode=percentage&width=700&height=475&rnd=132349646600000000";
            //project.Summary = "<p>The newest addition to the NAHU Umbraco website family, this project was very exciting to build.  Preparing for launch within the next few weeks, this site is designed to provide their users with a library of all company videos stored on Vimeo.  Videos are organized by categories allowing for easy filtering and allows management to designate if a video is public or private, thus requiring users to login before viewing.  Once a video has been completed, management can download a full list of all members and the videos they had fully watched.</p>";

            //project.LstCarouselShots.Add(new Link("", "/media/1155/01.jpg?crop=0.11933366560785157,0,0.11352134119881689,0.029212881340802136&cropmode=percentage&width=700&height=435&rnd=132349649980000000", "img 01"));
            //project.LstCarouselShots.Add(new Link("", "/media/1156/02.jpg?crop=0,0.15089285714285716,0.0000000000000001263187085796,0&cropmode=percentage&width=700&height=435&rnd=132349650200000000", "img 02"));

            //LstProjects.Add(project);
        }
    }
}
