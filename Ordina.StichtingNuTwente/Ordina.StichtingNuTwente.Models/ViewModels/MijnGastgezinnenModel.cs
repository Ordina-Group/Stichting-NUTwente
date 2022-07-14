using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class MijnGastgezinnenModel : BaseModel
    {
        public MijnGastgezinnenModel(UserDetails user)
        {
            MijnGastgezinnen = new List<GastgezinViewModel>();
            Vrijwilliger = new Vrijwilliger(user);
        }

        public List<GastgezinViewModel> MijnGastgezinnen { get; set; }
        public string? GastgezinnenVan {  get; set; }
        public Vrijwilliger Vrijwilliger { get; set; }
    }
}