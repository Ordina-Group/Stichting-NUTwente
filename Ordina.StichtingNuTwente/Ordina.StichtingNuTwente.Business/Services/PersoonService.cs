using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Business.Services
{
    public class PersoonService: IPersoonService
    {
        private readonly NuTwenteContext _context;
        public PersoonService(NuTwenteContext context)
        {
            _context = context;
        }

        public Persoon? GetPersoon(int id)
        {
            var persoonRepository = new Repository<Persoon>(_context);
            return persoonRepository.GetById(id);
        }

        public Persoon? GetPersoonByReactieId(int? reactieId)
        {
            var persoonRepository = new Repository<Persoon>(_context);
            return persoonRepository.GetFirstOrDefault(p => p.Reactie.Id == reactieId);
        }
    }
}
