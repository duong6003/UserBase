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

namespace Infrastructure.Modules.Users.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid userId);
    Task<(User? User, string? ErrorMessage)> GetDetailAsync(Guid userId);
    Task<string> GenerateAccessToken(User user);
    Task<string> GenerateRefreshToken();
    Task<(string AccesToken, string? ErrorMessage)> Authenticate(UserSignInRequest request);
    Task<(User? User, string? ErrorMessage)> CreateAccount(UserSignUpRequest request);
    //Task<(User? User, string? ErrorMessage)> UpdateAccount(UpdateUserRequest request);
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
    public async Task<string> GenerateAccessToken(User user)
    {
        List<string> roles = new();
        var userPermissions = await RepositoryWrapper.UserPermissions.FindByCondition(x => x.UserId == user.Id).Include(x => x.Permission).ToListAsync();

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.DenyOnlyPrimaryGroupSid, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress!),
            new Claim("avatar", user.Avatar!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        userPermissions.ForEach(x => roles.Add(x.Permission!.Code!));

        foreach (string role in roles)
        {
            claims.Add(new Claim("roles", role));
        }
        double timeExpire = double.Parse(Configuration["JwtSettings:ExpiredTime"]);

        return await TokenGenerate(Configuration["JwtSettings:Key"],
            Configuration["JwtSettings:IsUser"],
            Configuration["JwtSettings:IsAudience"],
            DateTime.UtcNow.AddMinutes(timeExpire),
            claims);

    }
    public async Task<string> GenerateRefreshToken()
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
    public async Task<(string AccesToken, string? ErrorMessage)> Authenticate(UserSignInRequest request)
    {
        User? user = await RepositoryWrapper.Users.FindByCondition(x => x.UserName == request.UserName).FirstOrDefaultAsync()!;
        if (user == null) return (null!, Messages.Users.UserNameNotFound);

        bool success = BCrypt.Net.BCrypt.Verify(request.Password!.HashPassword(), user.Password);
        if (!success) return (null!, Messages.Users.PasswordNotMatch);

        string token = await GenerateAccessToken(user);

        return (token, null!);
    }
    public async Task<(User? User, string? ErrorMessage)> CreateAccount(UserSignUpRequest request)
    {
        User? user = Mapper.Map<User>(request);
        await RepositoryWrapper.Users.AddAsync(user);
        return (user, null);
    }
    // public async Task<(User? User, string? ErrorMessage)> UpdateAccount(UpdateUserRequest request)
    // {
    //     await RepositoryWrapper.Users.UpdateAsync(user);
    // }
}
