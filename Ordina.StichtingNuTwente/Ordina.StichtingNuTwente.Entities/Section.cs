namespace Ordina.StichtingNuTwente.Entities
{
    public class Section
    {
        public int Id { get; set; }
        public int Number { get; set; } //Number shown to users 
        public int Title { get; set; }
        public List<Question> Questions { get; set; }
    }
}