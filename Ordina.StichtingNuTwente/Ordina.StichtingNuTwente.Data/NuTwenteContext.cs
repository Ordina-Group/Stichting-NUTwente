using Microsoft.EntityFrameworkCore;
using Ordina.StichtingNuTwente.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Data
{
    public class NuTwenteContext : DbContext
    {
        public NuTwenteContext(DbContextOptions<NuTwenteContext> options): base(options)
        {
            this.Database.EnsureCreated();
        }

        public DbSet<Vrijwilliger> Vrijwilligers { get; set; }
        public DbSet<Gastgezin> Gastgezinnen { get; set; }
    }
}
