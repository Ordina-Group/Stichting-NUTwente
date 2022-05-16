namespace Ordina.StichtingNuTwente.Models.Models;

[Flags]
public enum PlacementType
{
    Reservering = 0,
    Plaatsing = 1,
    GeplaatsteReservering = 2,
    VerwijderdePlaatsing = 3,
    VerwijderdeReservering =4
}