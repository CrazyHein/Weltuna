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

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Obelia.SlavePDOs.Generic
{
    /// <summary>
    /// GenericSlavePdosControl.xaml 的交互逻辑
    /// </summary>
    public partial class GenericSlavePdosControl : UserControl
    {
        private GenericSlavePdosDataModel __local_context;
        public GenericSlavePdosControl(GenericSlavePdosDataModel model, EtherCATIOMasterUtilityDataModel host)
        {
            InitializeComponent();
            DataContext = host;
            __local_context = model;

            SlaveNodes.DataContext = model;
            SlaveRxDetails.DataContext = host;
        }

        private void SlavesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SlaveTxDetails.ItemsSource = ((e.Source as ListBox).SelectedItem as Slave).TxVariables.ToList();
            SlaveRxDetails.ItemsSource = ((e.Source as ListBox).SelectedItem as Slave).RxVariables.ToList();

            __local_context.SelectedSlave = (e.Source as ListBox).SelectedItem as Slave;


            ListCollectionView view = CollectionViewSource.GetDefaultView(SlaveTxDetails.ItemsSource) as ListCollectionView;
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(new PropertyGroupDescription("PDO"));

            view = CollectionViewSource.GetDefaultView(SlaveRxDetails.ItemsSource) as ListCollectionView;
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(new PropertyGroupDescription("PDO"));
        }

        private void ProcessDataValue_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (box != null && e.Key == Key.Enter)
            {
                BindingExpression binding = box.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
                e.Handled = true;
            }
        }

        private void SwitchSyncMode_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as EtherCATIOMasterUtilityDataModel).SetSlavePdoInteractiveMode(!(DataContext as EtherCATIOMasterUtilityDataModel).InteractiveSyncSlavePdo);
        }
    }

    internal class IOStatusIndicator : IValueConverter
    {
        static SolidColorBrush NORMAL = new SolidColorBrush(Color.FromRgb(102, 205, 0));
        static SolidColorBrush FAULT = new SolidColorBrush(Color.FromRgb(190, 190, 190));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == false)
                return FAULT;
            else
                return NORMAL;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class ESMIndicator : IValueConverter
    {
        static public readonly SolidColorBrush OP = new SolidColorBrush(Color.FromRgb(64, 255, 0));
        static public readonly SolidColorBrush SAFE_OP = new SolidColorBrush(Color.FromRgb(0, 64, 255));
        static public readonly SolidColorBrush PRE_OP = new SolidColorBrush(Color.FromRgb(255, 128, 255));
        static public readonly SolidColorBrush INIT = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        static public readonly SolidColorBrush NA = new SolidColorBrush(Color.FromRgb(190, 190, 190));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ushort)value)
            {
                case (ushort)STATE_MACHINE_T.NONE:
                    return NA;
                case (ushort)STATE_MACHINE_T.OP:
                    return OP;
                case (ushort)STATE_MACHINE_T.SAFE_OP:
                    return SAFE_OP;
                case (ushort)STATE_MACHINE_T.PRE_OP:
                    return PRE_OP;
                case (ushort)STATE_MACHINE_T.INIT:
                    return INIT;
                default:
                    return NA;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
