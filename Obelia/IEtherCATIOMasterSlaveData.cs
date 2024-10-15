using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Obelia
{
    internal interface IEtherCATIOMasterSlaveData
    {
        void WriteTxPDO(ushort[] slaveESMs, ushort[] slaveErrors, ushort[] txData, ushort[] rxrbData);
        void WriteRxPDO(ushort[] rxData);
        void ReadRxPDO(ushort[] rxData);

        //oid ReadRxPDOWriteRequest(uint bytePos, Memory<byte> data);
    }
}
