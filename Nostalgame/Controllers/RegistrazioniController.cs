using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // GET: Registrazioni
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Registrazioni.Include(r => r.Abbonamento).Include(r => r.Utente);
            return View(await applicationDbContext.ToListAsync());
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

        // GET: Registrazioni/Create
        public IActionResult Create()
        {
            var abbonamenti = _context.Abbonamenti.ToList();
            ViewBag.IdAbbonamento = new SelectList(abbonamenti, "IdAbbonamento", "TipoAbbonamento");

            return View();
        }

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

                return RedirectToAction(nameof(Index));
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

        //GET - cambia tipo abbonamento
        public async Task<IActionResult> Edit(int? id)
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

            var viewModel = new RegistrazioneEditViewModel
            {
                IdRegistrazione = registrazione.IdRegistrazione,
                IdAbbonamento = registrazione.IdAbbonamento
            };

            ViewData["IdAbbonamento"] = new SelectList(_context.Abbonamenti, "IdAbbonamento", "TipoAbbonamento", registrazione.IdAbbonamento);
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

            var registrazione = await _context.Registrazioni.FindAsync(id);
            if (registrazione == null)
            {
                return NotFound();
            }

            registrazione.IdAbbonamento = viewModel.IdAbbonamento;

            _context.Update(registrazione);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

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
