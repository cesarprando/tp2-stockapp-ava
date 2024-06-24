using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StockApp.API.Middlewares;
using StockApp.Application.Interfaces;
using StockApp.Application.Services;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Repositories;
using StockApp.Infra.IoC;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.AddInfrastructureAPI(builder.Configuration);
        builder.Services.AddControllers();

        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IMfaService, MfaService>();
        builder.Services.AddScoped<IInventoryService, InventoryService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
        });

        var key = Encoding.ASCII.GetBytes("3xmpl3V3ryS3cur3S3cr3tK3y!@#123");
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:SuaAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "StockApp API", Version = "v1" });
            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "Authorization: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            c.AddSecurityDefinition("Bearer", securitySchema);
            var securityRequirement = new OpenApiSecurityRequirement
            {
                { securitySchema, new [] { "Bearer" } }
            };
            c.AddSecurityRequirement(securityRequirement);
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis");
        });

        var app = builder.Build();

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowAll");
        app.UseMiddleware<ErrorHandlerMiddleware>();
        app.UseStaticFiles();
        app.UseDirectoryBrowser();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}