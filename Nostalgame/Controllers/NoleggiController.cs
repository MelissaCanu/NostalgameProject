using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nostalgame.Data;
using Stripe;

namespace Nostalgame.Controllers
{
    public class NoleggiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NoleggiController> _logger;

        public NoleggiController(ApplicationDbContext context, ILogger<NoleggiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public class ChargeRequest
        {
            public string StripeEmail { get; set; }
            public string PaymentMethodId { get; set; }
            public int IdNoleggio { get; set; }
        }


        // GET: Noleggi
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Noleggi.Include(n => n.Videogioco);
            return View(await applicationDbContext.ToListAsync());
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
        public IActionResult Create()
        {
            ViewData["IdVideogioco"] = new SelectList(_context.Videogiochi, "IdVideogioco", "CasaProduttrice");
            return View();
        }

        // POST: Noleggi/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVideogioco,IdUtenteNoleggiante,DataInizio,DataFine,IndirizzoSpedizione,CostoNoleggio")] NoleggioViewModel noleggioViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Log degli errori di validazione del modello
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors });

                foreach (var error in errors)
                {
                    Console.WriteLine($"Campo: {error.Key}");
                    foreach (var errorMessage in error.Errors)
                    {
                        Console.WriteLine($"Errore: {errorMessage.ErrorMessage}");
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
                };

                _context.Add(noleggio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Payment), new { id = noleggio.IdNoleggio });
            }

            ViewData["IdVideogioco"] = new SelectList(_context.Videogiochi, "IdVideogioco", "CasaProduttrice", noleggioViewModel.IdVideogioco);
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

        [HttpPost]
        public async Task<IActionResult> Charge([FromBody] ChargeRequest request)
        {
            if (request == null)
            {
                _logger.LogError("La richiesta è null. Verifica che il corpo della richiesta sia un JSON valido.");
                return BadRequest();
            }

            var customers = new CustomerService();
            var paymentIntents = new PaymentIntentService();

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = request.StripeEmail,
                // Aggiungi qui eventuali altre opzioni
            });

            var noleggio = await _context.Noleggi.FindAsync(request.IdNoleggio);
            if (noleggio == null)
            {
                return NotFound();
            }

            var paymentIntent = paymentIntents.Create(new PaymentIntentCreateOptions
            {
                Amount = (long)noleggio.CostoNoleggio * 100, // Stripe richiede l'importo in centesimi
                Currency = "eur",
                Customer = customer.Id,
                PaymentMethod = request.PaymentMethodId,
                Confirm = true, // Conferma immediatamente il PaymentIntent
                ReturnUrl = "https://localhost:7288/Noleggi/Index", // Aggiungi il tuo URL di ritorno qui
            });


            if (paymentIntent.Status == "succeeded")
            {
                noleggio.StripePaymentId = paymentIntent.Id;
                _context.Update(noleggio);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                // Gestisci il caso in cui il pagamento non sia riuscito
                return BadRequest();
            }
        }


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

        // POST: Noleggi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var noleggio = await _context.Noleggi.FindAsync(id);
            if (noleggio != null)
            {
                _context.Noleggi.Remove(noleggio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoleggioExists(int id)
        {
            return _context.Noleggi.Any(e => e.IdNoleggio == id);
        }
    }
}
