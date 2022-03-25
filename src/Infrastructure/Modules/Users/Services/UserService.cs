using AutoMapper;
using Core.Utilities;
using Envelop.App.Ultilities;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
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

    Task<string> GenerateAccessTokenAsync(User user);

    Task<string> GenerateRefreshTokenAsync();

    Task<(string AccesToken, string? ErrorMessage)> AuthenticateAsync(UserSignInRequest request);

    string GeneratePasswordResetTokenAsync(User user);

    Task<(User? User, string? ErrorMessage)> ResetPassword(ResetPasswordRequest request);

    Task<string> ConfirmResetPassword(Guid userId, string code);
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
        User? user = await RepositoryWrapper.Users.FindByCondition(x=> x.Id == userId).Include(x=> x.UserPermissions).FirstOrDefaultAsync();
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
        if (permissions.Count > 0)
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
                .Select(x => new UserPermission() { UserId = user.Id, PermissionId = x.PermissionId }).ToListAsync();
        user.UserPermissions!.AddRange(rolePermissions);
        await RepositoryWrapper.Users.AddAsync(user);
        return (user, null);
    }

    public void AddRoleToUser()
    {
    }

    public async Task<(PaginationResponse<User>, string? ErrorMessage)> GetAllAsync(PaginationRequest request)
    {
        IQueryable<User>? users = RepositoryWrapper.Users.FindByCondition(x =>
      (
          string.IsNullOrEmpty(request.SearchContent)
          || x.UserName!.ToLower().Contains(request.SearchContent!.ToLower())
          || x.EmailAddress!.ToLower().Contains(request.SearchContent!.ToLower())
      )).Include(x=> x.UserPermissions);
        users = SortUtility<User>.ApplySort(users, request.OrderByQuery!);
        PaginationUtility<User>? data = await PaginationUtility<User>.ToPagedListAsync(users, request.PageNumber, request.PageSize);
        return (PaginationResponse<User>.PaginationInfo(data, data.PageInfo), Messages.Users.GetAllSuccessfully);
    }

    public async Task<(User? User, string? ErrorMessage)> UpdateAsync(User user, UserUpdateRequest request)
    {
        User? newUser = Mapper.Map<UserUpdateRequest, User>(request);
        //Get User permisstion Detail
        var newUserPermissions = request.UserPermissions;

        //new User permissions added
        var addedUserPermissionsReq = newUserPermissions!.Where(x => x.Id == Guid.Empty).ToList();
        var addedUserPermissions = Mapper.Map<List<UserPermission>>(addedUserPermissionsReq);

        //get updated User permissions
        var updatedUserPermissionsReq = newUserPermissions!.Where(x => x.Id != Guid.Empty).ToList();
        var updatedUserPermissions = Mapper.Map<List<UserPermission>>(updatedUserPermissionsReq);

        //Existed UserPermissions
        var existedUserPermissions = RepositoryWrapper.UserPermissions.FindByCondition(x => x.UserId == user.Id).ToList();
        if (user.RoleId != request.RoleId)
        {
            var newUserRolePermissions = await RepositoryWrapper.RolePermissions.FindByCondition(x => x.RoleId == request.RoleId)
                    .Select(x => new UserPermission { UserId = user.Id, PermissionId = x.PermissionId }).ToListAsync();
            addedUserPermissions.AddRange(newUserRolePermissions);
        }
        //Clear db
        newUser.UserPermissions!.Clear();

        foreach (UserPermission? userPermission in updatedUserPermissions)
        {
            await RepositoryWrapper.UserPermissions.UpdateAsync(userPermission);
        }

        foreach (UserPermission? userPermission in addedUserPermissions)
        {
            await RepositoryWrapper.UserPermissions.AddAsync(userPermission);
        }

        await RepositoryWrapper.UserPermissions.DeleteRangeAsync(existedUserPermissions.Except(updatedUserPermissions));

        await RepositoryWrapper.Users.UpdateAsync(newUser);
        return (newUser, null);
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

    public async Task<string> ConfirmResetPassword(Guid userId, string code)
    {
        User? user = await RepositoryWrapper.Users.GetByIdAsync(userId);
        if (user is null) return Messages.Users.IdNotFound;
        if (user!.ResetCode != code) return Messages.Users.UserResetCodeInvalid;
        return Messages.Users.UserResetCodeValid;
    }

    public async Task<(User? User, string? ErrorMessage)> ResetPassword(ResetPasswordRequest request)
    {
        User? user = await GetByEmailAsync(request.Email!);
        string newPassword = user!.Password!.HashPassword();
        user.Password = newPassword;
        await RepositoryWrapper.Users.UpdateAsync(user);
        return (user, Messages.Users.UserResetSuccesfully);
    }
}