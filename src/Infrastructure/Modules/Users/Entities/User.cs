﻿using Core.Bases;
using Infrastructure.Modules.Roles.Entities;
using Infrastructure.Modules.UserPermissions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Modules.Users.Entities;

public class User : BaseEntity
{
    public string? UserName { get; set; }
    public string? EmailAddress  { get; set; }
    public string? Password { get; set; }
    public string? Avatar { get; set; }
    public byte Status { get; set; }
    public Guid? RoleId { get; set; }
    public virtual Role? Role { get; set; }
    public virtual ICollection<UserPermission>? UserPermissions { get; set; }
}