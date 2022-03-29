using Infrastructure.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Modules.Users.Configurations
{
    public class UserPermissionConfigurations : IEntityTypeConfiguration<UserPermission>
    {
        public void Configure(EntityTypeBuilder<UserPermission> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("user_permission");

            entityTypeBuilder.HasKey(x => new { x.UserId, x.Code });
        }
    }
}