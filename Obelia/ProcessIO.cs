using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP;
using AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Obelia.SlavePDOs.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Obelia.ECAT_PROCESS_IN_DATA_T;
using static AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Obelia.ECAT_PROCESS_IN_DATA_T.PROCESS_IN_T;
using static AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Obelia.ECAT_PROCESS_OUT_DATA_T;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Obelia
{
    public enum SLAVE_PDO_SYNC_MODE_T
    {
        Disabled,
        Monitor,
        Interactive
    }
    public enum DEVICE_MODEL_TYPE_T : byte
    {
        RJ72EC92 = 0x01,
    }
    public enum CONFIGURATION_STATE_T : ushort
    {
        NOT_COMPLETE = 0x0000,
        COMPLETE = 0x0001
    }

    public enum COMMUNICATION_STATE_T : ushort
    {
        STOPPED = 0x0000,
        COMMUNICATING = 0x0001,
        DISCONNECTION = 0x0002
    }

    public enum STATE_MACHINE_T : ushort
    {
        NONE = 0x0000,
        INIT = 0x0001,
        PRE_OP = 0x0002,
        SAFE_OP = 0x0004,
        OP = 0x0008
    }

    public enum SDO_DATA_DISPLAY_FORMAT_T: ushort
    {
        BYTE_ARRAY = 0x0000,
        INT16 = 0x0001,
        UINT16 = 0x0002,
        INT32 = 0x0003,
        UINT32 = 0x0004,
        INT64 = 0x0005,
        UINT64 = 0x0006,
        SINGLE = 0x0007,
        DOUBLE = 0x0008,
        ASCII = 0x0010
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct MASTER_EVENT_T
    {
        public uint code;
        public ushort year;
        public ushort month;
        public ushort day;
        public ushort hour;
        public ushort minute;
        public ushort second;
        public ushort millisecond0;
        public ushort millisecond1;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct SDO_PARAMETER_T
    {
        public ushort slave_address;
        public ushort object_index;
        public ushort object_sub_index;
        public int data_size_in_byte;
    }

    public enum SDO_COMMAND_T : ushort
    {
        NONE = 0x0000,
        UPLOAD = 0x0001,
        DOWNLOAD = 0x0002
    }

    public enum CONTROL_COMMAND_T : ushort
    {
        NONE = 0x0000,
        STOP = 0x0001,
        START = 0x0002
    }

    public enum ASYNC_COMMAND_CODE_T
    {
        NONE,
        ENABLE,
        ENABLE_WITH_ENI,
        DISABLE,
        Y,
        START_IO_COMMUNICATION,
        STOP_IO_COMMUNICATION,
        INTERACTIVE_MODE,
        RELOAD_MASTER_EVENT_HISTORY,
        EXECUTE_SDO_COMMAND,
        REQUEST_MASTER_ESM,
        REQUEST_SLAVE_ESM,
        MASTER_CONTROL
    }

    public enum Y_REQUEST_T: ushort
    {
        CLEAR_MODULE_ERROR = 15
    }

    internal abstract class ASYNC_COMMAND_T
    {
        public ASYNC_COMMAND_CODE_T cmd { get; protected init; }
    }

    internal class ASYNC_RESULT_T
    {
        public ASYNC_COMMAND_CODE_T cmd { get; init; }
        public SLMP_EXCEPTION_CODE_T exception_code { get; init; }
        public ushort end_code { get; init; }
    }

    internal class ENABLE_COMMAND_T : ASYNC_COMMAND_T
    {
        public string device_address { get; init; } = "U000";
        public DEVICE_MODEL_TYPE_T model { get; init; } = DEVICE_MODEL_TYPE_T.RJ72EC92;
        public ENABLE_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.ENABLE; }
    }

    internal class ENABLE_RESULT_T : ASYNC_RESULT_T
    {
        public string device_address { get; init; } = "U000";
        public DEVICE_MODEL_TYPE_T model { get; init; } = DEVICE_MODEL_TYPE_T.RJ72EC92;
        public ENABLE_RESULT_T() : base() { cmd = ASYNC_COMMAND_CODE_T.ENABLE; }
    }

    internal class ENABLE_WITH_ENI_COMMAND_T : ASYNC_COMMAND_T
    {
        public string device_address { get; init; } = "U000";
        public DEVICE_MODEL_TYPE_T model { get; init; } = DEVICE_MODEL_TYPE_T.RJ72EC92;
        public GenericSlavePdosDataModel slaves { get; init; }
        public ENABLE_WITH_ENI_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.ENABLE_WITH_ENI; }
    }

    internal class ENABLE_WITH_ENI_RESULT_T : ASYNC_RESULT_T
    {
        public string device_address { get; init; } = "U000";
        public DEVICE_MODEL_TYPE_T model { get; init; } = DEVICE_MODEL_TYPE_T.RJ72EC92;
        public GenericSlavePdosDataModel slaves { get; init; }
        public ENABLE_WITH_ENI_RESULT_T() : base() { cmd = ASYNC_COMMAND_CODE_T.ENABLE_WITH_ENI; }
    }

    internal class DISABLE_COMMAND_T : ASYNC_COMMAND_T
    {
        public DISABLE_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.DISABLE; }
    }

    internal class DISABLE_RESULT_T : ASYNC_RESULT_T
    {
        public DISABLE_RESULT_T() : base() { cmd = ASYNC_COMMAND_CODE_T.DISABLE; }
    }

    internal class EXECUTE_Y_COMMAND_T : ASYNC_COMMAND_T
    {
        public Y_REQUEST_T request_command { get; init; }
        public bool request_value { get; init; }

        public EXECUTE_Y_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.Y; }
    }

    internal class EXECUTE_Y_RESULT_T : ASYNC_RESULT_T
    {
        public Y_REQUEST_T request_command { get; init; }
        public bool request_value { get; init; }
        public EXECUTE_Y_RESULT_T() : base() { cmd = ASYNC_COMMAND_CODE_T.Y; }
    }

    internal class SWITCH_INTERACTIVE_COMMAND_T : ASYNC_COMMAND_T
    {
        public bool enabled { get; init; }
        public SWITCH_INTERACTIVE_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.INTERACTIVE_MODE; }
    }

    internal class SWITCH_INTERACTIVE_RESULT_T : ASYNC_RESULT_T
    {
        public bool enabled { get; init; }
        public ushort[] rx_pdo;
        public SWITCH_INTERACTIVE_RESULT_T() : base() { cmd = ASYNC_COMMAND_CODE_T.INTERACTIVE_MODE; }
    }

    internal class RELOAD_MASTER_EVENT_HISTORY_COMMAND_T : ASYNC_COMMAND_T
    {
        public RELOAD_MASTER_EVENT_HISTORY_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.RELOAD_MASTER_EVENT_HISTORY; }
    }

    internal class RELOAD_MASTER_EVENT_HISTORY_RESULT_T : ASYNC_RESULT_T
    {
        public ushort[] events { get; init; }
        public RELOAD_MASTER_EVENT_HISTORY_RESULT_T() : base() { cmd = ASYNC_COMMAND_CODE_T.RELOAD_MASTER_EVENT_HISTORY; }
    }

    internal class BLOCKING_COMMAND_T : ASYNC_COMMAND_T
    {
        public uint timeout { get; init; }
        public int start_ticks { get; init; }
        public int step { get; set; }
    }

    internal class BLOCKING_RESULT_T : ASYNC_RESULT_T
    {
        public bool timeout { get; init; }
    }

    internal class EXECUTE_SDO_COMMAND_T : BLOCKING_COMMAND_T
    {
        public SDO_COMMAND_T sdo_command { get; init; }
        public SDO_PARAMETER_T header { get; init; }

        public ushort[]? data { get; init; }

        public EXECUTE_SDO_COMMAND_T():base(){ cmd = ASYNC_COMMAND_CODE_T.EXECUTE_SDO_COMMAND; }
    }

    internal class EXECUTE_SDO_RESULT_T : BLOCKING_RESULT_T
    {
        public EXECUTE_SDO_RESULT_T() : base() { cmd = ASYNC_COMMAND_CODE_T.EXECUTE_SDO_COMMAND; }
        public SDO_COMMAND_T sdo_command { get; init; }
        public SDO_PARAMETER_T header { get; init; }
        public ushort error { get; init; }
        public ushort[]? data { get; init; }
    }

    internal class REQUEST_MASTER_ESM_COMMAND_T : BLOCKING_COMMAND_T
    {
        
        public STATE_MACHINE_T esm { get; set; }
        public REQUEST_MASTER_ESM_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.REQUEST_MASTER_ESM; }
    }

    internal class REQUEST_MASTER_ESM_RESULT_T : BLOCKING_RESULT_T
    {
        public STATE_MACHINE_T esm { get; init; }
        public ushort error { get; init; }
        public REQUEST_MASTER_ESM_RESULT_T() : base() { cmd = ASYNC_COMMAND_CODE_T.REQUEST_MASTER_ESM; }
    }

    internal class REQUEST_SLAVE_ESM_COMMAND_T : BLOCKING_COMMAND_T
    {

        public STATE_MACHINE_T esm { get; init; }
        public ushort index { get; init; }
        public REQUEST_SLAVE_ESM_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.REQUEST_MASTER_ESM; }
    }

    internal class REQUEST_SLAVE_ESM_RESULT_T : BLOCKING_RESULT_T
    {
        public STATE_MACHINE_T esm { get; init; }
        public ushort index { get; init; }
        public ushort error { get; init; }
        public REQUEST_SLAVE_ESM_RESULT_T() : base() { cmd = ASYNC_COMMAND_CODE_T.REQUEST_MASTER_ESM; }
    }

    internal class XMASTER_CONTROL_COMMAND_T : BLOCKING_COMMAND_T
    {
        public CONTROL_COMMAND_T control { get; init; }
        public XMASTER_CONTROL_COMMAND_T() : base() { cmd = ASYNC_COMMAND_CODE_T.MASTER_CONTROL; }
    }

    internal class XMASTER_CONTROL_RESULT_T : BLOCKING_RESULT_T
    {
        public CONTROL_COMMAND_T control { get; init; }
        public XMASTER_CONTROL_RESULT_T() : base() { cmd = ASYNC_COMMAND_CODE_T.MASTER_CONTROL; }
    }

    internal struct ECAT_PROCESS_IN_DATA_T
    {
        public struct SLAVE_PDO_T
        {
            public ushort[] state_machines ;
            public ushort[] errors;
            public ushort[] tx_pdo;
            public ushort[] rx_readback_pdo;
            private ulong hash;

            public SLAVE_PDO_T(int numberOfSlaves, int txSizeInWord, int rxSizeInWord)
            {
                state_machines = new ushort[numberOfSlaves];
                errors = new ushort[numberOfSlaves];
                tx_pdo = new ushort[txSizeInWord];
                rx_readback_pdo = new ushort[rxSizeInWord];
                hash = Convert.ToUInt64($"{numberOfSlaves:X4}{txSizeInWord:X4}{rxSizeInWord:X4}", 16);
            }

            public void CopyTo(ref SLAVE_PDO_T? data)
            {
                if (data == null || data.Value.hash != hash)
                    data = new SLAVE_PDO_T(state_machines.Length, tx_pdo.Length, rx_readback_pdo.Length);

                Array.Copy(state_machines, data.Value.state_machines, state_machines.Length);
                Array.Copy(errors, data.Value.errors, errors.Length);
                Array.Copy(tx_pdo, data.Value.tx_pdo, tx_pdo.Length);
                Array.Copy(rx_readback_pdo, data.Value.rx_readback_pdo, rx_readback_pdo.Length);
            }
        }
        public struct PROCESS_IN_T
        {
            [StructLayout(LayoutKind.Sequential, Pack = 2)]
            public struct MASTER_DIAGNOSTIC_INFO_T
            {
                public ushort number_of_slaves_registered;
                public CONFIGURATION_STATE_T configuration_state;
                public COMMUNICATION_STATE_T communication_state;
                public ushort master_error;
                public ushort cable_error;
                public void Reset()
                {
                    number_of_slaves_registered = 0;
                    configuration_state = CONFIGURATION_STATE_T.NOT_COMPLETE;
                    communication_state = COMMUNICATION_STATE_T.STOPPED;
                    master_error = 0;
                    cable_error = 0;
                }
            }

            public MASTER_DIAGNOSTIC_INFO_T master_diagnostic_info;
            public ushort x_status;
            public ushort y_status;
            public STATE_MACHINE_T master_state_machine;

            public SLAVE_PDO_T? slave_pdo;

            public ushort pin_end_code;
            public SLMP_EXCEPTION_CODE_T pin_exception_code;

            public ushort pout_end_code;
            public SLMP_EXCEPTION_CODE_T pout_exception_code;

            //public ushort end_code;
            //public SLMP_EXCEPTION_CODE_T exception_code;

            public void CopyTo(ref PROCESS_IN_T data)
            {
                data.master_diagnostic_info = master_diagnostic_info;
                data.x_status = x_status;
                data.y_status = y_status;
                data.master_state_machine = master_state_machine;
                data.pin_end_code = pin_end_code;
                data.pin_exception_code = pin_exception_code;
                data.pout_end_code = pout_end_code;
                data.pout_exception_code = pout_exception_code;
                slave_pdo?.CopyTo(ref data.slave_pdo);
            }

            public void Reset()
            {
                master_diagnostic_info.Reset();
                x_status = 0;
                y_status = 0;
                master_state_machine = STATE_MACHINE_T.NONE;
                slave_pdo = null;
                pin_end_code = 0;
                pin_exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR;
                pout_end_code = 0;
                pout_exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR;
            }
        }

        public PROCESS_IN_T process_in;
        public ASYNC_RESULT_T? control_in;

        public void Reset()
        {
            process_in.Reset();
            control_in = null;
        }
    }

    internal struct ECAT_PROCESS_OUT_DATA_T
    {
        public struct SLAVE_PDO_T
        {
            public ushort[] rx_pdo;
            private ulong hash;
            public SLAVE_PDO_T(int rxSizeInWord)
            {
                rx_pdo = new ushort[rxSizeInWord];
                hash = hash = Convert.ToUInt64($"{rxSizeInWord:X4}", 16); ;
            }

            public void CopyTo(ref SLAVE_PDO_T? data)
            {
                if (data == null || data.Value.hash != hash)
                    data = new SLAVE_PDO_T(rx_pdo.Length);

                Array.Copy(rx_pdo, data.Value.rx_pdo, rx_pdo.Length);
            }
        }

        public struct PROCESS_OUT_T
        {
            public SLAVE_PDO_T? slave_pdo;

            public void CopyTo(ref PROCESS_OUT_T data)
            {
                slave_pdo?.CopyTo(ref data.slave_pdo);
            }

            public void Reset()
            {
                slave_pdo = null;
            }
        }
        public PROCESS_OUT_T process_out;
        public ASYNC_COMMAND_T? control_out;

        public void Reset()
        {
            process_out.Reset();
            control_out = null;
        }
    }
}
