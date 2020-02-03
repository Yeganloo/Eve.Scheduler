namespace Eve.Scheduler
{
    public class Message
    {
        public string Identifier { get; set; }

        public string MessageType { get; set; }
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

        public byte Status { get; set; }

        public byte[] Payload { get; set; }
    }
}
