namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Employment_Histories
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employment_Histories()
        {
            EMP_HIS_Tag_REL = new HashSet<EMP_HIS_Tag_REL>();
        }

        [Key]
        public int EMP_HIS_ID { get; set; }

        public int User_ID { get; set; }

        public int? Company_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Position { get; set; }

        [StringLength(50)]
        public string Department { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        [Column(TypeName = "date")]
        public DateTime From_Date { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Until_Date { get; set; }

        public bool Curr_Emp { get; set; }

        public int? REF_ID { get; set; }

        [StringLength(300)]
        public string Notes { get; set; }

        public virtual Companies Companies { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMP_HIS_Tag_REL> EMP_HIS_Tag_REL { get; set; }

        public virtual User_References User_References { get; set; }

        public virtual Users Users { get; set; }
    }
}
