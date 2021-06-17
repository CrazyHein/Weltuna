using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP;
using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.IOUtility;
using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Master;
using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Message;
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
    /// TargetProperty.xaml 的交互逻辑
    /// </summary>
    public partial class TargetPropertyControl : Window
    {
        private TargetManagerDataModel __host;
        private string __original_name;
        public TargetPropertyControl(TargetPropertyDataModel model, TargetManagerDataModel host, string originalName = null)
        {
            InitializeComponent();
            DataContext = model;
            __host = host;
            __original_name = originalName;
        }

        private int __property_error_counter = 0;
        private void TargetProperty_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                __property_error_counter++;
            else
                __property_error_counter--;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (__property_error_counter != 0)
                MessageBox.Show("At least one user input field is invalid.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (__original_name == null || __original_name != (DataContext as TargetPropertyDataModel).Name)
                {
                    if (__host.Find((DataContext as TargetPropertyDataModel).Name) != null)
                    {
                        MessageBox.Show("A target with the same name already exists.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                DialogResult = true;
                Close();
            }
        }

        private async void CommunicationTest_Click(object sender, RoutedEventArgs e)
        {
            if (__property_error_counter != 0)
                MessageBox.Show("At least one user input field is invalid.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                var property = DataContext as TargetPropertyDataModel;
                SocketInterface com = null;
                IsEnabled = false;
                _BusyIndicator.Visibility = Visibility.Visible;
                try
                {
                    DESTINATION_ADDRESS_T destination = new DESTINATION_ADDRESS_T(
                        property.NetworkNumber, property.StationNumber, property.ModuleIONumber, 
                        property.MultidropNumber, property.ExtensionStationNumber);

                    if (property.UDPTransportLayer == true)
                    {
                        com = new UDP(new System.Net.IPEndPoint(property.SourceIPv4, property.SourcePort),
                                new System.Net.IPEndPoint(property.DestinationIPv4, property.DestinationPort),
                                property.ReceiveBufferSize, property.SendTimeoutValue, property.ReceiveTimeoutValue);
                    }
                    else
                    {
                        com = new TCP(new System.Net.IPEndPoint(property.SourceIPv4, 0), 
                                new System.Net.IPEndPoint(property.DestinationIPv4, property.DestinationPort),
                                property.SendTimeoutValue, property.ReceiveTimeoutValue);
                        //__tcp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, new LingerOption(true, 0));
                        await Task.Run(() => (com as TCP).Connect());
                    }

                    var master = new RemoteOperationMaster(property.FrameType, property.DataCode, property.R_DedicatedMessageFormat, com, ref destination,
                        property.SendBufferSize, property.ReceiveBufferSize, null);

                    
                    (ushort end, string name, ushort code) = await master.ReadTypeNameAsync(property.MonitoringTimer);
                    
                    if(end == (ushort)RESPONSE_MESSAGE_ENDCODE_T.NO_ERROR)
                        MessageBox.Show($"Communication success, the destination device is { name.Trim()} (0x{code:X4}).", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show($"The destination device returns end code 0x{end:X4}.", "Warning Message", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
                catch(SLMPException ex)
                {
                    MessageBox.Show("At least one unexpected error occured while doing communication test.\n" + ex.Message , "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    _BusyIndicator.Visibility = Visibility.Hidden;
                    IsEnabled = true;
                    if (com != null) com.Dispose();
                    com = null;
                }
            }

        }
    }
}
