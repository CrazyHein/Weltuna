using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Ohestren
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DeviceNetUtilityControl : UserControl
    {
        public DeviceNetUtilityControl(DeviceNetUtilityDataModel data)
        {
            InitializeComponent();
            DataContext = data;
            XStatusIndicator0.IndicatorComments = new string[] { "WDT", "IOC", "MCC", "MFE", "SD", "MSGE", "PS", "PSC" };
            XStatusIndicator0.IndicatorToolTips = new string[]
            {
                "watchdog_timer_error",
                "io_communicating",
                "message_communication_completion",
                "master_function_error_set_signal",
                "slave_down_signal",
                "message_communication_error",
                "parameter_saving",
                "parameter_save_completion"
            };
            XStatusIndicator1.IndicatorComments = new string[] {"SFE", "N/A", "HDT", "HDTC", "HDTE", "N/A", "N/A", "RDY" };
            XStatusIndicator1.IndicatorToolTips = new string[]
            {
                "slave_function_error_set_signal",
                "prohibited09",
                "hardware_testing",
                "hardware_test_completion",
                "hardware_test_error_detection",
                "prohibited13",
                "prohibited14",
                "module_ready"
            };
            NetworkStatusIndicator.IndicatorComments = new string[] { "CE", "N/A", "PE", "NE" };
            NetworkStatusIndicator.IndicatorToolTips = new string[]
            {
                "There is a node with a communication error.",
                "This bit is always off.",
                "A parameter error occurs.",
                "The communication cannot be performed due to a serious problem in the network."
            };

            SlaveNodeStatusIndicator.IndicatorComments = new string[] { "NRSP", "N/A", "WRD", "IOIC", "N/A", "N/A", "N/A", "RSRV" };
            SlaveNodeStatusIndicator.IndicatorToolTips = new string[] {
                "not_respond", "used_by_system_1", "attr_write_access_denied",
                "io_data_set_inconsistence", "used_by_system_4", "used_by_system_5",
                "used_by_system_6", "reserved_node" };
        }

        private int __explicit_message_field_binding_error = 0;
        private void ExplicitMessageFieldBinding_Error(object sender, ValidationErrorEventArgs e)
        {
            {
                if (e.Action == ValidationErrorEventAction.Added)
                    __explicit_message_field_binding_error++;
                else
                    __explicit_message_field_binding_error--;
            }
        }

        private void Enable_Click(object sender, RoutedEventArgs e)
        {
            if(Validation.GetHasError(_DeviceAddress))
                MessageBox.Show(this.Parent as Window, "The input string for 'Device Address' is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                (DataContext as DeviceNetUtilityDataModel).Enable();
        }

        private void Disable_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as DeviceNetUtilityDataModel).Disable();
        }

        private void StartIO_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(_MasterControlTimeout))
                MessageBox.Show("The input string for 'Control Timeout' is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                (DataContext as DeviceNetUtilityDataModel).StartIOCommunication();
        }

        private void StopIO_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(_MasterControlTimeout))
                MessageBox.Show("The input string for 'Control Timeout' is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                (DataContext as DeviceNetUtilityDataModel).StopIOCommunication();
        }

        private void ResetError_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(_MasterControlTimeout))
                MessageBox.Show("The input string for 'Control Timeout' is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                (DataContext as DeviceNetUtilityDataModel).ResetError();
        }

        private void ExecuteExplicitMessage(object sender, RoutedEventArgs e)
        {
            if(__explicit_message_field_binding_error != 0)
                MessageBox.Show("The user input data is not in correct format.", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                switch((EXPLICIT_MESSAGE_TYPE_T)_ExplicitMessageType.SelectedItem)
                {
                    case EXPLICIT_MESSAGE_TYPE_T.READ_SLAVE_ATTRIBUTE:
                        (DataContext as DeviceNetUtilityDataModel).ReadSlaveAttribute();
                        break;
                    case EXPLICIT_MESSAGE_TYPE_T.WRITE_SLAVE_ATTRIBUTE:
                        (DataContext as DeviceNetUtilityDataModel).WriteSlaveAttribute();
                        break;
                    case EXPLICIT_MESSAGE_TYPE_T.READ_SLAVE_DIAGNOSTIC_INFO:
                        (DataContext as DeviceNetUtilityDataModel).ReadSlaveDiagnosticInfo();
                        break;
                    case EXPLICIT_MESSAGE_TYPE_T.EXECUTE_CUSTOMIZED_MESSAGE:
                        (DataContext as DeviceNetUtilityDataModel).PostCustomizedMessage();
                        break;
                    case EXPLICIT_MESSAGE_TYPE_T.SET_POLLING_ASSEMBLY_PATH:
                        (DataContext as DeviceNetUtilityDataModel).SetPollingAssemblyPath();
                        break;
                }
            }
        }
    }

    internal class UnshortUpperByte : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ushort)value >> 8;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class UnshortLowerByte : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ushort)value & 0x00FF;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class MasterFunctionStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ushort e = (ushort)((ushort)value >> 8);
            return Enum.IsDefined(typeof(DNM_IO_COMMUNICATION_STATUS_T), e) ?
                                 (DNM_IO_COMMUNICATION_STATUS_T)Enum.ToObject(typeof(DNM_IO_COMMUNICATION_STATUS_T), e) :
                                 DNM_IO_COMMUNICATION_STATUS_T.OFFLINE;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class IsExplicitMessageField : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint user = uint.Parse((string)parameter);
            EXPLICIT_MESSAGE_TYPE_T command = (EXPLICIT_MESSAGE_TYPE_T)value;
            int select = 0;
            switch (command)
            {
                case EXPLICIT_MESSAGE_TYPE_T.READ_SLAVE_ATTRIBUTE:
                    select = 1;
                    break;
                case EXPLICIT_MESSAGE_TYPE_T.WRITE_SLAVE_ATTRIBUTE:
                    select = 2;
                    break;
                case EXPLICIT_MESSAGE_TYPE_T.EXECUTE_CUSTOMIZED_MESSAGE:
                    select = 4;
                    break;
                case EXPLICIT_MESSAGE_TYPE_T.READ_SLAVE_DIAGNOSTIC_INFO:
                    select = 8;
                    break;
                case EXPLICIT_MESSAGE_TYPE_T.SET_POLLING_ASSEMBLY_PATH:
                    select = 16;
                    break;
            }

            if ((user & select) != 0)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class ExplicitMessageDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] data = value as byte[];
            if (data == null || data.Length == 0)
                return "< N/A >";
            else
                return string.Join(",", data);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string userInput = value as string;
            if (userInput == null || userInput.Length == 0)
                return null;
            string[] userInputArray = userInput.Split(',');
            byte[] rawByteArray = new byte[userInputArray.Length];
            int i = 0;
            try
            {
                for (i = 0; i < userInputArray.Count(); i++)
                    rawByteArray[i] = byte.Parse(userInputArray[i]);
            }
            catch (Exception e)
            {
                return e;
            }
            return rawByteArray;
        }
    }

    internal enum EXPLICIT_MESSAGE_TYPE_T
    {
        READ_SLAVE_ATTRIBUTE,
        WRITE_SLAVE_ATTRIBUTE,
        READ_SLAVE_DIAGNOSTIC_INFO,
        EXECUTE_CUSTOMIZED_MESSAGE,
        SET_POLLING_ASSEMBLY_PATH
    }
}