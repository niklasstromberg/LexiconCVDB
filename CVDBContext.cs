namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CVDBContext : DbContext
    {
        public CVDBContext()
            : base("name=CVDBContext")
        {
        }

        public virtual DbSet<Adresses> Adresses { get; set; }
        public virtual DbSet<Companies> Companies { get; set; }
        public virtual DbSet<Educations> Educations { get; set; }
        public virtual DbSet<EMP_HIS_Tag_REL> EMP_HIS_Tag_REL { get; set; }
        public virtual DbSet<Employment_Histories> Employment_Histories { get; set; }
        public virtual DbSet<Journals> Journals { get; set; }
        public virtual DbSet<Lexicon_Handles> Lexicon_Handles { get; set; }
        public virtual DbSet<Lexicon_Offices> Lexicon_Offices { get; set; }
        public virtual DbSet<Locations> Locations { get; set; }
        public virtual DbSet<Log_Events> Log_Events { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<Salary_Intervals> Salary_Intervals { get; set; }
        public virtual DbSet<Skills> Skills { get; set; }
        public virtual DbSet<Tags> Tags { get; set; }
        public virtual DbSet<User_Details> User_Details { get; set; }
        public virtual DbSet<User_EDU_REL> User_EDU_REL { get; set; }
        public virtual DbSet<User_LOC_REL> User_LOC_REL { get; set; }
        public virtual DbSet<User_References> User_References { get; set; }
        public virtual DbSet<User_Skill_REL> User_Skill_REL { get; set; }
        public virtual DbSet<User_Tag_REL> User_Tag_REL { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Adresses>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Adresses)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Educations>()
                .HasMany(e => e.User_EDU_REL)
                .WithRequired(e => e.Educations)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employment_Histories>()
                .HasMany(e => e.EMP_HIS_Tag_REL)
                .WithRequired(e => e.Employment_Histories)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lexicon_Handles>()
                .HasMany(e => e.Journals)
                .WithRequired(e => e.Lexicon_Handles)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lexicon_Handles>()
                .HasMany(e => e.Logs)
                .WithRequired(e => e.Lexicon_Handles)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Locations>()
                .HasMany(e => e.Lexicon_Offices)
                .WithRequired(e => e.Locations)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Locations>()
                .HasMany(e => e.User_LOC_REL)
                .WithRequired(e => e.Locations)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Log_Events>()
                .HasMany(e => e.Logs)
                .WithRequired(e => e.Log_Events)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Skills>()
                .HasMany(e => e.User_Skill_REL)
                .WithRequired(e => e.Skills)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tags>()
                .HasMany(e => e.EMP_HIS_Tag_REL)
                .WithRequired(e => e.Tags)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tags>()
                .HasMany(e => e.User_Tag_REL)
                .WithRequired(e => e.Tags)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Employment_Histories)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Journals)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Logs)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Users>()
                .HasOptional(e => e.User_Details)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.User_EDU_REL)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.User_LOC_REL)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.User_Skill_REL)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.User_Tag_REL)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(true);
        }
    }
}
