using System.ComponentModel.DataAnnotations;

public class QuizViewModel
{
    [Required]
    [Display(Name = "Domanda 1")]
    public string Domanda1 { get; set; }

    [Required]
    [Display(Name = "Domanda 2")]
    public string Domanda2 { get; set; }

    [Required]
    [Display(Name = "Domanda 3")]
    public string Domanda3 { get; set; }

    [Required]
    [Display(Name = "Domanda 4")]
    public string Domanda4 { get; set; }

    [Required]
    [Display(Name = "Domanda 5")]
    public string Domanda5 { get; set; }

    [Required]
    [Display(Name = "Domanda 6")]
    public string Domanda6 { get; set; }

    [Required]
    [Display(Name = "Domanda 7")]
    public string Domanda7 { get; set; }

    [Required]
    [Display(Name = "Domanda 8")]
    public string Domanda8 { get; set; }

    [Required]
    [Display(Name = "Domanda 9")]
    public string Domanda9 { get; set; }

    [Required]
    [Display(Name = "Domanda 10")]
    public string Domanda10 { get; set; }

    public bool HasAvatar { get; internal set; }

}
