using System;
using System.Runtime.InteropServices;

namespace AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Message
{
    public enum MESSAGE_FRAME_TYPE_T
    {
        MC_3E,
        MC_4E,
        STATION_NUM_EXTENSION
    }

    public enum MESSAGE_DATA_CODE_T
    {
        ASCII,
        BINARY
    }

    public enum REQUEST_DESTINATION_NETWORK_T : byte
    {
        CONNECTED_NETWORK                       = 0x00, 
    }

    public enum REQUEST_DESTINATION_STATION_T : byte
    {
        CONNECTED_STATION                       = 0xFF,
        STATION_NUMBER_EXTENSION_FRAME          = 0x7C,
        ASSIGNED_CONTROL_MASTER_STATION         = 0x7D,
        PRESENT_CONTROL_MASTER_STATION          = 0x7E,
    }

    public enum REQUEST_DESTINATION_EXTENSION_STATION_T : ushort
    {
        DESTINATION_STATION                     = 0x0000,
        ASSIGNED_CONTROL_MASTER_STATION         = 0x0000,
        CONNECTED_STATION                       = 0xFFFF,
    }

    public enum REQUEST_CPU_DESTINATION_MODULE_IO_T : ushort
    {
        OWN_STATION                             = 0x03FF,
        CONTROL_CPU                             = 0x03FF,
        MULTIPLE_CPU0                           = 0x03E0,
        MULTIPLE_CPU1                           = 0x03E1,
        MULTIPLE_CPU2                           = 0x03E2,
        CMULTIPLE_CPU3                          = 0x03E3,
        CONTROL_SYSTEM_CPU                      = 0x03D0,
        STANDBY_SYSTEM_CPU                      = 0x03D1,
        SYSTEM_A_CPU                            = 0x03D2,
        SYSTEM_B_CPU                            = 0x03D3,
    }

    public enum REQUEST_CCIEF_DESTINATION_MODULE_IO_T : ushort
    {
        OWN_STATION                             = 0x03FF,
        REMOTE_HEAD_MODULE_NO_1                 = 0x03E0,
        REMOTE_HEAD_MODULE_NO_2                 = 0x03E1,
        CONTROL_SYSTEM_MODULE                   = 0x03D0,
        STANDBY_SYSTEM_MODULE                   = 0x03D1
    }

    public readonly struct DESTINATION_ADDRESS_T
    {
        public readonly byte network_number;
        public readonly byte station_number;
        public readonly ushort module_io;
        public readonly byte multidrop_number;
        public readonly ushort extension_station_number;

        public DESTINATION_ADDRESS_T(byte network, byte station, ushort module, byte multidrop, ushort extensionStation)
        {
            this.network_number = network;
            this.station_number = station;
            this.module_io = module;
            this.multidrop_number = multidrop;
            this.extension_station_number = extensionStation;
        }

        public DESTINATION_ADDRESS_T(bool ownStation)
        {
            this.network_number = (byte)REQUEST_DESTINATION_NETWORK_T.CONNECTED_NETWORK;
            this.station_number = (byte)REQUEST_DESTINATION_STATION_T.CONNECTED_STATION;
            this.module_io = (ushort)REQUEST_CPU_DESTINATION_MODULE_IO_T.OWN_STATION;
            this.multidrop_number = 0;
            this.extension_station_number = (ushort)REQUEST_DESTINATION_EXTENSION_STATION_T.DESTINATION_STATION;
        }
    }

    class Message
    {
        private static bool __BIG_ENDIAN = false;
        static Message()
        {
            int i = 1;
            byte[] buf = BitConverter.GetBytes(i);
            if (buf[0] != 1)
                __BIG_ENDIAN = true;
        }
        
        public static int BINARY_TO_ASCII_ARRAY(byte data, byte[] dataArray, int startIndex)
        {
            byte res = (byte)(data / 16);
            if(res < 10)
                dataArray[startIndex] = (byte)(res + 0x30);
            else
                dataArray[startIndex] = (byte)(res - 10 + 0x41);
            res = (byte)(data % 16);
            if (res < 10)
                dataArray[startIndex + 1] = (byte)(res + 0x30);
            else
                dataArray[startIndex + 1] = (byte)(res - 10 + 0x41);
            return 2;
        }

        public static int BINARY_TO_ASCII_ARRAY(ushort data, byte[] dataArray, int startIndex)
        {
            int res = BINARY_TO_ASCII_ARRAY((byte)(data >> 8), dataArray, startIndex);
            res += BINARY_TO_ASCII_ARRAY((byte)(data & 0x00FF), dataArray, startIndex + res);
            return res;
        }

        public static int BINARY_TO_ASCII_ARRAY(uint data, byte[] dataArray, int startIndex)
        {
            int res = BINARY_TO_ASCII_ARRAY((ushort)(data >> 16), dataArray, startIndex);
            res += BINARY_TO_ASCII_ARRAY((ushort)(data & 0xFFFF), dataArray, startIndex + res);
            return res;
        }

        public static int BINARY_TO_BINARY_ARRAY(byte data, byte[] dataArray, int startIndex)
        {
            dataArray[startIndex] = data;
            return 1;
        }

        public static int BINARY_TO_BINARY_ARRAY(ushort data, byte[] dataArray, int startIndex)
        {
            dataArray[startIndex] = (byte)(data & 0x00FF);
            dataArray[startIndex + 1] = (byte)(data >> 8);
            return 2;
        }

        public static int BINARY_TO_BINARY_ARRAY(uint data, byte[] dataArray, int startIndex)
        {
            dataArray[startIndex] = (byte)(data & 0x000000FF);
            dataArray[startIndex + 1] = (byte)((data >> 8) & 0x000000FF);
            dataArray[startIndex + 2] = (byte)((data >> 16) & 0x000000FF);
            dataArray[startIndex + 3] = (byte)((data >> 24) & 0x000000FF);
            return 4;
        }

        public static int ASCII_ARRAY_TO_BINARY(byte[] dataArray, int startIndex, out byte data)
        {
            byte res = (byte)(dataArray[startIndex] - 0x30);
            if(res > 9)
                res = (byte)(dataArray[startIndex] - 0x41 + 10);
            if (res > 15)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_ASCII_CODE_VALUE);

            data = res;

            res = (byte)(dataArray[startIndex + 1] - 0x30);
            if (res > 9)
                res = (byte)(dataArray[startIndex + 1] - 0x41 + 10);
            if (res > 15)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_ASCII_CODE_VALUE);

            data = (byte)((data << 4) + res); 

            return 2;
        }

        public static int ASCII_ARRAY_TO_BINARY(ReadOnlySpan<byte> dataArray, int startIndex, out byte data)
        {
            byte res = (byte)(dataArray[startIndex] - 0x30);
            if (res > 9)
                res = (byte)(dataArray[startIndex] - 0x41 + 10);
            if (res > 15)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_ASCII_CODE_VALUE);

            data = res;

            res = (byte)(dataArray[startIndex + 1] - 0x30);
            if (res > 9)
                res = (byte)(dataArray[startIndex + 1] - 0x41 + 10);
            if (res > 15)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_ASCII_CODE_VALUE);

            data = (byte)((data << 4) + res);

            return 2;
        }

        public static int ASCII_ARRAY_TO_BINARY(byte[] dataArray, int startIndex, out ushort data)
        {
            byte d = 0;
            int res = ASCII_ARRAY_TO_BINARY(dataArray, startIndex, out d);
            data = d;
            res += ASCII_ARRAY_TO_BINARY(dataArray, startIndex + res, out d);
            data = (ushort)((data << 8) + d);
            return res;
        }

        public static int ASCII_ARRAY_TO_BINARY(ReadOnlySpan<byte> dataArray, int startIndex, out ushort data)
        {
            byte d = 0;
            int res = ASCII_ARRAY_TO_BINARY(dataArray, startIndex, out d);
            data = d;
            res += ASCII_ARRAY_TO_BINARY(dataArray, startIndex + res, out d);
            data = (ushort)((data << 8) + d);
            return res;
        }

        public static int BINARY_ARRAY_TO_BINARY(byte[] dataArray, int startIndex, out byte data)
        {
            data = dataArray[startIndex];
            return 1;
        }

        public static int BINARY_ARRAY_TO_BINARY(ReadOnlySpan<byte> dataArray, int startIndex, out byte data)
        {
            data = dataArray[startIndex];
            return 1;
        }

        public static int BINARY_ARRAY_TO_BINARY(byte[] dataArray, int startIndex, out ushort data)
        {
            data = (ushort)((dataArray[startIndex + 1] << 8) + dataArray[startIndex]);
            return 2;
        }

        public static int BINARY_ARRAY_TO_BINARY(ReadOnlySpan<byte> dataArray, int startIndex, out ushort data)
        {
            data = (ushort)((dataArray[startIndex + 1] << 8) + dataArray[startIndex]);
            return 2;
        }

        public static ushort SMALL_ENDIAN_MODE(ushort data)
        {
            if (__BIG_ENDIAN)
                return (ushort)(((data&0x00FF) << 8) + (data >> 8));
            else
                return data;
        }

        public static uint SMALL_ENDIAN_MODE(uint data)
        {
            if (__BIG_ENDIAN)
                return  ((data & 0x000000FF) << 24) + 
                        ((data & 0x0000FF00) << 8)  +
                        ((data & 0x00FF0000) >> 8)  +
                        ((data & 0xFF000000) >> 24);
            else
                return data;
        }

        public static bool BYTE_ARRAY_COMPARE(byte[] source, int sourceStartIndex, byte[] destination, int destinationStartIndex, int length)
        {
            for (int i = 0; i < length; ++i)
            {
                if (source[i + sourceStartIndex] != destination[i + destinationStartIndex])
                    return false;
            }
            return true;
        }
    }
}
