using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Infrastracture.MessageSender
{
    public class EmailService
    {
        public static async Task<bool> SendMailAsync(string subject, string body,string reciver)
        {
            return await Task.FromResult(true);
        }
    }
}
