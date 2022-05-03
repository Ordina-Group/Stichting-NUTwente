using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel(UserDetails userDetails)
        {
            FirstName = userDetails.FirstName;
            LastName = userDetails.LastName;
            Email = userDetails.Email;
            Roles = userDetails.Roles;
            PhoneNumber = userDetails.PhoneNumber;
            InDropdown = userDetails.InDropdown;
            Id = userDetails.Id;
        }

        public UserViewModel()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            Roles = new List<string>();
            InDropdown = false;
            Id = -1;
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool InDropdown{ get; set; }
        public ICollection<string> Roles { get; set; }


    }
}
