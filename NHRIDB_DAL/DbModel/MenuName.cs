//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NHRIDB_DAL.DbModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class MenuName
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MenuName()
        {
            this.Permissions = new HashSet<Permissions>();
        }
    
        public int menuId { get; set; }
        public string menuText { get; set; }
        public string controllerName { get; set; }
        public bool enable { get; set; }
        public Nullable<int> parentMenu { get; set; }
        public int sortIndex { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Permissions> Permissions { get; set; }
    }
}
