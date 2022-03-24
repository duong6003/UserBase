using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.UserRequests;
using Infrastructure.Persistence.Repositories;
using Core.Utilities;
using Envelop.App.Ultilities;
using Infrastructure.Persistence.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Modules.Users.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid userId);
    Task<(User? User, string? ErrorMessage)> GetDetailAsync(Guid userId);
    Task<(PaginationResponse<User>, string? ErrorMessage)> GetAllAsync(PaginationRequest request);
    Task<(User? User, string? ErrorMessage)> CreateAsync(UserSignUpRequest request);
    Task<(User? User, string? ErrorMessage)> UpdateAsync(User user, UserUpdateRequest request);
    Task<(User? User, string? ErrorMessage)> DeleteAsync(User user);
    Task<string> GenerateAccessTokenAsync(User user);
    Task<string> GenerateRefreshTokenAsync();
    Task<(string AccesToken, string? ErrorMessage)> AuthenticateAsync(UserSignInRequest request);
    string GenerateEmailConfirmationTokenAsync(User user);
}

public class UserService : IUserService
{
    private readonly IRepositoryWrapper RepositoryWrapper;
    private readonly IMapper Mapper;
    private readonly IUrlHelper UrlHelper;
    private readonly IConfiguration Configuration;

    public UserService(IConfiguration configuration, IRepositoryWrapper repositoryWrapper, IMapper mapper, IUrlHelper urlHelper)
    {
        RepositoryWrapper = repositoryWrapper;
        Configuration = configuration;
        Mapper = mapper;
        UrlHelper = urlHelper;
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await RepositoryWrapper.Users.GetByIdAsync(userId);
    }

    public async Task<(User? User, string? ErrorMessage)> GetDetailAsync(Guid userId)
    {
        User? user = await GetByIdAsync(userId);
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
            new Claim(JwtClaimsName.Avatar, user.Avatar!),
        };

        userPermissions.ForEach(x => permissions.Add(x.Permission!.Code!));

        foreach (string permission in permissions)
        {
            claims.Add(new Claim(JwtClaimsName.UserPermissions, permission));
        }
        double timeExpire = double.Parse(Configuration["JwtSettings:ExpiredTime"]);

        return await TokenGenerate(Configuration["JwtSettings:Key"],
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
        if (user == null) return (null!, Messages.Users.UserNameNotFound);

        bool success = BCrypt.Net.BCrypt.Verify(request.Password!.HashPassword(), user.Password);
        if (!success) return (null!, Messages.Users.PasswordNotMatch);

        if(user.Status == (byte)Status.InActive) return (null!, Messages.Users.UserIsLocked);

        string token = await GenerateAccessTokenAsync(user);

        return (token, null!);
    }
    public async Task<(User? User, string? ErrorMessage)> CreateAsync(UserSignUpRequest request)
    {
        List<RolePermission> rolePermissions = await RepositoryWrapper.RolePermissions.FindByCondition(x=> x.RoleId == request.RoleId).ToListAsync();
        User? user = Mapper.Map<User>(request);
        foreach(RolePermission rolePermission in rolePermissions)
        {
            user.UserPermissions!.Add(new UserPermission(){
                UserId = user.Id,
                PermissionId = rolePermission.PermissionId
            });
        }
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

    public async Task<(User? User, string? ErrorMessage)> UpdateAsync(User user, UserUpdateRequest request)
    {
        if(user.RoleId != request.RoleId)
        User?  newUser = Mapper.Map<UserUpdateRequest, User>(request);
        //Get User permisstion Detail
        ICollection<UserPermission>? newUserPermissions = newUser.UserPermissions;

        //new User permissions added
        List<UserPermission>? addedUserPermissions = newUserPermissions!.Where(x => x.Id == Guid.Empty || x.Id == null).ToList();

        //get updated User permissions
        List<UserPermission>? updatedUserPermissions = newUserPermissions!.Where(x => x.Id != Guid.Empty || x.Id == null).ToList();

        //Existed UserPermissions
        List<UserPermission>? existedUserPermissions = RepositoryWrapper.UserPermissions.FindByCondition(x => x.UserId == user.Id).ToList();

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
}
