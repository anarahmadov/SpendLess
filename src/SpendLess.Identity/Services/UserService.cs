﻿using Microsoft.AspNetCore.Identity;
using SpendLess.Application.Contracts.Identity;
using SpendLess.Application.Models.Identity;
using SpendLess.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return new User
            {
                Email = user.Email,
                Id = user.Id,
                Firstname = user.FirstName,
                Lastname = user.LastName
            };
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _userManager.GetUsersInRoleAsync("User");
            return users.Select(q => new User { 
                Id = q.Id,
                Email = q.Email,
                Firstname = q.FirstName,
                Lastname = q.LastName
            }).ToList();
        }
    }
}
