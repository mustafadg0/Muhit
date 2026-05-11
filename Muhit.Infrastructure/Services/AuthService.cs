using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Muhit.Application.Common;
using Muhit.Application.DTOs.Auth.Request;
using Muhit.Application.DTOs.Auth.Response;
using Muhit.Application.Interfaces;
using Muhit.Domain.Entities;
using Muhit.Persistence.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Muhit.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly MuhitDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(MuhitDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<BaseResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var response = new BaseResponse<AuthResponse>();

        var email = request.Email.Trim().ToLower();

        var emailExists = await _context.AppUsers.AnyAsync(x => x.Email == email);

        if (emailExists)
        {
            response.Success = false;
            response.Message = "Bu email adresi zaten kayýtlý.";
            return response;
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phoneExists = await _context.AppUsers.AnyAsync(x => x.PhoneNumber == request.PhoneNumber);

            if (phoneExists)
            {
                response.Success = false;
                response.Message = "Bu telefon numarasý zaten kayýtlý.";
                return response;
            }
        }

        CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);

        var user = new AppUser
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            CurrentNeighborhoodId = request.CurrentNeighborhoodId,
            IsEmailVerified = false,
            IsPhoneNumberVerified = false,
            CreatedDate = DateTime.UtcNow
        };

        await _context.AppUsers.AddAsync(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        response.Success = true;
        response.Message = "Kayýt baþarýlý.";
        response.Data = new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Token = token
        };

        return response;
    }

    public async Task<BaseResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var response = new BaseResponse<AuthResponse>();

        var email = request.Email.Trim().ToLower();

        var user = await _context.AppUsers
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user == null)
        {
            response.Success = false;
            response.Message = "Email veya þifre hatalý.";

            return response;
        }

        var passwordValid = VerifyPasswordHash(
            request.Password,
            user.PasswordHash,
            user.PasswordSalt);

        if (!passwordValid)
        {
            response.Success = false;
            response.Message = "Email veya þifre hatalý.";

            return response;
        }

        user.LastLoginDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        response.Success = true;
        response.Message = "Giriþ baþarýlý.";

        response.Data = new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Token = token
        };

        return response;
    }

    private static void CreatePasswordHash(
        string password,
        out string passwordHash,
        out string passwordSalt)
    {
        using var hmac = new HMACSHA512();

        passwordSalt = Convert.ToBase64String(hmac.Key);
        passwordHash = Convert.ToBase64String(
            hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    private static bool VerifyPasswordHash(
        string password,
        string storedHash,
        string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);

        using var hmac = new HMACSHA512(saltBytes);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        var computedHashString = Convert.ToBase64String(computedHash);

        return computedHashString == storedHash;
    }

    private string GenerateJwtToken(AppUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        var secretKey = jwtSettings["SecretKey"]!;
        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;
        var expireMinutes = Convert.ToInt32(jwtSettings["ExpireMinutes"]);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        if (user.CurrentNeighborhoodId.HasValue)
        {
            claims.Add(new Claim("CurrentNeighborhoodId", user.CurrentNeighborhoodId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
