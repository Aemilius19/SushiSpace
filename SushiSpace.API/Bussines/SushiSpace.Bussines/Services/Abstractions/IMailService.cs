using SushiSpace.Core.Helper.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Bussines.Services.Abstractions
{
   public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
