using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.IOUtility
{
    public class UDP : SocketInterface
    {
        private bool disposedValue;

        private Socket __udp;
        private IPEndPoint __source_endpoint;
        private IPEndPoint __destination_endpoint;
        private int __internal_buffer_pointer;
        private int __internal_buffer_available;
        private byte[] __internal_buffer_memory;

        public UDP(IPEndPoint source, IPEndPoint destination, int internalBufferSize, int sendTimeout, int receiveTimeout)
        {
            __udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            __udp.Bind(source);
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            __udp.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
            __udp.SendTimeout = sendTimeout;
            __udp.ReceiveTimeout = receiveTimeout;
            __source_endpoint = source;
            __destination_endpoint = destination;
            __internal_buffer_memory = new byte[internalBufferSize];
            __internal_buffer_pointer = 0;
            __internal_buffer_available = 0;
        }
        
        public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags = SocketFlags.None)
        {
            int length = Math.Min(size, __internal_buffer_available);
            if (length != 0)
            {
                Array.Copy(__internal_buffer_memory, __internal_buffer_pointer, buffer, offset, length);
                __internal_buffer_available -= length;
                if (__internal_buffer_available == 0)
                    __internal_buffer_pointer = 0;
                else
                    __internal_buffer_pointer += length;
            }
           
            int left = size - length;
            EndPoint point = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                while (left != 0)
                {
                    offset += length;
                    length = __udp.ReceiveFrom( __internal_buffer_memory, 
                                                __internal_buffer_pointer, 
                                                __internal_buffer_memory.Length,
                                                socketFlags, 
                                                ref point);

                    if (length <= left)
                    {
                        Array.Copy(__internal_buffer_memory, __internal_buffer_pointer, buffer, offset, length);
                        left -= length;      
                    }
                    else
                    {
                        Array.Copy(__internal_buffer_memory, __internal_buffer_pointer, buffer, offset, left);
                        __internal_buffer_pointer = left;
                        __internal_buffer_available = length - left;
                        left = 0;
                    }
                }
            }
            catch (Exception e)
            {
                throw new SLMPException(e);
            }
            return size;
        }

        public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags = SocketFlags.None)
        {
            try
            {
                return __udp.SendTo(buffer, offset, size, socketFlags, __destination_endpoint);
            }
            catch (Exception e)
            {
                throw new SLMPException(e);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    __udp.Close();
                    __udp.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~UDP()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public string Name()
        {
            return "UDP";
        }
    }
}
