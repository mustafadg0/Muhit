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

    public UserService(MuhitDbContext context)
    {
        _context = context;
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
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (user == null)
        {
            response.Success = false;
            response.Message = "Kullanıcı bulunamadı.";
            return response;
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phoneExists = await _context.AppUsers
                .AnyAsync(x => x.PhoneNumber == request.PhoneNumber && x.Id != request.Id);

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
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            response.Success = false;
            response.Message = "Kullanıcı bulunamadı.";
            response.Data = false;
            return response;
        }

        _context.AppUsers.Remove(user);
        await _context.SaveChangesAsync();

        response.Success = true;
        response.Message = "Kullanıcı başarıyla silindi.";
        response.Data = true;

        return response;
    }

    private IQueryable<UserResponse> Query()
    {
        return _context.AppUsers
            .AsNoTracking()
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
                LastLoginDate = x.LastLoginDate
            });
    }
}