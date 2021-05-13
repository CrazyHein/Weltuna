using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Command
{
    public enum DEVICE_ACCESS_RANGE_T
    {
        DECIMAL,
        HEXADECIMAL
    }

    public enum DEVICE_ACCESS_TYPE_T
    {
        BIT,
        WORD,
        DWORD
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DEVICE_SPECIFICATION_IN_QL_ASCII_T
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] device_code;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] head_device;

        public DEVICE_SPECIFICATION_IN_QL_ASCII_T(byte dummy = 0x30)
        {
            device_code = new byte[] { dummy, dummy };
            head_device = new byte[] { dummy, dummy, dummy, dummy, dummy, dummy };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DEVICE_SPECIFICATION_IN_R_ASCII_T
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] device_code;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] head_device;

        public DEVICE_SPECIFICATION_IN_R_ASCII_T(byte dummy = 0x30)
        {
            device_code = new byte[] { dummy, dummy, dummy, dummy };
            head_device = new byte[] { dummy, dummy, dummy, dummy, dummy, dummy, dummy, dummy };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DEVICE_SPECIFICATION_IN_QL_BINARY_T
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] head_device;
        public byte device_code;

        public DEVICE_SPECIFICATION_IN_QL_BINARY_T(byte dummy = 0x00)
        {
            head_device = new byte[] { dummy, dummy, dummy };
            device_code = dummy;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DEVICE_SPECIFICATION_IN_R_BINARY_T
    {
        public uint head_device;
        public ushort device_code;

        public DEVICE_SPECIFICATION_IN_R_BINARY_T(byte dummy = 0x00)
        {
            head_device = Message.Message.SMALL_ENDIAN_MODE(dummy);
            device_code = Message.Message.SMALL_ENDIAN_MODE(dummy);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] indirect_specification;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] extension_specification;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] extension_specification_modification;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] device_code;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] head_device;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] device_modification;

        public DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T(byte dummy = 0x30)
        {
            indirect_specification = new byte[] { dummy, dummy };
            extension_specification = new byte[] { dummy, dummy, dummy, dummy };
            extension_specification_modification = new byte[] { dummy, dummy, dummy };
            device_code = new byte[] { dummy, dummy };
            head_device = new byte[] { dummy, dummy, dummy, dummy, dummy, dummy };
            device_modification = new byte[] { dummy, dummy, dummy };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] indirect_specification;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] extension_specification;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] extension_specification_modification;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] device_code;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] head_device;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] device_modification;

        public DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T(byte dummy = 0x30)
        {
            indirect_specification = new byte[] { dummy, dummy };
            extension_specification = new byte[] { dummy, dummy, dummy, dummy };
            extension_specification_modification = new byte[] { dummy, dummy, dummy, dummy };
            device_code = new byte[] { dummy, dummy, dummy, dummy };
            head_device = new byte[] { dummy, dummy, dummy, dummy, dummy, dummy, dummy, dummy, dummy, dummy };
            device_modification = new byte[] { dummy, dummy, dummy, dummy };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T
    {
        public ushort device_modification;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] head_device;
        public byte device_code;
        public ushort extension_specification_modification;
        public ushort extension_specification;
        public byte direct_memory_specification;

        public DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T(byte dummy = 0x00)
        {
            device_modification = dummy;
            head_device = new byte[] { dummy, dummy, dummy };
            device_code = dummy;
            extension_specification_modification = dummy;
            extension_specification = dummy;
            direct_memory_specification = dummy;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T
    {
        public ushort device_modification;
        public uint head_device;
        public ushort device_code;
        public ushort extension_specification_modification;
        public ushort extension_specification;
        public byte direct_memory_specification;

        public DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T(byte dummy = 0x00)
        {
            device_modification = dummy;
            head_device = dummy;
            device_code = dummy;
            extension_specification_modification = dummy;
            extension_specification = dummy;
            direct_memory_specification = dummy;
        }
    }

    public enum DIRECT_MEMORY_SPECIFICATION_T : byte
    {
        LINK_DIRECT_DEVICE = 0xF9,
        MODULE_ACCESS_DEVICE = 0xF8,
        CPU_BUFFER_MEMORY_ACCESS_DEVICE = 0xFA,
    }

    [Flags]
    public enum DEVICE_ACCESS_MODIFICATION : ushort
    {
        INDIRECT_SPECIFICATION = 0x0008,
        Z_DEVICE_MODIFICATION = 0x0040,
        LZ_DEVICE_MODIFICATION = 0x0080
    }

    public class DeviceAccess
    {
        private static Dictionary<string, (byte[], DEVICE_ACCESS_RANGE_T, DEVICE_ACCESS_TYPE_T)> __DEVICE_ASCII_CODE_FOR_QL_MODULE;
        private static Dictionary<string, (byte[], DEVICE_ACCESS_RANGE_T, DEVICE_ACCESS_TYPE_T)> __DEVICE_ASCII_CODE_FOR_R_MODULE;
        private static Dictionary<string, (byte, DEVICE_ACCESS_RANGE_T, DEVICE_ACCESS_TYPE_T)> __DEVICE_BINARY_CODE_FOR_QL_MODULE;
        private static Dictionary<string, (ushort, DEVICE_ACCESS_RANGE_T, DEVICE_ACCESS_TYPE_T)> __DEVICE_BINARY_CODE_FOR_R_MODULE;

        private static Regex __EXTENSION_SPECIFICATION_PATTERN = new Regex(@"^[UJ][0-9,A-F]{3}$", RegexOptions.Compiled);
        private static Regex __QL_EXTENSION_MODIFICATION_PATTERN = new Regex(@"^[Z][0-9]{2}$", RegexOptions.Compiled);
        private static Regex __R_EXTENSION_MODIFICATION_PATTERN = new Regex(@"^(?:LZ|Z\x20)[0-9]{2}$",RegexOptions.Compiled);
        private static Regex __RZ_EXTENSION_MODIFICATION_PATTERN = new Regex(@"^Z\x20[0-9]{2}$", RegexOptions.Compiled);
        private static Regex __RLZ_EXTENSION_MODIFICATION_PATTERN = new Regex(@"^LZ[0-9]{2}$", RegexOptions.Compiled);

        static DeviceAccess()
        {
            __DEVICE_ASCII_CODE_FOR_QL_MODULE = new Dictionary<string, (byte[], DEVICE_ACCESS_RANGE_T, DEVICE_ACCESS_TYPE_T) > 
            { 
                {"SM",      (System.Text.Encoding.ASCII.GetBytes("SM"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
                {"SD",      (System.Text.Encoding.ASCII.GetBytes("SD"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
                {"X",       (System.Text.Encoding.ASCII.GetBytes("X*"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.BIT) },
                {"Y",       (System.Text.Encoding.ASCII.GetBytes("Y*"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.BIT) },
                {"M",       (System.Text.Encoding.ASCII.GetBytes("M*"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
                {"L",       (System.Text.Encoding.ASCII.GetBytes("L*"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
                {"F",       (System.Text.Encoding.ASCII.GetBytes("F*"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
                {"V",       (System.Text.Encoding.ASCII.GetBytes("V*"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
                {"B",       (System.Text.Encoding.ASCII.GetBytes("B*"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.BIT) },
                {"D",       (System.Text.Encoding.ASCII.GetBytes("D*"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
                {"W",       (System.Text.Encoding.ASCII.GetBytes("W*"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.WORD) },
                {"TS",      (System.Text.Encoding.ASCII.GetBytes("TS"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
                {"TC",      (System.Text.Encoding.ASCII.GetBytes("TC"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
                {"TN",      (System.Text.Encoding.ASCII.GetBytes("TN"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
                {"STS",     (System.Text.Encoding.ASCII.GetBytes("SS"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
                {"STC",     (System.Text.Encoding.ASCII.GetBytes("SC"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
                {"STN",     (System.Text.Encoding.ASCII.GetBytes("SN"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
                {"CS",      (System.Text.Encoding.ASCII.GetBytes("CS"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
                {"CC",      (System.Text.Encoding.ASCII.GetBytes("CC"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.BIT) },
                {"CN",      (System.Text.Encoding.ASCII.GetBytes("CN"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
                {"SB",      (System.Text.Encoding.ASCII.GetBytes("SB"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.BIT) },
                {"SW",      (System.Text.Encoding.ASCII.GetBytes("SW"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.WORD) },
                {"DX",      (System.Text.Encoding.ASCII.GetBytes("DX"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.BIT) },
                {"DY",      (System.Text.Encoding.ASCII.GetBytes("DY"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.BIT) },
                {"Z",       (System.Text.Encoding.ASCII.GetBytes("Z*"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
                {"R",       (System.Text.Encoding.ASCII.GetBytes("R*"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
                {"ZR",      (System.Text.Encoding.ASCII.GetBytes("ZR"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL,  DEVICE_ACCESS_TYPE_T.WORD) },
                {"G",       (System.Text.Encoding.ASCII.GetBytes("G*"), DEVICE_ACCESS_RANGE_T.DECIMAL,      DEVICE_ACCESS_TYPE_T.WORD) },
            };

            __DEVICE_ASCII_CODE_FOR_R_MODULE = new Dictionary<string, (byte[], DEVICE_ACCESS_RANGE_T, DEVICE_ACCESS_TYPE_T)>
            {
                {"SM",      (System.Text.Encoding.ASCII.GetBytes("SM**"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"SD",      (System.Text.Encoding.ASCII.GetBytes("SD**"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"X",       (System.Text.Encoding.ASCII.GetBytes("X***"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.BIT) },
                {"Y",       (System.Text.Encoding.ASCII.GetBytes("Y***"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.BIT) },
                {"M",       (System.Text.Encoding.ASCII.GetBytes("M***"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"L",       (System.Text.Encoding.ASCII.GetBytes("L***"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"F",       (System.Text.Encoding.ASCII.GetBytes("F***"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"V",       (System.Text.Encoding.ASCII.GetBytes("V***"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"B",       (System.Text.Encoding.ASCII.GetBytes("B***"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.BIT) },
                {"D",       (System.Text.Encoding.ASCII.GetBytes("D***"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"W",       (System.Text.Encoding.ASCII.GetBytes("W***"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.WORD) },
                {"TS",      (System.Text.Encoding.ASCII.GetBytes("TS**"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"TC",      (System.Text.Encoding.ASCII.GetBytes("TC**"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"TN",      (System.Text.Encoding.ASCII.GetBytes("TN**"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"LTS",     (System.Text.Encoding.ASCII.GetBytes("LTS*"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"LTC",     (System.Text.Encoding.ASCII.GetBytes("LTC*"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"LTN",     (System.Text.Encoding.ASCII.GetBytes("LTN*"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.DWORD) },
                {"STS",     (System.Text.Encoding.ASCII.GetBytes("STS*"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"STC",     (System.Text.Encoding.ASCII.GetBytes("STC*"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"STN",     (System.Text.Encoding.ASCII.GetBytes("STN*"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"LSTS",    (System.Text.Encoding.ASCII.GetBytes("LSTS"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"LSTC",    (System.Text.Encoding.ASCII.GetBytes("LSTC"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"LSTN",    (System.Text.Encoding.ASCII.GetBytes("LSTN"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.DWORD) },
                {"CS",      (System.Text.Encoding.ASCII.GetBytes("CS**"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"CC",      (System.Text.Encoding.ASCII.GetBytes("CC**"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"CN",      (System.Text.Encoding.ASCII.GetBytes("CN**"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"LCS",     (System.Text.Encoding.ASCII.GetBytes("LCS*"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"LCC",     (System.Text.Encoding.ASCII.GetBytes("LCC*"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"LCN",     (System.Text.Encoding.ASCII.GetBytes("LCN*"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.DWORD) },
                {"SB",      (System.Text.Encoding.ASCII.GetBytes("SB**"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.BIT) },
                {"SW",      (System.Text.Encoding.ASCII.GetBytes("SW**"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.WORD) },
                {"DX",      (System.Text.Encoding.ASCII.GetBytes("DX**"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.BIT) },
                {"DY",      (System.Text.Encoding.ASCII.GetBytes("DY**"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.BIT) },
                {"Z",       (System.Text.Encoding.ASCII.GetBytes("Z***"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"LZ",      (System.Text.Encoding.ASCII.GetBytes("LZ**"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.DWORD) },
                {"R",       (System.Text.Encoding.ASCII.GetBytes("R***"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"ZR",      (System.Text.Encoding.ASCII.GetBytes("ZR**"), DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.WORD) },
                {"RD",      (System.Text.Encoding.ASCII.GetBytes("RD**"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"G",       (System.Text.Encoding.ASCII.GetBytes("G***"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"HG",      (System.Text.Encoding.ASCII.GetBytes("HG**"), DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
            };

            __DEVICE_BINARY_CODE_FOR_QL_MODULE = new Dictionary<string, (byte, DEVICE_ACCESS_RANGE_T, DEVICE_ACCESS_TYPE_T)>
            {
                {"SM",      (0x91, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.BIT) },
                {"SD",      (0xA9, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.WORD) },
                {"X",       (0x9C, DEVICE_ACCESS_RANGE_T.HEXADECIMAL,   DEVICE_ACCESS_TYPE_T.BIT) },
                {"Y",       (0x9D, DEVICE_ACCESS_RANGE_T.HEXADECIMAL,   DEVICE_ACCESS_TYPE_T.BIT) },
                {"M",       (0x90, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.BIT) },
                {"L",       (0x92, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.BIT) },
                {"F",       (0x93, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.BIT) },
                {"V",       (0x94, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.BIT) },
                {"B",       (0xA0, DEVICE_ACCESS_RANGE_T.HEXADECIMAL,   DEVICE_ACCESS_TYPE_T.BIT) },
                {"D",       (0xA8, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.WORD) },
                {"W",       (0xB4, DEVICE_ACCESS_RANGE_T.HEXADECIMAL,   DEVICE_ACCESS_TYPE_T.WORD) },
                {"TS",      (0xC1, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.BIT) },
                {"TC",      (0xC0, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.BIT) },
                {"TN",      (0xC2, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.WORD) },
                {"STS",     (0xC7, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.BIT) },
                {"STC",     (0xC6, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.BIT) },
                {"STN",     (0xC8, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.WORD) },
                {"CS",      (0xC4, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.BIT) },
                {"CC",      (0xC3, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.BIT) },
                {"CN",      (0xC5, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.WORD) },
                {"SB",      (0xA1, DEVICE_ACCESS_RANGE_T.HEXADECIMAL,   DEVICE_ACCESS_TYPE_T.BIT) },
                {"SW",      (0xB5, DEVICE_ACCESS_RANGE_T.HEXADECIMAL,   DEVICE_ACCESS_TYPE_T.WORD) },
                {"DX",      (0xA2, DEVICE_ACCESS_RANGE_T.HEXADECIMAL,   DEVICE_ACCESS_TYPE_T.BIT) },
                {"DY",      (0xA3, DEVICE_ACCESS_RANGE_T.HEXADECIMAL,   DEVICE_ACCESS_TYPE_T.BIT) },
                {"Z",       (0xCC, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.WORD) },
                {"R",       (0xAF, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.WORD) },
                {"ZR",      (0xB0, DEVICE_ACCESS_RANGE_T.HEXADECIMAL,   DEVICE_ACCESS_TYPE_T.WORD) },
                {"G",       (0xAB, DEVICE_ACCESS_RANGE_T.DECIMAL,       DEVICE_ACCESS_TYPE_T.WORD) },
            };

            __DEVICE_BINARY_CODE_FOR_R_MODULE = new Dictionary<string, (ushort, DEVICE_ACCESS_RANGE_T, DEVICE_ACCESS_TYPE_T)>
            {
                {"SM",      (0x0091, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"SD",      (0x00A9, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"X",       (0x009C, DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.BIT) },
                {"Y",       (0x009D, DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.BIT) },
                {"M",       (0x0090, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"L",       (0x0092, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"F",       (0x0093, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"V",       (0x0094, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"B",       (0x00A0, DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.BIT) },
                {"D",       (0x00A8, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"W",       (0x00B4, DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.WORD) },
                {"TS",      (0x00C1, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"TC",      (0x00C0, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"TN",      (0x00C2, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"LTS",     (0x0051, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"LTC",     (0x0050, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"LTN",     (0x0052, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.DWORD) },
                {"STS",     (0x00C7, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"STC",     (0x00C6, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"STN",     (0x00C8, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"LSTS",    (0x0059, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"LSTC",    (0x0058, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"LSTN",    (0x005A, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.DWORD) },
                {"CS",      (0x00C4, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"CC",      (0x00C3, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"CN",      (0x00C5, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"LCS",     (0x0055, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"LCC",     (0x0054, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.BIT) },
                {"LCN",     (0x0056, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.DWORD) },
                {"SB",      (0x00A1, DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.BIT) },
                {"SW",      (0x00B5, DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.WORD) },
                {"DX",      (0x00A2, DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.BIT) },
                {"DY",      (0x00A3, DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.BIT) },
                {"Z",       (0x00CC, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"LZ",      (0x0062, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.DWORD) },
                {"R",       (0x00AF, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"ZR",      (0x00B0, DEVICE_ACCESS_RANGE_T.HEXADECIMAL, DEVICE_ACCESS_TYPE_T.WORD) },
                {"RD",      (0x002C, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"G",       (0x00AB, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
                {"HG",      (0x002E, DEVICE_ACCESS_RANGE_T.DECIMAL,     DEVICE_ACCESS_TYPE_T.WORD) },
            };
        }

        private static int __BINARY_TO_ASCII_ARRAY(uint value, DEVICE_ACCESS_RANGE_T range, Span<byte> array)
        {
            byte remainder = 0;
            switch (range)
            {
                case DEVICE_ACCESS_RANGE_T.DECIMAL:
                    for(int i = 1; i <= array.Length; ++i)
                    {
                        remainder = (byte)(value % 10);
                        array[^i] = (byte)(remainder + 0x30);
                        value /= 10;
                    }
                    break;
                case DEVICE_ACCESS_RANGE_T.HEXADECIMAL:
                    for (int i = 1; i <= array.Length; ++i)
                    {
                        remainder = (byte)(value % 16);
                        if(remainder < 10)
                            array[^i] = (byte)(remainder + 0x30);
                        else
                            array[^i] = (byte)(remainder - 10 + 0x41);
                        value /= 16;
                    }
                    break;
            }
            if (value != 0)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_ACCESS_OUT_OF_HEAD_RANGE);
            return array.Length;
        }

        private static int __BINARY_TO_BINARY_ARRAY(uint value, Span<byte> array)
        {
            byte remainder = 0;
            for (int i = 0; i < array.Length; ++i)
            {
                remainder = (byte)(value % 0x100);
                array[i] = remainder;
                value /= 0x100;
            }
            if (value != 0)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_ACCESS_OUT_OF_HEAD_RANGE);
            return array.Length;
        }

        public static int BUILD_DEVICE_READ_WRITE_BYTE_ARRAY_HEADER(MESSAGE_FRAME_TYPE_T frameType, MESSAGE_DATA_CODE_T dataCode, SUB_COMMANDS_T subcommand, string deviceCode, uint headDevice, ushort devicePoints,
                                                                string extension, string extensionModification, string deviceModification, string indirectSpecification,
                                                                byte[] dataArray, int startIndex)
        {
            int index = startIndex;
            switch(dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    if((subcommand & SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION) != 0)
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T>() + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII(deviceCode, headDevice, extension, extensionModification, deviceModification, indirectSpecification, dataArray, index);
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_ASCII_T>() + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += __BUILD_DEVICE_SPECIFICATION_IN_R_ASCII(deviceCode, headDevice, dataArray, index);
                        }
                    }
                    else
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T>() + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII(deviceCode, headDevice, extension, extensionModification, deviceModification, indirectSpecification, dataArray, index);
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_ASCII_T>() + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += __BUILD_DEVICE_SPECIFICATION_IN_QL_ASCII(deviceCode, headDevice, dataArray, index); 
                        }
                       
                    }
                    index += Message.Message.BINARY_TO_ASCII_ARRAY(devicePoints, dataArray, index);
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    if ((subcommand & SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION) != 0)
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T>() + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY(deviceCode, headDevice, extension, extensionModification, deviceModification, indirectSpecification, dataArray, index);
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_BINARY_T>() + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += __BUILD_DEVICE_SPECIFICATION_IN_R_BINARY(deviceCode, headDevice, dataArray, index);
                        }
                    }
                    else
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T>() + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY(deviceCode, headDevice, extension, extensionModification, deviceModification, indirectSpecification, dataArray, index);
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_BINARY_T>() + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += __BUILD_DEVICE_SPECIFICATION_IN_QL_BINARY(deviceCode, headDevice, dataArray, index);
                        } 
                    }
                    index += Message.Message.BINARY_TO_BINARY_ARRAY(devicePoints, dataArray, index);
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return index - startIndex;
        }

        public static int BUILD_DEVICE_READ_RANDOM_BYTE_ARRAY_HEADER(MESSAGE_FRAME_TYPE_T frameType, MESSAGE_DATA_CODE_T dataCode, SUB_COMMANDS_T subcommand,
                                                                    IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice)> devicein16,
                                                                    IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice)> devicein32,
                                                                    byte[] dataArray, int startIndex)
        {
            int index = startIndex;
            byte device32points = (byte)(devicein32 == null ? 0 : devicein32.Count()); ;
            byte device16points = (byte)(devicein16 == null ? 0 : devicein16.Count());
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    if ((subcommand & SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION) != 0)
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T>() * (device32points + device16points) + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device16points, dataArray, index);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device32points, dataArray, index);
                            if(devicein16 != null)
                                foreach(var d in devicein16)
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                            if (devicein32 != null)
                                foreach (var d in devicein32)
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_ASCII_T>() * (device32points + device16points) + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device16points, dataArray, index);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device32points, dataArray, index);
                            if (devicein16 != null)
                                foreach (var d in devicein16)
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                            if (devicein32 != null)
                                foreach (var d in devicein32)
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                        }
                    }
                    else
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T>() * (device32points + device16points) + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device16points, dataArray, index);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device32points, dataArray, index);
                            if (devicein16 != null)
                                foreach (var d in devicein16)
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                            if (devicein32 != null)
                                foreach (var d in devicein32)
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_ASCII_T>() * (device32points + device16points) + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device16points, dataArray, index);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device32points, dataArray, index);
                            if (devicein16 != null)
                                foreach (var d in devicein16)
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                            if (devicein32 != null)
                                foreach (var d in devicein32)
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                        }
                    }
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    if ((subcommand & SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION) != 0)
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T>() * (device32points + device16points) + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = device16points;
                            dataArray[index++] = device32points;
                            if (devicein16 != null)
                                foreach (var d in devicein16)
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                            if (devicein32 != null)
                                foreach (var d in devicein32)
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_BINARY_T>() * (device32points + device16points) + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = device16points;
                            dataArray[index++] = device32points;
                            if (devicein16 != null)
                                foreach (var d in devicein16)
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                            if (devicein32 != null)
                                foreach (var d in devicein32)
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                        }
                    }
                    else
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T>() * (device32points + device16points) + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = device16points;
                            dataArray[index++] = device32points;
                            if (devicein16 != null)
                                foreach (var d in devicein16)
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                            if (devicein32 != null)
                                foreach (var d in devicein32)
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_BINARY_T>() * (device32points + device16points) + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = device16points;
                            dataArray[index++] = device32points;
                            if (devicein16 != null)
                                foreach (var d in devicein16)
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                            if (devicein32 != null)
                                foreach (var d in devicein32)
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                        }
                    }
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return index - startIndex;
        }

        public static int BUILD_DEVICE_WRITE_RANDOM_BYTE_ARRAY_HEADER(MESSAGE_FRAME_TYPE_T frameType, MESSAGE_DATA_CODE_T dataCode, SUB_COMMANDS_T subcommand,
                                                                    IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice, byte value)> bitdevice,
                                                                    byte[] dataArray, int startIndex)
        {
            int index = startIndex;
            byte bitpoints = (byte)(bitdevice == null ? 0 : bitdevice.Count());
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    if ((subcommand & SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION) != 0)
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T>() + 2) * bitpoints + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(bitpoints, dataArray, index);
                            if (bitdevice != null)
                            {
                                foreach (var d in bitdevice)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY((ushort)d.value, dataArray, index);
                                }
                            }
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_ASCII_T>() + 2) * bitpoints + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(bitpoints, dataArray, index);
                            if (bitdevice != null)
                            {
                                foreach (var d in bitdevice)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY((ushort)d.value, dataArray, index);
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T>() + 2) * bitpoints + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(bitpoints, dataArray, index);
                            if (bitdevice != null)
                            {
                                foreach (var d in bitdevice)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.value, dataArray, index);
                                }
                            }
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_ASCII_T>() + 2) * bitpoints + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(bitpoints, dataArray, index);
                            if (bitdevice != null)
                            {
                                foreach (var d in bitdevice)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.value, dataArray, index);
                                }
                            }
                        }
                    }
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    if ((subcommand & SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION) != 0)
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T>() + 1) * bitpoints + 1)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = bitpoints;
                            if (bitdevice != null)
                            {
                                foreach (var d in bitdevice)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY((ushort)d.value, dataArray, index);
                                }
                            }
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_BINARY_T>() + 1) * bitpoints + 1)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = bitpoints;
                            if (bitdevice != null)
                            {
                                foreach (var d in bitdevice)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY((ushort)d.value, dataArray, index);
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T>() + 1) * bitpoints + 1)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = bitpoints;
                            if (bitdevice != null)
                            {
                                foreach (var d in bitdevice)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.value, dataArray, index);
                                }
                            }
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_BINARY_T>() + 1) * bitpoints + 1)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = bitpoints;
                            if (bitdevice != null)
                            {
                                foreach (var d in bitdevice)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.value, dataArray, index);
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return index - startIndex;
        }

        public static int BUILD_DEVICE_WRITE_RANDOM_BYTE_ARRAY_HEADER(MESSAGE_FRAME_TYPE_T frameType, MESSAGE_DATA_CODE_T dataCode, SUB_COMMANDS_T subcommand,
                                                                    IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice, ushort value)> devicein16,
                                                                    IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice, uint value)> devicein32,
                                                                    byte[] dataArray, int startIndex)
        {
            int index = startIndex;
            byte device32points = (byte)(devicein32 == null ? 0 : devicein32.Count()); ;
            byte device16points = (byte)(devicein16 == null ? 0 : devicein16.Count());
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    if ((subcommand & SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION) != 0)
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T>() + 4) * device16points + (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T>() + 8) * device32points + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device16points, dataArray, index);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device32points, dataArray, index);
                            if (devicein16 != null)
                            {
                                foreach (var d in devicein16)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.value, dataArray, index);
                                }
                            }
                            if (devicein32 != null)
                            {
                                foreach (var d in devicein32)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.value, dataArray, index);
                                }
                            }
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_ASCII_T>() + 4) * device16points + (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_ASCII_T>() + 8) * device32points + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device16points, dataArray, index);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device32points, dataArray, index);
                            if (devicein16 != null)
                            {
                                foreach (var d in devicein16)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.value, dataArray, index);
                                }
                            }
                            if (devicein32 != null)
                            {
                                foreach (var d in devicein32)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.value, dataArray, index);
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T>() + 4) * device16points + (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T>() + 8) * device32points + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device16points, dataArray, index);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device32points, dataArray, index);
                            if (devicein16 != null)
                            {
                                foreach (var d in devicein16)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.value, dataArray, index);
                                }
                            }
                            if (devicein32 != null)
                            {
                                foreach (var d in devicein32)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.value, dataArray, index);
                                }
                            }
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_ASCII_T>() + 4) * device16points + (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_ASCII_T>() + 8) * device32points + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device16points, dataArray, index);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(device32points, dataArray, index);
                            if (devicein16 != null)
                            {
                                foreach (var d in devicein16)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.value, dataArray, index);
                                }
                            }
                            if (devicein32 != null)
                            {
                                foreach (var d in devicein32)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.value, dataArray, index);
                                }
                            }
                        }
                    }
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    if ((subcommand & SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION) != 0)
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T>() + 2) * device16points + (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T>() + 4) * device32points + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = device16points;
                            dataArray[index++] = device32points;
                            if (devicein16 != null)
                            {
                                foreach (var d in devicein16)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.value, dataArray, index);
                                }
                            }
                            if (devicein32 != null)
                            {
                                foreach (var d in devicein32)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.value, dataArray, index);
                                }
                            }
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_BINARY_T>() + 2) * device16points + (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_BINARY_T>() + 4) * device32points + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = device16points;
                            dataArray[index++] = device32points;
                            if (devicein16 != null)
                            {
                                foreach (var d in devicein16)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.value, dataArray, index);
                                }
                            }
                            if (devicein32 != null)
                            {
                                foreach (var d in devicein32)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.value, dataArray, index);
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T>() + 2) * device16points + (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T>() + 4) * device32points + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = device16points;
                            dataArray[index++] = device32points;
                            if (devicein16 != null)
                            {
                                foreach (var d in devicein16)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.value, dataArray, index);
                                }
                            }
                            if (devicein32 != null)
                            {
                                foreach (var d in devicein32)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.value, dataArray, index);
                                }
                            }
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_BINARY_T>() + 2) * device16points + (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_BINARY_T>() + 4) * device32points + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = device16points;
                            dataArray[index++] = device32points;
                            if (devicein16 != null)
                            {
                                foreach (var d in devicein16)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.value, dataArray, index);
                                }
                            }
                            if (devicein32 != null)
                            {
                                foreach (var d in devicein32)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.value, dataArray, index);
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return index - startIndex;
        }

        public static int BUILD_DEVICE_READ_BLOCK_BYTE_ARRAY_HEADER(MESSAGE_FRAME_TYPE_T frameType, MESSAGE_DATA_CODE_T dataCode, SUB_COMMANDS_T subcommand,
                                                                    IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice, ushort devicePoints)> worddeviceblock,
                                                                    IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice, ushort devicePoints)> bitdeviceblock,
                                                                    byte[] dataArray, int startIndex)
        {
            int index = startIndex;
            byte worddeviceblockcounter = (byte)(worddeviceblock == null ? 0 : worddeviceblock.Count()); ;
            byte bitdeviceblockcounter = (byte)(bitdeviceblock == null ? 0 : bitdeviceblock.Count());
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    if ((subcommand & SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION) != 0)
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T>() + 4) * (worddeviceblockcounter + bitdeviceblockcounter) + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(worddeviceblockcounter, dataArray, index);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(bitdeviceblockcounter, dataArray, index);
                            if (worddeviceblock != null)
                                foreach (var d in worddeviceblock)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.devicePoints, dataArray, index);
                                }
                            if (bitdeviceblock != null)
                                foreach (var d in bitdeviceblock)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.devicePoints, dataArray, index);
                                }
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_ASCII_T>() + 4) * (worddeviceblockcounter + bitdeviceblockcounter) + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(worddeviceblockcounter, dataArray, index);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(bitdeviceblockcounter, dataArray, index);
                            if (worddeviceblock != null)
                                foreach (var d in worddeviceblock)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.devicePoints, dataArray, index);
                                }
                            if (bitdeviceblock != null)
                                foreach (var d in bitdeviceblock)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.devicePoints, dataArray, index);
                                }
                        }
                    }
                    else
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T>() + 4) * (worddeviceblockcounter + bitdeviceblockcounter) + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(worddeviceblockcounter, dataArray, index);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(bitdeviceblockcounter, dataArray, index);
                            if (worddeviceblock != null)
                                foreach (var d in worddeviceblock)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.devicePoints, dataArray, index);
                                }
                            if (bitdeviceblock != null)
                                foreach (var d in bitdeviceblock)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.devicePoints, dataArray, index);
                                }
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_ASCII_T>() + 4) * (worddeviceblockcounter + bitdeviceblockcounter) + 4)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(worddeviceblockcounter, dataArray, index);
                            index += Message.Message.BINARY_TO_ASCII_ARRAY(bitdeviceblockcounter, dataArray, index);
                            if (worddeviceblock != null)
                                foreach (var d in worddeviceblock)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.devicePoints, dataArray, index);
                                }
                            if (bitdeviceblock != null)
                                foreach (var d in bitdeviceblock)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_ASCII(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_ASCII_ARRAY(d.devicePoints, dataArray, index);
                                }
                        }
                    }
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    if ((subcommand & SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION) != 0)
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T>() + 2) * (worddeviceblockcounter + bitdeviceblockcounter) + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = worddeviceblockcounter;
                            dataArray[index++] = bitdeviceblockcounter;
                            if (worddeviceblock != null)
                                foreach (var d in worddeviceblock)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.devicePoints, dataArray, index);
                                }
                            if (bitdeviceblock != null)
                                foreach (var d in bitdeviceblock)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.devicePoints, dataArray, index);
                                }
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < (Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_BINARY_T>() + 2) * (worddeviceblockcounter + bitdeviceblockcounter) + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = worddeviceblockcounter;
                            dataArray[index++] = bitdeviceblockcounter;
                            if (worddeviceblock != null)
                                foreach (var d in worddeviceblock)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.devicePoints, dataArray, index);
                                }
                            if (bitdeviceblock != null)
                                foreach (var d in bitdeviceblock)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_R_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.devicePoints, dataArray, index);
                                }
                        }
                    }
                    else
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T>() * (worddeviceblockcounter + bitdeviceblockcounter) + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = worddeviceblockcounter;
                            dataArray[index++] = bitdeviceblockcounter;
                            if (worddeviceblock != null)
                                foreach (var d in worddeviceblock)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.devicePoints, dataArray, index);
                                }
                            if (bitdeviceblock != null)
                                foreach (var d in bitdeviceblock)
                                {
                                    index += __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.devicePoints, dataArray, index);
                                }
                        }
                        else
                        {
                            if (dataArray.Length - startIndex < Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_BINARY_T>() * (worddeviceblockcounter + bitdeviceblockcounter) + 2)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                            dataArray[index++] = worddeviceblockcounter;
                            dataArray[index++] = bitdeviceblockcounter;
                            if (worddeviceblock != null)
                                foreach (var d in worddeviceblock)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.devicePoints, dataArray, index);
                                }
                            if (bitdeviceblock != null)
                                foreach (var d in bitdeviceblock)
                                {
                                    index += __BUILD_DEVICE_SPECIFICATION_IN_QL_BINARY(d.deviceCode, d.headDevice, dataArray, index);
                                    index += Message.Message.BINARY_TO_BINARY_ARRAY(d.devicePoints, dataArray, index);
                                }
                        }
                    }
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return index - startIndex;
        }

        public static int BUILD_DEVICE_WRITE_BLOCK_BYTE_ARRAY_HEADER(MESSAGE_DATA_CODE_T dataCode, byte wordDeviceBlock, byte bitDeviceBlock,
                                                                    byte[] dataArray, int startIndex)
        {
            int index = startIndex;
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    if (dataArray.Length - startIndex < 4)
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                    index += Message.Message.BINARY_TO_ASCII_ARRAY(wordDeviceBlock, dataArray, index);
                    index += Message.Message.BINARY_TO_ASCII_ARRAY(bitDeviceBlock, dataArray, index);
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    if (dataArray.Length - startIndex < 2)
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
                    dataArray[index++] = wordDeviceBlock;
                    dataArray[index++] = bitDeviceBlock;
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return index - startIndex;
        }

        private static int __BUILD_DEVICE_SPECIFICATION_IN_QL_ASCII(string deviceCode, uint headDevice, byte[] dataArray, int startIndex)
        {
            try
            {
                var (data, range, _) = __DEVICE_ASCII_CODE_FOR_QL_MODULE[deviceCode];
                DEVICE_SPECIFICATION_IN_QL_ASCII_T dp = new DEVICE_SPECIFICATION_IN_QL_ASCII_T(0x30);
                Array.Copy(data, dp.device_code, data.Length);
                __BINARY_TO_ASCII_ARRAY(headDevice, range, dp.head_device);

                int res = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_ASCII_T>();
                IntPtr p = Marshal.AllocHGlobal(res);
                Marshal.StructureToPtr(dp, p, false);
                Marshal.Copy(p, dataArray, startIndex, res);
                Marshal.FreeHGlobal(p);

                return res;
            }
            catch(KeyNotFoundException)
            {
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_CODE);
            }
        }

        private static int __BUILD_DEVICE_SPECIFICATION_IN_R_ASCII(string deviceCode, uint headDevice, byte[] dataArray, int startIndex)
        {
            try
            {
                var (data, range, _) = __DEVICE_ASCII_CODE_FOR_R_MODULE[deviceCode];
                DEVICE_SPECIFICATION_IN_R_ASCII_T dp = new DEVICE_SPECIFICATION_IN_R_ASCII_T(0x30);
                Array.Copy(data, dp.device_code, data.Length);
                __BINARY_TO_ASCII_ARRAY(headDevice, range, dp.head_device);

                int res = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_ASCII_T>();
                IntPtr p = Marshal.AllocHGlobal(res);
                Marshal.StructureToPtr(dp, p, false);
                Marshal.Copy(p, dataArray, startIndex, res);
                Marshal.FreeHGlobal(p);

                return res;
            }
            catch (KeyNotFoundException)
            {
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_CODE);
            }
        }

        private static int __BUILD_DEVICE_SPECIFICATION_IN_QL_BINARY(string deviceCode, uint headDevice, byte[] dataArray, int startIndex)
        {
            try
            {
                var (data, range, _) = __DEVICE_BINARY_CODE_FOR_QL_MODULE[deviceCode];
                DEVICE_SPECIFICATION_IN_QL_BINARY_T dp = new DEVICE_SPECIFICATION_IN_QL_BINARY_T(0x00);
                __BINARY_TO_BINARY_ARRAY(headDevice, dp.head_device);
                dp.device_code = data;

                int res = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_BINARY_T>();
                IntPtr p = Marshal.AllocHGlobal(res);
                Marshal.StructureToPtr(dp, p, false);
                Marshal.Copy(p, dataArray, startIndex, res);
                Marshal.FreeHGlobal(p);

                return res;
            }
            catch (KeyNotFoundException)
            {
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_CODE);
            }
        }

        private static int __BUILD_DEVICE_SPECIFICATION_IN_R_BINARY(string deviceCode, uint headDevice, byte[] dataArray, int startIndex)
        {
            try
            {
                var (data, range, _) = __DEVICE_BINARY_CODE_FOR_R_MODULE[deviceCode];
                DEVICE_SPECIFICATION_IN_R_BINARY_T dp = new DEVICE_SPECIFICATION_IN_R_BINARY_T(0x00);
                dp.head_device = Message.Message.SMALL_ENDIAN_MODE(headDevice);
                dp.device_code = Message.Message.SMALL_ENDIAN_MODE(data);

                int res = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_BINARY_T>();
                IntPtr p = Marshal.AllocHGlobal(res);
                Marshal.StructureToPtr(dp, p, false);
                Marshal.Copy(p, dataArray, startIndex, res);
                Marshal.FreeHGlobal(p);

                return res;
            }
            catch (KeyNotFoundException)
            {
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_CODE);
            }
        }

        private static int __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII(string deviceCode, uint headDevice, string extension, string extensionModification, string deviceModification, string indirectSpecification, byte[] dataArray, int startIndex)
        {
            DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T dp = new DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T(0x30);
            if (indirectSpecification == "0@")
            {
                dp.indirect_specification[0] = 0x30;
                dp.indirect_specification[1] = 0x40;
            }
            else if(indirectSpecification != null)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_INDIRECT_SPECIFICATION);

            if (extension != null)
            {
                if (__EXTENSION_SPECIFICATION_PATTERN.IsMatch(extension))
                    Array.Copy(System.Text.Encoding.ASCII.GetBytes(extension), dp.extension_specification, dp.extension_specification.Length);
                else
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_EXTENSION_SPECIFICATION);
            }

            if (extensionModification != null)
            {
                if (__QL_EXTENSION_MODIFICATION_PATTERN.IsMatch(extensionModification))
                    Array.Copy(System.Text.Encoding.ASCII.GetBytes(extensionModification), dp.extension_specification_modification, dp.extension_specification_modification.Length);
                else
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_EXTENSION_MODIFICATION);
            }

            try
            {
                var (data, range, _) = __DEVICE_ASCII_CODE_FOR_QL_MODULE[deviceCode];
                Array.Copy(data, dp.device_code, data.Length);
                __BINARY_TO_ASCII_ARRAY(headDevice, range, dp.head_device);
            }
            catch (KeyNotFoundException)
            {
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_CODE);
            }

            if(deviceModification != null)
            {
                if (__QL_EXTENSION_MODIFICATION_PATTERN.IsMatch(deviceModification))
                    Array.Copy(System.Text.Encoding.ASCII.GetBytes(deviceModification), dp.device_modification, dp.device_modification.Length);
                else
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_MODIFICATION);
            }

            int res = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T>();
            IntPtr p = Marshal.AllocHGlobal(res);
            Marshal.StructureToPtr(dp, p, false);
            Marshal.Copy(p, dataArray, startIndex, res);
            Marshal.FreeHGlobal(p);

            return res;
        }

        private static int __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII(string deviceCode, uint headDevice, string extension, string extensionModification, string deviceModification, string indirectSpecification, byte[] dataArray, int startIndex)
        {
            DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T dp = new DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T(0x30);
            if (indirectSpecification == "0@")
            {
                dp.indirect_specification[0] = 0x30;
                dp.indirect_specification[1] = 0x40;
            }
            else if (indirectSpecification != null)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_INDIRECT_SPECIFICATION);

            if (extension != null)
            {
                if (__EXTENSION_SPECIFICATION_PATTERN.IsMatch(extension))
                    Array.Copy(System.Text.Encoding.ASCII.GetBytes(extension), dp.extension_specification, dp.extension_specification.Length);
                else
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_EXTENSION_SPECIFICATION);
            }

            if (extensionModification != null)
            {
                if (__R_EXTENSION_MODIFICATION_PATTERN.IsMatch(extensionModification))
                    Array.Copy(System.Text.Encoding.ASCII.GetBytes(extensionModification), dp.extension_specification_modification, dp.extension_specification_modification.Length);
                else
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_EXTENSION_MODIFICATION);
            }

            try
            {
                var (data, range, _) = __DEVICE_ASCII_CODE_FOR_R_MODULE[deviceCode];
                Array.Copy(data, dp.device_code, data.Length);
                __BINARY_TO_ASCII_ARRAY(headDevice, range, dp.head_device);
            }
            catch (KeyNotFoundException)
            {
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_CODE);
            }

            if (deviceModification != null)
            {
                if (__R_EXTENSION_MODIFICATION_PATTERN.IsMatch(deviceModification))
                    Array.Copy(System.Text.Encoding.ASCII.GetBytes(deviceModification), dp.device_modification, dp.device_modification.Length);
                else
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_MODIFICATION);
            }

            int res = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T>();
            IntPtr p = Marshal.AllocHGlobal(res);
            Marshal.StructureToPtr(dp, p, false);
            Marshal.Copy(p, dataArray, startIndex, res);
            Marshal.FreeHGlobal(p);

            return res;
        }

        private static int __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY(string deviceCode, uint headDevice, string extension, string extensionModification, string deviceModification, string indirectSpecification, byte[] dataArray, int startIndex)
        {
            DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T dp = new DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T(0x00);
            if (indirectSpecification == "0@")
                dp.device_modification |= Message.Message.SMALL_ENDIAN_MODE((ushort)DEVICE_ACCESS_MODIFICATION.INDIRECT_SPECIFICATION);
            else if (indirectSpecification != null)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_INDIRECT_SPECIFICATION);

            if (extension != null)
            {
                if (__EXTENSION_SPECIFICATION_PATTERN.IsMatch(extension))
                {
                    dp.extension_specification = Message.Message.SMALL_ENDIAN_MODE(Convert.ToUInt16(extension[1..], 16));
                    if (extension.StartsWith("U3E"))
                        dp.direct_memory_specification = (byte)(DIRECT_MEMORY_SPECIFICATION_T.CPU_BUFFER_MEMORY_ACCESS_DEVICE);
                    else if(extension.StartsWith("U"))
                        dp.direct_memory_specification = (byte)(DIRECT_MEMORY_SPECIFICATION_T.MODULE_ACCESS_DEVICE);
                    else
                        dp.direct_memory_specification = (byte)(DIRECT_MEMORY_SPECIFICATION_T.LINK_DIRECT_DEVICE);

                }
                else
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_EXTENSION_SPECIFICATION);
            }

            if (extensionModification != null)
            {
                if (__QL_EXTENSION_MODIFICATION_PATTERN.IsMatch(extensionModification))
                    dp.extension_specification_modification = Message.Message.SMALL_ENDIAN_MODE((ushort)((Convert.ToByte(extensionModification[1..], 10) << 8) + (ushort)DEVICE_ACCESS_MODIFICATION.Z_DEVICE_MODIFICATION));
                else
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_EXTENSION_MODIFICATION);
            }

            try
            {
                var (data, range, _) = __DEVICE_BINARY_CODE_FOR_QL_MODULE[deviceCode];
                dp.device_code = data;
                __BINARY_TO_BINARY_ARRAY(headDevice, dp.head_device);
            }
            catch (KeyNotFoundException)
            {
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_CODE);
            }

            if (deviceModification != null)
            {
                if (__QL_EXTENSION_MODIFICATION_PATTERN.IsMatch(deviceModification))
                    dp.device_modification |= Message.Message.SMALL_ENDIAN_MODE((ushort)((ushort)DEVICE_ACCESS_MODIFICATION.Z_DEVICE_MODIFICATION + (Convert.ToByte(deviceModification[1..], 10) << 8)));
                else
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_MODIFICATION);
            }

            int res = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T>();
            IntPtr p = Marshal.AllocHGlobal(res);
            Marshal.StructureToPtr(dp, p, false);
            Marshal.Copy(p, dataArray, startIndex, res);
            Marshal.FreeHGlobal(p);

            return res;
        }

        private static int __BUILD_DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY(string deviceCode, uint headDevice, string extension, string extensionModification, string deviceModification, string indirectSpecification, byte[] dataArray, int startIndex)
        {
            DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T dp = new DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T(0x00);
            if (indirectSpecification == "0@")
                dp.device_modification |= Message.Message.SMALL_ENDIAN_MODE((ushort)DEVICE_ACCESS_MODIFICATION.INDIRECT_SPECIFICATION);
            else if (indirectSpecification != null)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_INDIRECT_SPECIFICATION);

            if (extension != null)
            {
                if (__EXTENSION_SPECIFICATION_PATTERN.IsMatch(extension))
                {
                    dp.extension_specification = Message.Message.SMALL_ENDIAN_MODE(Convert.ToUInt16(extension[1..], 16));
                    if (extension.StartsWith("U3E"))
                        dp.direct_memory_specification = (byte)(DIRECT_MEMORY_SPECIFICATION_T.CPU_BUFFER_MEMORY_ACCESS_DEVICE);
                    else if (extension.StartsWith("U"))
                        dp.direct_memory_specification = (byte)(DIRECT_MEMORY_SPECIFICATION_T.MODULE_ACCESS_DEVICE);
                    else
                        dp.direct_memory_specification = (byte)(DIRECT_MEMORY_SPECIFICATION_T.LINK_DIRECT_DEVICE);

                }
                else
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_EXTENSION_SPECIFICATION);
            }

            if (extensionModification != null)
            {
                if (__RZ_EXTENSION_MODIFICATION_PATTERN.IsMatch(deviceModification))
                    dp.extension_specification_modification = Message.Message.SMALL_ENDIAN_MODE((ushort)((Convert.ToByte(extensionModification[2..], 10) << 8) + (ushort)DEVICE_ACCESS_MODIFICATION.Z_DEVICE_MODIFICATION));
                else if (__RLZ_EXTENSION_MODIFICATION_PATTERN.IsMatch(extensionModification))
                    dp.extension_specification_modification = Message.Message.SMALL_ENDIAN_MODE((ushort)((Convert.ToByte(extensionModification[2..], 10) << 8) + (ushort)DEVICE_ACCESS_MODIFICATION.LZ_DEVICE_MODIFICATION));
                else
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_EXTENSION_MODIFICATION);
            }

            try
            {
                var (data, range, _) = __DEVICE_BINARY_CODE_FOR_R_MODULE[deviceCode];
                dp.device_code = Message.Message.SMALL_ENDIAN_MODE(data);
                dp.head_device = Message.Message.SMALL_ENDIAN_MODE(headDevice);
            }
            catch (KeyNotFoundException)
            {
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_CODE);
            }

            if (deviceModification != null)
            {
                if (__RZ_EXTENSION_MODIFICATION_PATTERN.IsMatch(deviceModification))
                    dp.device_modification |= Message.Message.SMALL_ENDIAN_MODE((ushort)((ushort)DEVICE_ACCESS_MODIFICATION.Z_DEVICE_MODIFICATION + (Convert.ToByte(deviceModification[2..], 10) << 8)));
                else if (__RLZ_EXTENSION_MODIFICATION_PATTERN.IsMatch(deviceModification))
                    dp.device_modification |= Message.Message.SMALL_ENDIAN_MODE((ushort)((ushort)DEVICE_ACCESS_MODIFICATION.LZ_DEVICE_MODIFICATION + (Convert.ToByte(deviceModification[2..], 10) << 8)));
                else
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_MODIFICATION);
            }

            int res = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T>();
            IntPtr p = Marshal.AllocHGlobal(res);
            Marshal.StructureToPtr(dp, p, false);
            Marshal.Copy(p, dataArray, startIndex, res);
            Marshal.FreeHGlobal(p);

            return res;
        }
    
        public static int DEVICE_SPECIFICATION_LENGTH(MESSAGE_DATA_CODE_T dataCode, SUB_COMMANDS_T subcommand)
        {
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    if ((subcommand & SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION) != 0)
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                            return Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T>();
                        else
                            return Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_ASCII_T>();
                    }
                    else
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                            return Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T>();
                        else
                            return Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_ASCII_T>();
                    }
                case MESSAGE_DATA_CODE_T.BINARY:
                    if ((subcommand & SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION) != 0)
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                            return Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T>();   
                        else
                            return Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_BINARY_T>();
                    }
                    else
                    {
                        if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) != 0)
                            return Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T>();
                            
                        else
                            return Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_BINARY_T>();
                    }
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
        }

        public static int DEIVICE_REGISTER_DATA_ARRAY_LENGTH(MESSAGE_DATA_CODE_T dataCode, SUB_COMMANDS_T subcommand, ushort points)
        {
            int res = 0;
            switch(dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    if ((subcommand & SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT) != 0)
                        res = points; // bit unit ascii code
                    else
                        res = points * 4; //word unit ascii code
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    if ((subcommand & SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT) != 0)
                        res = points / 2 + points % 2; //bit unit binary code
                    else
                        res = points * 2; // word unit binary code
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return res;
        }

        public static int READ_BIT_DEVICE_IN_BIT_UNIT(ReadOnlySpan<byte> source, MESSAGE_DATA_CODE_T dataCode, ushort bitpoints, Span<byte> data)
        {
            int length = DEIVICE_REGISTER_DATA_ARRAY_LENGTH(dataCode, SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT, bitpoints);
            if (source.Length < length)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
            if (data.Length < bitpoints)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    for(int i = 0; i < bitpoints; ++i)
                    {
                        if (source[i] == 0x31)
                            data[i] = 1;
                        else if (source[i] == 0x30)
                            data[i] = 0;
                        else
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_REGISTER_DATA);
                    }
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    for (int i = 0; i < bitpoints; ++i)
                    {
                        if(i % 2 == 0)
                        {
                            if ((source[i / 2] & 0xF0) == 0x10)
                                data[i] = 1;
                            else if ((source[i / 2] & 0xF0) == 0x00)
                                data[i] = 0;
                            else
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_REGISTER_DATA);
                        }
                        else
                        {
                            if ((source[i / 2] & 0x0F) == 0x01)
                                data[i] = 1;
                            else if ((source[i / 2] & 0x0F) == 0x00)
                                data[i] = 0;
                            else
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_REGISTER_DATA);
                        }
                    }
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return bitpoints;
        }

        public static int READ_DEVICE_IN_WORD_UNIT(ReadOnlySpan<byte> source, MESSAGE_DATA_CODE_T dataCode, ushort wordpoints, Span<ushort> data)
        {
            int length = DEIVICE_REGISTER_DATA_ARRAY_LENGTH(dataCode, 0, wordpoints);
            if (source.Length < length)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
            if (data.Length < wordpoints)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    for(int i = 0; i < wordpoints; ++i)
                    {
                        try
                        {
                            data[i] = Convert.ToUInt16(System.Text.Encoding.ASCII.GetString(source.Slice(i * 4, 4)), 16);
                        }
                        catch(Exception)
                        {
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_REGISTER_DATA);
                        }
                    }
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    if (Message.Message.BIG_ENDIAN == true)
                        for (int i = 0; i < wordpoints; ++i)
                            data[i] = (ushort)(source[i * 2] + (source[i * 2 + 1] << 8));
                    else
                        source.Slice(0, length).CopyTo(MemoryMarshal.AsBytes(data));
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return wordpoints;
        }

        public static int READ_DEVICE_IN_DWORD_UNIT(ReadOnlySpan<byte> source, MESSAGE_DATA_CODE_T dataCode, ushort dwordpoints, Span<uint> data)
        {
            int length = DEIVICE_REGISTER_DATA_ARRAY_LENGTH(dataCode, 0, dwordpoints) * 2;
            if (source.Length < length)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
            if (data.Length < dwordpoints)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    for (int i = 0; i < dwordpoints; ++i)
                    {
                        try
                        {
                            data[i] = Convert.ToUInt32(System.Text.Encoding.ASCII.GetString(source.Slice(i * 8, 8)), 16);
                        }
                        catch (Exception)
                        {
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_REGISTER_DATA);
                        }
                    }
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    if (Message.Message.BIG_ENDIAN == true)
                        for (int i = 0; i < dwordpoints; ++i)
                            data[i] = (uint)(source[i * 4] + (source[i * 4 + 1] << 8) + (source[i * 4 + 2] << 16) + (source[i * 4 + 3] << 24));
                    else
                        source.Slice(0, length).CopyTo(MemoryMarshal.AsBytes(data));
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return dwordpoints;
        }

        public static int WRITE_BIT_DEVICE_IN_BIT_UNIT(ReadOnlySpan<byte> source, MESSAGE_DATA_CODE_T dataCode, ushort bitpoints, Span<byte> data)
        {
            int length = DEIVICE_REGISTER_DATA_ARRAY_LENGTH(dataCode, SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT, bitpoints);
            if (source.Length < bitpoints)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
            if (data.Length < length)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    for (int i = 0; i < length; ++i)
                    {
                        if (source[i] == 1)
                            data[i] = 0x31;
                        else if (source[i] == 0)
                            data[i] = 0x30;
                        else
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_REGISTER_DATA);
                    }
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    for (int i = 0; i < length; ++i)
                    {
                        if (source[i * 2] == 1)
                            data[i] = 0x10;
                        else if (source[i * 2] == 0)
                            data[i] = 0x00;
                        else
                            throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_REGISTER_DATA);

                        if (i * 2 + 1 < bitpoints)
                        {
                            if (source[i * 2 + 1] == 1)
                                data[i] += 0x01;
                            else if (source[i * 2 + 1] == 0)
                                data[i] += 0x00;
                            else
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DEVICE_REGISTER_DATA);
                        }
                    }
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return bitpoints;
        }

        public static int WRITE_DEVICE_IN_WORD_UNIT(ReadOnlySpan<ushort> source, MESSAGE_DATA_CODE_T dataCode, ushort wordpoints, Span<byte> data)
        {
            int length = DEIVICE_REGISTER_DATA_ARRAY_LENGTH(dataCode, 0, wordpoints);
            if (source.Length < wordpoints)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
            if (data.Length < length)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    for (int i = 0; i < wordpoints; ++i)
                    {
                        string s = source[i].ToString("X4");
                        System.Text.Encoding.ASCII.GetBytes(s, data.Slice(i*4, 4));
                    }
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    if (Message.Message.BIG_ENDIAN == true)
                    {
                        for (int i = 0; i < length; ++i)
                        {
                            if (i % 2 == 0)
                                data[i] = (byte)(source[i / 2] & 0x00FF);
                            else
                                data[i] = (byte)((source[i / 2] & 0xFF00) >> 8);
                        }
                    }
                    else
                        MemoryMarshal.AsBytes(source.Slice(0, wordpoints)).CopyTo(data);
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }

            return wordpoints;
        }

        public static int WRITE_DEVICE_IN_DWORD_UNIT(ReadOnlySpan<uint> source, MESSAGE_DATA_CODE_T dataCode, ushort dwordpoints, Span<byte> data)
        {
            int length = DEIVICE_REGISTER_DATA_ARRAY_LENGTH(dataCode, 0, dwordpoints) * 2;
            if (source.Length < dwordpoints)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
            if (data.Length < length)
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    for (int i = 0; i < dwordpoints; ++i)
                    {
                        string s = source[i].ToString("X8");
                        System.Text.Encoding.ASCII.GetBytes(s, data.Slice(i * 8, 8));
                    }
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    if (Message.Message.BIG_ENDIAN == true)
                    {
                        for (int i = 0; i < length; i += 4)
                        {
                            data[i] = (byte)(source[i / 4] & 0x000000FF);
                            data[i + 1] = (byte)((source[i / 4] >> 8) & 0x000000FF);
                            data[i + 2] = (byte)((source[i / 4] >> 16) & 0x000000FF);
                            data[i + 3] = (byte)((source[i / 4] >> 24) & 0x000000FF);
                        }
                    }
                    else
                        MemoryMarshal.AsBytes(source.Slice(0, dwordpoints)).CopyTo(data);
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }

            return dwordpoints;
        }

    }
}
