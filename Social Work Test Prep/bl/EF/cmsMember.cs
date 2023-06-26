namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cmsMember")]
    public partial class cmsMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int nodeId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Email { get; set; }

        [Required]
        [StringLength(1000)]
        public string LoginName { get; set; }

        [Required]
        [StringLength(1000)]
        public string Password { get; set; }
    }
}
