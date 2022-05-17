using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Models.ViewModels;

public class PlaatsingViewModel
{
    public PlaatsingViewModel(Plaatsing plaatsing)
    {
        Gastgezin = plaatsing.Gastgezin;
        Amount = plaatsing.Amount;
        AgeGroup = plaatsing.AgeGroup;
        PlacementType = plaatsing.PlacementType;
        DateTime = plaatsing.DateTime;
        Vrijwilliger = plaatsing.Vrijwilliger;
        Age = plaatsing.Age;
        Gender = plaatsing.Gender;
        Active = plaatsing.Active;
        Id = plaatsing.Id;
        Edit = false;
    }

    public Gastgezin Gastgezin { get; set; }

    public int Amount { get; set; }

    public AgeGroup AgeGroup { get; set; }

    public PlacementType PlacementType { get; set; }

    public DateTime DateTime { get; set; }

    public UserDetails Vrijwilliger { get; set; }

    public int Age { get; set; }

    public Gender Gender { get; set; }

    public bool Active { get; set; }

    public int Id { get; set; }

    public bool Edit { get; set; }
}