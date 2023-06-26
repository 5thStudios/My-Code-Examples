using System;
using System.Collections.Generic;


namespace bl.Models.api
{
    public class MemberData
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime CreateDateTime { get; set; }


        public MemberData() { }
    }
}



/*
    Used when calling
    http://api.swtp.localhost/Services/apiMigrateData.asmx/MigrateExams
 */