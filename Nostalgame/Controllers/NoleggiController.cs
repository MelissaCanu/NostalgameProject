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
    public class NoleggiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NoleggiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Noleggi
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Noleggi.Include(n => n.Noleggiante);
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
                .Include(n => n.Noleggiante)
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
            ViewData["NoleggianteId"] = new SelectList(_context.Utenti, "Id", "Id");
            return View();
        }

        // POST: Noleggi/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdNoleggio,IdVideogioco,DataInizio,DataFine,Restituito,CostoSpedizione,NoleggianteId")] Noleggio noleggio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(noleggio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NoleggianteId"] = new SelectList(_context.Utenti, "Id", "Id", noleggio.NoleggianteId);
            return View(noleggio);
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
            ViewData["NoleggianteId"] = new SelectList(_context.Utenti, "Id", "Id", noleggio.NoleggianteId);
            return View(noleggio);
        }

        // POST: Noleggi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdNoleggio,IdVideogioco,DataInizio,DataFine,Restituito,CostoSpedizione,NoleggianteId")] Noleggio noleggio)
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
            ViewData["NoleggianteId"] = new SelectList(_context.Utenti, "Id", "Id", noleggio.NoleggianteId);
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
                .Include(n => n.Noleggiante)
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
