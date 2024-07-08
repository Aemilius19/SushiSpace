namespace SushiSpace.Web.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string NewPassword { get; set; }
        public string ConfrimNewPassword { get; set; }
        public string? OldPassword { get; set; }

        public string userId { get; set; }
    }
}
