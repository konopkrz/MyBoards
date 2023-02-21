using Microsoft.EntityFrameworkCore;

namespace MyBoards.Entities
{
    public class MyBoardsContext : DbContext
    {
        public MyBoardsContext(DbContextOptions<MyBoardsContext> options) : base(options) { }
        
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Address> Adresses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<WorkItem>()
            //    .Property(x => x.State)
            //    .IsRequired();

            modelBuilder.Entity<WorkItem>(eb =>
            {
                //eb = entityBuilder, wi = workItem
                eb.Property(wi => wi.State).IsRequired();
                eb.Property(wi => wi.Area).HasColumnType("varchar(200)");
                eb.Property(wi => wi.IterationPath).HasColumnName("Iteration_Path");
                eb.Property(wi => wi.Effort).HasColumnType("decimal(5,2)");
                eb.Property(wi => wi.EndDate).HasPrecision(3);
                eb.Property(wi => wi.Activity).HasMaxLength(200);
                eb.Property(wi => wi.RemainingWork).HasPrecision(14, 2);
                eb.Property(wi => wi.Prority).HasDefaultValue(1);
            });

            modelBuilder.Entity<Comment>(eb =>
            {
                // ustawianie domyślnej wartości daty po stonie SQL'a
                eb.Property(ci => ci.CreatedDate).HasDefaultValueSql("getutcdate()");
                eb.Property(ci => ci.UpdatedDate).ValueGeneratedOnUpdate();
            });

            modelBuilder.Entity<User>()
                .HasOne(u => u.Address)
                .WithOne(a => a.User)
                .HasForeignKey<Address>(a => a.UserId);
        }
    }
}
