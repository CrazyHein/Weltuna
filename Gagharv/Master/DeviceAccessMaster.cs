﻿using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Command;
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
        private int __word_register_length;
        private int __bit_register_length;
        SUB_COMMANDS_T __subcommand;

        public SocketInterface COM
        {
            set
            {
                lock(__sync_object)
                {
                    __socket = value;
                }
            }
        }

        public DeviceAccessMaster(MESSAGE_FRAME_TYPE_T frameType, MESSAGE_DATA_CODE_T dataCode, bool dedicationR, SocketInterface sc, ref DESTINATION_ADDRESS_T destination, int sendBufferSize = 4096, int receiveBufferSize = 4096, object sync = null)
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
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_ASCII_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_3E_ASCII_T>();
                    __device_points_length = 4;
                    __word_register_length = 4;
                    __bit_register_length = 2;
                    __subcommand = 0;
                    __serial_number_generator = null;
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.ASCII, true):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_ASCII_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_ASCII_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_3E_ASCII_T>();
                    __device_points_length = 4;
                    __word_register_length = 4;
                    __bit_register_length = 4;
                    __subcommand = SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION;
                    __serial_number_generator = null;
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.BINARY, false):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_BINARY_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_3E_BINARY_T>();
                    __device_points_length = 2;
                    __word_register_length = 2;
                    __bit_register_length = 1;
                    __subcommand = 0;
                    __serial_number_generator = null;
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_3E, MESSAGE_DATA_CODE_T.BINARY, true):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_BINARY_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_3E_BINARY_T>();
                    __device_points_length = 2;
                    __word_register_length = 2;
                    __bit_register_length = 2;
                    __subcommand = SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION;
                    __serial_number_generator = null;
                    break;

                case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.ASCII, false):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_ASCII_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_ASCII_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_ASCII_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_4E_ASCII_T>();
                    __device_points_length = 4;
                    __word_register_length = 4;
                    __bit_register_length = 2;
                    __subcommand = 0;
                    __serial_number_generator = new Random();
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.ASCII, true):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_ASCII_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_ASCII_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_ASCII_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_4E_ASCII_T>();
                    __device_points_length = 4;
                    __word_register_length = 4;
                    __bit_register_length = 4;
                    __subcommand = SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION;
                    __serial_number_generator = new Random();
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.BINARY, false):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_BINARY_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_4E_BINARY_T>();
                    __device_points_length = 2;
                    __word_register_length = 2;
                    __bit_register_length = 1;
                    __subcommand = 0;
                    __serial_number_generator = new Random();
                    break;
                case (MESSAGE_FRAME_TYPE_T.MC_4E, MESSAGE_DATA_CODE_T.BINARY, true):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_3E_BINARY_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_BINARY_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_4E_BINARY_T>();
                    __device_points_length = 2;
                    __word_register_length = 2;
                    __bit_register_length = 2;
                    __subcommand = SUB_COMMANDS_T.R_MODULE_DEVICE_COMMAND_DEDICATION;
                    __serial_number_generator = new Random();
                    break;

                case (MESSAGE_FRAME_TYPE_T.STATION_NUM_EXTENSION, MESSAGE_DATA_CODE_T.BINARY, false):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_EX_BINARY_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_QL_BINARY_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_QL_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_EX_BINARY_T>();
                    __device_points_length = 2;
                    __word_register_length = 2;
                    __bit_register_length = 1;
                    __subcommand = 0;
                    __serial_number_generator = new Random();
                    break;
                case (MESSAGE_FRAME_TYPE_T.STATION_NUM_EXTENSION, MESSAGE_DATA_CODE_T.BINARY, true):
                    __command_header_length = Marshal.SizeOf<REQUEST_COMMAND_HEADER_IN_EX_BINARY_T>();
                    __device_specification_length = Marshal.SizeOf<DEVICE_SPECIFICATION_IN_R_BINARY_T>();
                    __device_extension_specification_length = Marshal.SizeOf<DEVICE_EXTENSION_SPECIFICATION_IN_R_BINARY_T>();
                    __response_message_header_length = Marshal.SizeOf<RESPONSE_MESSAGE_HEADER_IN_EX_BINARY_T>();
                    __device_points_length = 2;
                    __word_register_length = 2;
                    __bit_register_length = 2;
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
                serialNo0 = __serial_number_generator == null ? (ushort)0 : (ushort)__serial_number_generator.Next(65536);

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

        private void __read_device_random(ref DESTINATION_ADDRESS_T destination, SUB_COMMANDS_T subcommand,
            ushort monitoringTimer,
            IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice)> devicein16,
            IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice)> devicein32,
            out ushort endCode, Span<ushort> data16, Span<uint> data32)
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

                ushort device32points = (ushort)(devicein32 == null ? 0 : devicein32.Count()); 
                ushort device16points = (ushort)(devicein16 == null ? 0 : devicein16.Count());

                int offset = 0;
                serialNo0 = __serial_number_generator == null ? (ushort)0 : (ushort)__serial_number_generator.Next(65536);

                if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) == 0)
                {
                    offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                        destination.network_number, destination.station_number,
                                        destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                        (ushort)(__command_header_length + __device_specification_length * (device32points + device16points) + __device_points_length), monitoringTimer,
                                        __send_byte_array, 0);

                }
                else
                {
                    offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                        destination.network_number, destination.station_number,
                                        destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                        (ushort)(__command_header_length + __device_extension_specification_length * (device32points + device16points) + __device_points_length), monitoringTimer,
                                        __send_byte_array, 0);
                }

                offset += RequestCommand.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, COMMANDS_T.DEVICE_READ_RANDOM, subcommand,
                                        __send_byte_array, offset);

                offset += DeviceAccess.BUILD_DEVICE_READ_RANDOM_BYTE_ARRAY_HEADER(__frame_type, __data_code, subcommand, 
                                        devicein16, devicein32,
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
                    int device16bytes = DeviceAccess.DEIVICE_REGISTER_DATA_ARRAY_LENGTH(__data_code, subcommand, device16points);
                    int device32bytes = DeviceAccess.DEIVICE_REGISTER_DATA_ARRAY_LENGTH(__data_code, subcommand, device32points) * 2;

                    if (device16bytes + device32bytes != responseDataLength)
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);

                    if(device16points != 0)
                        DeviceAccess.READ_DEVICE_IN_WORD_UNIT(__receive_byte_array.AsSpan(__response_message_header_length, device16bytes), __data_code, device16points, data16);
                    if(device32points != 0)
                        DeviceAccess.READ_DEVICE_IN_DWORD_UNIT(__receive_byte_array.AsSpan(__response_message_header_length + device16bytes, device32bytes), __data_code, device32points, data32);
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

        public async Task<ushort> ReadLocalDeviceInWordAsync(ushort monitoringTimer, string deviceCode, uint headDevice, ushort devicePoints, Memory<ushort> data)
        {
            ushort end = 0;
            await Task.Run(() => __read_device(ref __destination, __subcommand,
                            null, null, null, null,
                            monitoringTimer, deviceCode, headDevice, devicePoints, out end, data.Span, null));
            return end;
        }

        public void ReadLocalDeviceInBit(ushort monitoringTimer, string deviceCode, uint headDevice, ushort devicePoints, out ushort endCode, Span<byte> data)
        {
            __read_device(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT,
                            null, null, null, null,
                            monitoringTimer, deviceCode, headDevice, devicePoints, out endCode, null, data);
        }

        public async Task<ushort> ReadLocalDeviceInBitAsync(ushort monitoringTimer, string deviceCode, uint headDevice, ushort devicePoints, Memory<byte> data)
        {
            ushort end = 0;
            await Task.Run(() => __read_device(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT,
                            null, null, null, null,
                            monitoringTimer, deviceCode, headDevice, devicePoints, out end, null, data.Span));
            return end;
        }

        public void ReadModuleAccessDeviceInWord(ushort monitoringTimer, string extensionSepcification, uint headDevice, ushort devicePoints, out ushort endCode, Span<ushort> data)
        {
            __read_device(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                            extensionSepcification, null, null, null,
                            monitoringTimer, "G", headDevice, devicePoints, out endCode, data, null);
        }

        public async Task<ushort> ReadModuleAccessDeviceInWordAsync(ushort monitoringTimer, string extensionSepcification, uint headDevice, ushort devicePoints, Memory<ushort> data)
        {
            ushort end = 0;
            await Task.Run(() => __read_device(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                            extensionSepcification, null, null, null,
                            monitoringTimer, "G", headDevice, devicePoints, out end, data.Span, null));
            return end;
        }

        public void ReadLocalDeviceInWord(ushort monitoringTimer, IEnumerable<(string deviceCode, uint headDevice)> wordDevice, IEnumerable<(string deviceCode, uint headDevice)> dwordDevice,
            out ushort endCode, Span<ushort> worddata, Span<uint> dworddata)
        {
            IEnumerable<(string, string, string, string, string, uint)> word = null;
            IEnumerable<(string, string, string, string, string, uint)> dword = null;
            if (wordDevice != null)
                word = wordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint>(null, null, null, null, d.deviceCode, d.headDevice));
            if(dwordDevice != null)
                dword = dwordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint>(null, null, null, null, d.deviceCode, d.headDevice));

            __read_device_random(ref __destination,
                            __subcommand,
                            monitoringTimer,
                            word, dword,
                            out endCode, worddata, dworddata);
        }

        public async Task<ushort> ReadLocalDeviceInWordAsync(ushort monitoringTimer, IEnumerable<(string deviceCode, uint headDevice)> wordDevice, IEnumerable<(string deviceCode, uint headDevice)> dwordDevice,
            Memory<ushort> worddata, Memory<uint> dworddata)
        {
            IEnumerable<(string, string, string, string, string, uint)> word = null;
            IEnumerable<(string, string, string, string, string, uint)> dword = null;
            if (wordDevice != null)
                word = wordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint>(null, null, null, null, d.deviceCode, d.headDevice));
            if (dwordDevice != null)
                dword = dwordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint>(null, null, null, null, d.deviceCode, d.headDevice));

            ushort end = 0;
            await Task.Run(() => __read_device_random(ref __destination,
                            __subcommand,
                            monitoringTimer,
                            word, dword,
                            out end, worddata.Span, dworddata.Span));
            return end;
        }

        public void ReadModuleAccessDeviceInWord(ushort monitoringTimer, IEnumerable<(string extensionSpecification, uint headDevice)> wordDevice, IEnumerable<(string extensionSpecification, uint headDevice)> dwordDevice, 
            out ushort endCode, Span<ushort> worddata, Span<uint> dworddata)
        {
            IEnumerable<(string, string, string, string, string, uint)> word = null;
            IEnumerable<(string, string, string, string, string, uint)> dword = null;
            if (wordDevice != null)
                word = wordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint>(d.extensionSpecification, null, null, null, "G", d.headDevice));
            if (dwordDevice != null)
                dword = dwordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint>(d.extensionSpecification, null, null, null, "G", d.headDevice));

            __read_device_random(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                            monitoringTimer,
                            word, dword,
                            out endCode, worddata, dworddata);
        }

        public async Task<ushort> ReadModuleAccessDeviceInWordAsync(ushort monitoringTimer, IEnumerable<(string extensionSpecification, uint headDevice)> wordDevice, IEnumerable<(string extensionSpecification, uint headDevice)> dwordDevice,
            Memory<ushort> worddata, Memory<uint> dworddata)
        {
            IEnumerable<(string, string, string, string, string, uint)> word = null;
            IEnumerable<(string, string, string, string, string, uint)> dword = null;
            if (wordDevice != null)
                word = wordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint>(d.extensionSpecification, null, null, null, "G", d.headDevice));
            if (dwordDevice != null)
                dword = dwordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint>(d.extensionSpecification, null, null, null, "G", d.headDevice));
            
            ushort end = 0;
            await Task.Run(() => __read_device_random(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                            monitoringTimer,
                            word, dword,
                            out end, worddata.Span, dworddata.Span));
            return end;
        }
            

        private void __read_device_block(ref DESTINATION_ADDRESS_T destination, SUB_COMMANDS_T subcommand,
            ushort monitoringTimer,
            IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice, ushort devicePoints)> wordDeviceBlock,
            IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice, ushort devicePoints)> bitDeviceBlock,
            out ushort endCode, Memory<ushort>[] wordDataBlock, Memory<ushort>[] bitDataBlock)
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

                ushort worddeviceblocks = (ushort)(wordDeviceBlock == null ? 0 : wordDeviceBlock.Count());
                ushort bitdeviceblocks = (ushort)(bitDeviceBlock == null ? 0 : bitDeviceBlock.Count());

                int offset = 0;
                serialNo0 = __serial_number_generator == null ? (ushort)0 : (ushort)__serial_number_generator.Next(65536);

                if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) == 0)
                {
                    offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                        destination.network_number, destination.station_number,
                                        destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                        (ushort)(__command_header_length + (__device_specification_length + __device_points_length) * (worddeviceblocks + bitdeviceblocks) + __device_points_length), monitoringTimer,
                                        __send_byte_array, 0);

                }
                else
                {
                    offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                        destination.network_number, destination.station_number,
                                        destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                        (ushort)(__command_header_length + (__device_extension_specification_length + __device_points_length) * (worddeviceblocks + bitdeviceblocks) + __device_points_length), monitoringTimer,
                                        __send_byte_array, 0);
                }

                offset += RequestCommand.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, COMMANDS_T.DEVICE_READ_BLOCK, subcommand,
                                        __send_byte_array, offset);

                offset += DeviceAccess.BUILD_DEVICE_READ_BLOCK_BYTE_ARRAY_HEADER(__frame_type, __data_code, subcommand,
                                        wordDeviceBlock, bitDeviceBlock,
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
                    offset = 0;
                    int block = 0;
                    int index = 0;
                    if(wordDeviceBlock != null)
                        foreach (var d in wordDeviceBlock)
                        {
                            block = DeviceAccess.DEIVICE_REGISTER_DATA_ARRAY_LENGTH(__data_code, subcommand, d.devicePoints);
                            if (responseDataLength - offset < block)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
                            DeviceAccess.READ_DEVICE_IN_WORD_UNIT(__receive_byte_array.AsSpan(__response_message_header_length + offset, block), __data_code, d.devicePoints, wordDataBlock[index++].Span);
                            offset += block;
                        }
                    index = 0;
                    if (bitDeviceBlock != null)
                        foreach (var d in bitDeviceBlock)
                        {
                            block = DeviceAccess.DEIVICE_REGISTER_DATA_ARRAY_LENGTH(__data_code, subcommand, d.devicePoints);
                            if (responseDataLength - offset < block)
                                throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
                            DeviceAccess.READ_DEVICE_IN_WORD_UNIT(__receive_byte_array.AsSpan(__response_message_header_length + offset, block), __data_code, d.devicePoints, bitDataBlock[index++].Span);
                            offset += block;
                        }
                }
            }
        }

        public void ReadLocalDeviceInWord(ushort monitoringTimer, IEnumerable<(string deviceCode, uint headDevice, ushort devicePoints)> wordDeviceBlock, IEnumerable<(string deviceCode, uint headDevice, ushort devicePoints)> bitDeviceBlock,
            out ushort endCode, Memory<ushort>[] wordDataBlock, Memory<ushort>[] bitDataBlock)
        {
            IEnumerable<(string, string, string, string, string, uint, ushort)> word = null;
            IEnumerable<(string, string, string, string, string, uint, ushort)> bit = null;
            if (wordDeviceBlock != null)
                word = wordDeviceBlock.Select(d => new ValueTuple<string, string, string, string, string, uint, ushort>(null, null, null, null, d.deviceCode, d.headDevice, d.devicePoints));
            if (bitDeviceBlock != null)
                bit = bitDeviceBlock.Select(d => new ValueTuple<string, string, string, string, string, uint, ushort>(null, null, null, null, d.deviceCode, d.headDevice, d.devicePoints));

            __read_device_block(ref __destination,
                            __subcommand,
                            monitoringTimer,
                            word, bit,
                            out endCode, wordDataBlock, bitDataBlock);
        }

        public async Task<ushort> ReadLocalDeviceInWordAsync(ushort monitoringTimer, IEnumerable<(string deviceCode, uint headDevice, ushort devicePoints)> wordDeviceBlock, IEnumerable<(string deviceCode, uint headDevice, ushort devicePoints)> bitDeviceBlock,
            Memory<ushort>[] wordDataBlock, Memory<ushort>[] bitDataBlock)
        {
            IEnumerable<(string, string, string, string, string, uint, ushort)> word = null;
            IEnumerable<(string, string, string, string, string, uint, ushort)> bit = null;
            if (wordDeviceBlock != null)
                word = wordDeviceBlock.Select(d => new ValueTuple<string, string, string, string, string, uint, ushort>(null, null, null, null, d.deviceCode, d.headDevice, d.devicePoints));
            if (bitDeviceBlock != null)
                bit = bitDeviceBlock.Select(d => new ValueTuple<string, string, string, string, string, uint, ushort>(null, null, null, null, d.deviceCode, d.headDevice, d.devicePoints));

            ushort end = 0;
            await Task.Run(() => __read_device_block(ref __destination,
                            __subcommand,
                            monitoringTimer,
                            word, bit,
                            out end, wordDataBlock, bitDataBlock));
            return end;
        }

        public void ReadModuleAccessDeviceInWord(ushort monitoringTimer, IEnumerable<(string extensionSpecification, uint headDevice, ushort devicePoints)> wordDeviceBlock,
            out ushort endCode, Memory<ushort>[] wordDataBlock)
        {
            __read_device_block(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                            monitoringTimer,
                            wordDeviceBlock.Select(d => new ValueTuple<string, string, string, string, string, uint, ushort>(d.extensionSpecification, null, null, null, "G", d.headDevice, d.devicePoints)), null,
                            out endCode, wordDataBlock, null);
        }

        public async Task<ushort> ReadModuleAccessDeviceInWordAsync(ushort monitoringTimer, IEnumerable<(string extensionSpecification, uint headDevice, ushort devicePoints)> wordDeviceBlock,
            Memory<ushort>[] wordDataBlock)
        {
            ushort end = 0;
            await Task.Run(() => __read_device_block(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                            monitoringTimer,
                            wordDeviceBlock.Select(d => new ValueTuple<string, string, string, string, string, uint, ushort>(d.extensionSpecification, null, null, null, "G", d.headDevice, d.devicePoints)), null,
                            out end, wordDataBlock, null));
            return end;
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
                serialNo0 = __serial_number_generator == null ? (ushort)0 : (ushort)__serial_number_generator.Next(65536);

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

        public async Task<ushort> WriteLocalDeviceInWordAsync(ushort monitoringTimer, string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data)
        {
            ushort end = 0;
            await Task.Run(() => __write_device(ref __destination,
                            __subcommand,
                            null, null, null, null,
                            monitoringTimer, deviceCode, headDevice, devicePoints, out end, data.Span, null));
            return end;
        }

        public void WriteLocalDeviceInBit(ushort monitoringTimer, string deviceCode, uint headDevice, ushort devicePoints, out ushort endCode, ReadOnlySpan<byte> data)
        {
            __write_device(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT,
                            null, null, null, null,
                            monitoringTimer, deviceCode, headDevice, devicePoints, out endCode, null, data);
        }

        public async Task<ushort> WriteLocalDeviceInBitAsync(ushort monitoringTimer, string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<byte> data)
        {
            ushort end = 0;
            await Task.Run(() => __write_device(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT,
                            null, null, null, null,
                            monitoringTimer, deviceCode, headDevice, devicePoints, out end, null, data.Span));
            return end;
        }

        public void WriteModuleAccessDeviceInWord(ushort monitoringTimer, string extensionSepcification, uint headDevice, ushort devicePoints, out ushort endCode, ReadOnlySpan<ushort> data)
        {
            __write_device(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                            extensionSepcification, null, null, null,
                            monitoringTimer, "G", headDevice, devicePoints, out endCode, data, null);
        }

        public async Task<ushort> WriteModuleAccessDeviceInWordAsync(ushort monitoringTimer, string extensionSepcification, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data)
        {
            ushort end = 0;
            await Task.Run(() => __write_device(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                            extensionSepcification, null, null, null,
                            monitoringTimer, "G", headDevice, devicePoints, out end, data.Span, null));
            return end;
        }

        private void __write_device_random(ref DESTINATION_ADDRESS_T destination, SUB_COMMANDS_T subcommand,
            ushort monitoringTimer,
            IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice, ushort value)> devicein16,
            IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice, uint value)> devicein32,
            out ushort endCode)
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

                ushort wordPoints = (ushort)(devicein16 == null ? 0 : devicein16.Count());
                ushort dwordPoints = (ushort)(devicein32 == null ? 0 : devicein32.Count());

                int offset = 0;
                serialNo0 = __serial_number_generator == null ? (ushort)0 : (ushort)__serial_number_generator.Next(65536);

                if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) == 0)
                {
                    offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                        destination.network_number, destination.station_number,
                                        destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                        (ushort)(__command_header_length + (__device_specification_length + __word_register_length) * wordPoints + 
                                        (__device_specification_length + __word_register_length * 2) * dwordPoints + __device_points_length), monitoringTimer,
                                        __send_byte_array, 0);

                }
                else
                {
                    offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                        destination.network_number, destination.station_number,
                                        destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                        (ushort)(__command_header_length + (__device_extension_specification_length + __word_register_length) * wordPoints + 
                                        (__device_extension_specification_length + __word_register_length * 2) * dwordPoints + __device_points_length), monitoringTimer,
                                        __send_byte_array, 0);
                }

                offset += RequestCommand.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, COMMANDS_T.DEVICE_WRITE_RANDOM, subcommand,
                                        __send_byte_array, offset);

                offset += DeviceAccess.BUILD_DEVICE_WRITE_RANDOM_BYTE_ARRAY_HEADER(__frame_type, __data_code, subcommand,
                                        devicein16, devicein32,
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
                    if (responseDataLength != 0)
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
                }
            }
        }

        private void __write_device_random(ref DESTINATION_ADDRESS_T destination, SUB_COMMANDS_T subcommand,
            ushort monitoringTimer,
            IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice, byte value)> bitdevice,
            out ushort endCode)
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

                ushort bitPoints = (ushort)(bitdevice == null ? 0 : bitdevice.Count());

                int offset = 0;
                serialNo0 = __serial_number_generator == null ? (ushort)0 : (ushort)__serial_number_generator.Next(65536);

                if ((subcommand & SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION) == 0)
                {
                    offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                        destination.network_number, destination.station_number,
                                        destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                        (ushort)(__command_header_length + (__device_specification_length + __bit_register_length) * bitPoints + __device_points_length / 2), monitoringTimer,
                                        __send_byte_array, 0);

                }
                else
                {
                    offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                        destination.network_number, destination.station_number,
                                        destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                        (ushort)(__command_header_length + (__device_extension_specification_length + __bit_register_length) * bitPoints + __device_points_length / 2), monitoringTimer,
                                        __send_byte_array, 0);
                }

                offset += RequestCommand.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, COMMANDS_T.DEVICE_WRITE_RANDOM, subcommand,
                                        __send_byte_array, offset);

                offset += DeviceAccess.BUILD_DEVICE_WRITE_RANDOM_BYTE_ARRAY_HEADER(__frame_type, __data_code, subcommand,
                                        bitdevice,
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
                    if (responseDataLength != 0)
                        throw new SLMPException(SLMP_EXCEPTION_CODE_T.DEVICE_REGISTER_DATA_CORRUPTED);
                }
            }
        }

        public void WriteLocalDeviceInWord(ushort monitoringTimer, IEnumerable<(string deviceCode, uint headDevice, ushort value)> wordDevice, IEnumerable<(string deviceCode, uint headDevice, uint value)> dwordDevice,
            out ushort endCode)
        {
            IEnumerable<(string, string, string, string, string, uint, ushort)> word = null;
            IEnumerable<(string, string, string, string, string, uint, uint)> dword = null;

            if (wordDevice != null)
                word = wordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint, ushort>(null, null, null, null, d.deviceCode, d.headDevice, d.value));
            if (dwordDevice != null)
                dword = dwordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint, uint>(null, null, null, null, d.deviceCode, d.headDevice, d.value));

            __write_device_random(ref __destination,
                __subcommand,
                monitoringTimer,
                word, dword,
                out endCode);
        }

        public async Task<ushort> WriteLocalDeviceInWordAsync(ushort monitoringTimer, IEnumerable<(string deviceCode, uint headDevice, ushort value)> wordDevice, IEnumerable<(string deviceCode, uint headDevice, uint value)> dwordDevice)
        {
            IEnumerable<(string, string, string, string, string, uint, ushort)> word = null;
            IEnumerable<(string, string, string, string, string, uint, uint)> dword = null;

            if (wordDevice != null)
                word = wordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint, ushort>(null, null, null, null, d.deviceCode, d.headDevice, d.value));
            if (dwordDevice != null)
                dword = dwordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint, uint>(null, null, null, null, d.deviceCode, d.headDevice, d.value));

            ushort end = 0;
            await Task.Run(() => __write_device_random(ref __destination,
                __subcommand,
                monitoringTimer,
                word, dword,
                out end));
            return end;
        }

        public void WriteLocalDeviceInBit(ushort monitoringTimer, IEnumerable<(string deviceCode, uint headDevice, byte value)> bitDevice,
            out ushort endCode)
        {
            __write_device_random(ref __destination,
                __subcommand | SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT,
                monitoringTimer,
                bitDevice.Select(d => new ValueTuple<string, string, string, string, string, uint, byte>(null, null, null, null, d.deviceCode, d.headDevice, d.value)),
                out endCode);
        }

        public async Task<ushort> WriteLocalDeviceInBitAsync(ushort monitoringTimer, IEnumerable<(string deviceCode, uint headDevice, byte value)> bitDevice)
        {
            ushort end = 0;
            await Task.Run(() => __write_device_random(ref __destination,
                __subcommand | SUB_COMMANDS_T.DEVICE_COMMAND_ACCESS_IN_BIT_UNIT,
                monitoringTimer,
                bitDevice.Select(d => new ValueTuple<string, string, string, string, string, uint, byte>(null, null, null, null, d.deviceCode, d.headDevice, d.value)),
                out end));
            return end;
        }

        public void WriteModuleAccessDeviceInWord(ushort monitoringTimer, IEnumerable<(string extensionSpecification, uint headDevice, ushort value)> wordDevice, IEnumerable<(string extensionSpecification, uint headDevice, uint value)> dwordDevice,
            out ushort endCode)
        {
            IEnumerable<(string, string, string, string, string, uint, ushort)> word = null;
            IEnumerable<(string, string, string, string, string, uint, uint)> dword = null;
            if (wordDevice != null)
                word = wordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint, ushort>(d.extensionSpecification, null, null, null, "G", d.headDevice, d.value));
            if (dwordDevice != null)
                dword = dwordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint, uint>(d.extensionSpecification, null, null, null, "G", d.headDevice, d.value));

            __write_device_random(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                            monitoringTimer,
                            word, dword,
                            out endCode);
        }

        public async Task<ushort> WriteModuleAccessDeviceInWordAsync(ushort monitoringTimer, IEnumerable<(string extensionSpecification, uint headDevice, ushort value)> wordDevice, IEnumerable<(string extensionSpecification, uint headDevice, uint value)> dwordDevice)
        {
            IEnumerable<(string, string, string, string, string, uint, ushort)> word = null;
            IEnumerable<(string, string, string, string, string, uint, uint)> dword = null;
            if (wordDevice != null)
                word = wordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint, ushort>(d.extensionSpecification, null, null, null, "G", d.headDevice, d.value));
            if (dwordDevice != null)
                dword = dwordDevice.Select(d => new ValueTuple<string, string, string, string, string, uint, uint>(d.extensionSpecification, null, null, null, "G", d.headDevice, d.value));

            ushort end = 0;
            await Task.Run(() => __write_device_random(ref __destination,
                            __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                            monitoringTimer,
                            word, dword,
                            out end));
            return end;
        }

        private void __write_device_block(ref DESTINATION_ADDRESS_T destination, SUB_COMMANDS_T subcommand,
            ushort monitoringTimer,
            IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data)> wordDeviceBlock,
            IEnumerable<(string extension, string extensionModification, string deviceModification, string indirectSpecification, string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data)> bitDeviceBlock,
            out ushort endCode)
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

                byte worddeviceblocks = (byte)(wordDeviceBlock == null ? 0 : wordDeviceBlock.Count());
                byte bitdeviceblocks = (byte)(bitDeviceBlock == null ? 0 : bitDeviceBlock.Count());
                int bodylength = 0;

                if(worddeviceblocks != 0)
                    foreach(var d in wordDeviceBlock)
                    {
                        bodylength += DeviceAccess.DEVICE_SPECIFICATION_LENGTH(__data_code, subcommand) + __device_points_length;
                        bodylength += DeviceAccess.DEIVICE_REGISTER_DATA_ARRAY_LENGTH(__data_code, subcommand, d.devicePoints);
                    }
                if (bitdeviceblocks != 0)
                    foreach (var d in bitDeviceBlock)
                    {
                        bodylength += DeviceAccess.DEVICE_SPECIFICATION_LENGTH(__data_code, subcommand) + __device_points_length;
                        bodylength += DeviceAccess.DEIVICE_REGISTER_DATA_ARRAY_LENGTH(__data_code, subcommand, d.devicePoints);
                    }

                int offset = 0;
                serialNo0 = __serial_number_generator == null ? (ushort)0 : (ushort)__serial_number_generator.Next(65536);

                offset = RequestMessage.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, serialNo0,
                                    destination.network_number, destination.station_number,
                                    destination.module_io, destination.multidrop_number, destination.extension_station_number,
                                    (ushort)(__command_header_length + bodylength + __device_points_length), monitoringTimer,
                                    __send_byte_array, 0);


                offset += RequestCommand.BUILD_BYTE_ARRAY_HEADER(__frame_type, __data_code, COMMANDS_T.DEVICE_WRITE_BLOCK, subcommand,
                                    __send_byte_array, offset);

                offset += DeviceAccess.BUILD_DEVICE_WRITE_BLOCK_BYTE_ARRAY_HEADER(__data_code, worddeviceblocks, bitdeviceblocks, __send_byte_array, offset);

                if (worddeviceblocks != 0)
                    foreach (var d in wordDeviceBlock)
                    {
                        offset += DeviceAccess.BUILD_DEVICE_READ_WRITE_BYTE_ARRAY_HEADER(__frame_type, __data_code, subcommand,
                            d.deviceCode, d.headDevice, d.devicePoints, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, __send_byte_array, offset);
                        DeviceAccess.WRITE_DEVICE_IN_WORD_UNIT(d.data.Span, __data_code, d.devicePoints, __send_byte_array.AsSpan(offset));
                        offset += DeviceAccess.DEIVICE_REGISTER_DATA_ARRAY_LENGTH(__data_code, subcommand, d.devicePoints);
                    }
                if (bitdeviceblocks != 0)
                    foreach (var d in bitDeviceBlock)
                    {
                        offset += DeviceAccess.BUILD_DEVICE_READ_WRITE_BYTE_ARRAY_HEADER(__frame_type, __data_code, subcommand,
                            d.deviceCode, d.headDevice, d.devicePoints, d.extension, d.extensionModification, d.deviceModification, d.indirectSpecification, __send_byte_array, offset);
                        DeviceAccess.WRITE_DEVICE_IN_WORD_UNIT(d.data.Span, __data_code, d.devicePoints, __send_byte_array.AsSpan(offset));
                        offset += DeviceAccess.DEIVICE_REGISTER_DATA_ARRAY_LENGTH(__data_code, subcommand, d.devicePoints);
                    }

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

        public void WriteLocalDeviceInWord(ushort monitoringTimer, IEnumerable<(string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data)> wordDeviceBlock, 
            IEnumerable<(string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data)> bitDeviceBlock,
            out ushort endCode)
        {
            IEnumerable<(string, string, string, string, string, uint, ushort, ReadOnlyMemory<ushort>)> word = null;
            IEnumerable<(string, string, string, string, string, uint, ushort, ReadOnlyMemory<ushort>)> bit = null;

            if (wordDeviceBlock != null)
                word = wordDeviceBlock.Select<(string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data), (string, string, string, string, string, uint, ushort, ReadOnlyMemory<ushort>)>(d => new (null, null, null, null, d.deviceCode, d.headDevice, d.devicePoints, new ValueTuple<ReadOnlyMemory<ushort>>(d.data)));
            if (bitDeviceBlock != null)
                bit = bitDeviceBlock.Select<(string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data), (string, string, string, string, string, uint, ushort, ReadOnlyMemory<ushort>)>(d => new(null, null, null, null, d.deviceCode, d.headDevice, d.devicePoints, new ValueTuple<ReadOnlyMemory<ushort>>(d.data)));

            __write_device_block(ref __destination,
                __subcommand,
                monitoringTimer,
                word, bit,
                out endCode);
        }

        public async Task<ushort> WriteLocalDeviceInWordAsync(ushort monitoringTimer, IEnumerable<(string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data)> wordDeviceBlock,
            IEnumerable<(string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data)> bitDeviceBlock)
        {
            IEnumerable<(string, string, string, string, string, uint, ushort, ReadOnlyMemory<ushort>)> word = null;
            IEnumerable<(string, string, string, string, string, uint, ushort, ReadOnlyMemory<ushort>)> bit = null;

            if (wordDeviceBlock != null)
                word = wordDeviceBlock.Select<(string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data), (string, string, string, string, string, uint, ushort, ReadOnlyMemory<ushort>)>(d => new(null, null, null, null, d.deviceCode, d.headDevice, d.devicePoints, new ValueTuple<ReadOnlyMemory<ushort>>(d.data)));
            if (bitDeviceBlock != null)
                bit = bitDeviceBlock.Select<(string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data), (string, string, string, string, string, uint, ushort, ReadOnlyMemory<ushort>)>(d => new(null, null, null, null, d.deviceCode, d.headDevice, d.devicePoints, new ValueTuple<ReadOnlyMemory<ushort>>(d.data)));

            ushort end = 0;
            await Task.Run(() => __write_device_block(ref __destination,
                __subcommand,
                monitoringTimer,
                word, bit,
                out end));
            return end;
        }

        public void WriteModuleAccessDeviceInWord(ushort monitoringTimer, IEnumerable<(string extensionSpecification, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data)> wordDeviceBlock, out ushort endCode)
        {
            __write_device_block(ref __destination,
                __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                monitoringTimer,
                wordDeviceBlock.Select<(string extensionSpecification, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data), (string, string, string, string, string, uint, ushort, ReadOnlyMemory<ushort>)>(d => new(d.extensionSpecification, null, null, null, "G", d.headDevice, d.devicePoints, new ValueTuple<ReadOnlyMemory<ushort>>(d.data))),
                null,
                out endCode);
        }

        public async Task<ushort> WriteModuleAccessDeviceInWordAsync(ushort monitoringTimer, IEnumerable<(string extensionSpecification, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data)> wordDeviceBlock)
        {
            ushort end = 0;
            await Task.Run(() => __write_device_block(ref __destination,
                __subcommand | SUB_COMMANDS_T.DEVICE_EXTENSION_SPECIFICATION,
                monitoringTimer,
                wordDeviceBlock.Select<(string extensionSpecification, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data), (string, string, string, string, string, uint, ushort, ReadOnlyMemory<ushort>)>(d => new(d.extensionSpecification, null, null, null, "G", d.headDevice, d.devicePoints, new ValueTuple<ReadOnlyMemory<ushort>>(d.data))),
                null,
                out end));
            return end;
        }
    }
}
