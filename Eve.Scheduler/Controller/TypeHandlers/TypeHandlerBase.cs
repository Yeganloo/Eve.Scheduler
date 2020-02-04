using Eve.Settings;

namespace Eve.Scheduler.Controller.TypeHandlers
{
    public abstract class TypeHandlerBase
    {
        private SettingsManager _SettingsManager;

        public TypeHandlerBase(SettingsManager settingsManager)
        {
            _SettingsManager = settingsManager;
        }

        public abstract byte[] Handle(Message message);

    }
}
