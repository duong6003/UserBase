using Infrastructure.Modules.Roles.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Modules.Roles.Configurations
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