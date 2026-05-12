using Microsoft.AspNetCore.Http;

namespace Muhit.Application.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadProfileImageAsync(int userId, IFormFile file);
    Task DeleteAsync(string fileUrl);
}