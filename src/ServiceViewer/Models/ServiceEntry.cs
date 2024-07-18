using System;

namespace ServiceViewer.Models;

public class ServiceEntry
{
    public ServiceEntryField Field { get; set; }
    public uint IndexFirstOptionRun { get; set; }

    public uint IndexSecondOptionRun { get; set; }

    public uint NumberOfOptions1 { get; set; }

    public uint NumberOfOptions2 { get; set; }

    public uint ServiceId { get; set; }

    public uint InstanceId { get; set; }

    public uint MajorVersion { get; set; }

    public uint Ttl { get; set; }

    public uint MinorVersion { get; set; }
}
