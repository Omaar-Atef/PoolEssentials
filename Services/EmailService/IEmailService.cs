using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.EmailService
{
    
        public interface IEmailsService
        {
        void SendEmail(EmailMessage message);
        Task SendEmailAsync(EmailMessage message);
    }
    
}
