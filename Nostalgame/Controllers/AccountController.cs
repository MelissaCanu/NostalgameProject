using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Nostalgame.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nostalgame.Controllers
{
    public class AccountController : Controller
    {
        // SignInManager<Utente> è un servizio di ASP.NET Core Identity che ci permette di gestire l'autenticazione degli utenti
        private readonly SignInManager<Utente> _signInManager;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<AccountController> _logger;
        // Costruttore che inizializza il servizio SignInManager<Utente>
        public AccountController(SignInManager<Utente> signInManager, ILogger<AccountController> logger, UserManager<Utente> userManager)
        {
            // Inizializzazione del servizio SignInManager<Utente>
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        // GET - Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST - Login

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Login avvenuto con successo!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                _logger.LogWarning("Login fallito per l'utente {UserName}", model.UserName);
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("L'utente {UserName} è bloccato.", model.UserName);
                }
                else if (result.IsNotAllowed)
                {
                    _logger.LogWarning("Non è permesso l'accesso all'utente {UserName}.", model.UserName);
                }
                ModelState.AddModelError(string.Empty, "Login non valido.");
                return View(model);
            }
        }

        //POST - Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Logout avvenuto con successo!";
            return RedirectToAction("Index", "Home");
        }


    }
}

