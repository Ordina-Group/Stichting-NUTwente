﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    public class UserController : Controller
    {
        public IUserService UserService { get; }
        public UserController(IUserService userService)
        {
            UserService = userService;
        }


        [Authorize]
        [Route("Account/SignOut")]
        public IActionResult SignOutCatch()
        {
            return Redirect("/User/Overview");
        }

        [Authorize]
        public IActionResult Overview()
        {
            var aadID = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
            if (aadID != null)
            {
                var userDetails = this.UserService.GetUserByAADId(aadID.Value);
                var email = User.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress"))?.Value;
                var givenname = User.Claims.FirstOrDefault(c => c.Type.Contains("givenname"))?.Value;
                var surname = User.Claims.FirstOrDefault(c => c.Type.Contains("surname"))?.Value;
                var groups = User.Claims.Where(c => c.Type.Contains("group")).Select(x => x.Value);
                if (userDetails != null)
                {
                    
                    if (userDetails.FirstName != givenname ||
                        userDetails.LastName != surname ||
                        userDetails.Email != email ||
                        !userDetails.Roles.All(groups.Contains) ||
                        !groups.All(userDetails.Roles.Contains))
                    {
                        var newUserDetails = new UserDetails()
                        {
                            FirstName = givenname,
                            LastName = surname,
                            Email = email,
                            Roles = groups.ToList(),
                            AADId = aadID.Value
                        };
                        UserService.UpdateUser(newUserDetails, aadID.Value);
                    }
                } 
                else
                {
                    var newUserDetails = new UserDetails()
                    {
                        FirstName = givenname,
                        LastName = surname,
                        Email = email,
                        Roles = groups.ToList(),
                        AADId = aadID.Value
                    };
                    UserService.Save(newUserDetails);
                }
                return View(new UserViewModel(new UserDetails()
                {
                    FirstName = givenname,
                    LastName = surname,
                    Email = email,
                    Roles = groups.ToList()
                }));
            }
            return View(new UserViewModel());
        }
    }
}