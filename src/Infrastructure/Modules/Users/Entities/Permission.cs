using Core.Bases;

namespace Infrastructure.Modules.Users.Entities
{
    public class Permission : BaseEntity
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
}
