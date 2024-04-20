
//Aggiungo il contesto del database 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nostalgame.Models;

namespace Nostalgame.Data
{
    // aggiungo l'estensione IdentityDbContext per poter utilizzare Identity per il login
    public class ApplicationDbContext : IdentityDbContext<Utente>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        //dbset per le tabelle
        public DbSet<Utente> Utenti { get; set; }
        public DbSet<Registrazione> Registrazioni { get; set; }
        public DbSet<Abbonamento> Abbonamenti { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<Genere> Generi { get; set; }

        public DbSet<Videogioco> Videogiochi { get; set; }

        public DbSet<PagamentoAbbonamento> PagamentiAbbonamenti { get; set; }

        public DbSet<Noleggio> Noleggi { get; set; }

        public DbSet<CarrelloNoleggio> CarrelliNoleggio { get; set; }


    }
}
