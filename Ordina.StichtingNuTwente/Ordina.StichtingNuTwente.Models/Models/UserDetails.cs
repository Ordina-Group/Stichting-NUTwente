using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Models
{
    public class UserDetails : BaseEntity
    {
        public UserDetails()
        {
        }
        public string AADId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public virtual Adres? Address { get; set; }
        public bool InDropdown { get; set; }
        public ICollection<string> Roles { get; set; }
        public ICollection<Reactie> Reacties { get; set; }
    }
}
