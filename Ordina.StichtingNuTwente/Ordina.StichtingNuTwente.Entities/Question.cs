namespace Ordina.StichtingNuTwente.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public int Number { get; set; } //Number shown to users 
        public string Text { get; set; }
        public string Description { get; set; }
        public string InputType { get; set; } //Input type like Textbox, Selection List
        public List<String> InputOptions { get; set; }
        public bool Required { get; set; }
    }
}