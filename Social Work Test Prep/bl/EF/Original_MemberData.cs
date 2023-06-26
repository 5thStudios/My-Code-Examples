namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Original_MemberData
    {
        public int Id { get; set; }

        public int MemberId { get; set; }

        [Required]
        [StringLength(150)]
        public string Email { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
