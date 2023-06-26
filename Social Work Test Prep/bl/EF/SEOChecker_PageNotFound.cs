namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SEOChecker_PageNotFound
    {
        [Key]
        public int NotFoundId { get; set; }

        [Required]
        [StringLength(255)]
        public string Url { get; set; }

        public int DocumentID { get; set; }

        public bool Ignore { get; set; }

        public int TimesAccessed { get; set; }

        public DateTime LastTimeAccessed { get; set; }

        [Required]
        [StringLength(255)]
        public string RedirectUrl { get; set; }

        [StringLength(255)]
        public string Referer { get; set; }

        [Required]
        [StringLength(255)]
        public string ContentType { get; set; }

        [Required]
        [StringLength(255)]
        public string Domain { get; set; }

        [Required]
        [StringLength(255)]
        public string QueryString { get; set; }

        public bool WildCard { get; set; }
    }
}
