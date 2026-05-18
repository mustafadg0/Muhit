using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Muhit.Api.Extensions;
using Muhit.Application.Interfaces;
using Muhit.Infrastructure.Services;
using Muhit.Persistence.Context;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<MuhitDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobileClient", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
    };
});

builder.Services.AddAuthorization();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INeighborhoodService, NeighborhoodService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAiNeighborhoodAnalysisService, AiNeighborhoodAnalysisService>();
builder.Services.AddScoped<INeighborhoodHomeService, NeighborhoodHomeService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

builder.Services.AddScoped<INeighborhoodAmenityService, NeighborhoodAmenityService>();
builder.Services.AddScoped<INeighborhoodDetailService, NeighborhoodDetailService>();
builder.Services.AddScoped<INeighborhoodAiAnalysisAppService, NeighborhoodAiAnalysisAppService>();

builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IDistrictService, DistrictService>();

builder.Services.AddHttpClient<IGoogleGeocodingService, GoogleGeocodingService>();
builder.Services.AddHttpClient<IGooglePlacesService, GooglePlacesService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseGlobalExceptionMiddleware();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowMobileClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();