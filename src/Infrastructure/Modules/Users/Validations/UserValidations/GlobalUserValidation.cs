using Infrastructure.Modules.Users.Entities;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.GlobalValidation;

namespace Infrastructure.Modules.Users.Validations.UserValidations
{
    public class GlobalUserValidation : GlobalValidation<User>
    {
        public GlobalUserValidation(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
