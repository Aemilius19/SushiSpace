using System.ComponentModel.DataAnnotations;

namespace SushiSpace.Web.Models.DTOs
{
    public record ChangePasswordDTO
    {
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }


        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        public string ConfrimNewPassword { get; set; }


        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        
    }
}
