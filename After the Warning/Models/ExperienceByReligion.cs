﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class ExperienceByReligion
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("heavenly")]
        public int Heavenly { get; set; }

        [JsonProperty("hellish")]
        public int Hellish { get; set; }

        [JsonProperty("purgatorial")]
        public int Purgatorial { get; set; }

        [JsonProperty("other")]
        public int Other { get; set; }



        public ExperienceByReligion() { }
        public ExperienceByReligion(string _label)
        {
            Label = _label;
            Heavenly = 0;
            Hellish = 0;
            Purgatorial = 0;
            Other = 0;
        }
    }
}