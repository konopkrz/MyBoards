using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyBoards.ViewModels.Configurations
{
    public class MessageAlbanianUserConfiguration : IEntityTypeConfiguration<MessageAlbanianUser>
    {
        public void Configure(EntityTypeBuilder<MessageAlbanianUser> builder)
        {
            builder.ToView("View_MessageAlbanianUsers");
            builder.HasNoKey();
        }
    }
}
