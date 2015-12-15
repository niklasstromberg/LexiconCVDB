namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User_EDU_REL
    {
        [Key]
        public int User_EDU_ID { get; set; }

        public int User_ID { get; set; }

        public int EDU_ID { get; set; }

        public virtual Educations Educations { get; set; }

        public virtual Users Users { get; set; }
    }
}
