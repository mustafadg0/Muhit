using Muhit.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Muhit.Application.Interfaces;
using Muhit.Infrastructure.Services;
using Muhit.Persistence.Context;

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
// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INeighborhoodService, NeighborhoodService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAiNeighborhoodAnalysisService, AiNeighborhoodAnalysisService>();
builder.Services.AddScoped<INeighborhoodHomeService, NeighborhoodHomeService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseGlobalExceptionMiddleware();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// JWT ekleyince bunun üstüne app.UseAuthentication(); gelecek
app.UseAuthorization();
app.UseCors("AllowMobileClient");
app.MapControllers();

app.Run();