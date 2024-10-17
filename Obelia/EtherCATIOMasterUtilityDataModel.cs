using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP;
using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Master;
using AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Control;
using AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Obelia.SlavePDOs.Generic;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Xml.Linq;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Obelia
{
    public class EtherCATIOMasterUtilityDataModel : DataModel
    {
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Factory()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public override void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public override void ExchangeDataWithDevice(DeviceAccessMaster master, ushort monitoringTimer)
        {
            __device_read_data();
            ushort end = 0;

            var res = __process_out_data_device.control_out;
            switch (res)
            {
                case ENABLE_COMMAND_T enable:
                    __device_sync_address = Convert.ToUInt32(enable.device_address[1..], 16) * 16;
                    __device_sync_specification = enable.device_address;
                    __device_sync_model = enable.model;
                    if (enable.model == DEVICE_MODEL_TYPE_T.RJ72EC92)
                    {
                        __process_in_address_table = new List<(string, uint, ushort)>()
                        {
                            (enable.device_address, (uint)RJ71EC92_ADDRESS_TABLE.BUF_NUM_OF_SLAVES,
                            (ushort)(Marshal.SizeOf<ECAT_PROCESS_IN_DATA_T.PROCESS_IN_T.MASTER_DIAGNOSTIC_INFO_T>()/2)),
                            (enable.device_address, (uint)RJ71EC92_ADDRESS_TABLE.BUF_MASTER_ESM_STATE,1),
                        };
                        __process_in_data = new Memory<ushort>[2]
                        {
                                new ushort[Marshal.SizeOf<ECAT_PROCESS_IN_DATA_T.PROCESS_IN_T.MASTER_DIAGNOSTIC_INFO_T>()/2],
                                new ushort[1]
                        };

                        __process_xy_address_table = new List<(string, uint, ushort)>()
                        {
                            ("X", __device_sync_address, 2),
                            ("Y", __device_sync_address, 2),
                        };
                        __process_xy_data = new Memory<ushort>[2]
                        {
                            new ushort[2],
                            new ushort[2],
                        };
                    }
                    __process_in_data_device.control_in = new ENABLE_RESULT_T() { device_address = __device_sync_specification, model = __device_sync_model };
                    __process_out_data_device.control_out = null;
                    __device_sync_control = true;
                    __slave_pdo_sync_mode = SLAVE_PDO_SYNC_MODE_T.Disabled;
                    break;
                case ENABLE_WITH_ENI_COMMAND_T enable:
                    __slave_pdo_data = enable.slaves;

                    __device_sync_address = Convert.ToUInt32(enable.device_address[1..], 16) * 16;
                    __device_sync_specification = enable.device_address;
                    __device_sync_model = enable.model;
                    if (enable.model == DEVICE_MODEL_TYPE_T.RJ72EC92)
                    {
                        __process_in_address_table = new List<(string, uint, ushort)>()
                        {
                            (enable.device_address, (uint)RJ71EC92_ADDRESS_TABLE.BUF_NUM_OF_SLAVES, (ushort)(Marshal.SizeOf<ECAT_PROCESS_IN_DATA_T.PROCESS_IN_T.MASTER_DIAGNOSTIC_INFO_T>()/2)),
                            (enable.device_address, (uint)RJ71EC92_ADDRESS_TABLE.BUF_MASTER_ESM_STATE,1),
                            (enable.device_address, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SLAVE_ESM_STATE, (ushort)enable.slaves.NumberOfSlaves),
                            (enable.device_address, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SLAVE_ERROR_STATUS, (ushort)enable.slaves.NumberOfSlaves),
                            (enable.device_address, (uint)RJ71EC92_ADDRESS_TABLE.BUF_INPUT_DATA_READ_REQUEST, 1),
                            (enable.device_address, (uint)RJ71EC92_ADDRESS_TABLE.BUF_INPUT_DATA_READ_RESPONSE, 1),
                            (enable.device_address, (uint)RJ71EC92_ADDRESS_TABLE.BUF_OUTPUT_DATA_READ_REQUEST, 1),
                            (enable.device_address, (uint)RJ71EC92_ADDRESS_TABLE.BUF_OUTPUT_DATA_READ_RESPONSE, 1),
                        };
                        __process_in_data = new Memory<ushort>[8]
                        {
                            new ushort[Marshal.SizeOf<ECAT_PROCESS_IN_DATA_T.PROCESS_IN_T.MASTER_DIAGNOSTIC_INFO_T>()/2],
                            new ushort[1],
                            new ushort[enable.slaves.NumberOfSlaves],
                            new ushort[enable.slaves.NumberOfSlaves],
                            new ushort[1],
                            new ushort[1],
                            new ushort[1],
                            new ushort[1],
                        };

                        __process_xy_address_table = new List<(string, uint, ushort)>()
                        {
                            ("X", __device_sync_address, 2),
                            ("Y", __device_sync_address, 2),
                        };
                        __process_xy_data = new Memory<ushort>[2]
                        {
                            new ushort[2],
                            new ushort[2],
                        };
                    }

                    __slave_tx_pdo_segments = new List<(uint dev_start, ushort dev_points, int host_start, int host_points)>();
                    __slave_rx_pdo_segments = new List<(uint dev_start, ushort dev_points, int host_start, int host_points)>();
                    uint start = 0;
                    uint lastpos = 0;
                    uint lastsize = 0;
                    ushort points = 0;
                    foreach (var v in enable.slaves.Slaves.SelectMany(s => s.TxVariables))
                    {
                        if (v.GlobalBitOffset >= ((ushort)RJ71EC92.BATCH_RW_POINTS + start) * 16 || v.GlobalBitOffset + v.BitSize > ((ushort)RJ71EC92.BATCH_RW_POINTS + start) * 16)
                        {
                            points = (ushort)((lastpos + lastsize) / 16 + ((lastpos + lastsize) % 16 == 0 ? 0 : 1) - start);
                            __slave_tx_pdo_segments.Add(((uint)RJ71EC92_ADDRESS_TABLE.BUF_INPUT_DATA_PTR + start, points, (int)start, points));
                            start = (ushort)(v.GlobalBitOffset / 16);
                        }
                        lastpos = v.GlobalBitOffset;
                        lastsize = v.BitSize;
                    }
                    points = (ushort)((lastpos + lastsize) / 16 + ((lastpos + lastsize) % 16 == 0 ? 0 : 1) - start);
                    __slave_tx_pdo_segments.Add(((uint)RJ71EC92_ADDRESS_TABLE.BUF_INPUT_DATA_PTR + start, points, (int)start, points));

                    start = 0;
                    lastpos = 0;
                    lastsize = 0;
                    points = 0;
                    foreach (var v in enable.slaves.Slaves.SelectMany(s => s.RxVariables))
                    {
                        if (v.GlobalBitOffset >= ((ushort)RJ71EC92.BATCH_RW_POINTS + start) * 16 || v.GlobalBitOffset + v.BitSize > ((ushort)RJ71EC92.BATCH_RW_POINTS + start) * 16)
                        {
                            points = (ushort)((lastpos + lastsize) / 16 + ((lastpos + lastsize) % 16 == 0 ? 0 : 1) - start);
                            __slave_rx_pdo_segments.Add(((uint)RJ71EC92_ADDRESS_TABLE.BUF_OUTPUT_DATA_PTR + start, points, (int)start, points));
                            start = (ushort)(v.GlobalBitOffset / 16);
                        }
                        lastpos = v.GlobalBitOffset;
                        lastsize = v.BitSize;
                    }
                    points = (ushort)((lastpos + lastsize) / 16 + ((lastpos + lastsize) % 16 == 0 ? 0 : 1) - start);
                    __slave_rx_pdo_segments.Add(((uint)RJ71EC92_ADDRESS_TABLE.BUF_OUTPUT_DATA_PTR + start, points, (int)start, points));



                    __process_in_data_device.process_in.slave_pdo = new ECAT_PROCESS_IN_DATA_T.SLAVE_PDO_T(enable.slaves.NumberOfSlaves, enable.slaves.TxSizeInWord, enable.slaves.RxSizeInWord);
                    __process_in_data_device.control_in = new ENABLE_WITH_ENI_RESULT_T() { device_address = __device_sync_specification, model = __device_sync_model, slaves = enable.slaves };
                    __process_out_data_device.control_out = null;
                    __device_sync_control = true;
                    __slave_pdo_sync_mode = SLAVE_PDO_SYNC_MODE_T.Monitor;
                    break;
                case DISABLE_COMMAND_T:
                    __process_in_data_device.control_in = new DISABLE_RESULT_T() { end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR };
                    __process_out_data_device.control_out = null;
                    __device_sync_control = false;
                    break;
                case EXECUTE_Y_COMMAND_T xy:
                    try
                    {
                        master.WriteLocalDeviceInBit(monitoringTimer, "Y", __device_sync_address + (ushort)xy.request_command, 1, out end, new byte[] { (byte)(xy.request_value ? 1 : 0) });
                        __process_in_data_device.control_in = new EXECUTE_Y_RESULT_T() { end_code = end, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, request_command = xy.request_command, request_value = xy.request_value };
                        __process_out_data_device.control_out = null;
                    }
                    catch (SLMPException ex)
                    {
                        __process_in_data_device.control_in = new EXECUTE_Y_RESULT_T() { end_code = end, exception_code = ex.ExceptionCode, request_command = xy.request_command, request_value = xy.request_value };
                        __process_out_data_device.control_out = null;

                        if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                            throw;
                    }
                    break;
                case SWITCH_INTERACTIVE_COMMAND_T sw:
                    __process_out_data_device.control_out = null;
                    if (sw.enabled)
                    {
                        ushort[] rx = new ushort[__process_in_data_device.process_in.slave_pdo.Value.rx_readback_pdo.Length];
                        try
                        {
                            foreach (var s in __slave_rx_pdo_segments)
                            {
                                master.ReadModuleAccessDeviceInWord(monitoringTimer,
                                    __device_sync_specification, s.dev_start, s.dev_points, out end,
                                    rx.AsSpan((int)s.host_start));
                                if (end != 0)
                                    break;
                            }
                            if (end == 0)
                            {
                                __slave_pdo_sync_mode = SLAVE_PDO_SYNC_MODE_T.Interactive;
                                __sync_process_out_data_device_with_user_specified(rx);
                                __process_in_data_device.control_in = new SWITCH_INTERACTIVE_RESULT_T() { end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, enabled = sw.enabled, rx_pdo = rx };
                            }
                            else
                                __process_in_data_device.control_in = new SWITCH_INTERACTIVE_RESULT_T() { end_code = end, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, enabled = sw.enabled };
                            __process_out_data_device.control_out = null;
                        }
                        catch (SLMPException ex)
                        {
                            __process_in_data_device.control_in = new SWITCH_INTERACTIVE_RESULT_T() { end_code = end, exception_code = ex.ExceptionCode, enabled = sw.enabled };
                            __process_out_data_device.control_out = null;

                            if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                                throw;
                        }
                    }
                    else
                    {
                        __slave_pdo_sync_mode = SLAVE_PDO_SYNC_MODE_T.Monitor;
                        __process_in_data_device.control_in = new SWITCH_INTERACTIVE_RESULT_T() { end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, enabled = sw.enabled };
                        __process_out_data_device.control_out = null;
                    }
                    break;
                case RELOAD_MASTER_EVENT_HISTORY_COMMAND_T:
                    try
                    {
                        ushort[] ev = new ushort[RJ71EC92.EVENTS_HISTORY_CAPACITY*Marshal.SizeOf<MASTER_EVENT_T>()/2 + 2]; //102
                        master.ReadModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_EVENT_HEAD, 502, out end,  ev.AsSpan(0));
                        if(end == 0)
                            master.ReadModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_EVENT_INFO + 500, 500, out end, ev.AsSpan(502));

                        if(end == 0)
                            __process_in_data_device.control_in = new RELOAD_MASTER_EVENT_HISTORY_RESULT_T() { end_code = 0, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR, events = ev };
                        else
                            __process_in_data_device.control_in = new RELOAD_MASTER_EVENT_HISTORY_RESULT_T() { end_code = end, exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR};
                        __process_out_data_device.control_out = null;
                    }
                    catch (SLMPException ex)
                    {
                        __process_in_data_device.control_in = new RELOAD_MASTER_EVENT_HISTORY_RESULT_T() { end_code = end, exception_code = ex.ExceptionCode};
                        __process_out_data_device.control_out = null;

                        if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                            throw;
                    }
                    break;

                case EXECUTE_SDO_COMMAND_T sdo:
                    try
                    {
                        switch (sdo.step)
                        {
                            case 0:
                                __parameter_address_table = new List<(string, uint, ushort)>()
                                {
                                    (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_CONTROL_COMMAND, 1),
                                    (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_CONTROL_RESPONSE, 1),
                                };
                                __parameter_values = new Memory<ushort>[]
                                {
                                    new ushort[1],
                                    new ushort[1]
                                };
                                master.WriteModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_CONTROL_COMMAND, 1, out end, new ushort[] { (ushort)SDO_COMMAND_T.NONE });
                                sdo.step = 1;
                                break;
                            case 1:
                                master.ReadModuleAccessDeviceInWord(monitoringTimer, __parameter_address_table, out end, __parameter_values);
                                if(end == 0)
                                {
                                    if (__parameter_values[0].Span[0] == __parameter_values[1].Span[0])
                                    {
                                        var sdo_header = new ushort[Marshal.SizeOf<SDO_PARAMETER_T>() / 2];
                                        var header = sdo.header;
                                        MemoryMarshal.Write<SDO_PARAMETER_T>(MemoryMarshal.Cast<ushort, byte>(sdo_header), ref header);
                                        if (sdo.header.data_size_in_byte != 0)
                                        {
                                            __parameters = new List<(string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data)>
                                            {
                                                (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_TRANSMIT_SLAVE_ADDRESS, (ushort)sdo_header.Length, sdo_header),
                                                (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_TRANSMIT_DATA_PTR, (ushort)sdo.data.Length, sdo.data),
                                                (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_CONTROL_COMMAND, 1, new ushort[]{ (ushort)sdo.sdo_command}),
                                            };
                                        }
                                        else
                                        {
                                            __parameters = new List<(string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data)>
                                            {
                                                (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_TRANSMIT_SLAVE_ADDRESS, (ushort)sdo_header.Length, sdo_header),
                                                (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_CONTROL_COMMAND, 1, new ushort[]{ (ushort)sdo.sdo_command}),
                                            };
                                        }
                                        master.WriteModuleAccessDeviceInWord(monitoringTimer, __parameters, out end);
                                        __parameter_address_table = new List<(string, uint, ushort)>()
                                        {
                                            (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_CONTROL_COMMAND, 1),
                                            (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_CONTROL_RESPONSE, 1),
                                        };
                                        __parameter_values = new Memory<ushort>[]
                                        {
                                            new ushort[1],
                                            new ushort[1]
                                        };
                                        sdo.step = 2;
                                    }
                                }
                                break;
                            case 2:
                                master.ReadModuleAccessDeviceInWord(monitoringTimer, __parameter_address_table, out end, __parameter_values);
                                if (end == 0)
                                {
                                    if (__parameter_values[0].Span[0] == __parameter_values[1].Span[0])
                                    {
                                        __parameter_address_table = new List<(string, uint, ushort)>()
                                        {
                                            (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_RECEIVED_SLAVE_ADDRESS, (ushort)(Marshal.SizeOf<SDO_PARAMETER_T>() / 2)),
                                            (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_RECEIVED_DATA_PTR, RJ71EC92.SDO_DATA_SIZE_IN_WORD),
                                            (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_CONTROL_ERROR, 1)
                                        };
                                        __parameter_values = new Memory<ushort>[]
                                        {
                                            new ushort[Marshal.SizeOf<SDO_PARAMETER_T>() / 2],
                                            new ushort[RJ71EC92.SDO_DATA_SIZE_IN_WORD],
                                            new ushort[1]
                                        };
                                        master.ReadModuleAccessDeviceInWord(monitoringTimer, __parameter_address_table, out end, __parameter_values);
                                        if(end == 0)
                                        {
                                            var sdo_header = MemoryMarshal.Read<SDO_PARAMETER_T>(MemoryMarshal.Cast<ushort, byte>(__parameter_values[0].Span));
                                            if(sdo_header.data_size_in_byte > 0 && __parameter_values[2].Span[0] == 0)
                                            {
                                                var sdo_data = new ushort[sdo_header.data_size_in_byte / 2 + (sdo_header.data_size_in_byte % 2 == 0 ? 0 : 1)];
                                                __parameter_values[1].Slice(0, sdo_data.Length).CopyTo(sdo_data);
                                                __process_in_data_device.control_in = new EXECUTE_SDO_RESULT_T()
                                                {
                                                    end_code = 0,
                                                    exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                                    timeout = false,
                                                    sdo_command = sdo.sdo_command,
                                                    header = sdo_header,
                                                    error = __parameter_values[2].Span[0],
                                                    data = sdo_data
                                                };
                                            }
                                            else
                                                __process_in_data_device.control_in = new EXECUTE_SDO_RESULT_T()
                                                {
                                                    end_code = 0,
                                                    exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                                    timeout = false,
                                                    sdo_command = sdo.sdo_command,
                                                    header = sdo_header,
                                                    error = __parameter_values[2].Span[0],
                                                };
                                            __process_out_data_device.control_out = null;
                                        }
                                    }
                                }
                                break;
                        }
                        if (__process_out_data_device.control_out != null)
                        {
                            if (end != 0)
                            {
                                __process_in_data_device.control_in = new EXECUTE_SDO_RESULT_T()
                                {
                                    end_code = end,
                                    exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                    sdo_command = sdo.sdo_command,
                                    header = sdo.header,
                                    data = null
                                };
                                __process_out_data_device.control_out = null;
                            }
                            else if (Math.Abs(Environment.TickCount - sdo.start_ticks) > sdo.timeout)
                            {
                                __process_in_data_device.control_in = new EXECUTE_SDO_RESULT_T()
                                {
                                    end_code = 0,
                                    exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                    timeout = true,
                                    header = sdo.header,
                                    sdo_command = sdo.sdo_command,
                                    data = null
                                };
                                __process_out_data_device.control_out = null;
                            }
                        }
                    }
                    catch (SLMPException ex)
                    {
                        __process_in_data_device.control_in = new EXECUTE_SDO_RESULT_T()
                        {
                            end_code = 0,
                            exception_code = ex.ExceptionCode,
                            timeout = false,
                            header = sdo.header,
                            data = null
                        };
                        __process_out_data_device.control_out = null;

                        if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                            throw;
                    }
                    finally
                    {
                        if (__process_out_data_device.control_out == null && sdo.step != 0)
                        {
                            master.WriteModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SDO_CONTROL_COMMAND, 1, out _, new ushort[] { (ushort)SDO_COMMAND_T.NONE });
                            __parameter_address_table = null;
                            __parameter_values = null;
                            __parameters = null;
                        }
                    }
                    break;
                case REQUEST_MASTER_ESM_COMMAND_T setmaster:
                    try
                    {
                        switch(setmaster.step)
                        {
                            case 0:
                                __parameter_address_table = new List<(string, uint, ushort)>()
                                {
                                    (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_MASTER_ESM_REQUEST, 1),
                                    (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_MASTER_ESM_RESPONSE, 1),
                                    (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_MASTER_ESM_REQUEST_RET, 1),
                                };
                                __parameter_values = new Memory<ushort>[]
                                {
                                    new ushort[1],
                                    new ushort[1],
                                    new ushort[1]
                                };
                                master.WriteModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_MASTER_ESM_REQUEST, 1, out end, new ushort[] { 0 });
                                setmaster.step = 1;
                                break;
                            case 1:
                                master.ReadModuleAccessDeviceInWord(monitoringTimer, __parameter_address_table, out end, __parameter_values);
                                if (end == 0)
                                {
                                    if (__parameter_values[0].Span[0] == __parameter_values[1].Span[0])
                                    {
                                        master.WriteModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_MASTER_ESM_REQUEST, 1, out end, new ushort[] { (ushort)setmaster.esm });
                                        setmaster.step = 2;
                                    }
                                }
                                break;
                            case 2:
                                master.ReadModuleAccessDeviceInWord(monitoringTimer, __parameter_address_table, out end, __parameter_values);
                                if (end == 0)
                                {
                                    if (__parameter_values[0].Span[0] == __parameter_values[1].Span[0])
                                    {
                                        
                                        if (end == 0)
                                        {
                                            __process_in_data_device.control_in = new REQUEST_MASTER_ESM_RESULT_T()
                                            {
                                                end_code = 0,
                                                exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                                timeout = false,
                                                esm = setmaster.esm,
                                                error = __parameter_values[2].Span[0],
                                            };
                                            __process_out_data_device.control_out = null;
                                        }
                                    }
                                }
                                break;
                        }
                        if (__process_out_data_device.control_out != null)
                        {
                            if (end != 0)
                            {
                                __process_in_data_device.control_in = new REQUEST_MASTER_ESM_RESULT_T()
                                {
                                    end_code = end,
                                    exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                    esm = setmaster.esm
                                };
                                __process_out_data_device.control_out = null;
                            }
                            else if (Math.Abs(Environment.TickCount - setmaster.start_ticks) > setmaster.timeout)
                            {
                                __process_in_data_device.control_in = new REQUEST_MASTER_ESM_RESULT_T()
                                {
                                    end_code = 0,
                                    exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                    timeout = true,
                                    esm = setmaster.esm
                                };
                                __process_out_data_device.control_out = null;
                            }
                        }
                    }
                    catch (SLMPException ex)
                    {
                        __process_in_data_device.control_in = new REQUEST_MASTER_ESM_RESULT_T()
                        {
                            end_code = 0,
                            exception_code = ex.ExceptionCode,
                            timeout = false,
                            esm = setmaster.esm
                        };
                        __process_out_data_device.control_out = null;

                        if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                            throw;
                    }
                    finally
                    {
                        if (__process_out_data_device.control_out == null && setmaster.step != 0)
                        {
                            master.WriteModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_MASTER_ESM_REQUEST, 1, out _, new ushort[] { 0 });
                            __parameter_address_table = null;
                            __parameter_values = null;
                            __parameters = null;
                        }
                    }
                    break;
                case REQUEST_SLAVE_ESM_COMMAND_T setslave:
                    try
                    {
                        switch (setslave.step)
                        {
                            case 0:
                                __parameter_address_table = new List<(string, uint, ushort)>()
                                {
                                    (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SLAVE_ESM_REQUEST + setslave.index, 1),
                                    (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SLAVE_ESM_RESPONSE + setslave.index, 1),
                                    (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SLAVE_ESM_REQUEST_RET + setslave.index, 1),
                                };
                                __parameter_values = new Memory<ushort>[]
                                {
                                    new ushort[1],
                                    new ushort[1],
                                    new ushort[1]
                                };
                                master.WriteModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SLAVE_ESM_REQUEST + setslave.index, 1, out end, new ushort[] { 0 });
                                setslave.step = 1;
                                break;
                            case 1:
                                master.ReadModuleAccessDeviceInWord(monitoringTimer, __parameter_address_table, out end, __parameter_values);
                                if (end == 0)
                                {
                                    if (__parameter_values[0].Span[0] == __parameter_values[1].Span[0])
                                    {
                                        master.WriteModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SLAVE_ESM_REQUEST + setslave.index, 1, out end, new ushort[] { (ushort)(((ushort)setslave.esm)|0x0100) });
                                        setslave.step = 2;
                                    }
                                }
                                break;
                            case 2:
                                master.ReadModuleAccessDeviceInWord(monitoringTimer, __parameter_address_table, out end, __parameter_values);
                                if (end == 0)
                                {
                                    if (__parameter_values[0].Span[0] == __parameter_values[1].Span[0])
                                    {

                                        if (end == 0)
                                        {
                                            __process_in_data_device.control_in = new REQUEST_MASTER_ESM_RESULT_T()
                                            {
                                                end_code = 0,
                                                exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                                timeout = false,
                                                esm = setslave.esm,
                                                error = __parameter_values[2].Span[0],
                                            };
                                            __process_out_data_device.control_out = null;
                                        }
                                    }
                                }
                                break;
                        }
                        if (__process_out_data_device.control_out != null)
                        {
                            if (end != 0)
                            {
                                __process_in_data_device.control_in = new REQUEST_SLAVE_ESM_RESULT_T()
                                {
                                    end_code = end,
                                    exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                    esm = setslave.esm,
                                    index = setslave.index
                                };
                                __process_out_data_device.control_out = null;
                            }
                            else if (Math.Abs(Environment.TickCount - setslave.start_ticks) > setslave.timeout)
                            {
                                __process_in_data_device.control_in = new REQUEST_SLAVE_ESM_RESULT_T()
                                {
                                    end_code = 0,
                                    exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                    timeout = true,
                                    esm = setslave.esm,
                                    index = setslave.index
                                };
                                __process_out_data_device.control_out = null;
                            }
                        }
                    }
                    catch (SLMPException ex)
                    {
                        __process_in_data_device.control_in = new REQUEST_SLAVE_ESM_RESULT_T()
                        {
                            end_code = 0,
                            exception_code = ex.ExceptionCode,
                            timeout = false,
                            esm = setslave.esm,
                            index = setslave.index 
                        };
                        __process_out_data_device.control_out = null;

                        if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                            throw;
                    }
                    finally
                    {
                        if (__process_out_data_device.control_out == null && setslave.step != 0)
                        {
                            master.WriteModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_SLAVE_ESM_REQUEST + setslave.index, 1, out _, new ushort[] { 0 });
                            __parameter_address_table = null;
                            __parameter_values = null;
                            __parameters = null;
                        }
                    }
                    break;
                case XMASTER_CONTROL_COMMAND_T xmaster:
                    try
                    {
                        switch (xmaster.step)
                        {
                            case 0:
                                __parameter_address_table = new List<(string, uint, ushort)>()
                                {
                                    (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_CONTROL_COMMAND, 1),
                                    (__device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_CONTROL_RESPONSE, 1),
                                };
                                __parameter_values = new Memory<ushort>[]
                                {
                                    new ushort[1],
                                    new ushort[1],
                                };
                                master.WriteModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_CONTROL_COMMAND, 1, out end, new ushort[] { (ushort)CONTROL_COMMAND_T.NONE });
                                xmaster.step = 1;
                                break;
                            case 1:
                                master.ReadModuleAccessDeviceInWord(monitoringTimer, __parameter_address_table, out end, __parameter_values);
                                if (end == 0)
                                {
                                    if (__parameter_values[0].Span[0] == __parameter_values[1].Span[0])
                                    {
                                        master.WriteModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_CONTROL_COMMAND, 1, out end, new ushort[] { (ushort)(xmaster.control) });
                                        xmaster.step = 2;
                                    }
                                }
                                break;
                            case 2:
                                master.ReadModuleAccessDeviceInWord(monitoringTimer, __parameter_address_table, out end, __parameter_values);
                                if (end == 0)
                                {
                                    if (__parameter_values[0].Span[0] == __parameter_values[1].Span[0])
                                    {

                                        if (end == 0)
                                        {
                                            __process_in_data_device.control_in = new XMASTER_CONTROL_RESULT_T()
                                            {
                                                end_code = 0,
                                                exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                                timeout = false,
                                                control = xmaster.control
                                            };
                                            __process_out_data_device.control_out = null;
                                        }
                                    }
                                }
                                break;
                        }
                        if (__process_out_data_device.control_out != null)
                        {
                            if (end != 0)
                            {
                                __process_in_data_device.control_in = new XMASTER_CONTROL_RESULT_T()
                                {
                                    end_code = end,
                                    exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                    control = xmaster.control
                                };
                                __process_out_data_device.control_out = null;
                            }
                            else if (Math.Abs(Environment.TickCount - xmaster.start_ticks) > xmaster.timeout)
                            {
                                __process_in_data_device.control_in = new XMASTER_CONTROL_RESULT_T()
                                {
                                    end_code = 0,
                                    exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR,
                                    timeout = true,
                                    control = xmaster.control
                                };
                                __process_out_data_device.control_out = null;
                            }
                        }
                    }
                    catch (SLMPException ex)
                    {
                        __process_in_data_device.control_in = new XMASTER_CONTROL_RESULT_T()
                        {
                            end_code = 0,
                            exception_code = ex.ExceptionCode,
                            timeout = false,
                            control = xmaster.control
                        };
                        __process_out_data_device.control_out = null;

                        if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                            throw;
                    }
                    finally
                    {
                        if (__process_out_data_device.control_out == null && xmaster.step != 0)
                        {
                            master.WriteModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification, (uint)RJ71EC92_ADDRESS_TABLE.BUF_CONTROL_COMMAND, 1, out _, new ushort[] { (ushort)CONTROL_COMMAND_T.NONE });
                            __parameter_address_table = null;
                            __parameter_values = null;
                            __parameters = null;
                        }
                    }
                    break;
            }

            if (__device_sync_control)
            { 
                try
                {
                    if (__device_sync_model == DEVICE_MODEL_TYPE_T.RJ72EC92)
                    {
                        master.ReadModuleAccessDeviceInWord(monitoringTimer, __process_in_address_table, out __process_in_data_device.process_in.pin_end_code, __process_in_data);
                        if (__process_in_data_device.process_in.pin_end_code == 0)
                        {
                            __process_in_data_device.process_in.master_diagnostic_info = MemoryMarshal.Read<ECAT_PROCESS_IN_DATA_T.PROCESS_IN_T.MASTER_DIAGNOSTIC_INFO_T>(MemoryMarshal.AsBytes(__process_in_data[0].Span));
                            __process_in_data_device.process_in.master_state_machine = MemoryMarshal.Read<STATE_MACHINE_T>(MemoryMarshal.AsBytes(__process_in_data[1].Span));
                            if (__process_in_address_table.Count == 8)
                            {
                                __process_in_data[2].CopyTo(__process_in_data_device.process_in.slave_pdo.Value.state_machines.AsMemory());
                                __process_in_data[3].CopyTo(__process_in_data_device.process_in.slave_pdo.Value.errors.AsMemory());
                            }
                        }

                        if (__process_in_data_device.process_in.pin_end_code == 0)
                        {
                            master.ReadLocalDeviceInWord(monitoringTimer, null, __process_xy_address_table, out __process_in_data_device.process_in.pin_end_code, null, __process_xy_data);
                            if (__process_in_data_device.process_in.pin_end_code == 0)
                            {
                                __process_in_data_device.process_in.x_status = __process_xy_data[0].Span[0];
                                __process_in_data_device.process_in.y_status = __process_xy_data[1].Span[0];
                            }
                        }

                        if (__process_in_data_device.process_in.pin_end_code == 0 && __slave_pdo_sync_mode != SLAVE_PDO_SYNC_MODE_T.Disabled)
                        {
                            if (__slave_pdo_sync_mode == SLAVE_PDO_SYNC_MODE_T.Interactive)
                            {
                                if (__process_in_data[4].Span[0] != __process_in_data[5].Span[0])
                                {
                                    __read_slave_pdo_from_device(master, monitoringTimer);
                                    if(__process_in_data_device.process_in.pin_end_code == 0)
                                    {
                                        master.WriteModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification,
                                            (uint)RJ71EC92_ADDRESS_TABLE.BUF_INPUT_DATA_READ_RESPONSE,
                                            1, out __process_in_data_device.process_in.pin_end_code,
                                            __process_in_data[4].Span);
                                    }
                                }
                            }
                            else
                                __read_slave_pdo_from_device(master, monitoringTimer);
                        }

                        if (__process_in_data_device.process_in.pin_end_code != 0)
                            __device_sync_control = false;
                        __process_in_data_device.process_in.pin_exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR;
                    }
                }
                catch (SLMPException ex)
                {
                    __process_in_data_device.process_in.pin_end_code = 0;
                    __process_in_data_device.process_in.pin_exception_code = ex.ExceptionCode;
                    __device_sync_control = false;
                    if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                        throw;
                }

                if (__device_sync_control)
                {
                    try
                    {
                        if (__device_sync_model == DEVICE_MODEL_TYPE_T.RJ72EC92)
                        {
                            if (__slave_pdo_sync_mode == SLAVE_PDO_SYNC_MODE_T.Interactive)
                            {
                                ushort request = __process_in_data[6].Span[0];
                                if (request == __process_in_data[7].Span[0])
                                {
                                    __write_slave_pdo_to_device(master, monitoringTimer);
                                    if (__process_in_data_device.process_in.pout_end_code == 0)
                                    {
                                        __process_in_data[6].Span[0] = (ushort)((++request) & 0xFF);
                                        master.WriteModuleAccessDeviceInWord(monitoringTimer, __device_sync_specification,
                                            (uint)RJ71EC92_ADDRESS_TABLE.BUF_OUTPUT_DATA_READ_REQUEST,
                                            1, out __process_in_data_device.process_in.pout_end_code,
                                            __process_in_data[6].Span);
                                    }
                                }
                            }

                            if (__process_in_data_device.process_in.pout_end_code != 0)
                                __device_sync_control = false;
                            __process_in_data_device.process_in.pout_exception_code = SLMP_EXCEPTION_CODE_T.NO_ERROR;
                        }
                    }
                    catch (SLMPException ex)
                    {
                        __process_in_data_device.process_in.pout_end_code = 0;
                        __process_in_data_device.process_in.pout_exception_code = ex.ExceptionCode;
                        __device_sync_control = false;
                        if (ex.ExceptionCode == SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                            throw;
                    }
                }
            }

            __device_write_data();
        }

        public override void ExchangeDataWithUserInterface()
        {
            __user_read_data();
            var res = __process_in_data_user_interface.control_in;
            if (res != null)
            {
                switch (res)
                {
                    case ENABLE_RESULT_T:
                        IsEnabled = true;
                        break;
                    case ENABLE_WITH_ENI_RESULT_T ret:
                        IsEnabled = true;
                        ENIOpened = true;
                        ENIPath = ret.slaves.ENIPath;
                        __process_out_data_user_interface.process_out.slave_pdo = new ECAT_PROCESS_OUT_DATA_T.SLAVE_PDO_T(ret.slaves.RxSizeInWord);
                        break;
                    case DISABLE_RESULT_T:
                        IsEnabled = false;
                        break;
                    case SWITCH_INTERACTIVE_RESULT_T ret:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"SWITCH_INTERACTIVE({ret.enabled})", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"SWITCH_INTERACTIVE({ret.enabled})", res.end_code);
                        else
                        {
                            if (ret.enabled)
                            {
                                __slave_pdo_data.WriteRxPDO(ret.rx_pdo);
                                Array.Copy(ret.rx_pdo, __process_out_data_user_interface.process_out.slave_pdo.Value.rx_pdo, ret.rx_pdo.Length);
                            }
                            InteractiveSyncSlavePdo = ret.enabled;
                        }
                        break;
                    case RELOAD_MASTER_EVENT_HISTORY_RESULT_T ret:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"RELOAD_MASTER_EVENT_HISTORY", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"RELOAD_MASTER_EVENT_HISTORY", res.end_code);
                        else
                        {
                            __master_event_histroy = new List<string>();
                            unsafe
                            {
                                fixed (ushort* raw = ret.events)
                                {
                                    int next = *(int*)raw;
                                    ushort* eventsPtr = raw + 2;
                                    var start = raw + 2 + next * Marshal.SizeOf<MASTER_EVENT_T>() / 2;
                                    for(int i = 0; i < RJ71EC92.EVENTS_HISTORY_CAPACITY - next; ++i)
                                    {
                                        MASTER_EVENT_T* eventPtr = (MASTER_EVENT_T*)(start + i * Marshal.SizeOf<MASTER_EVENT_T>() / 2);
                                        __master_event_histroy.Add(__master_event_to_string(eventPtr));
                                    }
                                    for (int i = 0; i < next; ++i)
                                    {
                                        MASTER_EVENT_T* eventPtr = (MASTER_EVENT_T*)(eventsPtr + i * Marshal.SizeOf<MASTER_EVENT_T>() / 2);
                                        __master_event_histroy.Add(__master_event_to_string(eventPtr));
                                    }
                                }
                            }
                            OnPropertyChanged("MasterEventHistory", false);
                        }
                        break;
                    case EXECUTE_SDO_RESULT_T ret:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"EXECUTE_SDO: {ret.sdo_command}", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"EXECUTE_SDO: {ret.sdo_command}", res.end_code);
                        else if (ret.timeout == true)
                            __add_exception_info($"EXECUTE_SDO: {ret.sdo_command}", "Operation Timout");
                        else
                        {
                            SlaveSdoOperationError = ret.error;
                            if (ret.sdo_command == SDO_COMMAND_T.UPLOAD)
                            {
                                if (ret.header.data_size_in_byte > 0)
                                    MemoryMarshal.Cast<ushort, byte>(ret.data).Slice(0, (int)ret.header.data_size_in_byte).CopyTo(__recv_sdo_data_array);
                                __recv_sdo_data_array_length = ret.header.data_size_in_byte;

                                OnPropertyChanged("SlaveSdoRecvData", false);
                            }
                        }
                        break;
                    case REQUEST_MASTER_ESM_RESULT_T ret:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"REQUEST_MASTER_ESM: {ret.esm}", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"REQUEST_MASTER_ESM: {ret.esm}", res.end_code);
                        else if (ret.timeout == true)
                            __add_exception_info($"REQUEST_MASTER_ESM: {ret.esm}", "Operation Timout");
                        else
                            MasterStateMachineErrorCode = ret.error;
                        break;
                    case REQUEST_SLAVE_ESM_RESULT_T ret:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"REQUEST_SLAVE_ESM: ID {ret.index} -> {ret.esm}", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"REQUEST_SLAVE_ESM: ID {ret.index} -> {ret.esm}", res.end_code);
                        else if (ret.timeout == true)
                            __add_exception_info($"REQUEST_SLAVE_ESM: ID {ret.index} -> {ret.esm}", "Operation Timout");
                        else
                            SlaveStateMachineErrorCode = ret.error;
                        break;
                    case XMASTER_CONTROL_RESULT_T ret:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"EXECUTE MASTER CONTROL COMMAND: {ret.control}", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"EXECUTE MASTER CONTROL COMMAND: {ret.control}", res.end_code);
                        else if (ret.timeout == true)
                            __add_exception_info($"EXECUTE MASTER CONTROL COMMAND: {ret.control}", "Operation Timout");
                        break;
                    case EXECUTE_Y_RESULT_T ret:
                        if (res.exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                            __add_exception_info($"EXECUTE Y REQUEST: {ret.request_command} - {ret.request_value}", res.exception_code);
                        else if (res.end_code != 0)
                            __add_exception_info($"EXECUTE Y REQUEST: {ret.request_command} - {ret.request_value}", res.end_code);
                        break;
                }
                __process_in_data_user_interface.control_in = null;
                CommandPending = false;
            }

            if (IsEnabled)
            {
                if (__process_in_data_user_interface.process_in.pin_end_code != 0)
                {
                    __add_exception_info("Reading ProcessIn", __process_in_data_user_interface.process_in.pin_end_code);
                    IsEnabled = false;
                }
                else if (__process_in_data_user_interface.process_in.pin_exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                {
                    __add_exception_info("Reading ProcessIn", __process_in_data_user_interface.process_in.pin_exception_code);
                    IsEnabled = false;
                }
                else if (__process_in_data_user_interface.process_in.pout_end_code != 0)
                {
                    __add_exception_info("Writing ProcessOut", __process_in_data_user_interface.process_in.pout_end_code);
                    IsEnabled = false;
                }
                else if (__process_in_data_user_interface.process_in.pout_exception_code != SLMP_EXCEPTION_CODE_T.NO_ERROR)
                {
                    __add_exception_info("Writing ProcessOut", __process_in_data_user_interface.process_in.pout_exception_code);
                    IsEnabled = false;
                }
                else
                {
                    NumberOfSlavesRegistered = __process_in_data_user_interface.process_in.master_diagnostic_info.number_of_slaves_registered;
                    ConfigurationState = __process_in_data_user_interface.process_in.master_diagnostic_info.configuration_state;
                    CommunicationState = __process_in_data_user_interface.process_in.master_diagnostic_info.communication_state;
                    MasterErrorStatus = __process_in_data_user_interface.process_in.master_diagnostic_info.master_error;
                    CableErrorStatus = __process_in_data_user_interface.process_in.master_diagnostic_info.cable_error;
                    MasterESM = __process_in_data_user_interface.process_in.master_state_machine;
                    XStatus = __process_in_data_user_interface.process_in.x_status;

                    ClearModuleErrorRequested = (__process_in_data_user_interface.process_in.y_status & (1 << (int)Y_REQUEST_T.CLEAR_MODULE_ERROR)) != 0;

                    if (ENIOpened)
                    {
                        __slave_pdo_data.WriteTxPDO(__process_in_data_user_interface.process_in.slave_pdo.Value.state_machines,
                            __process_in_data_user_interface.process_in.slave_pdo.Value.errors,
                            __process_in_data_user_interface.process_in.slave_pdo.Value.tx_pdo,
                            __process_in_data_user_interface.process_in.slave_pdo.Value.rx_readback_pdo);

                        if (InteractiveSyncSlavePdo)
                        {
                            __slave_pdo_data.ReadRxPDO(__process_out_data_user_interface.process_out.slave_pdo.Value.rx_pdo);
                        }
                    }
                }
            }

            __user_write_data(InteractiveSyncSlavePdo);
        }

        public override long Restore(ref Utf8JsonReader reader)
        {
            long start = reader.BytesConsumed;
            if (reader.Read() == false || reader.TokenType != JsonTokenType.StartObject)
                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The given JSON object is not a valid data model instance.");

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string property = reader.GetString()!;
                    switch (property)
                    {
                        case "Device Address":
                            if (reader.Read() && reader.TokenType == JsonTokenType.String && _MODULE_ACCESS_EXTENSION_PATTERN.IsMatch(reader.GetString()!))
                                SetProperty(ref __device_address!, reader.GetString(), false, nameof(DeviceAddress));
                            else
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid string.");
                            break;
                        case "Operation Timeout":
                            if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt32(out __operation_timeout)) == false)
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid uint32 number.");
                            else
                                SetProperty(ref __operation_timeout, __operation_timeout, false, nameof(OperationTimeout));
                            break;
                        case "Slave Sdo Operation Node Address":
                            if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt16(out __slave_sdo_node_address)) == false)
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid uint16 number.");
                            else
                                SetProperty(ref __slave_sdo_node_address, __slave_sdo_node_address, false, nameof(SlaveSdoOperationNodeAddress));
                            break;
                        case "Slave Sdo Operation Object Index":
                            if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt16(out __slave_sdo_index)) == false)
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid uint16 number.");
                            else
                                SetProperty(ref __slave_sdo_index, __slave_sdo_index, false, nameof(SlaveSdoOperationObjectIndex));
                            break;
                        case "Slave Sdo Operation Object SubIndex":
                            if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt16(out __slave_sdo_sub_index)) == false)
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid uint16 number.");
                            else
                                SetProperty(ref __slave_sdo_sub_index, __slave_sdo_sub_index, false, nameof(SlaveSdoOperationObjectSubIndex));
                            break;
                        case "Slave Sdo Data Display Format":
                            if ((reader.Read() && reader.TokenType == JsonTokenType.String && Enum.TryParse(reader.GetString(), true, out __slave_sdo_data_display_format)) == false)
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid <SDO_DATA_DISPLAY_FORMAT_T> string.");
                            else
                                SetProperty(ref __slave_sdo_data_display_format, __slave_sdo_data_display_format, false, nameof(SlaveSdoDataDisplayFormat));
                            break;
                        case "Slave State Machine Node Index":
                            if ((reader.Read() && reader.TokenType == JsonTokenType.Number && reader.TryGetUInt16(out __slave_esm_node_index)) == false)
                                throw new ArgumentException($"{this.GetType().Assembly.FullName} : The property value of {property} in given JSON object is not a valid uint16 number.");
                            else
                                SetProperty(ref __slave_esm_node_index, __slave_esm_node_index, false, nameof(SlaveStateMachineNodeIndex));
                            break;
                    }
                }
            }
            return reader.BytesConsumed - start;
        }

        public override long Save(Utf8JsonWriter writer)
        {
            long start = writer.BytesPending;
            writer.WriteStartObject();

            writer.WritePropertyName("Device Address");
            writer.WriteStringValue(DeviceAddress);

            writer.WritePropertyName("Operation Timeout");
            writer.WriteNumberValue(OperationTimeout);

            writer.WritePropertyName("Slave Sdo Operation Node Address");
            writer.WriteNumberValue(SlaveSdoOperationNodeAddress);
            writer.WritePropertyName("Slave Sdo Operation Object Index");
            writer.WriteNumberValue(SlaveSdoOperationObjectIndex);
            writer.WritePropertyName("Slave Sdo Operation Object SubIndex");
            writer.WriteNumberValue(SlaveSdoOperationObjectSubIndex);

            writer.WritePropertyName("Slave Sdo Data Display Format");
            writer.WriteStringValue(SlaveSdoDataDisplayFormat.ToString());

            writer.WritePropertyName("Slave State Machine Node Index");
            writer.WriteNumberValue(SlaveStateMachineNodeIndex);

            writer.WriteEndObject();
            return writer.BytesPending - start;
        }

        protected override void _online_state_changed(bool online)
        {
            if (online == false)
            {
                IsEnabled = false;
                CommandPending = false;
                __reset();
            }
        }

        private unsafe static string __master_event_to_string(MASTER_EVENT_T* eventPtr)
        {
            uint code = eventPtr->code;
            byte* time = (byte*)eventPtr + 4;
            if (RJ71EC92.EventCodes.ContainsKey(code))
                return string.Format("{0}/{1}/{2} {3}:{4}:{5}:{6} -- {7} - {8}",
                                                    ASCIIEncoding.ASCII.GetString(time, 2),
                                                    ASCIIEncoding.ASCII.GetString(time + 2, 2),
                                                    ASCIIEncoding.ASCII.GetString(time + 4, 2),
                                                    ASCIIEncoding.ASCII.GetString(time + 6, 2),
                                                    ASCIIEncoding.ASCII.GetString(time + 8, 2),
                                                    ASCIIEncoding.ASCII.GetString(time + 10, 2),
                                                    ASCIIEncoding.ASCII.GetString(time + 12, 4),
                                                    code.ToString("X8"),
                                                    RJ71EC92.EventCodes[code]);
            else
                return string.Format("{0}/{1}/{2} {3}:{4}:{5}:{6} -- {7}",
                                    ASCIIEncoding.ASCII.GetString(time, 2),
                                    ASCIIEncoding.ASCII.GetString(time + 2, 2),
                                    ASCIIEncoding.ASCII.GetString(time + 4, 2),
                                    ASCIIEncoding.ASCII.GetString(time + 6, 2),
                                    ASCIIEncoding.ASCII.GetString(time + 8, 2),
                                    ASCIIEncoding.ASCII.GetString(time + 10, 2),
                                    ASCIIEncoding.ASCII.GetString(time + 12, 4),
                                    code.ToString("X8"));

        }
        public void __read_slave_pdo_from_device(DeviceAccessMaster master, ushort monitoringTimer)
        {
            foreach (var s in __slave_tx_pdo_segments)
            {
                master.ReadModuleAccessDeviceInWord(monitoringTimer,
                    __device_sync_specification, s.dev_start, s.dev_points, out __process_in_data_device.process_in.pin_end_code,
                    __process_in_data_device.process_in.slave_pdo.Value.tx_pdo.AsSpan((int)s.host_start));
                if (__process_in_data_device.process_in.pin_end_code != 0)
                    break;
            }
            if (__process_in_data_device.process_in.pin_end_code == 0)
            {
                foreach (var s in __slave_rx_pdo_segments)
                {
                    master.ReadModuleAccessDeviceInWord(monitoringTimer,
                        __device_sync_specification, s.dev_start, s.dev_points, out __process_in_data_device.process_in.pin_end_code,
                        __process_in_data_device.process_in.slave_pdo.Value.rx_readback_pdo.AsSpan((int)s.host_start));
                    if (__process_in_data_device.process_in.pin_end_code != 0)
                        break;
                }
            }
        }

        public void __write_slave_pdo_to_device(DeviceAccessMaster master, ushort monitoringTimer)
        {
            foreach (var s in __slave_rx_pdo_segments)
            {
                master.WriteModuleAccessDeviceInWord(monitoringTimer,
                    __device_sync_specification, s.dev_start, s.dev_points, out __process_in_data_device.process_in.pout_end_code,
                    __process_out_data_device.process_out.slave_pdo.Value.rx_pdo.AsSpan((int)s.host_start));
                if (__process_in_data_device.process_in.pout_end_code != 0)
                    break;
            }
        }

        public void __add_exception_info(string opertaion, string user)
        {
            if (ExceptionInfoCollection.Count > 128)
                ExceptionInfoCollection.RemoveAt(127);
            ExceptionInfoCollection.Insert(0, $"[{DateTime.Now.ToLocalTime()}]: <{opertaion}> : {user}.");
            Notification.Show(new ExceptionNotification(FriendlyName, $"<{opertaion}> : {user}."),
                ShowAnimation.VerticalMove, false);
        }

        public void __add_exception_info(string opertaion, ushort endcode)
        {
            if (ExceptionInfoCollection.Count > 128)
                ExceptionInfoCollection.RemoveAt(127);
            ExceptionInfoCollection.Insert(0, $"[{DateTime.Now.ToLocalTime()}]: <{opertaion}> returns end code 0x{endcode:X4}.");
            Notification.Show(new ExceptionNotification(FriendlyName, $"<{opertaion}> returns end code 0x{endcode:X4}."),
                ShowAnimation.VerticalMove, false);
        }

        private void __add_exception_info(string opertaion, SLMP_EXCEPTION_CODE_T code)
        {
            if (ExceptionInfoCollection.Count > 128)
                ExceptionInfoCollection.RemoveAt(127);
            ExceptionInfoCollection.Insert(0, $"[{DateTime.Now.ToLocalTime()}]: <{opertaion}> returns exception code <{code}>.");
            if (code != SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
            {
                Notification.Show(new ExceptionNotification(FriendlyName, $"<{opertaion}> returns exception code <{code}>."),
                    ShowAnimation.VerticalMove, false);
            }
        }

        private void __sync_process_out_data_device_with_user_specified(ushort[] rx)
        {
            lock(_synchronizer)
            {
                __process_out_data_device.process_out.slave_pdo = new ECAT_PROCESS_OUT_DATA_T.SLAVE_PDO_T(rx.Length);
                Array.Copy(rx, __process_out_data_device.process_out.slave_pdo.Value.rx_pdo, rx.Length);
                __process_out_data_device.process_out.CopyTo(ref __process_out_data_share.process_out);
                //__process_out_data_device.process_out.CopyTo(ref __process_out_data_user_interface.process_out);
            }
        }

        private void __device_read_data()
        {
            lock (_synchronizer)
            {
                __process_out_data_share.process_out.CopyTo(ref __process_out_data_device.process_out);
                if (__process_out_data_device.control_out == null && __process_out_data_share.control_out != null)
                {
                    __process_out_data_device.control_out = __process_out_data_share.control_out;
                    __process_out_data_share.control_out = null;
                }
            }
        }

        private void __device_write_data()
        {
            lock (_synchronizer)
            {
                __process_in_data_device.process_in.CopyTo(ref __process_in_data_share.process_in);
                if (__process_in_data_share.control_in == null && __process_in_data_device.control_in != null)
                {
                    __process_in_data_share.control_in = __process_in_data_device.control_in;
                    __process_in_data_device.control_in = null;
                }
            }
        }

        private void __user_read_data()
        {
            lock (_synchronizer)
            {
                __process_in_data_share.process_in.CopyTo(ref __process_in_data_user_interface.process_in);
                if (__process_in_data_user_interface.control_in == null && __process_in_data_share.control_in != null)
                {
                    __process_in_data_user_interface.control_in = __process_in_data_share.control_in;
                    __process_in_data_share.control_in = null;
                }
            }
        }

        private void __user_write_data(bool interactived)
        {
            lock (_synchronizer)
            {
                if(interactived)
                    __process_out_data_user_interface.process_out.CopyTo(ref __process_out_data_share.process_out);
                if (__process_out_data_share.control_out == null && __process_out_data_user_interface.control_out != null)
                {
                    __process_out_data_share.control_out = __process_out_data_user_interface.control_out;
                    __process_out_data_user_interface.control_out = null;
                }
            }
        }

        public bool Enable()
        {
            if (CommandPending)
                return false;

            __process_out_data_user_interface.control_out = new ENABLE_COMMAND_T() { device_address = DeviceAddress, model = DEVICE_MODEL_TYPE_T.RJ72EC92 };
            CommandPending = true;
            return true;
        }

        public bool EnableWithENI(GenericSlavePdosDataModel model)
        {
            if (CommandPending)
                return false;

            __process_out_data_user_interface.control_out = new ENABLE_WITH_ENI_COMMAND_T() { device_address = DeviceAddress, model = DEVICE_MODEL_TYPE_T.RJ72EC92, slaves = model };
            CommandPending = true;
            return true;
        }

        public bool Disable()
        {
            if (CommandPending)
                return false;

            __process_out_data_user_interface.control_out = new DISABLE_COMMAND_T();
            CommandPending = true;
            return true;
        }

        public bool SetSlavePdoInteractiveMode(bool enable)
        {
            if (CommandPending || ENIOpened == false)
                return false;

            __process_out_data_user_interface.control_out = new SWITCH_INTERACTIVE_COMMAND_T() { enabled = enable};
            CommandPending = true;
            return true;
        }

        private void __reset()
        {
            __process_in_address_table = null;
            __process_in_data = null;
            __process_xy_address_table = null;
            __process_xy_data = null;

            __slave_pdo_data = null;
            __slave_tx_pdo_segments = null;
            __slave_rx_pdo_segments = null;

            __process_in_data_device.Reset();
            __process_in_data_share.Reset();
            __process_in_data_user_interface.Reset();
            __process_out_data_device.Reset();
            __process_out_data_share.Reset();
            __process_out_data_user_interface.Reset();

            __device_sync_control = false;
            __slave_pdo_sync_mode = SLAVE_PDO_SYNC_MODE_T.Disabled;
        }

        public ObservableCollection<string> ExceptionInfoCollection { get; private set; } = new ObservableCollection<string>();
        private ECAT_PROCESS_IN_DATA_T __process_in_data_device = new ECAT_PROCESS_IN_DATA_T();
        private ECAT_PROCESS_IN_DATA_T __process_in_data_share = new ECAT_PROCESS_IN_DATA_T();
        private ECAT_PROCESS_IN_DATA_T __process_in_data_user_interface = new ECAT_PROCESS_IN_DATA_T();
        private ECAT_PROCESS_OUT_DATA_T __process_out_data_device = new ECAT_PROCESS_OUT_DATA_T();
        private ECAT_PROCESS_OUT_DATA_T __process_out_data_share = new ECAT_PROCESS_OUT_DATA_T();
        private ECAT_PROCESS_OUT_DATA_T __process_out_data_user_interface = new ECAT_PROCESS_OUT_DATA_T();
        private bool __device_sync_control = false;
        private uint __device_sync_address;
        private string __device_sync_specification = "U000";
        private DEVICE_MODEL_TYPE_T __device_sync_model;
        List<(string, uint, ushort)>? __process_in_address_table = null;
        Memory<ushort>[]? __process_in_data = null;
        List<(string, uint, ushort)>? __process_xy_address_table = null;
        Memory<ushort>[]? __process_xy_data = null;
        List<(string, uint, ushort)> __parameter_address_table = null;
        Memory<ushort>[] __parameter_values = null;
        List<(string deviceCode, uint headDevice, ushort devicePoints, ReadOnlyMemory<ushort> data)> __parameters = null;

        IEtherCATIOMasterSlaveData? __slave_pdo_data = null;
        SLAVE_PDO_SYNC_MODE_T __slave_pdo_sync_mode = SLAVE_PDO_SYNC_MODE_T.Disabled;
        List<(uint dev_start, ushort dev_points, int host_start, int host_points)>? __slave_tx_pdo_segments = null;
        List<(uint dev_start, ushort dev_points, int host_start, int host_points)>? __slave_rx_pdo_segments = null;
        ushort[] __tx_sync_access = new ushort[2];
        ushort[] __rx_sync_access = new ushort[2];

        private bool __command_pending;
        public bool CommandPending
        {
            get { return __command_pending; }
            private set { if (value != __command_pending) SetProperty(ref __command_pending, value, false); }
        }
        private bool __enabled;
        public bool IsEnabled
        {
            get { return __enabled; }
            private set
            {
                if (value != __enabled)
                {
                    SetProperty(ref __enabled, value, false);
                    if (value == false)
                    {
                        ENIOpened = false;
                    }
                }
            }
        }

        private string __device_address = "U032";
        public string DeviceAddress
        {
            get { return __device_address; }
            set
            {
                if (_MODULE_ACCESS_EXTENSION_PATTERN.IsMatch(value))
                {
                    if (__device_address != value)
                        SetProperty(ref __device_address, value);
                }
                else
                    throw new ArgumentException(@"The input string for 'Device Address' is not in a correct format.");
            }
        }

        private bool __eni_opened;
        public bool ENIOpened
        {
            get { return __eni_opened; }
            private set
            {
                if (value != __eni_opened)
                {
                    SetProperty(ref __eni_opened, value, false);
                    if (value == false)
                    {
                        InteractiveSyncSlavePdo = false;
                        ENIPath = "N/A";
                    }
                }
            }
        }

        private string __eni_path;
        public string ENIPath
        {
            get { return __eni_path; }
            private set
            {
                if (value != __eni_path)
                {
                    SetProperty(ref __eni_path, value, false);
                }
            }
        }

        private bool __clear_module_error_requested = false;
        public bool ClearModuleErrorRequested
        {
            get { return __clear_module_error_requested; }
            private set
            {
                if (value != __clear_module_error_requested)
                {
                    SetProperty(ref __clear_module_error_requested, value, false);
                }
            }
        }

        private bool __interactive_sync_slave_pdo = false;
        public bool InteractiveSyncSlavePdo
        {
            get { return __interactive_sync_slave_pdo; }
            private set
            {
                if (value != __interactive_sync_slave_pdo)
                {
                    SetProperty(ref __interactive_sync_slave_pdo, value, false);
                }
            }
        }

        public ushort NumberOfSlavesRegistered
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic_info.number_of_slaves_registered; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic_info.number_of_slaves_registered, value, false); }
        }

        public CONFIGURATION_STATE_T ConfigurationState
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic_info.configuration_state; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic_info.configuration_state, value, false); }
        }

        public COMMUNICATION_STATE_T CommunicationState
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic_info.communication_state; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic_info.communication_state, value, false); }
        }

        public ushort MasterErrorStatus
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic_info.master_error; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic_info.master_error, value, false); }
        }

        public ushort CableErrorStatus
        {
            get { return __process_in_data_user_interface.process_in.master_diagnostic_info.cable_error; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_diagnostic_info.cable_error, value, false); }
        }

        public ushort XStatus
        {
            get { return __process_in_data_user_interface.process_in.x_status; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.x_status, value, false); }
        }

        public STATE_MACHINE_T MasterESM
        {
            get { return __process_in_data_user_interface.process_in.master_state_machine; }
            private set { SetProperty(ref __process_in_data_user_interface.process_in.master_state_machine, value, false); }
        }


        private List<string> __master_event_histroy = new List<string>();
        public IEnumerable<string> MasterEventHistory { get { return __master_event_histroy.Reverse<string>(); } }

        public bool ReloadMasterEventHistory()
        {
            if (CommandPending)
                return false;

            __process_out_data_user_interface.control_out = new RELOAD_MASTER_EVENT_HISTORY_COMMAND_T();
            CommandPending = true;
            return true;
        }

        private uint __operation_timeout = 5000;
        public uint OperationTimeout
        {
            get { return __operation_timeout; }
            set { SetProperty(ref __operation_timeout, value, true); }
        }

        private ushort __slave_sdo_node_address = 1001;
        public ushort SlaveSdoOperationNodeAddress
        {
            get { return __slave_sdo_node_address; }
            set { SetProperty(ref __slave_sdo_node_address, value, true); }
        }

        private ushort __slave_sdo_index = 0;
        public ushort SlaveSdoOperationObjectIndex
        {
            get { return __slave_sdo_index; }
            set { SetProperty(ref __slave_sdo_index, value, true); }
        }

        private ushort __slave_sdo_sub_index = 0;
        public ushort SlaveSdoOperationObjectSubIndex
        {
            get { return __slave_sdo_sub_index; }
            set { SetProperty(ref __slave_sdo_sub_index, value, true); }
        }

        private SDO_DATA_DISPLAY_FORMAT_T __slave_sdo_data_display_format = SDO_DATA_DISPLAY_FORMAT_T.BYTE_ARRAY;
        public SDO_DATA_DISPLAY_FORMAT_T SlaveSdoDataDisplayFormat
        {
            get { return __slave_sdo_data_display_format; }
            set
            {
                SetProperty(ref __slave_sdo_data_display_format, value, true);
                OnPropertyChanged("SlaveSdoRecvData", false);
            }
        }

        private uint __sdo_operation_error = 0;
        public uint SlaveSdoOperationError
        {
            get { return __sdo_operation_error; }
            private set { SetProperty(ref __sdo_operation_error, value, false); }
        }

        private byte[] __recv_sdo_data_array = new byte[(int)(RJ71EC92.SDO_DATA_SIZE_IN_BYTE)];
        private int __recv_sdo_data_array_length = 0;
        public string SlaveSdoRecvData
        {
            get
            {
                try
                {
                    switch (__slave_sdo_data_display_format)
                    {
                        case SDO_DATA_DISPLAY_FORMAT_T.BYTE_ARRAY:
                            return String.Join(", ", __recv_sdo_data_array.Take(__recv_sdo_data_array_length).Select(x => x.ToString("X2")));
                        case SDO_DATA_DISPLAY_FORMAT_T.INT16:
                            if (__recv_sdo_data_array_length >= 2)
                                return System.BitConverter.ToInt16(__recv_sdo_data_array, 0).ToString();
                            else
                                return "Unsupported data length";
                        case SDO_DATA_DISPLAY_FORMAT_T.UINT16:
                            if (__recv_sdo_data_array_length >= 2)
                                return System.BitConverter.ToUInt16(__recv_sdo_data_array, 0).ToString();
                            else
                                return "Unsupported data length";
                        case SDO_DATA_DISPLAY_FORMAT_T.INT32:
                            if (__recv_sdo_data_array_length >= 4)
                                return System.BitConverter.ToInt32(__recv_sdo_data_array, 0).ToString();
                            else
                                return "Unsupported data length";
                        case SDO_DATA_DISPLAY_FORMAT_T.UINT32:
                            if (__recv_sdo_data_array_length >= 4)
                                return System.BitConverter.ToUInt32(__recv_sdo_data_array, 0).ToString();
                            else
                                return "Unsupported data length";
                        case SDO_DATA_DISPLAY_FORMAT_T.INT64:
                            if (__recv_sdo_data_array_length >= 8)
                                return System.BitConverter.ToInt64(__recv_sdo_data_array, 0).ToString();
                            else
                                return "Unsupported data length";
                        case SDO_DATA_DISPLAY_FORMAT_T.UINT64:
                            if (__recv_sdo_data_array_length >= 8)
                                return System.BitConverter.ToUInt64(__recv_sdo_data_array, 0).ToString();
                            else
                                return "Unsupported data length";
                        case SDO_DATA_DISPLAY_FORMAT_T.SINGLE:
                            if (__recv_sdo_data_array_length >= 4)
                                return System.BitConverter.ToSingle(__recv_sdo_data_array, 0).ToString();
                            else
                                return "Unsupported data length";
                        case SDO_DATA_DISPLAY_FORMAT_T.DOUBLE:
                            if (__recv_sdo_data_array_length >= 8)
                                return System.BitConverter.ToDouble(__recv_sdo_data_array, 0).ToString();
                            else
                                return "Unsupported data length";
                        case SDO_DATA_DISPLAY_FORMAT_T.ASCII:
                            return ASCIIEncoding.ASCII.GetString(__recv_sdo_data_array, 0, __recv_sdo_data_array_length);
                        default:
                            return String.Join(", ", __recv_sdo_data_array.Take(__recv_sdo_data_array_length).Select(x => x.ToString("X2")));

                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        private byte[] __send_sdo_data_array;
        private int __send_sdo_data_array_length = 0;

        public string SlaveSdoSendData
        {
            set
            {
                if (value == null || value.Length == 0)
                {
                    __send_sdo_data_array = new byte[0];
                    __send_sdo_data_array_length = 0;
                }
                else
                {
                    switch (__slave_sdo_data_display_format)
                    {
                        case SDO_DATA_DISPLAY_FORMAT_T.BYTE_ARRAY:
                            var bytes = value.Split(',').Select(x => System.Convert.ToByte(x.Trim(), 16)).ToArray();
                            if (bytes.Length > RJ71EC92.SDO_DATA_SIZE_IN_BYTE)
                                throw new ArgumentOutOfRangeException($"The SDO size should be less than or equal to {RJ71EC92.SDO_DATA_SIZE_IN_BYTE} bytes.");
                            __send_sdo_data_array = bytes;
                            __send_sdo_data_array_length = bytes.Length;
                            break;
                        case SDO_DATA_DISPLAY_FORMAT_T.INT16:
                            __send_sdo_data_array = System.BitConverter.GetBytes(short.Parse(value));
                            __send_sdo_data_array_length = __send_sdo_data_array.Length;
                            break;
                        case SDO_DATA_DISPLAY_FORMAT_T.UINT16:
                            __send_sdo_data_array = System.BitConverter.GetBytes(ushort.Parse(value));
                            __send_sdo_data_array_length = __send_sdo_data_array.Length;
                            break;
                        case SDO_DATA_DISPLAY_FORMAT_T.INT32:
                            __send_sdo_data_array = System.BitConverter.GetBytes(int.Parse(value));
                            __send_sdo_data_array_length = __send_sdo_data_array.Length;
                            break;
                        case SDO_DATA_DISPLAY_FORMAT_T.UINT32:
                            __send_sdo_data_array = System.BitConverter.GetBytes(uint.Parse(value));
                            __send_sdo_data_array_length = __send_sdo_data_array.Length;
                            break;
                        case SDO_DATA_DISPLAY_FORMAT_T.INT64:
                            __send_sdo_data_array = System.BitConverter.GetBytes(long.Parse(value));
                            __send_sdo_data_array_length = __send_sdo_data_array.Length;
                            break;
                        case SDO_DATA_DISPLAY_FORMAT_T.UINT64:
                            __send_sdo_data_array = System.BitConverter.GetBytes(ulong.Parse(value));
                            __send_sdo_data_array_length = __send_sdo_data_array.Length;
                            break;
                        case SDO_DATA_DISPLAY_FORMAT_T.SINGLE:
                            __send_sdo_data_array = System.BitConverter.GetBytes(float.Parse(value));
                            __send_sdo_data_array_length = __send_sdo_data_array.Length;
                            break;
                        case SDO_DATA_DISPLAY_FORMAT_T.DOUBLE:
                            __send_sdo_data_array = System.BitConverter.GetBytes(double.Parse(value));
                            __send_sdo_data_array_length = __send_sdo_data_array.Length;
                            break;
                        case SDO_DATA_DISPLAY_FORMAT_T.ASCII:
                            bytes = ASCIIEncoding.ASCII.GetBytes(value);
                            if (bytes.Length > RJ71EC92.SDO_DATA_SIZE_IN_BYTE)
                                throw new ArgumentOutOfRangeException($"The SDO size should be less than or equal to {RJ71EC92.SDO_DATA_SIZE_IN_BYTE} bytes.");
                            __send_sdo_data_array = bytes;
                            __send_sdo_data_array_length = bytes.Length;
                            break;
                        default:
                            __send_sdo_data_array = new byte[0];
                            __send_sdo_data_array_length = 0;
                            break;
                    }
                }
            }
        }


        public bool UploadSDO()
        {
            if (CommandPending) return false;

            __process_out_data_user_interface.control_out = new EXECUTE_SDO_COMMAND_T()
            {
                sdo_command = SDO_COMMAND_T.UPLOAD,
                timeout = OperationTimeout,
                start_ticks = Environment.TickCount,
                header = new SDO_PARAMETER_T() { slave_address = SlaveSdoOperationNodeAddress, object_index = SlaveSdoOperationObjectIndex, object_sub_index = SlaveSdoOperationObjectSubIndex },
                data = null
            };
            CommandPending = true;
            return true;
        }

        public bool DownloadSDO()
        {
            if (CommandPending) return false;

            ushort[] sdo_data = null;
            if (__send_sdo_data_array_length > 0)
            {
                sdo_data = new ushort[__send_sdo_data_array_length / 2 + (__send_sdo_data_array_length % 2 == 0 ? 0 : 1)];
                __send_sdo_data_array.AsSpan().CopyTo(MemoryMarshal.Cast<ushort, byte>(sdo_data));
            }

            __process_out_data_user_interface.control_out = new EXECUTE_SDO_COMMAND_T()
            {
                sdo_command = SDO_COMMAND_T.DOWNLOAD,
                timeout = OperationTimeout,
                start_ticks = Environment.TickCount,
                header = new SDO_PARAMETER_T() { slave_address = SlaveSdoOperationNodeAddress, object_index = SlaveSdoOperationObjectIndex, object_sub_index = SlaveSdoOperationObjectSubIndex, data_size_in_byte = __send_sdo_data_array_length },
                data = sdo_data
            };
            CommandPending = true;
            return true;
        }


        private STATE_MACHINE_T __master_requested_state_machine = STATE_MACHINE_T.OP;
        public STATE_MACHINE_T MasterRequestedStateMachine
        {
            get { return __master_requested_state_machine; }
            set { SetProperty(ref __master_requested_state_machine, value, false); }
        }

        private ushort __master_esm_error_code = 0;
        public ushort MasterStateMachineErrorCode
        {
            get { return __master_esm_error_code; }
            private set { SetProperty(ref __master_esm_error_code, value, false); }
        }

        public bool RequestMasterStateMachine()
        {
            if (CommandPending) return false;
            __process_out_data_user_interface.control_out = new REQUEST_MASTER_ESM_COMMAND_T()
            {
                timeout = OperationTimeout,
                start_ticks = Environment.TickCount,
                esm = MasterRequestedStateMachine
            };
            CommandPending = true;
            return true;
        }

        private STATE_MACHINE_T __slave_requested_state_machine = STATE_MACHINE_T.OP;
        public STATE_MACHINE_T SlaveRequestedStateMachine
        {
            get { return __slave_requested_state_machine; }
            set { SetProperty(ref __slave_requested_state_machine, value, false); }
        }

        private ushort __slave_esm_error_code = 0;
        public ushort SlaveStateMachineErrorCode
        {
            get { return __slave_esm_error_code; }
            private set { SetProperty(ref __slave_esm_error_code, value, false); }
        }

        private ushort __slave_esm_node_index = 0;
        public ushort SlaveStateMachineNodeIndex
        {
            get { return __slave_esm_node_index; }
            set { SetProperty(ref __slave_esm_node_index, value); }
        }
        public bool RequestSlaveStateMachine()
        {
            if (CommandPending) return false;
            __process_out_data_user_interface.control_out = new REQUEST_SLAVE_ESM_COMMAND_T()
            {
                timeout = OperationTimeout,
                start_ticks = Environment.TickCount,
                esm = SlaveRequestedStateMachine,
                index = SlaveStateMachineNodeIndex
            };
            CommandPending = true;
            return true;
        }

        private CONTROL_COMMAND_T __master_control_command = CONTROL_COMMAND_T.NONE;
        public CONTROL_COMMAND_T MasterControlCommand
        {
            get { return __master_control_command; }
            set { SetProperty(ref __master_control_command, value, false); }
        }

        public bool ExecuteMasterControlCommand()
        {
            if (CommandPending) return false;
            __process_out_data_user_interface.control_out = new XMASTER_CONTROL_COMMAND_T()
            {
                timeout = OperationTimeout,
                start_ticks = Environment.TickCount,
                control = MasterControlCommand,
            };
            CommandPending = true;
            return true;
        }

        public bool RequestClearModuleError(bool rsq)
        {
            if (CommandPending) return false;
            __process_out_data_user_interface.control_out = new EXECUTE_Y_COMMAND_T()
            {
                request_command = Y_REQUEST_T.CLEAR_MODULE_ERROR,
                request_value = rsq,
            };
            CommandPending = true;
            return true;
        }
    }

    public class Factory : ICabinet
    {
        private string __version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        public string Name { get => "EtherCATIOMasterUtility"; set => throw new NotImplementedException(); }
        public string Description { get => @"EtherCAT IO Master Diagnostic Tool. Compatible with RJ71EC92"; set => throw new NotImplementedException(); }
        public string Version { get => __version; set => throw new NotImplementedException(); }

        public object CreateInstance(PropertyChangedEventHandler propertyChangedEventHandler)
        {
            EtherCATIOMasterUtilityDataModel data = new EtherCATIOMasterUtilityDataModel() { FriendlyName = "EtherCATIOMasterUtility" };
            data.UserPropertyChanged += propertyChangedEventHandler;
            return new EtherCATIOMasterUtilityControl(data);
        }
    }
}
