using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyBoards.Entities.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.Property(t => t.Activity).HasMaxLength(200);
            builder.Property(t => t.RemainingWork).HasPrecision(14, 2);
        }
    }
}
