using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceViewer.Models
{
    internal class DataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; }

        public IPAddress SrcAddress { get; }

        public int SrcPort { get; } 

        public DataReceivedEventArgs(byte[] data, IPAddress srcAddress, int srcPort)
        {
            this.Data = data;
            this.SrcAddress = srcAddress;
            this.SrcPort = srcPort;
        }
    }
}
