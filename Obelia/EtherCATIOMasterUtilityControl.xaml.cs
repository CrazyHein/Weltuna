using AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Obelia.SlavePDOs.Generic;
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

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Obelia
{
    /// <summary>
    /// EtherCATIOMasterUtilityControl.xaml 的交互逻辑
    /// </summary>
    public partial class EtherCATIOMasterUtilityControl : UserControl
    {
        public EtherCATIOMasterUtilityControl(EtherCATIOMasterUtilityDataModel data)
        {
            InitializeComponent();
            DataContext = data;

            MasterErrorStatus0.IndicatorToolTips = new string[] {
                "configuration_exception", "subdevice_mismatch", "before_init", "before_preop", "before_safeop", "before_op", "tx_pdo_data_corruption", "rx_pdo_data_corruption"
            };

            MasterErrorStatus1.IndicatorToolTips = new string[] {
                "cyclic_frame_missing", "n/a", "at_least_one_slave_not_in_expected_esm_state", "n/a", "n/a", "n/a", "n/a", "n/a"
            };

            CableErrorStatus0.IndicatorToolTips = new string[] {
                "cable_redundancy_not_activated", "cable_redundancy_broken", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a"
            };

            CableErrorStatus1.IndicatorToolTips = new string[] {
                "n/a", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a"
            };
        }

        private void OnDataBindingError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                (DataContext as EtherCATIOMasterUtilityDataModel)!.BindingErrors++;
            else
                (DataContext as EtherCATIOMasterUtilityDataModel)!.BindingErrors--;
        }

        int __sdo_command_parameters_errors = 0;
        private void OnSDOCommandParametersError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                __sdo_command_parameters_errors++;
            else
                __sdo_command_parameters_errors--;
        }

        int __request_master_esm_parameters_errors = 0;
        private void OnRequestMasterEsmParametersError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                __request_master_esm_parameters_errors++;
            else
                __request_master_esm_parameters_errors--;
        }

        int __request_slave_esm_parameters_errors = 0;
        private void OnRequestSlaveEsmParametersError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                __request_slave_esm_parameters_errors++;
            else
                __request_slave_esm_parameters_errors--;
        }

        int __execute_master_control_command_parameters_errors = 0;
        private void OnExecuteMasterControlCommandParametersError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                __execute_master_control_command_parameters_errors++;
            else
                __execute_master_control_command_parameters_errors--;
        }

        private void Enable_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(_DeviceAddress))
                MessageBox.Show("The input string for 'Device Address' is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                (DataContext as EtherCATIOMasterUtilityDataModel)!.Enable();
                SlaveDiagnostic.Content = null;
            }
        }

        private void EnableWithENI_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(_DeviceAddress))
                MessageBox.Show("The input string for 'Device Address' is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
                open.Filter = "EtherCAT-Network-Information Files(*.xml)|*.xml";
                open.Multiselect = false;
                if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        GenericSlavePdosDataModel model = new GenericSlavePdosDataModel(open.FileName);
                        GenericSlavePdosControl control = new GenericSlavePdosControl(model, DataContext as EtherCATIOMasterUtilityDataModel);
                        SlaveDiagnostic.Content = control;

                        (DataContext as EtherCATIOMasterUtilityDataModel)!.EnableWithENI(model);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("At least one exception has occurred during the operation :\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Disable_Click(object sender, RoutedEventArgs e)
        {
            SlaveDiagnostic.Content = null;
            (DataContext as EtherCATIOMasterUtilityDataModel)?.Disable();
        }

        private void ReloadMasterEventHistory_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as EtherCATIOMasterUtilityDataModel).ReloadMasterEventHistory();
        }

        private void UploadSDO_Click(object sender, RoutedEventArgs e)
        {
            if (__sdo_command_parameters_errors != 0)
                MessageBox.Show("At least one SDO pamarameter is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                (DataContext as EtherCATIOMasterUtilityDataModel).UploadSDO();
        }

        private void DownloadSDO_Click(object sender, RoutedEventArgs e)
        {
            var binding = SdoDataToBeDownloaded.GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
            if (__sdo_command_parameters_errors != 0)
                MessageBox.Show("At least one SDO pamarameter is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                (DataContext as EtherCATIOMasterUtilityDataModel).DownloadSDO();
        }

        private void RequstMasterESM_Click(object sender, RoutedEventArgs e)
        {
            if (__request_master_esm_parameters_errors != 0)
                MessageBox.Show("At least one REQUEST MASTER ESM pamarameter is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                (DataContext as EtherCATIOMasterUtilityDataModel).RequestMasterStateMachine();
        }

        private void RequstSlaveESM_Click(object sender, RoutedEventArgs e)
        {
            if (__request_slave_esm_parameters_errors != 0)
                MessageBox.Show("At least one REQUEST SLAVE ESM pamarameter is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                (DataContext as EtherCATIOMasterUtilityDataModel).RequestSlaveStateMachine();
        }

        private void SdoDataToBeDownloaded_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                var binding = SdoDataToBeDownloaded.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }
        }

        private void ExecuteMasterControlCommand_Click(object sender, RoutedEventArgs e)
        {
            if (__execute_master_control_command_parameters_errors != 0)
                MessageBox.Show("At least one EXECUTE MASTER CONTROL COMMAND pamarameter is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                (DataContext as EtherCATIOMasterUtilityDataModel).ExecuteMasterControlCommand();
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

    public class HexadecimalUInt16 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ushort)value).ToString("X4");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToUInt16((string)value, 16);
            }
            catch
            {
                return new ArgumentException();
            }
        }
    }
}
