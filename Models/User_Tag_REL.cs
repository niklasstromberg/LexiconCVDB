namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User_Tag_REL
    {
        [Key]
        public int User_Tag_ID { get; set; }

        public int User_ID { get; set; }

        public int Tag_ID { get; set; }

        public virtual Tags Tags { get; set; }

        public virtual Users Users { get; set; }
    }
}
