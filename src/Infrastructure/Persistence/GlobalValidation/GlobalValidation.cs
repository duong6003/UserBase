using System.Linq.Expressions;
using Core.Bases;
using Core.Common.Validation;
using FluentValidation;
using FluentValidation.Validators;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Infrastructure.Persistence.GlobalValidation
{
    public static class FormFileExtention
    {
        public static bool IsValidContentType(this IFormFile file, string contentType)
        {
            return file.ContentType.Contains(contentType);
        }

        public static IRuleBuilderOptions<T, IFormFile> IsValidMediaType<T>(this IRuleBuilder<T, IFormFile> ruleBuilder, string contentType)
        {
            return ruleBuilder.Must(x => x.IsValidContentType(contentType)).WithMessage("Mediatype.Invalid");
        }

    }
    public class ValidMediaTypeValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        private string _mediatype;
        public ValidMediaTypeValidator(string mediatype)
        {
            _mediatype = mediatype;
        }
        public override string Name => "ValidMediaTypeValidator";

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            IFormFile file = (IFormFile)value!;
            return file.IsValidContentType(_mediatype);
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
          => "'{PropertyName}' is invalid content type";
    }
    public class GlobalValidation<TEntity, Tid> : IGlobalValidation<TEntity, Tid>, IDisposable where TEntity : BaseEntity
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IActionContextAccessor _actionContextAccessor;

        public GlobalValidation(ApplicationDbContext dbContext, IActionContextAccessor actionContextAccessor)
        {
            _dbContext = dbContext;
            _actionContextAccessor = actionContextAccessor;
        }

        public void Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
            }
        }

        public bool IsExistId(Tid id)
        {
            IQueryable<TEntity> items = _dbContext.Set<TEntity>();
            return items.Any(x => x.Id.Equals(id));
        }
    }
}