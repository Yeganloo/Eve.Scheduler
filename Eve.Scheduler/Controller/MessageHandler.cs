using Eve.Caching;
using Eve.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Eve.Scheduler.Controller
{
    public class MessageHandler
    {
        private static readonly DateTime UnixStartTime = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
        private double UnixUTCNow
        {
            get
            {
                return DateTime.UtcNow.Subtract(UnixStartTime).TotalSeconds;
            }
        }
        private Timer Checker;
        private ICacheProvider<string, Message> cache;
        private readonly Func<SettingsManager, Message, byte[]> Handler;
        public string MessageType { get; }

        public MessageHandler(string messageType, Func<SettingsManager, Message, byte[]> handler, ICacheProvider<string, Message> cacheProvider = null)
        {
            cache = cacheProvider ?? new SimpleCacheProvider<Message>();
            Handler = handler;
            Checker = new Timer(40000);
            Checker.AutoReset = true;
            Checker.Elapsed += Checker_Elapsed;
            MessageType = messageType;
        }

        private void Checker_Elapsed(object sender, ElapsedEventArgs e)
        {
            //TODO get 1 or 2 recent blocks to process
        }

        public void Handle(Message message)
        {
            if (message.ExpiresDateTime < UnixUTCNow)
                //TODO Log expired message.
                return;

            int delay;
            if (message.StartDateTime.HasValue && (delay = (int)(message.StartDateTime - UnixUTCNow)) > 0)
                ScheduleForLater(delay, message);
            else
            {

            }

        }

        private void PeriodicRun()
        {

        }

        private async Task ScheduleForLater(int delay, Message message)
        {
            //Schedule a thread whithin 5 minutes
            if (delay < 300000)
            {
                await Task.Run(() =>
                {
                    Task.Delay(delay);
                    Handle(message);
                });
            }
            else
            {
                //TODO queue jobs to cache later
            }
        }

    }
}
