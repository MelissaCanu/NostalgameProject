using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nostalgame.Data;
using Nostalgame.Models;

namespace Nostalgame.Controllers
{
    public class RegistrazioniController : Controller
    {
        private readonly UserManager<Utente> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegistrazioniController> _logger;

        public RegistrazioniController(UserManager<Utente> userManager, ApplicationDbContext context, ILogger<RegistrazioniController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;

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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(RegistrazioneViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password) || model.Registrazione == null)
            {
                _logger.LogWarning("Invalid model");
                return View(model);
            }

            var user = new Utente { UserName = model.UserName, Email = model.Registrazione.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
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

                return RedirectToAction(nameof(Index));
            }
            else
            {
                _logger.LogWarning("User creation failed: {Errors}", result.Errors);
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

            var registrazione = await _context.Registrazioni.FindAsync(id);
            if (registrazione == null)
            {
                return NotFound();
            }
            ViewData["IdAbbonamento"] = new SelectList(_context.Abbonamenti, "IdAbbonamento", "TipoAbbonamento", registrazione.IdAbbonamento);
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Id", registrazione.IdUtente);
            return View(registrazione);
        }

        // POST: Registrazioni/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRegistrazione,IdUtente,IdAbbonamento,Nome,Cognome,Indirizzo,Citta,Email")] Registrazione registrazione)
        {
            if (id != registrazione.IdRegistrazione)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registrazione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistrazioneExists(registrazione.IdRegistrazione))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAbbonamento"] = new SelectList(_context.Abbonamenti, "IdAbbonamento", "TipoAbbonamento", registrazione.IdAbbonamento);
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Id", registrazione.IdUtente);
            return View(registrazione);
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
