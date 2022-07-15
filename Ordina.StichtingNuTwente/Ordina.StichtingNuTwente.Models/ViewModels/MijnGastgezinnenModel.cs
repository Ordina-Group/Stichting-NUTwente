using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class MijnGastgezinnenModel : BaseModel
    {
        public MijnGastgezinnenModel(UserDetails user, int intakerCount, int buddyCount)
        {
            MijnGastgezinnen = new List<GastgezinViewModel>();
            Vrijwilliger = new Vrijwilliger(user);
            IntakerCount = intakerCount;
            BuddyCount = buddyCount;
            EditAddress = false;
        }

        public List<GastgezinViewModel> MijnGastgezinnen { get; set; }
        public Vrijwilliger Vrijwilliger { get; set; }
        public string? GastgezinnenVan { get; set; }
        public int IntakerCount { get; set; }
        public int BuddyCount { get; set; }
        public bool EditAddress { get; set; }
    }
}