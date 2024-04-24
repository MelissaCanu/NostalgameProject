using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Nostalgame.Data;
using Nostalgame.Models;
using Stripe;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;


// CreateBuilder � un metodo statico che crea un nuovo oggetto WebApplicationBuilder - serve per configurare l'applicazione web
var builder = WebApplication.CreateBuilder(args);

//AddControllersWithViews � un metodo di estensione che aggiunge i servizi MVC al contenitore di servizi
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Aggiungo la configurazione della localizzazione qui - questa serve per uniformare il formato delle date e delle valute
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { CultureInfo.InvariantCulture };
    options.DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
//Aggiungo servizi per Identity
builder.Services.AddIdentity<Utente, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Tokens.ProviderMap.Remove("Default"); // Rimuovo il provider di token predefinito
    options.Tokens.AuthenticatorTokenProvider = null; // Imposto il provider di token dell'autenticatore su null
    options.User.RequireUniqueEmail = true; // Richiedo che ogni utente abbia un'email unica
    options.SignIn.RequireConfirmedEmail = false; // Non richiedo che l'email sia confermata per l'accesso
})
.AddEntityFrameworkStores<ApplicationDbContext>();

//Configuro i Cookie 
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14); // Imposto la durata del cookie a 14 giorni

    options.LoginPath = "/Account/Login"; //Path di default se l'utente non � autenticato
    options.AccessDeniedPath = "/Account/Login"; // Path di default se l'utente non ha i permessi
    options.SlidingExpiration = true;
});

//Aggiungo il db context

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//qua var app � un oggetto WebApplication che rappresenta l'applicazione web, builder.Build() costruisce l'applicazione web
var app = builder.Build();

// Ottiengo il contesto del database e i servizi UserManager e RoleManager dal provider di servizi
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<Utente>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Mi assicuro che il database sia stato creato
    context.Database.EnsureCreated();


    //Setto la Stripe API Key
    StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

    // Configuro le richieste HTTP - ovvero le richieste che arrivano al server da un client
    //App.Use � un metodo di estensione che aggiunge un middleware all'applicazione, ovvero un componente che gestisce le richieste HTTP

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/html";

                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();

                if (exceptionHandlerPathFeature?.Error is Exception exception)
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception, "Un'eccezione non gestita si � verificata");
                }

                await context.Response.WriteAsync("Si � verificato un errore. Contatta l'amministratore del sistema.");
            });
        });
        app.UseHsts();
    }


    app.UseHttpsRedirection();

    //questo middleware serve per utilizzare i file statici come immagini, fogli di stile e script JavaScript
    app.UseStaticFiles();

    //l'ordine � importante qui, in quanto il middleware di autenticazione deve essere eseguito prima del middleware di routing,
    //in modo che l'utente venga autenticato prima che la richiesta venga instradata al controller
    app.UseRouting();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
