using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Scheduler
{
    public class MessageScheduleOptions
    {
        /// <summary>
        /// Unix Start Time. (Optional)
        /// </summary>
        public long? StartDateTime { get; set; }
        /// <summary>
        /// Unix End Time. (Optional)
        /// </summary>
        public long? ExpiresDateTime { get; set; }

        public int Period { get; set; }

        public int Retries { get; set; }

        public int? Timeout { get; set; }
    }
}
