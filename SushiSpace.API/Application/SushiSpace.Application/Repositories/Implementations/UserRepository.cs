using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SushiSpace.Application.Data.Context;
using SushiSpace.Application.Repositories.Abstractions;
using SushiSpace.Bussines.Services.Abstractions;
using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Application.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly SushiSpaceDb _conxtext;



        public UserRepository(UserManager<User> userManager, IRoleRepository roleRepository, IMapper mapper, SushiSpaceDb conxtext)
        {

            _userManager = userManager;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _conxtext = conxtext;
        }

        public async Task<bool> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id was null");
            }
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<IQueryable<User>> GetAllUsers(Expression<Func<User, object>> orderby, Expression<Func<User, bool>> filters, params string[] includes)
        {
            IQueryable<User> users = _conxtext.Users.AsQueryable();
            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    users = users.Include(include);
                }
            }
            if (filters is not null)
            {
                users = users.Where(filters);
            }
            if (orderby is not null)
            {
                users = users.OrderBy(orderby);
            }
            return users;
        }

        public async Task<User> GetUserByEmail(string email, Expression<Func<User, bool>> filters, params string[] includes)
        {
            if (email == null)
            {
                throw new ArgumentNullException("email was null");
            }
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new Exception("User is null");
            }
            IQueryable<User> userQuery = _conxtext.Users.AsQueryable();
            if (includes is not not null)
            {
                foreach (var include in includes)
                {
                    userQuery = userQuery.Include(include);
                }
            }
            if (filters is not null)
            {
                userQuery = userQuery.Where(filters);
            }
            var userWI = await userQuery.FirstOrDefaultAsync(x => x.Id == user.Id);
            return userWI;
        }

        public async Task<User> GetUserbyId(string id, Expression<Func<User, bool>> filters, params string[] includes)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                throw new Exception("User was not found");
            }
            IQueryable<User> userQuery = _conxtext.Users.AsQueryable();
            if (includes is not not null)
            {
                foreach (var include in includes)
                {
                    userQuery = userQuery.Include(include);
                }
            }
            if (filters is not null)
            {
                userQuery = userQuery.Where(filters);
            }
            var userWI = await userQuery.FirstOrDefaultAsync(x => x.Id == user.Id);
            return userWI;
        }

        public async Task<User> GetUserByUsername(string username, Expression<Func<User, bool>> filters, params string[] includes)
        {
            if (username == null)
            {
                throw new ArgumentNullException();
            }
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
            {
                throw new Exception("User was not found");
            }
            IQueryable<User> userQuery = _conxtext.Users.AsQueryable();
            if (includes is not not null)
            {
                foreach (var include in includes)
                {
                    userQuery = userQuery.Include(include);
                }
            }
            if (filters is not null)
            {
                userQuery = userQuery.Where(filters);
            }
            var userWI = await userQuery.FirstOrDefaultAsync(x => x.Id == user.Id);
            return userWI;
        }

        public async Task<(bool, User)> RegisterUser(RegisterDTO dto, string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var user = _mapper.Map<User>(dto);

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return (false, null);

            var role = dto.IsAdmin ? "Admin" : "User";
            await _userManager.AddToRoleAsync(user, role);

            return (true, user);
        }

        public async Task<bool> UpdateUser(UpdateDTO dto,string userid)
        {
            var existingUser = await _userManager.FindByIdAsync (userid);

            if (existingUser == null)
            {
                throw new ArgumentException("User not found");
            }

            // Применяем изменения к существующему пользователю
            _mapper.Map(dto,existingUser);

            _conxtext.SaveChangesAsync();

            try
            {
                await _conxtext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Обработка исключения оптимистической блокировки
                // Возможно, в этом месте стоит логировать исключение или обрабатывать иначе, в зависимости от вашего бизнес-контекста
                // Подробнее о конфликтах версий: https://docs.microsoft.com/en-us/ef/core/saving/concurrency
                throw new Exception("Concurrency error occurred", ex);
            }
        }
    }
}
