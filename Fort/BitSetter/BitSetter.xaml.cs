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

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Control
{
    /// <summary>
    /// BitSetter.xaml 的交互逻辑
    /// </summary>
    public partial class BitSetter : UserControl
    {
        public static readonly DependencyProperty SetterValueProperty;
        public static readonly DependencyProperty SetterCommentsProperty;
        public static readonly DependencyProperty SetterToolTipsProperty;

        static BitSetter()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(new byte(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);
            SetterValueProperty = DependencyProperty.Register("SetterValue", typeof(byte), typeof(BitSetter), metadata);
            metadata = new FrameworkPropertyMetadata(new string[] { "0", "1", "2 ", "3", "4", "5", "6", "7" });
            SetterCommentsProperty = DependencyProperty.Register("SetterComments", typeof(string[]), typeof(BitSetter), metadata);
            metadata = new FrameworkPropertyMetadata(new string[] { "N/A", "N/A", "N/A ", "N/A", "N/A", "N/A", "N/A", "N/A"});
            SetterToolTipsProperty = DependencyProperty.Register("SetterToolTips", typeof(string[]), typeof(BitSetter), metadata);
        }


        public BitSetter()
        {
            InitializeComponent();
        }

        public byte SetterValue
        {
            get { return (byte)GetValue(SetterValueProperty); }
            set { SetValue(SetterValueProperty, value); }
        }

        public string[] SetterComments
        {
            set { SetValue(SetterCommentsProperty, value); }
        }

        public string[] SetterToolTips
        {
            set { SetValue(SetterToolTipsProperty, value); }
        }

        public int StartIndex
        {
            set
            {
                SetValue(SetterCommentsProperty, new string[]
                {
                    value.ToString(),
                    (value+1).ToString(CultureInfo.InvariantCulture),
                    (value+2).ToString(CultureInfo.InvariantCulture),
                    (value+3).ToString(CultureInfo.InvariantCulture),
                    (value+4).ToString(CultureInfo.InvariantCulture),
                    (value+5).ToString(CultureInfo.InvariantCulture),
                    (value+6).ToString(CultureInfo.InvariantCulture),
                    (value+7).ToString(CultureInfo.InvariantCulture),
                });
            }
        }


        private void SwitchContainer_Click(object sender, RoutedEventArgs e)
        {
            CheckBox switcher = e.OriginalSource as CheckBox;
            if (switcher != null)
            {
                int bitIndex = Int32.Parse(switcher.Uid);
                if (switcher.IsChecked == true)
                    SetterValue = (byte)(SetterValue | (1 << bitIndex));
                else
                    SetterValue = (byte)(SetterValue & ~(1 << bitIndex));
            }
        }
    }

    internal class SetterValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int bitIndex = (int)parameter;
            return (((byte)value) & (1 << bitIndex)) != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class SetterCommentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int bitIndex = (int)parameter;
            string[] data = (string[])value;
            if (data == null || data.Length <= bitIndex)
                return "N/A";
            else
                return data[bitIndex];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class SetterBackgroundConverter : IValueConverter
    {
        private SolidColorBrush __on_solid_brush = new SolidColorBrush(Colors.LawnGreen);
        private SolidColorBrush __off_solid_brush = new SolidColorBrush(Colors.LightGray);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
                return __on_solid_brush;
            else
                return __off_solid_brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
