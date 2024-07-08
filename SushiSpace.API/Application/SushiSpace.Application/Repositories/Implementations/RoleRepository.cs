using Microsoft.AspNetCore.Identity;
using SushiSpace.Bussines.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Bussines.Services.Concretes
{
    namespace SushiSpace.Bussines.Services.Concretes
    {
        public class RoleRepository : IRoleRepository
        {
            private readonly RoleManager<IdentityRole> _roleManager;

            public RoleRepository(RoleManager<IdentityRole> roleManager)
            {
                _roleManager = roleManager;
            }

            public async Task<string> CreateRole(string roleName)
            {
                var result = await _roleManager.CreateAsync(
                    new IdentityRole()
                    {
                        Name = roleName,
                    }
                );
                if (!result.Succeeded)
                {
                    throw new Exception("Role was not created");
                }
                return roleName;
            }
        }
    }

}
