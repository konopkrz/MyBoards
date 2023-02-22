﻿using Microsoft.EntityFrameworkCore;

namespace MyBoards.Entities
{
    public class MyBoardsContext : DbContext
    {
        public MyBoardsContext(DbContextOptions<MyBoardsContext> options) : base(options) { }
        
        public DbSet<WorkItem> WorkItems { get; set; }

        public DbSet<Epic> Epics { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Task> Tasks { get; set; }

        public DbSet<Address> Adresses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<WorkItemState> WorkItemsStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Epic>()
                .Property(e => e.EndDate)
                .HasPrecision(3);

            modelBuilder.Entity<Issue>()
                .Property(i => i.Effort)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Task>(eb =>
            {
                eb.Property(t => t.Activity).HasMaxLength(200);
                eb.Property(t => t.RemainingWork).HasPrecision(14, 2);

            });

            modelBuilder.Entity<WorkItem>(eb =>
            {

                eb.Property(wi => wi.Area).HasColumnType("varchar(200)");
                eb.Property(wi => wi.IterationPath).HasColumnName("Iteration_Path");
                eb.Property(wi => wi.Prority).HasDefaultValue(1);

                eb.HasMany(wi => wi.Comments)
                   .WithOne(c => c.WorkItem)
                   .HasForeignKey(c => c.WorkItemId);
                
                eb.HasOne(wi => wi.Author)
                    .WithMany(u => u.WorkItems)
                    .HasForeignKey(wi => wi.AuthorId);

                eb.HasMany(wi => wi.Tags)
                    .WithMany(t => t.WorkItems)
                    .UsingEntity<WorkItemTag>(
                        w => w.HasOne(wit => wit.Tag)
                        .WithMany()
                        .HasForeignKey(wit => wit.TagId),
                        w => w.HasOne(wit => wit.WorkItem)
                        .WithMany()
                        .HasForeignKey(wit => wit.WorkItemId),
                        wit =>
                        {
                            wit.HasKey(x => new { x.TagId, x.WorkItemId });
                            wit.Property(x => x.PublicationDate).HasDefaultValueSql("getutcdate()");
                        }
                    );

                eb.HasOne(wi => wi.State)
                    .WithMany(s => s.WorkItems)
                    .HasForeignKey(wi => wi.StateId);
                    
            });

            modelBuilder.Entity<Comment>(eb =>
            {
                // ustawianie domyślnej wartości daty po stonie SQL'a
                eb.Property(c => c.CreatedDate).HasDefaultValueSql("getutcdate()");
                eb.Property(c => c.UpdatedDate).ValueGeneratedOnUpdate();
                eb.HasOne(c => c.Author)
                   .WithMany(u => u.Comments)
                   .HasForeignKey(c => c.AuthorId)
                   .OnDelete(DeleteBehavior.NoAction);

            });

            modelBuilder.Entity<WorkItemState>(eb => 
            {
                eb.Property(s => s.Value).IsRequired();
                eb.Property(s => s.Value).HasMaxLength(60);
            });

            modelBuilder.Entity<User>()
                .HasOne(u => u.Address)
                .WithOne(a => a.User)
                .HasForeignKey<Address>(a => a.UserId);

        }
    }
}
