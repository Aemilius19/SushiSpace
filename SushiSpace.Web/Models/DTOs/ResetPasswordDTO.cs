using System.ComponentModel.DataAnnotations;

namespace SushiSpace.Web.Models.DTOs
{
    public record ResetPasswordDTO
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
