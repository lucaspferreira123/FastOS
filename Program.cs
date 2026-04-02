using MeuProjeto.Business;
using MeuProjeto.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TesteMVC.Models;
using TesteMVC.IRepository;
using TesteMVC.Relatorios;
using TesteMVC.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure())
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/Index";
    options.AccessDeniedPath = "/Login/Index";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<UsuarioBusiness>();
builder.Services.AddScoped<ClienteBusiness>();
builder.Services.AddScoped<ProdutoBusiness>();
builder.Services.AddScoped<OrdemServicoBusiness>();
builder.Services.AddScoped<StatusBusiness>();
builder.Services.AddScoped<ItemOrdemServicoBusiness>();
builder.Services.AddScoped<RelatorioBusiness>();
builder.Services.AddScoped<OrcamentoBusiness>();

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>(); 
builder.Services.AddScoped<StatusRepository>();
builder.Services.AddScoped<IItemOrdemServicoRepository ,ItemOrdemServicoRepository>();
builder.Services.AddScoped<IOrcamentoRepository ,OrcamentoRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Minha API",
        Version = "v1"
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeed");

    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var identitySeedSection = builder.Configuration.GetSection("IdentitySeed");
        var adminEmail = identitySeedSection["Email"] ?? "admin@fastos.local";
        var adminPassword = identitySeedSection["Password"] ?? "Admin1234";
        var adminName = identitySeedSection["Nome"] ?? "Administrador";

        var user = await userManager.FindByEmailAsync(adminEmail);
        if (user == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nome = adminName,
                Ativo = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                logger.LogWarning("Falha ao criar usuário inicial do Identity: {Errors}",
                    string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Não foi possível executar seed do Identity. Verifique migrações e banco de dados.");
    }
}

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
    // Deixa vazio ou "swagger" se quiser acessar em /swagger
    c.RoutePrefix = string.Empty;
});
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
     pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
