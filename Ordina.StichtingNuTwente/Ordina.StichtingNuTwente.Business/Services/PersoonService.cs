using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Business.Services
{
    public class PersoonService: IPersoonService
    {
        private readonly IRepository<Persoon> PersoonRepository;

        public PersoonService(IRepository<Persoon> persoonRepository)
        {
            PersoonRepository = persoonRepository;
        }

        public PersoonService()
        {
        }

        public Persoon? GetPersoon(int id)
        {
            return PersoonRepository.GetById(id);
        }

        public Persoon? GetPersoonByReactieId(int? reactieId)
        {
            return PersoonRepository.GetFirstOrDefault(p => p.Reactie.Id == reactieId);
        }
        public ICollection<Persoon> GetAllPersonen()
        {
            var personen = PersoonRepository.GetAll("Reactie");
            return personen.ToList();
        }
        public ICollection<Persoon> GetAllVluchtelingen()
        {
            var vluchtelingen = PersoonRepository.GetAll("Reactie").Where(v => v.Reactie != null && v.Reactie.FormulierId == 3);
            return vluchtelingen.ToList();
        }
    }
}
