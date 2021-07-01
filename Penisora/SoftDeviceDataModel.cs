using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP;
using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Command;
using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Master;
using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Penisora
{
    public class SoftDeviceDataModel : DataModel
    {
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Factory()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public override void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public static IReadOnlyDictionary<string, (DEVICE_ACCESS_RANGE_T, DEVICE_ACCESS_TYPE_T)> LOCAL_DEVICE_INFO { get; } = new Dictionary<string, (DEVICE_ACCESS_RANGE_T, DEVICE_ACCESS_TYPE_T)>
        {
            {"SM",      (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
            {"SD",      (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
            {"X",       (DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.BIT) },
            {"Y",       (DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.BIT) },
            {"M",       (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
            {"L",       (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
            {"F",       (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
            {"V",       (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
            {"B",       (DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.BIT) },
            {"D",       (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
            {"W",       (DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.WORD) },
            {"TS",      (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
            {"TC",      (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
            {"TN",      (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
            {"STS",     (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
            {"STC",     (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
            {"STN",     (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
            {"CS",      (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
            {"CC",      (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
            {"CN",      (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
            {"SB",      (DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.BIT) },
            {"SW",      (DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.WORD) },
            {"DX",      (DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.BIT) },
            {"DY",      (DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.BIT) },
            {"Z",       (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
            {"R",       (DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
            {"ZR",      (DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.WORD) }
        };

        public ObservableCollection<SoftDeviceRowDataModel> RowDataCollection { get; private set; } = new ObservableCollection<SoftDeviceRowDataModel>();
        public ObservableCollection<string> ExceptionInfoCollection { get; private set; } = new ObservableCollection<string>();
 
        private bool __power = false;
        public bool Power
        {
            get { return __power; }
            set { if(value != __power) SetProperty(ref __power, value, false);}
        }

        private bool __select_local_device = true;
        public bool SelectLocalDevice
        {
            get { return __select_local_device; }
            set { if (value != __select_local_device) SetProperty(ref __select_local_device, value); }
        }

        private string __loacal_device_name = "D";
        public string LocalDeviceName
        {
            get { return __loacal_device_name; }
            set
            {
                if (SoftDeviceDataModel.LOCAL_DEVICE_INFO.ContainsKey(value))
                {
                    if (__loacal_device_name != value)
                        SetProperty(ref __loacal_device_name, value);
                }
                else
                    throw new ArgumentException(@"The input string for 'Local Device Name' is not supported.");
            }
        }

        private string __module_access_device_name = "U000";
        public string ModuleAccessDeviceName
        {
            get { return __module_access_device_name; }
            set 
            {
                if (_MODULE_ACCESS_EXTENSION_PATTERN.IsMatch(value))
                {
                    if (__module_access_device_name != value)
                        SetProperty(ref __module_access_device_name, value);
                }
                else
                    throw new ArgumentException(@"The input string for 'Moduel Access Device Name' is not in a correct format.");
            }
        }

        private uint __head_device;
        public uint HeadDevice
        {
            get { return __head_device; }
            set { if(__head_device != value) SetProperty(ref __head_device, value); }
        }

        private ushort __device_points = 16;
        public ushort DevicePoints
        {
            get { return __device_points; }
            set 
            {
                if (value <= 512 && value > 0)
                {
                    if (__device_points != value)
                        SetProperty(ref __device_points, value);
                }
                else
                    throw new ArgumentException(@"The input value for 'Device Point' is out of range([1 -- 512]).");
            }
        }

        /*
        public ushort __read_op_end_code = 0;
        public ushort ReadOperationEndCode
        {
            get { return __read_op_end_code; }
            set { if(value != __read_op_end_code) SetProperty(ref __read_op_end_code, value, false); }
        }

        public ushort __write_op_end_code = 0;
        public ushort WriteOperationEndCode
        {
            get { return __write_op_end_code; }
            set { if (value != __write_op_end_code) SetProperty(ref __write_op_end_code, value, false); }
        }

        public SLMP_EXCEPTION_CODE_T __read_op_exception = SLMP_EXCEPTION_CODE_T.NO_ERROR;
        public SLMP_EXCEPTION_CODE_T ReadOperationException
        {
            get { return __read_op_exception; }
            set { if (value != __read_op_exception) SetProperty(ref __read_op_exception, value, false); }
        }

        public SLMP_EXCEPTION_CODE_T __write_op_exception = SLMP_EXCEPTION_CODE_T.NO_ERROR;
        public SLMP_EXCEPTION_CODE_T WriteOperationException
        {
            get { return __write_op_exception; }
            set { if (value != __write_op_exception) SetProperty(ref __write_op_exception, value, false); }
        }
        */
        private ConcurrentQueue<USER_COMMAND_T> __explicit_commands = new ConcurrentQueue<USER_COMMAND_T>();
        private ConcurrentQueue<USER_COMMAND_T> __explicit_results = new ConcurrentQueue<USER_COMMAND_T>();
        private HashSet<(bool local, string name, uint index)> __pending_write_requests = new HashSet<(bool local, string name, uint index)>(16);
        private PROCESS_IN_DATA_T __process_in_data_share;
        private PROCESS_IN_DATA_T __process_in_data_device;
        private PROCESS_IN_DATA_T __process_in_data_user_interface;
        private uint __bit_access;

        private void __add_read_exception_info(ushort endcode)
        {
            if (ExceptionInfoCollection.Count > 128)
                ExceptionInfoCollection.RemoveAt(127);
            ExceptionInfoCollection.Insert(0, $"[{DateTime.Now.ToLocalTime()}]: Read Operation returns end code 0x{endcode:X4}.");
            Notification.Show(new ExceptionNotification(FriendlyName, $"Read Operation returns end code 0x{endcode:X4}."),
                ShowAnimation.VerticalMove, false);
        }

        private void __add_read_exception_info(SLMP_EXCEPTION_CODE_T code)
        {
            if (ExceptionInfoCollection.Count > 128)
                ExceptionInfoCollection.RemoveAt(127);
            ExceptionInfoCollection.Insert(0, $"[{DateTime.Now.ToLocalTime()}]: Read Operation returns exception code <{code}>.");
            if (code != SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
            {
                Notification.Show(new ExceptionNotification(FriendlyName, $"Read Operation returns exception code <{code}>."),
                    ShowAnimation.VerticalMove, false);
            }
        }

        private void __add_write_exception_info(ushort endcode)
        {
            if (ExceptionInfoCollection.Count > 128)
                ExceptionInfoCollection.RemoveAt(127);
            ExceptionInfoCollection.Insert(0, $"[{DateTime.Now.ToLocalTime()}]: Write Operation returns end code 0x{endcode:X4}.");
            Notification.Show(new ExceptionNotification(FriendlyName, $"Write Operation returns end code 0x{endcode:X4}."),
                ShowAnimation.VerticalMove, false);
        }

        private void __add_write_exception_info(SLMP_EXCEPTION_CODE_T code)
        {
            if (ExceptionInfoCollection.Count > 128)
                ExceptionInfoCollection.RemoveAt(127);
            ExceptionInfoCollection.Insert(0, $"[{DateTime.Now.ToLocalTime()}]: Write Operation returns exception code <{code}>.");
            if (code != SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
            {
                Notification.Show(new ExceptionNotification(FriendlyName, $"Write Operation returns exception code <{code}>."),
                    ShowAnimation.VerticalMove, false);
            }
        }

        protected override void _online_state_changed(bool online)
        {
            if (!online)
            {
                __explicit_commands.Clear();
                __explicit_results.Clear();
                __pending_write_requests.Clear();
                __process_in_data_share = new PROCESS_IN_DATA_T();
                __process_in_data_device = new PROCESS_IN_DATA_T();
                __process_in_data_user_interface = new PROCESS_IN_DATA_T();
                RowDataCollection.Clear();
                Power = false;
                //__explicit_commands.Enqueue(new STOP_MONITOR_T());
            }
        }

        public bool PostStartCommand()
        {
            __explicit_commands.Enqueue(new START_MONITOR_T()
            {
                local_device = SelectLocalDevice,
                device_name = SelectLocalDevice ? LocalDeviceName : ModuleAccessDeviceName,
                head_device = HeadDevice,
                device_points = DevicePoints
            });
            return true;
        }

        public bool PostStopCommand()
        {
            __explicit_commands.Enqueue(new STOP_MONITOR_T());
            return true;
        }

        public bool PostWriteSingleCommand(bool localDecice, string deviceName, uint head, ushort value)
        {
            __explicit_commands.Enqueue(new WRITE_SINGLE_DEVICE_T() { local_device = localDecice, device_name = deviceName, head_device = head, value = value});
            __pending_write_requests.Add((localDecice, deviceName, head));
            return true;
        }

        public override void ExchangeDataWithDevice(DeviceAccessMaster master, ushort monitoring)
        {
            if (__explicit_commands.TryDequeue(out var command))
            {
                switch (command.cmd)
                {
                    case USER_COMMAND_CODE_T.START:
                        var readcmd = (START_MONITOR_T)command;
                        PROCESS_IN_METADATA_T meta = new PROCESS_IN_METADATA_T()
                        {
                            local_device = readcmd.local_device,
                            device_name = readcmd.device_name,
                            head_device = readcmd.head_device,
                            device_points = readcmd.device_points,
                        };
                        __bit_access = PROCESS_IN_METADATA_T.BIT_ACCESS(readcmd.local_device, readcmd.device_name);
                        __process_in_data_device = new PROCESS_IN_DATA_T(ref meta);
                        __process_in_data_device.CopyTo(ref __process_in_data_share, _synchronizer);
                        command.op_end_code = 0;
                        command.op_slmp_exception = SLMP_EXCEPTION_CODE_T.NO_ERROR;
                        __explicit_results.Enqueue(command);
                        break;
                    case USER_COMMAND_CODE_T.STOP:
                        __process_in_data_device.data_array = null;
                        command.op_end_code = 0;
                        command.op_slmp_exception = SLMP_EXCEPTION_CODE_T.NO_ERROR;
                        __explicit_results.Enqueue(command);
                        break;
                    case USER_COMMAND_CODE_T.WRITE_SINGLE:
                        var writecmd = (WRITE_SINGLE_DEVICE_T)command;
                        ushort endcode;
                        try
                        {
                            ushort[] readback = new ushort[1] { writecmd.value };
                            if (writecmd.local_device)
                            {
                                master.WriteLocalDeviceInWord(monitoring, writecmd.device_name, writecmd.head_device * __bit_access, 1, out endcode, readback);
                                //if (endcode == 0)
                                    //master.ReadLocalDeviceInWord(monitoring, writecmd.device_name, writecmd.head_device * __bit_access, 1, out endcode, readback);
                            }
                            else
                            {
                                master.WriteModuleAccessDeviceInWord(monitoring, writecmd.device_name, writecmd.head_device, 1, out endcode, readback);
                                //if (endcode == 0)
                                    //master.ReadModuleAccessDeviceInWord(monitoring, writecmd.device_name, writecmd.head_device, 1, out endcode, readback);
                            }
                            lock(_synchronizer)
                            {
                                __process_in_data_share.data_array[writecmd.head_device - __process_in_data_share.metadata.head_device] = readback[0];
                            }
                            writecmd.op_end_code = endcode;
                            writecmd.op_slmp_exception = SLMP_EXCEPTION_CODE_T.NO_ERROR;
                        }
                        catch (SLMPException ex)
                        {
                            writecmd.op_end_code = 0;
                            writecmd.op_slmp_exception = ex.ExceptionCode;
                            if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                                throw;
                        }
                        finally
                        {
                            __explicit_results.Enqueue(writecmd);
                        }
                        break;
                }
            }

            if (__process_in_data_device.data_array != null && __process_in_data_device.data_array.Length > 0)
            {
                try
                {
                    if (__process_in_data_device.metadata.local_device)
                        master.ReadLocalDeviceInWord(monitoring, __process_in_data_device.metadata.device_name, 
                            __process_in_data_device.metadata.head_device * __bit_access, __process_in_data_device.metadata.device_points,
                            out __process_in_data_device.end_code, __process_in_data_device.data_array);
                    else
                        master.ReadModuleAccessDeviceInWord(monitoring, __process_in_data_device.metadata.device_name,
                            __process_in_data_device.metadata.head_device, __process_in_data_device.metadata.device_points,
                            out __process_in_data_device.end_code, __process_in_data_device.data_array);
                    __process_in_data_device.slmp_exception = SLMP_EXCEPTION_CODE_T.NO_ERROR;
                }
                catch (SLMPException ex)
                {
                    __process_in_data_device.end_code = 0;
                    __process_in_data_device.slmp_exception = ex.ExceptionCode;
                    if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                        throw;
                }
                finally
                {
                    __process_in_data_device.CopyTo(ref __process_in_data_share, _synchronizer);
                }
            }
            else
            {
                __process_in_data_device.end_code = 0;
                __process_in_data_device.slmp_exception = SLMP_EXCEPTION_CODE_T.NO_ERROR;
            } 
        }

        public override void ExchangeDataWihtUserInterface()
        {
            if (__explicit_results.TryDequeue(out var result))
            {
                switch (result.cmd)
                {
                    case USER_COMMAND_CODE_T.START:
                        var start = (START_MONITOR_T)result;
                        string name = start.local_device ? start.device_name : start.device_name + "/G";
                        string format = "D";
                        uint coefficient = PROCESS_IN_METADATA_T.BIT_ACCESS(start.local_device, start.device_name);
                        if (start.local_device)
                        {
                            (DEVICE_ACCESS_RANGE_T range, DEVICE_ACCESS_TYPE_T type) = SoftDeviceDataModel.LOCAL_DEVICE_INFO[start.device_name];
                            if (range == DEVICE_ACCESS_RANGE_T.HEXADECIMAL)
                                format = "X";
                            if (type == DEVICE_ACCESS_TYPE_T.BIT)
                                coefficient = 16;
                        }
                        RowDataCollection.Clear();
                        for (uint index = start.head_device * coefficient; index < (start.head_device + start.device_points) * coefficient; index += coefficient)
                        {
                            SoftDeviceRowDataModel row = new SoftDeviceRowDataModel(name + index.ToString(format, CultureInfo.InvariantCulture), 0);
                            RowDataCollection.Add(row);
                        }
                        //__process_in_data_user_interface.CopyFrom(ref __process_in_data_share, _synchronizer);
                        Power = true;
                        break;
                    case USER_COMMAND_CODE_T.STOP:
                        RowDataCollection.Clear();
                        Power = false;
                        break;
                    case USER_COMMAND_CODE_T.WRITE_SINGLE:
                        if (result.op_end_code != 0)
                            __add_write_exception_info(result.op_end_code);
                        else if (result.op_slmp_exception != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_write_exception_info(result.op_slmp_exception);
                        WRITE_SINGLE_DEVICE_T cmd = result as WRITE_SINGLE_DEVICE_T;
                        __pending_write_requests.Remove((cmd.local_device, cmd.device_name, cmd.head_device));
                        break;
                }
            }

            if(Power)
            {
                __process_in_data_user_interface.CopyFrom(ref __process_in_data_share, _synchronizer);
                if (__process_in_data_user_interface.end_code == 0 && __process_in_data_user_interface.slmp_exception == SLMP_EXCEPTION_CODE_T.NO_ERROR)
                {
                    for (int i = 0; i < __process_in_data_user_interface.metadata.device_points; ++i)
                    {
                        if (__pending_write_requests.Contains((__process_in_data_user_interface.metadata.local_device,
                            __process_in_data_user_interface.metadata.device_name, 
                            (uint)(__process_in_data_user_interface.metadata.head_device + i))))
                            continue;
                        RowDataCollection[i].WordValue = __process_in_data_user_interface.data_array[i];
                    }
                }
                else
                {
                    if (__process_in_data_user_interface.end_code != 0)
                        __add_read_exception_info(__process_in_data_user_interface.end_code);
                    else if (__process_in_data_user_interface.slmp_exception != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                        __add_read_exception_info(__process_in_data_user_interface.slmp_exception);
                    RowDataCollection.Clear();
                    Power = false;
                    __explicit_commands.Enqueue(new STOP_MONITOR_T());
                }
            }
        }

        public override long Restore(ref Utf8JsonReader reader)
        {
            long start = reader.BytesConsumed;
            if (reader.Read() == false || reader.TokenType != JsonTokenType.StartObject)
                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The given JSON object is not a valid data model instance.");

            while(reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if(reader.TokenType == JsonTokenType.PropertyName)
                {
                    string property = reader.GetString();
                    switch(property)
                    {
                        case "Local Device":
                            if (reader.Read() && (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False))
                                SetProperty(ref __select_local_device, reader.GetBoolean(), false, nameof(SelectLocalDevice));
                            else
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid boolean value.");
                            break;
                        case "Local Device Name":
                            if (reader.Read() && reader.TokenType == JsonTokenType.String)
                                SetProperty(ref __loacal_device_name, reader.GetString(), false, nameof(LocalDeviceName));
                            else
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid string.");
                            break;
                        case "Module Access Device Name":
                            if (reader.Read() && reader.TokenType == JsonTokenType.String)
                                SetProperty(ref __module_access_device_name, reader.GetString(), false, nameof(ModuleAccessDeviceName));
                            else
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid string.");
                            break;
                        case "Head Device":
                            if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt32(out __head_device)) == false)
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid uint32 number.");
                            else
                                SetProperty(ref __head_device, __head_device, false, nameof(HeadDevice));
                            break;
                        case "Device Points":
                            if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt16(out __device_points)) == false)
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid uint16 number.");
                            else
                                SetProperty(ref __device_points, __device_points, false, nameof(DevicePoints));
                            break;
                    }
                }
            }
            return reader.BytesConsumed - start;
        }

        public override long Save(Utf8JsonWriter writer)
        {
            long start = writer.BytesPending;
            writer.WriteStartObject();

            writer.WritePropertyName("Local Device");
            writer.WriteBooleanValue(SelectLocalDevice);

            writer.WritePropertyName("Local Device Name");
            writer.WriteStringValue(LocalDeviceName);

            writer.WritePropertyName("Module Access Device Name");
            writer.WriteStringValue(ModuleAccessDeviceName);

            writer.WritePropertyName("Head Device");
            writer.WriteNumberValue(HeadDevice);

            writer.WritePropertyName("Device Points");
            writer.WriteNumberValue(DevicePoints);

            writer.WriteEndObject();
            return writer.BytesPending - start;
        }
    }

    public class Factory : ICabinet
    {
        private string __version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string Name { get => "SoftDeviceMonitor"; set => throw new NotImplementedException(); }
        public string Description { get => @"Device/Buffer Memory Batch Monitor"; set => throw new NotImplementedException(); }
        public string Version  { get => __version; set => throw new NotImplementedException(); }

        public object CreateInstance(PropertyChangedEventHandler propertyChangedEventHandler)
        {
            SoftDeviceDataModel data = new SoftDeviceDataModel() { FriendlyName = "SoftDeviceMonitor" };
            data.UserPropertyChanged += propertyChangedEventHandler;
            return new SoftDeviceMonitor(data);
        }
    }

    public class SoftDeviceRowDataModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        virtual internal protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {

            if (object.Equals(storage, value))
                return;
            storage = value;
            OnPropertyChanged(propertyName);
        }

        private string __word_name;
        private ushort __word_value;

        public SoftDeviceRowDataModel(string wordName, ushort wordValue)
        {
            __word_name = wordName;
            __word_value = wordValue;
        }

        public string WordName
        {
            get { return __word_name; }
            set { SetProperty(ref __word_name, value); }
        }

        public ushort WordValue
        {
            get { return __word_value; }
            set { SetProperty(ref __word_value, value); }
        }
    }

    

    internal struct PROCESS_IN_METADATA_T
    {
        public bool local_device;
        public string device_name;
        public uint head_device;
        public ushort device_points;

        public static uint BIT_ACCESS(bool local, string name)
        {
            return local && SoftDeviceDataModel.LOCAL_DEVICE_INFO[name].Item2 == DEVICE_ACCESS_TYPE_T.BIT ? 16 : (uint)1;
        }
    }

    internal struct PROCESS_IN_DATA_T
    {
        public PROCESS_IN_METADATA_T metadata;
        public ushort end_code;
        public SLMP_EXCEPTION_CODE_T slmp_exception;
        public ushort[] data_array;

        public PROCESS_IN_DATA_T(ref PROCESS_IN_METADATA_T metadata)
        {
            this.metadata = new PROCESS_IN_METADATA_T()
            {
                local_device = metadata.local_device,
                device_name = metadata.device_name,
                head_device = metadata.head_device,
                device_points = metadata.device_points
            };
            end_code = 0;
            slmp_exception = SLMP_EXCEPTION_CODE_T.NO_ERROR;
            data_array = new ushort[this.metadata.device_points];
        }

        public void CopyTo(ref PROCESS_IN_DATA_T data, object sync)
        {
            lock (sync)
            {
                data.metadata = metadata;
                if (data.data_array == null || data.data_array.Length != data_array.Length)
                    data.data_array = new ushort[data_array.Length];
                data.end_code = end_code;
                data.slmp_exception = slmp_exception;
                Buffer.BlockCopy(data_array, 0, data.data_array, 0, data_array.Length * 2);
            }
        }

        public void CopyFrom(ref PROCESS_IN_DATA_T data, object sync)
        {
            lock (sync)
            {
                metadata = data.metadata;
                if (data_array == null || data.data_array.Length != data_array.Length)
                    data_array = new ushort[data.data_array.Length];
                end_code = data.end_code;
                slmp_exception = data.slmp_exception;
                Buffer.BlockCopy(data.data_array, 0, data_array, 0, data.data_array.Length * 2);
            }
        }
    }


    internal enum USER_COMMAND_CODE_T
    {
        NONE,
        START,
        STOP,
        WRITE_SINGLE,
    }

    internal abstract class USER_COMMAND_T
    {
        public USER_COMMAND_CODE_T cmd { get; protected set; }
        public SLMP_EXCEPTION_CODE_T op_slmp_exception { get; set; }
        public ushort op_end_code { get; set; }
    }

    internal class WRITE_SINGLE_DEVICE_T : USER_COMMAND_T
    {
        public bool local_device { get; init; }
        public string device_name { get; init; }
        public uint head_device { get; init; }
        public ushort value { get; init; }

        public WRITE_SINGLE_DEVICE_T() : base() { cmd = USER_COMMAND_CODE_T.WRITE_SINGLE; }
    }

    internal class START_MONITOR_T: USER_COMMAND_T
    {
        public bool local_device { get; init; }
        public string device_name { get; init; }
        public uint head_device { get; init; }
        public ushort device_points { get; init; }
        public START_MONITOR_T() : base() { cmd = USER_COMMAND_CODE_T.START; }
    }

    internal class STOP_MONITOR_T : USER_COMMAND_T
    {
        public STOP_MONITOR_T() : base() { cmd = USER_COMMAND_CODE_T.STOP; }
    }
}
