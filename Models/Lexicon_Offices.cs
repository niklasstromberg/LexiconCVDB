namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Lexicon_Offices
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lexicon_Offices()
        {
            Lexicon_Handles = new HashSet<Lexicon_Handles>();
            User_Details = new HashSet<User_Details>();
        }

        [Key]
        public int Lexicon_Office_ID { get; set; }

        [StringLength(50)]
        public string Office_Name { get; set; }

        public int Location_ID { get; set; }

        public int? Adress_ID { get; set; }

        public virtual Adresses Adresses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lexicon_Handles> Lexicon_Handles { get; set; }

        public virtual Locations Locations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User_Details> User_Details { get; set; }
    }
}
