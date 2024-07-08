using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.DTOs
{
    public record ChangePasswordDTO
    {
        public string NewPassword { get; set; }
        public string ConfrimNewPassword { get; set; }
        public string? OldPassword { get; set; }
    }
}
