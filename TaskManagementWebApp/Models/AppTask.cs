//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaskManagementWebApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AppTask
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AppTask()
        {
            this.AppNotification = new HashSet<AppNotification>();
        }
    
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public int Status { get; set; }
        public Nullable<int> AssignedToUserId { get; set; }
        public int CreatedByUserId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AppNotification> AppNotification { get; set; }
        public virtual AppUser AppUser { get; set; }
        public virtual AppUser AppUser1 { get; set; }
    }
}
