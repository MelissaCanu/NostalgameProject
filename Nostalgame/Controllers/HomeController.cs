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

            //guid serve a randomizzare l'ordine dei videogiochi, take prende i primi 20, tolistasync li trasforma in lista
            var videogiochi = await _context.Videogiochi.OrderBy(v => Guid.NewGuid()).Take(20).ToListAsync();
            var giochiAppenaAggiunti = await _context.Videogiochi.OrderByDescending(v => v.DataCreazione).Take(9).ToListAsync();

            var model = new HomeViewModel
            {
                TuttiVideogiochi = videogiochi,
                GiochiAppenaAggiunti = giochiAppenaAggiunti,
                HasAvatar = user != null && user.IdAvatar != null // Imposta HasAvatar a true se l'utente ha un avatar
            };

            return View(model);
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
