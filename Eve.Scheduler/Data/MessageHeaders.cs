using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Scheduler
{
    public class MessageHeaders
    {
        public string kid { get; set; }

        public string alg { get; set; }

        public string cty { get; set; }

        public byte[] cek { get; set; }

        public string enc { get; set; }
    }
}
