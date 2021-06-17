using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP;
using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.IOUtility;
using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Master;
using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Message;
using AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks
{
    public enum DATA_SYNCHRONIZER_STATE_T : byte
    {
        EXCEPTION = 0x01,
        READY = 0x02,
        CONNECTING = 0x03,
        CONNECTED = 0x04,
    }

    public class DataSynchronizer
    {
        private object __sync_property_access_lock = new object();
        private SemaphoreSlim __sync_operation_access_lock = new SemaphoreSlim(1);
        SocketInterface __io = null;
        private string __sync_exception_message = "";
        private int __polling_interval = 0;
        private IEnumerable<DataModel> __tools_collection;
        private AutoResetEvent __stop_event = new AutoResetEvent(false);
        private Thread __data_sync_thread;
        private DATA_SYNCHRONIZER_STATE_T __sync_state = DATA_SYNCHRONIZER_STATE_T.READY;
        private uint __heartbeat_counter = 0;

        public DataSynchronizer(IEnumerable<DataModel> tools)
        {
            __tools_collection = tools;
        }

        public async Task<DATA_SYNCHRONIZER_STATE_T> Startup(TargetPropertyDataModel target)
        {
            __sync_operation_access_lock.Wait();
            try
            {
                switch (State)
                {
                    case DATA_SYNCHRONIZER_STATE_T.CONNECTED:
                        //throw new InvalidOperationException("The data synchronization thread is still running.");
                        return DATA_SYNCHRONIZER_STATE_T.CONNECTED;
                    case DATA_SYNCHRONIZER_STATE_T.EXCEPTION:
                        if (__data_sync_thread != null)
                        {
                            __data_sync_thread.Join();
                            __data_sync_thread = null;
                        }
                        break;
                    case DATA_SYNCHRONIZER_STATE_T.READY:
                        break;
                }

                __stop_event.Reset();
                if (target.UDPTransportLayer)
                    __io = new UDP(new System.Net.IPEndPoint(target.SourceIPv4, target.SourcePort),
                                    new System.Net.IPEndPoint(target.DestinationIPv4, target.DestinationPort),
                                    target.ReceiveBufferSize, target.SendTimeoutValue, target.ReceiveTimeoutValue);
                else
                    __io = new TCP(new System.Net.IPEndPoint(target.SourceIPv4, 0),
                                    new System.Net.IPEndPoint(target.DestinationIPv4, target.DestinationPort),
                                    target.SendTimeoutValue, target.ReceiveTimeoutValue);
                DESTINATION_ADDRESS_T destination = new DESTINATION_ADDRESS_T(target.NetworkNumber, target.StationNumber, target.ModuleIONumber, target.MultidropNumber, target.ExtensionStationNumber);

                DeviceAccessMaster master = new DeviceAccessMaster(target.FrameType, target.DataCode, target.R_DedicatedMessageFormat, __io,
                                                                    ref destination, target.SendBufferSize, target.ReceiveBufferSize);

                if (__io is TCP)
                {
                    State = DATA_SYNCHRONIZER_STATE_T.CONNECTING;
                    await Task.Run(() => (__io as TCP).Connect());
                }

                __heartbeat_counter = 0;
                __data_sync_thread = new Thread(new ParameterizedThreadStart(__data_sync_routine));
                __data_sync_thread.Start(Tuple.Create(master, (ushort)(target.MonitoringTimer / 250), target.PollingInterval));
                State = DATA_SYNCHRONIZER_STATE_T.CONNECTED;
                return DATA_SYNCHRONIZER_STATE_T.CONNECTED;
            }
            catch (SLMPException ex)
            {
                State = DATA_SYNCHRONIZER_STATE_T.EXCEPTION;
                ExceptionMessage = ex.ToString();
                return DATA_SYNCHRONIZER_STATE_T.EXCEPTION;
            }
            finally
            {
                __sync_operation_access_lock.Release();
            }
        }

        public async Task<DATA_SYNCHRONIZER_STATE_T> Stop()
        {
            __sync_operation_access_lock.Wait();
            try
            {
                switch (State)
                {
                    case DATA_SYNCHRONIZER_STATE_T.READY:
                        //throw new InvalidOperationException("The data synchronization thread has not started yet.");
                        return DATA_SYNCHRONIZER_STATE_T.READY;
                    case DATA_SYNCHRONIZER_STATE_T.CONNECTED:
                        __stop_event.Set();
                        break;
                    case DATA_SYNCHRONIZER_STATE_T.EXCEPTION:
                        __stop_event.Reset();
                        break;
                }

                if (__data_sync_thread != null)
                {
                    await Task.Run(() => __data_sync_thread.Join());
                    __data_sync_thread = null;
                }
                if (__io != null)
                {
                    __io.Dispose();
                    __io = null;
                }
                State = DATA_SYNCHRONIZER_STATE_T.READY;
                return DATA_SYNCHRONIZER_STATE_T.READY;
            }
            finally
            {
                __sync_operation_access_lock.Release();
            }
        }

        public DATA_SYNCHRONIZER_STATE_T State
        {
            get
            {
                lock(__sync_property_access_lock)
                {
                    return __sync_state;
                }
            }
            private set
            {
                lock (__sync_property_access_lock)
                {
                    __sync_state = value;
                }
            }
        }

        public string ExceptionMessage
        {
            get
            {
                lock (__sync_property_access_lock)
                {
                    return __sync_exception_message;
                }
            }
            private set
            {
                lock (__sync_property_access_lock)
                {
                    __sync_exception_message = value;
                }
            }
        }

        public int PollingInterval
        {
            get
            {
                lock (__sync_property_access_lock)
                {
                    return __polling_interval;
                }
            }
            private set
            {
                lock (__sync_property_access_lock)
                {
                    __polling_interval = value;
                }
            }
        }

        public uint Counter
        {
            get
            {
                lock (__sync_property_access_lock)
                {
                    return __heartbeat_counter;
                }
            }
            private set
            {
                lock (__sync_property_access_lock)
                {
                    __heartbeat_counter = value;
                }
            }
        }

        private void __data_sync_routine(object param)
        {
            (DeviceAccessMaster master, ushort monitoring, int interval) = (Tuple<DeviceAccessMaster, ushort, int>)param;
            uint counter = 0;
            Stopwatch sw = new Stopwatch();

            sw.Start();
            while (true)
            {
                if(__stop_event.WaitOne(0))
                {
                    break;
                }
                foreach (var tool in __tools_collection)
                {
                    try
                    {
                        tool.ExchangeDataWithDevice(master, monitoring);
                        Counter = counter++;
                    }
                    catch (SLMPException ex)
                    {
                        if (ex.ExceptionCode != SLMP_EXCEPTION_CODE_T.RUNTIME_ERROR)
                        {
                            continue;
                        }
                        else
                        {
                            ExceptionMessage = ex.RuntimeException.Message;
                            Counter = 0;
                            State = DATA_SYNCHRONIZER_STATE_T.EXCEPTION;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionMessage = ex.Message;
                        Counter = 0;
                        State = DATA_SYNCHRONIZER_STATE_T.EXCEPTION;
                        return;
                    }

                }
                int ms = (int)(sw.ElapsedMilliseconds);
                if (ms < interval)
                    Thread.Sleep(interval - ms);
                PollingInterval = (int)sw.ElapsedMilliseconds;
                sw.Restart();
            }    
        }
    }
}
