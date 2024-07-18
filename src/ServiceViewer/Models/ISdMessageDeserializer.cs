using System;

namespace ServiceViewer.Models;

public interface ISdMessageDeserializer
{
    SdMessage Deserialize(byte[] data);
}