using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP;
using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Master;
using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Ohestren
{
    public class DeviceNetUtilityDataModel : DataModel
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

        public ObservableCollection<string> ExceptionInfoCollection { get; private set; } = new ObservableCollection<string>();
        private DNM_PROCESS_IN_DATA_T __process_in_data_device = new DNM_PROCESS_IN_DATA_T();
        private DNM_PROCESS_IN_DATA_T __process_in_data_share = new DNM_PROCESS_IN_DATA_T();
        private DNM_PROCESS_IN_DATA_T __process_in_data_user_interface = new DNM_PROCESS_IN_DATA_T();
        private DNM_PROCESS_OUT_DATA_T __process_out_data_device = new DNM_PROCESS_OUT_DATA_T();
        private DNM_PROCESS_OUT_DATA_T __process_out_data_share = new DNM_PROCESS_OUT_DATA_T();
        private DNM_PROCESS_OUT_DATA_T __process_out_data_user_interface = new DNM_PROCESS_OUT_DATA_T();
        private Queue<ASYNC_COMMAND_T> __user_interface_buffered_messages = new Queue<ASYNC_COMMAND_T>();
        private bool __device_sync_control = false;
        private uint __device_sync_address;
        private string __device_sync_specification;
        private DN_DEVICE_MODEL_TYPE_T __device_sync_model;
        List<(string, uint, ushort)> __device_info_address_table = null;
        List<(string, uint, ushort)> __process_io_address_table = null;
        Memory<ushort>[] __process_io_data = null;

        private void __device_read_data()
        {
            lock(_synchronizer)
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
                __process_in_data_share.process_in = __process_in_data_device.process_in;
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
                __process_in_data_user_interface.process_in = __process_in_data_share.process_in;
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

        private string __device_address = "U002";
        public string DeviceAddress
        {
            get { return __device_address; }
            set
            {
                if (_MODULE_ACCESS_EXTENSION_PATTERN.IsMatch(value))
                {
                    if (__device_address != value)
                        SetProperty(ref __device_address, value);
                }
                else
                    throw new ArgumentException(@"The input string for 'Device Address' is not in a correct format.");
            }
        }
        private DN_DEVICE_MODEL_TYPE_T __model = DN_DEVICE_MODEL_TYPE_T.RJ71DN91;
        public DN_DEVICE_MODEL_TYPE_T Model
        {
            get { return __model; }
            set { if (value != __model) SetProperty(ref __model, value); }
        }

        private string __model_name;
        public string ModelName
        {
            get { return __model_name; }
            private set { if (value != __model_name) SetProperty(ref __model_name, value, false); }
        }
        private ushort __node_address;
        public ushort NodeAddress
        {
            get { return __node_address; }
            private set { if (value != __node_address) SetProperty(ref __node_address, value, false);}
        }
        private DN_DEVICE_MODE_SWITCH_T __node_mode;
        public DN_DEVICE_MODE_SWITCH_T NodeMode
        {
            get { return __node_mode; }
            private set { if (value != __node_mode) SetProperty(ref __node_mode, value, false); }
        }

        public ushort XStatus
        {
            get { return __process_in_data_user_interface.process_in.x_status; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.x_status, value, false); }
        }
        public ushort MasterCommunicationStatus
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.master_communication_status; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.master_communication_status, value, false); }
        }
        public ushort MasterErrorCode
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.master_communication_error_code; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.master_communication_error_code, value, false); }
        }
        public ushort BusErrorCounter
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.bus_error_counter; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.bus_error_counter, value, false); }
        }
        public ushort BusOffCounter
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.bus_off_counter; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.bus_off_counter, value, false); }
        }
        public ushort CurrentLinkScanTime
        {
            get { return __process_in_data_user_interface.process_in.link_scan.courrent_linkscan_time; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.link_scan.courrent_linkscan_time, value, false); }
        }
        public ushort MinimumLinkScanTime
        {
            get { return __process_in_data_user_interface.process_in.link_scan.minimum_linkscan_time; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.link_scan.minimum_linkscan_time, value, false); }
        }
        public ushort MaximumLinkScanTime
        {
            get { return __process_in_data_user_interface.process_in.link_scan.maximum_linkscan_time; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.link_scan.maximum_linkscan_time, value, false); }
        }
        public ushort NodeConfigurationStatus0015
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_configuration_status_0; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_configuration_status_0, value, false); }
        }
        public ushort NodeConfigurationStatus1631
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_configuration_status_1; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_configuration_status_1, value, false); }
        }
        public ushort NodeConfigurationStatus3247
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_configuration_status_2; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_configuration_status_2, value, false); }
        }
        public ushort NodeConfigurationStatus4863
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_configuration_status_3; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_configuration_status_3, value, false); }
        }
        public ushort NodeIOCommunicationStatus0015
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_io_communication_status_0; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_io_communication_status_0, value, false); }
        }
        public ushort NodeIOCommunicationStatus1631
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_io_communication_status_1; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_io_communication_status_1, value, false); }
        }
        public ushort NodeIOCommunicationStatus3247
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_io_communication_status_2; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_io_communication_status_2, value, false); }
        }
        public ushort NodeIOCommunicationStatus4863
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_io_communication_status_3; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_io_communication_status_3, value, false); }
        }
        public ushort NodeIOErrorStatus0015
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_io_error_status_0; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_io_error_status_0, value, false); }
        }
        public ushort NodeIOErrorStatus1631
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_io_error_status_1; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_io_error_status_1, value, false); }
        }
        public ushort NodeIOErrorStatus3247
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_io_error_status_2; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_io_error_status_2, value, false); }
        }
        public ushort NodeIOErrorStatus4863
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_io_error_status_3; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_io_error_status_3, value, false); }
        }
        public ushort NodeDiagnosticInfoStatus0015
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_diagnostic_info_status_0; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_diagnostic_info_status_0, value, false); }
        }
        public ushort NodeDiagnosticInfoStatus1631
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_diagnostic_info_status_1; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_diagnostic_info_status_1, value, false); }
        }
        public ushort NodeDiagnosticInfoStatus3247
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_diagnostic_info_status_2; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_diagnostic_info_status_2, value, false); }
        }
        public ushort NodeDiagnosticInfoStatus4863
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic.node_diagnostic_info_status_3; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic.node_diagnostic_info_status_3, value, false); }
        }

        public uint MasterControlTimeout { get; set; } = 5000;
        public uint ExplicitMessageProcessTimeout { get; set; } = 10000;

        private ushort __explicit_message_execution_error_code = 0;
        public ushort ExplicitMessageExecutionErrorCode
        {
            get { return __explicit_message_execution_error_code; }
            private set { SetProperty(ref __explicit_message_execution_error_code, value, false); }
        }
        private byte __explicit_message_slave_node_address = 1;
        public byte ExplicitMessageSlaveNodeAddress
        {
            get { return __explicit_message_slave_node_address; }
            set { SetProperty(ref __explicit_message_slave_node_address, value, false); }
        }
        private byte __explicit_message_class_id = 48;
        public byte ExplicitMessageClassID
        {
            get { return __explicit_message_class_id; }
            set { SetProperty(ref __explicit_message_class_id, value, false); }
        }
        private ushort __explicit_message_instance_id = 1;
        public ushort ExplicitMessageInstanceID
        {
            get { return __explicit_message_instance_id; }
            set { SetProperty(ref __explicit_message_instance_id, value, false); }
        }
        private byte __explicit_message_attribute_id = 3;
        public byte ExplicitMessageAttributeID
        {
            get { return __explicit_message_attribute_id; }
            set { SetProperty(ref __explicit_message_attribute_id, value, false); }
        }
        private byte __explicit_message_size_in_byte;
        public byte ExplicitMessageSizeInByte
        {
            get { return __explicit_message_size_in_byte; }
            private set { SetProperty(ref __explicit_message_size_in_byte, value, false); }
        }
        private byte[] __explicit_message_data;
        public byte[] ExplicitMessageData
        {
            get { return __explicit_message_data; }
            set 
            {
                if (__device_sync_model == DN_DEVICE_MODEL_TYPE_T.RJ71DN91 && value != null && value.Length > (int)RJ71DN91_ADDRESS_TABLE.MESSAGE_DATA_SIZE_IN_BYTE)
                    throw new ArgumentException($"The message data length is out of range({(int)RJ71DN91_ADDRESS_TABLE.MESSAGE_DATA_SIZE_IN_BYTE}) bytes.");
                SetProperty(ref __explicit_message_data, value, false); 
            }
        }

        private ushort __slave_node_status;
        public ushort SlaveNodeStatus
        {
            get { return __slave_node_status; }
            private set { SetProperty(ref __slave_node_status, value, false); }
        }
        private ushort __slave_message_communication_error_code;
        public ushort SlaveMessageCommunicationErrorCode
        {
            get { return __slave_message_communication_error_code; }
            private set { SetProperty(ref __slave_message_communication_error_code, value, false); }
        }
        private ushort __slave_general_dnet_error_code;
        public ushort SlaveGeneralDNetErrorCode
        {
            get { return __slave_general_dnet_error_code; }
            private set { SetProperty(ref __slave_general_dnet_error_code, value, false); }
        }
        private ushort __slave_additionnal_dnet_error_code;
        public ushort SlaveAdditionalDNetErrorCode
        {
            get { return __slave_additionnal_dnet_error_code; }
            private set { SetProperty(ref __slave_additionnal_dnet_error_code, value, false); }
        }
        private ushort __number_of_slave_down;
        public ushort NumberOfSlaveDown
        {
            get { return __number_of_slave_down; }
            private set { SetProperty(ref __number_of_slave_down, value, false); }
        }

        private byte __explicit_message_command_number;
        public byte ExplicitMessageCommandNumber
        {
            get { return __explicit_message_command_number; }
            set { SetProperty(ref __explicit_message_command_number, value, false); }
        }

        public byte SlaveProducedInstance { get; set; }
        public byte SlaveConsumedInstance { get; set; }

        public bool Enable()
        {
            if (CommandPending)
                return false;

            __process_out_data_user_interface.control_out = new ENABLE_COMMAND_T() { device_address = DeviceAddress, model = Model };
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

        public bool StartIOCommunication()
        {
            if (CommandPending)
                return false;

            __process_out_data_user_interface.control_out = new START_IO_COM_COMMAND_T() { timeout = MasterControlTimeout, start_ticks = Environment.TickCount };
            CommandPending = true;
            return true;
        }

        public bool StopIOCommunication()
        {
            if (CommandPending)
                return false;

            __process_out_data_user_interface.control_out = new STOP_IO_COM_COMMAND_T() { timeout = MasterControlTimeout, start_ticks = Environment.TickCount };
            CommandPending = true;
            return true;
        }

        public bool ResetError()
        {
            if (CommandPending)
                return false;

            __process_out_data_user_interface.control_out = new RESET_ERROR_COMMAND_T() { timeout = MasterControlTimeout, start_ticks = Environment.TickCount };
            CommandPending = true;
            return true;
        }

        public bool ReadSlaveAttribute()
        {
            if (CommandPending)
                return false;

            if (__device_sync_model == DN_DEVICE_MODEL_TYPE_T.RJ71DN91)
            {
                __process_out_data_user_interface.control_out = new EXECUTE_EXPLICIT_MESSAGE_COMMAND_T()
                {
                    timeout = ExplicitMessageProcessTimeout,
                    start_ticks = Environment.TickCount,
                    header = new EXPLICIT_MESSAGE_REQUEST_HEADER_T()
                    {
                        command_number = EXPLICIT_MESSAGE_COMMAND_NUMBER_T.R_READ_ATTRIBUTE,
                        slave_node_address = ExplicitMessageSlaveNodeAddress,
                        object_class_id = ExplicitMessageClassID,
                        object_instance_id = ExplicitMessageInstanceID,
                        object_attribute_id = ExplicitMessageAttributeID,
                        command_data_length = 0
                    }
                };
            }
            else
                return false;
            CommandPending = true;
            return true;
        }

        public bool WriteSlaveAttribute()
        {
            if (CommandPending)
                return false;

            byte[] byteArray = null;
            if (ExplicitMessageData != null)
            {
                if (ExplicitMessageData.Length % 2 == 0)
                    byteArray = ExplicitMessageData;
                else
                {
                    byteArray = new byte[ExplicitMessageData.Length + 1];
                    Buffer.BlockCopy(ExplicitMessageData, 0, byteArray, 0, ExplicitMessageData.Length);
                }
            }
            if (__device_sync_model == DN_DEVICE_MODEL_TYPE_T.RJ71DN91)
            {
                __process_out_data_user_interface.control_out = new EXECUTE_EXPLICIT_MESSAGE_COMMAND_T()
                {
                    timeout = ExplicitMessageProcessTimeout,
                    start_ticks = Environment.TickCount,
                    header = new EXPLICIT_MESSAGE_REQUEST_HEADER_T()
                    {
                        command_number = EXPLICIT_MESSAGE_COMMAND_NUMBER_T.R_WRITE_ATTRIBUTE,
                        slave_node_address = ExplicitMessageSlaveNodeAddress,
                        object_class_id = ExplicitMessageClassID,
                        object_instance_id = ExplicitMessageInstanceID,
                        object_attribute_id = ExplicitMessageAttributeID,
                        command_data_length = (byte)(ExplicitMessageData == null ? 0 : ExplicitMessageData.Length)
                    },
                    data = new ReadOnlyMemory<byte>(byteArray)
                };
            }
            else
                return false;
            CommandPending = true;
            return true;
        }

        public bool ReadSlaveDiagnosticInfo()
        {
            if (CommandPending)
                return false;

            if (__device_sync_model == DN_DEVICE_MODEL_TYPE_T.RJ71DN91)
            {
                __process_out_data_user_interface.control_out = new EXECUTE_EXPLICIT_MESSAGE_COMMAND_T()
                {
                    timeout = ExplicitMessageProcessTimeout,
                    start_ticks = Environment.TickCount,
                    header = new EXPLICIT_MESSAGE_REQUEST_HEADER_T()
                    {
                        command_number = EXPLICIT_MESSAGE_COMMAND_NUMBER_T.R_READ_DIAGNOSTIC_INFO,
                        slave_node_address = ExplicitMessageSlaveNodeAddress,
                    },
                };
            }
            else
                return false;
            CommandPending = true;
            return true;
        }

        public bool PostCustomizedMessage()
        {
            if (CommandPending)
                return false;

            byte[] byteArray = null;
            if (ExplicitMessageData != null)
            {
                if (ExplicitMessageData.Length % 2 == 0)
                    byteArray = ExplicitMessageData;
                else
                {
                    byteArray = new byte[ExplicitMessageData.Length + 1];
                    Buffer.BlockCopy(ExplicitMessageData, 0, byteArray, 0, ExplicitMessageData.Length);
                }
            }
            if (__device_sync_model == DN_DEVICE_MODEL_TYPE_T.RJ71DN91)
            {
                __process_out_data_user_interface.control_out = new EXECUTE_EXPLICIT_MESSAGE_COMMAND_T()
                {
                    timeout = ExplicitMessageProcessTimeout,
                    start_ticks = Environment.TickCount,
                    header = new EXPLICIT_MESSAGE_REQUEST_HEADER_T()
                    {
                        command_number = EXPLICIT_MESSAGE_COMMAND_NUMBER_T.R_OTHER + ExplicitMessageCommandNumber,
                        slave_node_address = ExplicitMessageSlaveNodeAddress,
                        object_class_id = ExplicitMessageClassID,
                        object_instance_id = ExplicitMessageInstanceID,
                        object_attribute_id = ExplicitMessageAttributeID,
                        command_data_length = (byte)(ExplicitMessageData == null ? 0 : ExplicitMessageData.Length)
                    },
                    data = new ReadOnlyMemory<byte>(byteArray)
                };
            }
            else
                return false;
            CommandPending = true;
            return true;
        }

        public bool SetPollingAssemblyPath()
        {
            if (CommandPending)
                return false;

            if (__device_sync_model == DN_DEVICE_MODEL_TYPE_T.RJ71DN91)
            {
                __process_out_data_user_interface.control_out = new EXECUTE_EXPLICIT_MESSAGE_COMMAND_T()
                {
                    timeout = ExplicitMessageProcessTimeout,
                    start_ticks = Environment.TickCount,
                    header = new EXPLICIT_MESSAGE_REQUEST_HEADER_T()
                    {
                        command_number = EXPLICIT_MESSAGE_COMMAND_NUMBER_T.R_OTHER + 0x4B,
                        slave_node_address = ExplicitMessageSlaveNodeAddress,
                        object_class_id = 3,
                        object_instance_id = 1,
                        object_attribute_id = 2,
                        command_data_length = 1
                    },
                    data = new byte[2] { (byte)NodeAddress, 0 }
                };

                __user_interface_buffered_messages.Enqueue(new EXECUTE_EXPLICIT_MESSAGE_COMMAND_T()
                {
                    timeout = ExplicitMessageProcessTimeout,
                    start_ticks = Environment.TickCount,
                    header = new EXPLICIT_MESSAGE_REQUEST_HEADER_T()
                    {
                        command_number = EXPLICIT_MESSAGE_COMMAND_NUMBER_T.R_WRITE_ATTRIBUTE,
                        slave_node_address = ExplicitMessageSlaveNodeAddress,
                        object_class_id = 5,
                        object_instance_id = 2,
                        object_attribute_id = 14,
                        command_data_length = 6
                    },
                    data = new byte[6] { 0x20, 0x04, 0x24, SlaveProducedInstance, 0x30, 0x03 }
                });

                __user_interface_buffered_messages.Enqueue(new EXECUTE_EXPLICIT_MESSAGE_COMMAND_T()
                {
                    timeout = ExplicitMessageProcessTimeout,
                    start_ticks = Environment.TickCount,
                    header = new EXPLICIT_MESSAGE_REQUEST_HEADER_T()
                    {
                        command_number = EXPLICIT_MESSAGE_COMMAND_NUMBER_T.R_WRITE_ATTRIBUTE,
                        slave_node_address = ExplicitMessageSlaveNodeAddress,
                        object_class_id = 5,
                        object_instance_id = 2,
                        object_attribute_id = 16,
                        command_data_length = 6
                    },
                    data = new byte[6] { 0x20, 0x04, 0x24, SlaveConsumedInstance, 0x30, 0x03 }
                });

                __user_interface_buffered_messages.Enqueue(new EXECUTE_EXPLICIT_MESSAGE_COMMAND_T()
                {
                    timeout = ExplicitMessageProcessTimeout,
                    start_ticks = Environment.TickCount,
                    header = new EXPLICIT_MESSAGE_REQUEST_HEADER_T()
                    {
                        command_number = EXPLICIT_MESSAGE_COMMAND_NUMBER_T.R_OTHER + 0x4C,
                        slave_node_address = ExplicitMessageSlaveNodeAddress,
                        object_class_id = 3,
                        object_instance_id = 1,
                        object_attribute_id = 2,
                        command_data_length = 1
                    },
                    data = new byte[2] { (byte)NodeAddress, 0 }
                });
            }
            else
                return false;
            CommandPending = true;
            return true;
        }

        public override void ExchangeDataWithDevice(DeviceAccessMaster master, ushort monitoring)
        {
            __device_read_data();
            Memory<ushort>[] data;
            ushort end = 0;
            ushort[] temporaryUSHORT = new ushort[1];
            byte[] temporaryBYTE = new byte[1];
            var res = __process_out_data_device.control_out;
            switch (res)
            {
                case ENABLE_COMMAND_T enable:
                    __device_sync_address = Convert.ToUInt32(enable.device_address[1..], 16) * 16;
                    __device_sync_specification = enable.device_address;
                    __device_sync_model = enable.model;
                    try
                    {
                        if (enable.model == DN_DEVICE_MODEL_TYPE_T.RJ71DN91)
                        {
                            __device_info_address_table = new List<(string, uint, ushort)>()
                            {
                                (enable.device_address, (uint)RJ71DN91_ADDRESS_TABLE.BUF_MODEL_DISPLAY01, 5),
                                (enable.device_address, (uint)RJ71DN91_ADDRESS_TABLE.BUF_NODE_ADDRESS, 1),
                                (enable.device_address, (uint)RJ71DN91_ADDRESS_TABLE.BUF_MODE_SWITCH_NUMBER, 1),
                            };
                            __process_io_address_table = new List<(string, uint, ushort)>()
                            {
                                (enable.device_address, (uint)RJ71DN91_ADDRESS_TABLE.BUF_MASTER_FUNCTION_COMMUNICATION_STATUS,
                                (ushort)(Marshal.SizeOf<DNM_PROCESS_IN_DATA_T.PROCESS_IN_T.MASTER_NODE_DIAGNOSTIC_INFO_T>()/2)),
                                (enable.device_address, (uint)RJ71DN91_ADDRESS_TABLE.BUF_CURRENT_LINK_SCAN_TIME,
                                (ushort)(Marshal.SizeOf<DNM_PROCESS_IN_DATA_T.PROCESS_IN_T.LINK_SCAN_TIME_T>()/2)),
                            };
                            __process_io_data = new Memory<ushort>[2]
                            {
                                new ushort[Marshal.SizeOf<DNM_PROCESS_IN_DATA_T.PROCESS_IN_T.MASTER_NODE_DIAGNOSTIC_INFO_T>()/2],
                                new ushort[Marshal.SizeOf<DNM_PROCESS_IN_DATA_T.PROCESS_IN_T.LINK_SCAN_TIME_T>()/2]
                            };
                        }
                        data = new Memory<ushort>[3] { new ushort[5], new ushort[1], new ushort[1] };
                        master.ReadModuleAccessDeviceInWord(monitoring, __device_info_address_table, out end, data);
                        if (end != 0)
                            __process_in_data_device.control_in = new ENABLE_RESULT_T() { end_code = end, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR };
                        else
                            __process_in_data_device.control_in = new ENABLE_RESULT_T()
                            {
                                end_code = end,
                                exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                model_name = Encoding.ASCII.GetString(MemoryMarshal.AsBytes(data[0].Span)),
                                node_address = data[1].Span[0],
                                mode_switch = data[2].Span[0]
                            };
                        __process_out_data_device.control_out = null;
                        __device_sync_control = true;
                    }
                    catch (SLMPException ex)
                    {
                        __process_in_data_device.control_in = new ENABLE_RESULT_T() { end_code = 0, exception_code = ex.ExceptionCode };
                        __process_out_data_device.control_out = null;
                        __device_sync_control = false;
                        if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                            throw;
                    }
                    break;
                case DISABLE_COMMAND_T:
                    __process_in_data_device.control_in = new DISABLE_RESULT_T() { end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR };
                    __process_out_data_device.control_out = null;
                    __device_sync_control = false;
                    break;
                case EXECUTE_EXPLICIT_MESSAGE_COMMAND_T msg:
                    try
                    {
                        if (__device_sync_model == DN_DEVICE_MODEL_TYPE_T.RJ71DN91)
                        {
                            switch (msg.step)
                            {
                                case 0:
                                    master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)RJ71DN91_ADDRESS_TABLE.Y_MESSAGE_COMMUNICATION_REQUEST, 1, out end, new byte[] { 0 });
                                    if (end == 0) msg.step++;
                                    break;
                                case 1:
                                    master.ReadLocalDeviceInBit(monitoring, "X", __device_sync_address + (uint)RJ71DN91_ADDRESS_TABLE.X_MESSAGE_COMMUNICATION_COMPLETION, 1, out end, temporaryBYTE);
                                    if (end == 0 && temporaryBYTE[0] == 0)
                                        master.ReadLocalDeviceInBit(monitoring, "X", __device_sync_address + (uint)RJ71DN91_ADDRESS_TABLE.X_MESSAGE_COMMUNICATION_ERROR, 1, out end, temporaryBYTE);
                                    if (end == 0 && temporaryBYTE[0] == 0) msg.step++;
                                    break;
                                case 2:
                                    var requsetheaderbytes = new ushort[Marshal.SizeOf<EXPLICIT_MESSAGE_REQUEST_HEADER_T>() / 2];
                                    var requestheaderdata = msg.header;
                                    MemoryMarshal.Write(MemoryMarshal.Cast<ushort, byte>(requsetheaderbytes), ref requestheaderdata);
                                    List<(string, uint, ushort, ReadOnlyMemory<ushort>)> requestdeviceblocks;
                                    if (msg.data.IsEmpty)
                                        requestdeviceblocks = new List<(string, uint, ushort, ReadOnlyMemory<ushort>)>()
                                        {
                                            (__device_sync_specification, (uint)RJ71DN91_ADDRESS_TABLE.BUF_MESSAGE_COMMUNICATION_COMMAND_HEAD,
                                                (ushort)(Marshal.SizeOf<EXPLICIT_MESSAGE_REQUEST_HEADER_T>()/2), requsetheaderbytes),
                                        };
                                    else
                                    {
                                        Debug.Assert(msg.data.Length % 2 == 0);
                                        requestdeviceblocks = new List<(string, uint, ushort, ReadOnlyMemory<ushort>)>()
                                        {
                                            (__device_sync_specification, (uint)RJ71DN91_ADDRESS_TABLE.BUF_MESSAGE_COMMUNICATION_COMMAND_HEAD,
                                                (ushort)(Marshal.SizeOf<EXPLICIT_MESSAGE_REQUEST_HEADER_T>()/2), requsetheaderbytes),
                                            (__device_sync_specification, (uint)RJ71DN91_ADDRESS_TABLE.BUF_MESSAGE_COMMUNICATION_DATA,
                                                (ushort)(msg.data.Length/2), MemoryMarshal.Cast<byte, ushort>(msg.data.Span).ToArray())
                                        };
                                    }
                                    master.WriteModuleAccessDeviceInWord(monitoring, requestdeviceblocks, out end);
                                    if (end == 0)
                                        master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)RJ71DN91_ADDRESS_TABLE.Y_MESSAGE_COMMUNICATION_REQUEST, 1, out end, new byte[] { 1 });
                                    if (end == 0) msg.step++;
                                    break;
                                case 3:
                                    master.ReadLocalDeviceInBit(monitoring, "X", __device_sync_address + (uint)RJ71DN91_ADDRESS_TABLE.X_MESSAGE_COMMUNICATION_COMPLETION, 1, out end, temporaryBYTE);
                                    if (end == 0 && temporaryBYTE[0] == 1)
                                    {
                                        List<(string, uint, ushort)> reponsedeviceblocks = new List<(string, uint, ushort)>()
                                                {
                                                    (__device_sync_specification, (uint)RJ71DN91_ADDRESS_TABLE.BUF_MESSAGE_COMMUNICATION_RESULT_HEAD,
                                                        (ushort)(Marshal.SizeOf<EXPLICIT_MESSAGE_RESULT_HEADER_T>()/2)),
                                                    (__device_sync_specification, (uint)RJ71DN91_ADDRESS_TABLE.BUF_MESSAGE_COMMUNICATION_DATA,
                                                        (ushort)(uint)RJ71DN91_ADDRESS_TABLE.MESSAGE_DATA_SIZE_IN_BYTE/2),
                                                };
                                        Memory<ushort>[] result = new Memory<ushort>[2]
                                        {
                                                    new ushort[Marshal.SizeOf<EXPLICIT_MESSAGE_RESULT_HEADER_T>()/2],
                                                    new ushort[(uint)RJ71DN91_ADDRESS_TABLE.MESSAGE_DATA_SIZE_IN_BYTE/2]
                                        };
                                        master.ReadModuleAccessDeviceInWord(monitoring, reponsedeviceblocks, out end, result);
                                        if (end == 0)
                                        {
                                            byte[] resultmsgdata = new byte[(uint)RJ71DN91_ADDRESS_TABLE.MESSAGE_DATA_SIZE_IN_BYTE];
                                            MemoryMarshal.AsBytes(result[1].Span).CopyTo(resultmsgdata);
                                            __process_in_data_device.control_in = new EXECUTE_EXPLICIT_MESSAGE_RESULT_T()
                                            {
                                                end_code = 0,
                                                exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                                timeout = false,
                                                header = MemoryMarshal.Read<EXPLICIT_MESSAGE_RESULT_HEADER_T>(MemoryMarshal.Cast<ushort, byte>(result[0].Span)),
                                                data = resultmsgdata
                                            };
                                            __process_out_data_device.control_out = null;
                                        }
                                    }
                                    break;
                            }
                        }
                        if (__process_out_data_device.control_out != null)
                        {
                            if (end != 0)
                            {
                                __process_in_data_device.control_in = new EXECUTE_EXPLICIT_MESSAGE_RESULT_T() 
                                { 
                                    end_code = end, 
                                    exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                    timeout = false,
                                    header = new EXPLICIT_MESSAGE_RESULT_HEADER_T()
                                    {
                                         command_number = msg.header.command_number
                                    }
                                };
                                __process_out_data_device.control_out = null;
                            }
                            else if (Math.Abs(Environment.TickCount - msg.start_ticks) > msg.timeout)
                            {
                                __process_in_data_device.control_in = new EXECUTE_EXPLICIT_MESSAGE_RESULT_T() 
                                { 
                                    end_code = 0, 
                                    exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, 
                                    timeout = true,
                                    header = new EXPLICIT_MESSAGE_RESULT_HEADER_T()
                                    {
                                        command_number = msg.header.command_number
                                    }
                                };
                                __process_out_data_device.control_out = null;
                            }
                        }
                    }
                    catch (SLMPException ex)
                    {
                        __process_in_data_device.control_in = new EXECUTE_EXPLICIT_MESSAGE_RESULT_T() 
                        { 
                            end_code = 0, 
                            exception_code = ex.ExceptionCode,
                            timeout = false,
                            header = new EXPLICIT_MESSAGE_RESULT_HEADER_T()
                            {
                                command_number = msg.header.command_number
                            }
                        };
                        __process_out_data_device.control_out = null;

                        if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                            throw;
                    }
                    finally
                    {
                        if (__device_sync_model == DN_DEVICE_MODEL_TYPE_T.RJ71DN91)
                        {
                            if (__process_out_data_device.control_out == null && msg.step != 0)
                                master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)RJ71DN91_ADDRESS_TABLE.Y_MESSAGE_COMMUNICATION_REQUEST, 1, out end, new byte[] { 0 });
                        }
                    }
                    break;
                case BLOCKING_COMMAND_T block:
                    try
                    {
                        if (__device_sync_model == DN_DEVICE_MODEL_TYPE_T.RJ71DN91)
                        {
                            switch (block.cmd)
                            {
                                case ASYNC_COMMAND_CODE_T.RESET_ERROR:
                                    switch (block.step)
                                    {
                                        case 0:
                                            master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)RJ71DN91_ADDRESS_TABLE.Y_MASTER_FUNCTION_ERROR_RESET_REQUEST, 1, out end, new byte[] { 1 });
                                            if (end == 0) block.step++;
                                            break;
                                        case 1:
                                            master.ReadModuleAccessDeviceInWord(monitoring, __device_sync_specification, (uint)RJ71DN91_ADDRESS_TABLE.BUF_MASTER_FUNCTION_ERROR_INFO, 1, out end, temporaryUSHORT);
                                            if (end == 0 && temporaryUSHORT[0] == 0)
                                            {
                                                __process_in_data_device.control_in = new BLOCKING_RESULT() { cmd = block.cmd, end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, timeout = false };
                                                __process_out_data_device.control_out = null;
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case ASYNC_COMMAND_CODE_T.START_IO_COMMUNICATION:
                                    switch (block.step)
                                    {
                                        case 0:
                                            //master.WriteModuleAccessDeviceInWord(monitoring, __device_sync_specification, (uint)RJ71DN91_ADDRESS_TABLE.BUF_DATA_CONSISTENCY_SETTING, 1, out end, new ushort[] { 1 });
                                            //if(end == 0)
                                            master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)RJ71DN91_ADDRESS_TABLE.Y_IO_COMMUNICATION_REQUEST, 1, out end, new byte[] { 1 });
                                            if (end == 0) block.step++;
                                            break;
                                        case 1:
                                            master.ReadLocalDeviceInBit(monitoring, "X",  __device_sync_address + (uint)RJ71DN91_ADDRESS_TABLE.X_IO_COMMUNICATING, 1, out end, temporaryBYTE);
                                            if (end == 0 && temporaryBYTE[0] == 1)
                                            {
                                                __process_in_data_device.control_in = new BLOCKING_RESULT() { cmd = block.cmd, end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, timeout = false };
                                                __process_out_data_device.control_out = null;
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case ASYNC_COMMAND_CODE_T.STOP_IO_COMMUNICATION:
                                    switch (block.step)
                                    {
                                        case 0:
                                            //master.WriteModuleAccessDeviceInWord(monitoring, __device_sync_specification, (uint)RJ71DN91_ADDRESS_TABLE.BUF_DATA_CONSISTENCY_SETTING, 1, out end, new ushort[] { 1 });
                                            //if(end == 0)
                                            master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)RJ71DN91_ADDRESS_TABLE.Y_IO_COMMUNICATION_REQUEST, 1, out end, new byte[] { 0 });
                                            if (end == 0) block.step++;
                                            break;
                                        case 1:
                                            master.ReadLocalDeviceInBit(monitoring, "X", __device_sync_address + (uint)RJ71DN91_ADDRESS_TABLE.X_IO_COMMUNICATING, 1, out end, temporaryBYTE);
                                            if (end == 0 && temporaryBYTE[0] == 0)
                                            {
                                                __process_in_data_device.control_in = new BLOCKING_RESULT() { cmd = block.cmd, end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, timeout = false };
                                                __process_out_data_device.control_out = null;
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                default:
                                    break;
                            }
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
                        if (__device_sync_model == DN_DEVICE_MODEL_TYPE_T.RJ71DN91)
                        {
                            if (__process_out_data_device.control_out == null)
                            {
                                switch (block.cmd)
                                {
                                    case ASYNC_COMMAND_CODE_T.RESET_ERROR:
                                        if(block.step != 0)
                                            master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)RJ71DN91_ADDRESS_TABLE.Y_MASTER_FUNCTION_ERROR_RESET_REQUEST, 1, out _, new byte[] { 0 });
                                        break;
                                    case ASYNC_COMMAND_CODE_T.START_IO_COMMUNICATION:
                                        BLOCKING_RESULT blk = __process_in_data_device.control_in as BLOCKING_RESULT;
                                        if (blk.end_code != 0 || blk.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR || blk.timeout == true)
                                            master.WriteLocalDeviceInBit(monitoring, "Y", __device_sync_address + (uint)RJ71DN91_ADDRESS_TABLE.Y_IO_COMMUNICATION_REQUEST, 1, out end, new byte[] { 0 });
                                        break;
                                }
                            }
                        }
                    }
                    break;
            }

            if (__device_sync_control)
            {
                try
                {
                    if (__device_sync_model == DN_DEVICE_MODEL_TYPE_T.RJ71DN91)
                    {
                        master.ReadLocalDeviceInWord(monitoring, "X", __device_sync_address, 1, out __process_in_data_device.process_in.end_code, temporaryUSHORT);
                        if(__process_in_data_device.process_in.end_code == 0)
                        {
                            __process_in_data_device.process_in.x_status = temporaryUSHORT[0];
                            master.ReadModuleAccessDeviceInWord(monitoring, __process_io_address_table, out __process_in_data_device.process_in.end_code, __process_io_data);
                            if (__process_in_data_device.process_in.end_code == 0)
                            {
                                __process_in_data_device.process_in.master_diagnostic = MemoryMarshal.Read<DNM_PROCESS_IN_DATA_T.PROCESS_IN_T.MASTER_NODE_DIAGNOSTIC_INFO_T>(
                                    MemoryMarshal.AsBytes(__process_io_data[0].Span));
                                __process_in_data_device.process_in.link_scan = MemoryMarshal.Read<DNM_PROCESS_IN_DATA_T.PROCESS_IN_T.LINK_SCAN_TIME_T>(
                                    MemoryMarshal.AsBytes(__process_io_data[1].Span));
                            }
                        }
                        __process_in_data_device.process_in.exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR;
                    }
                }
                catch (SLMPException ex)
                {
                    __process_in_data_device.process_in.end_code = 0;
                    __process_in_data_device.process_in.exception_code = ex.ExceptionCode;
                    __device_sync_control = false;
                    if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                        throw;
                }
            }

            __device_write_data();
        }

        public override void ExchangeDataWihtUserInterface()
        {
            __user_read_data();
            var res = __process_in_data_user_interface.control_in;
            if (res != null)
            {
                switch (res)
                {
                    case ENABLE_RESULT_T start:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"{res.cmd}", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"{res.cmd}", res.end_code);
                        else
                        {
                            IsEnabled = true;
                            ModelName = start.model_name;
                            NodeAddress = start.node_address;
                            NodeMode = Enum.IsDefined(typeof(DN_DEVICE_MODE_SWITCH_T), start.mode_switch) ?
                                    (DN_DEVICE_MODE_SWITCH_T)Enum.ToObject(typeof(DN_DEVICE_MODE_SWITCH_T), start.mode_switch) :
                                    DN_DEVICE_MODE_SWITCH_T.UNKNOWN_SWITCH;
                        }
                        break;
                    case DISABLE_RESULT_T:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"{res.cmd}", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"{res.cmd}", res.end_code);
                        else
                            IsEnabled = false;
                        break;
                    case EXECUTE_EXPLICIT_MESSAGE_RESULT_T msg:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"{res.cmd}({msg.header.command_number})", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"{res.cmd}({msg.header.command_number})", res.end_code);
                        else if (msg.timeout == true)
                        {
                            __add_exception_info($"{msg.cmd}({msg.header.command_number})", "Operation Timout");
                            __user_interface_buffered_messages.Clear();
                        }
                        else
                        {
                            ExplicitMessageExecutionErrorCode = msg.header.execution_error_code;
                            if (msg.header.execution_error_code != 0)
                                 __user_interface_buffered_messages.Clear();
                            else if (__user_interface_buffered_messages.Count == 0)
                            {
                                ExplicitMessageSlaveNodeAddress = msg.header.slave_node_address;
                                switch (msg.header.command_number)
                                {
                                    case EXPLICIT_MESSAGE_COMMAND_NUMBER_T.R_READ_ATTRIBUTE:
                                    case EXPLICIT_MESSAGE_COMMAND_NUMBER_T.R_WRITE_ATTRIBUTE:
                                        ExplicitMessageClassID = msg.header.object_class_id;
                                        ExplicitMessageInstanceID = msg.header.object_instance_id;
                                        ExplicitMessageAttributeID = msg.header.object_attribute_id;
                                        ExplicitMessageSizeInByte = msg.header.command_data_length;
                                        if (msg.header.command_data_length != 0)
                                            ExplicitMessageData = msg.data[0..msg.header.command_data_length].ToArray();
                                        else
                                            ExplicitMessageData = null;
                                        break;
                                    case EXPLICIT_MESSAGE_COMMAND_NUMBER_T.R_READ_DIAGNOSTIC_INFO:
                                        SLAVE_NODE_DIAGNOSTIC_INFO_T info = MemoryMarshal.Read<SLAVE_NODE_DIAGNOSTIC_INFO_T>(msg.data.Span);
                                        SlaveNodeStatus = info.slave_status;
                                        SlaveMessageCommunicationErrorCode = info.message_communication_error;
                                        SlaveGeneralDNetErrorCode = info.general_dnet_error_code;
                                        SlaveAdditionalDNetErrorCode = info.additional_error_code;
                                        NumberOfSlaveDown = info.number_of_heartbeat_timeout;
                                        break;
                                    default:
                                        if ((msg.header.command_number & EXPLICIT_MESSAGE_COMMAND_NUMBER_T.R_OTHER) == EXPLICIT_MESSAGE_COMMAND_NUMBER_T.R_OTHER)
                                        {
                                            ExplicitMessageCommandNumber = (byte)((byte)msg.header.command_number & 0xFF);
                                            ExplicitMessageClassID = msg.header.object_class_id;
                                            ExplicitMessageInstanceID = msg.header.object_instance_id;
                                            ExplicitMessageAttributeID = msg.header.object_attribute_id;
                                            ExplicitMessageSizeInByte = msg.header.command_data_length;
                                            if (msg.header.command_data_length != 0)
                                                ExplicitMessageData = msg.data[0..msg.header.command_data_length].ToArray();
                                            else
                                                ExplicitMessageData = null;
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    case BLOCKING_RESULT blocking:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"{res.cmd}", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"{res.cmd}", res.end_code);
                        else if (blocking.timeout == true)
                            __add_exception_info($"{blocking.cmd}", "Operation Timout");
                        break;
                }
                __process_in_data_user_interface.control_in = null;
                if(__user_interface_buffered_messages.TryDequeue(out __process_out_data_user_interface.control_out) == false)
                    CommandPending = false;
            }

            if (IsEnabled)
            {
                if (__process_in_data_user_interface.process_in.end_code != 0)
                {
                    __add_exception_info("Reading ProcessIO", __process_in_data_user_interface.process_in.end_code);
                    IsEnabled = false;
                }
                else if (__process_in_data_user_interface.process_in.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                {
                    __add_exception_info("Reading ProcessIO", __process_in_data_user_interface.process_in.exception_code);
                    IsEnabled = false;
                }
                else
                {
                    XStatus = __process_in_data_user_interface.process_in.x_status;
                    MasterCommunicationStatus = __process_in_data_user_interface.process_in.master_diagnostic.master_communication_status;
                    MasterErrorCode = __process_in_data_user_interface.process_in.master_diagnostic.master_communication_error_code;
                    BusErrorCounter = __process_in_data_user_interface.process_in.master_diagnostic.bus_error_counter;
                    BusOffCounter = __process_in_data_user_interface.process_in.master_diagnostic.bus_off_counter;
                    CurrentLinkScanTime = __process_in_data_user_interface.process_in.link_scan.courrent_linkscan_time;
                    MinimumLinkScanTime = __process_in_data_user_interface.process_in.link_scan.minimum_linkscan_time;
                    MaximumLinkScanTime = __process_in_data_user_interface.process_in.link_scan.maximum_linkscan_time;
                    NodeConfigurationStatus0015 = __process_in_data_user_interface.process_in.master_diagnostic.node_configuration_status_0;
                    NodeConfigurationStatus1631 = __process_in_data_user_interface.process_in.master_diagnostic.node_configuration_status_1;
                    NodeConfigurationStatus3247 = __process_in_data_user_interface.process_in.master_diagnostic.node_configuration_status_2;
                    NodeConfigurationStatus4863 = __process_in_data_user_interface.process_in.master_diagnostic.node_configuration_status_3;
                    NodeIOCommunicationStatus0015 = __process_in_data_user_interface.process_in.master_diagnostic.node_io_communication_status_0;
                    NodeIOCommunicationStatus1631 = __process_in_data_user_interface.process_in.master_diagnostic.node_io_communication_status_1;
                    NodeIOCommunicationStatus3247 = __process_in_data_user_interface.process_in.master_diagnostic.node_io_communication_status_2;
                    NodeIOCommunicationStatus4863 = __process_in_data_user_interface.process_in.master_diagnostic.node_io_communication_status_3;
                    NodeIOErrorStatus0015 = __process_in_data_user_interface.process_in.master_diagnostic.node_io_error_status_0;
                    NodeIOErrorStatus1631 = __process_in_data_user_interface.process_in.master_diagnostic.node_io_error_status_1;
                    NodeIOErrorStatus3247 = __process_in_data_user_interface.process_in.master_diagnostic.node_io_error_status_2;
                    NodeIOErrorStatus4863 = __process_in_data_user_interface.process_in.master_diagnostic.node_io_error_status_3;
                    NodeDiagnosticInfoStatus0015 = __process_in_data_user_interface.process_in.master_diagnostic.node_diagnostic_info_status_0;
                    NodeDiagnosticInfoStatus1631 = __process_in_data_user_interface.process_in.master_diagnostic.node_diagnostic_info_status_1;
                    NodeDiagnosticInfoStatus3247 = __process_in_data_user_interface.process_in.master_diagnostic.node_diagnostic_info_status_2;
                    NodeDiagnosticInfoStatus4863 = __process_in_data_user_interface.process_in.master_diagnostic.node_diagnostic_info_status_3;
                }
            }

            __user_write_data();
        }

        protected override void _online_state_changed(bool online)
        {
            if(online == false)
            {
                __process_in_data_device = new DNM_PROCESS_IN_DATA_T();
                __process_in_data_share = new DNM_PROCESS_IN_DATA_T();
                __process_in_data_user_interface = new DNM_PROCESS_IN_DATA_T();
                __process_out_data_device = new DNM_PROCESS_OUT_DATA_T();
                __process_out_data_share = new DNM_PROCESS_OUT_DATA_T();
                __process_out_data_user_interface = new DNM_PROCESS_OUT_DATA_T();
                IsEnabled = false;
                CommandPending = false;
                __device_sync_control = false;
                //
            }
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
                    switch (property)
                    {
                        case "Device Address":
                            if (reader.Read() && reader.TokenType == JsonTokenType.String)
                                SetProperty(ref __device_address, reader.GetString(), false, nameof(DeviceAddress));
                            else
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid string.");
                            break;
                        case "Device Model":
                            if (reader.Read() && reader.TokenType == JsonTokenType.String)
                            {
                                if(Enum.TryParse<DN_DEVICE_MODEL_TYPE_T>(reader.GetString(), out DN_DEVICE_MODEL_TYPE_T res))
                                    SetProperty(ref __model, res, false, nameof(Model));
                                else
                                    throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value({reader.GetString()}) of {property} in given JSON object is not a valid string of DN_DEVICE_MODEL_TYPE_T.");
                            }
                            else
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid string.");
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

            writer.WritePropertyName("Device Model");
            writer.WriteStringValue(Model.ToString());

            writer.WriteEndObject();
            return writer.BytesPending - start;
        }
    }

    public class Factory : ICabinet
    {
        private string __version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string Name { get => "DeviceNetMasterUtility"; set => throw new NotImplementedException(); }
        public string Description { get => @"DeviceNet Master Information and DeviceNet Master/Slave Diagnostic Tool. Compatiable with RJ71DN91(master) and QJ71DN91(master)"; set => throw new NotImplementedException(); }
        public string Version { get => __version; set => throw new NotImplementedException(); }

        public object CreateInstance(PropertyChangedEventHandler propertyChangedEventHandler)
        {
            DeviceNetUtilityDataModel data = new DeviceNetUtilityDataModel() { FriendlyName = "DeviceNetMasterUtility" };
            data.UserPropertyChanged += propertyChangedEventHandler;
            return new DeviceNetUtilityControl(data);
        }
    }
}
