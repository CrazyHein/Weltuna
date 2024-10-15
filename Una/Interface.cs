using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Master;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool
{
    public abstract class DataModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler UserPropertyChanged;
        virtual internal protected void OnPropertyChanged(string propertyName, bool userProperty)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if(userProperty)
                UserPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void SetProperty<T>(ref T storage, T value, bool userProperty = true, [CallerMemberName] String propertyName = null)
        {
            storage = value;
            OnPropertyChanged(propertyName, userProperty);
        }

        protected static readonly JsonSerializerOptions _SERIALIZER_OPTIONS = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.Preserve,
            Converters =
            {
                new JsonStringEnumConverter(null, false)
            }
        };

        public string FriendlyName { get; init; }

        public abstract long Save(Utf8JsonWriter writer);
        public abstract long Restore(ref Utf8JsonReader reader);

        protected abstract void _online_state_changed(bool online);
        public abstract void ExchangeDataWithUserInterface();
        public abstract void Dispose();

        private bool __online;
        public bool Online 
        {
            get { return __online; }
            set
            {
                if (value != __online)
                {
                    _online_state_changed(value);
                    SetProperty(ref __online, value, false);
                }
            }
        }

        public abstract void ExchangeDataWithDevice(DeviceAccessMaster master, ushort monitoringTimer);

        protected object _synchronizer = new object();

        protected static Regex _MODULE_ACCESS_EXTENSION_PATTERN = new Regex(@"^[UJ][0-9,A-F]{3}$", RegexOptions.Compiled);

        public int BindingErrors { get; set; }
    }

    public interface ICabinet
    {
        public object CreateInstance(PropertyChangedEventHandler propertyChangedEventHandler);

        public string Name { get; set; }

        public string Description { get; set; }

        public string Version { get; set; }
    }
}
