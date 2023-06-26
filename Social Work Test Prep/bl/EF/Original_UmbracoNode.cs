namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Original_UmbracoNode
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        public int parentID { get; set; }

        public short level { get; set; }

        [StringLength(255)]
        public string text { get; set; }

        public DateTime createDate { get; set; }

        [Required]
        [StringLength(150)]
        public string path { get; set; }

        public bool isAddedToSite { get; set; }
    }
}
