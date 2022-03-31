using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

using Ordina.StichtingNuTwente.Business;
using Ordina.StichtingNuTwente.Business.DataLayer;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Business.Services;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromHours(8);
});

builder.Services.AddScoped<IFormBusiness, FormBusiness>();

builder.Services.AddDatabaseContext(config);
builder.Services.AddScoped<IReactionService, ReactionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGastgezinService, GastgezinService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IMailService, MailService>();


builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureB2C"));

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to 
    // the default policy
    options.AddPolicy("RequireVrijwilligerRole", policy => policy.RequireClaim("groups", "group-vrijwilliger"));
    options.AddPolicy("RequireSecretariaatRole", policy => policy.RequireClaim("groups", "group-secretariaat"));
    options.AddPolicy("RequireCoördinatorRole", policy => policy.RequireClaim("groups", "group-coördinator"));
    options.AddPolicy("RequireSuperAdminRole", policy => policy.RequireClaim("groups", "group-superadmin"));
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddRazorPages(options => {})
        .AddMvcOptions(options => { })
        .AddMicrosoftIdentityUI();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Overview}/{id?}");

app.Run();
