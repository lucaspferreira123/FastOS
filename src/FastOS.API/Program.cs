using FastOS.CrossCutting.IoC;
using FastOS.Domain.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

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
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

        // Seed das roles
        foreach (var role in new[] { "Admin", "Tecnico" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        var identitySeedSection = builder.Configuration.GetSection("IdentitySeed");
        var adminEmail    = identitySeedSection["Email"]    ?? "admin@fastos.local";
        var adminPassword = identitySeedSection["Password"] ?? "Admin1234";
        var adminName     = identitySeedSection["Nome"]     ?? "Administrador";

        var user = await userManager.FindByEmailAsync(adminEmail);
        if (user == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName       = adminEmail,
                Email          = adminEmail,
                Nome           = adminName,
                Ativo          = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
            else
                logger.LogWarning("Falha ao criar usuário inicial do Identity: {Errors}",
                    string.Join("; ", result.Errors.Select(e => e.Description)));
        }
        else if (!await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.AddToRoleAsync(user, "Admin");
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

// ── Health check endpoint ────────────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new { status = "online", timestamp = DateTime.Now }))
   .AllowAnonymous();

app.Run();
