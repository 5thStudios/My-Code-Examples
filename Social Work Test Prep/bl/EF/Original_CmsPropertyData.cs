namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Original_CmsPropertyData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        public int contentNodeId { get; set; }

        public int propertytypeid { get; set; }

        public int? dataInt { get; set; }

        public DateTime? dataDate { get; set; }

        [StringLength(500)]
        public string dataNvarchar { get; set; }

        [Column(TypeName = "ntext")]
        public string dataNtext { get; set; }
    }
}
