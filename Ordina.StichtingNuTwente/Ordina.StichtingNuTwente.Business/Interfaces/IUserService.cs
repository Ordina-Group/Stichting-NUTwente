using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IUserService
    {
        public UserDetails? GetUserByAADId(string id);
        public UserDetails? UpdateUser(UserDetails user, string aadId);
        public UserDetails? UpdateUserFromProfileEdit(UserDetails user, string aadId);
        public ICollection<UserDetails> GetUsersByRole(string role);
        public void Save(UserDetails user);
    }
}
