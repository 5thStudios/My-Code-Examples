using System.Collections.Generic;

namespace Models
{
    public class MsgList
    {
        public int? VisionaryId { get; set; }
        public string VisionaryName { get; set; }
        public List<MsgLink> lstMsgLinks { get; set; } 
        public Pagination Pagination { get; set; } 


        public MsgList()
        {
            lstMsgLinks = new List<MsgLink>();
            Pagination = new Pagination();
        }
    }
}