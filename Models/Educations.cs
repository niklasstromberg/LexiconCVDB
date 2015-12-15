namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Educations
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Educations()
        {
            User_EDU_REL = new HashSet<User_EDU_REL>();
        }

        [Key]
        public int EDU_ID { get; set; }

        [StringLength(100)]
        public string School { get; set; }

        [StringLength(100)]
        public string Course { get; set; }

        [StringLength(100)]
        public string Degree { get; set; }

        public short Year { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User_EDU_REL> User_EDU_REL { get; set; }
    }
}
