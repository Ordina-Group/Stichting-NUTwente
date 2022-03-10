namespace Ordina.StichtingNuTwente.Entities
{
    public class Section
    {
        public int Id { get; set; }
        public int Number { get; set; } //Number shown to users 
        public string Title { get; set; }
        public string Header { get; set; } //Text underneath the section title
        public List<Question> Questions { get; set; }
    }
}