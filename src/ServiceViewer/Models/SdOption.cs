using System;
using System.Reactive;

namespace ServiceViewer.Models;

public class SdOption
{
    public ushort Length { get; set; }

    public OptionType Type { get; set; }

    public byte[]? Data { get; set; }
}
