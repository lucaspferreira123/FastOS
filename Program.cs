using MeuProjeto.Business;
using MeuProjeto.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure())
);


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<UsuarioBusiness>();
builder.Services.AddScoped<ClienteBusiness>();
builder.Services.AddScoped<ProdutoBusiness>();
builder.Services.AddScoped<OrdemBusiness>();
builder.Services.AddScoped<StatusBusiness>();
builder.Services.AddScoped<ItemOrdemServicoBusiness>();

builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<OrdemRepository>();
builder.Services.AddScoped<StatusRepository>();
builder.Services.AddScoped<ItemOrdemServicoRepository>();


builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "Minha API",
//        Version = "v1"
//    });
//});

var app = builder.Build();

////Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
//// Deixa vazio ou "swagger" se quiser acessar em /swagger
//c.RoutePrefix = string.Empty;
//});
//}
//else
//{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
     pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
