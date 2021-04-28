using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Command
{
    public enum COMMANDS_T:ushort
    {
        DEVICE_READ                                 = 0x0401,
        DEVICE_WRITE                                = 0x1401,
        DEVICE_READ_RANDOM                          = 0x0403,
        DEVICE_WRITE_RANDOM                         = 0x1402,
        DEVICE_ENTRY_MONITOR_DEVICE                 = 0x0801,
        DEVICE_EXECUTE_MONITOR                      = 0x0802,
        DEVICE_READ_BLOCK                           = 0x0406,
        DEVICE_WRITE_BLOCK                          = 0x1406,

        ARRAY_LABEL_READ                            = 0x041A,
        ARRAY_LABEL_WRITE                           = 0x141A,
        LABEL_READ_RANDOM                           = 0x041C,
        LABEL_WRITE_RANDOM                          = 0x141B,

        REMOTE_RUN                                  = 0x1001,
        REMOTE_STOP                                 = 0x1002,
        REMOTE_PAUSE                                = 0x1003,
        REMOTE_LATCH_CLEAR                          = 0x1005,
        REMOTE_RESET                                = 0x1006,
        TYPE_NAME_READ                              = 0x0101,

        //File Operation

        //
        SELF_TEST                                   = 0x0619,
        ERROR_CLEAR                                 = 0x1617,
        ONDEMAND                                    = 0x2101,

        UNKNOWN                                     = 0xFFFF
    }

    [Flags]
    public enum SUB_COMMANDS_T : ushort
    {
        DEVICE_COMMAND_ACCESS_IN_BIT_UNIT           = 0x0001,
        R_MODULE_DEVICE_COMMAND_DEDICATION          = 0x0002,
        L_MODULE_FILE_OPERATION_DEDICATION          = 0x0004,
        R_MODULE_FILE_OPERATION_DEDICATION          = 0x0040,
        DEVICE_EXTENSION_SPECIFICATION              = 0x0080,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct REQUEST_COMMAND_HEADER_IN_3E_ASCII_T
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] command;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] subcommand;
        public REQUEST_COMMAND_HEADER_IN_3E_ASCII_T(byte dummy = 0x30)
        {
            command = new byte[] { dummy, dummy, dummy, dummy };
            subcommand = new byte[] { dummy, dummy, dummy, dummy };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct REQUEST_COMMAND_HEADER_IN_3E_BINARY_T
    {
        public ushort command;
        public ushort subcommand;

        public REQUEST_COMMAND_HEADER_IN_3E_BINARY_T(byte dummy = 0x00)
        {
            command = Message.Message.SMALL_ENDIAN_MODE(dummy);
            subcommand = Message.Message.SMALL_ENDIAN_MODE(dummy);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct REQUEST_COMMAND_HEADER_IN_EX_BINARY_T
    {
        public ushort command;
        public ushort subcommand;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] fixed_value;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] system_area;

        public REQUEST_COMMAND_HEADER_IN_EX_BINARY_T(byte dummy = 0x00)
        {
            command = Message.Message.SMALL_ENDIAN_MODE(dummy);
            subcommand = Message.Message.SMALL_ENDIAN_MODE(dummy);
            fixed_value = new byte[] { dummy };
            system_area = new byte[] { dummy, dummy, dummy, dummy, dummy };
        }
    }

    public class RequestCommand
    {
        private static byte[] __FIXED_VALUE = new byte[] { 0x00 };
        private static byte[] __SYSTEM_AREA = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 };

        public static int BUILD_BYTE_ARRAY_HEADER(MESSAGE_FRAME_TYPE_T frameType, MESSAGE_DATA_CODE_T dataCode, COMMANDS_T command, SUB_COMMANDS_T subcommand, 
            byte[] dataArray, int startIndex)
        {
            int headerLength = 0;
            IntPtr p = IntPtr.Zero;
            try
            {
                switch (frameType, dataCode)
                {
                    case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.ASCII):
                    case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.ASCII):
                        headerLength = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_ASCII_T>();
                        if (dataArray.Length - startIndex < headerLength)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

                        REQUEST_COMMAND_HEADER_IN_3E_ASCII_T header3a = new REQUEST_COMMAND_HEADER_IN_3E_ASCII_T(0x30);
                        Message.Message.BINARY_TO_ASCII_ARRAY((ushort)command, header3a.command, 0);
                        Message.Message.BINARY_TO_ASCII_ARRAY((ushort)subcommand, header3a.subcommand, 0);
                        p = Marshal.AllocHGlobal(headerLength);
                        Marshal.StructureToPtr(header3a, p, false);
                        break;
                    case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.BINARY):
                    case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.BINARY):
                        headerLength = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>();
                        if (dataArray.Length - startIndex < headerLength)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

                        REQUEST_COMMAND_HEADER_IN_3E_BINARY_T header3b = new REQUEST_COMMAND_HEADER_IN_3E_BINARY_T(0x00);
                        header3b.command = Message.Message.SMALL_ENDIAN_MODE((ushort)command);
                        header3b.subcommand = Message.Message.SMALL_ENDIAN_MODE((ushort)subcommand);
                        p = Marshal.AllocHGlobal(headerLength);
                        Marshal.StructureToPtr(header3b, p, false);
                        break;
                    case (MESSAGE_FRAME_TYPE_T.STATION_NUM_EXTENSION, MESSAGE_DATA_CODE_T.BINARY):
                        headerLength = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_EX_BINARY_T>();
                        if (dataArray.Length - startIndex < headerLength)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

                        REQUEST_COMMAND_HEADER_IN_EX_BINARY_T headerex = new REQUEST_COMMAND_HEADER_IN_EX_BINARY_T(0x00);
                        headerex.command = Message.Message.SMALL_ENDIAN_MODE((ushort)command);
                        headerex.subcommand = Message.Message.SMALL_ENDIAN_MODE((ushort)subcommand);
                        p = Marshal.AllocHGlobal(headerLength);
                        Marshal.StructureToPtr(headerex, p, false);
                        break;
                    default:
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_SUBHEADER);
                }
                Marshal.Copy(p, dataArray, startIndex, headerLength);
            }
            finally
            {
                if (p != IntPtr.Zero) Marshal.FreeHGlobal(p);
            }
            return headerLength;
        }

        public static ReadOnlySpan<byte> PARSE_BYTE_ARRAY(ReadOnlySpan<byte> source, MESSAGE_FRAME_TYPE_T frameType, MESSAGE_DATA_CODE_T dataCode,
            out COMMANDS_T command, out ushort subcommand)
        {
            ushort cmd = 0;
            int headerLength = 0;
            IntPtr p = IntPtr.Zero;
            try
            {
                switch (frameType, dataCode)
                {
                    case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.ASCII):
                    case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.ASCII):
                        headerLength = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_ASCII_T>();
                        if (source.Length < headerLength)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.COMMAND_MESSAGE_CORRUPTED);
                        p = Marshal.AllocHGlobal(headerLength);
                        unsafe
                        {
                            Span<byte> sp = new Span<byte>((void*)p, headerLength);
                            source.Slice(0, headerLength).CopyTo(sp);
                        }
                        REQUEST_COMMAND_HEADER_IN_3E_ASCII_T header3a = Marshal.PtrToStructure<REQUEST_COMMAND_HEADER_IN_3E_ASCII_T>(p);
                        Message.Message.ASCII_ARRAY_TO_BINARY(header3a.command, 0, out cmd);
                        Message.Message.ASCII_ARRAY_TO_BINARY(header3a.subcommand, 0, out subcommand);
                        break;
                    case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.BINARY):
                    case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.BINARY):
                        headerLength = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>();
                        if (source.Length < headerLength)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.COMMAND_MESSAGE_CORRUPTED);
                        p = Marshal.AllocHGlobal(headerLength);
                        unsafe
                        {
                            Span<byte> sp = new Span<byte>((void*)p, headerLength);
                            source.Slice(0, headerLength).CopyTo(sp);
                        }
                        REQUEST_COMMAND_HEADER_IN_3E_BINARY_T header3b = Marshal.PtrToStructure<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>(p);
                        cmd = Message.Message.SMALL_ENDIAN_MODE(header3b.command);
                        subcommand = Message.Message.SMALL_ENDIAN_MODE(header3b.subcommand);
                        break;
                    case (MESSAGE_FRAME_TYPE_T.STATION_NUM_EXTENSION, MESSAGE_DATA_CODE_T.BINARY):
                        headerLength = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_EX_BINARY_T>();
                        if (source.Length < headerLength)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.COMMAND_MESSAGE_CORRUPTED);
                        p = Marshal.AllocHGlobal(headerLength);
                        unsafe
                        {
                            Span<byte> sp = new Span<byte>((void*)p, headerLength);
                            source.Slice(0, headerLength).CopyTo(sp);
                        }
                        REQUEST_COMMAND_HEADER_IN_EX_BINARY_T headerex = Marshal.PtrToStructure<REQUEST_COMMAND_HEADER_IN_EX_BINARY_T>(p);
                        cmd = Message.Message.SMALL_ENDIAN_MODE(headerex.command);
                        subcommand = Message.Message.SMALL_ENDIAN_MODE(headerex.subcommand);
                        break;
                    default:
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_SUBHEADER);
                }
            }
            finally
            {
                if (p != IntPtr.Zero) Marshal.FreeHGlobal(p);
            }
            if (Enum.IsDefined(typeof(COMMANDS_T), cmd) == false)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_COMMAND_CODE);
            command = (COMMANDS_T)cmd;
            return source.Slice(headerLength, source.Length - headerLength);
        }
    }

    public class ResponseCommand
    {
        private static byte[] __FIXED_VALUE = new byte[] { 0x00 };
        private static byte[] __SYSTEM_AREA = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 };

        public static int BUILD_BYTE_ARRAY_HEADER(MESSAGE_FRAME_TYPE_T frameType, MESSAGE_DATA_CODE_T dataCode, COMMANDS_T command, SUB_COMMANDS_T subcommand,
            ReadOnlySpan<byte> errorInformation,
            byte[] dataArray, int startIndex)
        {
            int headerLength = 0;
            IntPtr p = IntPtr.Zero;
            try
            {
                switch (frameType, dataCode)
                {
                    case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.ASCII):
                    case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.ASCII):
                        if (errorInformation == null)
                            headerLength = 0;
                        else
                        {
                            headerLength = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_ASCII_T>();
                            if (dataArray.Length - startIndex < headerLength + errorInformation.Length)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            REQUEST_COMMAND_HEADER_IN_3E_ASCII_T header3a = new REQUEST_COMMAND_HEADER_IN_3E_ASCII_T(0x30);
                            Message.Message.BINARY_TO_ASCII_ARRAY((ushort)command, header3a.command, 0);
                            Message.Message.BINARY_TO_ASCII_ARRAY((ushort)subcommand, header3a.subcommand, 0);
                            p = Marshal.AllocHGlobal(headerLength);
                            Marshal.StructureToPtr(header3a, p, false);
                            errorInformation.CopyTo(dataArray.AsSpan(startIndex));
                            Marshal.Copy(p, dataArray, startIndex + errorInformation.Length, headerLength);
                            headerLength += errorInformation.Length;
                        }
                        break;
                    case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.BINARY):
                    case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.BINARY):
                        if (errorInformation == null)
                            headerLength = 0;
                        else
                        {
                            headerLength = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>();
                            if (dataArray.Length - startIndex < headerLength + errorInformation.Length)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            REQUEST_COMMAND_HEADER_IN_3E_BINARY_T header3b = new REQUEST_COMMAND_HEADER_IN_3E_BINARY_T(0x00);
                            header3b.command = Message.Message.SMALL_ENDIAN_MODE((ushort)command);
                            header3b.subcommand = Message.Message.SMALL_ENDIAN_MODE((ushort)subcommand);
                            p = Marshal.AllocHGlobal(headerLength);
                            Marshal.StructureToPtr(header3b, p, false);
                            errorInformation.CopyTo(dataArray.AsSpan(startIndex));
                            Marshal.Copy(p, dataArray, startIndex + errorInformation.Length, headerLength);
                            headerLength += errorInformation.Length;
                        }
                        break;
                    case (MESSAGE_FRAME_TYPE_T.STATION_NUM_EXTENSION, MESSAGE_DATA_CODE_T.BINARY):
                        headerLength = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_EX_BINARY_T>();
                        if (dataArray.Length - startIndex < headerLength + (errorInformation == null ? 0 : errorInformation.Length))
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                        REQUEST_COMMAND_HEADER_IN_EX_BINARY_T headerex = new REQUEST_COMMAND_HEADER_IN_EX_BINARY_T(0x00);
                        headerex.command = Message.Message.SMALL_ENDIAN_MODE((ushort)command);
                        headerex.subcommand = Message.Message.SMALL_ENDIAN_MODE((ushort)subcommand);
                        p = Marshal.AllocHGlobal(headerLength);
                        Marshal.StructureToPtr(headerex, p, false);
                        Marshal.Copy(p, dataArray, startIndex, headerLength);
                        if (errorInformation != null)
                        {
                            errorInformation.CopyTo(dataArray.AsSpan(headerLength));
                            headerLength += errorInformation.Length;
                        }
                        break;
                    default:
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_SUBHEADER);
                }
            }
            finally
            {
                if (p != IntPtr.Zero) Marshal.FreeHGlobal(p);
            }
            return headerLength;
        }

        public static ReadOnlySpan<byte> PARSE_BYTE_ARRAY(ReadOnlySpan<byte> source, MESSAGE_FRAME_TYPE_T frameType, MESSAGE_DATA_CODE_T dataCode,
            out COMMANDS_T command, out ushort subcommand)
        {
            ushort cmd = 0;
            int headerLength = 0;
            IntPtr p = IntPtr.Zero;
            switch (frameType, dataCode)
            {
                case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.ASCII):
                case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.ASCII):
                case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.BINARY):
                case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.BINARY):
                    headerLength = 0;
                    command = COMMANDS_T.UNKNOWN;
                    subcommand = 0;
                    break;
                case (MESSAGE_FRAME_TYPE_T.STATION_NUM_EXTENSION, MESSAGE_DATA_CODE_T.BINARY):
                    headerLength = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_EX_BINARY_T>();
                    if (source.Length < headerLength)
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.COMMAND_MESSAGE_CORRUPTED);
                    p = Marshal.AllocHGlobal(headerLength);
                    unsafe
                    {
                        Span<byte> sp = new Span<byte>((void*)p, headerLength);
                        source.Slice(0, headerLength).CopyTo(sp);
                    }
                    REQUEST_COMMAND_HEADER_IN_EX_BINARY_T headerex = Marshal.PtrToStructure<REQUEST_COMMAND_HEADER_IN_EX_BINARY_T>(p);
                    cmd = Message.Message.SMALL_ENDIAN_MODE(headerex.command);
                    subcommand = Message.Message.SMALL_ENDIAN_MODE(headerex.subcommand);
                    Marshal.FreeHGlobal(p);
                    if (Enum.IsDefined(typeof(COMMANDS_T), cmd) == false)
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_COMMAND_CODE);
                    command = (COMMANDS_T)cmd;
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_SUBHEADER);
            }
            return source.Slice(headerLength, source.Length - headerLength);
        }
    }
}
