using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace MyBoards.Entities.Configurations
{
    public class WorkItemStateConfiguration : IEntityTypeConfiguration<WorkItemState>
    {
        public void Configure(EntityTypeBuilder<WorkItemState> builder)
        {
            builder.Property(s => s.Value).IsRequired();
            builder.Property(s => s.Value).HasMaxLength(60);

            builder.HasData(new WorkItemState() { Id = 1, Value = "To do" },
                new WorkItemState() { Id = 2, Value = "Doing" },
                new WorkItemState() { Id = 3, Value = "Done" }
            );
        }
    }
}
