using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Obelia.SlavePDOs.Generic
{
    public enum VariableDisplayFormat
    {
        DEC = 10,
        HEX = 16,
        BIN = 2,
        OCT = 8,
        BOOL = 1
    }

    public enum VariableDataType
    {
        BOOL,
        USINT,
        SINT,
        UINT,
        INT,
        UDINT,
        DINT,
        ULINT,
        LINT,
        REAL,
        BYTE_ARRAY,
        UNKNOWN
    }

    public class GenericSlavePdosDataModel : IEtherCATIOMasterSlaveData
    {
        public void ReadRxPDO(ushort[] rxData)
        {
            if (SelectedSlave != null)
            {
                foreach (var v in SelectedSlave.RxVariables)
                {
                    if (v.GlobalBitOffset + v.BitSize <= rxData.Length * 16 && v.DataType != VariableDataType.UNKNOWN && v.RxRequest)
                    {
                        unsafe
                        {
                            fixed (ushort* pTx = rxData)
                            {
                                byte* pRaw = (byte*)pTx + v.GlobalByteOffset;
                                if (v.BitSize == 1)
                                    if (v.BooleanData)
                                        (*pRaw) |= (byte)(1 << (byte)(v.GlobalBitOffset % 8));
                                    else
                                        (*pRaw) &= (byte)~(1 << (byte)(v.GlobalBitOffset % 8));
                                else
                                    v.GetValue(pRaw, v.BitSize / 8);
                            }
                        }
                    }
                }
            }
        }

        public void WriteRxPDO(ushort[] rxData)
        {
            foreach (var slv in __slaves)
            {
                foreach (var v in slv.RxVariables)
                {
                    if (v.GlobalBitOffset + v.BitSize <= rxData.Length * 16 && v.DataType != VariableDataType.UNKNOWN)
                    {
                        unsafe
                        {
                            fixed (ushort* pRx = rxData)
                            {
                                byte* pRaw = (byte*)pRx + v.GlobalByteOffset;
                                if (v.BitSize == 1)
                                    v.BooleanData = ((*pRaw) & (1 << (byte)(v.GlobalBitOffset % 8))) != 0;
                                else
                                    v.SetValue(pRaw, v.BitSize / 8);
                            }
                        }
                    }
                }
            }
        }

        public void WriteTxPDO(ushort[] slaveESMs, ushort[] slaveErrors, ushort[] txData, ushort[] rxrbData)
        {
            foreach (var slv in __slaves.Select((s, i) => (s, i)))
            {
                slv.s.ErrorStatus = slaveErrors[slv.i];
                slv.s.IOStatus = slaveErrors[slv.i] == 0 && slaveESMs[slv.i] == (ushort)STATE_MACHINE_T.OP;
                slv.s.ESM = slaveESMs[slv.i];
            }

            if (SelectedSlave != null)
            {
                foreach (var v in SelectedSlave.TxVariables)
                {
                    if (v.GlobalBitOffset + v.BitSize <= txData.Length * 16 && v.DataType != VariableDataType.UNKNOWN)
                    {
                        unsafe
                        {
                            fixed (ushort* pTx = txData)
                            {
                                byte* pRaw = (byte*)pTx + v.GlobalByteOffset;
                                if (v.BitSize == 1)
                                    v.BooleanData = ((*pRaw) & (1 << (byte)(v.GlobalBitOffset % 8))) != 0;
                                else
                                    v.SetValue(pRaw, v.BitSize / 8);
                            }
                        }
                    }
                }
                foreach (var v in SelectedSlave.RxVariables)
                {
                    if (v.Readback.GlobalBitOffset + v.Readback.BitSize <= rxrbData.Length * 16 && v.DataType != VariableDataType.UNKNOWN)
                    {
                        unsafe
                        {
                            fixed (ushort* pRxRb = rxrbData)
                            {
                                byte* pRaw = (byte*)pRxRb + v.Readback.GlobalByteOffset;
                                if (v.Readback.BitSize == 1)
                                    v.Readback.BooleanData = ((*pRaw) & (1 << (byte)(v.Readback.GlobalBitOffset % 8))) != 0;
                                else
                                    v.Readback.SetValue(pRaw, v.Readback.BitSize / 8);
                            }
                        }
                    }
                }
            }
        }

        private List<Slave> __slaves;

        public string ENIPath { get; private set; }
        public IReadOnlyList<Slave> Slaves { get { return __slaves; } }
        public Slave? SelectedSlave { get; set; }

        public int TxSizeInWord { get; private set; }
        public int RxSizeInWord { get; private set; }
        public int NumberOfSlaves { get; private set; }

        public GenericSlavePdosDataModel()
        {
            __slaves = new List<Slave>();
            ENIPath = "N/A";
        }
        public GenericSlavePdosDataModel(string path)
        {
            XmlDocument eni = new XmlDocument();
            List<Slave> slaves = new List<Slave>();

            string slaveName = null;
            ushort slaveAddr = 0;
            uint vendorID = 0;
            uint productCode = 0;
            uint revisionNumber = 0;
            uint globalTxOffset = 0;
            uint globalRxOffset = 0;

            eni.Load(path);
            foreach (XmlNode slaveNode in eni.SelectNodes("/EtherCATConfig/Config/Slave|/EncryptedEtherCATConfig/Config/Slave"))
            {
                slaveName = slaveNode.SelectSingleNode("Info/Name").FirstChild.Value;

                int temp = 0;
                if (uint.TryParse(slaveNode.SelectSingleNode("Info/VendorId").FirstChild.Value, out vendorID) == false)
                {
                    int.TryParse(slaveNode.SelectSingleNode("Info/VendorId").FirstChild.Value, out temp);
                    vendorID = (uint)temp;
                }
                temp = 0;
                if (uint.TryParse(slaveNode.SelectSingleNode("Info/ProductCode").FirstChild.Value, out productCode) == false)
                {
                    int.TryParse(slaveNode.SelectSingleNode("Info/ProductCode").FirstChild.Value, out temp);
                    productCode = (uint)temp;
                }
                temp = 0;
                if (uint.TryParse(slaveNode.SelectSingleNode("Info/RevisionNo").FirstChild.Value, out revisionNumber) == false)
                {
                    int.TryParse(slaveNode.SelectSingleNode("Info/RevisionNo").FirstChild.Value, out temp);
                    revisionNumber = (uint)temp;
                }

                slaveAddr = Convert.ToUInt16(slaveNode.SelectSingleNode("Info/PhysAddr").FirstChild.Value);

                List<Pdo> txPDOs = new List<Pdo>();
                ushort localOffset = 0;
                uint globalOffset = globalTxOffset;
                foreach (XmlNode pdoNode in slaveNode.SelectNodes("ProcessData/TxPdo"))
                {
                    if (pdoNode.Attributes["Sm"] == null)
                        continue;
                    string pdoName = pdoNode.SelectSingleNode("Name").FirstChild.Value;
                    ushort pdoIndex = Convert.ToUInt16(pdoNode.SelectSingleNode("Index").FirstChild.Value.Substring(2), 16);
                    List<Variable> variables = new List<Variable>();
                    foreach (XmlNode varEntry in pdoNode.SelectNodes("Entry"))
                    {
                        ushort varSize = Convert.ToUInt16(varEntry.SelectSingleNode("BitLen").FirstChild.Value);
                        if (varEntry.SelectSingleNode("Name") != null)
                        {
                            string varName = varEntry.SelectSingleNode("Name").FirstChild.Value;
                            string varType = varEntry.SelectSingleNode("DataType").FirstChild.Value.ToUpper();
                            ushort varIndex = Convert.ToUInt16(varEntry.SelectSingleNode("Index").FirstChild.Value.Substring(2), 16);
                            byte varSubIndex = Convert.ToByte(varEntry.SelectSingleNode("SubIndex").FirstChild.Value, 10);

                            variables.Add(new Variable(localOffset + globalOffset, localOffset, varSize, varType, $"[0x{pdoIndex:X4}] [Tx] {pdoName}", varIndex, varSubIndex, varName, true, null));
                        }
                        localOffset += varSize;
                    }
                    txPDOs.Add(new Pdo(pdoIndex, pdoName, true, variables));
                }

                List<Pdo> rxPDOs = new List<Pdo>();
                localOffset = 0;
                globalOffset = globalRxOffset;
                foreach (XmlNode pdoNode in slaveNode.SelectNodes("ProcessData/RxPdo"))
                {
                    if (pdoNode.Attributes["Sm"] == null)
                        continue;
                    string pdoName = pdoNode.SelectSingleNode("Name").FirstChild.Value;
                    ushort pdoIndex = Convert.ToUInt16(pdoNode.SelectSingleNode("Index").FirstChild.Value.Substring(2), 16);
                    List<Variable> variables = new List<Variable>();
                    foreach (XmlNode varEntry in pdoNode.SelectNodes("Entry"))
                    {
                        ushort varSize = Convert.ToUInt16(varEntry.SelectSingleNode("BitLen").FirstChild.Value);
                        if (varEntry.SelectSingleNode("Name") != null)
                        {
                            string varName = varEntry.SelectSingleNode("Name").FirstChild.Value;
                            string varType = varEntry.SelectSingleNode("DataType").FirstChild.Value.ToUpper();
                            ushort varIndex = Convert.ToUInt16(varEntry.SelectSingleNode("Index").FirstChild.Value.Substring(2), 16);
                            byte varSubIndex = Convert.ToByte(varEntry.SelectSingleNode("SubIndex").FirstChild.Value, 10);

                            var readback = new Variable(localOffset + globalOffset, localOffset, varSize, varType, $"[0x{pdoIndex:X4}] [Rx] {pdoName}", varIndex, varSubIndex, varName, true, null);
                            variables.Add(new Variable(localOffset + globalOffset, localOffset, varSize, varType, $"[0x{pdoIndex:X4}] [Rx] {pdoName}", varIndex, varSubIndex, varName, false, readback));
                        }
                        localOffset += varSize;
                    }
                    rxPDOs.Add(new Pdo(pdoIndex, pdoName, false, variables));
                }

                slaves.Add(new Slave(slaveAddr, slaveName, vendorID, productCode, revisionNumber, globalTxOffset / 8, globalRxOffset / 8, txPDOs, rxPDOs));

                uint size = 0;
                if (slaveNode.SelectSingleNode("ProcessData/Recv/BitLength") != null)
                {
                    size = Convert.ToUInt32(slaveNode.SelectSingleNode("ProcessData/Recv/BitLength").FirstChild.Value);
                    globalTxOffset += (uint)(size / 16 + (size % 16 == 0 ? 0 : 1)) * 16;
                }
                if (slaveNode.SelectSingleNode("ProcessData/Send/BitLength") != null)
                {
                    size = Convert.ToUInt32(slaveNode.SelectSingleNode("ProcessData/Send/BitLength").FirstChild.Value);
                    globalRxOffset += (uint)(size / 16 + (size % 16 == 0 ? 0 : 1)) * 16;
                }
            }

            TxSizeInWord = (int)(globalTxOffset / 16);
            RxSizeInWord = (int)(globalRxOffset / 16);
            NumberOfSlaves = slaves.Count;

            __slaves = slaves;
            ENIPath = path;
        }
    }



    public class Variable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void _notify_property_changed([CallerMemberName] String propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private byte[] __data_storage;

        private static Dictionary<VariableDataType, List<VariableDisplayFormat>> __available_display_formats;
        public static IReadOnlyDictionary<VariableDataType, List<VariableDisplayFormat>> AvailableDisplayFormats { get { return __available_display_formats; } }

        static Variable()
        {
            __available_display_formats = new Dictionary<VariableDataType, List<VariableDisplayFormat>>();
            __available_display_formats[VariableDataType.BOOL] = new List<VariableDisplayFormat>() { VariableDisplayFormat.BOOL };
            __available_display_formats[VariableDataType.UNKNOWN] = __available_display_formats[VariableDataType.BOOL];
            __available_display_formats[VariableDataType.BYTE_ARRAY] =
                new List<VariableDisplayFormat>() { VariableDisplayFormat.HEX, VariableDisplayFormat.DEC };
            __available_display_formats[VariableDataType.REAL] =
                new List<VariableDisplayFormat>() { VariableDisplayFormat.DEC };
            __available_display_formats[VariableDataType.USINT] =
                new List<VariableDisplayFormat>() { VariableDisplayFormat.DEC, VariableDisplayFormat.HEX,
                                                            VariableDisplayFormat.OCT, VariableDisplayFormat.BIN};
            __available_display_formats[VariableDataType.SINT] = __available_display_formats[VariableDataType.USINT];
            __available_display_formats[VariableDataType.UINT] = __available_display_formats[VariableDataType.USINT];
            __available_display_formats[VariableDataType.INT] = __available_display_formats[VariableDataType.USINT];
            __available_display_formats[VariableDataType.UDINT] = __available_display_formats[VariableDataType.USINT];
            __available_display_formats[VariableDataType.DINT] = __available_display_formats[VariableDataType.USINT];

            __available_display_formats[VariableDataType.ULINT] = new List<VariableDisplayFormat>() { VariableDisplayFormat.DEC, VariableDisplayFormat.HEX };
            __available_display_formats[VariableDataType.LINT] = __available_display_formats[VariableDataType.ULINT];
        }

        public ushort Index { get; }
        public byte SubIndex { get; }
        public string Name { get; }
        public string FullName { get { return $"[0x{Index:X4}:{SubIndex:X2}]-{Name}"; } }
        public VariableDataType DataType { get; }
        public ushort BitSize { get; }
        public ushort LocalBitOffset { get; }
        public ushort LocalByteOffset { get { return (ushort)(LocalBitOffset / 8); } }
        public uint GlobalBitOffset { get; }
        public uint GlobalByteOffset { get { return (ushort)(GlobalBitOffset / 8); } }
        public string PDO { get; }
        public bool IsReadOnly { get; }
        public Variable? Readback { get; } = null;


        private VariableDisplayFormat __display_format;
        public VariableDisplayFormat DisplayFormat
        {
            get { return __display_format; }
            set
            {
                if (__display_format == value)
                    return;

                if (AvailableDisplayFormats[DataType].Contains(value))
                {
                    __display_format = value;
                    _notify_property_changed("DataString");
                    _notify_property_changed();
                }
            }
        }

        public Variable(uint globalbitpos, ushort localbitpos, ushort bitSize, string datatype, string pdo, ushort index, byte sub, string name, bool isReadOnly, Variable? readback)
        {
            GlobalBitOffset = globalbitpos;
            LocalBitOffset = localbitpos;
            BitSize = bitSize;
            Index = index;
            SubIndex = sub;
            Name = name;
            PDO = pdo;
            Readback = readback;
            __data_storage = new byte[(bitSize / 8 + (bitSize % 8 == 0 ? 0 : 1))];
            switch (datatype)
            {
                case "BOOL":
                    if (bitSize == 1) DataType = VariableDataType.BOOL;
                    else DataType = VariableDataType.UNKNOWN;
                    break;
                case "USINT":
                case "BYTE":
                    if (bitSize == 8 && globalbitpos % 8 == 0 && localbitpos % 8 == 0) DataType = VariableDataType.USINT;
                    else DataType = VariableDataType.UNKNOWN;
                    break;
                case "SINT":
                    if (bitSize == 8 && globalbitpos % 8 == 0 && localbitpos % 8 == 0) DataType = VariableDataType.SINT;
                    else DataType = VariableDataType.UNKNOWN;
                    break;
                case "UINT":
                    if (bitSize == 16 && globalbitpos % 8 == 0 && localbitpos % 8 == 0) DataType = VariableDataType.UINT;
                    else DataType = VariableDataType.UNKNOWN;
                    break;
                case "INT":
                    if (bitSize == 16 && globalbitpos % 8 == 0 && localbitpos % 8 == 0) DataType = VariableDataType.INT;
                    else DataType = VariableDataType.UNKNOWN;
                    break;
                case "UDINT":
                    if (bitSize == 32 && globalbitpos % 8 == 0 && localbitpos % 8 == 0) DataType = VariableDataType.UDINT;
                    else DataType = VariableDataType.UNKNOWN;
                    break;
                case "DINT":
                    if (bitSize == 32 && globalbitpos % 8 == 0 && localbitpos % 8 == 0) DataType = VariableDataType.DINT;
                    else DataType = VariableDataType.UNKNOWN;
                    break;
                case "ULINT":
                    if (bitSize == 64 && globalbitpos % 8 == 0 && localbitpos % 8 == 0) DataType = VariableDataType.ULINT;
                    else DataType = VariableDataType.UNKNOWN;
                    break;
                case "LINT":
                    if (bitSize == 64 && globalbitpos % 8 == 0 && localbitpos % 8 == 0) DataType = VariableDataType.LINT;
                    else DataType = VariableDataType.UNKNOWN;
                    break;
                case "REAL":
                    if (bitSize == 32 && globalbitpos % 8 == 0 && localbitpos % 8 == 0) DataType = VariableDataType.REAL;
                    else DataType = VariableDataType.UNKNOWN;
                    break;
                default:
                    if (bitSize > 0 && bitSize % 8 == 0 && globalbitpos % 8 == 0 && localbitpos % 8 == 0) DataType = VariableDataType.BYTE_ARRAY;
                    else DataType = VariableDataType.UNKNOWN;
                    break;
            }
            DisplayFormats = AvailableDisplayFormats[DataType];
            DisplayFormat = DisplayFormats.First();
            IsReadOnly = isReadOnly;
        }

        public IReadOnlyCollection<VariableDisplayFormat> DisplayFormats { get; private set; }

        public bool RxRequest { get; private set; } = false;

        unsafe public void SetValue(byte* pRaw, int sizeInByte)
        {
            fixed (byte* pStorage = __data_storage)
            {
                bool dirty = false;
                for (int i = 0; i < sizeInByte; ++i)
                {
                    if (*(pRaw + i) != *(pStorage + i))
                    {
                        dirty = true;
                        break;
                    }
                }
                if (dirty)
                {
                    Buffer.MemoryCopy(pRaw, pStorage, __data_storage.Length, sizeInByte);
                    _notify_property_changed("DataString");
                }
            }
        }

        unsafe public void GetValue(byte* pRaw, int sizeInByte)
        {
            fixed (byte* pStorage = __data_storage)
            {
                Buffer.MemoryCopy(pStorage, pRaw, sizeInByte, __data_storage.Length);
                //_notify_property_changed("DataString");
                RxRequest = false;
            }
        }

        public bool BooleanData
        {
            get
            {
                RxRequest = false;
                return (__data_storage[0] & (1 << (int)(GlobalBitOffset % 8))) != 0;
            }
            set
            {
                if (value != ((__data_storage[0] & (1 << (int)(GlobalBitOffset % 8))) != 0))
                {
                    if (value)
                        __data_storage[0] |= (byte)(1 << (int)(GlobalBitOffset % 8));
                    else
                        __data_storage[0] &= (byte)~(1 << (int)(GlobalBitOffset % 8));
                    _notify_property_changed("DataString");
                }
            }
        }

        public string DataString
        {
            get
            {
                //_notify_property_changed("DisplayFormat");
                unsafe
                {
                    fixed (byte* pStorage = __data_storage)
                        switch (DataType)
                        {
                            case VariableDataType.BOOL: return (__data_storage[0] & (1 << (int)(GlobalBitOffset % 8))) != 0 ? "True" : "False";
                            case VariableDataType.USINT:
                                switch (DisplayFormat)
                                {
                                    case VariableDisplayFormat.DEC:
                                        return (*(byte*)(pStorage)).ToString();
                                    case VariableDisplayFormat.BIN:
                                        return Convert.ToString((*(byte*)(pStorage)), 2).PadLeft(8, '0');
                                    case VariableDisplayFormat.HEX:
                                        return Convert.ToString((*(byte*)(pStorage)), 16).PadLeft(2, '0');
                                    case VariableDisplayFormat.OCT:
                                        return Convert.ToString((*(byte*)(pStorage)), 8).PadLeft(3, '0');
                                    default:
                                        return "N/A";
                                }
                            case VariableDataType.SINT:
                                switch (DisplayFormat)
                                {
                                    case VariableDisplayFormat.DEC:
                                        return (*(sbyte*)(pStorage)).ToString();
                                    case VariableDisplayFormat.BIN:
                                        return Convert.ToString((*(sbyte*)(pStorage)), 2).PadLeft(8, '0');
                                    case VariableDisplayFormat.HEX:
                                        return Convert.ToString((*(sbyte*)(pStorage)), 16).PadLeft(2, '0');
                                    case VariableDisplayFormat.OCT:
                                        return Convert.ToString((*(sbyte*)(pStorage)), 8).PadLeft(3, '0');
                                    default:
                                        return "N/A";
                                }
                            case VariableDataType.UINT:
                                switch (DisplayFormat)
                                {
                                    case VariableDisplayFormat.DEC:
                                        return (*(ushort*)(pStorage)).ToString();
                                    case VariableDisplayFormat.BIN:
                                        return Convert.ToString((*(ushort*)(pStorage)), 2).PadLeft(16, '0');
                                    case VariableDisplayFormat.HEX:
                                        return Convert.ToString((*(ushort*)(pStorage)), 16).PadLeft(4, '0');
                                    case VariableDisplayFormat.OCT:
                                        return Convert.ToString((*(ushort*)(pStorage)), 8).PadLeft(6, '0');
                                    default:
                                        return "N/A";
                                }
                            case VariableDataType.INT:
                                switch (DisplayFormat)
                                {
                                    case VariableDisplayFormat.DEC:
                                        return (*(short*)(pStorage)).ToString();
                                    case VariableDisplayFormat.BIN:
                                        return Convert.ToString((*(short*)(pStorage)), 2).PadLeft(16, '0');
                                    case VariableDisplayFormat.HEX:
                                        return Convert.ToString((*(short*)(pStorage)), 16).PadLeft(4, '0');
                                    case VariableDisplayFormat.OCT:
                                        return Convert.ToString((*(short*)(pStorage)), 8).PadLeft(6, '0');
                                    default:
                                        return "N/A";
                                }
                            case VariableDataType.UDINT:
                                switch (DisplayFormat)
                                {
                                    case VariableDisplayFormat.DEC:
                                        return (*(uint*)(pStorage)).ToString();
                                    case VariableDisplayFormat.BIN:
                                        return Convert.ToString((*(uint*)(pStorage)), 2).PadLeft(32, '0');
                                    case VariableDisplayFormat.HEX:
                                        return Convert.ToString((*(uint*)(pStorage)), 16).PadLeft(8, '0');
                                    case VariableDisplayFormat.OCT:
                                        return Convert.ToString((*(uint*)(pStorage)), 8).PadLeft(11, '0');
                                    default:
                                        return "N/A";
                                }
                            case VariableDataType.DINT:
                                switch (DisplayFormat)
                                {
                                    case VariableDisplayFormat.DEC:
                                        return (*(int*)(pStorage)).ToString();
                                    case VariableDisplayFormat.BIN:
                                        return Convert.ToString((*(int*)(pStorage)), 2).PadLeft(32, '0');
                                    case VariableDisplayFormat.HEX:
                                        return Convert.ToString((*(int*)(pStorage)), 16).PadLeft(8, '0');
                                    case VariableDisplayFormat.OCT:
                                        return Convert.ToString((*(int*)(pStorage)), 8).PadLeft(11, '0');
                                    default:
                                        return "N/A";
                                }
                            case VariableDataType.ULINT:
                                switch (DisplayFormat)
                                {
                                    case VariableDisplayFormat.HEX:
                                        return (*(ulong*)(pStorage)).ToString("X016");
                                    default:
                                        return (*(ulong*)(pStorage)).ToString();
                                }
                            case VariableDataType.LINT:
                                switch (DisplayFormat)
                                {
                                    case VariableDisplayFormat.HEX:
                                        return (*(long*)(pStorage)).ToString("X016");
                                    default:
                                        return (*(long*)(pStorage)).ToString();
                                }
                            case VariableDataType.REAL: return (*(float*)(pStorage)).ToString("G9");
                            case VariableDataType.BYTE_ARRAY:
                                switch (DisplayFormat)
                                {
                                    case VariableDisplayFormat.HEX:
                                        return string.Join(" ", __data_storage.Select(d => d.ToString("X02")));
                                    default:
                                        return string.Join(" ", __data_storage.Select(d => d.ToString()));
                                }
                            default:
                                return "N/A";
                        }
                }
            }
            set
            {
                try
                {
                    unsafe
                    {
                        fixed (byte* pStorage = __data_storage)
                            switch (DataType)
                            {
                                case VariableDataType.BOOL:
                                    if (value.ToLower() == "true" || value.ToLower() == "t")
                                        __data_storage[0] |= (byte)(1 << (int)(GlobalBitOffset % 8));
                                    else if (value.ToLower() == "false" || value.ToLower() == "f")
                                        __data_storage[0] &= (byte)~(1 << (int)(GlobalBitOffset % 8));
                                    break;
                                case VariableDataType.USINT:
                                    *(byte*)pStorage = Convert.ToByte(value, (int)DisplayFormat);
                                    break;
                                case VariableDataType.SINT:
                                    *(sbyte*)pStorage = Convert.ToSByte(value, (int)DisplayFormat);
                                    break;
                                case VariableDataType.UINT:
                                    *(ushort*)pStorage = Convert.ToUInt16(value, (int)DisplayFormat);
                                    break;
                                case VariableDataType.INT:
                                    *(short*)pStorage = Convert.ToInt16(value, (int)DisplayFormat);
                                    break;
                                case VariableDataType.UDINT:
                                    *(uint*)pStorage = Convert.ToUInt32(value, (int)DisplayFormat);
                                    break;
                                case VariableDataType.DINT:
                                    *(int*)pStorage = Convert.ToInt32(value, (int)DisplayFormat);
                                    break;
                                case VariableDataType.ULINT:
                                    *(ulong*)pStorage = Convert.ToUInt64(value, (int)DisplayFormat);
                                    break;
                                case VariableDataType.LINT:
                                    *(long*)pStorage = Convert.ToInt64(value, (int)DisplayFormat);
                                    break;
                                case VariableDataType.REAL:
                                    *(float*)pStorage = Convert.ToSingle(value);
                                    break;
                                case VariableDataType.BYTE_ARRAY:
                                    byte[] data = null;
                                    if (DisplayFormat == VariableDisplayFormat.HEX)
                                        data = value.Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
                                    else if (DisplayFormat == VariableDisplayFormat.DEC)
                                        data = value.Split(' ').Select(s => Convert.ToByte(s, 10)).ToArray();

                                    if (data.Length != __data_storage.Length)
                                        throw new ArgumentException();
                                    data.CopyTo(__data_storage, 0);
                                    break;
                                default:
                                    throw new ArgumentException();
                            }
                    }

                    RxRequest = true;
                    _notify_property_changed();
                }
                catch
                {
                    throw;
                }
            }
        }
    }

    public class Pdo
    {
        public ushort Index { get; }
        public string Name { get; }
        public string Access { get; }
        private List<Variable> __variables;
        public IReadOnlyList<Variable> Variables { get; }

        public override string ToString()
        {
            return $"[0x{Index:X4}] [{Access}] {Name}";
        }

        public bool IsReadOnly { get { return Access == "Tx"; } }

        public Pdo(ushort index, string name, bool tx, IEnumerable<Variable> variables)
        {
            Index = index;
            Name = name;
            Access = tx ? "Tx" : "Rx";
            __variables = new List<Variable>(variables);
            Variables = __variables;
        }
    }

    public class Slave : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void _notify_property_changed([CallerMemberName] String propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ushort Addr { get; }
        public string Name { get; }
        public uint VendorID { get; }
        public uint ProductCode { get; }
        public uint RevisionNumber { get; }
        public List<Pdo> __tx_pdos;
        public List<Pdo> __rx_pdos;
        public IReadOnlyList<Pdo> TxPDOs { get; }
        public IReadOnlyList<Pdo> RxPDOs { get; }
        public uint TxGlobalByteOffset { get; }
        public uint RxGlobalByteOffset { get; }
        public override string ToString()
        {
            return $"[{Addr:D5}] {Name}";
        }

        private bool __io_status = false;
        public bool IOStatus
        {
            get { return __io_status; }
            set
            {
                if (value != __io_status)
                {
                    __io_status = value;
                    _notify_property_changed();
                }
            }
        }

        private ushort __error_status = 0;
        public ushort ErrorStatus
        {
            get { return __error_status; }
            set
            {
                if (value != __error_status)
                {
                    __error_status = value;
                    _notify_property_changed();
                }
            }
        }

        public ushort __esm = (ushort)STATE_MACHINE_T.NONE;
        public ushort ESM
        {
            get { return __esm; }
            set
            {
                if (value != __esm)
                {
                    __esm = value;
                    _notify_property_changed();
                }
            }
        }

        public IEnumerable<Variable> TxVariables => TxPDOs.SelectMany(x => x.Variables);
        public IEnumerable<Variable> RxVariables => RxPDOs.SelectMany(x => x.Variables);

        public Slave(ushort addr, string name, uint vendor, uint product, uint revision, uint txByteOffset, uint rxByteOffset, IEnumerable<Pdo> txPdos, IEnumerable<Pdo> rxPdos)
        {
            Addr = addr;
            Name = name;
            VendorID = vendor;
            ProductCode = product;
            RevisionNumber = revision;
            TxGlobalByteOffset = txByteOffset;
            RxGlobalByteOffset = rxByteOffset;
            __tx_pdos = new List<Pdo>(txPdos);
            __rx_pdos = new List<Pdo>(rxPdos);
            TxPDOs = __tx_pdos;
            RxPDOs = __rx_pdos;
        }
    }
}
