using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nostalgame.Data;
using Nostalgame.Models;
using Stripe;

namespace Nostalgame.Controllers
{
    public class NoleggiController : Controller
    {   // Dichiarazione delle variabili di classe - servono per accedere al database, all'utente corrente e per registrare i log
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<NoleggiController> _logger;

        public NoleggiController(ApplicationDbContext context, UserManager<Utente> userManager, ILogger<NoleggiController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Modello per la richiesta di pagamento
        // - contiene l'email dell'utente, l'ID del metodo di pagamento e l'ID del noleggio e viene passato al metodo Charge
        public class ChargeRequest
        {
            public string StripeEmail { get; set; }
            public string PaymentMethodId { get; set; }
            public int IdNoleggio { get; set; }
        }


        public async Task<IActionResult> Index()
        {
            var userName = User.Identity.Name;

            if (User.IsInRole("Admin"))
            {
                return View(await _context.Noleggi.ToListAsync());
            }
            else
            {
                var noleggiUtente = _context.Noleggi.Where(n => n.IdUtenteNoleggiante == userName).Include(n => n.Videogioco);
                return View(await noleggiUtente.ToListAsync());
            }
        }



        // GET: Noleggi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noleggio = await _context.Noleggi
                .Include(n => n.Videogioco)
                .FirstOrDefaultAsync(m => m.IdNoleggio == id);
            if (noleggio == null)
            {
                return NotFound();
            }

            return View(noleggio);
        }

        // GET: Noleggi/Create
        public async Task<IActionResult> Create(int idVideogioco)
        {
            // Ottieni l'utente attualmente loggato
            var user = await _userManager.GetUserAsync(User);

            // Ottieni l'abbonamento dell'utente
            var registrazione = await _context.Registrazioni
                .Include(r => r.Abbonamento)
                .FirstOrDefaultAsync(r => r.IdUtente == user.Id);

            if (registrazione == null)
            {
                // Gestisci il caso in cui l'utente non ha un abbonamento
                return NotFound("Abbonamento non trovato per l'utente corrente");
            }

            // Recupera l'oggetto Videogioco dal database
            var videogioco = await _context.Videogiochi.FindAsync(idVideogioco);
            _logger.LogInformation("Inizio metodo Create con idVideogioco: {idVideogioco}", idVideogioco);


            // Verifica se l'oggetto Videogioco è null
            if (videogioco == null)
            {
                _logger.LogWarning("Videogioco con id {idVideogioco} non trovato", idVideogioco);

                // Gestisci il caso in cui non esiste un Videogioco con l'IdVideogioco fornito
                return NotFound("Videogioco non trovato");
            }
            _logger.LogInformation("Videogioco con id {idVideogioco} trovato", idVideogioco);


            // Verifica se il videogioco è disponibile
            if (!videogioco.Disponibile)
            {
                // Gestisci il caso in cui il videogioco non è disponibile
                return BadRequest("Questo videogioco non è attualmente disponibile per il noleggio");
            }

            // Crea un nuovo modello di noleggio
            var noleggioViewModel = new NoleggioViewModel();

            // Imposta l'ID del videogioco e l'ID dell'utente noleggiante nel modello
            noleggioViewModel.IdVideogioco = idVideogioco;
            noleggioViewModel.IdUtenteNoleggiante = user.UserName;
            noleggioViewModel.DataInizio = DateTime.Now;
            noleggioViewModel.DataFine = DateTime.Now.AddDays(10); //imposto data fine a 7 gg da data inizio

            // Verifica se l'utente ha un abbonamento premium
            if (registrazione.Abbonamento.TipoAbbonamento == "Premium")
            {
                // Se l'utente ha un abbonamento premium, il costo del noleggio è 3 e le spese di spedizione sono 0
                noleggioViewModel.CostoNoleggio = 3;
                noleggioViewModel.SpeseSpedizione = 0;
            }
            else
            {
                // Se l'utente non ha un abbonamento premium, il costo del noleggio è 3 e le spese di spedizione sono un valore specifico
                noleggioViewModel.CostoNoleggio = 5;
                noleggioViewModel.SpeseSpedizione = 3; // Imposta questo valore in base alle tue esigenze
            }

            ViewData["IdVideogioco"] = new SelectList(_context.Videogiochi, "IdVideogioco", "Titolo", idVideogioco);
            return View(noleggioViewModel);
        }

        // POST: Noleggi/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVideogioco,IdUtenteNoleggiante,DataInizio,DataFine,IndirizzoSpedizione,CostoNoleggio, SpeseSpedizione")] NoleggioViewModel noleggioViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Log degli errori di validazione del modello
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors });

                foreach (var error in errors)
                {
                    _logger.LogError($"Campo: {error.Key}");
                    foreach (var errorMessage in error.Errors)
                    {
                        _logger.LogError($"Errore: {errorMessage.ErrorMessage}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var noleggio = new Noleggio
                {
                    // Copia le proprietà dal ViewModel al modello
                    IdVideogioco = noleggioViewModel.IdVideogioco,
                    IdUtenteNoleggiante = noleggioViewModel.IdUtenteNoleggiante,
                    DataInizio = noleggioViewModel.DataInizio,
                    DataFine = noleggioViewModel.DataFine,
                    IndirizzoSpedizione = noleggioViewModel.IndirizzoSpedizione,
                    CostoNoleggio = noleggioViewModel.CostoNoleggio,
                    SpeseSpedizione = noleggioViewModel.SpeseSpedizione
                };

                // Trova il videogioco associato al noleggio
                var videogioco = await _context.Videogiochi.FindAsync(noleggio.IdVideogioco);

                // Imposta il videogioco come non disponibile
                if (videogioco != null)
                {
                    videogioco.Disponibile = false;
                    _context.Update(videogioco);
                }

                _context.Add(noleggio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Payment), new { id = noleggio.IdNoleggio });
            }

            ViewData["IdVideogioco"] = new SelectList(_context.Videogiochi, "IdVideogioco", "Titolo", noleggioViewModel.IdVideogioco);
            return View(noleggioViewModel);
        }


        // GET: Noleggi/Payment/5 - Mostra la pagina di pagamento per il noleggio 
        public async Task<IActionResult> Payment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noleggio = await _context.Noleggi.FindAsync(id);


            return View(noleggio);
        }


        // POST: Noleggi/Charge - Effettua il pagamento tramite Stripe e salva il Noleggio nel database 
        //utilizzo FromBody per ricevere i dati in formato JSON - in questo modo posso inviare i dati tramite una richiesta AJAX, ovvero senza ricaricare la pagina
        [HttpPost]
        public async Task<IActionResult> Charge([FromBody] ChargeRequest request)
        {
            if (request == null)
            {
                _logger.LogError("La richiesta è null. Verifica che il corpo della richiesta sia un JSON valido.");
                return Json(new { status = "error", message = "La richiesta è null. Verifica che il corpo della richiesta sia un JSON valido." });
            }

            var customers = new CustomerService();
            var paymentIntents = new PaymentIntentService();

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = request.StripeEmail,
                
            });

            var noleggio = await _context.Noleggi.FindAsync(request.IdNoleggio);
            if (noleggio == null)
            {
                return Json(new { status = "error", message = "Noleggio non trovato" });
            }

            //paymentIntent è l'oggetto che rappresenta il pagamento da effettuare tramite Stripe

            var paymentIntent = paymentIntents.Create(new PaymentIntentCreateOptions
            {   
                //questi dati sono necessari per creare un pagamento tramite Stripe - l'importo è in centesimi
                //addiziono il costo del noleggio e le spese di spedizione per il totale
                Amount = (long)(noleggio.CostoNoleggio + noleggio.SpeseSpedizione) * 100, 
                Currency = "eur",
                Customer = customer.Id,
                PaymentMethod = request.PaymentMethodId,
                Confirm = true, 
                ReturnUrl = "https://localhost:7288/Noleggi/Index", 
            });


            if (paymentIntent.Status == "succeeded")
            {
                noleggio.StripePaymentId = paymentIntent.Id;
                _context.Update(noleggio);
                await _context.SaveChangesAsync();
                TempData["PaymentSuccess"] = true;
                return Json(new { status = "success", message = "Il pagamento è riuscito", redirectUrl = Url.Action("Details", "Noleggi", new { id = noleggio.IdNoleggio }) });
            }
            else
            {
                // Gestisci il caso in cui il pagamento non sia riuscito
                return Json(new { status = "error", message = "Il pagamento non è riuscito" });
            }
        }

        [Authorize(Roles = "Admin")]


        // GET: Noleggi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noleggio = await _context.Noleggi.FindAsync(id);
            if (noleggio == null)
            {
                return NotFound();
            }
            ViewData["IdVideogioco"] = new SelectList(_context.Videogiochi, "IdVideogioco", "CasaProduttrice", noleggio.IdVideogioco);
            return View(noleggio);
        }

        [Authorize(Roles = "Admin")]

        // POST: Noleggi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdNoleggio,IdVideogioco,IdUtenteNoleggiante,DataInizio,DataFine,IndirizzoSpedizione,CostoNoleggio,StripePaymentId")] Noleggio noleggio)
        {
            if (id != noleggio.IdNoleggio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(noleggio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoleggioExists(noleggio.IdNoleggio))
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
            ViewData["IdVideogioco"] = new SelectList(_context.Videogiochi, "IdVideogioco", "CasaProduttrice", noleggio.IdVideogioco);
            return View(noleggio);
        }

        [Authorize(Roles = "Admin")]

        // GET: Noleggi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noleggio = await _context.Noleggi
                .Include(n => n.Videogioco)
                .FirstOrDefaultAsync(m => m.IdNoleggio == id);
            if (noleggio == null)
            {
                return NotFound();
            }

            return View(noleggio);
        }


        [Authorize(Roles = "Admin")]


        // POST: Noleggi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var noleggio = await _context.Noleggi.FindAsync(id);
            var videogioco = await _context.Videogiochi.FindAsync(noleggio.IdVideogioco);
            videogioco.Disponibile = true;
            _context.Noleggi.Remove(noleggio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Noleggi/Restituisci/5
        public async Task<IActionResult> Restituisci(int id)
        {
            var noleggio = await _context.Noleggi.FindAsync(id);
            var videogioco = await _context.Videogiochi.FindAsync(noleggio.IdVideogioco);
            videogioco.Disponibile = true;
            noleggio.DataFine = DateTime.Now; // Aggiorna la data di fine del noleggio
            await _context.SaveChangesAsync();

            TempData["Message"] = "Hai terminato correttamente il noleggio! Riceverai un'email con le istruzioni per il ritiro del videogioco a breve!";
            return RedirectToAction(nameof(Index));
        }


        private bool NoleggioExists(int id)
        {
            return _context.Noleggi.Any(e => e.IdNoleggio == id);
        }
    }
}
