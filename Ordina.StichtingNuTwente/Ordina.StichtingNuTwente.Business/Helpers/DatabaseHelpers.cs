using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordina.StichtingNuTwente.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.DataLayer
{
    public static class DatabaseHelpers
    {
        public static void AddDatabaseContext(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<NuTwenteContext>(
                        options => options.UseSqlServer(configuration.GetConnectionString("NuTwente"))
                        );
        }
    }
}
