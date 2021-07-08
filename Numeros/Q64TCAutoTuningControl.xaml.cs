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

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Numeros
{
    /// <summary>
    /// Q64TCUtilityControl.xaml 的交互逻辑
    /// </summary>
    public partial class Q64TCAutoTuningControl : UserControl
    {
        public Q64TCAutoTuningControl(Q64TCAutoTuningDataModel data)
        {
            InitializeComponent();
            DataContext = data;
        }


        private void Enable_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(_DeviceAddress))
                MessageBox.Show("The input string for 'Device Address' is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                (DataContext as Q64TCAutoTuningDataModel).Enable();
        }

        private void Disable_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as Q64TCAutoTuningDataModel).Disable();
        }

        private void SwtichChannel_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as Q64TCAutoTuningDataModel).SwitchChannel();
        }

        private void SetDeviceOperationMode_Checked(object sender, RoutedEventArgs e)
        {
            if ((DataContext as Q64TCAutoTuningDataModel).IsEnabled)
                (DataContext as Q64TCAutoTuningDataModel).SetDeviceOperationMode();
        }

        private void SetDeviceSettingMode_Checked(object sender, RoutedEventArgs e)
        {
            if((DataContext as Q64TCAutoTuningDataModel).IsEnabled)
                (DataContext as Q64TCAutoTuningDataModel).SetDeviceSettingMode();
        }

        private void SetChannelAutoMode_Checked(object sender, RoutedEventArgs e)
        {
            if ((DataContext as Q64TCAutoTuningDataModel).IsEnabled)
                (DataContext as Q64TCAutoTuningDataModel).SetChannelAutoMode();
        }

        private void SetChannelManualMode_Checked(object sender, RoutedEventArgs e)
        {
            if ((DataContext as Q64TCAutoTuningDataModel).IsEnabled)
                (DataContext as Q64TCAutoTuningDataModel).SetChannelManualMode();
        }

        private void ClearDeviceError_Click(object sender, RoutedEventArgs e)
        {
            if ((DataContext as Q64TCAutoTuningDataModel).IsEnabled)
                (DataContext as Q64TCAutoTuningDataModel).ClearDeviceError();
        }

        private void SetATRequest_Checked(object sender, RoutedEventArgs e)
        {
            if ((DataContext as Q64TCAutoTuningDataModel).IsEnabled)
            {
                if (__errors != 0)
                {
                    MessageBox.Show("The user input is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                    (e.OriginalSource as CheckBox).IsChecked = false;
                }
                else
                    (DataContext as Q64TCAutoTuningDataModel).SetATFlag();
            }
        }

        private void ResetATRequest_Checked(object sender, RoutedEventArgs e)
        {
            if ((DataContext as Q64TCAutoTuningDataModel).IsEnabled)
                (DataContext as Q64TCAutoTuningDataModel).ResetATFlag();
        }

        private int __errors = 0;
        private void UserInput_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                __errors++;
            else
                __errors--;
        }

        private void BackupDevicePIDConstants_Click(object sender, RoutedEventArgs e)
        {
            if (__errors != 0)
                MessageBox.Show("The user input is not in correct format.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                (DataContext as Q64TCAutoTuningDataModel).BackupDevicePIDConstants();
        }

        private void OnDataBindingError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                (DataContext as Q64TCAutoTuningDataModel).BindingErrors++;
            else
                (DataContext as Q64TCAutoTuningDataModel).BindingErrors--;
        }
    }

    internal class SelectedChannelToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == (int)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            return (int)parameter;
        }
    }

    internal class ProcessValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DECIMAL_POINT_POSITION_T position = (DECIMAL_POINT_POSITION_T)values[0];
            if (position == DECIMAL_POINT_POSITION_T.NOTHING)
                return $"{values[1]} {parameter}";
            else
            {
                double value = ((short)values[1]) / 10.0;
                return $"{value:F1} {parameter}";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class PercentageValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double percent;
            if (value is ushort)
                percent = ((ushort)value) / 100.0;
            else
                percent = ((short)value) / 100.0;
            //return $"{percent: 0.0} %";
            return $"{percent:F1} %";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class TimeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{value} s";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class FixedPointValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double real;
            if (value is ushort)
                real = ((ushort)value) / 10.0;
            else
                real = ((short)value) / 10.0;
            //return $"{percent: 0.0} %";
            return $"{real:F1} {parameter}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class DeviceOperationModeIndicator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DEVICE_OPERATION_MODE_T)value == (DEVICE_OPERATION_MODE_T)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class ChannnelOperaionIndicatior : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ushort flag = (ushort)values[0];
            int channel = (int)values[1];

            if (values.Length == 2)
            {
                return (flag & (1 << channel)) == 0 ? "AUTO Mode" : "MAN Mode";
            }
            else if (values.Length == 3)
            {
                CHANNEL_OPERATION_MODE_T mode = (flag & (1 << channel)) == 0 ? CHANNEL_OPERATION_MODE_T.AUTO : CHANNEL_OPERATION_MODE_T.MANUAL;
                return mode == (CHANNEL_OPERATION_MODE_T)values[2];
            }
            else
                return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class ChannnelATStatusIndicatior : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ushort flag = (ushort)values[0];
            int channel = (int)values[1];
            if (values.Length == 2)
                return (flag & (1 << channel)) == 0 ? "The auto tuning / self-tuning is not being performed or is completed." : "The auto tuning / self-tuning is being performed.";
            else if(values.Length == 3)
            {
                int request = (flag & (1 << channel)) == 0 ? 0 : 1;
                return request == (int)values[2];
            }
            else
                return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class ChannelATCanExecute : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)values[0];
            DEVICE_OPERATION_MODE_T device = (DEVICE_OPERATION_MODE_T)values[1];
            ushort channel = (ushort)values[2];
            return device == DEVICE_OPERATION_MODE_T.OPERATION_MODE && (channel & (1 << index)) == 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class DeviceErrorCodeInfo : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DeviceMetadata.ERROR_INFO((ushort)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}