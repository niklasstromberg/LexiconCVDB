namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User_Details
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int User_ID { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime Created { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime Modified { get; set; }

        public bool Available { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Available_Date { get; set; }

        public bool Active { get; set; }

        public int? Lexicon_Office_ID { get; set; }

        public int? Lexicon_Handle_ID { get; set; }

        public int? Salary { get; set; }

        public virtual Lexicon_Handles Lexicon_Handles { get; set; }

        public virtual Lexicon_Offices Lexicon_Offices { get; set; }

        public virtual Users Users { get; set; }
    }
}
