using System.Linq.Expressions;
using Core.Bases;
using FluentValidation;
using FluentValidation.Validators;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using static Infrastructure.Definitions.Messages;

namespace Infrastructure.Persistence.GlobalValidation
{
    public static class CommonValidationExtention
    {

        public static bool IsValidFile(this IFormFile file, string contentType)
        {
            return file.ContentType.Contains(contentType);
        }
        public static IRuleBuilderOptions<T, IFormFile?> IsValidFile<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder, string contentType)
        {
            return ruleBuilder.Must(x => x!.IsValidFile(contentType)).WithMessage(Files.InValid);
        }
        public static IRuleBuilderOptions<T, IFormFile?> IsValidSize<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder, int size)
        {
            return ruleBuilder.Must(x => x!.Length <= size).WithMessage(Files.OverSize);
        }
        public static IRuleBuilderOptions<T, string?> IsValidVietNamName<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Matches(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂẾưăạảấầẩẫậắằẳẵặẹẻẽềềểếỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ\s|_]+$");
        }
    }
    // public class ValidMediaTypeValidator<T, TProperty> : PropertyValidator<T, TProperty>

    // {
    //     private string _mediatype;
    //     public ValidMediaTypeValidator(string mediatype)
    //     {
    //         _mediatype = mediatype;
    //     }
    //     public override string Name => "ValidMediaTypeValidator";

    //     public override bool IsValid(ValidationContext<T> context, TProperty value)
    //     {
    //         IFormFile file = (IFormFile)value!;
    //         return file.IsValidContentType(_mediatype);
    //     }

    //     protected override string GetDefaultMessageTemplate(string errorCode)
    //       => "'{PropertyName}' is invalid content type";
    // }
    
    // public interface IGlobalValidation<TEntity> where TEntity : class
    // {
    //     Task<bool> IsExistId<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
    //     bool IsExistProperty(Expression<Func<TEntity, bool>> expression);
    // }
    // public class GlobalValidation<TEntity> : RepositoryBase<TEntity>, IGlobalValidation<TEntity>, IDisposable where TEntity : BaseEntity
    // {
    //     private readonly ApplicationDbContext _dbContext;
    //     public GlobalValidation(ApplicationDbContext dbContext) : base(dbContext)
    //     {
    //         _dbContext = dbContext;
    //     }

    //     public void Dispose()
    //     {
    //         if (_dbContext != null)
    //         {
    //             _dbContext.Dispose();
    //         }
    //     }

    //     // public async Task<bool> IsExistId<Tid>(Tid id, CancellationToken cancellationToken = default) where Tid : notnull
    //     // {
    //     //     var entity = await GetByIdAsync(id, cancellationToken);
    //     //     return entity != null;
    //     // }
    //     // public bool IsExistProperty(Expression<Func<TEntity, bool>> expression)
    //     // {
    //     //     return FindByCondition(expression).Any();
    //     // }
    // }

}