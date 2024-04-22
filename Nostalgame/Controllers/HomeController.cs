using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nostalgame.Data;
using Nostalgame.Models;
using System.Diagnostics;
using System.Linq;
using System;

namespace Nostalgame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {   
            //guid serve a randomizzare l'ordine dei videogiochi, take prende i primi 20, tolistasync li trasforma in lista
            var videogiochi = await _context.Videogiochi.OrderBy(v => Guid.NewGuid()).Take(20).ToListAsync();
            var giochiAppenaAggiunti = await _context.Videogiochi.OrderByDescending(v => v.DataCreazione).Take(9).ToListAsync();

            var model = new HomeViewModel
            {
                TuttiVideogiochi = videogiochi,
                GiochiAppenaAggiunti = giochiAppenaAggiunti
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
