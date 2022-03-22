using Infrastructure.Modules.Permissions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Modules.Permissions.Configurations
{
    public class PermissionConfigurations : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable(nameof(Permission));

            entityTypeBuilder.HasKey(x => x.Id);
        }
    }
}
