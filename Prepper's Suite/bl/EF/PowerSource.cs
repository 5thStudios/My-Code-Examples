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
    
    public partial class PowerSource
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PowerSource()
        {
            this.Items = new HashSet<Item>();
        }
    
        public int PowerSourceId { get; set; }
        public Nullable<int> BatteriesId { get; set; }
        public Nullable<int> FuelId { get; set; }
        public bool RequiresAC { get; set; }
        public bool RequiresBatteries { get; set; }
        public bool RequiresFuel { get; set; }
    
        public virtual Battery Battery { get; set; }
        public virtual Fuel Fuel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item> Items { get; set; }
    }
}
