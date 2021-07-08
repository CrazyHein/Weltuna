using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Control
{
    /// <summary>
    /// ExceptionNotification.xaml 的交互逻辑
    /// </summary>
    public partial class ExceptionNotification
    {
        public ExceptionNotification(string caption, string message)
        {
            InitializeComponent();
            DataContext = new ExceptionContent() { Caption = caption, Message = message};
        }

        public string Message { get; set; }
    }

    public class ExceptionContent
    {
        public string Caption { get; set; }
        public string Message { get; set; }
    }
}
