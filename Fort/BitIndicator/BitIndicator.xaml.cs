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
    /// WordIndicator.xaml 的交互逻辑
    /// </summary>
    public partial class BitIndicator : UserControl
    {
        public static readonly DependencyProperty IndicatorValueProperty;
        public static readonly DependencyProperty IndicatorCommentsProperty;
        public static readonly DependencyProperty IndicatorToolTipsProperty;

        static BitIndicator()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(new byte());
            IndicatorValueProperty = DependencyProperty.Register("IndicatorValue", typeof(byte), typeof(BitIndicator), metadata);
            metadata = new FrameworkPropertyMetadata(new string[] { "0", "1", "2 ", "3", "4", "5", "6", "7" });
            IndicatorCommentsProperty = DependencyProperty.Register("IndicatorComments", typeof(string[]), typeof(BitIndicator), metadata);
            metadata = new FrameworkPropertyMetadata(new string[] { "N/A", "N/A", "N/A ", "N/A", "N/A", "N/A", "N/A", "N/A"});
            IndicatorToolTipsProperty = DependencyProperty.Register("IndicatorToolTips", typeof(string[]), typeof(BitIndicator), metadata);
        }

        public BitIndicator()
        {
            InitializeComponent();
        }

        public byte IndicatorValue
        {
            get { return (byte)GetValue(IndicatorValueProperty); }
            set { SetValue(IndicatorValueProperty, value); }
        }

        public string[] IndicatorComments
        {
            set { SetValue(IndicatorCommentsProperty, value); }
        }

        public string[] IndicatorToolTips
        {
            set { SetValue(IndicatorToolTipsProperty, value); }
        }

        public int StartIndex
        {
            set
            {
                SetValue(IndicatorCommentsProperty, new string[]
                {
                    value.ToString(CultureInfo.InvariantCulture),
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
    }

    internal class IndicatorCommentConverter : IValueConverter
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

    internal class IndicatorValueConverter : IValueConverter
    {
        private SolidColorBrush __on_solid_brush = new SolidColorBrush(Colors.LawnGreen);
        private SolidColorBrush __off_solid_brush = new SolidColorBrush(Colors.LightGray);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int bitIndex = (int)parameter;
            byte data = (byte)value;
            if ((data & (1 << bitIndex)) != 0)
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
