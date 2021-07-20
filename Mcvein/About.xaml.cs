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
using System.Windows.Shapes;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks
{
    /// <summary>
    /// about.xaml 的交互逻辑
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string AssemblyTitle { get; init; } = "Mcvein";
        public string Description { get; init; } = "The container of SLMP based diagnostic tool for Mitsubishi PLC.";

        public string AssemblyVersion { get; init; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
