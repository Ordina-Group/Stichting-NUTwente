using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Entities
{
    public class Form
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Header { get; set; } //Text underneath the form title
        public List<Section> Sections { get; set; }

        public UserDetails? UserDetails { get; set; }
        public List<UserDetails> AllUsers { get;} = new List<UserDetails>();
    }
}