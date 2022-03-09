namespace Ordina.StichtingNuTwente.Entities
{
    public class Form
    {
        public List<Section> Sections { get; set; }
    }

    public class Section
    {
        public List<Question> Questions { get; set; }
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}