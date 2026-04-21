using FastOS.Application.Services;
using FastOS.Domain.Entities;
using FastOS.Domain.Interfaces;
using FastOS.Infrastructure.Data;
using FastOS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FastOS.CrossCutting.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddFastOsDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure()));

        services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 4;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Login/Index";
            options.AccessDeniedPath = "/Login/Index";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
        });

        services.AddScoped<UsuarioBusiness>();
        services.AddScoped<ClienteBusiness>();
        services.AddScoped<ProdutoBusiness>();
        services.AddScoped<OrdemServicoBusiness>();
        services.AddScoped<StatusBusiness>();
        services.AddScoped<ItemOrdemServicoBusiness>();
        services.AddScoped<RelatorioBusiness>();
        services.AddScoped<OrcamentoBusiness>();

        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<UsuarioRepository>();
        services.AddScoped<ClienteRepository>();
        services.AddScoped<ProdutoRepository>();
        services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();
        services.AddScoped<StatusRepository>();
        services.AddScoped<IItemOrdemServicoRepository, ItemOrdemServicoRepository>();
        services.AddScoped<IOrcamentoRepository, OrcamentoRepository>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FastOS API",
                Version = "v1"
            });
        });

        return services;
    }

    public static void UseFastOsSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "FastOS API v1");
            c.RoutePrefix = string.Empty;
        });
    }
}
