namespace Ordina.StichtingNuTwente.Models.ViewModels;

public class AlleGastgezinnenModel : BaseModel
{
    public AlleGastgezinnenModel()
    {
        Vrijwilligers = new List<Vrijwilliger>();
        Gastgezinnen = new List<GastGezin>();
        SortDropdownText = "";
    }

    public List<Vrijwilliger> Vrijwilligers { get; set; }

    public List<GastGezin> Gastgezinnen { get; set; }

    public string SortDropdownText { get; set; }
}