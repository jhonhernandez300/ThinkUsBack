using Microsoft.Extensions.Configuration;
using ThinkUs.Interfaces;
using ThinkUs;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FluentAssertions.Common;
using System.Configuration;
using ThinkUs.Services;
//using KontrolarCloud.Middlewares;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigins",
        builder => builder.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("EF")
    )
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options => {
//        IConfiguration configuration = builder.Configuration;

//        if (configuration != null)
//        {
//            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//            {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateLifetime = true,
//                ValidateIssuerSigningKey = true,
//                ValidIssuer = configuration["Jwt:Issuer"],
//                ValidAudience = configuration["Jwt:Audience"],
//                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
//            };
//        }
//        else
//        {
//            throw new InvalidOperationException("Configuration is null.");
//        }
//    });

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowOrigins");

//app.UseMiddleware<TokenValidationMiddleware>();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Use(async (context, next) =>
{
    // Log de la solicitud
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});

app.Run();
