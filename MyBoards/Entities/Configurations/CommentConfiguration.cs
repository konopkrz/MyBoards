using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyBoards.Entities.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            // ustawianie domyślnej wartości daty po stonie SQL'a
            builder.Property(c => c.CreatedDate).HasDefaultValueSql("getutcdate()");
            builder.Property(c => c.UpdatedDate).ValueGeneratedOnUpdate();
            builder.HasOne(c => c.Author)
               .WithMany(u => u.Comments)
               .HasForeignKey(c => c.AuthorId)
               //.OnDelete(DeleteBehavior.NoAction);
               .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
