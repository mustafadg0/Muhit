using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Muhit.Application.Common;
using Muhit.Application.DTOs.User.Request;
using Muhit.Application.DTOs.User.Response;
using Muhit.Application.Interfaces;
using Muhit.Persistence.Context;

namespace Muhit.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly MuhitDbContext _context;
    private readonly IBlobStorageService _blobStorageService;
    public UserService(MuhitDbContext context, IBlobStorageService blobStorageService)
    {
        _context = context;
        _blobStorageService = blobStorageService;
    }

    public async Task<BaseResponse<List<UserResponse>>> GetAllAsync()
    {
        var response = new BaseResponse<List<UserResponse>>();

        var users = await Query()
            .ToListAsync();

        response.Success = true;
        response.Message = "Kullanıcılar başarıyla getirildi.";
        response.Data = users;

        return response;
    }

    public async Task<BaseResponse<UserResponse>> GetByIdAsync(int id)
    {
        var response = new BaseResponse<UserResponse>();

        var user = await Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            response.Success = false;
            response.Message = "Kullanıcı bulunamadı.";
            return response;
        }

        response.Success = true;
        response.Message = "Kullanıcı başarıyla getirildi.";
        response.Data = user;

        return response;
    }

    public async Task<BaseResponse<UserResponse>> UpdateAsync(UpdateUserRequest request)
    {
        var response = new BaseResponse<UserResponse>();

        var user = await _context.AppUsers
            .FirstOrDefaultAsync(x =>
                x.Id == request.Id &&
                x.IsActive &&
                !x.IsDeleted);

        if (user == null)
        {
            response.Success = false;
            response.Message = "Kullanıcı bulunamadı.";
            return response;
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phoneExists = await _context.AppUsers
                .AnyAsync(x =>
                    x.PhoneNumber == request.PhoneNumber &&
                    x.Id != request.Id &&
                    !x.IsDeleted);

            if (phoneExists)
            {
                response.Success = false;
                response.Message = "Bu telefon numarası başka bir kullanıcı tarafından kullanılıyor.";
                return response;
            }
        }

        user.FullName = request.FullName.Trim();
        user.PhoneNumber = request.PhoneNumber;
        user.CurrentNeighborhoodId = request.CurrentNeighborhoodId;
        user.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        response.Success = true;
        response.Message = "Kullanıcı başarıyla güncellendi.";
        response.Data = await Query().FirstOrDefaultAsync(x => x.Id == user.Id);

        return response;
    }

    public async Task<BaseResponse<bool>> DeleteAsync(int id)
    {
        var response = new BaseResponse<bool>();

        var user = await _context.AppUsers
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);

        if (user == null)
        {
            response.Success = false;
            response.Message = "Kullanıcı bulunamadı.";
            response.Data = false;
            return response;
        }

        user.IsActive = false;
        user.IsDeleted = true;
        user.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        response.Success = true;
        response.Message = "Kullanıcı başarıyla silindi.";
        response.Data = true;

        return response;
    }

    public async Task<BaseResponse<string>> UploadProfileImageAsync(
        int userId,
        IFormFile file)
    {
        var response = new BaseResponse<string>();

        if (file == null || file.Length == 0)
        {
            response.Success = false;
            response.Message = "Dosya boş.";
            return response;
        }

        const int maxFileSize = 5 * 1024 * 1024;

        if (file.Length > maxFileSize)
        {
            response.Success = false;
            response.Message = "Maksimum dosya boyutu 5 MB olabilir.";
            return response;
        }

        var allowedTypes = new[]
        {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

        if (!allowedTypes.Contains(file.ContentType))
        {
            response.Success = false;
            response.Message = "Sadece jpg, png veya webp yüklenebilir.";
            return response;
        }

        var extension = Path.GetExtension(file.FileName).ToLower();

        var allowedExtensions = new[]
        {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

        if (!allowedExtensions.Contains(extension))
        {
            response.Success = false;
            response.Message = "Geçersiz dosya uzantısı.";
            return response;
        }

        var user = await _context.AppUsers.FindAsync(userId);

        if (user == null)
        {
            response.Success = false;
            response.Message = "Kullanıcı bulunamadı.";
            return response;
        }

        if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
        {
            await _blobStorageService.DeleteAsync(user.ProfileImageUrl);
        }

        var imageUrl = await _blobStorageService.UploadProfileImageAsync(userId, file);

        user.ProfileImageUrl = imageUrl;
        user.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        response.Success = true;
        response.Message = "Profil resmi güncellendi.";
        response.Data = imageUrl;

        return response;
    }
    private IQueryable<UserResponse> Query()
    {
        return _context.AppUsers
            .AsNoTracking()
            .Where(x => x.IsActive && !x.IsDeleted)
            .Include(x => x.CurrentNeighborhood)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                IsEmailVerified = x.IsEmailVerified,
                PhoneNumber = x.PhoneNumber,
                IsPhoneNumberVerified = x.IsPhoneNumberVerified,
                CurrentNeighborhoodId = x.CurrentNeighborhoodId,
                CurrentNeighborhoodName = x.CurrentNeighborhood != null
                    ? x.CurrentNeighborhood.Name
                    : null,
                CreatedDate = x.CreatedDate,
                LastLoginDate = x.LastLoginDate,
                MembershipType = x.MembershipType,
                ProfileImageUrl = x.ProfileImageUrl
            });
    }
}