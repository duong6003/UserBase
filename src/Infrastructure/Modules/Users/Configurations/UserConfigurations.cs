using Infrastructure.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Modules.Users.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entityTypeBuilder)
    {
        entityTypeBuilder.ToTable(nameof(User));

        entityTypeBuilder.HasKey(x => x.Id);
    }
}
