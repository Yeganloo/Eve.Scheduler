using Eve.Scheduler.Controller.TypeHandlers;
using Eve.Settings;
using MessagePack;
using MessagePack.Resolvers;
using RestSharp;
using System;

namespace Eve.Scheduler.APICaller
{
    public class APIHandler : TypeHandlerBase
    {
        public APIHandler(string messageType, SettingsManager settingsManager) : base(messageType, settingsManager)
        {
        }

        public override byte[] Handle(Message message)
        {
            var apiInfo = MessagePackSerializer.Deserialize<APIInfo>(message.Payload, ContractlessStandardResolver.Options);
            RestClient cli = new RestClient();
            RestRequest request = new RestRequest(apiInfo.Address);
            if (apiInfo.Headers != null)
                foreach (var h in apiInfo.Headers)
                {
                    request.AddHeader(h.Key, h.Value);
                }
            if (!string.IsNullOrEmpty(apiInfo.Method))
                request.Method = (Method)Enum.Parse(typeof(Method), apiInfo.Method);

            var res = cli.Execute(request);
            return MessagePackSerializer.Serialize(new { res.StatusCode, res.RawBytes, res.Content, Successful = res.IsSuccessful }, ContractlessStandardResolver.Options);
        }
    }
}
