using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IPersoonService
    {
        public Persoon? GetPersoon(int id);
        public Persoon? GetPersoonByReactieId(int? reactionId);
        public ICollection<Persoon> GetAllPersonen();
        public ICollection<Persoon> GetAllVluchtelingen();
    }
}
