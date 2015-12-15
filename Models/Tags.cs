namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Tags
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tags()
        {
            EMP_HIS_Tag_REL = new HashSet<EMP_HIS_Tag_REL>();
            User_Tag_REL = new HashSet<User_Tag_REL>();
        }

        [Key]
        public int Tag_ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Tag_Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMP_HIS_Tag_REL> EMP_HIS_Tag_REL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User_Tag_REL> User_Tag_REL { get; set; }
    }
}
