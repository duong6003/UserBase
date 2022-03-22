using Infrastructure.Modules.RolePermisstions.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Modules.RolePermisstions.Configurations
{

    public class RolePermissionConfigurations : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<RolePermission> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("role_permission");

            entityTypeBuilder.HasKey(x => x.Id);
        }
    }
}
