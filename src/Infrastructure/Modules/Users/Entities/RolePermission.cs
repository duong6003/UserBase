using Core.Bases;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Modules.Users.Entities
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role? Role { get; set; }
        public string? Code { get; set; }
        [ForeignKey("Code")]
        public Permission? Permission { get; set; }

    }
}
