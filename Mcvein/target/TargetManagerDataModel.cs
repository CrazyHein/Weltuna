using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks
{
    public class TargetManagerDataModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        virtual internal protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            storage = value;
            OnPropertyChanged(propertyName);
        }

        public ObservableCollection<TargetPropertyDataModel> TargetList { get; init; }
        private TargetPropertyDataModel __actived_target;
        public TargetPropertyDataModel ActivedTarget
        {
            get { return __actived_target; }
            set
            {
                if (value == __actived_target || value == null)
                    return;
                else
                    SetProperty(ref __actived_target, value);
            }
        }

        private TargetPropertyDataModel __selected_target;
        [JsonIgnore]
        public TargetPropertyDataModel SelectedTarget
        {
            get { return __selected_target; }
            set
            {
                if (value == __selected_target)
                    return;
                else
                    SetProperty(ref __selected_target, value);
            }
        }

        public TargetManagerDataModel()
        {
            TargetList = new ObservableCollection<TargetPropertyDataModel>();
            __actived_target = null;
        }

        public TargetManagerDataModel(IEnumerable<TargetPropertyDataModel> list, TargetPropertyDataModel actived)
        {
            TargetList = new ObservableCollection<TargetPropertyDataModel>(list);
            __actived_target = actived;
        }

        public void Add(TargetPropertyDataModel target)
        {
            if (Find(target.Name) != null)
                throw new ArgumentException("A target with the same name already exists.");
            TargetList.Add(target);
        }

        public void Remove(TargetPropertyDataModel target) => TargetList.Remove(target);

        public void Modify(TargetPropertyDataModel original, TargetPropertyDataModel newone)
        {
            if (original.Name != newone.Name && Find(newone.Name) != null)
                throw new ArgumentException("A target with the same name already exists.");
            TargetList[TargetList.IndexOf(original)] = newone;
            if (original == ActivedTarget)
                ActivedTarget = newone;
        }

        public TargetPropertyDataModel Find(string name)
        {
            foreach (var tar in TargetList)
                if (tar.Name == name)
                    return tar;
            return null;
        }

        public override string ToString()
        {
            var option = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve,
                Converters =
                {
                    new JsonStringEnumConverter(null, false)
                }
            };
            return JsonSerializer.Serialize(this, option);
        }

        public string ToString(JsonSerializerOptions option = null)
        {
            return JsonSerializer.Serialize(this, option);
        }

        private static JsonSerializerOptions __SERIALIZER_OPTIONS = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.Preserve,
            Converters =
            {
                new JsonStringEnumConverter(null, false)
            }
        };

        public static void SAVE_TO_JSON(Utf8JsonWriter writer, TargetManagerDataModel data)
        {
            JsonSerializer.Serialize(writer, data, __SERIALIZER_OPTIONS);
        }

        public static void RESTORE_FROM_JSON(ref Utf8JsonReader reader, out TargetManagerDataModel model)
        {
            var res = JsonSerializer.Deserialize<TargetManagerDataModel>(ref reader, __SERIALIZER_OPTIONS);
            model =  __VALIDATE(res);
        }

        private static TargetManagerDataModel __VALIDATE(TargetManagerDataModel data)
        {
            if(data == null)
                throw new ArgumentException(@"<Targets> node is missing or object data is corrupted.");
            try
            {
                Dictionary<string, TargetPropertyDataModel> dic =
                    new Dictionary<string, TargetPropertyDataModel>(data.TargetList.Select((target) => new KeyValuePair<string, TargetPropertyDataModel>(target.Name, target)));
                if(data.ActivedTarget != null && dic.Contains(new KeyValuePair<string, TargetPropertyDataModel>(data.ActivedTarget.Name, data.ActivedTarget)) == false)
                    throw new ArgumentException($"The actived target ({data.ActivedTarget.Name}) is invalid.");
            }
            catch(ArgumentException)
            {
                throw new ArgumentException("A target with the same name already exists.");
            }
            return data;
        }
    }
}
