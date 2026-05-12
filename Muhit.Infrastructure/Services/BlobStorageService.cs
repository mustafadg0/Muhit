using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Muhit.Application.Interfaces;

namespace Muhit.Infrastructure.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString =
            configuration["AzureBlobStorageConnectionString"];

        var containerName =
            configuration["AzureBlobStorageProfileImageContainerName"];

        _containerClient = new BlobContainerClient(
            connectionString,
            containerName);
    }

    public async Task<string> UploadProfileImageAsync(
        int userId,
        IFormFile file)
    {
        var extension =
            Path.GetExtension(file.FileName).ToLower();

        var fileName =
            $"users/{userId}/{Guid.NewGuid()}{extension}";

        var blobClient =
            _containerClient.GetBlobClient(fileName);

        var headers = new BlobHttpHeaders
        {
            ContentType = file.ContentType
        };

        await using var stream =
            file.OpenReadStream();

        await blobClient.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = headers
            });

        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return;

        var uri = new Uri(fileUrl);

        var blobName =
            string.Join("", uri.Segments.Skip(2));

        var blobClient =
            _containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync();
    }
}