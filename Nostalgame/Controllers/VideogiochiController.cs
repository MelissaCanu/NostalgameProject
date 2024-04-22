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
    public class VideogiochiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VideogiochiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Videogiochi
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Videogiochi
         .Include(v => v.Genere)
         .Include(v => v.Proprietario)
         .Include(v => v.Noleggi); // Include i noleggi per ogni videogioco
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Videogiochi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videogioco = await _context.Videogiochi
                .Include(v => v.Genere)
                .Include(v => v.Proprietario)
                .FirstOrDefaultAsync(m => m.IdVideogioco == id);
            if (videogioco == null)
            {
                return NotFound();
            }

            return View(videogioco);
        }

        // GET: Videogiochi/Create
        public IActionResult Create()
        {
            ViewData["IdGenere"] = new SelectList(_context.Generi, "IdGenere", "Nome");
            ViewData["IdProprietario"] = new SelectList(_context.Utenti, "Id", "Id");
            return View();
        }

        // POST: Videogiochi/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVideogioco,Titolo,Piattaforma,CasaProduttrice,Descrizione,Foto,Anno,Disponibile,IdGenere,IdProprietario")] Videogioco videogioco)
        {
            if (ModelState.IsValid)
            {
                _context.Add(videogioco);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdGenere"] = new SelectList(_context.Generi, "IdGenere", "Nome", videogioco.IdGenere);
            ViewData["IdProprietario"] = new SelectList(_context.Utenti, "Id", "Id", videogioco.IdProprietario);
            return View(videogioco);
        }

        // GET: Videogiochi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videogioco = await _context.Videogiochi.FindAsync(id);
            if (videogioco == null)
            {
                return NotFound();
            }
            ViewData["IdGenere"] = new SelectList(_context.Generi, "IdGenere", "Nome", videogioco.IdGenere);
            ViewData["IdProprietario"] = new SelectList(_context.Utenti, "Id", "Id", videogioco.IdProprietario);
            return View(videogioco);
        }

        // POST: Videogiochi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVideogioco,Titolo,Piattaforma,CasaProduttrice,Descrizione,Foto,Anno,Disponibile,IdGenere,IdProprietario")] Videogioco videogioco)
        {
            if (id != videogioco.IdVideogioco)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(videogioco);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideogiocoExists(videogioco.IdVideogioco))
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
            ViewData["IdGenere"] = new SelectList(_context.Generi, "IdGenere", "Nome", videogioco.IdGenere);
            ViewData["IdProprietario"] = new SelectList(_context.Utenti, "Id", "Id", videogioco.IdProprietario);
            return View(videogioco);
        }

        // GET: Videogiochi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videogioco = await _context.Videogiochi
                .Include(v => v.Genere)
                .Include(v => v.Proprietario)
                .FirstOrDefaultAsync(m => m.IdVideogioco == id);
            if (videogioco == null)
            {
                return NotFound();
            }

            return View(videogioco);
        }

        // POST: Videogiochi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var videogioco = await _context.Videogiochi.FindAsync(id);
            if (videogioco != null)
            {
                _context.Videogiochi.Remove(videogioco);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VideogiocoExists(int id)
        {
            return _context.Videogiochi.Any(e => e.IdVideogioco == id);
        }
    }
}
