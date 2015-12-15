namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Users()
        {
            Employment_Histories = new HashSet<Employment_Histories>();
            Journals = new HashSet<Journals>();
            Logs = new HashSet<Logs>();
            User_EDU_REL = new HashSet<User_EDU_REL>();
            User_LOC_REL = new HashSet<User_LOC_REL>();
            User_Skill_REL = new HashSet<User_Skill_REL>();
            User_Tag_REL = new HashSet<User_Tag_REL>();
        }

        [Key]
        public int User_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string First_Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Last_Name { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date_of_Birth { get; set; }

        public int Adress_ID { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Mobile { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public byte? Swedish { get; set; }

        public byte? English { get; set; }

        public bool Drivers_Licence { get; set; }

        [StringLength(1024)]
        public string Personal_Information { get; set; }

        [StringLength(300)]
        public string Synopsis { get; set; }

        [StringLength(100)]
        public string LinkedIn { get; set; }

        [StringLength(200)]
        public string Photo { get; set; }

        public int? Salary_Interval_ID { get; set; }

        public virtual Adresses Adresses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employment_Histories> Employment_Histories { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Journals> Journals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Logs> Logs { get; set; }

        public virtual Salary_Intervals Salary_Intervals { get; set; }

        public virtual User_Details User_Details { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User_EDU_REL> User_EDU_REL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User_LOC_REL> User_LOC_REL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User_Skill_REL> User_Skill_REL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User_Tag_REL> User_Tag_REL { get; set; }
    }
}
