using Eve.Scheduler.Data;
using System.Collections.Generic;

namespace Eve.Scheduler
{
    public class Settings
    {
        public IEnumerable<SocketSettings> SocketSettings { get; set; }

        public IEnumerable<HandlerInfo> Handlers { get; set; }
    }
}
