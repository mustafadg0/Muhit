using Muhit.Application.Common;
using Muhit.Application.DTOs.User.Request;
using Muhit.Application.DTOs.User.Response;

namespace Muhit.Application.Interfaces;

public interface IUserService
{
    Task<BaseResponse<List<UserResponse>>> GetAllAsync();
    Task<BaseResponse<UserResponse>> GetByIdAsync(int id);
    Task<BaseResponse<UserResponse>> UpdateAsync(UpdateUserRequest request);
    Task<BaseResponse<bool>> DeleteAsync(int id);
}