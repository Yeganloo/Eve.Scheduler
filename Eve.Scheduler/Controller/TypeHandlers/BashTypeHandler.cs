using Eve.Settings;
using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }
    }
}
