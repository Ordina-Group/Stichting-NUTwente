using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Models.ViewModels;

public class GastgezinViewModel
{
    public int Id { get; set; }

    public string Naam { get; set; }

    public string Adres { get; set; }

    public string Woonplaats { get; set; }

    public string Postcode { get; set; }

    public string Telefoonnummer { get; set; }

    public string Email { get; set; }

    public string Intaker { get; set; }

    public int IntakerId { get; set; }

    public string Buddy { get; set; }

    public int BuddyId { get; set; }

    public int AanmeldFormulierId { get; set; }

    public DateTime? AanmeldDatum { get; set; }

    public int IntakeFormulierId { get; set; }

    public DateTime? IntakeDatum { get; set; }

    public string? PlaatsingTag { get; set; }

    public string? ReserveTag { get; set; }

    public string? Note { get; set; }

    public PlaatsingsInfo? PlaatsingsInfo { get; set; }

    public bool? HasVOG { get; set; }

    public GastgezinStatus? Status { get; set; }

    public bool HeeftBekeken { get; set; }

    public int? MaxOlderThanTwo { get; set; }

    public int? MaxYoungerThanThree { get; set; }

    public Comment? RejectionComment { get; set; }

    public Comment? IntakeCompletedComment { get; set; }

    public bool Deleted { get; set; }

    public Comment? DeletionComment { get; set; }

    public string VrijwilligerOpmerkingen { get; internal set; }

    public string CoordinatorOpmerkingen { get; internal set; }

    public List<ContactLog> ContactLogs { get; internal set; }

    public DateTime? OnHoldTill { get; internal set; }

    public bool OnHold { get; internal set; }

    public bool NoodOpvang { get; internal set; }
}