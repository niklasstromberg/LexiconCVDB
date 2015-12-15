namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User_Skill_REL
    {
        [Key]
        public int User_Skill_ID { get; set; }

        public int User_ID { get; set; }

        public int Skill_ID { get; set; }

        public byte? User_Skill_EXP { get; set; }

        public virtual Skills Skills { get; set; }

        public virtual Users Users { get; set; }
    }
}
