using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nostalgame.Data;
using Nostalgame.Models;
using System.Diagnostics;
using System.Linq;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Nostalgame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Utente> _userManager;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, UserManager<Utente> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [AllowAnonymous]

        public async Task<IActionResult> Index()
        {
            // Recupera l'utente corrente
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.FindAsync(userId);

            //recupero le piattaforme per il filtro per il random game
            var piattaforme = await _context.Videogiochi.Select(v => v.Piattaforma).Distinct().ToListAsync();


            //guid serve a randomizzare l'ordine dei videogiochi, take prende i primi 20, tolistasync li trasforma in lista
            var videogiochi = await _context.Videogiochi.OrderBy(v => Guid.NewGuid()).Take(20).ToListAsync();
            var giochiAppenaAggiunti = await _context.Videogiochi.OrderByDescending(v => v.DataCreazione).Take(9).ToListAsync();

            var model = new HomeViewModel
            {
                TuttiVideogiochi = videogiochi,
                GiochiAppenaAggiunti = giochiAppenaAggiunti,
                HasAvatar = user != null && user.IdAvatar != null, // Imposta HasAvatar a true se l'utente ha un avatar
                Piattaforme = piattaforme // Aggiungo le piattaforme al modello

            };

            return View(model);
        }

        // GET: videogioco random
        public async Task<IActionResult> RandomGame(string piattaforma)
        {
            var videogiochiDisponibili = await _context.Videogiochi
                .Where(v => v.Disponibile && (string.IsNullOrEmpty(piattaforma) || v.Piattaforma == piattaforma))
                .ToListAsync();

            if (!videogiochiDisponibili.Any())
            {
                // Gestisci il caso in cui non ci sono videogiochi disponibili
                return View("NoGamesAvailable");
            }

            var random = new Random();
            var randomGame = videogiochiDisponibili[random.Next(videogiochiDisponibili.Count)];

            return PartialView("_RandomGame", randomGame);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
