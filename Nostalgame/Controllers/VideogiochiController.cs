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
using Microsoft.AspNetCore.Identity;


namespace Nostalgame.Controllers
{
    public class VideogiochiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<AvatarsController> _logger;
        private readonly UserManager<Utente> _userManager;


        public VideogiochiController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, ILogger<AvatarsController> logger, UserManager<Utente> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _userManager = userManager;
        }

        //GET - Videogiochi/Index
        public async Task<IActionResult> Index(string searchField, string searchString)
        {
            var userId = _userManager.GetUserId(User);
            ViewData["UserId"] = userId;
            var videogiochi = from v in _context.Videogiochi
                              select v;

            if (!String.IsNullOrEmpty(searchString))
            {
                var lowerSearchString = searchString.ToLowerInvariant();
                switch (searchField)
                {
                    case "titolo":
                        videogiochi = videogiochi.Where(v => EF.Functions.Like(v.Titolo, $"%{lowerSearchString}%"));
                        break;
                    case "piattaforma":
                        videogiochi = videogiochi.Where(v => EF.Functions.Like(v.Piattaforma, $"%{lowerSearchString}%"));
                        break;
                    case "casaProduttrice":
                        videogiochi = videogiochi.Where(v => EF.Functions.Like(v.CasaProduttrice, $"%{lowerSearchString}%"));
                        break;
                    case "anno":
                        if (int.TryParse(searchString, out int anno))
                        {
                            videogiochi = videogiochi.Where(v => v.Anno == anno);
                        }
                        break;
                    case "genere":
                        videogiochi = videogiochi.Where(v => EF.Functions.Like(v.Genere.Nome, $"%{lowerSearchString}%"));
                        break;
                }
            }

            videogiochi = videogiochi.Include(v => v.Noleggi);
            return View(await videogiochi.ToListAsync());
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
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            ViewData["UserRole"] = roles.FirstOrDefault();
            ViewData["UserId"] = _userManager.GetUserId(User);

            return View(videogioco);
        }

        // GET: Videogiochi/Create
        public IActionResult Create()
        {
            ViewData["IdGenere"] = new SelectList(_context.Generi, "IdGenere", "Nome");
            ViewData["IdProprietario"] = _userManager.GetUserId(User); // Passa l'ID dell'utente alla vista
            return View();
        }

        //POST: Videogiochi/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VideogiocoViewModel videogiocoViewModel)
        {
            if (videogiocoViewModel.File == null)
            {
                ModelState.AddModelError("File", "Il campo file è obbligatorio");
            }

                if (ModelState.IsValid)
            {
                var videogioco = new Videogioco
                {
                    Titolo = videogiocoViewModel.Titolo,
                    Piattaforma = videogiocoViewModel.Piattaforma,
                    CasaProduttrice = videogiocoViewModel.CasaProduttrice,
                    Descrizione = videogiocoViewModel.Descrizione,
                    Anno = videogiocoViewModel.Anno,
                    Disponibile = videogiocoViewModel.Disponibile,
                    IdGenere = videogiocoViewModel.IdGenere,
                    IdProprietario = _userManager.GetUserId(User),//imposto l'id dell'utente loggato come proprietario del videogioco
                    DataCreazione = DateTime.Now
                   

                };

                if (videogiocoViewModel.File != null)
                {
                    var fileName = Path.GetFileName(videogiocoViewModel.File.FileName);
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "img/videogiochi", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await videogiocoViewModel.File.CopyToAsync(fileStream);
                    }

                    videogioco.Foto = "/img/videogiochi/" + fileName;
                }

                _context.Add(videogioco);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(videogiocoViewModel);
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

            var userId = _userManager.GetUserId(User);
            if (videogioco.IdProprietario != userId)
            {
                return Forbid(); // o return RedirectToAction("Index");
            }

            var videogiocoViewModel = new VideogiocoViewModel
            {
                IdVideogioco = videogioco.IdVideogioco,
                Titolo = videogioco.Titolo,
                Piattaforma = videogioco.Piattaforma,
                CasaProduttrice = videogioco.CasaProduttrice,
                Descrizione = videogioco.Descrizione,
                Anno = videogioco.Anno,
                Disponibile = videogioco.Disponibile,
                IdGenere = videogioco.IdGenere,
                IdProprietario = videogioco.IdProprietario,
                Foto = videogioco.Foto
            };

            ViewData["IdGenere"] = new SelectList(_context.Generi, "IdGenere", "Nome", videogioco.IdGenere);
            ViewData["IdProprietario"] = new SelectList(_context.Utenti, "Id", "Id", videogioco.IdProprietario);
            return View(videogiocoViewModel);
        }

        // POST: Videogiochi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVideogioco,Titolo,Piattaforma,CasaProduttrice,Descrizione,Anno,Disponibile,IdGenere,IdProprietario,File")] VideogiocoViewModel videogiocoViewModel)
        {
            if (id != videogiocoViewModel.IdVideogioco)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors })
                    .ToArray();
                foreach (var error in errors)
                {
                    foreach (var subError in error.Errors)
                    {
                        var errorMessage = subError.ErrorMessage;
                        var fieldName = error.Key;

                        // Log the error
                        _logger.LogError($"Campo: {fieldName} Errore: {errorMessage}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Videogioco videogioco = new Videogioco
                    {
                        IdVideogioco = videogiocoViewModel.IdVideogioco,
                        Titolo = videogiocoViewModel.Titolo,
                        Piattaforma = videogiocoViewModel.Piattaforma,
                        CasaProduttrice = videogiocoViewModel.CasaProduttrice,
                        Descrizione = videogiocoViewModel.Descrizione,
                        Anno = videogiocoViewModel.Anno,
                        Disponibile = videogiocoViewModel.Disponibile,
                        IdGenere = videogiocoViewModel.IdGenere,
                        IdProprietario = videogiocoViewModel.IdProprietario,
                    };

                    if (videogiocoViewModel.File != null)
                    {
                        var fileName = Path.GetFileName(videogiocoViewModel.File.FileName);
                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "img/videogiochi", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await videogiocoViewModel.File.CopyToAsync(fileStream);
                        }

                        videogioco.Foto = "/img/videogiochi/" + fileName;
                    }

                    _context.Update(videogioco);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideogiocoExists(videogiocoViewModel.IdVideogioco))
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
            ViewData["IdGenere"] = new SelectList(_context.Generi, "IdGenere", "Nome", videogiocoViewModel.IdGenere);
            ViewData["IdProprietario"] = new SelectList(_context.Utenti, "Id", "Id", videogiocoViewModel.IdProprietario);
            return View(videogiocoViewModel);
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

        // GET: Videogiochi/MieiVideogiochi - Visualizza i videogiochi dell'utente loggato
        public async Task<IActionResult> MieiVideogiochi()
        {
            var userId = _userManager.GetUserId(User);
            var mieiVideogiochi = _context.Videogiochi.Where(v => v.IdProprietario == userId);
            return View(await mieiVideogiochi.ToListAsync());
        }


        private bool VideogiocoExists(int id)
        {
            return _context.Videogiochi.Any(e => e.IdVideogioco == id);
        }
    }
}
