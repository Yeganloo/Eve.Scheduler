using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Scheduler
{
    public class SocketSettings
    {
        public string Name { get; set; }

        public ISet<string> WhiteList { get; set; }

        public string Address { get; set; }

    }
}
