using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP
{
    public enum SLMP_EXCEPTION_CODE_T: UInt32
    {
        NO_ERROR                                                    = 0x00000000,
        RUNTIME_ERROR                                               = 0xFFFFFFFF,

        INVALID_SUBHEADER                                           = 0x00000001,
        INVALID_DATA_CODE                                           = 0x00000002,
        INVALID_FRAME_TYPE                                          = 0x00000003,
        MESSAGE_FRAME_CORRUPTED                                     = 0x00000004,

        INVALID_COMMAND_CODE                                        = 0x00000010,
        COMMAND_MESSAGE_CORRUPTED                                   = 0x00000011,

        DEVICE_ACCESS_OUT_OF_HEAD_RANGE                             = 0x00000020,
        INVALID_DEVICE_CODE                                         = 0x00000021,
        INVALID_DEVICE_INDIRECT_SPECIFICATION                       = 0x00000023,
        INVALID_DEVICE_EXTENSION_SPECIFICATION                      = 0x00000024,
        INVALID_DEVICE_EXTENSION_MODIFICATION                       = 0x00000025,
        INVALID_DEVICE_MODIFICATION                                 = 0x00000026,
        DEVICE_REGISTER_DATA_CORRUPTED                              = 0x00000027,
        INVALID_DEVICE_REGISTER_DATA                                = 0x00000028,

        REMOTE_STATION_DISCONNECTED                                 = 0x00000030,

        INVALID_REMOTE_OPERATION                                    = 0x00000040,
        INVALID_MODEL_CODE                                          = 0x00000041,

        RECEIVED_UNMATCHED_MESSAGE                                  = 0x00000080,


        INSUFFICIENT_DATA_ARRAY_BUFFER                              = 0x000000F0,
        INVALID_ASCII_CODE_VALUE                                    = 0x000000F1,
    }
    
    public class SLMPException : Exception
    {
        public SLMP_EXCEPTION_CODE_T ExceptionCode { get; private set; }
        public Exception RuntimeException { get; private set; }

        public SLMPException(SLMP_EXCEPTION_CODE_T code)
        {
            ExceptionCode = code;
        }

        public SLMPException(Exception exp)
        {
            ExceptionCode = SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR;
            RuntimeException = exp;
        }

        public override string ToString()
        {
            if (ExceptionCode != SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                return ExceptionCode.ToString();
            else if (RuntimeException != null)
                return RuntimeException.ToString();
            else
                return "UNDEFINED_RUNTIME_ERROR";
        }

        public override string Message
        {
            get
            {
                if (ExceptionCode != SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                    return ExceptionCode.ToString();
                else if (RuntimeException != null)
                    return RuntimeException.Message;
                else
                    return "UNDEFINED_RUNTIME_ERROR";
            }
        }
      
    }
}
