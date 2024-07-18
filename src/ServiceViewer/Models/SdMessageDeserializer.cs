using System;
using System.Collections.Generic;

namespace ServiceViewer.Models;

public class SdMessageDeserializer : ISdMessageDeserializer
{
    public SdMessage Deserialize(byte[] data)
    {
        if (data.Length < 28)
        {
            throw new ArgumentException("Data length must be at least 28 bytes");
        }

        uint messageId = BigEndianBitConverter.ToUInt32(data, 0);
        uint serviceId = messageId >> 16;
        uint methodId = messageId & 0x0000FFFF;

        uint length = BigEndianBitConverter.ToUInt32(data, 4);
        uint requestId = BigEndianBitConverter.ToUInt32(data, 8);
        uint clientID = requestId >> 16;
        uint sessionId = requestId & 0x0000FFFF;

        byte protocolVersion = data[12];
        byte interfaceVersion = data[13];
        byte messageType = data[14];
        byte returnCode = data[15];
        byte flags = data[16];
        uint lengthEntries = BigEndianBitConverter.ToUInt32(data, 20);

        const int entrySize = 16;
        uint numEntries = lengthEntries / entrySize;
        ServiceEntry[] entries = new ServiceEntry[numEntries];

        const int entriesStartIndex = 24;
        for (int i = 0; i < numEntries; i++)
        {
            int entryStartIndex = entriesStartIndex + i * entrySize;

            byte numberOptions = data[entryStartIndex + 3];

            entries[i] = new ServiceEntry
            {
                Field = (ServiceEntryField)data[entryStartIndex],
                IndexFirstOptionRun = data[entryStartIndex + 1],
                IndexSecondOptionRun = data[entryStartIndex + 2]
            };

            entries[i].NumberOfOptions1 = (uint)(numberOptions >> 4);
            entries[i].NumberOfOptions2 = (uint)(numberOptions & 0x0F);
            entries[i].ServiceId = BigEndianBitConverter.ToUInt16(data, entryStartIndex + 4);
            entries[i].InstanceId = BigEndianBitConverter.ToUInt16(data, entryStartIndex + 6);
            entries[i].MajorVersion = data[entryStartIndex + 8];
            var tmpTtl = new byte[4];
            Array.Copy(data, entryStartIndex + 9, tmpTtl, 1, tmpTtl.Length - 1);
            entries[i].Ttl = BigEndianBitConverter.ToUInt32(tmpTtl, 0);
            entries[i].MinorVersion = BigEndianBitConverter.ToUInt32(data, entryStartIndex + 12);
        }

        uint lengthOptions = BigEndianBitConverter.ToUInt32(data, entriesStartIndex + (int)lengthEntries);
        uint startOptions = entriesStartIndex + lengthEntries + 4;

        uint optionBytesRead = 0;
        var options = new List<SdOption>();

        while (optionBytesRead < lengthOptions - 3) // 3 bytes for length (2) and type (1)
        {
            ushort lengthOption = BigEndianBitConverter.ToUInt16(data, (int)(startOptions + optionBytesRead));
            byte type = data[startOptions + 2 + optionBytesRead];
            
            byte[] optionData = new byte[lengthOption ]; // minus 3 bytes for length (2) and type (1)

            int sourceIndex = (int)(startOptions + optionBytesRead + 3);

            if (sourceIndex + lengthOption > data.Length)
            {
                break;
            }

            Array.Copy(data, sourceIndex, optionData, 0, lengthOption);

            options.Add(new SdOption()
            {
                Length = lengthOption,
                Type = (OptionType)type,
                Data = optionData
            });

            optionBytesRead += lengthOption;
        }

        return new SdMessage()
        {
            ServiceId = serviceId,
            MethodId = methodId,
            Length = length,
            ClientId = clientID,
            SessionId = sessionId,
            ProtocolVersion = protocolVersion,
            InterfaceVersion = interfaceVersion,
            MessageType = messageType,
            ReturnCode = returnCode,
            Flags = flags,
            EntriesArray = entries,
            OptionsArray = options.ToArray()
        };
    }
}
