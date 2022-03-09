namespace Ordina.StichtingNuTwente.Entities
{
    public class Form
    {
        public int Id { get; set; }
        public List<Section> Sections { get; set; }
    }

    public class Section
    {
        public int Id { get; set; }
        public int Number { get; set; } //Number shown to users 
        public List<Question> Questions { get; set; }
    }

    public class Question
    {
        public int Id { get; set; }
        public int Number { get; set; } //Number shown to users 
        public string Text { get; set; }
    }
}