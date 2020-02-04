using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Scheduler.APICaller
{
    public class APIInfo
    {
        public string Address { get; set; }

        public string Method { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }
    }
}
