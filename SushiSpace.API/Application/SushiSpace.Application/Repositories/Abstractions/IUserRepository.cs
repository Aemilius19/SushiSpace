using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Application.Repositories.Abstractions
{
    public interface IUserRepository
    {
        Task<User> GetUserbyId(string id, Expression<Func<User, bool>> filters, params string[] includes);
        Task<User> GetUserByEmail(string email, Expression<Func<User, bool>> filters, params string[] includes);
        Task<User> GetUserByUsername(string username, Expression<Func<User, bool>> filters, params string[] includes);
        Task<(bool, User)> RegisterUser(RegisterDTO dto,string password);
        Task<bool> UpdateUser(UpdateDTO dto, string userid);
        Task<bool> DeleteUser(string id);
        Task<IQueryable<User>> GetAllUsers(Expression<Func<User,object>> orderby,Expression<Func<User, bool>> filters, params string[] includes);   
    }
}
