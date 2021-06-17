using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Ohestren
{
    public class DeviceNetUtilityDataModel : DataModel
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

        public override void ExchangeDataWithDevice(DeviceAccessMaster master, ushort monitoring)
        {
            //throw new NotImplementedException();
        }

        public override long Restore(ref Utf8JsonReader reader)
        {
            //throw new NotImplementedException();
            return 0;
        }

        public override long Save(Utf8JsonWriter writer)
        {
            //throw new NotImplementedException();
            return 0;
        }

        public override void ExchangeDataWihtUserInterface()
        {
            //throw new NotImplementedException();
        }

        protected override void _online_state_changed(bool online)
        {
            //throw new NotImplementedException();
        }
    }

    public class Factory : ICabinet
    {
        private string __version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string Name { get => "DeviceNetUtility"; set => throw new NotImplementedException(); }
        public string Description { get => @"123456"; set => throw new NotImplementedException(); }
        public string Version { get => __version; set => throw new NotImplementedException(); }

        public object CreateInstance(PropertyChangedEventHandler propertyChangedEventHandler)
        {
            DeviceNetUtilityDataModel data = new DeviceNetUtilityDataModel();
            data.UserPropertyChanged += propertyChangedEventHandler;
            return new DeviceNetUtilityControl(data);
        }
    }
}
