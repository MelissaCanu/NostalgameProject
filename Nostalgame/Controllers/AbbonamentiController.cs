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
    public class AbbonamentiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AbbonamentiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Abbonamenti
        public async Task<IActionResult> Index()
        {
            return View(await _context.Abbonamenti.ToListAsync());
        }

        // GET: Abbonamenti/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var abbonamento = await _context.Abbonamenti
                .FirstOrDefaultAsync(m => m.IdAbbonamento == id);
            if (abbonamento == null)
            {
                return NotFound();
            }

            return View(abbonamento);
        }

        // GET: Abbonamenti/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Abbonamenti/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAbbonamento,TipoAbbonamento,CostoMensile")] Abbonamento abbonamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(abbonamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(abbonamento);
        }

        // GET: Abbonamenti/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var abbonamento = await _context.Abbonamenti.FindAsync(id);
            if (abbonamento == null)
            {
                return NotFound();
            }
            return View(abbonamento);
        }

        // POST: Abbonamenti/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAbbonamento,TipoAbbonamento,CostoMensile")] Abbonamento abbonamento)
        {
            if (id != abbonamento.IdAbbonamento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(abbonamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AbbonamentoExists(abbonamento.IdAbbonamento))
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
            return View(abbonamento);
        }

        // GET: Abbonamenti/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var abbonamento = await _context.Abbonamenti
                .FirstOrDefaultAsync(m => m.IdAbbonamento == id);
            if (abbonamento == null)
            {
                return NotFound();
            }

            return View(abbonamento);
        }

        // POST: Abbonamenti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var abbonamento = await _context.Abbonamenti.FindAsync(id);
            if (abbonamento != null)
            {
                _context.Abbonamenti.Remove(abbonamento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AbbonamentoExists(int id)
        {
            return _context.Abbonamenti.Any(e => e.IdAbbonamento == id);
        }
    }
}
