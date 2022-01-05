using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Contact
    {
        public string Description { get; set; }
        public string BackgroundImgUrl { get; set; }
        public string BackgroundTransparency { get; set; }
        public string ServiceAreas { get; set; }
        public string Email { get; set; }
        public Link LnkPhone { get; set; }

        public string RecaptchaPublicKey { get; set; }
        public string RecaptchaPrivateKey { get; set; }
    }
}