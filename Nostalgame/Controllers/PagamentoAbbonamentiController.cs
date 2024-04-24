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
using Stripe;

namespace Nostalgame.Controllers
{
    public class PagamentoAbbonamentiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PagamentoAbbonamentiController> _logger;
        private readonly UserManager<Utente> _userManager;


        public PagamentoAbbonamentiController(ApplicationDbContext context, ILogger<PagamentoAbbonamentiController> logger, UserManager<Utente> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: PagamentoAbbonamenti
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PagamentiAbbonamenti.Include(p => p.Utente);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PagamentoAbbonamenti/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pagamentoAbbonamento = await _context.PagamentiAbbonamenti
                .Include(p => p.Utente)
                .FirstOrDefaultAsync(m => m.IdPagamentoAbbonamento == id);
            if (pagamentoAbbonamento == null)
            {
                return NotFound();
            }

            return View(pagamentoAbbonamento);
        }

        // GET: PagamentoAbbonamenti/Create
        public IActionResult Create(string idUtente)
        {
            // Ottieni l'ID dell'utente correntemente loggato
            var userId = _userManager.GetUserId(User);

            // Verifica se l'utente correntemente loggato corrisponde all'ID dell'utente passato come parametro
            if (userId != idUtente)
            {
                // Se non corrispondono, reindirizza l'utente a una pagina di errore o alla pagina di login
                return RedirectToAction("Login", "Account");
            }

            // Trova l'utente corrispondente
            var utente = _context.Utenti.Find(userId);

            // Trova la registrazione dell'utente
            var registrazione = _context.Registrazioni.FirstOrDefault(r => r.IdUtente == userId);

            // Trova l'abbonamento dell'utente
            var abbonamento = _context.Abbonamenti.FirstOrDefault(a => a.IdAbbonamento == registrazione.IdAbbonamento);

            // Crea un nuovo AbbonamentoViewModel con i dettagli dell'abbonamento dell'utente
            var abbonamentoViewModel = new AbbonamentoViewModel
            {
                IdUtente = userId, // Imposta l'ID dell'utente
                IdAbbonamento = abbonamento.IdAbbonamento, // Imposta l'ID dell'abbonamento
                CostoMensile = abbonamento.CostoMensile, // Imposta il costo annuale
                ImportoPagato = abbonamento.CostoMensile, // Imposta l'importo al costo mensile
                DataPagamento = DateTime.UtcNow // Imposta la data di pagamento a quella corrente in UTC 
            };

            // Passa l'abbonamento alla vista
            ViewBag.Abbonamento = abbonamento;
            return View(abbonamentoViewModel);
        }

        // POST: PagamentoAbbonamenti/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPagamentoAbbonamento,IdUtente,IdAbbonamento,DataPagamento,CostoMensile,ImportoPagato")] AbbonamentoViewModel abbonamentoViewModel, string stripeToken)
        {
            _logger.LogInformation($"Stripe Token: {stripeToken}");
            _logger.LogInformation($"Model is valid: {ModelState.IsValid}");
            _logger.LogInformation("Create method was called.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(p => p.ErrorMessage)).ToList();

                foreach (var error in errors)
                {
                    _logger.LogInformation($"Validation error: {error}");
                }
            }
            if (ModelState.IsValid)
            {
                // Imposta la data e l'ora del pagamento sulla data e l'ora UTC correnti
                abbonamentoViewModel.DataPagamento = DateTime.UtcNow;

                //ottengo l'utente
                var user = await _userManager.FindByIdAsync(abbonamentoViewModel.IdUtente);

                //log per StripeCustomerId
                _logger.LogInformation($"StripeCustomerId: {user.StripeCustomerId}");

                // Crea un metodo di pagamento e collegalo al cliente Stripe
                var options = new PaymentMethodAttachOptions
                {
                    Customer = user.StripeCustomerId,
                };
                var service = new PaymentMethodService();
                PaymentMethod paymentMethod = service.Attach(stripeToken, options);
                // Imposta il metodo di pagamento come predefinito per il cliente
                var customerService = new CustomerService();
                var customerOptions = new CustomerUpdateOptions
                {
                    InvoiceSettings = new CustomerInvoiceSettingsOptions
                    {
                        DefaultPaymentMethod = paymentMethod.Id,
                    },
                };
                Customer customer = customerService.Update(user.StripeCustomerId, customerOptions);

                // Crea una sottoscrizione su Stripe
                var subscriptionOptions = new SubscriptionCreateOptions
                {
                    Customer = user.StripeCustomerId, // Usa l'ID del cliente Stripe dell'utente
                    Items = new List<SubscriptionItemOptions>
            {
                new SubscriptionItemOptions
                {
                    Price = "price_1P6vs7Rs6OMBWVDjmYkq6Ksf", // Sostituisci con l'ID del prezzo Stripe dell'abbonamento
                    Quantity = 1,
                },
            },
                };

                var subscriptionService = new SubscriptionService();
                Subscription subscription = subscriptionService.Create(subscriptionOptions);

                // Crea un nuovo PagamentoAbbonamento
                var pagamentoAbbonamento = new PagamentoAbbonamento
                {
                    IdPagamentoAbbonamento = abbonamentoViewModel.IdPagamentoAbbonamento,
                    IdUtente = abbonamentoViewModel.IdUtente,
                    IdAbbonamento = abbonamentoViewModel.IdAbbonamento,
                    DataPagamento = abbonamentoViewModel.DataPagamento,
                    ImportoPagato = abbonamentoViewModel.CostoMensile, // Assegna CostoMensile a ImportoPagato
                    StripeSubscriptionId = subscription.Id, // Salva l'ID della sottoscrizione Stripe
                };

                // Aggiungi il nuovo PagamentoAbbonamento al contesto e salva le modifiche
                _context.Add(pagamentoAbbonamento);
                await _context.SaveChangesAsync();

                // Reindirizza l'utente all'azione Index
                return RedirectToAction(nameof(Index));
            }

            // Se il modello non è valido, restituisci la vista con il modello originale
            return View(abbonamentoViewModel);
        }


        private void HandleInvoicePaid(Invoice invoice)
        {
            // Trova il pagamento corrispondente all'ID della sottoscrizione Stripe
            var pagamento = _context.PagamentiAbbonamenti.FirstOrDefault(p => p.StripeSubscriptionId == invoice.SubscriptionId);

            if (pagamento != null)
            {
                // Aggiorna il pagamento per riflettere il pagamento riuscito
                pagamento.DataPagamento = DateTime.Now;
                pagamento.ImportoPagato = invoice.Total / 100.0m; // Stripe utilizza centesimi, quindi dividiamo per 100

                _context.Update(pagamento);
                _context.SaveChanges();
            }
            else
            {
                // Gestisci il caso in cui non esiste un pagamento corrispondente
                _logger.LogWarning($"Non è stato trovato nessun pagamento per la sottoscrizione Stripe {invoice.SubscriptionId}");
            }
        }


        //Configuro i webhook di Stripe per ricevere notifiche quando si verifica un evento di fatturazione, come il rinnovo di una sottoscrizione. 
        [HttpPost("stripe/webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);

                // Handle the event
                if (stripeEvent.Type == Events.InvoicePaid)
                {
                    var invoice = stripeEvent.Data.Object as Invoice;
                    // Then define and call a method to handle the successful payment:
                    HandleInvoicePaid(invoice);
                }
                else
                {
                    // Unexpected event type
                    return BadRequest();
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }



        // GET: PagamentoAbbonamenti/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pagamentoAbbonamento = await _context.PagamentiAbbonamenti.FindAsync(id);
            if (pagamentoAbbonamento == null)
            {
                return NotFound();
            }
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Id", pagamentoAbbonamento.IdUtente);
            return View(pagamentoAbbonamento);
        }

        // POST: PagamentoAbbonamenti/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPagamentoAbbonamento,IdUtente,IdAbbonamento,DataPagamento,ImportoPagato")] PagamentoAbbonamento pagamentoAbbonamento)
        {
            if (id != pagamentoAbbonamento.IdPagamentoAbbonamento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pagamentoAbbonamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagamentoAbbonamentoExists(pagamentoAbbonamento.IdPagamentoAbbonamento))
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
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Id", pagamentoAbbonamento.IdUtente);
            return View(pagamentoAbbonamento);
        }

        // GET: PagamentoAbbonamenti/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pagamentoAbbonamento = await _context.PagamentiAbbonamenti
                .Include(p => p.Utente)
                .FirstOrDefaultAsync(m => m.IdPagamentoAbbonamento == id);
            if (pagamentoAbbonamento == null)
            {
                return NotFound();
            }

            return View(pagamentoAbbonamento);
        }

        // POST: PagamentoAbbonamenti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pagamentoAbbonamento = await _context.PagamentiAbbonamenti.FindAsync(id);
            if (pagamentoAbbonamento != null)
            {
                _context.PagamentiAbbonamenti.Remove(pagamentoAbbonamento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PagamentoAbbonamentoExists(int id)
        {
            return _context.PagamentiAbbonamenti.Any(e => e.IdPagamentoAbbonamento == id);
        }
    }
}
