using Infrastructure.Modules.UserPermissions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Modules.UserPermissions.Configurations
{
    public class UserPermissionConfigurations : IEntityTypeConfiguration<UserPermission>
    {
        public void Configure(EntityTypeBuilder<UserPermission> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("user_permission");

            entityTypeBuilder.HasKey(x => x.Id);
        }
    }
}
