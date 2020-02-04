using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.Scheduler.Controller
{
    public static class LogExtentions
    {
        public static object ToLog(this Exception ex, string message)
        {
            return new { Message = message, Exception = ex.Message, ex.StackTrace, InnerException = ex.InnerException?.Message };
        }
    }
}
