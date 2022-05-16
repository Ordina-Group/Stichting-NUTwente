using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Models.ViewModels;

public class VluchtelingViewModel
{
    public VluchtelingViewModel()
    {
        Naam = "";
        GeboorteDatum = "";
        Geboorteplaats = "";
        Email = "";
        Telefoonnummer = "";
        Mobiel = "";
        Nationaliteit = "";
        Talen = "";
    }

    public string Naam { get; set; }
    public string GeboorteDatum { get; set; }
    public string Geboorteplaats { get; set; }
    public string Email { get; set; }
    public string Telefoonnummer { get; set; }
    public string Mobiel { get; set; }
    public string Nationaliteit { get; set; }
    public string Talen { get; set; }
    public Gastgezin? Gastgezin { get; set; }
    public List<Reactie>? Reactie { get; set; }
    public Adres? Adres { get; set; }
}