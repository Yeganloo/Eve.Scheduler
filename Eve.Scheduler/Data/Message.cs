namespace Eve.Scheduler
{
    public class Message
    {
        public string Identifier { get; set; }

        public string MessageType { get; set; }

        public MessageScheduleOptions Options { get; set; }

        public MessageHeaders Headers { get; set; }

        public byte[] Payload { get; set; }

        public byte[] Tag { get; set; }

        public byte[] IV { get; set; }
    }
}
