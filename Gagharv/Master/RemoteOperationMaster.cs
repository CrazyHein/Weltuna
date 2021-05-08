using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Command;
using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.IOUtility;
using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Master
{
    public class RemoteOperationMaster
    {
        private SocketInterface __socket;
        private MESSAGE_FRAME_TYPE_T __frame_type;
        private MESSAGE_DATA_CODE_T __data_code;
        private byte[] __send_byte_array, __receive_byte_array;
        private object __sync_object;
        private DESTINATION_ADDRESS_T __destination;
        private Random __serial_number_generator;
        private int __command_header_length;
        private int __response_message_header_length;
        private SUB_COMMANDS_T __subcommand;

        public SocketInterface COM
        {
            set
            {
                lock (__sync_object)
                {
                    __socket = value;
                }
            }
        }

        public RemoteOperationMaster(MESSAGE_FRAME_TYPE_T frameType, MESSAGE_DATA_CODE_T dataCode, bool dedicationR, SocketInterface sc, ref DESTINATION_ADDRESS_T destination, int sendBufferSize = 4096, int receiveBufferSize = 4096, object sync = null)
        {
            __socket = sc;
            __frame_type = frameType;
            __data_code = dataCode;
            __destination = destination;
            __send_byte_array = new byte[sendBufferSize];
            __receive_byte_array = new byte[receiveBufferSize];
            __sync_object = sync ?? new object();

            switch (frameType, dataCode, dedicationR)
            {
                case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.ASCII, false):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_ASCII_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_3E_ASCII_T>();
                    __subcommand = 0;
                    __serial_number_generator = null;
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.ASCII, true):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_ASCII_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_3E_ASCII_T>();
                    __subcommand = SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION;
                    __serial_number_generator = null;
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.BINARY, false):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_3E_BINARY_T>();
                    __subcommand = 0;
                    __serial_number_generator = null;
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.BINARY, true):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_3E_BINARY_T>();
                    __subcommand = SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION;
                    __serial_number_generator = null;
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.ASCII, false):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_ASCII_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_4E_ASCII_T>();
                    __subcommand = 0;
                    __serial_number_generator = new Random();
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.ASCII, true):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_ASCII_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_4E_ASCII_T>();
                    __subcommand = SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION;
                    __serial_number_generator = new Random();
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.BINARY, false):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_4E_BINARY_T>();
                    __subcommand = 0;
                    __serial_number_generator = new Random();
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.BINARY, true):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_4E_BINARY_T>();
                    __subcommand = SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION;
                    __serial_number_generator = new Random();
                    break;
                case (MESSAGE_FRAME_TYPE_T.STATION_NUM_EXTENSION, MESSAGE_DATA_CODE_T.BINARY, false):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_EX_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_EX_BINARY_T>();
                    __subcommand = 0;
                    __serial_number_generator = new Random();
                    break;
                case (MESSAGE_FRAME_TYPE_T.STATION_NUM_EXTENSION, MESSAGE_DATA_CODE_T.BINARY, true):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_EX_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_EX_BINARY_T>();
                    __subcommand = SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION;
                    __serial_number_generator = new Random();
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_SUBHEADER);
            }
        }

        private void __post_remote_operation_request(ref DESTINATION_ADDRESS_T destination, ushort monitoringTimer, 
            REMOTE_OPERATION_T operation, REMOTE_CONTROL_MODE_T controlMode, REMOTE_CLEAR_MODE_T clearMode, out ushort endCode)
        {
            lock (__sync_object)
            {
                MESSAGE_FRAME_TYPE_T frameType;
                byte destinationNetwork;
                byte destinationStation;
                ushort destinationModuleIO;
                byte destinationMultidrop;
                ushort destinationExtensionStation;
                ushort serialNo0;
                ushort serialNo1;
                ushort responseDataLength;

                int offset = 0;
                serialNo0 = __serial_number_generator == null ? (ushort)0 : (ushort)__serial_number_generator.Next(65536);

                offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                        destination.network_number, destination.station_number,
                                        destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                        (ushort)(__command_header_length + RemoteOperation.REMOTE_OPERATION_REQUEST_LENGTH(__data_code, operation)), monitoringTimer,
                                        __send_byte_array, 0);

                switch (operation)
                {
                    case REMOTE_OPERATION_T.RUN:
                        offset += RequestCommand.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, COMMANDS_T.REMOTE_RUN, __subcommand,
                                    __send_byte_array, offset);
                        offset += RemoteOperation.BUILD_REMOTE_RUN_REQUEST(__data_code, controlMode, clearMode, __send_byte_array, offset);
                        break;
                    case REMOTE_OPERATION_T.STOP:
                        offset += RequestCommand.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, COMMANDS_T.REMOTE_STOP, __subcommand,
                                    __send_byte_array, offset);
                        offset += RemoteOperation.BUILD_REMOTE_STOP_REQUEST(__data_code, __send_byte_array, offset);
                        break;
                    case REMOTE_OPERATION_T.PAUSE:
                        offset += RequestCommand.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, COMMANDS_T.REMOTE_PAUSE, __subcommand,
                                    __send_byte_array, offset);
                        offset += RemoteOperation.BUILD_REMOTE_PAUSE_REQUEST(__data_code, controlMode, __send_byte_array, offset);
                        break;
                    case REMOTE_OPERATION_T.LATCH_CLEAR:
                        offset += RequestCommand.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, COMMANDS_T.REMOTE_LATCH_CLEAR, __subcommand,
                                    __send_byte_array, offset);
                        offset += RemoteOperation.BUILD_REMOTE_LATCH_CLEAR_REQUEST(__data_code, __send_byte_array, offset);
                        break;
                    case REMOTE_OPERATION_T.RESET:
                        offset += RequestCommand.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, COMMANDS_T.REMOTE_RESET, __subcommand,
                                    __send_byte_array, offset);
                        offset += RemoteOperation.BUILD_REMOTE_RESET_REQUEST(__data_code, __send_byte_array, offset);
                        break;
                    default:
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_REMOTE_OPERATION);
                }
                __socket.Send(__send_byte_array, 0, offset);

                __socket.Receive(__receive_byte_array, 0, __response_message_header_length);
                ResponseMessage.PARSE_BYTE_ARRAY_HEADER(__receive_byte_array.AsSpan(0, __response_message_header_length), __data_code,
                    out frameType, out destinationNetwork, out destinationStation, out destinationModuleIO, out destinationMultidrop, out destinationExtensionStation,
                    out serialNo1, out responseDataLength, out endCode);

                __socket.Receive(__receive_byte_array, __response_message_header_length, responseDataLength);

                if (frameType != __frame_type || destinationNetwork != __destination.network_number ||
                    destinationStation != __destination.station_number ||
                    destinationModuleIO != __destination.module_io ||
                    destinationMultidrop != __destination.multidrop_number ||
                    destinationExtensionStation != __destination.extension_station_number ||
                    serialNo1 != serialNo0)
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.RECEIVED_UNMATCHED_MESSAGE);

                if (endCode == (ushort)RESPONSE_MESSAGE_ENDCODE_T.NO_ERROR)
                {
                    if (RemoteOperation.REMOTE_OPERATION_RESPONSE_LENGTH(__data_code, operation) != responseDataLength)
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
                }
            }
        }

        public void Run(ushort monitoringTimer, REMOTE_CONTROL_MODE_T controlMode, REMOTE_CLEAR_MODE_T clearMode, out ushort endCode)
        {
            __post_remote_operation_request(ref __destination, monitoringTimer, REMOTE_OPERATION_T.RUN, controlMode, clearMode, out endCode);
        }

        public void Stop(ushort monitoringTimer, out ushort endCode)
        {
            __post_remote_operation_request(ref __destination, monitoringTimer, REMOTE_OPERATION_T.STOP, 0, 0, out endCode);
        }

        public void Pause(ushort monitoringTimer, REMOTE_CONTROL_MODE_T controlMode, out ushort endCode)
        {
            __post_remote_operation_request(ref __destination, monitoringTimer, REMOTE_OPERATION_T.PAUSE, controlMode, 0, out endCode);
        }

        public void LatchClear(ushort monitoringTimer, out ushort endCode)
        {
            __post_remote_operation_request(ref __destination, monitoringTimer, REMOTE_OPERATION_T.LATCH_CLEAR, 0, 0, out endCode);
        }

        public void Reset(ushort monitoringTimer, out ushort endCode)
        {
            __post_remote_operation_request(ref __destination, monitoringTimer, REMOTE_OPERATION_T.RESET, 0, 0, out endCode);
        }

        public void ReadTypeName(ushort monitoringTimer, out ushort endCode, out string modelName, out ushort modelCode)
        {
            lock (__sync_object)
            {
                MESSAGE_FRAME_TYPE_T frameType;
                byte destinationNetwork;
                byte destinationStation;
                ushort destinationModuleIO;
                byte destinationMultidrop;
                ushort destinationExtensionStation;
                ushort serialNo0;
                ushort serialNo1;
                ushort responseDataLength;

                int offset = 0;
                serialNo0 = __serial_number_generator == null ? (ushort)0 : (ushort)__serial_number_generator.Next(65536);

                offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                        __destination.network_number, __destination.station_number,
                                        __destination.module_io, __destination.multidrop_number, __destination.extension_station_number,
                                        (ushort)(__command_header_length + RemoteOperation.REMOTE_OPERATION_REQUEST_LENGTH(__data_code, REMOTE_OPERATION_T.READ_TYPE_NAME)), monitoringTimer,
                                        __send_byte_array, 0);
                offset += RequestCommand.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, COMMANDS_T.TYPE_NAME_READ, __subcommand,
                                        __send_byte_array, offset);

                __socket.Send(__send_byte_array, 0, offset);

                __socket.Receive(__receive_byte_array, 0, __response_message_header_length);
                ResponseMessage.PARSE_BYTE_ARRAY_HEADER(__receive_byte_array.AsSpan(0, __response_message_header_length), __data_code,
                    out frameType, out destinationNetwork, out destinationStation, out destinationModuleIO, out destinationMultidrop, out destinationExtensionStation,
                    out serialNo1, out responseDataLength, out endCode);

                __socket.Receive(__receive_byte_array, __response_message_header_length, responseDataLength);

                if (frameType != __frame_type || destinationNetwork != __destination.network_number ||
                    destinationStation != __destination.station_number ||
                    destinationModuleIO != __destination.module_io ||
                    destinationMultidrop != __destination.multidrop_number ||
                    destinationExtensionStation != __destination.extension_station_number ||
                    serialNo1 != serialNo0)
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.RECEIVED_UNMATCHED_MESSAGE);

                if (endCode == (ushort)RESPONSE_MESSAGE_ENDCODE_T.NO_ERROR)
                {
                    if (RemoteOperation.REMOTE_OPERATION_RESPONSE_LENGTH(__data_code, REMOTE_OPERATION_T.READ_TYPE_NAME) != responseDataLength)
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
                    RemoteOperation.PARSE_REMOTE_READ_TYPE_NAME_RESPONSE(__data_code, __receive_byte_array.AsSpan(__response_message_header_length, responseDataLength),
                        out modelName, out modelCode);
                }
                else
                {
                    modelName = null;
                    modelCode = 0;
                }
            }
        }
    }
}
