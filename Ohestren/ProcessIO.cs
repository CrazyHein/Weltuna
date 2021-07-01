using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Ohestren
{
    public enum DN_DEVICE_MODE_SWITCH_T : ushort
    {
        MASTER_MODE_AT_125K = 0x00,
        MASTER_MODE_AT_250K = 0x01,
        MASTER_MODE_AT_500K = 0x02,
        SLAVE_MODE_AT_125K = 0x03,
        SLAVE_MODE_AT_250K = 0x04,
        SLAVE_MODE_AT_500K = 0x05,
        HYBRID_MODE_AT_125K = 0x06,
        HYBRID_MODE_AT_250K = 0x07,
        HYBRID_MODE_AT_500K = 0x08,
        HARDWARE_TEST = 0x09,
        COMMUNICATION_TEST_AT_125K = 0x0A,
        COMMUNICATION_TEST_AT_250K = 0x0B,
        COMMUNICATION_TEST_AT_500K = 0x0C,

        UNKNOWN_SWITCH = 0xFF
    }

    public enum DN_DEVICE_MODEL_TYPE_T : byte
    {
        RJ71DN91 = 0x01,
        //QJ71DN91 = 0x02
    }

    internal enum DNM_IO_COMMUNICATION_STATUS_T : ushort
    {
        OFFLINE = 0x0000,
        STOP = 0x0040,
        OPERATE = 0x00C0,
    }

    internal struct DNM_PROCESS_IN_DATA_T
    {
        public struct PROCESS_IN_T
        {
            [StructLayout(LayoutKind.Sequential, Pack = 2)]
            public struct MASTER_NODE_DIAGNOSTIC_INFO_T
            {
                public ushort master_communication_status;
                public ushort master_communication_error_code;
                public ushort bus_error_counter;
                public ushort bus_off_counter;
                public ushort node_configuration_status_0;
                public ushort node_configuration_status_1;
                public ushort node_configuration_status_2;
                public ushort node_configuration_status_3;
                public ushort dummy_0;
                public ushort dummy_1;
                public ushort dummy_2;
                public ushort dummy_3;
                public ushort node_io_communication_status_0;
                public ushort node_io_communication_status_1;
                public ushort node_io_communication_status_2;
                public ushort node_io_communication_status_3;
                public ushort node_io_error_status_0;
                public ushort node_io_error_status_1;
                public ushort node_io_error_status_2;
                public ushort node_io_error_status_3;
                public ushort node_diagnostic_info_status_0;
                public ushort node_diagnostic_info_status_1;
                public ushort node_diagnostic_info_status_2;
                public ushort node_diagnostic_info_status_3;
            }
            [StructLayout(LayoutKind.Sequential, Pack = 2)]
            public struct LINK_SCAN_TIME_T
            {
                public ushort courrent_linkscan_time;
                public ushort minimum_linkscan_time;
                public ushort maximum_linkscan_time;
            }

            public ushort end_code;
            public SLMP_EXCEPTION_CODE_T exception_code;

            public ushort x_status;
            public MASTER_NODE_DIAGNOSTIC_INFO_T master_diagnostic;
            public LINK_SCAN_TIME_T link_scan;
        };
        public PROCESS_IN_T process_in;
        public ASYNC_RESULT_T control_in;
    }

    internal struct DNM_PROCESS_OUT_DATA_T
    {
        public ASYNC_COMMAND_T control_out;
    }

    public enum ASYNC_COMMAND_CODE_T
    {
        NONE,
        ENABLE,
        DISABLE,
        START_IO_COMMUNICATION,
        STOP_IO_COMMUNICATION,
        RESET_ERROR,
        EXECUTE_EXPLICIT_MESSAGE,
    }

    internal abstract class ASYNC_COMMAND_T
    {
        public ASYNC_COMMAND_CODE_T cmd { get; protected init; }
    }

    internal class ENABLE_COMMAND_T : ASYNC_COMMAND_T
    {
        public string device_address { get; init; }
        public DN_DEVICE_MODEL_TYPE_T model { get; init; }
        public ENABLE_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.ENABLE; }
    }

    internal class DISABLE_COMMAND_T : ASYNC_COMMAND_T
    {
        public DISABLE_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.DISABLE; }
    }

    internal class BLOCKING_COMMAND_T : ASYNC_COMMAND_T
    {
        public uint timeout { get; init; }
        public int start_ticks { get; init; }
        public int step { get; set; }
    }

    internal class START_IO_COM_COMMAND_T : BLOCKING_COMMAND_T
    {
        public START_IO_COM_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.START_IO_COMMUNICATION; }
    }

    internal class STOP_IO_COM_COMMAND_T : BLOCKING_COMMAND_T
    {
        public STOP_IO_COM_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.STOP_IO_COMMUNICATION; }
    }

    internal class RESET_ERROR_COMMAND_T : BLOCKING_COMMAND_T
    {
        public RESET_ERROR_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.RESET_ERROR; }
    }

    internal class EXECUTE_EXPLICIT_MESSAGE_COMMAND_T : BLOCKING_COMMAND_T
    {
        public EXECUTE_EXPLICIT_MESSAGE_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.EXECUTE_EXPLICIT_MESSAGE; }
        public EXPLICIT_MESSAGE_REQUEST_HEADER_T header { get; init; }
        public ReadOnlyMemory<byte> data { get; init; }
    }

    internal class ASYNC_RESULT_T
    {
        public ASYNC_COMMAND_CODE_T cmd { get; init; }
        public SLMP_EXCEPTION_CODE_T exception_code { get; init; }
        public ushort end_code { get; init; }
    }

    internal class ENABLE_RESULT_T : ASYNC_RESULT_T
    {
        public string model_name { get; init; }
        public ushort node_address { get; init; }
        public ushort mode_switch { get; init; }
        public ENABLE_RESULT_T() : base() { cmd = ASYNC_COMMAND_CODE_T.ENABLE; }
    }

    internal class DISABLE_RESULT_T : ASYNC_RESULT_T
    {
        public DISABLE_RESULT_T() : base() { cmd = ASYNC_COMMAND_CODE_T.DISABLE; }
    }

    internal class BLOCKING_RESULT : ASYNC_RESULT_T
    {
        public bool timeout { get; init; }
    }

    internal class EXECUTE_EXPLICIT_MESSAGE_RESULT_T : BLOCKING_RESULT
    {
        public EXECUTE_EXPLICIT_MESSAGE_RESULT_T() : base() { cmd = ASYNC_COMMAND_CODE_T.EXECUTE_EXPLICIT_MESSAGE; }
        public EXPLICIT_MESSAGE_RESULT_HEADER_T header { get; init; }
        public ReadOnlyMemory<byte> data { get; init; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct EXPLICIT_MESSAGE_REQUEST_HEADER_T
    {
        public EXPLICIT_MESSAGE_COMMAND_NUMBER_T command_number;
        public byte slave_node_address;
        public byte object_class_id;
        public ushort object_instance_id;
        public byte object_attribute_id;
        public byte command_data_length;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct EXPLICIT_MESSAGE_RESULT_HEADER_T
    {
        public EXPLICIT_MESSAGE_COMMAND_NUMBER_T command_number;
        public ushort execution_error_code;
        public byte slave_node_address;
        public byte object_class_id;
        public ushort object_instance_id;
        public byte object_attribute_id;
        public byte command_data_length;
    }
    
    internal enum EXPLICIT_MESSAGE_COMMAND_NUMBER_T : ushort
    {
        R_READ_ATTRIBUTE = 0x0101,
        R_WRITE_ATTRIBUTE = 0x0102,
        R_READ_DIAGNOSTIC_INFO = 0x0001,
        R_RESET = 0x0201,

        R_OTHER = 0xFE00,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct SLAVE_NODE_DIAGNOSTIC_INFO_T
    {
        public ushort slave_status;
        public ushort dummy1;
        public ushort message_communication_error;
        public ushort general_dnet_error_code;
        public ushort additional_error_code;
        public ushort number_of_heartbeat_timeout;
    }
}
