using Infrastructure.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Modules.Users.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Permission> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable(nameof(Permission));

            entityTypeBuilder.HasKey(x => x.Code);
        }
    }
}