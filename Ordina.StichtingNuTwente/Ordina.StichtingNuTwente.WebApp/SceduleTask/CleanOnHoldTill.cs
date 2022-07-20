using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.WebApp.BackgroundService;

namespace Ordina.StichtingNuTwente.WebApp.SceduleTask
{
    public class CleanOnHoldTill : ScheduledProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CleanOnHoldTill(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override string Schedule => "0 3 * * *"; // every day at 3 AM 

        public override async Task ProcessInScope(IServiceProvider scopeServiceProvider)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IGastgezinService _gastgezinService = scope.ServiceProvider.GetService<IGastgezinService>();
                _gastgezinService.CheckOnholdGastgezinnen();
            }

            Console.WriteLine("SampleTask1 : " + DateTime.Now.ToString());

            // return Task.CompletedTask;


            await Task.Run(() => {
                return Task.CompletedTask;
            });
        }
    }
}
