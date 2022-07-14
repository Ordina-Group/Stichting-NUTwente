namespace Ordina.StichtingNuTwente.Models.ViewModels;
using Ordina.StichtingNuTwente.Models.Models;

public class Vrijwilliger
{
    public int Id { get; set; }
    public string Naam { get; set; }
    public string Email { get; set; }
    public string Woonplaats { get; set; }
    public string Postcode { get; set; }
    public string Adres { get; set; }

    public Vrijwilliger(UserDetails user)
    {
        Id = user.Id;
        Naam = $"{user.FirstName} {user.LastName}";
        Email = user.Email;
        if (user.Address != null)
        {
            Woonplaats = user.Address.Woonplaats;
            Postcode = user.Address.Postcode;
            Adres = user.Address.Straat;
        }
        else
        {
            Woonplaats = "";
            Postcode = "";
            Adres = "";
        }
    }
}