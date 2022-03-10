namespace Ordina.StichtingNuTwente.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public int Number { get; set; } //Number shown to users 
        public string Text { get; set; }
        public string Description { get; set; } //Additional text underneath the question
        public string InputType { get; set; } //Input type like text, radio, selection list
        public List<string> InputOptions { get; set; } //Input options for e.g. radio buttons
        public bool Required { get; set; }
    }
}