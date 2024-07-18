using System;

namespace ServiceViewer.Models;
public class SdMessage
{
    public uint ServiceId { get; set; }
    
    public uint MethodId { get; set; }
    
    public uint Length { get; set; }
    
    public uint ClientId { get; set; }
    
    public uint SessionId { get; set; }
    
    public byte ProtocolVersion { get; set; }
    
    public byte InterfaceVersion { get; set; }
    
    public byte MessageType { get; set; }
    
    public byte ReturnCode { get; set; }
    
    public byte Flags { get; set; }
    
    public ServiceEntry[]? EntriesArray { get; set; }

    public SdOption[]? OptionsArray { get; set; }

    public DateTime ReceptionTime { get; set; } = DateTime.Now;
}

