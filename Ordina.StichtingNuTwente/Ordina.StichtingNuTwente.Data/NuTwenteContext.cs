using Microsoft.EntityFrameworkCore;
using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Data
{
    public class NuTwenteContext : DbContext
    {


        public NuTwenteContext(DbContextOptions<NuTwenteContext> options) : base(options)
        {
            this.Database.EnsureCreated();
            this.Database.Migrate();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Gastgezin>().HasMany(x => x.Vluchtelingen).WithOne(x => x.Gastgezin);
            modelBuilder.Entity<Gastgezin>().HasOne(g => g.Begeleider);
            modelBuilder.Entity<Gastgezin>().HasOne(g => g.Contact);
            modelBuilder.Entity<UserDetails>().Property(u => u.Roles).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }

        public DbSet<Vrijwilliger> Vrijwilligers { get; set; }

        public DbSet<Gastgezin> Gastgezinnen { get; set; }

        public DbSet<Antwoord> Antwoorden { get; set; }

        public DbSet<Reactie> Reacties { get; set; }

        public DbSet<Adres> Adres { get; set; }

        public DbSet<Persoon> Persoon { get; set; }
        public DbSet<UserDetails> Users { get; set; }
    }
}
