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
    public class DeviceAccessMaster
    {
        private SocketInterface __socket;
        private MESSAGE_FRAME_TYPE_T __frame_type;
        private MESSAGE_DATA_CODE_T __data_code;
        private byte[] __send_byte_array, __receive_byte_array;
        private object __sync_object;
        private DESTINATION_ADDRESS_T __destination;
        private Random __serial_number_generator;
        private int __command_header_length;
        private int __device_specification_length;
        private int __device_extension_specification_length;
        private int __response_message_header_length;
        private int __device_points_length;
        SUB_COMMANDS_T __subcommand;


        public DeviceAccessMaster(MESSAGE_FRAME_TYPE_T frameType, MESSAGE_DATA_CODE_T dataCode, bool dedicationR, SocketInterface sc, ref DESTINATION_ADDRESS_T destination, int sendBufferSize, int receiveBufferSize, object sync)
        {
            __socket = sc;
            __frame_type = frameType;
            __data_code = dataCode;
            __destination = destination;
            __send_byte_array = new byte[sendBufferSize];
            __receive_byte_array = new byte[receiveBufferSize];
            __sync_object = sync;

            switch (frameType, dataCode, dedicationR)
            {
                case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.ASCII, false):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_ASCII_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_ASCII_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_3E_ASCII_T>();
                    __device_points_length = 4;
                    __subcommand = 0;
                    __serial_number_generator = null;
                    break;

                case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.ASCII, true):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_ASCII_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_ASCII_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_4E_ASCII_T>();
                    __device_points_length = 4;
                    __subcommand = SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION;
                    __serial_number_generator = new Random();
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.BINARY, false):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_BINARY_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_3E_BINARY_T>();
                    __device_points_length = 2;
                    __subcommand = 0;
                    __serial_number_generator = null;
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.BINARY, true):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_BINARY_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_4E_BINARY_T>();
                    __device_points_length = 2;
                    __subcommand = SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION;
                    __serial_number_generator = new Random();
                    break;
                case (MESSAGE_FRAME_TYPE_T.STATION_NUM_EXTENSION, MESSAGE_DATA_CODE_T.BINARY, false):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_EX_BINARY_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_BINARY_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_EX_BINARY_T>();
                    __device_points_length = 2;
                    __subcommand = 0;
                    __serial_number_generator = new Random();
                    break;
                case (MESSAGE_FRAME_TYPE_T.STATION_NUM_EXTENSION, MESSAGE_DATA_CODE_T.BINARY, true):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_EX_BINARY_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_BINARY_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_EX_BINARY_T>();
                    __device_points_length = 2;
                    __subcommand = SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION;
                    __serial_number_generator = new Random();
                    break;
                default:
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.INVALID_SUBHEADER);
            }
        }


        private void __read_device(ref DESTINATION_ADDRESS_T destination, SUB_COMMANDS_T subcommand,
            string extension, string extensionModification, string deviceModification, string indirectSpecification,
            ushort monitoringTimer, string deviceCode, uint headDevice, ushort devicePoints, out ushort endCode, Span<ushort> worddata, Span<byte> bytedata)
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
                if (__frame_type == MESSAGE_FRAME_TYPE_T.MC_3E)
                    serialNo0 = 0;
                else
                    serialNo0 = (ushort)__serial_number_generator.Next(65536);
                if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) == 0)
                    offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                        destination.network_number, destination.station_number,
                                        destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                        (ushort)(__command_header_length + __device_specification_length + __device_points_length), monitoringTimer,
                                        __send_byte_array, 0);
                else
                    offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                        destination.network_number, destination.station_number,
                                        destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                        (ushort)(__command_header_length + __device_extension_specification_length + __device_points_length), monitoringTimer,
                                        __send_byte_array, 0);

                offset += RequestCommand.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, COMMANDS_T.DEVICE_READ, subcommand,
                                    __send_byte_array, offset);
                offset += DeviceAccess.BUILD_DEVICE_READ_WRITE_BYTE_ARRAY_HEADER(__frame_type, __data_code, subcommand, deviceCode, headDevice, devicePoints,
                                    extension, extensionModification, deviceModification, indirectSpecification,
                                    __send_byte_array, offset);

                __socket.Send(__send_byte_array, 0, offset);

                __socket.Receive(__receive_byte_array, 0, __response_message_header_length);
                ResponseMessage.PARSE_BYTE_ARRAY_HEADER(__receive_byte_array.AsSpan(0, __response_message_header_length), __data_code,
                    out frameType, out destinationNetwork, out destinationStation, out destinationModuleIO, out destinationMultidrop, out destinationExtensionStation,
                    out serialNo1, out responseDataLength, out endCode);


                __socket.Receive(__receive_byte_array, __response_message_header_length, responseDataLength);

                if (frameType != __frame_type || destinationNetwork != destination.network_number ||
                    destinationStation != destination.station_number ||
                    destinationModuleIO != destination.module_io ||
                    destinationMultidrop != destination.multidrop_number ||
                    destinationExtensionStation != destination.extension_station_number ||
                    serialNo1 != serialNo0)
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.RECEIVED_UNMATCHED_MESSAGE);

                if (endCode == (ushort)RESPONSE_MESSAGE_ENDCODE_T.NO_ERROR)
                {
                    if (DeviceAccess.DEIVICE_REGISTER_DATA_ARRAY_LENGTH(__data_code, subcommand, devicePoints) != responseDataLength)
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
                    if ((subcommand & SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT) != 0)
                        DeviceAccess.READ_BIT_DEVICE_IN_BIT_UNIT(__receive_byte_array.AsSpan(__response_message_header_length, responseDataLength), __data_code, devicePoints, bytedata);
                    else
                        DeviceAccess.READ_DEVICE_IN_WORD_UNIT(__receive_byte_array.AsSpan(__response_message_header_length, responseDataLength), __data_code, devicePoints, worddata);
                }
            }
        }

        public void ReadLocalDeviceInWord(ushort monitoringTimer, string deviceCode, uint headDevice, ushort devicePoints, out ushort endCode, Span<ushort> data)
        {
            __read_device(ref __destination,
                            __subcommand,
                            null, null, null, null,
                            monitoringTimer, deviceCode, headDevice, devicePoints, out endCode, data, null);

        }

        public void ReadLocalDeviceInBit(ushort monitoringTimer, string deviceCode, uint headDevice, ushort devicePoints, out ushort endCode, Span<byte> data)
        {
            __read_device(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT,
                            null, null, null, null,
                            monitoringTimer, deviceCode, headDevice, devicePoints, out endCode, null, data);
        }

        public void ReadModuleAccessDeviceInWord(ushort monitoringTimer, string extensionSepcification, uint headDevice, ushort devicePoints, out ushort endCode, Span<ushort> data)
        {
            __read_device(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                            extensionSepcification, null, null, null,
                            monitoringTimer, "G", headDevice, devicePoints, out endCode, data, null);
        }

        private void __write_device(ref DESTINATION_ADDRESS_T destination, SUB_COMMANDS_T subcommand,
            string extension, string extensionModification, string deviceModification, string indirectSpecification,
            ushort monitoringTimer, string deviceCode, uint headDevice, ushort devicePoints, out ushort endCode, ReadOnlySpan<ushort> worddata, ReadOnlySpan<byte> bytedata)
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

                int devicePointLength = DeviceAccess.DEIVICE_REGISTER_DATA_ARRAY_LENGTH(__data_code, subcommand, devicePoints);

                int offset = 0;
                if (__frame_type == MESSAGE_FRAME_TYPE_T.MC_3E)
                    serialNo0 = 0;
                else
                    serialNo0 = (ushort)__serial_number_generator.Next(65536);

                if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) == 0)
                    offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                    destination.network_number, destination.station_number,
                                    destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                    (ushort)(__command_header_length + __device_specification_length + __device_points_length + devicePointLength), monitoringTimer,
                                    __send_byte_array, 0);
                else
                    offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                    destination.network_number, destination.station_number,
                                    destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                    (ushort)(__command_header_length + __device_extension_specification_length + __device_points_length + devicePointLength), monitoringTimer,
                                    __send_byte_array, 0);

                offset += RequestCommand.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, COMMANDS_T.DEVICE_WRITE, subcommand,
                                    __send_byte_array, offset);
                offset += DeviceAccess.BUILD_DEVICE_READ_WRITE_BYTE_ARRAY_HEADER(__frame_type, __data_code, subcommand, deviceCode, headDevice, devicePoints,
                                    extension, extensionModification, deviceModification, indirectSpecification,
                                    __send_byte_array, offset);

                if ((subcommand & SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT) != 0)
                    DeviceAccess.WRITE_BIT_DEVICE_IN_BIT_UNIT(bytedata, __data_code, devicePoints, __send_byte_array.AsSpan(offset, devicePointLength));
                else
                    DeviceAccess.WRITE_DEVICE_IN_WORD_UNIT(worddata, __data_code, devicePoints, __send_byte_array.AsSpan(offset, devicePointLength));
                offset += devicePointLength;

                __socket.Send(__send_byte_array, 0, offset);

                __socket.Receive(__receive_byte_array, 0, __response_message_header_length);
                ResponseMessage.PARSE_BYTE_ARRAY_HEADER(__receive_byte_array.AsSpan(0, __response_message_header_length), __data_code,
                    out frameType, out destinationNetwork, out destinationStation, out destinationModuleIO, out destinationMultidrop, out destinationExtensionStation,
                    out serialNo1, out responseDataLength, out endCode);


                __socket.Receive(__receive_byte_array, __response_message_header_length, responseDataLength);

                if (frameType != __frame_type || destinationNetwork != destination.network_number ||
                    destinationStation != destination.station_number ||
                    destinationModuleIO != destination.module_io ||
                    destinationMultidrop != destination.multidrop_number ||
                    destinationExtensionStation != destination.extension_station_number ||
                    serialNo1 != serialNo0)
                    throw new SLMPException(SLMP_EXCEPTION_CODE_T.RECEIVED_UNMATCHED_MESSAGE);

                if (endCode == (ushort)RESPONSE_MESSAGE_ENDCODE_T.NO_ERROR)
                {
                    if (responseDataLength != 0)
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
                }
            }
        }

        public void WriteLocalDeviceInWord(ushort monitoringTimer, string deviceCode, uint headDevice, ushort devicePoints, out ushort endCode, ReadOnlySpan<ushort> data)
        {
            __write_device(ref __destination,
                            __subcommand,
                            null, null, null, null,
                            monitoringTimer, deviceCode, headDevice, devicePoints, out endCode, data, null);
        }

        public void WriteLocalDeviceInBit(ushort monitoringTimer, string deviceCode, uint headDevice, ushort devicePoints, out ushort endCode, ReadOnlySpan<byte> data)
        {
            __write_device(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT,
                            null, null, null, null,
                            monitoringTimer, deviceCode, headDevice, devicePoints, out endCode, null, data);
        }

        public void WriteModuleAccessDeviceInWord(ushort monitoringTimer, string extensionSepcification, uint headDevice, ushort devicePoints, out ushort endCode, ReadOnlySpan<ushort> data)
        {
            __write_device(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                            extensionSepcification, null, null, null,
                            monitoringTimer, "G", headDevice, devicePoints, out endCode, data, null);
        }
    }
}
