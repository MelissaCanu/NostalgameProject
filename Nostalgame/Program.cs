using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nostalgame.Data;
using Nostalgame.Models;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Adding services for Identity here
builder.Services.AddIdentity<Utente, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Tokens.ProviderMap.Remove("Default"); // Rimuove il provider di token predefinito
    options.Tokens.AuthenticatorTokenProvider = null; // Imposta il provider di token dell'autenticatore su null
    options.User.RequireUniqueEmail = true; // Richiede che ogni utente abbia un'email unica
    options.SignIn.RequireConfirmedEmail = false; // Non richiede che l'email sia confermata per l'accesso
})
.AddEntityFrameworkStores<ApplicationDbContext>();


//Adding DbContext here

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

//Setting Stripe API Key
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

// Configure the HTTP request pipeline.
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

app.UseRouting();

app.UseAuthorization();

//Adding Authentication here
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
