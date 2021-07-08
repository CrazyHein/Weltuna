using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Numeros
{
    public enum DECIMAL_POINT_POSITION_T : ushort
    {
        NOTHING = 0,
        FIRST_DECIMAL_PLACE = 1
    }

    public enum DEVICE_OPERATION_MODE_T : byte
    {
        SETTING_MODE = 0,
        OPERATION_MODE = 1
    }

    public enum CHANNEL_AUTO_TUNING_STATUS : byte
    {
        BEING_PERFORMED = 1,
        NOT_BEING_PERFORMED = 0
    }

    public enum CHANNEL_OPERATION_MODE_T : ushort
    {
        AUTO = 0,
        MANUAL = 1
    }

    public enum CHANNEL_AT_MODE_T : ushort
    {
        STANDARD_MODE = 0,
        HIGH_RESPONSE_MODE = 1
    }
    
    internal struct CHANNNEL_PROCESS_IN_DATA_T
    {
        public ushort point_position;
        public short temperature_process_value;
        public short set_value;
        public short manipulated_value_of_heating;
        public short manipulated_value_of_cooling;
        public ushort heating_proportional_band;//0-10000
        public ushort cooling_proportional_band;//1-10000
        public ushort integral_time;//0-3600
        public ushort derivative_time;//0-3600
        public ushort loop_disconnection_detection; //0-7200
    }

    internal struct DEVICE_PROCESS_IN_DATA_T
    {
        public ushort device_error_code;
        public ushort device_operation_mode;
        public ushort device_operation_request;
        public ushort man_mode_shift_completion_flag;
        public ushort man_mode_shift_request_flag;
        public ushort at_request;
        public ushort at_status;
        public int index;
    }
    
    internal struct Q64TC_PROCESS_IN_DATA_T
    {
        public ushort end_code;
        public SLMP_EXCEPTION_CODE_T exception_code;
        public DEVICE_PROCESS_IN_DATA_T device_process_in;
        public CHANNNEL_PROCESS_IN_DATA_T[] channel_process_in;
        public ASYNC_RESULT_T control_in;
    }
    
    internal struct Q64TC_PROCESS_OUT_DATA_T
    {
        public ASYNC_COMMAND_T control_out;
    }

    public enum ASYNC_COMMAND_CODE_T
    {
        NONE,
        ENABLE,
        DISABLE,
        SWITCH_PROCESS_IO_CHANNEL,
        SWITCH_DEVICE_OPERATION_MODE,
        SWITCH_CHANNEL_OPERATION_MODE,
        CLEAR_DEVICE_ERROR,
        SET_AT_FLAG,
        RESET_AT_FLAG,
        BACKUP_DEVICE_CONSTANTS
    }

    internal abstract class ASYNC_COMMAND_T
    {
        //public int channel_index { get; init; }
        public ASYNC_COMMAND_CODE_T cmd { get; protected init; }
    }

    internal class ASYNC_RESULT_T
    {
        //public int channel_index { get; init; }
        public ASYNC_COMMAND_CODE_T cmd { get; init; }
        public SLMP_EXCEPTION_CODE_T exception_code { get; init; }
        public ushort end_code { get; init; }
    }

    internal class ENABLE_COMMAND_T : ASYNC_COMMAND_T
    {
        public string device_address { get; init; }
        public ENABLE_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.ENABLE; }
    }

    internal class ENABLE_RESULT_T : ASYNC_RESULT_T
    {
        public ushort position0 { get; init; }
        public ushort position1 { get; init; }
        public ushort position2 { get; init; }
        public ushort position3 { get; init; }
        public ENABLE_RESULT_T() { cmd = ASYNC_COMMAND_CODE_T.ENABLE; }
    }

    internal class DISABLE_COMMAND_T : ASYNC_COMMAND_T
    {
        public DISABLE_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.DISABLE; }
    }

    internal class SWITCH_CHANNEL_T : ASYNC_COMMAND_T
    {
        public int channel_index { get; init; }
        public SWITCH_CHANNEL_T() : base() { cmd = ASYNC_COMMAND_CODE_T.SWITCH_PROCESS_IO_CHANNEL; }
    }

    internal class SET_AT_REQUEST : ASYNC_COMMAND_T
    {
        public int channel_index { get; init; }
        public short set_value_setting { get; init; }
        public short at_bias_setting { get; init; }
        public bool at_loop_disconnection_detection_flag { get; init; }
        public ushort at_loop_disconnection_detection_setting { get; init; }
        public CHANNEL_AT_MODE_T at_mode { get; init; }
        public bool at_automatic_backup_flag { get; init; }
        public SET_AT_REQUEST() : base() { cmd = ASYNC_COMMAND_CODE_T.SET_AT_FLAG; }
    }

    internal class RESET_AT_REQUEST : ASYNC_COMMAND_T
    {
        public int channel_index { get; init; }
        public RESET_AT_REQUEST() : base() { cmd = ASYNC_COMMAND_CODE_T.RESET_AT_FLAG; }
    }

    internal class BLOCKING_COMMAND_T : ASYNC_COMMAND_T
    {
        public uint timeout { get; init; }
        public int start_ticks { get; init; }
        public int step { get; set; }
    }

    internal class BLOCKING_RESULT : ASYNC_RESULT_T
    {
        public bool timeout { get; init; }
    }

    internal class CLEAR_DEVICE_ERROR_T : BLOCKING_COMMAND_T
    {
        public CLEAR_DEVICE_ERROR_T() { cmd = ASYNC_COMMAND_CODE_T.CLEAR_DEVICE_ERROR; }
    }

    internal class SWITCH_DEVICE_OPERATION_MODE_T : BLOCKING_COMMAND_T
    {
        public DEVICE_OPERATION_MODE_T mode { get; init; }
        public SWITCH_DEVICE_OPERATION_MODE_T() { cmd = ASYNC_COMMAND_CODE_T.SWITCH_DEVICE_OPERATION_MODE; }
    }

    internal class SWITCH_CHANNEL_OPERATION_MODE_T : BLOCKING_COMMAND_T
    {
        public int channel_index { get; init; }
        public CHANNEL_OPERATION_MODE_T mode { get; init; }
        public SWITCH_CHANNEL_OPERATION_MODE_T() { cmd = ASYNC_COMMAND_CODE_T.SWITCH_CHANNEL_OPERATION_MODE; }
    }

    internal class BACKUP_PID_CONSTANTS_T : BLOCKING_COMMAND_T
    {
        public ReadOnlyMemory<ushort> Ph { get; init; }
        public ReadOnlyMemory<ushort> Pc { get; init; }
        public ReadOnlyMemory<ushort> I { get; init; }
        public ReadOnlyMemory<ushort> D { get; init; }
        public ReadOnlyMemory<ushort> LP { get; init; }
        public BACKUP_PID_CONSTANTS_T() { cmd = ASYNC_COMMAND_CODE_T.BACKUP_DEVICE_CONSTANTS; }
    }

    internal class BACKUP_PID_CONSTANTS_RESULT_T : BLOCKING_RESULT
    {
        public bool failure { get; init; }
        public BACKUP_PID_CONSTANTS_RESULT_T() { cmd = ASYNC_COMMAND_CODE_T.BACKUP_DEVICE_CONSTANTS; }
    }
}
