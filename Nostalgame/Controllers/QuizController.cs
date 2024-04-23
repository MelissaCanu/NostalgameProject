using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nostalgame.Data;
using Nostalgame.Models;

public class QuizController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Utente> _userManager;
    private readonly ILogger<QuizController> _ILogger;

    public QuizController(ApplicationDbContext context, UserManager<Utente> userManager, ILogger<QuizController> ILogger)
    {
        _context = context;
        _userManager = userManager;
        _ILogger = ILogger;
    }

    public IActionResult Index()
    {
        return View(new QuizViewModel());
    }


    [HttpGet]
    public IActionResult Submit()
    {
        return View(new QuizViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Submit(QuizViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Calcola l'avatar in base alle risposte del quiz
        int idAvatar = CalcolaAvatar(model);

        // Assegna l'avatar all'utente
        var userId = _userManager.GetUserId(User);
        var user = _context.Users.Find(userId);
        user.IdAvatar = idAvatar;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }


    private int CalcolaAvatar(QuizViewModel model)
    {
        var risposte = new List<string> { model.Domanda1, model.Domanda2, model.Domanda3, model.Domanda4, model.Domanda5, model.Domanda6, model.Domanda7, model.Domanda8, model.Domanda9, model.Domanda10 };

        var conteggioGeneri = risposte.Where(r => r != null).GroupBy(r => r).ToDictionary(g => g.Key, g => g.Count());
        var generePreferito = conteggioGeneri.OrderByDescending(g => g.Value).First().Key;

        switch (generePreferito)
        {
            case "Platform":
                return 3;
            case "Azione":
                return 4;
            case "RPG":
                return 5;
            case "Horror":
                return 6;
            case "Sparatutto":
                return 7;
            default:
                return 8; // Default in caso di pareggio
        }
    }


}
