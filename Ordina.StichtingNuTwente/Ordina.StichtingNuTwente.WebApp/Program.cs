using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

using Ordina.StichtingNuTwente.Business;
using Ordina.StichtingNuTwente.Business.DataLayer;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Business.Services;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.WebApp.SceduleTask;
using Microsoft.Graph;
using Azure.Identity;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddSingleton<IHostedService,CleanOnHoldTill>();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromHours(8);
});

builder.Services.AddDatabaseContext(config);
builder.Services.AddScoped<IReactionService, ReactionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGastgezinService, GastgezinService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IPersoonService, PersoonService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IPersoonService, PersoonService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IPlaatsingenService, PlaatsingenService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration, "AzureB2C")
        .EnableTokenAcquisitionToCallDownstreamApi()
        .AddMicrosoftGraphAppOnly(
            graphServiceClient => new GraphServiceClient(
                new ClientSecretCredential(
                    builder.Configuration.GetSection("Graph").GetValue<string>("TenantId"),
                    builder.Configuration.GetSection("AzureB2C").GetValue<string>("ClientId"),
                    builder.Configuration.GetSection("Graph").GetValue<string>("ClientSecret")
                    )
                )
            )
        .AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to 
    // the default policy
    options.AddPolicy("RequireVrijwilligerRole", policy => policy.RequireClaim("groups", "group-vrijwilliger","group-secretariaat", "group-coordinator", "group-superadmin"));
    options.AddPolicy("RequireSecretariaatRole", policy => policy.RequireClaim("groups", "group-secretariaat", "group-coordinator", "group-superadmin"));
    options.AddPolicy("RequireCoordinatorRole", policy => policy.RequireClaim("groups", "group-coordinator", "group-superadmin"));
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
