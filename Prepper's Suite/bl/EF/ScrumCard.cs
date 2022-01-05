//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class ScrumCard
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ScrumCard()
        {
            this.ScrumChecklists = new HashSet<ScrumChecklist>();
            this.ScrumActivities = new HashSet<ScrumActivity>();
        }
    
        public int CardId { get; set; }
        public int AccountId { get; set; }
        public int ToolId { get; set; }
        public int StatusId { get; set; }
        public int SortId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CompletionDate { get; set; }
        public bool IsComplete { get; set; }
        public bool IsArchived { get; set; }
        public System.DateTime CreatedTimestamp { get; set; }
        public Nullable<System.DateTime> LastUpdatedTimestamp { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual ScrumStatusList ScrumStatusList { get; set; }
        public virtual Tool Tool { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScrumChecklist> ScrumChecklists { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScrumActivity> ScrumActivities { get; set; }
    }
}