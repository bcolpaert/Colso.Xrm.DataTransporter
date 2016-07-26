using System;

namespace Colso.DataTransporter.AppCode
{
    public class StatusMessageEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public StatusMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
