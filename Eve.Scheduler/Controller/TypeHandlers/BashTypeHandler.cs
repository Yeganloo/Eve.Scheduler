using Eve.Settings;
using System;
using System.Text;

namespace Eve.Scheduler.Controller.TypeHandlers
{
    public class BashTypeHandler : TypeHandlerBase
    {
        public BashTypeHandler(SettingsManager settingsManager) : base(settingsManager)
        {

        }

        public override byte[] Handle(Message message)
        {
            Console.WriteLine(Encoding.UTF8.GetString(YamlProvider.GetBytes(message)));
            return null;
        }
    }
}
