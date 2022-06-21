using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Mappings;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
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
        private readonly IRepository<UserDetails> UserDetailRepository;
        private readonly IRepository<Persoon> PersoonRepository;
        public UserService(IRepository<UserDetails> userDetailRepository, IRepository<Persoon> persoonRepository)
        {
            UserDetailRepository = userDetailRepository;
            PersoonRepository = persoonRepository;
        }
        public UserDetails? GetUserByAADId(string id)
        {
            return UserDetailRepository.GetAll().FirstOrDefault(u => u.AADId == id);
        }

        public UserDetails? GetUserById(int id)
        {
            return UserDetailRepository.GetAll().FirstOrDefault(u => u.Id == id);
        }

        public ICollection<UserDetails> GetUsersByRole(string role)
        {
            return UserDetailRepository.GetAll().Where(u => u.Roles.Contains(role)).ToList();
        }

        public ICollection<UserDetails> GetAllUsers()
        {
            return UserDetailRepository.GetAll().ToList();
        }

        public List<AnswerListModel> GetMyReacties(string AADId)
        {
            List<AnswerListModel> viewModel = new List<AnswerListModel>();
            var reacties = UserDetailRepository.GetAll("Reacties").FirstOrDefault(u => u.AADId == AADId).Reacties.Where(r => !r.Deleted);
            var people = PersoonRepository.GetAll("Reactie,Adres");

            var t = from reactie in reacties
                    join add in people
                    on reactie.Id equals add.Reactie?.Id
                    into PeopleReaction
                    from persoon in PeopleReaction.DefaultIfEmpty()
                    select new { reactie, persoon };

            viewModel = t.OrderByDescending(x => x.reactie.Id).ToList().ConvertAll(p =>
            {
                var awnser = ReactieMapping.FromDatabaseToWebListModel(p.reactie);
                awnser.Persoon = p.persoon;
                return awnser;
            });
            return viewModel;
        }
        private UserDetails? UpdateUser(UserDetails user, UserDetails userInDB)
        {
            userInDB.Roles = user.Roles;
            userInDB.Email = user.Email;
            userInDB.FirstName = user.FirstName;
            userInDB.LastName = user.LastName;
            userInDB.PhoneNumber = user.PhoneNumber;
            userInDB.InDropdown = user.InDropdown;
            UserDetailRepository.Update(userInDB);
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
            userInDB.FirstName = user.FirstName;
            userInDB.LastName = user.LastName;
            userInDB.PhoneNumber = user.PhoneNumber;
            UserDetailRepository.Update(userInDB);
            return userInDB;
        }

        public void Save(UserDetails user)
        {
            var dbModel = UserDetailRepository.Create(user);
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
            return UserDetailRepository.GetAll().FirstOrDefault(u => u.AADId == aadId);
        }

        public ICollection<UserDetails> GetAllDropdownUsers()
        {
            return UserDetailRepository.GetAll().Where(u => u.InDropdown).ToList();
        }
    }
}
