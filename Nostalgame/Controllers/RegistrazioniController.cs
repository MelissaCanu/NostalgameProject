using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nostalgame.Data;
using Nostalgame.Models;
using Stripe;

namespace Nostalgame.Controllers
{
    public class RegistrazioniController : Controller
    {
        private readonly UserManager<Utente> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegistrazioniController> _logger;
        private readonly SignInManager<Utente> _signInManager;
        public RegistrazioniController(UserManager<Utente> userManager, ApplicationDbContext context, ILogger<RegistrazioniController> logger, SignInManager<Utente> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _signInManager = signInManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string emailSearchString, string usernameSearchString)
        {
            var applicationDbContext = _context.Registrazioni.Include(r => r.Abbonamento).Include(r => r.Utente);


            IQueryable<Registrazione> registrazioni = applicationDbContext;

            if (!String.IsNullOrEmpty(emailSearchString))
            {
                registrazioni = registrazioni.Where(s => s.Email.Contains(emailSearchString));
            }

            if (!String.IsNullOrEmpty(usernameSearchString))
            {
                registrazioni = registrazioni.Where(s => s.Utente.UserName.Contains(usernameSearchString));
            }

            return View(await registrazioni.OrderBy(r => r.Cognome).ToListAsync());
        }
            // GET: Registrazioni/Details/5
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registrazione = await _context.Registrazioni
                .Include(r => r.Abbonamento)
                .Include(r => r.Utente)
                .FirstOrDefaultAsync(m => m.IdRegistrazione == id);
            if (registrazione == null)
            {
                return NotFound();
            }



            return View(registrazione);
        }

        [AllowAnonymous]
        // GET: Registrazioni/Create
        public IActionResult Create()
        {
            var abbonamenti = _context.Abbonamenti.ToList();
            ViewBag.IdAbbonamento = new SelectList(abbonamenti, "IdAbbonamento", "TipoAbbonamento");

            return View();
        }

        [AllowAnonymous]
        // POST: Registrazioni/Create

        [HttpPost]
        public async Task<IActionResult> Create(RegistrazioneViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password) || model.Registrazione == null)
            {
                _logger.LogWarning("Invalid model");
                return View(model);
            }

            // Crea un cliente Stripe per l'utente
            var customerOptions = new CustomerCreateOptions
            {
                Name = model.Registrazione.Nome,
                Email = model.Registrazione.Email,
                // Puoi aggiungere qui altri campi come l'indirizzo, il numero di telefono, ecc.
            };
            var customerService = new CustomerService();
            Customer customer = customerService.Create(customerOptions);

            // Controlla se la creazione del cliente Stripe è riuscita
            if (customer == null)
            {
                _logger.LogWarning("Failed to create Stripe customer");
                return View(model);
            }

            var user = new Utente { UserName = model.UserName, Email = model.Registrazione.Email, StripeCustomerId = customer.Id };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                var abbonamento = _context.Abbonamenti.Find(model.Registrazione.IdAbbonamento);
                if (abbonamento == null)
                {
                    _logger.LogWarning("Abbonamento not found: {Id}", model.Registrazione.IdAbbonamento);
                    return View(model);
                }

                model.Registrazione.IdUtente = user.Id;
                model.Registrazione.Utente = user;
                model.Registrazione.Abbonamento = abbonamento;

                _context.Add(model.Registrazione);
                await _context.SaveChangesAsync();

                // Se l'utente ha selezionato un abbonamento premium, reindirizza alla pagina di pagamento
                if (model.Registrazione.IdAbbonamento == 1)
                {
                    // Reindirizza alla pagina di pagamento dell'abbonamento
                    return RedirectToAction("Create", "PagamentoAbbonamenti", new { idUtente = user.Id });
                }

                // Reindirizza alla vista Details per l'utente registrato/loggato
                return RedirectToAction("Details", new { id = model.Registrazione.IdRegistrazione });

            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogWarning("User creation failed: {Code} {Description}", error.Code, error.Description);
                }
            }

            // Se siamo arrivati fin qui, qualcosa è andato storto, mostra di nuovo il form
            return View(model);
        }

        // GET: Registrazioni/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Include l'abbonamento quando recuperi la registrazione
            var registrazione = await _context.Registrazioni
                .Include(r => r.Abbonamento) // Include l'abbonamento
                .FirstOrDefaultAsync(r => r.IdRegistrazione == id);
            
            if (registrazione == null)
            {
                return NotFound();
            }

            // Se l'utente ha già un abbonamento Premium, reindirizza alla vista Details
            if (registrazione.Abbonamento.TipoAbbonamento == "Premium")
            {
                return RedirectToAction("Details", new { id = id });
            }

            var viewModel = new RegistrazioneEditViewModel
            {
                IdRegistrazione = registrazione.IdRegistrazione,
                IdAbbonamento = registrazione.IdAbbonamento
            };

            return View(viewModel);
        }

        //POST - cambia tipo abbonamento

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RegistrazioneEditViewModel viewModel)
        {
            if (id != viewModel.IdRegistrazione)
            {
                return NotFound();
            }

            var registrazione = await _context.Registrazioni
                .Include(r => r.Abbonamento) // Include l'abbonamento
                .Include(r => r.Utente) // Include l'utente
                .FirstOrDefaultAsync(r => r.IdRegistrazione == id); // Trova la registrazione con l'ID specificato

            if (registrazione == null)
            {
                return NotFound();
            }

            // Se l'utente ha già un abbonamento Premium, reindirizza alla vista Details
            if (registrazione.Abbonamento.TipoAbbonamento == "Premium")
            {
                return RedirectToAction("Details", new { id = id });
            }

            // Salva le modifiche
            _context.Update(registrazione);
            await _context.SaveChangesAsync();

            // Reindirizza alla pagina di pagamento
            return RedirectToAction("Create", "PagamentoAbbonamenti", new { idUtente = registrazione.Utente.Id });
        }


        //downgrade

        public async Task<IActionResult> Downgrade(int id)
        {
            // Recupera la registrazione con l'ID specificato
            var registrazione = await _context.Registrazioni
                .Include(r => r.Abbonamento) // Include l'abbonamento
                .Include(r => r.Utente) // Include l'utente
                .FirstOrDefaultAsync(r => r.IdRegistrazione == id); // Trova la registrazione con l'ID specificato

            if (registrazione == null)
            {
                return NotFound();
            }

            // Recupera l'utente
            var user = registrazione.Utente;

            // Recupera il pagamento abbonamento associato all'utente
            var pagamentoAbbonamento = await _context.PagamentiAbbonamenti
                .FirstOrDefaultAsync(p => p.IdUtente == user.Id);

            if (pagamentoAbbonamento == null)
            {
                return NotFound();
            }

            // Recupera la sottoscrizione Stripe dell'utente
            var subscriptionService = new SubscriptionService();
            Subscription subscription = null;

            try
            {
                _logger.LogInformation("Recupero la sottoscrizione Stripe con ID {SubscriptionId}", pagamentoAbbonamento.StripeSubscriptionId);
                subscription = subscriptionService.Get(pagamentoAbbonamento.StripeSubscriptionId);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Errore nel recupero della sottoscrizione Stripe con ID {SubscriptionId}", pagamentoAbbonamento.StripeSubscriptionId);
                return BadRequest("Errore nel recupero della sottoscrizione Stripe");
            }

            // Annulla la sottoscrizione Stripe
            if (subscription != null)
            {
                try
                {
                    _logger.LogInformation("Annullamento della sottoscrizione Stripe con ID {SubscriptionId}", subscription.Id);
                    var options = new SubscriptionCancelOptions { InvoiceNow = true, Prorate = true };
                    subscriptionService.Cancel(subscription.Id, options);
                }
                catch (StripeException ex)
                {
                    _logger.LogError(ex, "Errore nell'annullamento della sottoscrizione Stripe con ID {SubscriptionId}", subscription.Id);
                    return BadRequest("Errore nell'annullamento della sottoscrizione Stripe");
                }
            }
            else
            {
                _logger.LogWarning("Nessuna sottoscrizione Stripe trovata con ID {SubscriptionId}", pagamentoAbbonamento.StripeSubscriptionId);
                return NotFound("Nessuna sottoscrizione Stripe trovata");
            }

            // Aggiorna l'abbonamento della registrazione a Standard
            var abbonamentoStandard = await _context.Abbonamenti.FirstOrDefaultAsync(a => a.TipoAbbonamento == "Standard");
            if (abbonamentoStandard == null)
            {
                return NotFound();
            }

            registrazione.IdAbbonamento = abbonamentoStandard.IdAbbonamento;
            registrazione.Abbonamento = abbonamentoStandard;

            // Salva le modifiche
            _context.Update(registrazione);
            await _context.SaveChangesAsync();

            // Reindirizza l'utente alla pagina di dettaglio
            return RedirectToAction("Details", new { id = id });
        }



        //cambio dettagli registrazione.cs

        public async Task<IActionResult> EditDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registrazione = await _context.Registrazioni.FindAsync(id);
            if (registrazione == null)
            {
                return NotFound();
            }

            var viewModel = new RegistrazioneDetailsViewModel
            {
                IdRegistrazione = registrazione.IdRegistrazione,
                Nome = registrazione.Nome,
                Cognome = registrazione.Cognome,
                Indirizzo = registrazione.Indirizzo,
                Citta = registrazione.Citta,
                Email = registrazione.Email
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDetails(int id, RegistrazioneDetailsViewModel viewModel)
        {
            if (id != viewModel.IdRegistrazione)
            {
                return NotFound();
            }

            var registrazione = await _context.Registrazioni.FindAsync(id);
            if (registrazione == null)
            {
                return NotFound();
            }

            registrazione.Nome = viewModel.Nome;
            registrazione.Cognome = viewModel.Cognome;
            registrazione.Indirizzo = viewModel.Indirizzo;
            registrazione.Citta = viewModel.Citta;
            registrazione.Email = viewModel.Email;

            _context.Update(registrazione);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id });
        }


        //cambio username e password
        public async Task<IActionResult> ChangeUsernamePassword()
        {

            var user = await _userManager.GetUserAsync(User);
            var registrazione = await _context.Registrazioni.FirstOrDefaultAsync(r => r.IdUtente == user.Id);

            if (registrazione == null)
            {
                return NotFound();
            }

            var viewModel = new ChangeUsernamePasswordViewModel
            {
                CurrentUsername = user.UserName,
                IdRegistrazione = registrazione.IdRegistrazione
            };
            ViewBag.IdRegistrazione = registrazione.IdRegistrazione; // Passo l'ID della registrazione alla vista

            return View(viewModel);
        }
    

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUsernamePassword(ChangeUsernamePasswordViewModel model)
        {   

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossibile caricare l'utente con ID '{_userManager.GetUserId(User)}'.");
            }

            if (model.CurrentUsername != user.UserName)
            {
                ModelState.AddModelError(string.Empty, "L'username corrente non è corretto.");
                return View(model);
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!passwordCheck)
            {
                ModelState.AddModelError(string.Empty, "La password corrente non è corretta.");
                return View(model);
            }

            var changeUsernameResult = await _userManager.SetUserNameAsync(user, model.NewUsername);
            if (!changeUsernameResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Errore nel cambiamento dell'username.");
                return View(model);
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Errore nel cambiamento della password.");
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Details", new { id = model.IdRegistrazione });
        }


        [Authorize(Roles = "Admin")]

        // GET: Registrazioni/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registrazione = await _context.Registrazioni
                .Include(r => r.Abbonamento)
                .Include(r => r.Utente)
                .FirstOrDefaultAsync(m => m.IdRegistrazione == id);
            if (registrazione == null)
            {
                return NotFound();
            }

            return View(registrazione);
        }

        [Authorize(Roles = "Admin")]

        // POST: Registrazioni/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registrazione = await _context.Registrazioni.FindAsync(id);
            var user = await _userManager.FindByIdAsync(registrazione.IdUtente); // Trova l'utente associato alla registrazione

            // Rimuovi la registrazione
            _context.Registrazioni.Remove(registrazione);

            // Rimuovi l'utente
            if (user != null)
            {
                try
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        _logger.LogError("Errore nella cancellazione dell'utente {UserId}", user.Id);
                        throw new InvalidOperationException("Errore inaspettato durante la cancellazione dell'utente.");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Se l'utente non esiste più nel database, ignora l'eccezione
                    if (await _userManager.FindByIdAsync(user.Id) != null)
                    {
                        throw;
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Se la registrazione non esiste più nel database, ignora l'eccezione
                var exceptionEntry = ex.Entries.Single();
                var databaseEntry = exceptionEntry.GetDatabaseValues();
                if (databaseEntry == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }




        private bool RegistrazioneExists(int id)
        {
            return _context.Registrazioni.Any(e => e.IdRegistrazione == id);
        }
    }
}
