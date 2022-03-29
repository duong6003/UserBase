using Infrastructure.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Modules.Users.Configurations
{
    public class RolePermissionConfigurations : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<RolePermission> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("role_permission");

            entityTypeBuilder.HasKey(x => new { x.RoleId, x.Code });
        }
    }
}