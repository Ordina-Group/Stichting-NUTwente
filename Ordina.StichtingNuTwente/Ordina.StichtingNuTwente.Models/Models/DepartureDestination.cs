﻿using System.ComponentModel.DataAnnotations;

namespace Ordina.StichtingNuTwente.Models.Models;

[Flags]
public enum DepartureDestination
{
    [Display(Name = "Onbekend")]
    Onbekend = 0,
    [Display(Name = "Terugkeer naar Oekraïne")]
    Oekraine = 1,
    [Display(Name = "Vertrek naar bestemming binnen Nederland")]
    BinnenNederland = 2,
    [Display(Name = "Vertrek naar bestemming buiten Nederland (niet Oekraïne)")]
    BuitenNederland = 3,
    [Display(Name = "Administratieve Correctie")]
    Correctie = 4,
    [Display(Name = "Herplaatsing bij ander gastgezin NuTwente")]
    Herplaatsing = 5,
}