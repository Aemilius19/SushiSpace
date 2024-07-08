using Microsoft.AspNetCore.Identity.Data;
using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Bussines.Services.Abstractions
{
    public interface IUserService
    {
        Task<User> Register(RegisterDTO dto);
        Task<User> SignIn(LoginDTO dto);

        Task<bool> SignOut();

        Task<bool> ChangePassword(ChangePasswordDTO dto,string userid);

        Task<bool> SendResetPassword(ResetPasswordDTO dto,string link,string password);
        Task<bool> ResetPassword(string userid, string token, string password);

        Task<bool> SendConfirmEmail(string userId, string token);



        Task<bool> ConfirmEmail(string userId, string token);

        //testing
        Task<User> GetUserById(string userId);
        Task<User> GetUserByUserName(string userName);

        Task<User> GetUserByEmail(string email);
        Task<bool> UpdateUser(UpdateDTO dto, string userId);
        Task<bool> DeleteUser(string userId);
        Task<IQueryable<User>> GetUsers();
        Task<bool> IsInRole(string userId, string role);

    }
}
