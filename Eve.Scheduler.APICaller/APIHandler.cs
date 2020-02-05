using Eve.Scheduler.Controller.TypeHandlers;
using Eve.Settings;
using MessagePack;
using MessagePack.Resolvers;
using System.Net;
using System.Text;

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
            WebClient client = new WebClient();
            foreach (var h in apiInfo.Headers)
                client.Headers.Add(h.Key, h.Value);
            return Encoding.UTF8.GetBytes(client.UploadString(apiInfo.Address, apiInfo.Method, apiInfo.Data));
        }
    }
}
