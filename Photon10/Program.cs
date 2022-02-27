using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Photon10.Areas.Identity;
using Photon10.Data;
using Photon10.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Register the application database with EntityFramework
builder.Services.AddDbContextFactory<PlayerDbContext>(options =>
{
    
string connectionString;
    //Heroku deployment is expected to identify self as Production environment
    //If env var is not found, or not Production, assume development PC and load dev's database from local config
    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseSqlServer(connectionString);
}
    //Production environment; so do some fancy magic to get our connection string
    else
{
        var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        var databaseUri = new Uri(connUrl);
        var userInfo = databaseUri.UserInfo.Split(':');

        connectionString = $"Host=" + databaseUri.Host + "; Port=" + databaseUri.Port + "; Username=" + userInfo[0]
                        + "; Password=" + userInfo[1] + "; Database=" + databaseUri.LocalPath.TrimStart('/')
                        + "; SslMode=Require; TrustServerCertificate=true;";
        options.UseNpgsql(connectionString);

    }
    
});
    
    

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//Don't need an identity provider. Left below lines in for posterity or something
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
//TODO: remove all the weather stuff
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //Use default port assignments for local development
    //app.Urls.Clear();
    //app.Urls.Add("http://*" + 80);
    //app.Urls.Add("https://*" + 443);
    
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
