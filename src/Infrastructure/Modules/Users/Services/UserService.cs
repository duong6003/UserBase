using AutoMapper;
using Core.Utilities;
using Envelop.App.Ultilities;
using Hangfire;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.UserPermissionRequests;
using Infrastructure.Modules.Users.Requests.UserRequests;
using Infrastructure.Persistence.Definitions;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Modules.Users.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid userId);

    Task<User?> GetByEmailAsync(string email);

    Task<(User? User, string? ErrorMessage)> GetDetailAsync(Guid userId);

    Task<(PaginationResponse<User>, string? ErrorMessage)> GetAllAsync(PaginationRequest request);

    Task<(User? User, string? ErrorMessage)> CreateAsync(UserSignUpRequest request);

    Task<(User? User, string? ErrorMessage)> UpdateAsync(User user, UserUpdateRequest request);

    Task<(User? User, string? ErrorMessage)> DeleteAsync(User user);
    Task<(User User, string? ErrorMessage)> AddUserPermissionAsync(User user, List<CreateUserPermissionRequest> request);
    Task<(User User, string? ErrorMessage)> DeleteUserPermissionAsync(User user, List<DeleteUserPermissionRequest> request);
    Task<string> GenerateAccessTokenAsync(User user);

    Task<string> GenerateRefreshTokenAsync();

    Task<(string AccesToken, string? ErrorMessage)> AuthenticateAsync(UserSignInRequest request);

    string GeneratePasswordResetTokenAsync(User user);

    Task<(User? User, string? ErrorMessage)> ResetPassword(User user, ResetPasswordRequest request);

    Task<string> ConfirmResetPassword(Guid userId, string code, DateTime expireTime);
    Task<(User? User, string? ErrorMessage)> ResetPasswordByConfirm(User user, string code ,ResetPasswordRequest request);
}

public class UserService : IUserService
{
    private readonly IRepositoryWrapper RepositoryWrapper;
    private readonly IMapper Mapper;
    private readonly IConfiguration Configuration;

    public UserService(IConfiguration configuration, IRepositoryWrapper repositoryWrapper, IMapper mapper)
    {
        RepositoryWrapper = repositoryWrapper;
        Configuration = configuration;
        Mapper = mapper;
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await RepositoryWrapper.Users.GetByIdAsync(userId);
    }

    public async Task<User?> GetByEmailAsync(string emailAddress)
    {
        return await RepositoryWrapper.Users.FindByCondition(x => x.EmailAddress == emailAddress).FirstOrDefaultAsync();
    }

    public async Task<(User? User, string? ErrorMessage)> GetDetailAsync(Guid userId)
    {
        User? user = await RepositoryWrapper.Users.FindByCondition(x => x.Id == userId).Include(x => x.UserPermissions).FirstOrDefaultAsync();
        if (user is null)
        {
            return (null, Messages.Users.IdNotFound);
        }
        return (user, null);
    }

    private Task<string> TokenGenerate(string secretKey, string isUser, string isAudience, DateTime? expireTime, IEnumerable<Claim> claims = null!)
    {
        SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(isUser,
            isAudience,
            claims,
            expires: expireTime,
            signingCredentials: creds);
        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public async Task<string> GenerateAccessTokenAsync(User user)
    {
        List<string> permissions = new();
        var userPermissions = await RepositoryWrapper.UserPermissions.FindByCondition(x => x.UserId == user.Id).Include(x => x.Permission).ToListAsync();

        List<Claim> claims = new()
        {
            new Claim(JwtClaimsName.Identification, user.Id.ToString()),
            new Claim(JwtClaimsName.UserName, user.UserName!),
            new Claim(JwtClaimsName.Email, user.EmailAddress!),
        };
        if (userPermissions.Count > 0)
        {
            userPermissions.ForEach(x => permissions.Add(x.Permission!.Code!));

            foreach (string permission in permissions)
            {
                claims.Add(new Claim(JwtClaimsName.UserPermissions, permission));
            }
        }
        if (user.Avatar is not null)
        {
            claims.Add(new Claim(JwtClaimsName.Avatar, user.Avatar));
        }
        double timeExpire = double.Parse(Configuration["JwtSettings:ExpiredTime"]);

        return await TokenGenerate(Configuration["JwtSettings:SecretKey"],
            Configuration["JwtSettings:IsUser"],
            Configuration["JwtSettings:IsAudience"],
            DateTime.UtcNow.AddMinutes(timeExpire),
            claims);
    }

    public async Task<string> GenerateRefreshTokenAsync()
    {
        double timeExpire;
        bool ok = double.TryParse(Configuration["JwtSettings:RefreshTokenExpire"], out timeExpire);
        if (!ok) throw new Exception("Check appsettings.json -> JwtSettings");

        return await TokenGenerate(Configuration["JwtSettings:RefreshKey"],
            Configuration["JwtSettings:IsUser"],
            Configuration["JwtSettings:IsAudience"],
            DateTime.UtcNow.AddDays(double.Parse(Configuration["RefreshTokenExpiredTime"]))
            );
    }

    public async Task<(string AccesToken, string? ErrorMessage)> AuthenticateAsync(UserSignInRequest request)
    {
        User? user = await RepositoryWrapper.Users.FindByCondition(x => x.UserName == request.UserName).FirstOrDefaultAsync()!;

        bool success = BCrypt.Net.BCrypt.Verify(request.Password, user!.Password);
        if (!success) return (null!, Messages.Users.PasswordNotMatch);

        if (user.Status == (byte)Status.InActive) return (null!, Messages.Users.UserIsLocked);

        string token = await GenerateAccessTokenAsync(user);

        return (token, null!);
    }

    public async Task<(User? User, string? ErrorMessage)> CreateAsync(UserSignUpRequest request)
    {
        User? user = Mapper.Map<User>(request);
        List<UserPermission> rolePermissions = await RepositoryWrapper.RolePermissions.FindByCondition(x => x.RoleId == request.RoleId)
                .Select(x => new UserPermission() { UserId = user.Id, Code = x.Code }).ToListAsync();
        user.UserPermissions!.AddRange(rolePermissions);
        await RepositoryWrapper.Users.AddAsync(user);
        return (user, null);
    }

    public async Task<(PaginationResponse<User>, string? ErrorMessage)> GetAllAsync(PaginationRequest request)
    {
        IQueryable<User>? users = RepositoryWrapper.Users.FindByCondition(x =>
      (
          string.IsNullOrEmpty(request.SearchContent)
          || x.UserName!.ToLower().Contains(request.SearchContent!.ToLower())
          || x.EmailAddress!.ToLower().Contains(request.SearchContent!.ToLower())
      ));
        users = SortUtility<User>.ApplySort(users, request.OrderByQuery!);
        PaginationUtility<User>? data = await PaginationUtility<User>.ToPagedListAsync(users, request.PageNumber, request.PageSize);
        return (PaginationResponse<User>.PaginationInfo(data, data.PageInfo), Messages.Users.GetAllSuccessfully);
    }
    public async Task<(User User, string? ErrorMessage)> AddUserPermissionAsync(User user, List<CreateUserPermissionRequest> request)
    {
        List<UserPermission> newUserRolePermissions = Mapper.Map<List<UserPermission>>(request);
        await RepositoryWrapper.UserPermissions.AddRangeAsync(newUserRolePermissions);
        return (user, null);
    }
    public async Task<(User User, string? ErrorMessage)> DeleteUserPermissionAsync(User user, List<DeleteUserPermissionRequest> request)
    {
        List<UserPermission> deletedUserRolePermissions = Mapper.Map<List<UserPermission>>(request);
        await RepositoryWrapper.UserPermissions.DeleteRangeAsync(deletedUserRolePermissions);
        return (user, null);
    }
    public async Task<(User? User, string? ErrorMessage)> UpdateAsync(User user, UserUpdateRequest request)
    {

        if (user.RoleId != request.RoleId)
        {
            var oldUserRolePermissions = await RepositoryWrapper.RolePermissions
                .FindByCondition(x => x.RoleId != request.RoleId)
                .Select(x => new UserPermission { UserId = user.Id, Code = x.Code })
                .ToListAsync();
            var newUserRolePermissions = await RepositoryWrapper.RolePermissions
                .FindByCondition(x => x.RoleId == request.RoleId)
                .Select(x => new UserPermission { UserId = user.Id, Code = x.Code })
                .ToListAsync();
            using var transaction = RepositoryWrapper.BeginTransactionAsync();
            try
            {
                await RepositoryWrapper.UserPermissions.DeleteRangeAsync(oldUserRolePermissions);
                await RepositoryWrapper.UserPermissions.AddRangeAsync(newUserRolePermissions);

                await RepositoryWrapper.CommitTransactionAsync();
            }
            catch (System.Exception ex)
            {
                await RepositoryWrapper.RollbackTransactionAsync();
                BackgroundJob.Enqueue(() => Console.WriteLine($"--> Add new user permission failed: {ex.Message}"));
            }
        }
        await RepositoryWrapper.Users.UpdateAsync(user);
        return (user, null);
    }

    public async Task<(User? User, string? ErrorMessage)> DeleteAsync(User user)
    {
        await RepositoryWrapper.Users.DeleteAsync(user);
        return (user, Messages.Users.DeleteUserSuccessfully);
    }

    public string GeneratePasswordResetTokenAsync(User user)
    {
        Random rd = new Random();
        string code = rd.Next(1000000, 9999999).ToString();
        user.ResetCode = code;
        RepositoryWrapper.Users.UpdateAsync(user);
        return code;
    }

    public async Task<(User? User, string? ErrorMessage)> ResetPassword(User? user, string code, string password)
    {
        if (user!.ResetCode != code)
        {
            return (null, Messages.Users.ResetCodeNotValid);
        }
        user.Password = password.HashPassword();
        await RepositoryWrapper.Users.UpdateAsync(user);
        return (user, null);
    }

    public async Task<string> ConfirmResetPassword(Guid userId, string code, DateTime expireTime)
    {
        if(DateTime.UtcNow.CompareTo(expireTime) < 0) return Messages.Users.UserResetCodeExpire;
        User? user = await RepositoryWrapper.Users.GetByIdAsync(userId);
        if (user is null) return Messages.Users.IdNotFound;
        if (user!.ResetCode != code) return Messages.Users.UserResetCodeInvalid;
        return Messages.Users.UserResetCodeValid;
    }

    public async Task<(User? User, string? ErrorMessage)> ResetPassword(User user, ResetPasswordRequest request)
    {
        string newPassword = user!.Password!.HashPassword();
        user.Password = newPassword;
        await RepositoryWrapper.Users.UpdateAsync(user);
        return (user, Messages.Users.UserResetSuccesfully);
    }
    public async Task<(User? User, string? ErrorMessage)> ResetPasswordByConfirm(User user, string code, ResetPasswordRequest request)
    {
        if(user!.ResetCode != code) return (user, Messages.Users.ResetCodeNotValid);

        string newPassword = user!.Password!.HashPassword();
        user.Password = newPassword;
        await RepositoryWrapper.Users.UpdateAsync(user);
        return (user, Messages.Users.UserResetSuccesfully);
    }
}