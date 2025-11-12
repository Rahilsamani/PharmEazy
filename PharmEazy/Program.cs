using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmEazy.BAL.Services;
using PharmEazy.DAL.Contacts;
using PharmEazy.DAL.Data;
using PharmEazy.DAL.Repositories;
using PharmEazy.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UserDbContextConnection") ?? throw new InvalidOperationException("Connection string 'UserDbContextConnection' not found."); ;

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryServices, CategoryServices>();
builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
builder.Services.AddScoped<IMedicineServices, MedicineServices>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 2;
    config.Position = NotyfPosition.TopRight;
});

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseNotyf();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

// creates roles every time program starts
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Buyer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

//using (var scope = app.Services.CreateScope())
//{
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

//    string email = "admin@admin.com";
//    string password = "Test1234";

//    if(await userManager.FindByEmailAsync(email) == null)
//    {
//        var user = new IdentityUser();
//        user.Email = email;
//        await userManager.CreateAsync(user, password);

//        await userManager.AddToRoleAsync(user, "Admin");

//    }

//}

app.Run();
