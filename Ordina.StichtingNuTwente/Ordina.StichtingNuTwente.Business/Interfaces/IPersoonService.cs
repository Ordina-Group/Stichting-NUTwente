using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IPersoonService
    {
        public Persoon? getPersoon(int id);
        public Persoon? getPersoonByReactionId(int reactionId);
        public Persoon? GetPersoon(int id);
        public Persoon? GetPersoonByReactieId(int? reactionId);
        public ICollection<Persoon> GetAllPersonen();
        public ICollection<Persoon> GetAllVluchtelingen();
    }
}
