using Gold.SharedKernel.Contract;
using IPE.SmsIrClient;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Gold.Infrastracture.ExternalService
{
    public class SmsIrService : ISMSSender
    {
        private readonly SmsIr _smsIr;
        private readonly long _lineNumber;
        private readonly ILogManager _logManager;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        public SmsIrService(ILogManager logManager, IConfiguration configuration)
        {
            _configuration = configuration;
            _logManager = logManager;
            try
            {
                _apiKey = _configuration.GetSection("SmsIrSetting:ApiKey").Value;
                _lineNumber = long.Parse(_configuration.GetSection("SmsIrSetting:LineNumber").Value);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
            }
            _smsIr = new SmsIr(_apiKey);
        }
        public void Send(string number, string content)
        {
            try
            {
                var bulkSendResult = _smsIr.BulkSend(_lineNumber, content, new string[] { number });
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
            }
        }

        public async Task SendAsync(string number, string content, CancellationToken cancellationToken)
        {

            try
            {
                var bulkSendResult = await _smsIr.BulkSendAsync(_lineNumber, content, new string[] { number });
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
            }

        }

        public void SendMany(string[] numbers, string content)
        {
            try
            {
                var bulkSendResult = _smsIr.BulkSend(_lineNumber, content, numbers);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
            }
        }

        public async Task SendManyAsync(string[] numbers, string content, CancellationToken cancellationToken = default)
        {
            try
            {
                var bulkSendResult = await _smsIr.BulkSendAsync(_lineNumber, content, numbers);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
            }
        }

        public async Task SendByVerifyAsync(string number, int templateId, Dictionary<string, string> smsParameters, CancellationToken cancellationToken = default)
        {
            try
            {
                SmsIrModel smsIrModel = new SmsIrModel
                {
                    Mobile = number,
                    TemplateId = templateId,
                    Parameters=new SmsIrParameter[smsParameters.Count]
                };
                List<SmsIrParameter> smsIrParameters = new List<SmsIrParameter>();
                foreach (var item in smsParameters)
                {
                    SmsIrParameter param = new(item.Key, item.Value);
                    smsIrParameters.Add(param);
                }
                smsIrModel.Parameters = smsIrParameters.ToArray();
                //string jsonData = smsIrModel.ConvertObjectToJson();
                string jsonData = JsonConvert.SerializeObject(smsIrModel);

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://api.sms.ir/v1/send/verify", content);
                var result = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
            }
        }
    }

    public class SmsIrModel
    {
        public string Mobile { get; set; }
        public int TemplateId { get; set; }
        public SmsIrParameter[] Parameters { get; set; }
    }
    public class SmsIrParameter
    {
        public SmsIrParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
