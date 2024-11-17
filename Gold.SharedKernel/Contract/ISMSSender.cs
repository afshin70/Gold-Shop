using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.Contract
{
    public interface ISMSSender
    {
        Task SendManyAsync(string[] numbers,string content ,CancellationToken cancellationToken= default);
        Task SendAsync(string number,string content,CancellationToken cancellationToken);
        void SendMany(string[] numbers,string content);
        void Send(string number,string content);
        Task SendByVerifyAsync(string number, int templateId, Dictionary<string, string> smsParameters, CancellationToken cancellationToken = default);
    }
}
