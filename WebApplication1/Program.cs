using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// VERÝTABANI BAÐLANTISI 
builder.Services.AddDbContext<UygulamaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebApplication1Db")));

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(); // Hot reload etkisi verir

var app = builder.Build();

// HATA YÖNETÝMÝ
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// ROTA TANIMI
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
