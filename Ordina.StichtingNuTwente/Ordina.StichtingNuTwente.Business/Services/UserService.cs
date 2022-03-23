using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Services
{
    public class UserService : IUserService
    {
        private readonly NuTwenteContext _context;
        public UserService(NuTwenteContext context)
        {
            _context = context;
        }
        public UserDetails? GetUserByAADId(string id)
        {
            var userRepository = new Repository<UserDetails>(_context);
            return userRepository.GetAll().SingleOrDefault(u => u.AADId == id);
        }

        public ICollection<UserDetails> GetUsersByRole(string role)
        {
            var userRepository = new Repository<UserDetails>(_context);
            return userRepository.GetAll().Where(u => u.Roles.Contains(role)).ToList();
        }

        public UserDetails? UpdateUser(UserDetails user, string aadId)
        {
            var userInDB = GetUserByAADId(aadId);
            if (userInDB == null)
            {
                return null;
            }
            var userRepository = new Repository<UserDetails>(_context);
            userInDB.Roles = user.Roles;
            userInDB.Email = user.Email;
            userInDB.FirstName = user.FirstName;
            userInDB.LastName = user.LastName;
            userRepository.Update(userInDB);
            return userInDB;
        }

        public bool Save(UserDetails user)
        {
            var userRepository = new Repository<UserDetails>(_context);
            user.Id = -1;
            var dbModel = userRepository.Create(user);
            return dbModel.Id > 0;
        }
    }
}
