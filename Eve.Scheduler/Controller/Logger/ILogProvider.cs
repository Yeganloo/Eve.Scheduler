using System;
using System.IO;

namespace Eve.Scheduler.Controller
{
    public interface ILogProvider
    {
        string Extention { get; }
        void Init(Stream stream);
        byte[] GetByte(object obj);
        void Dispose(Stream stream);
    }
}
