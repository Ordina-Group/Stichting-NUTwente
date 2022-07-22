using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public ICollection<UserDetails> GetAllUsers();
        public List<AnswerListModel> GetMyReacties(string aadId);
        public void checkIfUserExists(ClaimsPrincipal user);
        public void Save(UserDetails user);
        public UserDetails? getUserFromClaimsPrincipal(ClaimsPrincipal user);
        public UserDetails? GetUserById(int id);
        public UserDetails? UpdateUser(UserDetails user, int id);
        public ICollection<UserDetails> GetAllDropdownUsers();
        /// <summary>
        /// Soft deletes a "Vrijwilliger" by setting "deleted" property to true.
        /// </summary>
        /// <param name="id">Id of the "Vrijwilliger" to delete.</param>
        /// <param name="user">Person who is deleting the "Vrijwilliger".</param>
        /// <param name="comment">Reason for deletion.</param>
        public bool Delete(int id, UserDetails user, string comment);

        /// <summary>
        /// Restores a "Vrijwilliger" by setting "deleted" property to false.
        /// </summary>
        /// <param name="id">id of the vrijwilliger</param>
        public bool Restore(int id);

        /// <summary>
        /// Returns all softdeleted users
        /// </summary>
        /// <returns></returns>
        public ICollection<UserDetails> GetAllDeletedUsers();
    }
}
