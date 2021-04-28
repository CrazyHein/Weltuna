using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.IOUtility
{
    public interface SocketInterface
    {
        int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags = SocketFlags.None);
        int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags = SocketFlags.None);
    }
}
