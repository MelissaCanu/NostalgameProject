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

        // Reindirizza l'utente alla vista dell'avatar
        return RedirectToAction("MostraAvatar");
    }


    private int CalcolaAvatar(QuizViewModel model)
    {
        var mappaRisposteGeneri = new Dictionary<string, string>
    {
        // Risposte per il genere "Platform"
        { "Rilassato", "Platform" },
        { "Esplorare mondi colorati e fantasiosi", "Platform" },
        { "Mondi fantastici e colorati", "Platform" },
        { "Saltare da piattaforma a piattaforma", "Platform" },
        { "Storie semplici e divertenti", "Platform" },
        { "Sfide di abilità e tempismo", "Platform" },
        { "Personaggi agili e veloci", "Platform" },
        { "Grafica colorata e cartoonesca", "Platform" },
        { "Gameplay basato su abilità e tempismo", "Platform" },
        { "Atmosfera allegra e divertente", "Platform" },

        // Risposte per il genere "Azione"
        { "Adrenalinico", "Azione" },
        { "Combattere contro nemici e boss", "Azione" },
        { "Ambienti urbani o futuristici", "Azione" },
        { "Combattere in battaglie intense", "Azione" },
        { "Storie d'azione e avventura", "Azione" },
        { "Sfide di combattimento e strategia", "Azione" },
        { "Personaggi forti e potenti", "Azione" },
        { "Grafica dettagliata e realistica", "Azione" },
        { "Gameplay basato su azione e combattimento", "Azione" },
        { "Atmosfera emozionante e intensa", "Azione" },

        // Risposte per il genere "RPG"
        { "Dipende dal mio umore", "RPG" },
        { "Immergermi in una storia coinvolgente", "RPG" },
        { "Mondi vasti e dettagliati da esplorare", "RPG" },
        { "Sviluppare il mio personaggio e la mia storia", "RPG" },
        { "Storie complesse con molte scelte", "RPG" },
        { "Sfide di strategia e pianificazione", "RPG" },
        { "Personaggi che posso personalizzare", "RPG" },
        { "Grafica artistica e unica", "RPG" },
        { "Gameplay basato su scelte e strategia", "RPG" },
        { "Atmosfera immersiva e coinvolgente", "RPG" },

        // Risposte per il genere "Horror"
        { "Preferisco i giochi spaventosi", "Horror" },
        { "Sentire l'adrenalina della paura", "Horror" },
        { "Ambienti oscuri e spaventosi", "Horror" },
        { "Risolvere enigmi sotto pressione", "Horror" },
        { "Storie horror e misteriose", "Horror" },
        { "Sfide di sopravvivenza e risoluzione di enigmi", "Horror" },
        { "Personaggi normali in situazioni straordinarie", "Horror" },
        { "Grafica oscura e spaventosa", "Horror" },
        { "Gameplay basato su sopravvivenza e paura", "Horror" },
        { "Atmosfera spaventosa e tesa", "Horror" },

        // Risposte per il genere "Sparatutto"
        { "Mi piacciono i giochi competitivi", "Sparatutto" },
        { "Competere con altri giocatori", "Sparatutto" },
        { "Campi di battaglia realistici", "Sparatutto" },
        { "Migliorare le mie abilità di mira", "Sparatutto" },
        { "Storie di guerra e conflitto", "Sparatutto" },
        { "Sfide di precisione e reattività", "Sparatutto" },
        { "Personaggi militari o soldati", "Sparatutto" },
        { "Grafica realistica e dettagliata", "Sparatutto" },
        { "Gameplay basato su precisione e competizione", "Sparatutto" },
        { "Atmosfera competitiva e intensa", "Sparatutto" },
    };

        var conteggioGeneri = model.Domande
            .Where(r => r != null && mappaRisposteGeneri.ContainsKey(r))
            .GroupBy(r => mappaRisposteGeneri[r])
            .ToDictionary(g => g.Key, g => g.Count());

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

    [HttpGet]
    public async Task<IActionResult> MostraAvatar()
    {
        var userId = _userManager.GetUserId(User);
        var user = await _context.Users.FindAsync(userId);

        var avatar = await _context.Avatars.FindAsync(user.IdAvatar);

        var model = new MostraAvatarViewModel
        {
            Nome = avatar.Nome,
            ImmagineAvatar = avatar.Foto
        };

        return View(model);
    }


}
