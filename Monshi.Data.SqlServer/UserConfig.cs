using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monshi.Domain.Users.Entities;

namespace Monshi.Data.SqlServer;

public class UserConfig:IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Username).HasMaxLength(50);
        builder.HasIndex(x => x.Username).IsUnique();
    }
}