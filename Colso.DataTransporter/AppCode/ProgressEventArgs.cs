using System;

namespace Colso.DataTransporter.AppCode
{
    public class ProgressEventArgs : EventArgs
    {
        public int Progress { get; private set; }
        public string UserState { get; private set; }

        public ProgressEventArgs(int progress, string userState)
        {
            Progress = progress;
            UserState = userState;
        }
    }
}
