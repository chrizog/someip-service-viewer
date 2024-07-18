using System;
using System.Net;
using System.Reactive;

namespace ServiceViewer.Models;

class Ipv4EndpointOption : SdOption
{
    public IPAddress? Address { get; set; }

    public TransportProtocol Protocol { get; set; }

    public ushort Port { get; set; }
}
