using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Message
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct REQUEST_MESSAGE_HEADER_IN_3E_ASCII_T
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] subheader_fixed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] network;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] station;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] module_io;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] multidrop_station;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] request_length;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] monitoring_timer;

        public REQUEST_MESSAGE_HEADER_IN_3E_ASCII_T(byte dummy = 0x30)
        {
            subheader_fixed = new byte[4] { 0x35, 0x30, 0x30, 0x30 };
            network = new byte[2] { dummy, dummy };
            station = new byte[2] { dummy, dummy };
            module_io = new byte[4] { dummy, dummy, dummy, dummy };
            multidrop_station = new byte[2] { dummy, dummy };
            request_length = new byte[4] { dummy, dummy, dummy, dummy };
            monitoring_timer = new byte[4] { dummy, dummy, dummy, dummy };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct REQUEST_MESSAGE_HEADER_IN_4E_ASCII_T
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] subheader_fixed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] serial_number;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dummy_fixed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] network;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] station;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] module_io;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] multidrop_station;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] request_length;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] monitoring_timer;

        public REQUEST_MESSAGE_HEADER_IN_4E_ASCII_T(byte dummy = 0x30)
        {
            subheader_fixed = new byte[4] { 0x35, 0x34, 0x30, 0x30 };
            serial_number = new byte[4] { 0x30, 0x30, 0x30, 0x30 };
            dummy_fixed = new byte[4] { 0x30, 0x30, 0x30, 0x30 };
            network = new byte[2] { dummy, dummy };
            station = new byte[2] { dummy, dummy };
            module_io = new byte[4] { dummy, dummy, dummy, dummy };
            multidrop_station = new byte[2] { dummy, dummy };
            request_length = new byte[4] { dummy, dummy, dummy, dummy };
            monitoring_timer = new byte[4] { dummy, dummy, dummy, dummy };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct REQUEST_MESSAGE_HEADER_IN_3E_BINARY_T
    {
        public ushort subheader_fixed;
        public byte network;
        public byte station;
        public ushort module_io;
        public byte multidrop_station;
        public ushort request_length;
        public ushort monitoring_timer;

        public REQUEST_MESSAGE_HEADER_IN_3E_BINARY_T(byte dummy = 0x00)
        {
            subheader_fixed = Message.SMALL_ENDIAN_MODE(0x50);
            network = dummy;
            station = dummy;
            module_io = Message.SMALL_ENDIAN_MODE(dummy);
            multidrop_station = dummy;
            request_length = Message.SMALL_ENDIAN_MODE(dummy);
            monitoring_timer = Message.SMALL_ENDIAN_MODE(dummy);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct REQUEST_MESSAGE_HEADER_IN_4E_BINARY_T
    {
        public ushort subheader_fixed;
        public ushort serial_number;
        public ushort dummy_fixed;
        public byte network;
        public byte station;
        public ushort module_io;
        public byte multidrop_station;
        public ushort request_length;
        public ushort monitoring_timer;

        public REQUEST_MESSAGE_HEADER_IN_4E_BINARY_T(byte dummy = 0x00)
        {
            subheader_fixed = Message.SMALL_ENDIAN_MODE(0x54);
            serial_number = Message.SMALL_ENDIAN_MODE(dummy);
            dummy_fixed = Message.SMALL_ENDIAN_MODE(dummy);
            network = dummy;
            station = dummy;
            module_io = Message.SMALL_ENDIAN_MODE(dummy);
            multidrop_station = dummy;
            request_length = Message.SMALL_ENDIAN_MODE(dummy);
            monitoring_timer = Message.SMALL_ENDIAN_MODE(dummy);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct REQUEST_MESSAGE_HEADER_IN_EX_BINARY_T
    {
        public ushort subheader_fixed;
        public ushort serial_number;
        public ushort dummy_fixed0;
        public byte network;
        public byte station;
        public ushort module_io;
        public byte multidrop_station;
        public byte dummy_fixed1;
        public ushort extension_station;
        public ushort request_length;
        public ushort monitoring_timer;

        public REQUEST_MESSAGE_HEADER_IN_EX_BINARY_T(byte dummy = 0x00)
        {
            subheader_fixed = Message.SMALL_ENDIAN_MODE(0x68);
            serial_number = Message.SMALL_ENDIAN_MODE(dummy);
            dummy_fixed0 = Message.SMALL_ENDIAN_MODE(dummy);
            network = dummy;
            station = dummy;
            module_io = Message.SMALL_ENDIAN_MODE(dummy);
            multidrop_station = dummy;
            dummy_fixed1 = dummy;
            extension_station = Message.SMALL_ENDIAN_MODE(dummy);
            request_length = Message.SMALL_ENDIAN_MODE(dummy);
            monitoring_timer = Message.SMALL_ENDIAN_MODE(dummy);
        }
    }

    public class RequestMessage
    {
        private static readonly byte[] __MC3E_ASCII_SUBHEADER = new byte[4] { 0x35, 0x30, 0x30, 0x30 };
        private static readonly byte[] __MC3E_BINARY_SUBHEADER = new byte[2] { 0x50, 0x00 };
        private static readonly byte[] __MC4E_ASCII_SUBHEADER = new byte[12] { 0x35, 0x34, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };
        private static readonly byte[] __MC4E_BINARY_SUBHEADER = new byte[6] { 0x54, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private static readonly byte[] __STATION_EXTENSION_SUBHEADER = new byte[6] { 0x68, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public static int BUILD_BYTE_ARRAY_HEADER(MESSAGE_FRAME_TYPE_T frameType, MESSAGE_DATA_CODE_T dataCode, ushort serialNo,
           byte destinationNetwork, byte destinationStation, ushort destinationModuleIO, byte destinationMultidropStation, ushort destinationExtensionStation,
           ushort requestLength, ushort monitoringTimer, byte[] dataArray, int startIndex)
        {
            int headerLength = 0;
            IntPtr p = IntPtr.Zero;
            try
            {
                switch (frameType, dataCode)
                {
                    case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.ASCII):
                        headerLength = Marshal.SizeOf<REQUEST_MESSAGE_HEADER_IN_3E_ASCII_T>();
                        if (headerLength > dataArray.Length - startIndex)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

                        REQUEST_MESSAGE_HEADER_IN_3E_ASCII_T header3a = new REQUEST_MESSAGE_HEADER_IN_3E_ASCII_T(0x30);
                        Message.BINARY_TO_ASCII_ARRAY(destinationNetwork, header3a.network, 0);
                        Message.BINARY_TO_ASCII_ARRAY(destinationStation, header3a.station, 0);
                        Message.BINARY_TO_ASCII_ARRAY(destinationModuleIO, header3a.module_io, 0);
                        Message.BINARY_TO_ASCII_ARRAY(destinationMultidropStation, header3a.multidrop_station, 0);
                        Message.BINARY_TO_ASCII_ARRAY(monitoringTimer, header3a.monitoring_timer, 0);
                        Message.BINARY_TO_ASCII_ARRAY((ushort)(requestLength + 4), header3a.request_length, 0);

                        p = Marshal.AllocHGlobal(headerLength);
                        Marshal.StructureToPtr(header3a, p, false);
                        break;
                    case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.BINARY):
                        headerLength = Marshal.SizeOf<REQUEST_MESSAGE_HEADER_IN_3E_BINARY_T>();
                        if (headerLength > dataArray.Length - startIndex)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

                        REQUEST_MESSAGE_HEADER_IN_3E_BINARY_T header3b = new REQUEST_MESSAGE_HEADER_IN_3E_BINARY_T(0x00);
                        header3b.network = destinationNetwork;
                        header3b.station = destinationStation;
                        header3b.module_io = Message.SMALL_ENDIAN_MODE(destinationModuleIO);
                        header3b.multidrop_station = destinationMultidropStation;
                        header3b.monitoring_timer = Message.SMALL_ENDIAN_MODE(monitoringTimer);
                        header3b.request_length = Message.SMALL_ENDIAN_MODE((ushort)(requestLength + 2));

                        p = Marshal.AllocHGlobal(headerLength);
                        Marshal.StructureToPtr(header3b, p, false);
                        break;
                    case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.ASCII):
                        headerLength = Marshal.SizeOf<REQUEST_MESSAGE_HEADER_IN_4E_ASCII_T>();
                        if (headerLength > dataArray.Length - startIndex)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

                        REQUEST_MESSAGE_HEADER_IN_4E_ASCII_T header4a = new REQUEST_MESSAGE_HEADER_IN_4E_ASCII_T(0x30);
                        Message.BINARY_TO_ASCII_ARRAY(serialNo, header4a.serial_number, 0);
                        Message.BINARY_TO_ASCII_ARRAY(destinationNetwork, header4a.network, 0);
                        Message.BINARY_TO_ASCII_ARRAY(destinationStation, header4a.station, 0);
                        Message.BINARY_TO_ASCII_ARRAY(destinationModuleIO, header4a.module_io, 0);
                        Message.BINARY_TO_ASCII_ARRAY(destinationMultidropStation, header4a.multidrop_station, 0);
                        Message.BINARY_TO_ASCII_ARRAY(monitoringTimer, header4a.monitoring_timer, 0);
                        Message.BINARY_TO_ASCII_ARRAY((ushort)(requestLength + 4), header4a.request_length, 0);

                        p = Marshal.AllocHGlobal(headerLength);
                        Marshal.StructureToPtr(header4a, p, false);
                        break;
                    case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.BINARY):
                        headerLength = Marshal.SizeOf<REQUEST_MESSAGE_HEADER_IN_4E_BINARY_T>();
                        if (headerLength > dataArray.Length - startIndex)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

                        REQUEST_MESSAGE_HEADER_IN_4E_BINARY_T header4b = new REQUEST_MESSAGE_HEADER_IN_4E_BINARY_T(0x00);
                        header4b.serial_number = Message.SMALL_ENDIAN_MODE(serialNo);
                        header4b.network = destinationNetwork;
                        header4b.station = destinationStation;
                        header4b.module_io = Message.SMALL_ENDIAN_MODE(destinationModuleIO);
                        header4b.multidrop_station = destinationMultidropStation;
                        header4b.monitoring_timer = Message.SMALL_ENDIAN_MODE(monitoringTimer);
                        header4b.request_length = Message.SMALL_ENDIAN_MODE((ushort)(requestLength + 2));

                        p = Marshal.AllocHGlobal(headerLength);
                        Marshal.StructureToPtr(header4b, p, false);

                        break;
                    case (MESSAGE_FRAME_TYPE_T.STATION_NUM_EXTENSION, MESSAGE_DATA_CODE_T.BINARY):
                        headerLength = Marshal.SizeOf<REQUEST_MESSAGE_HEADER_IN_EX_BINARY_T>();
                        if (headerLength > dataArray.Length - startIndex)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

                        REQUEST_MESSAGE_HEADER_IN_EX_BINARY_T headerex = new REQUEST_MESSAGE_HEADER_IN_EX_BINARY_T(0x00);
                        headerex.serial_number = Message.SMALL_ENDIAN_MODE(serialNo);
                        headerex.network = destinationNetwork;
                        headerex.station = destinationStation;
                        headerex.module_io = Message.SMALL_ENDIAN_MODE(destinationModuleIO);
                        headerex.multidrop_station = destinationMultidropStation;
                        headerex.monitoring_timer = Message.SMALL_ENDIAN_MODE(monitoringTimer);
                        headerex.extension_station = Message.SMALL_ENDIAN_MODE(destinationExtensionStation);
                        headerex.request_length = Message.SMALL_ENDIAN_MODE((ushort)(requestLength + 2));

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
                if(p != IntPtr.Zero) Marshal.FreeHGlobal(p);
            }
            return headerLength;
        }

        public static ReadOnlySpan<byte> PARSE_BYTE_ARRAY(ReadOnlySpan<byte> source,  MESSAGE_DATA_CODE_T dataCode,
            out MESSAGE_FRAME_TYPE_T frameType,
            out byte destinationNetwork, out byte destinationStation, out ushort destinationModuleIO, out byte destinationMultidropStation, out ushort destinationExtensionStation,
            out ushort serialNo, out ushort monitoringTimer)
        {
            switch(dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    if (source.StartsWith(__MC3E_ASCII_SUBHEADER))
                        frameType = MESSAGE_FRAME_TYPE_T.MC_3E;
                    else if (source.StartsWith(__MC4E_ASCII_SUBHEADER.AsSpan(0,4)) && source.Slice(8).StartsWith(__MC4E_ASCII_SUBHEADER.AsSpan(8)))
                        frameType = MESSAGE_FRAME_TYPE_T.MC_4E;
                    else
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_FRAME_TYPE);
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    if (source.StartsWith(__MC3E_BINARY_SUBHEADER))
                        frameType = MESSAGE_FRAME_TYPE_T.MC_3E;
                    else if (source.StartsWith(__MC4E_BINARY_SUBHEADER.AsSpan(0, 2)) && source.Slice(4).StartsWith(__MC4E_BINARY_SUBHEADER.AsSpan(4)))
                        frameType = MESSAGE_FRAME_TYPE_T.MC_4E;
                    else if (source.StartsWith(__STATION_EXTENSION_SUBHEADER.AsSpan(0, 2)) && source.Slice(4).StartsWith(__STATION_EXTENSION_SUBHEADER.AsSpan(4)))
                        frameType = MESSAGE_FRAME_TYPE_T.STATION_NUM_EXTENSION;
                    else
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_FRAME_TYPE);
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            int headerLength = 0;
            ushort requestDataLength = 0;
            IntPtr p = IntPtr.Zero;
            try
            {
                switch (frameType, dataCode)
                {
                    case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.ASCII):
                        headerLength = Marshal.SizeOf<REQUEST_MESSAGE_HEADER_IN_3E_ASCII_T>();
                        if (headerLength > source.Length)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.MESSAGE_FRAME_CORRUPTED);

                        p = Marshal.AllocHGlobal(headerLength);
                        unsafe
                        {
                            Span<byte> sp = new Span<byte>((void*)p, headerLength);
                            source.Slice(0, headerLength).CopyTo(sp);
                        }
                        REQUEST_MESSAGE_HEADER_IN_3E_ASCII_T header3a = Marshal.PtrToStructure<REQUEST_MESSAGE_HEADER_IN_3E_ASCII_T>(p);
                        Message.ASCII_ARRAY_TO_BINARY(header3a.network, 0, out destinationNetwork);
                        Message.ASCII_ARRAY_TO_BINARY(header3a.station, 0, out destinationStation);
                        Message.ASCII_ARRAY_TO_BINARY(header3a.module_io, 0, out destinationModuleIO);
                        Message.ASCII_ARRAY_TO_BINARY(header3a.multidrop_station, 0, out destinationMultidropStation);
                        Message.ASCII_ARRAY_TO_BINARY(header3a.request_length, 0, out requestDataLength);
                        Message.ASCII_ARRAY_TO_BINARY(header3a.monitoring_timer, 0, out monitoringTimer);
                        requestDataLength -= 4;
                        destinationExtensionStation = 0;
                        serialNo = 0;
                        break;
                    case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.BINARY):
                        headerLength = Marshal.SizeOf<REQUEST_MESSAGE_HEADER_IN_3E_BINARY_T>();
                        if (headerLength > source.Length)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.MESSAGE_FRAME_CORRUPTED);

                        p = Marshal.AllocHGlobal(headerLength);
                        unsafe
                        {
                            Span<byte> sp = new Span<byte>((void*)p, headerLength);
                            source.Slice(0, headerLength).CopyTo(sp);
                        }
                        REQUEST_MESSAGE_HEADER_IN_3E_BINARY_T header3b = Marshal.PtrToStructure<REQUEST_MESSAGE_HEADER_IN_3E_BINARY_T>(p);
                        destinationNetwork = header3b.network;
                        destinationStation = header3b.station;
                        destinationModuleIO = Message.SMALL_ENDIAN_MODE(header3b.module_io);
                        destinationMultidropStation = header3b.multidrop_station;
                        requestDataLength = (ushort)(Message.SMALL_ENDIAN_MODE(header3b.request_length) - 2);
                        monitoringTimer = Message.SMALL_ENDIAN_MODE(header3b.monitoring_timer);
                        destinationExtensionStation = 0;
                        serialNo = 0;
                        break;
                    case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.ASCII):
                        headerLength = Marshal.SizeOf<REQUEST_MESSAGE_HEADER_IN_4E_ASCII_T>();
                        if (headerLength > source.Length)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.MESSAGE_FRAME_CORRUPTED);

                        p = Marshal.AllocHGlobal(headerLength);
                        unsafe
                        {
                            Span<byte> sp = new Span<byte>((void*)p, headerLength);
                            source.Slice(0, headerLength).CopyTo(sp);
                        }
                        REQUEST_MESSAGE_HEADER_IN_4E_ASCII_T header4a = Marshal.PtrToStructure<REQUEST_MESSAGE_HEADER_IN_4E_ASCII_T>(p);

                        Message.ASCII_ARRAY_TO_BINARY(header4a.serial_number, 0, out serialNo);
                        Message.ASCII_ARRAY_TO_BINARY(header4a.network, 0, out destinationNetwork);
                        Message.ASCII_ARRAY_TO_BINARY(header4a.station, 0, out destinationStation);
                        Message.ASCII_ARRAY_TO_BINARY(header4a.module_io, 0, out destinationModuleIO);
                        Message.ASCII_ARRAY_TO_BINARY(header4a.multidrop_station, 0, out destinationMultidropStation);
                        Message.ASCII_ARRAY_TO_BINARY(header4a.request_length, 0, out requestDataLength);
                        Message.ASCII_ARRAY_TO_BINARY(header4a.monitoring_timer, 0, out monitoringTimer);
                        requestDataLength -= 4;
                        destinationExtensionStation = 0;
                        break;
                    case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.BINARY):
                        headerLength = Marshal.SizeOf<REQUEST_MESSAGE_HEADER_IN_4E_BINARY_T>();
                        if (headerLength > source.Length)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.MESSAGE_FRAME_CORRUPTED);

                        p = Marshal.AllocHGlobal(headerLength);
                        unsafe
                        {
                            Span<byte> sp = new Span<byte>((void*)p, headerLength);
                            source.Slice(0, headerLength).CopyTo(sp);
                        }
                        REQUEST_MESSAGE_HEADER_IN_4E_BINARY_T header4b = Marshal.PtrToStructure<REQUEST_MESSAGE_HEADER_IN_4E_BINARY_T>(p);

                        serialNo = Message.SMALL_ENDIAN_MODE(header4b.serial_number);
                        destinationNetwork = header4b.network;
                        destinationStation = header4b.station;
                        destinationModuleIO = Message.SMALL_ENDIAN_MODE(header4b.module_io);
                        destinationMultidropStation = header4b.multidrop_station;
                        requestDataLength = (ushort)(Message.SMALL_ENDIAN_MODE(header4b.request_length) - 2);
                        monitoringTimer = Message.SMALL_ENDIAN_MODE(header4b.monitoring_timer);
                        destinationExtensionStation = 0;
                        break;
                    case (MESSAGE_FRAME_TYPE_T.STATION_NUM_EXTENSION, MESSAGE_DATA_CODE_T.BINARY):
                        headerLength = Marshal.SizeOf<REQUEST_MESSAGE_HEADER_IN_EX_BINARY_T>();
                        if (headerLength > source.Length)
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.MESSAGE_FRAME_CORRUPTED);

                        p = Marshal.AllocHGlobal(headerLength);
                        unsafe
                        {
                            Span<byte> sp = new Span<byte>((void*)p, headerLength);
                            source.Slice(0, headerLength).CopyTo(sp);
                        }
                        REQUEST_MESSAGE_HEADER_IN_EX_BINARY_T headerex = Marshal.PtrToStructure<REQUEST_MESSAGE_HEADER_IN_EX_BINARY_T>(p);

                        serialNo = Message.SMALL_ENDIAN_MODE(headerex.serial_number);
                        destinationNetwork = headerex.network;
                        destinationStation = headerex.station;
                        destinationModuleIO = Message.SMALL_ENDIAN_MODE(headerex.module_io);
                        destinationMultidropStation = headerex.multidrop_station;
                        requestDataLength = (ushort)(Message.SMALL_ENDIAN_MODE(headerex.request_length) - 2);
                        monitoringTimer = Message.SMALL_ENDIAN_MODE(headerex.monitoring_timer);
                        destinationExtensionStation = Message.SMALL_ENDIAN_MODE(headerex.extension_station);
                        break;
                    default:
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_SUBHEADER);
                }
            }
            finally
            {
                if (p != IntPtr.Zero) Marshal.FreeHGlobal(p);
            }
            if (source.Length - headerLength < requestDataLength)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.MESSAGE_FRAME_CORRUPTED);
            return source.Slice(headerLength, requestDataLength); 
        }
    }
}
