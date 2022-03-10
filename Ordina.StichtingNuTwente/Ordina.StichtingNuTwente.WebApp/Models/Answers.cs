namespace Ordina.StichtingNuTwente.WebApp.Models
{
    public class Answers
    {
        public int Nummer { get; set; }

        public string Antwoord { get; set; }

    }

    public class AnswersViewModel
    {
        public AnswersViewModel()
        {
            answer = new List<Answers>();
        }
        public List<Answers> answer { get; set; }

        public string id { get; set; }

    }
}
