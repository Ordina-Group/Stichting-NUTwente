
using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class InputPartialViewModel
    {
        public Question Question { get; set; }
        public UserDetails? UserDetails { get; set; }
        public List<UserDetails> AllUsers { get; set; }
    }
}
