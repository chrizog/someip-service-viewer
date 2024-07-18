using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServiceViewer.Models
{
    internal class SdListener : ISdListener
    {
        private IPAddress _sdIpAddress;
        private int _sdPort;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;
        private bool isRunning;

        public event EventHandler<DataReceivedEventArgs>? DataReceived;
        public event EventHandler<StateChangedEventArgs>? StateChanged;

        public SdListener(IPAddress sdIpAddress, int sdPort)
        {
            // Initialize listener
            this._sdIpAddress = sdIpAddress;
            this._sdPort = sdPort;
            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationTokenSource.Token;
            this.isRunning = false;
        }

        private void OnDataReceived(byte[] data, IPAddress srcAddress, int srcPort)
        {
            DataReceived?.Invoke(this, new DataReceivedEventArgs(data, srcAddress, srcPort));
        }
        private void OnStateChanged()
        {
            StateChanged?.Invoke(this, new StateChangedEventArgs(this.isRunning));
        }

        private async Task Receive()
        {
            UdpClient udpClient = new UdpClient();
            try
            {
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, this._sdPort));
                udpClient.JoinMulticastGroup(this._sdIpAddress);
            }
            catch
            {
                this.isRunning = false;
                this.OnStateChanged();
                return;
            }

            // Start listening for datagrams
            while (!this.cancellationToken.IsCancellationRequested)
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    var result = await udpClient.ReceiveAsync(cancellationToken);
                    this.OnDataReceived(result.Buffer, result.RemoteEndPoint.Address, result.RemoteEndPoint.Port);
                }
                catch (Exception)
                {

                }                         
            }

            try
            {
                udpClient.Close();
            }
            catch (Exception)
            {

            }

            this.isRunning = false;
            this.OnStateChanged();
        }

        public void StartReceiving()
        {
            // Start receiving data
            if (this.isRunning)
            {
               return;
            }

            this.isRunning = true;
            this.OnStateChanged();
            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationTokenSource.Token;
            Task.Run(async () => await this.Receive());
        }

        public void StopReceiving()
        {
            // Stop receiving data
            cancellationTokenSource.Cancel();
        }
    }
}
