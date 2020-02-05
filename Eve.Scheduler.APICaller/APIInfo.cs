namespace Eve.Scheduler.APICaller
{
    using System.Collections.Generic;

    public class APIInfo
    {
        public string Address { get; set; }

        public string Method { get; set; }

        public string Data { get; set; }

        public IEnumerable<Data.KeyValuePair<string, string>> Headers { get; set; }
    }
}
