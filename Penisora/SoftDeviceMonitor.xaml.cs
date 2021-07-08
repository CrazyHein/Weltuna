using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Penisora
{
    /// <summary>
    /// SoftDeviceMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class SoftDeviceMonitor : UserControl
    {
        public SoftDeviceMonitor(SoftDeviceDataModel data)
        {
            InitializeComponent();
            DataContext = data;
        }

        private void CommitUpdates(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox)
                {
                    TextBox box = sender as TextBox;
                    BindingExpression binding = box.GetBindingExpression(TextBox.TextProperty);
                    binding.UpdateSource();
                    if (Validation.GetHasError(box))
                        e.Handled = true;
                }
                else if (sender is ListViewItem)
                {
                    var model = DataContext as SoftDeviceDataModel;
                    var row = (sender as ListBoxItem).Content as SoftDeviceRowDataModel;
                    uint head = (uint)model.RowDataCollection.IndexOf(row)
                        + (DataContext as SoftDeviceDataModel).HeadDevice;

                    //if (model.SelectLocalDevice && (SoftDeviceDataModel.LOCAL_DEVICE_INFO[model.LocalDeviceName].Item2 == CommunicationProtocol.CrazyHein.SLMP.Command.DEVICE_ACCESS_TYPE_T.BIT))
                        //head *= 16;

                    (DataContext as SoftDeviceDataModel).PostWriteSingleCommand(model.SelectLocalDevice,
                        model.SelectLocalDevice ? model.LocalDeviceName : model.ModuleAccessDeviceName,
                        head, row.WordValue);
                }
            }
        }

        private void CancelUpdates(object sender, RoutedEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (box != null)
            {
                BindingExpression binding = box.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateTarget();
            }
        }

        private bool __import_user_settings()
        {
            bool error = false;
            var model = DataContext as SoftDeviceDataModel;

            /*
            BindingExpression binding;
            binding = _LocalDeviceSelected.GetBindingExpression(RadioButton.IsCheckedProperty);
            binding.UpdateSource();

            if (model.SelectLocalDevice == true)
                binding = _LocalDeviceName.GetBindingExpression(TextBox.TextProperty);
            else
                binding = _ModuleAccessDeviceName.GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();

            binding = _HeadDevice.GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
            binding = _DevicePoint.GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
            */
                
            if (model.SelectLocalDevice == true)
            {
                error = Validation.GetHasError(_LocalDeviceName) || Validation.GetHasError(_HeadDevice) || Validation.GetHasError(_DevicePoint);
            }
            else
            {
                error = Validation.GetHasError(_ModuleAccessDeviceName) || Validation.GetHasError(_HeadDevice) || Validation.GetHasError(_DevicePoint);
            }

            return error;
        }

        private void OnDeviceSettings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && e.OriginalSource is TextBox)
            {
                if (__import_user_settings() == true)
                {
                    MessageBox.Show("The user input is not valid.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    (DataContext as SoftDeviceDataModel).PostStartCommand();
            }
        }

        private void Power_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as SoftDeviceDataModel).PostStopCommand();
        }

        private void OnDataBindingError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                (DataContext as SoftDeviceDataModel).BindingErrors++;
            else
                (DataContext as SoftDeviceDataModel).BindingErrors--;
        }
    }

    internal class WordToBinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringBuilder sb = new StringBuilder(16);
            ushort v = (ushort)value;
            for (int i = 0; i < 16; ++i)
            {
                sb.Insert(0, v % 2 == 0 ? "0" : "1");
                v /= 2;
            }
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string)value;
            ushort v = 0;
            if (s.Length > 16)
                //throw new ArgumentException("Invalid Input String");
                return null;
            else
            {
                for (int i = 0; i < s.Length; ++i)
                {
                    v *= 2;
                    switch (s[i])
                    {
                        case '1': v += 1; break;
                        case '0': break;
                        default: return null;//throw new ArgumentException("Invalid Input String");
                    }
                }
            }
            return v;
        }
    }

    internal class WordToHexConverter : IValueConverter
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
                return null;
            }
        }
    }

    internal class WordToASCIIConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringBuilder sb = new StringBuilder(2);
            ushort v = (ushort)value;
            byte[] b = new byte[] { (byte)(v / 0x100), (byte)(v % 0x100) };
            for (int i = 0; i < 2; ++i)
            {
                try
                {
                    char c = Encoding.ASCII.GetString(b, i, 1)[0];
                    if (char.IsLetterOrDigit(c) || char.IsPunctuation(c))
                        sb.Append(c);
                    else
                    {
                        sb.Append(@"\x");
                        sb.Append(b[i].ToString("X2"));
                    }
                }
                catch
                {
                    sb.Append("N/A");
                }

                sb.Append("  ");
            }


            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class SwitchStateToImageConverter : IValueConverter
    {
        private readonly BitmapImage on_image = new BitmapImage(new Uri("imgs/switch_on.png", UriKind.Relative));
        private readonly BitmapImage off_image = new BitmapImage(new Uri("imgs/switch_off.png", UriKind.Relative));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)(value) == true)
                return on_image;
            else
                return off_image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}