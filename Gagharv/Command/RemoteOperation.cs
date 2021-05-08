using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Command
{ 
    public enum REMOTE_OPERATION_T
    {
        RUN,
        STOP,
        PAUSE,
        LATCH_CLEAR,
        RESET,
        READ_TYPE_NAME
    }
    
    public enum REMOTE_CONTROL_MODE_T:ushort
    {
        FORCED_EXECUTION_NOT_ALLOWED    = 0x0001,
        FORCED_EXECUTION_ALLOWED        = 0x0003
    }

    public enum REMOTE_CLEAR_MODE_T : byte
    {
        DO_NOT_CLEAR_DEVICE             = 0x00,
        CLEAR_DEVICE_EXCEPT_LATCHED     = 0x01,
        CLEAR_ALL_DEVICE                = 0x02
    }

    public class RemoteOperation
    {
        private static readonly byte __TYPE_NAME_LENGTH = 16;
        
        public static int REMOTE_OPERATION_REQUEST_LENGTH(MESSAGE_DATA_CODE_T dataCode, REMOTE_OPERATION_T operation)
        {
            int asc = 0;
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    asc = 2;
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    asc = 1;
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            switch (operation)
            {
                case REMOTE_OPERATION_T.RUN:
                    return 4 * asc;
                case REMOTE_OPERATION_T.STOP:
                case REMOTE_OPERATION_T.PAUSE:
                case REMOTE_OPERATION_T.LATCH_CLEAR:
                case REMOTE_OPERATION_T.RESET:
                    return 2 * asc;
                case REMOTE_OPERATION_T.READ_TYPE_NAME:
                    return 0;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_REMOTE_OPERATION);
            }
        }

        public static int REMOTE_OPERATION_RESPONSE_LENGTH(MESSAGE_DATA_CODE_T dataCode, REMOTE_OPERATION_T operation)
        {
            int asc = 0;
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    asc = 2;
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    asc = 1;
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            switch (operation)
            {
                case REMOTE_OPERATION_T.RUN:
                case REMOTE_OPERATION_T.STOP:
                case REMOTE_OPERATION_T.PAUSE:
                case REMOTE_OPERATION_T.LATCH_CLEAR:
                case REMOTE_OPERATION_T.RESET:
                    return 0;
                case REMOTE_OPERATION_T.READ_TYPE_NAME:
                    return __TYPE_NAME_LENGTH + 2 * asc;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_REMOTE_OPERATION);
            }
        }


        public static int BUILD_REMOTE_RUN_REQUEST(MESSAGE_DATA_CODE_T dataCode, REMOTE_CONTROL_MODE_T controlMode, REMOTE_CLEAR_MODE_T clearMode,
            byte[] dataArray, int startIndex)
        {
            int index = startIndex;
            if (dataArray.Length - startIndex < REMOTE_OPERATION_REQUEST_LENGTH(dataCode, REMOTE_OPERATION_T.RUN))
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    index += Message.Message.BINARY_TO_ASCII_ARRAY((ushort)controlMode, dataArray, index);
                    index += Message.Message.BINARY_TO_ASCII_ARRAY((byte)clearMode, dataArray, index);
                    index += Message.Message.BINARY_TO_ASCII_ARRAY((byte)0, dataArray, index);
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    index += Message.Message.BINARY_TO_BINARY_ARRAY((ushort)controlMode, dataArray, index);
                    index += Message.Message.BINARY_TO_BINARY_ARRAY((byte)clearMode, dataArray, index);
                    index += Message.Message.BINARY_TO_BINARY_ARRAY((byte)0, dataArray, index);
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return index - startIndex;
        }

        public static int BUILD_REMOTE_STOP_REQUEST(MESSAGE_DATA_CODE_T dataCode, byte[] dataArray, int startIndex)
        {
            int index = startIndex;
            if (dataArray.Length - startIndex < REMOTE_OPERATION_REQUEST_LENGTH(dataCode, REMOTE_OPERATION_T.STOP))
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    index += Message.Message.BINARY_TO_ASCII_ARRAY((ushort)0x0001, dataArray, index);
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    index += Message.Message.BINARY_TO_BINARY_ARRAY((ushort)0x0001, dataArray, index);
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return index - startIndex;
        }

        public static int BUILD_REMOTE_PAUSE_REQUEST(MESSAGE_DATA_CODE_T dataCode, REMOTE_CONTROL_MODE_T controlMode, byte[] dataArray, int startIndex)
        {
            int index = startIndex;
            if (dataArray.Length - startIndex < REMOTE_OPERATION_REQUEST_LENGTH(dataCode, REMOTE_OPERATION_T.PAUSE))
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    index += Message.Message.BINARY_TO_ASCII_ARRAY((ushort)controlMode, dataArray, index);
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    index += Message.Message.BINARY_TO_BINARY_ARRAY((ushort)controlMode, dataArray, index);
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return index - startIndex;
        }

        public static int BUILD_REMOTE_LATCH_CLEAR_REQUEST(MESSAGE_DATA_CODE_T dataCode, byte[] dataArray, int startIndex)
        {
            int index = startIndex;
            if (dataArray.Length - startIndex < REMOTE_OPERATION_REQUEST_LENGTH(dataCode, REMOTE_OPERATION_T.LATCH_CLEAR))
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    index += Message.Message.BINARY_TO_ASCII_ARRAY((ushort)0x0001, dataArray, index);
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    index += Message.Message.BINARY_TO_BINARY_ARRAY((ushort)0x0001, dataArray, index);
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return index - startIndex;
        }

        public static int BUILD_REMOTE_RESET_REQUEST(MESSAGE_DATA_CODE_T dataCode, byte[] dataArray, int startIndex)
        {
            int index = startIndex;
            if (dataArray.Length - startIndex < REMOTE_OPERATION_REQUEST_LENGTH(dataCode, REMOTE_OPERATION_T.RESET))
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    index += Message.Message.BINARY_TO_ASCII_ARRAY((ushort)0x0001, dataArray, index);
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    index += Message.Message.BINARY_TO_BINARY_ARRAY((ushort)0x0001, dataArray, index);
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return index - startIndex;
        }

        public static int PARSE_REMOTE_READ_TYPE_NAME_RESPONSE(MESSAGE_DATA_CODE_T dataCode, ReadOnlySpan<byte> source, out string modelName, out ushort modelCode)
        {
            int index = 0;
            if (source.Length < REMOTE_OPERATION_REQUEST_LENGTH(dataCode, REMOTE_OPERATION_T.READ_TYPE_NAME))
                throw new SLMPException(SLMP_EXCEPTION_CODE_T.INSUFFICIENT_DATA_ARRAY_BUFFER);

            modelName = System.Text.Encoding.ASCII.GetString(source.Slice(0, __TYPE_NAME_LENGTH));
            index += 16;
            switch (dataCode)
            {
                case MESSAGE_DATA_CODE_T.ASCII:
                    try
                    {
                        modelCode = Convert.ToUInt16(System.Text.Encoding.ASCII.GetString(source.Slice(__TYPE_NAME_LENGTH, 4)), 16);
                    }
                    catch (Exception)
                    {
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_MODEL_CODE);
                    }
                    index += 4;
                    break;
                case MESSAGE_DATA_CODE_T.BINARY:
                    modelCode = (ushort)(source[__TYPE_NAME_LENGTH] + (source[__TYPE_NAME_LENGTH + 1] << 8));
                    index += 2;
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_DATA_CODE);
            }
            return index;
        }
    }
}
