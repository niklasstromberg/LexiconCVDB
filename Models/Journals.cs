namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Journals
    {
        [Key]
        public int Journal_ID { get; set; }

        public int User_ID { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime Created { get; set; }

        public int? Lexicon_Handle_ID { get; set; }

        [StringLength(1024)]
        public string Notes { get; set; }

        public virtual Lexicon_Handles Lexicon_Handles { get; set; }

        public virtual Users Users { get; set; }
    }
}
