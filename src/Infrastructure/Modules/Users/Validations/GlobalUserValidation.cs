using Core.Common.Validation;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.GlobalValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Modules.Users.Validations
{
    public class GlobalUserValidation : GlobalValidation<User>
    {
        public GlobalUserValidation(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
