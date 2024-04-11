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
    public class AvatarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvatarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Avatar
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Avatars.Include(a => a.Utente);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Avatar/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avatar = await _context.Avatars
                .Include(a => a.Utente)
                .FirstOrDefaultAsync(m => m.IdAvatar == id);
            if (avatar == null)
            {
                return NotFound();
            }

            return View(avatar);
        }

        // GET: Avatar/Create
        public IActionResult Create()
        {
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Id");
            return View();
        }

        // POST: Avatar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAvatar,IdUtente,Nome,Foto")] Avatar avatar)
        {
            if (ModelState.IsValid)
            {
                _context.Add(avatar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Id", avatar.IdUtente);
            return View(avatar);
        }

        // GET: Avatar/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avatar = await _context.Avatars.FindAsync(id);
            if (avatar == null)
            {
                return NotFound();
            }
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Id", avatar.IdUtente);
            return View(avatar);
        }

        // POST: Avatar/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAvatar,IdUtente,Nome,Foto")] Avatar avatar)
        {
            if (id != avatar.IdAvatar)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(avatar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvatarExists(avatar.IdAvatar))
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
            ViewData["IdUtente"] = new SelectList(_context.Utenti, "Id", "Id", avatar.IdUtente);
            return View(avatar);
        }

        // GET: Avatar/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avatar = await _context.Avatars
                .Include(a => a.Utente)
                .FirstOrDefaultAsync(m => m.IdAvatar == id);
            if (avatar == null)
            {
                return NotFound();
            }

            return View(avatar);
        }

        // POST: Avatar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var avatar = await _context.Avatars.FindAsync(id);
            if (avatar != null)
            {
                _context.Avatars.Remove(avatar);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvatarExists(int id)
        {
            return _context.Avatars.Any(e => e.IdAvatar == id);
        }
    }
}
