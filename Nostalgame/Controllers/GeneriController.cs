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
    public class GeneriController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GeneriController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Generi
        public async Task<IActionResult> Index()
        {
            return View(await _context.Generi.ToListAsync());
        }

        // GET: Generi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genere = await _context.Generi
                .FirstOrDefaultAsync(m => m.IdGenere == id);
            if (genere == null)
            {
                return NotFound();
            }

            return View(genere);
        }

        // GET: Generi/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Generi/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdGenere,Nome")] Genere genere)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genere);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genere);
        }

        // GET: Generi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genere = await _context.Generi.FindAsync(id);
            if (genere == null)
            {
                return NotFound();
            }
            return View(genere);
        }

        // POST: Generi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdGenere,Nome")] Genere genere)
        {
            if (id != genere.IdGenere)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genere);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenereExists(genere.IdGenere))
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
            return View(genere);
        }

        // GET: Generi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genere = await _context.Generi
                .FirstOrDefaultAsync(m => m.IdGenere == id);
            if (genere == null)
            {
                return NotFound();
            }

            return View(genere);
        }

        // POST: Generi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genere = await _context.Generi.FindAsync(id);
            if (genere != null)
            {
                _context.Generi.Remove(genere);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GenereExists(int id)
        {
            return _context.Generi.Any(e => e.IdGenere == id);
        }
    }
}
