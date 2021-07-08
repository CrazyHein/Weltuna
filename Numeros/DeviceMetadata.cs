using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Numeros
{
    class DeviceMetadata
    {
        public static string ERROR_INFO(ushort code)
        {
            switch(code)
            {
                case 0x0000:
                    return "No error.";
                case 0x0001:
                    return "Hardware error.";
                case 0x001E:
                    return "The current control mode and the control mode backed up in the E2PROM are different due to the change of the control mode selection.";
                case 0x000F:
                    return "Values set in the intelligent function module switch setting are those outside the setting range.";
                case var c when (c & 0xF) == 0x0002:
                    return $"Data (other than 0) is being written to the system area.({c >> 4:X4})";
                case var c when (c & 0xF) == 0x0003:
                    return $"Data is being written in the operation mode to the area where data can be written only in the setting mode.({c >> 4:X4})";
                case var c when (c & 0xF) == 0x0004:
                    return $"Data outside the settable range is being written.({c >> 4:X4})";
                case var c when (c & 0xF) == 0x0005:
                    return $"The setting of the upper / lower limit value output limiter or the upper / lower limit setting limiter is invalid.({c >> 4:X4})";
                case var c when (c & 0xF) == 0x0006:
                    return $"The setting value is being changed while Default setting registration instruction(Yn9) was on.({c >> 4:X4})";
                case var c when (c & 0xF) == 0x0007:
                    return $"2-point sensor compensation setting is invalid.({c >> 4:X4})";
                case var c when (c & 0xFF0F) == 0x010A:
                    return $"The temperature process value(PV) has exceeded the temperature measurement range that was set as the input range.({(c & 0x00F0) >> 4})";
                case var c when (c & 0xFF0F) == 0x020A:
                    return $"The temperature process value(PV) is below the temperature measurement range that was set as the input range.({(c & 0x00F0) >> 4})";
                case var c when (c & 0xFF0F) == 0x030A:
                    return $"A loop disconnection has been detected.({(c & 0x00F0) >> 4})";
                case var c when (c & 0xFF0F) == 0x040A:
                    return $"A heater disconnection has been detected.({(c & 0x00F0) >> 4})";
                case var c when (c & 0xFF0F) == 0x050A:
                    return $"A current error at an output off - time has been detected.({(c & 0x00F0) >> 4})";
                case var c when (c & 0xFF0F) == 0x060A:
                    return $"Alert 1 has occurred.({(c & 0x00F0) >> 4})";
                case var c when (c & 0xFF0F) == 0x070A:
                    return $"Alert 2 has occurred.({(c & 0x00F0) >> 4})";
                case var c when (c & 0xFF0F) == 0x080A:
                    return $"Alert 3 has occurred.({(c & 0x00F0) >> 4})";
                case var c when (c & 0xFF0F) == 0x090A:
                    return $"Alert 4 has occurred.({(c & 0x00F0) >> 4})";
                default:
                    return "Unknown error.";
            }
        }
    }

    enum Q64TC_ADDRESS_TABLE_T : ushort
    {
        DV_WRITE_DATA_ERROR_CODE = 0x0000,

        CH0_DECIMAL_POINT_POSITION = 0x0001,
        CH1_DECIMAL_POINT_POSITION = 0x0002,
        CH2_DECIMAL_POINT_POSITION = 0x0003,
        CH3_DECIMAL_POINT_POSITION = 0x0004,

        CH0_TEMPERATURE_PROCESS_VALUE = 0x0009,
        CH1_TEMPERATURE_PROCESS_VALUE = 0x000A,
        CH2_TEMPERATURE_PROCESS_VALUE = 0x000B,
        CH3_TEMPERATURE_PROCESS_VALUE = 0x000C,

        CH0_SET_VALUE_MONITOR = 0x0019,
        CH1_SET_VALUE_MONITOR = 0x001A,
        CH2_SET_VALUE_MONITOR = 0x001B,
        CH3_SET_VALUE_MONITOR = 0x001C,

        CH0_MANIPULATED_VALUE_OF_HEATING = 0x000D,
        CH1_MANIPULATED_VALUE_OF_HEATING = 0x000E,
        CH2_MANIPULATED_VALUE_OF_HEATING = 0x000F,
        CH3_MANIPULATED_VALUE_OF_HEATING = 0x0010,

        CH0_MANIPULATED_VALUE_OF_COOLING = 0x02C0,
        CH1_MANIPULATED_VALUE_OF_COOLING = 0x02C1,
        CH2_MANIPULATED_VALUE_OF_COOLING = 0x02C2,
        CH3_MANIPULATED_VALUE_OF_COOLING = 0x02C3,

        CHANNEL_DISTANCE = 0x0020,

        CH0_HEATING_PROPORTIONAL_BAND = 0x0023,
        CH0_INTEGRAL_TIME = 0x0024,
        CH0_DERIVATIVE_TIME = 0x0025,
        CH0_LOOP_DISCONNECTION_DETECTION = 0x003B,

        CH1_HEATING_PROPORTIONAL_BAND = 0x0043,
        CH1_INTEGRAL_TIME = 0x0044,
        CH1_DERIVATIVE_TIME = 0x0045,
        CH1_LOOP_DISCONNECTION_DETECTION = 0x005B,

        CH2_HEATING_PROPORTIONAL_BAND = 0x0063,
        CH2_INTEGRAL_TIME = 0x0064,
        CH2_DERIVATIVE_TIME = 0x0065,
        CH2_LOOP_DISCONNECTION_DETECTION = 0x007B,

        CH3_HEATING_PROPORTIONAL_BAND = 0x0083,
        CH3_INTEGRAL_TIME = 0x0084,
        CH3_DERIVATIVE_TIME = 0x0085,
        CH3_LOOP_DISCONNECTION_DETECTION = 0x009B,

        CH0_COOLING_PROPORTIONAL_BAND = 0x02D0,
        CH1_COOLING_PROPORTIONAL_BAND = 0x02E0,
        CH2_COOLING_PROPORTIONAL_BAND = 0x02F0,
        CH3_COOLING_PROPORTIONAL_BAND = 0x0300,

        CH0_AUTO_MAN_MODE_SHIFT = 50,
        CH1_AUTO_MAN_MODE_SHIFT = 82,
        CH2_AUTO_MAN_MODE_SHIFT = 114,
        CH3_AUTO_MAN_MODE_SHIFT = 146,

        CH0_SET_VALUE_SETTING = 34,
        CH1_SET_VALUE_SETTING = 66,
        CH2_SET_VALUE_SETTING = 98,
        CH3_SET_VALUE_SETTING = 130,

        CH0_AT_BIAS_SETTING = 53,
        CH1_AT_BIAS_SETTING = 85,
        CH2_AT_BIAS_SETTING = 117,
        CH3_AT_BIAS_SETTING = 149,

        CH0_AT_MODE_SELECTION = 184,
        CH1_AT_MODE_SELECTION = 185,
        CH2_AT_MODE_SELECTION = 186,
        CH3_AT_MODE_SELECTION = 187,

        CH0_AUTOMATIC_BACKUP_SETTING = 63,
        CH1_AUTOMATIC_BACKUP_SETTING = 95,
        CH2_AUTOMATIC_BACKUP_SETTING = 127,
        CH3_AUTOMATIC_BACKUP_SETTING = 159,

        CH_AT_LOOP_DISCONNECTION_DETECTION_FLAG = 571,

        DV_MAN_SHIFT_COMPLETION = 0x001E,

        Y_DV_OPERATION_MODE_INSTRUCTION = 0x01,
        X_DV_OPERATION_MODE_STATUS = 0x01,

        Y_DV_ERROR_RESET = 0x02,
        X_DV_ERROR_FLAG = 0x02,

        Y_CH0_AUTO_TUNING_REQUEST = 0x04,

        Y_DV_E2PROM_BACKUP_INSTRUCTION = 0x08,
        X_DV_E2PROM_BACKUP_COMPLETION = 0x08,
        X_DV_E2PROM_BACKUP_FAILURE = 0x0A,
    }
}
