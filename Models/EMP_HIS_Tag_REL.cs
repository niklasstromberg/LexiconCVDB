namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EMP_HIS_Tag_REL
    {
        [Key]
        public int EMP_HIS_Tag_ID { get; set; }

        public int EMP_HIS_ID { get; set; }

        public int Tag_ID { get; set; }

        public virtual Employment_Histories Employment_Histories { get; set; }

        public virtual Tags Tags { get; set; }
    }
}
