using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceViewer.Models
{
    internal interface ISdListener
    {
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public event EventHandler<StateChangedEventArgs> StateChanged;

        public void StartReceiving();

        public void StopReceiving();
    }
}
