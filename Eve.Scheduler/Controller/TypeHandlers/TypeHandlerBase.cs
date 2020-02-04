using Eve.Settings;

namespace Eve.Scheduler.Controller.TypeHandlers
{
    public abstract class TypeHandlerBase
    {
        private SettingsManager _SettingsManager;
        public string MessageType { get; private set; }

        public TypeHandlerBase(string messageType, SettingsManager settingsManager)
        {
            _SettingsManager = settingsManager;
            MessageType = messageType;
        }

        public abstract byte[] Handle(Message message);

    }
}
