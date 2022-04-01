using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ICollection<Persoon> GetAllPersonen()
        {
            var persoonRepository = new Repository<Persoon>(_context);

            var personen = persoonRepository.GetAll("Reactie");
            return personen.ToList();
        }
        public ICollection<Persoon> GetAllVluchtelingen()
        {
            var persoonRepository = new Repository<Persoon>(_context);

            var vluchtelingen = persoonRepository.GetAll("Reactie").Where(v => v.Reactie != null && v.Reactie.FormulierId == 3);
            return vluchtelingen.ToList();
        }
    }
}
