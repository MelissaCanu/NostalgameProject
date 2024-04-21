using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Nostalgame.Data;
using Nostalgame.Models;
using Stripe;
using System.Globalization;

// CreateBuilder è un metodo statico che crea un nuovo oggetto WebApplicationBuilder - serve per configurare l'applicazione web
var builder = WebApplication.CreateBuilder(args);

//AddControllersWithViews è un metodo di estensione che aggiunge i servizi MVC al contenitore di servizi
builder.Services.AddControllersWithViews();

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


//Aggiungo il db context

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//qua var app è un oggetto WebApplication che rappresenta l'applicazione web, builder.Build() costruisce l'applicazione web
var app = builder.Build();

//Setto la Stripe API Key
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

// Configuro le richieste HTTP - ovvero le richieste che arrivano al server da un client
//App.Use è un metodo di estensione che aggiunge un middleware all'applicazione, ovvero un componente che gestisce le richieste HTTP

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
                logger.LogError(exception, "Un'eccezione non gestita si è verificata");
            }

            await context.Response.WriteAsync("Si è verificato un errore. Contatta l'amministratore del sistema.");
        });
    });
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

//l'ordine è importante qui, in quanto il middleware di autenticazione deve essere eseguito prima del middleware di routing,
//in modo che l'utente venga autenticato prima che la richiesta venga instradata al controller
app.UseRouting();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
