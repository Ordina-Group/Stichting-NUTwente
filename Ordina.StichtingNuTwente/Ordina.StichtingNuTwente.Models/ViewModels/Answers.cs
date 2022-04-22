using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class Answers
    {
        public string Nummer { get; set; }

        public string Antwoord { get; set; }

    }

    public class AnswersViewModel : BaseModel
    {
        public AnswersViewModel()
        {
            answer = new List<Answers>();
        }
        public List<Answers> answer { get; set; }

        public string Id { get; set; }

        public DateTime AnswerDate { get; set; }

        //public string UserId { get; set; }

    }

    public class AnswerModel : BaseModel
    {
        public List<AnswerListModel> AnswerLists { get; set; }
    }

    public class AnswerListModel
    {
        public int ReactieId { get; set; }

        public DateTime AnswerDate { get; set; }

        public string FormulierId { get; set; }

        public string FormulierNaam { get; set; }

        public Persoon? Persoon { get; set; }

        //public string UserId { get; set; }
    }
}
