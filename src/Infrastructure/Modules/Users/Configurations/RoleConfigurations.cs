using Infrastructure.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Modules.Users.Configurations
{
    public class RoleConfigurations : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable(nameof(Role));

            entityTypeBuilder.HasKey(x => x.Id);
        }
    }
}