namespace Eve.Scheduler
{
    public enum MessageStatus : byte
    {
        Periodic = 1,
        Retry = 2,
        SaveResult = 4
    }
}
