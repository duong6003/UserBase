using Core.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Modules.Permissions.Entities
{
    public class Permission : BaseEntity
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
}
