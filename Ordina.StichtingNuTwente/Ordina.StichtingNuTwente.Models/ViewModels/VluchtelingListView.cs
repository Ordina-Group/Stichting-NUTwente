using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Models.ViewModels;

public class VluchtelingListView
{
    public VluchtelingListView(Persoon vluchteling) {
        Naam = vluchteling.Naam;
        Email = vluchteling.Email;
        Telefoonnummer = vluchteling.Telefoonnummer;
        Gastgezin = vluchteling.Gastgezin;
        Reactie = vluchteling.Reactie;
    }
    public string Naam { get; set; }
    public string Email { get; set; }
    public string Telefoonnummer { get; set; }
    public Gastgezin? Gastgezin { get; set; }
    public Reactie? Reactie { get; set; }

}