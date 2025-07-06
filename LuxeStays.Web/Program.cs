using LuxeStays.Application.Common.Interfaces;
using LuxeStays.Infrastructure.Data;
using LuxeStays.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using LuxeStays.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using LuxeStays.Domain.Entities;
using Stripe;
using DotNetEnv;


var builder = WebApplication.CreateBuilder(args);
Env.Load();

builder.Services.AddScoped<IS3Service, S3Service>();

string stripeSecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
string stripePublishableKey = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY");
builder.Configuration["Stripe:SecretKey"] = stripeSecretKey;
builder.Configuration["Stripe:PublishableKey"] = stripePublishableKey;

//string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");



//string? herokuDbUrl = Environment.GetEnvironmentVariable("postgres://ua8it618v4a44p:p45fb6699505b3bfac3925c6aa56b46e7fcdc47c4f525d0689f82cd03c1c3cbdd@c9mq4861d16jlm.cluster-czrs8kj4isg7.us-east-1.rds.amazonaws.com:5432/ddlss5jul0db7o");
//string? herokuDbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");



var connectionString = "Host=c9mq4861d16jlm.cluster-czrs8kj4isg7.us-east-1.rds.amazonaws.com;Port=5432;Database=ddlss5jul0db7o;Username=ua8it618v4a44p;Password=p45fb6699505b3bfac3925c6aa56b46e7fcdc47c4f525d0689f82cd03c1c3cbdd;Trust Server Certificate=true";




//if (!string.IsNullOrEmpty(herokuDbUrl))
//{
//    connectionString = herokuDbUrl;
//}
//else
//{
//    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//}



// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();




//connection to heroku database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));


//connection to local database
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//        options.UseSqlServer(connectionString));





builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
StripeConfiguration.ApiKey = stripeSecretKey;
var app = builder.Build();



//StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

//builder.Services.ConfigureApplicationCookie(option =>
//{
//    option.AccessDeniedPath = "/Account/AccessDenied";
//    option.LoginPath = "/Account/Login";
//});

//builder.Services.Configure<IdentityOptions>(Option =>
//{
//    Option.Password.RequiredLength = 6;
//});
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


//add UseAuthentication 2025/7/5
app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


//auto-migration  2025/7/5
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// for heroku version
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

app.Run();
