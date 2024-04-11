using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nostalgame.Data;
using Nostalgame.Models;

namespace Nostalgame.Controllers
{
    public class PagamentoAbbonamentiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PagamentoAbbonamentiController(ApplicationDbContext context)
        {
            _context = context;
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
        public IActionResult Create()
        {
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Id");
            return View();
        }

        // POST: PagamentoAbbonamenti/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPagamentoAbbonamento,IdUtente,IdAbbonamento,DataPagamento,ImportoPagato")] PagamentoAbbonamento pagamentoAbbonamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pagamentoAbbonamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Id", pagamentoAbbonamento.IdUtente);
            return View(pagamentoAbbonamento);
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
