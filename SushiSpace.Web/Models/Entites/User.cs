using Microsoft.AspNetCore.Identity;

namespace SushiSpace.Web.Models.Entites
{
    public class User:IdentityUser
    {
        
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }=false;
    }
}
