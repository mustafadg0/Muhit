using Muhit.Application.Common;
using Muhit.Application.DTOs.Auth.Request;
using Muhit.Application.DTOs.Auth.Response;

namespace Muhit.Application.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<BaseResponse<AuthResponse>> LoginAsync(LoginRequest request);
    }
}
