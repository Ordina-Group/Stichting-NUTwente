namespace Ordina.StichtingNuTwente.Models.ViewModels;

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