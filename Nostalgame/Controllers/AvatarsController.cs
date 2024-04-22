using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nostalgame.Data;
using Nostalgame.Models;
using Microsoft.AspNetCore.Hosting;

namespace Nostalgame.Controllers
{
    public class AvatarsController : Controller
    {
        //qua inietto i servizi necessari per il funzionamento del controller
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<AvatarsController> _logger;

        //costruttore del controller
        public AvatarsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, ILogger<AvatarsController> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        // GET: Avatars
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Avatars.Include(a => a.Genere);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Avatars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avatar = await _context.Avatars
                .Include(a => a.Genere)
                .FirstOrDefaultAsync(m => m.IdAvatar == id);
            if (avatar == null)
            {
                return NotFound();
            }

            return View(avatar);
        }

        // GET: Avatars/Create
        public IActionResult Create()
        {
            ViewData["IdGenere"] = new SelectList(_context.Generi, "IdGenere", "Nome");
            return View();
        }

        // POST: Avatars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AvatarViewModel avatarViewModel)
        {
            if (ModelState.IsValid)
            {
                Avatar avatar = new Avatar
                {
                    IdAvatar = avatarViewModel.IdAvatar,
                    Nome = avatarViewModel.Nome,
                    IdGenere = avatarViewModel.IdGenere,
                };

                if (avatarViewModel.File != null && avatarViewModel.File.Length > 0)
                {
                    //carico l'immagine nella cartella img/avatars e salvo il percorso nel campo Foto del database 
                    var path = Path.Combine(_hostingEnvironment.WebRootPath, "img/avatars", avatarViewModel.File.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await avatarViewModel.File.CopyToAsync(stream);
                    }
                    avatar.Foto = "/img/avatars/" + avatarViewModel.File.FileName;
                }
                else
                {
                    avatar.Foto = "/img/avatars/defaultavatar.jpeg";
                }

                _context.Add(avatar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Model is not valid");
                foreach (var modelStateKey in ViewData.ModelState.Keys)
                {
                    var modelStateVal = ViewData.ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                    }
                }
            }
            ViewData["IdGenere"] = new SelectList(_context.Generi, "IdGenere", "Nome", avatarViewModel.IdGenere);
            return View(avatarViewModel);
        }


        // GET: Avatars/Edit/5
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
            ViewData["IdGenere"] = new SelectList(_context.Generi, "IdGenere", "Nome", avatar.IdGenere);
            return View(avatar);
        }

        // POST: Avatars/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAvatar,Nome,Foto,IdGenere")] Avatar avatar)
        {
            if (id != avatar.IdAvatar)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (avatar.File != null)
                    {
                        var fileName = Path.GetFileName(avatar.File.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await avatar.File.CopyToAsync(stream);
                        }
                        avatar.Foto = "/img/" + fileName;
                    }
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
            ViewData["IdGenere"] = new SelectList(_context.Generi, "IdGenere", "Nome", avatar.IdGenere);
            return View(avatar);
        }

        // GET: Avatars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avatar = await _context.Avatars
                .Include(a => a.Genere)
                .FirstOrDefaultAsync(m => m.IdAvatar == id);
            if (avatar == null)
            {
                return NotFound();
            }

            return View(avatar);
        }

        // POST: Avatars/Delete/5
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
