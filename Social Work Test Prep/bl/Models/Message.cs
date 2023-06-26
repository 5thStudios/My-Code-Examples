using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models
{
    public class Message
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string MessageText { get; set; }
        public bool IsValid { get; set; }


        public Message() { }
    }
}
