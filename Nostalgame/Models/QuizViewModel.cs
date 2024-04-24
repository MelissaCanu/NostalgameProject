using System.ComponentModel.DataAnnotations;

public class QuizViewModel
{
    [Required(ErrorMessage = "È richiesta una risposta")]
    public string[] Domande { get; set; } = new string[10];

    public string[] Risposte { get; } = {
        // Risposte per la domanda 1
        "Rilassato", "Adrenalinico", "Dipende dal mio umore", "Preferisco i giochi spaventosi", "Mi piacciono i giochi competitivi",
        // Risposte per la domanda 2
        "Esplorare mondi colorati e fantasiosi", "Combattere contro nemici e boss", "Immergermi in una storia coinvolgente", "Sentire l'adrenalina della paura", "Competere con altri giocatori",
        // Risposte per la domanda 3
        "Mondi fantastici e colorati", "Ambienti urbani o futuristici", "Mondi vasti e dettagliati da esplorare", "Ambienti oscuri e spaventosi", "Campi di battaglia realistici",
        // Risposte per la domanda 4
        "Saltare da piattaforma a piattaforma", "Combattere in battaglie intense", "Sviluppare il mio personaggio e la mia storia", "Risolvere enigmi sotto pressione", "Migliorare le mie abilità di mira",
        // Risposte per la domanda 5
        "Storie semplici e divertenti", "Storie d'azione e avventura", "Storie complesse con molte scelte", "Storie horror e misteriose", "Storie di guerra e conflitto",
        // Risposte per la domanda 6
        "Sfide di abilità e tempismo", "Sfide di combattimento e strategia", "Sfide di strategia e pianificazione", "Sfide di sopravvivenza e risoluzione di enigmi", "Sfide di precisione e reattività",
        // Risposte per la domanda 7
        "Personaggi agili e veloci", "Personaggi forti e potenti", "Personaggi che posso personalizzare", "Personaggi normali in situazioni straordinarie", "Personaggi militari o soldati",
        // Risposte per la domanda 8
        "Grafica colorata e cartoonesca", "Grafica dettagliata e realistica", "Grafica artistica e unica", "Grafica oscura e spaventosa", "Grafica realistica e dettagliata",
        // Risposte per la domanda 9
        "Gameplay basato su abilità e tempismo", "Gameplay basato su azione e combattimento", "Gameplay basato su scelte e strategia", "Gameplay basato su sopravvivenza e paura", "Gameplay basato su precisione e competizione",
        // Risposte per la domanda 10
        "Atmosfera allegra e divertente", "Atmosfera emozionante e intensa", "Atmosfera immersiva e coinvolgente", "Atmosfera spaventosa e tesa", "Atmosfera competitiva e intensa"
    };

    public bool HasAvatar { get; internal set; }
}
