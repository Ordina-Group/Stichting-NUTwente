using System.ComponentModel.DataAnnotations;

namespace Ordina.StichtingNuTwente.Models.Models;

[Flags]
public enum DepartureDestination
{
    [Display(Name = "Terugkeer naar Oekraïne.")]
    Oekraine = 1,
    [Display(Name = "Vertrek naar bestemming binnen Nederland.")]
    BinnenNederland = 2,
    [Display(Name = "Vertrek naar bestemming buiten Nederland (niet Oekraïne).")]
    BuitenNederland = 3
}