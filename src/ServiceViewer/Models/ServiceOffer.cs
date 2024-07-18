using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServiceViewer.Models
{
    public class ServiceOffer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int instanceId;

        public int InstanceId
        {
            get => this.instanceId; set
            {
                if (this.instanceId != value)
                {
                    this.instanceId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int ServiceId { get; set; }

        public string IpAddress { get; set; } = "";

        public int Port { get; set; }

        public uint MajorVersion { get; set; }

        public uint Ttl { get; set; }

        public uint MinorVersion { get; set; }

        private ulong receptionTime = (ulong)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

        public ulong ReceptionTime { get => this.receptionTime;
            set
            {
                if (this.receptionTime != value)
                {
                    this.receptionTime = value;
                    NotifyPropertyChanged();
                }
            }
        } 

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ServiceOffer serviceOffer = (ServiceOffer)obj;
            return InstanceId == serviceOffer.InstanceId && ServiceId == serviceOffer.ServiceId && IpAddress == serviceOffer.IpAddress && Port == serviceOffer.Port &&
                MajorVersion == serviceOffer.MajorVersion && Ttl == serviceOffer.Ttl && MinorVersion == serviceOffer.MinorVersion;

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static IEnumerable<ServiceOffer> FromSdMessage(SdMessage sdMessage)
        {
            var result = new List<ServiceOffer>();
            // If TTL == 0x00 it is a Stop Offer entry
            var offerEntries = sdMessage.EntriesArray?.Where(entry => entry.Field == ServiceEntryField.OFFER_SERVICE && entry.Ttl != 0x00);
            if (offerEntries == null)
            {
                return result;
            }

            foreach (ServiceEntry serviceEntry in offerEntries)
            {
                // TODO: Complete, implement parsing both number of options
                var numOption = serviceEntry.NumberOfOptions1;
                var indexOption = serviceEntry.IndexFirstOptionRun;

                if (numOption == 0 || sdMessage.OptionsArray == null || indexOption >= sdMessage.OptionsArray.Length)
                {
                    continue;
                }

                var option = sdMessage.OptionsArray[indexOption];

                if (option.Type != OptionType.IP4_ENDPOINT)
                {
                    continue;
                }

                if (option.Data == null)
                {
                    continue;
                }

                // Parse Ipv4Endpoint Option
                uint address = BigEndianBitConverter.ToUInt32(option.Data, 1);
                // Reserved 8 bit
                var transportProtocol = (TransportProtocol)option.Data[6];
                ushort port = BigEndianBitConverter.ToUInt16(option.Data, 7);

                result.Add(new ServiceOffer()
                {
                    ServiceId = (int)serviceEntry.ServiceId,
                    InstanceId = (int)serviceEntry.InstanceId,
                    IpAddress = new IPAddress(address).ToString(),
                    Port = port,
                    MajorVersion = serviceEntry.MajorVersion,
                    Ttl = serviceEntry.Ttl,
                    MinorVersion = serviceEntry.MinorVersion
                });

            }
            return result;
        }
    }
}
