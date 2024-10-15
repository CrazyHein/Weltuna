using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP;
using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Master;
using AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Control;
using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Numeros
{
    public class Q64TCAutoTuningDataModel : DataModel
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

        private  static Regex __PROCESS_VALUE_STRING_PATTERN = new Regex(@"^\-?\d+(\.\d{1})?$", RegexOptions.Compiled);
        public ObservableCollection<string> ExceptionInfoCollection { get; private set; } = new ObservableCollection<string>();
        private Q64TC_PROCESS_IN_DATA_T __process_in_data_device = new Q64TC_PROCESS_IN_DATA_T() { channel_process_in = new CHANNNEL_PROCESS_IN_DATA_T[4] };
        private Q64TC_PROCESS_IN_DATA_T __process_in_data_share = new Q64TC_PROCESS_IN_DATA_T() { channel_process_in = new CHANNNEL_PROCESS_IN_DATA_T[4] };
        private Q64TC_PROCESS_IN_DATA_T __process_in_data_user_interface = new Q64TC_PROCESS_IN_DATA_T() { channel_process_in = new CHANNNEL_PROCESS_IN_DATA_T[4] };
        private Q64TC_PROCESS_OUT_DATA_T __process_out_data_device = new Q64TC_PROCESS_OUT_DATA_T();
        private Q64TC_PROCESS_OUT_DATA_T __process_out_data_share = new Q64TC_PROCESS_OUT_DATA_T();
        private Q64TC_PROCESS_OUT_DATA_T __process_out_data_user_interface = new Q64TC_PROCESS_OUT_DATA_T();
        private bool __device_sync_control;
        private uint __device_sync_address;
        private string __device_sync_specification;
        private int __device_sync_channel;
        List<(string, uint)> __device_module_access_address_table = null;
        List<(string, uint)>[] __channel_module_access_address_table = null;
        private ushort[] __device_module_access_data;
        private ushort[][] __channel_module_access_data;
        private uint[][] __at_parameter_address_table = new uint[4][]
        {
            new uint[]
            {
                (uint)Q64TC_ADDRESS_TABLE_T.CH0_SET_VALUE_SETTING,
                (uint)Q64TC_ADDRESS_TABLE_T.CH0_AT_BIAS_SETTING,
                (uint)Q64TC_ADDRESS_TABLE_T.CH_AT_LOOP_DISCONNECTION_DETECTION_FLAG,
                (uint)Q64TC_ADDRESS_TABLE_T.CH0_LOOP_DISCONNECTION_DETECTION,
                (uint)Q64TC_ADDRESS_TABLE_T.CH0_AT_MODE_SELECTION,
                (uint)Q64TC_ADDRESS_TABLE_T.CH0_AUTOMATIC_BACKUP_SETTING
            },
            new uint[]
            {
                (uint)Q64TC_ADDRESS_TABLE_T.CH1_SET_VALUE_SETTING,
                (uint)Q64TC_ADDRESS_TABLE_T.CH1_AT_BIAS_SETTING,
                (uint)Q64TC_ADDRESS_TABLE_T.CH_AT_LOOP_DISCONNECTION_DETECTION_FLAG,
                (uint)Q64TC_ADDRESS_TABLE_T.CH1_LOOP_DISCONNECTION_DETECTION,
                (uint)Q64TC_ADDRESS_TABLE_T.CH1_AT_MODE_SELECTION,
                (uint)Q64TC_ADDRESS_TABLE_T.CH1_AUTOMATIC_BACKUP_SETTING
            },
            new uint[]
            {
                (uint)Q64TC_ADDRESS_TABLE_T.CH2_SET_VALUE_SETTING,
                (uint)Q64TC_ADDRESS_TABLE_T.CH2_AT_BIAS_SETTING,
                (uint)Q64TC_ADDRESS_TABLE_T.CH_AT_LOOP_DISCONNECTION_DETECTION_FLAG,
                (uint)Q64TC_ADDRESS_TABLE_T.CH2_LOOP_DISCONNECTION_DETECTION,
                (uint)Q64TC_ADDRESS_TABLE_T.CH2_AT_MODE_SELECTION,
                (uint)Q64TC_ADDRESS_TABLE_T.CH2_AUTOMATIC_BACKUP_SETTING
            },
            new uint[]
            {
                (uint)Q64TC_ADDRESS_TABLE_T.CH3_SET_VALUE_SETTING,
                (uint)Q64TC_ADDRESS_TABLE_T.CH3_AT_BIAS_SETTING,
                (uint)Q64TC_ADDRESS_TABLE_T.CH_AT_LOOP_DISCONNECTION_DETECTION_FLAG,
                (uint)Q64TC_ADDRESS_TABLE_T.CH3_LOOP_DISCONNECTION_DETECTION,
                (uint)Q64TC_ADDRESS_TABLE_T.CH3_AT_MODE_SELECTION,
                (uint)Q64TC_ADDRESS_TABLE_T.CH3_AUTOMATIC_BACKUP_SETTING
            }
        };
        private uint[][] __pid_constant_address_table = new uint[4][]
        {
            new uint[]
            {
                (uint)Q64TC_ADDRESS_TABLE_T.CH0_HEATING_PROPORTIONAL_BAND,
                (uint)Q64TC_ADDRESS_TABLE_T.CH0_COOLING_PROPORTIONAL_BAND,
                (uint)Q64TC_ADDRESS_TABLE_T.CH0_INTEGRAL_TIME,
                (uint)Q64TC_ADDRESS_TABLE_T.CH0_DERIVATIVE_TIME,
                (uint)Q64TC_ADDRESS_TABLE_T.CH0_LOOP_DISCONNECTION_DETECTION
            },
            new uint[]
            {
                (uint)Q64TC_ADDRESS_TABLE_T.CH1_HEATING_PROPORTIONAL_BAND,
                (uint)Q64TC_ADDRESS_TABLE_T.CH1_COOLING_PROPORTIONAL_BAND,
                (uint)Q64TC_ADDRESS_TABLE_T.CH1_INTEGRAL_TIME,
                (uint)Q64TC_ADDRESS_TABLE_T.CH1_DERIVATIVE_TIME,
                (uint)Q64TC_ADDRESS_TABLE_T.CH1_LOOP_DISCONNECTION_DETECTION
            },
            new uint[]
            {
                (uint)Q64TC_ADDRESS_TABLE_T.CH2_HEATING_PROPORTIONAL_BAND,
                (uint)Q64TC_ADDRESS_TABLE_T.CH2_COOLING_PROPORTIONAL_BAND,
                (uint)Q64TC_ADDRESS_TABLE_T.CH2_INTEGRAL_TIME,
                (uint)Q64TC_ADDRESS_TABLE_T.CH2_DERIVATIVE_TIME,
                (uint)Q64TC_ADDRESS_TABLE_T.CH2_LOOP_DISCONNECTION_DETECTION
            },
            new uint[]
            {
                (uint)Q64TC_ADDRESS_TABLE_T.CH3_HEATING_PROPORTIONAL_BAND,
                (uint)Q64TC_ADDRESS_TABLE_T.CH3_COOLING_PROPORTIONAL_BAND,
                (uint)Q64TC_ADDRESS_TABLE_T.CH3_INTEGRAL_TIME,
                (uint)Q64TC_ADDRESS_TABLE_T.CH3_DERIVATIVE_TIME,
                (uint)Q64TC_ADDRESS_TABLE_T.CH3_LOOP_DISCONNECTION_DETECTION
            }
        };
        private void __device_read_data()
        {
            lock (_synchronizer)
            {
                if (__process_out_data_device.control_out == null && __process_out_data_share.control_out != null)
                {
                    __process_out_data_device = __process_out_data_share;
                    __process_out_data_share.control_out = null;
                }
            }
        }

        private void __device_write_data()
        {
            lock (_synchronizer)
            {
                __process_in_data_share.end_code = __process_in_data_device.end_code;
                __process_in_data_share.exception_code = __process_in_data_device.exception_code;
                __process_in_data_share.device_process_in = __process_in_data_device.device_process_in;
                __process_in_data_share.channel_process_in[__process_in_data_share.device_process_in.index] = 
                    __process_in_data_device.channel_process_in[__process_in_data_device.device_process_in.index];
                if (__process_in_data_share.control_in == null && __process_in_data_device.control_in != null)
                {
                    __process_in_data_share.control_in = __process_in_data_device.control_in;
                    __process_in_data_device.control_in = null;
                }
            }
        }

        private void __user_read_data()
        {
            lock (_synchronizer)
            {
                __process_in_data_user_interface.end_code = __process_in_data_share.end_code;
                __process_in_data_user_interface.exception_code = __process_in_data_share.exception_code;
                __process_in_data_user_interface.device_process_in = __process_in_data_share.device_process_in;
                __process_in_data_user_interface.channel_process_in[__process_in_data_user_interface.device_process_in.index] = 
                    __process_in_data_share.channel_process_in[__process_in_data_share.device_process_in.index];
                if (__process_in_data_user_interface.control_in == null && __process_in_data_share.control_in != null)
                {
                    __process_in_data_user_interface.control_in = __process_in_data_share.control_in;
                    __process_in_data_share.control_in = null;
                }
            }
        }

        private void __user_write_data()
        {
            lock (_synchronizer)
            {
                if (__process_out_data_share.control_out == null && __process_out_data_user_interface.control_out != null)
                {
                    __process_out_data_share = __process_out_data_user_interface;
                    __process_out_data_user_interface.control_out = null;
                }
            }
        }

        public void __add_exception_info(string opertaion, string user)
        {
            if (ExceptionInfoCollection.Count > 128)
                ExceptionInfoCollection.RemoveAt(127);
            ExceptionInfoCollection.Insert(0, $"[{DateTime.Now.ToLocalTime()}]: <{opertaion}> : {user}.");
            Notification.Show(new ExceptionNotification(FriendlyName, $"<{opertaion}> : {user}."),
                ShowAnimation.VerticalMove, false);
        }

        public void __add_exception_info(string opertaion, ushort endcode)
        {
            if (ExceptionInfoCollection.Count > 128)
                ExceptionInfoCollection.RemoveAt(127);
            ExceptionInfoCollection.Insert(0, $"[{DateTime.Now.ToLocalTime()}]: <{opertaion}> returns end code 0x{endcode:X4}.");
            Notification.Show(new ExceptionNotification(FriendlyName, $"<{opertaion}> returns end code 0x{endcode:X4}."),
                ShowAnimation.VerticalMove, false);
        }

        private void __add_exception_info(string opertaion, SLMP_EXCEPTION_CODE_T code)
        {
            if (ExceptionInfoCollection.Count > 128)
                ExceptionInfoCollection.RemoveAt(127);
            ExceptionInfoCollection.Insert(0, $"[{DateTime.Now.ToLocalTime()}]: <{opertaion}> returns exception code <{code}>.");
            if (code != SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
            {
                Notification.Show(new ExceptionNotification(FriendlyName, $"<{opertaion}> returns exception code <{code}>."),
                    ShowAnimation.VerticalMove, false);
            }
        }

        private static string __PROCESS_VALUE_TO_STRING(DECIMAL_POINT_POSITION_T position, short value)
        {
            string s = value.ToString(CultureInfo.InvariantCulture);
            if (position == DECIMAL_POINT_POSITION_T.FIRST_DECIMAL_PLACE)
            {
                if ((value < 0 && s.Length == 2) || (value >= 0 && s.Length == 1))
                    return s.Insert(s.Length - 1, "0.");
                //else if (s.EndsWith('0'))
                    //return s.Remove(s.Length - 1);
                else
                    return s.Insert(s.Length - 1, ".");
            }
            else
                return s;
        }

        private short __STRING_TO_PROCESS_VALUE(DECIMAL_POINT_POSITION_T position, string value)
        {
            short res;
            if (__PROCESS_VALUE_STRING_PATTERN.IsMatch(value) == false)
                throw new FormatException("The input string is not in a correct format.");

            if (__point_position == DECIMAL_POINT_POSITION_T.FIRST_DECIMAL_PLACE)
            {
                if(value.IndexOf('.') == -1)
                    res = short.Parse(value + "0");
                else
                    res = short.Parse(value.Remove(value.IndexOf('.'), 1));
            }
            else
                res = short.Parse(value);
            return res;
        }

        private bool __command_pending;
        public bool CommandPending
        {
            get { return __command_pending; }
            private set { if (value != __command_pending) SetProperty(ref __command_pending, value, false); }
        }
        private bool __enabled;
        public bool IsEnabled
        {
            get { return __enabled; }
            private set { if (value != __enabled) SetProperty(ref __enabled, value, false); }
        }

        private string __device_address = "U008";
        public string DeviceAddress
        {
            get { return __device_address; }
            set
            {
                if (_MODULE_ACCESS_EXTENSION_PATTERN.IsMatch(value))
                {
                    if (__device_address != value)
                        SetProperty(ref __device_address, value, true);
                }
                else
                    throw new ArgumentException(@"The input string for 'Device Address' is not in a correct format.");
            }
        }

        private DECIMAL_POINT_POSITION_T[] __startup_point_position = new DECIMAL_POINT_POSITION_T[4];

        private ushort __device_error_code;
        public ushort DeviceErrorCode
        {
            get { return __device_error_code; }
            private set { if (value != __device_error_code) SetProperty(ref __device_error_code, value, false); }
        }

        private DEVICE_OPERATION_MODE_T __device_operation_mode = DEVICE_OPERATION_MODE_T.SETTING_MODE;
        public DEVICE_OPERATION_MODE_T DeviceOperationMode
        {
            get { return __device_operation_mode; }
            private set { if (value != __device_operation_mode) SetProperty(ref __device_operation_mode, value, false); }
        }
        private DEVICE_OPERATION_MODE_T __device_operation_request = DEVICE_OPERATION_MODE_T.SETTING_MODE;
        public DEVICE_OPERATION_MODE_T DeviceOperationRequest
        {
            get { return __device_operation_request; }
            private set { if (value != __device_operation_request) SetProperty(ref __device_operation_request, value, false); }
        }
        private ushort __channel_operation_mode;
        public ushort ChannelOperationMode
        {
            get { return __channel_operation_mode;}
            private set { if(value != __channel_operation_mode) SetProperty(ref __channel_operation_mode, value, false); }
        }
        private ushort __channel_operation_request;
        public ushort ChannelOperationRequest
        {
            get { return __channel_operation_request; }
            private set { if (value != __channel_operation_request) SetProperty(ref __channel_operation_request, value, false); }
        }
        private ushort __at_status;
        public ushort ATStatus
        {
            get { return __at_status; }
            private set { if (value != __at_status) SetProperty(ref __at_status, value, false); }
        }
        private ushort __at_request;
        public ushort ATRequest
        {
            get { return __at_request; }
            private set { if (value != __at_request) SetProperty(ref __at_request, value, false); }
        }

        private int __selected_channel_index;
        public int SelectedChannelIndex 
        {
            get { return __selected_channel_index; }
            set { if(value != __selected_channel_index) SetProperty(ref __selected_channel_index, value, false); }
        }

        private DECIMAL_POINT_POSITION_T __point_position;
        public DECIMAL_POINT_POSITION_T FieldPointPosition 
        {
            get { return __point_position; } 
            private set { if (value != __point_position) SetProperty(ref __point_position, value, false); }
        }

        private short __temperature_process_value;
        public short TemperatureProcessValue
        {
            get { return __temperature_process_value; }
            private set { if(value != __temperature_process_value) SetProperty(ref __temperature_process_value, value, false); }
        }

        private short __set_value_monitor;
        public short SetValueMonitor
        {
            get { return __set_value_monitor; }
            private set { if(value != __set_value_monitor) SetProperty(ref __set_value_monitor, value, false); }
        }

        public short __manipulated_value_of_heating;
        public short ManipulatedValueOfHeating
        {
            get { return __manipulated_value_of_heating; }
            private set { if(value != __manipulated_value_of_heating) SetProperty(ref __manipulated_value_of_heating, value, false); }
        }
        public short __manipulated_value_of_cooling;
        public short ManipulatedValueOfCooling
        {
            get { return __manipulated_value_of_cooling; }
            private set { if (value != __manipulated_value_of_cooling) SetProperty(ref __manipulated_value_of_cooling, value, false); }
        }

        private ushort __heating_proportional_band;
        public ushort HeatingProportionalBand
        {
            get { return __heating_proportional_band; }
            private set { if (value != __heating_proportional_band) SetProperty(ref __heating_proportional_band, value, false); }
        }
        private ushort __cooling_proportional_band;
        public ushort CoolingProportionalBand
        {
            get { return __cooling_proportional_band; }
            private set { if (value != __cooling_proportional_band) SetProperty(ref __cooling_proportional_band, value, false); }
        }
        private ushort __integral_time;
        public ushort IntegralTime
        {
            get { return __integral_time; }
            private set { if (value != __integral_time) SetProperty(ref __integral_time, value, false); }
        }
        private ushort __derivative_time;
        public ushort DerivativeTime
        {
            get { return __derivative_time; }
            private set { if (value != __derivative_time) SetProperty(ref __derivative_time, value, false); }
        }
        private ushort __loop_disconnectin_detection;
        public ushort LoopDisconnectionDetection
        {
            get { return __loop_disconnectin_detection; }
            private set { if (value != __loop_disconnectin_detection) SetProperty(ref __loop_disconnectin_detection, value, false); }
        }

        private short[] __at_set_value_settings = new short[4];
        public double ATSetValueSetting 
        {
            get 
            {
                if (__startup_point_position[__selected_channel_index] == DECIMAL_POINT_POSITION_T.NOTHING)
                    return __at_set_value_settings[__selected_channel_index];
                else
                    return __at_set_value_settings[__selected_channel_index] / 10.0;
            }
            set 
            {
                short svalue;
                if (__startup_point_position[__selected_channel_index] == DECIMAL_POINT_POSITION_T.NOTHING)
                    svalue = Convert.ToInt16(value);
                else
                    svalue = Convert.ToInt16(value * 10.0);
                if (svalue != __at_set_value_settings[__selected_channel_index]) 
                    SetProperty(ref __at_set_value_settings[__selected_channel_index], svalue, true); }
        }
        public string SetValueSettingString
        {
            get 
            {
                return __PROCESS_VALUE_TO_STRING(__startup_point_position[__selected_channel_index], __at_set_value_settings[__selected_channel_index]);
            }
            set
            {
                short v = __STRING_TO_PROCESS_VALUE(__startup_point_position[__selected_channel_index], value);
                if (v != __at_set_value_settings[__selected_channel_index]) 
                    SetProperty(ref __at_set_value_settings[__selected_channel_index], v, true);
            }
        }

        private short[] __at_bias_settings = new short[4];
        public double ATBiasSetting 
        {
            get
            {
                if (__startup_point_position[__selected_channel_index] == DECIMAL_POINT_POSITION_T.NOTHING)
                    return __at_bias_settings[__selected_channel_index];
                else
                    return __at_bias_settings[__selected_channel_index] / 10.0;
            }
            set
            {
                short svalue;
                if (__startup_point_position[__selected_channel_index] == DECIMAL_POINT_POSITION_T.NOTHING)
                    svalue = Convert.ToInt16(value);
                else
                    svalue = Convert.ToInt16(value * 10.0);
                if (svalue != __at_bias_settings[__selected_channel_index])
                    SetProperty(ref __at_bias_settings[__selected_channel_index], svalue, true);
            }
        }
       
        private bool[] __at_loop_disconnection_detection_flag = new bool[4];
        public bool ATLoopDisconnectionDetectionFlag
        {
            get { return __at_loop_disconnection_detection_flag[__selected_channel_index]; }
            set { if (value != __at_loop_disconnection_detection_flag[__selected_channel_index]) SetProperty(ref __at_loop_disconnection_detection_flag[__selected_channel_index], value, true); }
        }

        private ushort[] __at_loop_disconnection_detection_settings = new ushort[4] { 480, 480, 480, 480};
        public ushort ATLoopDisconnectionDetectionSetting
        {
            get { return __at_loop_disconnection_detection_settings[__selected_channel_index]; }
            set
            {
                if (value > 7200)
                    throw new ArgumentException("The setting range is 0 to 7200 (s).");
                if (value != __at_loop_disconnection_detection_settings[__selected_channel_index]) 
                    SetProperty(ref __at_loop_disconnection_detection_settings[__selected_channel_index], value, true); 
            }
        }

        public CHANNEL_AT_MODE_T[] __at_mode_selections = new CHANNEL_AT_MODE_T[4] { CHANNEL_AT_MODE_T.STANDARD_MODE, CHANNEL_AT_MODE_T.STANDARD_MODE, CHANNEL_AT_MODE_T.STANDARD_MODE, CHANNEL_AT_MODE_T.STANDARD_MODE };
        public CHANNEL_AT_MODE_T ATModeSelection
        {
            get { return __at_mode_selections[__selected_channel_index]; }
            set { if (value != __at_mode_selections[__selected_channel_index]) SetProperty(ref __at_mode_selections[__selected_channel_index], value, true); }
        }

        private bool[] __at_automatic_backup_flag = new bool[4];
        public bool ATAutomaticBackupFlag
        {
            get { return __at_automatic_backup_flag[__selected_channel_index]; }
            set { if (value != __at_automatic_backup_flag[__selected_channel_index]) SetProperty(ref __at_automatic_backup_flag[__selected_channel_index], value, true); }
        }

        private ushort[] __ph = new ushort[4] { 30, 30, 30, 30 };
        public ushort Ph
        {
            get { return __ph[__selected_channel_index]; }
            set
            {
                if (value > 10000)
                    throw new ArgumentException("Heating proportional band (Ph) setting: 0 to 10000 (0.0% to 1000.0%).");
                if (value != __ph[__selected_channel_index]) 
                    SetProperty(ref __ph[__selected_channel_index], value, true); 
            }
        }
        private ushort[] __pc = new ushort[4];
        public ushort Pc
        {
            get { return __pc[__selected_channel_index]; }
            set
            {
                if (value > 10000 || value < 1)
                    throw new ArgumentException("Cooling proportional band (Pc) setting: 1 to 10000 (0.1% to 1000.0%).");
                if (value != __pc[__selected_channel_index]) 
                    SetProperty(ref __pc[__selected_channel_index], value, true); 
            }
        }
        private ushort[] __i = new ushort[4] { 240, 240, 240, 240};
        public ushort I
        {
            get { return __i[__selected_channel_index]; }
            set
            {
                if (value > 3600)
                    throw new ArgumentException("The setting range is 0 to 3600 (0 to 3600s).");
                if (value != __i[__selected_channel_index]) 
                    SetProperty(ref __i[__selected_channel_index], value, true); 
            }
        }
        private ushort[] __d = new ushort[4] { 60, 60, 60, 60 };
        public ushort D
        {
            get { return __d[__selected_channel_index]; }
            set
            {
                if (value > 3600)
                    throw new ArgumentException("The setting range is 0 to 3600 (0 to 3600s).");
                if (value != __d[__selected_channel_index])
                    SetProperty(ref __d[__selected_channel_index], value, true);
            }
        }
        private ushort[] __lp = new ushort[4] { 480, 480, 480, 480 };
        public ushort LP
        {
            get { return __lp[__selected_channel_index]; }
            set
            {
                if (value > 7200)
                    throw new ArgumentException("The setting range is 0 to 7200 (s).");
                if (value != __lp[__selected_channel_index])
                    SetProperty(ref __lp[__selected_channel_index], value, true);
            }
        }

        public bool Enable()
        {
            if (CommandPending)
                return false;

            __process_out_data_user_interface.control_out = new ENABLE_COMMAND_T() { device_address = DeviceAddress };
            CommandPending = true;
            return true;
        }

        public bool Disable()
        {
            if (CommandPending)
                return false;

            __process_out_data_user_interface.control_out = new DISABLE_COMMAND_T();
            CommandPending = true;
            return true;
        }

        public bool ClearDeviceError()
        {
            if (CommandPending)
                return false;

            __process_out_data_user_interface.control_out = new CLEAR_DEVICE_ERROR_T()
            {
                start_ticks = Environment.TickCount,
                timeout = 5000
            };
            CommandPending = true;
            return true;
        }

        public bool SwitchChannel()
        {
            if (CommandPending)
                return false;

            __process_out_data_user_interface.control_out = new SWITCH_CHANNEL_T() { channel_index = SelectedChannelIndex };
            CommandPending = true;
            return true;
        }

        public bool SetDeviceSettingMode()
        {
            if (CommandPending)
                return false;
            __process_out_data_user_interface.control_out = new SWITCH_DEVICE_OPERATION_MODE_T()
            { 
                mode = DEVICE_OPERATION_MODE_T.SETTING_MODE,
                start_ticks = Environment.TickCount,
                timeout = 5000
            };
            CommandPending = true;
            return true;
        }

        public bool SetDeviceOperationMode()
        {
            if (CommandPending)
                return false;
            __process_out_data_user_interface.control_out = new SWITCH_DEVICE_OPERATION_MODE_T()
            {
                mode = DEVICE_OPERATION_MODE_T.OPERATION_MODE,
                start_ticks = Environment.TickCount,
                timeout = 5000
            };
            CommandPending = true;
            return true;
        }

        public bool SetChannelAutoMode()
        {
            if (CommandPending)
                return false;
            __process_out_data_user_interface.control_out = new SWITCH_CHANNEL_OPERATION_MODE_T()
            {
                channel_index = SelectedChannelIndex,
                mode = CHANNEL_OPERATION_MODE_T.AUTO,
                start_ticks = Environment.TickCount,
                timeout = 5000
            };
            CommandPending = true;
            return true;
        }

        public bool SetChannelManualMode()
        {
            if (CommandPending)
                return false;
            __process_out_data_user_interface.control_out = new SWITCH_CHANNEL_OPERATION_MODE_T()
            {
                channel_index = SelectedChannelIndex,
                mode = CHANNEL_OPERATION_MODE_T.MANUAL,
                start_ticks = Environment.TickCount,
                timeout = 5000
            };
            CommandPending = true;
            return true;
        }

        public bool SetATFlag()
        {
            if (CommandPending)
                return false;
            __process_out_data_user_interface.control_out = new SET_AT_REQUEST()
            {
                channel_index = SelectedChannelIndex,
                set_value_setting = __at_set_value_settings[SelectedChannelIndex],
                at_bias_setting = __at_bias_settings[SelectedChannelIndex],
                at_loop_disconnection_detection_flag = ATLoopDisconnectionDetectionFlag,
                at_loop_disconnection_detection_setting = ATLoopDisconnectionDetectionSetting,
                at_mode = ATModeSelection,
                at_automatic_backup_flag = ATAutomaticBackupFlag
            };
            CommandPending = true;
            return true;
        }

        public bool ResetATFlag()
        {
            if (CommandPending)
                return false;
            __process_out_data_user_interface.control_out = new RESET_AT_REQUEST()
            {
                channel_index = SelectedChannelIndex
            };
            CommandPending = true;
            return true;
        }

        public bool BackupDevicePIDConstants()
        {
            if (CommandPending)
                return false;
            __process_out_data_user_interface.control_out = new BACKUP_PID_CONSTANTS_T()
            {
                Ph = new ReadOnlyMemory<ushort>(__ph),
                Pc = new ReadOnlyMemory<ushort>(__pc),
                I = new ReadOnlyMemory<ushort>(__i),
                D = new ReadOnlyMemory<ushort>(__d),
                LP = new ReadOnlyMemory<ushort>(__lp),
                start_ticks = Environment.TickCount,
                timeout = 30000
            };
            CommandPending = true;
            return true;
        }

        public override void ExchangeDataWithUserInterface()
        {
            __user_read_data();
            var res = __process_in_data_user_interface.control_in;
            if (res != null)
            {
                switch (res)
                {
                    case BACKUP_PID_CONSTANTS_RESULT_T bpres:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"{res.cmd}", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"{res.cmd}", res.end_code);
                        else if (bpres.timeout == true)
                            __add_exception_info($"{bpres.cmd}", "Operation Timout");
                        else if (bpres.failure == true)
                            __add_exception_info($"{bpres.cmd}", "Operation Failed");
                        break;
                    case BLOCKING_RESULT bres:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"{res.cmd}", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"{res.cmd}", res.end_code);
                        else if (bres.timeout == true)
                            __add_exception_info($"{bres.cmd}", "Operation Timout");
                        break;
                    case ASYNC_RESULT_T ares:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"{res.cmd}", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"{res.cmd}", res.end_code);
                        else
                        {
                            switch (ares.cmd)
                            {
                                case ASYNC_COMMAND_CODE_T.ENABLE:
                                    IsEnabled = true;
                                    __startup_point_position[0] = (ares as ENABLE_RESULT_T).position0 == (ushort)DECIMAL_POINT_POSITION_T.NOTHING ? DECIMAL_POINT_POSITION_T.NOTHING : DECIMAL_POINT_POSITION_T.FIRST_DECIMAL_PLACE;
                                    __startup_point_position[1] = (ares as ENABLE_RESULT_T).position1 == (ushort)DECIMAL_POINT_POSITION_T.NOTHING ? DECIMAL_POINT_POSITION_T.NOTHING : DECIMAL_POINT_POSITION_T.FIRST_DECIMAL_PLACE;
                                    __startup_point_position[2] = (ares as ENABLE_RESULT_T).position2 == (ushort)DECIMAL_POINT_POSITION_T.NOTHING ? DECIMAL_POINT_POSITION_T.NOTHING : DECIMAL_POINT_POSITION_T.FIRST_DECIMAL_PLACE;
                                    __startup_point_position[3] = (ares as ENABLE_RESULT_T).position3 == (ushort)DECIMAL_POINT_POSITION_T.NOTHING ? DECIMAL_POINT_POSITION_T.NOTHING : DECIMAL_POINT_POSITION_T.FIRST_DECIMAL_PLACE;
                                    SetProperty(ref __at_set_value_settings[__selected_channel_index], __at_set_value_settings[__selected_channel_index], false, nameof(ATSetValueSetting));
                                    SetProperty(ref __at_bias_settings[__selected_channel_index], __at_bias_settings[__selected_channel_index], false, nameof(ATBiasSetting));
                                    break;
                                case ASYNC_COMMAND_CODE_T.DISABLE:
                                    IsEnabled = false;
                                    break;
                                case ASYNC_COMMAND_CODE_T.SWITCH_PROCESS_IO_CHANNEL:
                                    SetProperty(ref __at_set_value_settings[__selected_channel_index], __at_set_value_settings[__selected_channel_index], false, nameof(ATSetValueSetting));
                                    SetProperty(ref __at_bias_settings[__selected_channel_index], __at_bias_settings[__selected_channel_index], false, nameof(ATBiasSetting));
                                    SetProperty(ref __at_loop_disconnection_detection_flag[__selected_channel_index], __at_loop_disconnection_detection_flag[__selected_channel_index], false, nameof(ATLoopDisconnectionDetectionFlag));
                                    SetProperty(ref __at_loop_disconnection_detection_settings[__selected_channel_index], __at_loop_disconnection_detection_settings[__selected_channel_index], false, nameof(ATLoopDisconnectionDetectionSetting));
                                    SetProperty(ref __at_mode_selections[__selected_channel_index], __at_mode_selections[__selected_channel_index], false, nameof(ATModeSelection));
                                    SetProperty(ref __at_automatic_backup_flag[__selected_channel_index], __at_automatic_backup_flag[__selected_channel_index], false, nameof(ATAutomaticBackupFlag));

                                    SetProperty(ref __ph[__selected_channel_index], __ph[__selected_channel_index], false, nameof(Ph));
                                    SetProperty(ref __pc[__selected_channel_index], __pc[__selected_channel_index], false, nameof(Pc));
                                    SetProperty(ref __i[__selected_channel_index], __i[__selected_channel_index], false, nameof(I));
                                    SetProperty(ref __d[__selected_channel_index], __d[__selected_channel_index], false, nameof(D));
                                    SetProperty(ref __lp[__selected_channel_index], __lp[__selected_channel_index], false, nameof(LP));
                                    break;
                            }
                        }
                        break;
                }
                __process_in_data_user_interface.control_in = null;
                CommandPending = false;
            }
            if(IsEnabled)
            {
                if (__process_in_data_user_interface.end_code != 0)
                {
                    __add_exception_info("Reading ProcessIO", __process_in_data_user_interface.end_code);
                    IsEnabled = false;
                }
                else if (__process_in_data_user_interface.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                {
                    __add_exception_info("Reading ProcessIO", __process_in_data_user_interface.exception_code);
                    IsEnabled = false;
                }
                else
                {
                    DeviceErrorCode = __process_in_data_user_interface.device_process_in.device_error_code;
                    DeviceOperationMode =
                        __process_in_data_user_interface.device_process_in.device_operation_mode == (ushort)DEVICE_OPERATION_MODE_T.OPERATION_MODE ? DEVICE_OPERATION_MODE_T.OPERATION_MODE : DEVICE_OPERATION_MODE_T.SETTING_MODE;
                    DeviceOperationRequest =
                        __process_in_data_user_interface.device_process_in.device_operation_request == (ushort)DEVICE_OPERATION_MODE_T.OPERATION_MODE ? DEVICE_OPERATION_MODE_T.OPERATION_MODE : DEVICE_OPERATION_MODE_T.SETTING_MODE;
                    ChannelOperationMode = __process_in_data_user_interface.device_process_in.man_mode_shift_completion_flag;
                    ChannelOperationRequest = __process_in_data_user_interface.device_process_in.man_mode_shift_request_flag;
                    ATStatus = __process_in_data_user_interface.device_process_in.at_status;
                    ATRequest = __process_in_data_user_interface.device_process_in.at_request;

                    int index = __process_in_data_user_interface.device_process_in.index;
                    FieldPointPosition = 
                        __process_in_data_user_interface.channel_process_in[index].point_position == (ushort)DECIMAL_POINT_POSITION_T.NOTHING ? DECIMAL_POINT_POSITION_T.NOTHING : DECIMAL_POINT_POSITION_T.FIRST_DECIMAL_PLACE;
                    TemperatureProcessValue = __process_in_data_user_interface.channel_process_in[index].temperature_process_value;
                    SetValueMonitor = __process_in_data_user_interface.channel_process_in[index].set_value;
                    ManipulatedValueOfHeating = __process_in_data_user_interface.channel_process_in[index].manipulated_value_of_heating;
                    ManipulatedValueOfCooling = __process_in_data_user_interface.channel_process_in[index].manipulated_value_of_cooling;
                    HeatingProportionalBand = __process_in_data_user_interface.channel_process_in[index].heating_proportional_band;
                    CoolingProportionalBand = __process_in_data_user_interface.channel_process_in[index].cooling_proportional_band;
                    IntegralTime = __process_in_data_user_interface.channel_process_in[index].integral_time;
                    DerivativeTime = __process_in_data_user_interface.channel_process_in[index].derivative_time;
                    LoopDisconnectionDetection = __process_in_data_user_interface.channel_process_in[index].loop_disconnection_detection;
                }
            }
            __user_write_data();
        }

        public override void ExchangeDataWithDevice(DeviceAccessMaster master, ushort monitoring)
        {
            __device_read_data();
            ushort end = 0;
            ushort[] temporaryX = new ushort[1];
            ushort[] temporaryY = new ushort[1];
            byte[] temporaryBYTE = new byte[1];
            var res = __process_out_data_device.control_out;
            switch (res)
            {
                case ENABLE_COMMAND_T enable:
                    __device_sync_address = Convert.ToUInt32(enable.device_address[1..], 16) * 16;
                    __device_sync_specification = enable.device_address;

                    __device_module_access_address_table = new List<(string, uint)>()
                    {
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.DV_WRITE_DATA_ERROR_CODE),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.DV_MAN_SHIFT_COMPLETION)
                    };
                    __device_module_access_data = new ushort[2];
                    __channel_module_access_address_table = new List<(string, uint)>[4];
                    __channel_module_access_data = new ushort[4][];
                    __channel_module_access_address_table[0] = new List<(string, uint)>()
                    {
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH0_DECIMAL_POINT_POSITION),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH0_TEMPERATURE_PROCESS_VALUE),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH0_SET_VALUE_MONITOR),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH0_MANIPULATED_VALUE_OF_HEATING),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH0_MANIPULATED_VALUE_OF_COOLING),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH0_HEATING_PROPORTIONAL_BAND),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH0_COOLING_PROPORTIONAL_BAND),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH0_INTEGRAL_TIME),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH0_DERIVATIVE_TIME),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH0_LOOP_DISCONNECTION_DETECTION),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH0_AUTO_MAN_MODE_SHIFT)
                    };
                    __channel_module_access_data[0] = new ushort[__channel_module_access_address_table[0].Count];
                    __channel_module_access_address_table[1] = new List<(string, uint)>()
                    {
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH1_DECIMAL_POINT_POSITION),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH1_TEMPERATURE_PROCESS_VALUE),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH1_SET_VALUE_MONITOR),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH1_MANIPULATED_VALUE_OF_HEATING),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH1_MANIPULATED_VALUE_OF_COOLING),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH1_HEATING_PROPORTIONAL_BAND),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH1_COOLING_PROPORTIONAL_BAND),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH1_INTEGRAL_TIME),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH1_DERIVATIVE_TIME),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH1_LOOP_DISCONNECTION_DETECTION),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH1_AUTO_MAN_MODE_SHIFT)
                    };
                    __channel_module_access_data[1] = new ushort[__channel_module_access_address_table[1].Count];
                    __channel_module_access_address_table[2] = new List<(string, uint)>()
                    {
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH2_DECIMAL_POINT_POSITION),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH2_TEMPERATURE_PROCESS_VALUE),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH2_SET_VALUE_MONITOR),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH2_MANIPULATED_VALUE_OF_HEATING),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH2_MANIPULATED_VALUE_OF_COOLING),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH2_HEATING_PROPORTIONAL_BAND),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH2_COOLING_PROPORTIONAL_BAND),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH2_INTEGRAL_TIME),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH2_DERIVATIVE_TIME),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH2_LOOP_DISCONNECTION_DETECTION),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH2_AUTO_MAN_MODE_SHIFT)
                    };
                    __channel_module_access_data[2] = new ushort[__channel_module_access_address_table[2].Count];
                    __channel_module_access_address_table[3] = new List<(string, uint)>()
                    {
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH3_DECIMAL_POINT_POSITION),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH3_TEMPERATURE_PROCESS_VALUE),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH3_SET_VALUE_MONITOR),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH3_MANIPULATED_VALUE_OF_HEATING),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH3_MANIPULATED_VALUE_OF_COOLING),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH3_HEATING_PROPORTIONAL_BAND),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH3_COOLING_PROPORTIONAL_BAND),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH3_INTEGRAL_TIME),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH3_DERIVATIVE_TIME),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH3_LOOP_DISCONNECTION_DETECTION),
                        (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH3_AUTO_MAN_MODE_SHIFT)
                    };
                    __channel_module_access_data[3] = new ushort[__channel_module_access_address_table[3].Count];

                    ushort[] positions = new ushort[4];
                    master.ReadModuleAccessDeviceInWord(monitoring, 
                        new (string, uint)[4]
                        {
                            (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH0_DECIMAL_POINT_POSITION),
                            (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH1_DECIMAL_POINT_POSITION),
                            (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH2_DECIMAL_POINT_POSITION),
                            (enable.device_address, (uint)Q64TC_ADDRESS_TABLE_T.CH3_DECIMAL_POINT_POSITION),
                        }, null, out end, positions, null);
                    if (end == 0)
                        __process_in_data_device.control_in = new ENABLE_RESULT_T()
                        {
                            end_code = end,
                            exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                            position0 = positions[0],
                            position1 = positions[1],
                            position2 = positions[2],
                            position3 = positions[3],
                        };
                    else
                        __process_in_data_device.control_in = new ENABLE_RESULT_T() { cmd = ASYNC_COMMAND_CODE_T.ENABLE,  end_code = end, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR };
                    __process_out_data_device.control_out = null;
                    __device_sync_control = true;
                    __device_sync_channel = 0;
                    __process_in_data_device.device_process_in.index = 0;
                    break;
                case DISABLE_COMMAND_T:
                    __process_in_data_device.control_in = new ASYNC_RESULT_T() { cmd = ASYNC_COMMAND_CODE_T.DISABLE, end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR };
                    __process_out_data_device.control_out = null;
                    __device_sync_control = false;
                    break;
                case SWITCH_CHANNEL_T sc:
                    __process_in_data_device.control_in = new ASYNC_RESULT_T() { cmd = ASYNC_COMMAND_CODE_T.SWITCH_PROCESS_IO_CHANNEL, end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR };
                    __process_out_data_device.control_out = null;
                    __device_sync_channel = sc.channel_index;
                    break;
                case SET_AT_REQUEST sar:
                    try
                    {
                        master.ReadModuleAccessDeviceInWord(monitoring, __device_sync_specification, (uint)Q64TC_ADDRESS_TABLE_T.CH_AT_LOOP_DISCONNECTION_DETECTION_FLAG, 1, out end, temporaryX);
                        ushort flag;
                        if (sar.at_loop_disconnection_detection_flag)
                            flag = (ushort)(temporaryX[0] | (ushort)(1 << sar.channel_index));
                        else
                            flag = (ushort)(temporaryX[0] & ((ushort)~(1 << sar.channel_index)));
                        if (end == 0)
                        {
                            var variables = new List<(string, uint, ushort)>()
                            {
                                (__device_address, __at_parameter_address_table[sar.channel_index][0], (ushort)sar.set_value_setting),
                                (__device_address, __at_parameter_address_table[sar.channel_index][1], (ushort)sar.at_bias_setting),
                                (__device_address, __at_parameter_address_table[sar.channel_index][2], flag),
                                (__device_address, __at_parameter_address_table[sar.channel_index][3], sar.at_loop_disconnection_detection_setting),
                                (__device_address, __at_parameter_address_table[sar.channel_index][4], (ushort)sar.at_mode),
                                (__device_address, __at_parameter_address_table[sar.channel_index][5], sar.at_automatic_backup_flag == true?(ushort)1:(ushort)0)
                            };
                            master.WriteModuleAccessDeviceInWord(monitoring, variables, null, out end);
                        }
                        if(end == 0)
                            master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)((ushort)Q64TC_ADDRESS_TABLE_T.Y_CH0_AUTO_TUNING_REQUEST + sar.channel_index), 1, out end, new byte[] { 1 });
                        __process_in_data_device.control_in = new ASYNC_RESULT_T() { cmd = ASYNC_COMMAND_CODE_T.SET_AT_FLAG, end_code = end, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR };
                        __process_out_data_device.control_out = null;
                    }
                    catch (SLMPException ex)
                    {
                        __process_in_data_device.control_in = new ASYNC_RESULT_T() { cmd = ASYNC_COMMAND_CODE_T.SET_AT_FLAG, end_code = 0, exception_code = ex.ExceptionCode };
                        __process_out_data_device.control_out = null;
                        if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                            throw;
                    }
                    break;
                case RESET_AT_REQUEST rar:
                    try
                    {
                        master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)((ushort)Q64TC_ADDRESS_TABLE_T.Y_CH0_AUTO_TUNING_REQUEST + rar.channel_index), 1, out end, new byte[] { 0 });
                        __process_in_data_device.control_in = new ASYNC_RESULT_T() { cmd = ASYNC_COMMAND_CODE_T.RESET_AT_FLAG, end_code = end, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR };
                        __process_out_data_device.control_out = null;
                    }
                    catch (SLMPException ex)
                    {
                        __process_in_data_device.control_in = new ASYNC_RESULT_T() { cmd = ASYNC_COMMAND_CODE_T.RESET_AT_FLAG, end_code = 0, exception_code = ex.ExceptionCode };
                        __process_out_data_device.control_out = null;
                        if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                            throw;
                    }
                    break;
                case BLOCKING_COMMAND_T block:
                    try 
                    {
                        switch(block.cmd)
                        {
                            case ASYNC_COMMAND_CODE_T.CLEAR_DEVICE_ERROR:
                                switch (block.step)
                                {
                                    case 0:
                                        master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)Q64TC_ADDRESS_TABLE_T.Y_DV_ERROR_RESET, 1, out end, new byte[] { 1 });
                                        if (end == 0) block.step++;
                                        break;
                                    case 1:
                                        master.ReadLocalDeviceInBit(monitoring, "X", __device_sync_address + (uint)Q64TC_ADDRESS_TABLE_T.X_DV_ERROR_FLAG, 1, out end, temporaryBYTE);
                                        if (end == 0 && temporaryBYTE[0] == 0)
                                        {
                                            __process_in_data_device.control_in = new BLOCKING_RESULT() { cmd = block.cmd, end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, timeout = false };
                                            __process_out_data_device.control_out = null;
                                        }
                                        break;
                                }
                                break;
                            case ASYNC_COMMAND_CODE_T.SWITCH_DEVICE_OPERATION_MODE:
                                switch(block.step)
                                {
                                    case 0:
                                        master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)Q64TC_ADDRESS_TABLE_T.Y_DV_OPERATION_MODE_INSTRUCTION, 1, out end, new byte[] { (byte)(block as SWITCH_DEVICE_OPERATION_MODE_T).mode });
                                        if (end == 0) block.step++;
                                        break;
                                    case 1:
                                        master.ReadLocalDeviceInBit(monitoring, "X", __device_sync_address + (uint)Q64TC_ADDRESS_TABLE_T.X_DV_OPERATION_MODE_STATUS, 1, out end, temporaryBYTE);
                                        if (end == 0 && temporaryBYTE[0] == (byte)(block as SWITCH_DEVICE_OPERATION_MODE_T).mode)
                                        {
                                            __process_in_data_device.control_in = new BLOCKING_RESULT() { cmd = block.cmd, end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, timeout = false };
                                            __process_out_data_device.control_out = null;
                                        }
                                        break;
                                }
                                break;
                            case ASYNC_COMMAND_CODE_T.SWITCH_CHANNEL_OPERATION_MODE:
                                SWITCH_CHANNEL_OPERATION_MODE_T scm = block as SWITCH_CHANNEL_OPERATION_MODE_T;
                                switch (block.step)
                                {
                                    case 0:
                                        master.WriteModuleAccessDeviceInWord(monitoring, __device_sync_specification,
                                            (uint)((uint)Q64TC_ADDRESS_TABLE_T.CH0_AUTO_MAN_MODE_SHIFT + ((uint)Q64TC_ADDRESS_TABLE_T.CHANNEL_DISTANCE * scm.channel_index)),
                                            1, out end, new ushort[1] { (ushort)scm.mode });
                                        if (end == 0) block.step++;
                                        break;
                                    case 1:
                                        master.ReadModuleAccessDeviceInWord(monitoring, __device_sync_specification,
                                            (uint)Q64TC_ADDRESS_TABLE_T.DV_MAN_SHIFT_COMPLETION, 1, out end, temporaryX);
                                        if (end == 0 && ((temporaryX[0] & (1 << scm.channel_index)) >> scm.channel_index) == (ushort)scm.mode)
                                        {
                                            __process_in_data_device.control_in = new BLOCKING_RESULT() { cmd = block.cmd, end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, timeout = false };
                                            __process_out_data_device.control_out = null;
                                        }
                                        break;
                                }
                                break;
                            case ASYNC_COMMAND_CODE_T.BACKUP_DEVICE_CONSTANTS:
                                BACKUP_PID_CONSTANTS_T bpc = block as BACKUP_PID_CONSTANTS_T;
                                switch (block.step)
                                {
                                    case 0:
                                        master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)Q64TC_ADDRESS_TABLE_T.Y_DV_E2PROM_BACKUP_INSTRUCTION, 1, out end, new byte[] { 0 });
                                        if (end == 0) block.step++;
                                        break;
                                    case 1:
                                        master.ReadLocalDeviceInBit(monitoring, "X", __device_sync_address + (uint)Q64TC_ADDRESS_TABLE_T.X_DV_E2PROM_BACKUP_COMPLETION, 1, out end, temporaryBYTE);
                                        if (end == 0 && temporaryBYTE[0] == 0) block.step++;
                                        break;
                                    case 2:
                                        var variables = new List<(string, uint, ushort)>();
                                        for(int i = 0; i < 4; ++i)
                                        {
                                            variables.Add((__device_address, __pid_constant_address_table[i][0], bpc.Ph.Span[i]));
                                            variables.Add((__device_address, __pid_constant_address_table[i][1], bpc.Pc.Span[i]));
                                            variables.Add((__device_address, __pid_constant_address_table[i][2], bpc.I.Span[i]));
                                            variables.Add((__device_address, __pid_constant_address_table[i][3], bpc.D.Span[i]));
                                            variables.Add((__device_address, __pid_constant_address_table[i][4], bpc.LP.Span[i]));
                                        }
                                        master.WriteModuleAccessDeviceInWord(monitoring, variables, null, out end);
                                        if(end == 0)
                                            master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)Q64TC_ADDRESS_TABLE_T.Y_DV_E2PROM_BACKUP_INSTRUCTION, 1, out end, new byte[] { 1 });
                                        if (end == 0) block.step++;
                                        break;
                                    case 3:
                                        master.ReadLocalDeviceInBit(monitoring, "X", __device_sync_address + (uint)Q64TC_ADDRESS_TABLE_T.X_DV_E2PROM_BACKUP_COMPLETION, 1, out end, temporaryBYTE);
                                        if (end == 0 && temporaryBYTE[0] == 1)
                                        {
                                            __process_in_data_device.control_in = new BLOCKING_RESULT() { cmd = block.cmd, end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, timeout = false };
                                            __process_out_data_device.control_out = null;
                                        }
                                        break;
                                }
                                break;
                        }
                        if (__process_out_data_device.control_out != null)
                        {
                            if (end != 0)
                            {
                                __process_in_data_device.control_in = new BLOCKING_RESULT() { cmd = block.cmd, end_code = end, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, timeout = false };
                                __process_out_data_device.control_out = null;
                            }
                            else if (Math.Abs(Environment.TickCount - block.start_ticks) > block.timeout)
                            {
                                if(block.cmd == ASYNC_COMMAND_CODE_T.BACKUP_DEVICE_CONSTANTS)
                                {
                                    master.ReadLocalDeviceInBit(monitoring, "X", __device_sync_address + (uint)Q64TC_ADDRESS_TABLE_T.X_DV_E2PROM_BACKUP_FAILURE, 1, out end, temporaryBYTE);
                                    if (end != 0)
                                        __process_in_data_device.control_in = new BLOCKING_RESULT() { cmd = block.cmd, end_code = end, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, timeout = false };
                                    else if (temporaryBYTE[0] == 1)
                                        __process_in_data_device.control_in = new BACKUP_PID_CONSTANTS_RESULT_T() { end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, timeout = false, failure = true };
                                    else
                                        __process_in_data_device.control_in = new BLOCKING_RESULT() { cmd = block.cmd, end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, timeout = true };
                                }
                                else
                                    __process_in_data_device.control_in = new BLOCKING_RESULT() { cmd = block.cmd, end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, timeout = true };
                                __process_out_data_device.control_out = null;
                            }
                        }
                    }
                    catch (SLMPException ex)
                    {
                        __process_in_data_device.control_in = new BLOCKING_RESULT() { cmd = block.cmd, end_code = 0, exception_code = ex.ExceptionCode };
                        __process_out_data_device.control_out = null;

                        if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                            throw;
                    }
                    finally
                    {
                        if (__process_out_data_device.control_out == null)
                        {
                            switch(block.cmd)
                            {
                                case ASYNC_COMMAND_CODE_T.CLEAR_DEVICE_ERROR:
                                    if(block.step != 0)
                                        master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)Q64TC_ADDRESS_TABLE_T.Y_DV_ERROR_RESET, 1, out _, new byte[] { 0 });
                                    break;
                                case ASYNC_COMMAND_CODE_T.BACKUP_DEVICE_CONSTANTS:
                                    if (block.step != 0)
                                        master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)Q64TC_ADDRESS_TABLE_T.Y_DV_E2PROM_BACKUP_INSTRUCTION, 1, out _, new byte[] { 0 });
                                    break;
                            }
                        }
                    }
                    break;
            }
            if (__device_sync_control)
            {
                try
                {
                    master.ReadLocalDeviceInWord(monitoring, "X", __device_sync_address, 1, out __process_in_data_device.end_code, temporaryX);
                    if (__process_in_data_device.end_code == 0)
                        master.ReadLocalDeviceInWord(monitoring, "Y", __device_sync_address, 1, out __process_in_data_device.end_code, temporaryY);
                    if (__process_in_data_device.end_code == 0)
                        master.ReadModuleAccessDeviceInWord(monitoring, __channel_module_access_address_table[__device_sync_channel], null, out __process_in_data_device.end_code,  __channel_module_access_data[__device_sync_channel], null);
                    if (__process_in_data_device.end_code == 0)
                        master.ReadModuleAccessDeviceInWord(monitoring, __device_module_access_address_table, null, out __process_in_data_device.end_code, __device_module_access_data, null);
                    if (__process_in_data_device.end_code == 0)
                    {
                        __process_in_data_device.device_process_in.device_operation_mode = (ushort)((temporaryX[0] & 0x0002) >> 1);
                        __process_in_data_device.device_process_in.at_status = (ushort)((temporaryX[0] & 0x00F0) >> 4);
                        __process_in_data_device.device_process_in.device_error_code = __device_module_access_data[0];
                        __process_in_data_device.device_process_in.man_mode_shift_completion_flag = __device_module_access_data[1];
                        __process_in_data_device.device_process_in.at_request = (ushort)((temporaryY[0] & 0x00F0) >> 4);
                        __process_in_data_device.device_process_in.device_operation_request = (ushort)((temporaryY[0] & 0x0002) >> 1);

                        __process_in_data_device.device_process_in.index = __device_sync_channel;
                        __process_in_data_device.channel_process_in[__device_sync_channel].point_position = __channel_module_access_data[__device_sync_channel][0];
                        __process_in_data_device.channel_process_in[__device_sync_channel].temperature_process_value = (short)__channel_module_access_data[__device_sync_channel][1];
                        __process_in_data_device.channel_process_in[__device_sync_channel].set_value = (short)__channel_module_access_data[__device_sync_channel][2];
                        __process_in_data_device.channel_process_in[__device_sync_channel].manipulated_value_of_heating = (short)__channel_module_access_data[__device_sync_channel][3];
                        __process_in_data_device.channel_process_in[__device_sync_channel].manipulated_value_of_cooling = (short)__channel_module_access_data[__device_sync_channel][4];
                        __process_in_data_device.channel_process_in[__device_sync_channel].heating_proportional_band = __channel_module_access_data[__device_sync_channel][5];
                        __process_in_data_device.channel_process_in[__device_sync_channel].cooling_proportional_band = __channel_module_access_data[__device_sync_channel][6];
                        __process_in_data_device.channel_process_in[__device_sync_channel].integral_time = __channel_module_access_data[__device_sync_channel][7];
                        __process_in_data_device.channel_process_in[__device_sync_channel].derivative_time = __channel_module_access_data[__device_sync_channel][8];
                        __process_in_data_device.channel_process_in[__device_sync_channel].loop_disconnection_detection = __channel_module_access_data[__device_sync_channel][9];
                        if (__channel_module_access_data[__device_sync_channel][10] == 0)
                            __process_in_data_device.device_process_in.man_mode_shift_request_flag &= (ushort)~(1 << __device_sync_channel);
                        else
                            __process_in_data_device.device_process_in.man_mode_shift_request_flag |= (ushort)(1 << __device_sync_channel);

                    }
                    else
                        __device_sync_control = false;
                    __process_in_data_device.exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR;
                }
                catch (SLMPException ex)
                {
                    __process_in_data_device.end_code = 0;
                    __process_in_data_device.exception_code = ex.ExceptionCode;
                    __device_sync_control = false;
                    if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                        throw;
                }
            }
            __device_write_data();
        }

        public override long Restore(ref Utf8JsonReader reader)
        {
            long start = reader.BytesConsumed;
            if (reader.Read() == false || reader.TokenType != JsonTokenType.StartObject)
                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The given JSON object is not a valid data model instance.");

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string property = reader.GetString();
                    switch(property)
                    {
                        case "Device Address":
                            if (reader.Read() && reader.TokenType == JsonTokenType.String && _MODULE_ACCESS_EXTENSION_PATTERN.IsMatch(reader.GetString()))
                                SetProperty(ref __device_address, reader.GetString(), false, nameof(DeviceAddress));
                            else
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid string.");
                            break;
                        default:
                            if(property.StartsWith("CH") && int.TryParse(property[2..3], out int index))
                            {
                                if (reader.Read() == false || reader.TokenType != JsonTokenType.StartObject)
                                    throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid object.");
                                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                                {
                                    if (reader.TokenType == JsonTokenType.PropertyName)
                                    {
                                        string channelProperty = reader.GetString();
                                        switch (channelProperty)
                                        {
                                            case nameof(ATModeSelection):
                                                if (reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt16(out ushort mode) && 
                                                    Enum.IsDefined(typeof(CHANNEL_AT_MODE_T), mode))
                                                    SetProperty(ref __at_mode_selections[index], (CHANNEL_AT_MODE_T)mode, false, nameof(ATModeSelection));
                                                else
                                                    throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {channelProperty} in given JSON object is not a valid <CHANNEL_AT_MODE_T> value.");
                                                break;
                                            case nameof(ATSetValueSetting):
                                                if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetInt16(out __at_set_value_settings[index])) == false)
                                                    throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {channelProperty} in given JSON object is not a valid int16 number.");
                                                else
                                                    SetProperty(ref __at_set_value_settings[index], __at_set_value_settings[index], false, nameof(ATSetValueSetting));
                                                break;
                                            case nameof(ATBiasSetting):
                                                if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetInt16(out __at_bias_settings[index])) == false)
                                                    throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {channelProperty} in given JSON object is not a valid int16 number.");
                                                else
                                                    SetProperty(ref __at_bias_settings[index], __at_bias_settings[index], false, nameof(ATBiasSetting));
                                                break;
                                            case nameof(ATLoopDisconnectionDetectionFlag):
                                                if (reader.Read() && (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False))
                                                    SetProperty(ref __at_loop_disconnection_detection_flag[index], reader.GetBoolean(), false, nameof(ATLoopDisconnectionDetectionFlag));
                                                else
                                                    throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {channelProperty} in given JSON object is not a valid boolean value.");
                                                break;
                                            case nameof(ATAutomaticBackupFlag):
                                                if (reader.Read() && (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False))
                                                    SetProperty(ref __at_automatic_backup_flag[index], reader.GetBoolean(), false, nameof(ATAutomaticBackupFlag));
                                                else
                                                    throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {channelProperty} in given JSON object is not a valid boolean value.");
                                                break;
                                            case nameof(ATLoopDisconnectionDetectionSetting):
                                                if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt16(out __at_loop_disconnection_detection_settings[index])) == false &&
                                                    __at_loop_disconnection_detection_settings[index] <= 7200)
                                                    throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {channelProperty} in given JSON object is not a valid uint16 number.");
                                                else
                                                    SetProperty(ref __at_loop_disconnection_detection_settings[index], __at_loop_disconnection_detection_settings[index], false, nameof(ATLoopDisconnectionDetectionSetting));
                                                break;
                                            case nameof(Ph):
                                                if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt16(out __ph[index])) == false &&
                                                    __ph[index] <= 10000)
                                                    throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {channelProperty} in given JSON object is not a valid uint16 number.");
                                                else
                                                    SetProperty(ref __ph[index], __ph[index], false, nameof(Ph));
                                                break;
                                            case nameof(Pc):
                                                if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt16(out __pc[index])) == false &&
                                                    __pc[index] <= 10000 && __pc[index] >= 1)
                                                    throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {channelProperty} in given JSON object is not a valid uint16 number.");
                                                else
                                                    SetProperty(ref __pc[index], __pc[index], false, nameof(Pc));
                                                break;
                                            case nameof(I):
                                                if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt16(out __i[index])) == false &&
                                                    __i[index] <= 3600)
                                                    throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {channelProperty} in given JSON object is not a valid uint16 number.");
                                                else
                                                    SetProperty(ref __i[index], __i[index], false, nameof(I));
                                                break;
                                            case nameof(D):
                                                if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt16(out __d[index])) == false &&
                                                    __d[index] <= 3600)
                                                    throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {channelProperty} in given JSON object is not a valid uint16 number.");
                                                else
                                                    SetProperty(ref __d[index], __d[index], false, nameof(D));
                                                break;
                                            case nameof(LP):
                                                if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt16(out __lp[index])) == false &&
                                                    __lp[index] <= 7200)
                                                    throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {channelProperty} in given JSON object is not a valid uint16 number.");
                                                else
                                                    SetProperty(ref __lp[index], __lp[index], false, nameof(LP));
                                                break;
                                        }

                                    }
                                }
                            }
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

            writer.WritePropertyName("Device Address");
            writer.WriteStringValue(DeviceAddress);

            for (int i = 0; i < 4; ++i)
            {
                writer.WriteStartObject($"CH{i}");

                writer.WriteNumber(nameof(ATSetValueSetting), __at_set_value_settings[i]);
                writer.WriteNumber(nameof(ATBiasSetting), __at_bias_settings[i]);
                writer.WriteBoolean(nameof(ATLoopDisconnectionDetectionFlag), __at_loop_disconnection_detection_flag[i]);
                writer.WriteNumber(nameof(ATLoopDisconnectionDetectionSetting), __at_loop_disconnection_detection_settings[i]);
                writer.WriteNumber(nameof(ATModeSelection), (ushort)__at_mode_selections[i]);
                writer.WriteBoolean(nameof(ATAutomaticBackupFlag), __at_automatic_backup_flag[i]);
                writer.WriteNumber(nameof(Ph), __ph[i]);
                writer.WriteNumber(nameof(Pc), __pc[i]);
                writer.WriteNumber(nameof(I), __i[i]);
                writer.WriteNumber(nameof(D), __d[i]);
                writer.WriteNumber(nameof(LP), __lp[i]);

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
            return writer.BytesPending - start;
        }

        protected override void _online_state_changed(bool online)
        {
            if (online == false)
            {
                __process_in_data_device = new Q64TC_PROCESS_IN_DATA_T() { channel_process_in = new CHANNNEL_PROCESS_IN_DATA_T[4] };
                __process_in_data_share = new Q64TC_PROCESS_IN_DATA_T() { channel_process_in = new CHANNNEL_PROCESS_IN_DATA_T[4] };
                __process_in_data_user_interface = new Q64TC_PROCESS_IN_DATA_T() { channel_process_in = new CHANNNEL_PROCESS_IN_DATA_T[4] };
                __process_out_data_device = new Q64TC_PROCESS_OUT_DATA_T();
                __process_out_data_share = new Q64TC_PROCESS_OUT_DATA_T();
                __process_out_data_user_interface = new Q64TC_PROCESS_OUT_DATA_T();
                IsEnabled = false;
                CommandPending = false;
                __device_sync_control = false;
            }
        }
    }

    public class Factory : ICabinet
    {
        private string __version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string Name { get => "Q64TCAutoTuning"; set => throw new NotImplementedException(); }
        public string Description { get => @"Auto Tuning Utility for Q64TCTTN Q64TCTTBWN Q64TCRTN Q64TCRTBWN"; set => throw new NotImplementedException(); }
        public string Version { get => __version; set => throw new NotImplementedException(); }

        public object CreateInstance(PropertyChangedEventHandler propertyChangedEventHandler)
        {
            Q64TCAutoTuningDataModel data = new Q64TCAutoTuningDataModel() { FriendlyName = "Q64TCAutoTuning" };
            data.UserPropertyChanged += propertyChangedEventHandler;
            return new Q64TCAutoTuningControl(data);
        }
    }
}
