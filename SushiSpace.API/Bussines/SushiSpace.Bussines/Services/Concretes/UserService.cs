using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Org.BouncyCastle.Bcpg.Sig;
using SushiSpace.Application.Repositories.Abstractions;
using SushiSpace.Bussines.Services.Abstractions;
using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;
using SushiSpace.Core.Helper.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Bussines.Services.Concretes
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IMailService _mailService;

        public UserService(IUserRepository userRepository, SignInManager<User> signInManager, UserManager<User> userManager, IMailService mailService)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _mailService = mailService;
        }

        public async Task<bool> ChangePassword(ChangePasswordDTO dto, string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if(user is null) { throw new ArgumentNullException(nameof(user)); }
            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                throw new Exception("password was not changed");
            }
            return result.Succeeded;
        }

        public async Task<bool> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }
        public async Task<bool> SendConfirmEmail(string userId, string link)
        {
            var user = await _userRepository.GetUserbyId(userId, null, "Carts", "Orders");
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string confirmLink = $"{link}&token={Uri.EscapeDataString(token)}"; // Изменено здесь

            await _mailService.SendEmailAsync(new MailRequest()
            {
                Subject = "No-Reply Sushi Space",
                ToEmail = user.Email,
                Body = $"Confirm your Email by clicking this link: <a href='{confirmLink}'>Click Me</a>"
            });

            return true;
        }


        public Task<bool> DeleteUser(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException("email");
            }
            var result= await _userRepository.GetUserByEmail(email,x=>x.NormalizedEmail==email.ToUpper(),"Carts","Orders");
            if (result is null)
            {
                throw new Exception("user was not found");
            }
            return result;

        }

        public async Task<User> GetUserById(string userId)
        {
            if (userId is null)
            {
                throw new ArgumentNullException("id");
            }
            var result = await _userRepository.GetUserbyId(userId, null, "Carts", "Orders");
            if (result is null)
            {
                throw new Exception("user was not found");
            }
            return result;
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            if(userName is null)
            {
                throw new ArgumentNullException("username");
            }
            var result = await _userRepository.GetUserByUsername(userName, null, "Carts", "Orders");
            if (result is null)
            {
                throw new Exception("user was not found");
            }
            return result;
        }

        public async Task<IQueryable<User>> GetUsers()
        {
            var data = await _userRepository.GetAllUsers(x => x.Id, x => x.Email != null, "Carts", "Orders");
            return data;
        }

        public async Task<bool> IsInRole(string userId, string role)
        {
            var user=await _userRepository.GetUserbyId(userId,null,"Carts","Orders");
            
            var check= await _userManager.IsInRoleAsync(user, role);
            if (!check)
            {
                return false;
            }
            return true;
        }

        public async Task<User> Register(RegisterDTO dto)
        {
            var (isSuccess, user) = await _userRepository.RegisterUser(dto, dto.Password);
            if (!isSuccess)
                throw new InvalidOperationException("User was not created");

            return user;
        }

        public async Task<bool> SendResetPassword(ResetPasswordDTO dto,string link,string password)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            string token=await _userManager.GeneratePasswordResetTokenAsync(user);
            string resetLink = $"{link}&token={Uri.EscapeDataString(token)}&password={Uri.EscapeDataString(password)}";
            await _mailService.SendEmailAsync(new MailRequest()
            {
                Subject = "No-reply Sushi Space",
                ToEmail = user.Email,
                Body=$"Please reset your password by clicking link: <a href='{resetLink}'>Click Me</a>"

            }) ;
            return true;
        }

        public async Task<bool> ResetPassword(string userid, string token, string password)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (result.Succeeded)
            {
                return true;
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Password was not reset. Errors: {errors}");
        }



        public async Task<User> SignIn(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return user;
            }
            return null;
        }

        public async Task<bool> SignOut()
        {
            await _signInManager.SignOutAsync();
            return true;
        }

        public async Task<bool> UpdateUser(UpdateDTO dto, string userId)
        {  
           var result= await _userRepository.UpdateUser(dto,userId);
            if (!result)
            {
                throw new Exception("can not save changes");
            }
            return result;
        }
    }
}
