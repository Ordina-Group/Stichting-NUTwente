namespace Ordina.StichtingNuTwente.WebApp.Models
{
    public class Answers
    {
        public int Id { get; set; }

        public string value { get; set; }

    }

    public class AnswersViewModel
    {
        public AnswersViewModel()
        {
            answer = new List<Answers>();
        }
        IList<Answers> answer { get; set; }
    }
}
