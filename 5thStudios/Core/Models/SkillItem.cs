using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class SkillItem
    {
        public int Percentage { get; set; }
        public string Skill { get; set; }
        public string ExperienceLevel { get; set; }




        public SkillItem() { }
        public SkillItem(int _Percentage = 0, string _Skill = "")
        {
            Percentage = _Percentage;
            Skill = _Skill;
        }
        public SkillItem(int _Percentage = 0, string _Skill = "", string _ExperienceLevel = "")
        {
            Percentage = _Percentage;
            Skill = _Skill;
            ExperienceLevel = _ExperienceLevel;
        }
    }
}
