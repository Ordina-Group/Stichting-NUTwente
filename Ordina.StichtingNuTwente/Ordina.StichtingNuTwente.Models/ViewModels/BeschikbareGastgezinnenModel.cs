namespace Ordina.StichtingNuTwente.Models.ViewModels;

public class BeschikbareGastgezinnenModel : BaseModel
{
    public BeschikbareGastgezinnenModel()
    {
        MijnGastgezinnen = new List<GastgezinViewModel>();
        SortDropdownText = "";
        SearchQueries = new List<SearchQueryViewModel>();
        TotalPlaatsingTag = "";
        TotalResTag = "";
        TotalMaxAdults = 0;
        TotalMaxChildren = 0;
    }

    public List<GastgezinViewModel> MijnGastgezinnen { get; set; }
    public List<SearchQueryViewModel> SearchQueries { get; set; }
        
    public string SortDropdownText { get; set; }
    public string TotalPlaatsingTag { get; set; }
    public string TotalResTag { get; set; }
    public int? TotalMaxAdults { get; set; }
    public int? TotalMaxChildren { get; set; }
}