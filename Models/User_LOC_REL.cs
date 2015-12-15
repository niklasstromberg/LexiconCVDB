namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User_LOC_REL
    {
        [Key]
        public int User_LOC_ID { get; set; }

        public int User_ID { get; set; }

        public int Location_ID { get; set; }

        public virtual Locations Locations { get; set; }

        public virtual Users Users { get; set; }
    }
}
