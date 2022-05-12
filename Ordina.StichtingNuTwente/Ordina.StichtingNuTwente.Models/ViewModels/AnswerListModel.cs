using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Models.ViewModels;

public class AnswerListModel
{
    public int ReactieId { get; set; }

    public DateTime AnswerDate { get; set; }

    public string FormulierId { get; set; }

    public string FormulierNaam { get; set; }

    public Persoon? Persoon { get; set; }

    //public string UserId { get; set; }
}