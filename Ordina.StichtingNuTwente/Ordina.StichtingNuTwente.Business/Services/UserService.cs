using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            return userRepository.GetAll().FirstOrDefault(u => u.AADId == id);
        }

        public UserDetails? GetUserById(int id)
        {
            var userRepository = new Repository<UserDetails>(_context);
            return userRepository.GetAll().FirstOrDefault(u => u.Id == id);
        }

        public ICollection<UserDetails> GetUsersByRole(string role)
        {
            var userRepository = new Repository<UserDetails>(_context);
            return userRepository.GetAll().Where(u => u.Roles.Contains(role)).ToList();
        }

        public ICollection<UserDetails> GetAllUsers()
        {
            var userRepository = new Repository<UserDetails>(_context);
            return userRepository.GetAll().ToList();
        }

        public ICollection<Reactie> GetMyReacties(string AADId)
        {
            var userRepository = new Repository<UserDetails>(_context);
            var reactions = userRepository.GetAll("Reacties").FirstOrDefault(u => u.AADId == AADId).Reacties;
            return reactions;
        }
        private UserDetails? UpdateUser(UserDetails user, UserDetails userInDB)
        {
            var userRepository = new Repository<UserDetails>(_context);
            userInDB.Roles = user.Roles;
            userInDB.Email = user.Email;
            userInDB.FirstName = user.FirstName;
            userInDB.LastName = user.LastName;
            userInDB.PhoneNumber = user.PhoneNumber;
            userInDB.InDropdown = user.InDropdown;
            userRepository.Update(userInDB);
            return userInDB;
        }

        public UserDetails? UpdateUser(UserDetails user, string aadId)
        {
            var userInDB = GetUserByAADId(aadId);
            if (userInDB == null)
            {
                return null;
            }
            return UpdateUser(user, userInDB);
        }
        public UserDetails? UpdateUser(UserDetails user, int id)
        {
            var userInDB = GetUserById(id);
            if (userInDB == null)
            {
                return null;
            }
            return UpdateUser(user, userInDB);

        }

        public UserDetails? UpdateUserFromProfileEdit(UserDetails user, string aadId)
        {
            var userInDB = GetUserByAADId(aadId);
            if (userInDB == null)
            {
                return null;
            }
            var userRepository = new Repository<UserDetails>(_context);
            userInDB.FirstName = user.FirstName;
            userInDB.LastName = user.LastName;
            userInDB.PhoneNumber = user.PhoneNumber;
            userRepository.Update(userInDB);
            return userInDB;
        }

        public void Save(UserDetails user)
        {
            var userRepository = new Repository<UserDetails>(_context);
            var dbModel = userRepository.Create(user);
        }

        public void checkIfUserExists(ClaimsPrincipal user)
        {
            var aadID = user.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
            if (aadID != null)
            {
                var userDetails = GetUserByAADId(aadID.Value);
                var email = user.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress"))?.Value;
                var givenname = user.Claims.FirstOrDefault(c => c.Type.Contains("givenname"))?.Value;
                var surname = user.Claims.FirstOrDefault(c => c.Type.Contains("surname"))?.Value;
                var phoneNumber = user.Claims.FirstOrDefault(c => c.Type.Contains("phone_number"))?.Value;
                var groups = user.Claims.Where(c => c.Type.Contains("group")).Select(x => x.Value);
                if (givenname == null)
                    givenname = "";
                if (surname == null)
                    surname = "";
                if (phoneNumber == null)
                    phoneNumber = "";
                if (userDetails != null)
                {

                    if (userDetails.FirstName != givenname ||
                        userDetails.LastName != surname ||
                        userDetails.Email != email ||
                        userDetails.PhoneNumber != phoneNumber ||
                        !userDetails.Roles.All(groups.Contains) ||
                        !groups.All(userDetails.Roles.Contains))
                    {
                        var newUserDetails = new UserDetails()
                        {
                            FirstName = givenname,
                            LastName = surname,
                            Email = email,
                            PhoneNumber = phoneNumber,
                            Roles = groups.ToList(),
                            AADId = aadID.Value
                        };
                        if (user.HasClaim("http://schemas.microsoft.com/claims/authnclassreference", "b2c_1a_profileedit")) UpdateUserFromProfileEdit(newUserDetails, aadID.Value);
                        else UpdateUser(newUserDetails, aadID.Value);
                    }
                }
                else
                {
                    var newUserDetails = new UserDetails()
                    {
                        FirstName = givenname,
                        LastName = surname,
                        Email = email,
                        PhoneNumber = phoneNumber,
                        Roles = groups.ToList(),
                        AADId = aadID.Value
                    };
                    Save(newUserDetails);
                }
            }
        }

        public UserDetails? getUserFromClaimsPrincipal(ClaimsPrincipal user)
        {
            var aadId = user.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            var userRepository = new Repository<UserDetails>(_context);
            return userRepository.GetAll().FirstOrDefault(u => u.AADId == aadId);
        }

        public ICollection<UserDetails> GetAllDropdownUsers()
        {
            var userRepository = new Repository<UserDetails>(_context);

            return userRepository.GetAll().Where(u => u.InDropdown).ToList();
        }
    }
}
