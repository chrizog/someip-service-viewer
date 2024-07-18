using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceViewer.Models
{
    internal class StateChangedEventArgs : EventArgs
    {
        public bool IsRunning { get; }

        public StateChangedEventArgs(bool isRunning)
        {
            this.IsRunning = isRunning;
        }
    }
}
