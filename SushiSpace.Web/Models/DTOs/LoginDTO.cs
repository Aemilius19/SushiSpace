using System.ComponentModel.DataAnnotations;

namespace SushiSpace.Web.Models.DTOs
{
    public class LoginDTO
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
