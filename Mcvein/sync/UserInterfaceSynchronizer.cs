using AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks
{
    class UserInterfaceSynchronizer
    {
        private Timer __ui_data_refresh_timer;
        private Window __host;
        private Action __action;

        public UserInterfaceSynchronizer(Window host, Action action)
        {
            __host = host;
            __action = action;
            __ui_data_refresh_timer = new Timer(__ui_data_refresh_routine, null, Timeout.Infinite, 100);
        }

        public void Startup(int interval)
        {
            __ui_data_refresh_timer.Change(0, interval);
        }

        public void Stop()
        {
            __ui_data_refresh_timer.Change(Timeout.Infinite, 100);
        }


        private void __ui_data_refresh_routine(object param)
        {
            try
            {
                __host.Dispatcher.Invoke(__action, DispatcherPriority.Render);
            }
            catch
            {

            }
        }
    }
}
