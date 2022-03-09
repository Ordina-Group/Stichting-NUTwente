namespace Ordina.StichtingNuTwente.Entities
{
    public class Form
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Header { get; set; } //Text underneath the form title
        public List<Section> Sections { get; set; }
    }
}