namespace XBAPLexiconCVDBInterface
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Page2Model : DbContext
    {
        public Page2Model()
            : base("name=Page2Model")
        {
        }

        public virtual DbSet<Adresses> Adresses { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Adresses>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Adresses)
                .WillCascadeOnDelete(false);
        }
    }
}
