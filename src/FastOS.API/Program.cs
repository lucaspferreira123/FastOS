using FastOS.CrossCutting.IoC;
using FastOS.Domain.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddFastOsDependencies(builder.Configuration);

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

if (app.Environment.IsDevelopment())
{
    app.UseFastOsSwagger();
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
