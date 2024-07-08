using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Bussines.Services.Abstractions
{
    public interface IRoleRepository
    {
        Task<string> CreateRole(string roleName);
    }
}
