using AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Loader;
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
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TargetManagerDataModel __target_manager_data_model;
        private ToolsNavigationDataModel<LayoutDocument> __cabinets_navigation_data_model;
        private MainDataModel __main_wnd_data_model;
        private ToolLayout __docking_documents_layout;
        private readonly ToolLayout __docking_documents_layout_null;
        private DataSynchronizer __data_synchronizer;
        private UserInterfaceSynchronizer __user_interface_synchronizer;
        private string[] __counter = new string[]{ "-", "/", "|", "\\" };
        public MainWindow()
        {
            InitializeComponent();
            __main_wnd_data_model = new MainDataModel();
            DataContext = __main_wnd_data_model;
            try
            {     
                __cabinets_navigation_data_model = new ToolsNavigationDataModel<LayoutDocument>(null, null, Plugin_PropertyChanged);
                _CabinetContainer.Content = new ToolsNavigationControl(__cabinets_navigation_data_model);
                __docking_documents_layout = __save_documents_docking_layout(_ToolboxContainer);
                __docking_documents_layout_null = __docking_documents_layout;

                __data_synchronizer = new DataSynchronizer(__cabinets_navigation_data_model.ToolDataCollection);
                __user_interface_synchronizer = new UserInterfaceSynchronizer(this, __ui_data_refresh_handler);

            }
            catch(Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(this, $"At least one unexpected error occured during the boot process.\n{ex.Message}", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private DataModel __any_binding_error()
        {
            foreach(var m in __cabinets_navigation_data_model.ToolDataCollection)
            {
                if (m.BindingErrors != 0)
                    return m;
            }
            return null;
        }

        private void Switch_Main_Console_Themme(object sender, ExecutedRoutedEventArgs e)
        {
            var model = DataContext as MainDataModel;
            model.CurrentTheme = e.Parameter as Theme;
        }

        private void ActivateTarget_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = __target_manager_data_model != null &&__target_manager_data_model.SelectedTarget != null && __main_wnd_data_model.IsOnline == false;
        }

        private void ActivateTarget_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (__target_manager_data_model.ActivedTarget != __target_manager_data_model.SelectedTarget)
            {
                __target_manager_data_model.ActivedTarget = __target_manager_data_model.SelectedTarget;
                __main_wnd_data_model.IsDirty = true;
            }
        }

        private void AddTarget_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = __target_manager_data_model != null;
        }

        private void AddTarget_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var targetPropertyDataModel = new TargetPropertyDataModel();
            targetPropertyDataModel.EnableRemoteOperation = __main_wnd_data_model.IsOnline == false;
            var targetPropertyControl = new TargetPropertyControl(targetPropertyDataModel, __target_manager_data_model, null);
            if (targetPropertyControl.ShowDialog() == true)
            {
                __target_manager_data_model.Add(targetPropertyDataModel);
                __main_wnd_data_model.IsDirty = true;
            }
        }

        private void RemoveTarget_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = __target_manager_data_model != null && __target_manager_data_model.SelectedTarget != null &&
                (__main_wnd_data_model.IsOnline == false || (__target_manager_data_model.SelectedTarget != __target_manager_data_model.ActivedTarget));
        }

        private void RemoveTarget_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (HandyControl.Controls.MessageBox.Show(this, $"Do you want to remove {__target_manager_data_model.SelectedTarget.Name} ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                __target_manager_data_model.Remove(__target_manager_data_model.SelectedTarget);
                __main_wnd_data_model.IsDirty = true;
            }
        }

        private void TargetProperty_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = __target_manager_data_model != null && __target_manager_data_model.SelectedTarget != null;
        }

        private void TargetProperty_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var targetPropertyDataModel = __target_manager_data_model.SelectedTarget.ShallowCopy();
            targetPropertyDataModel.EnableRemoteOperation = __main_wnd_data_model.IsOnline == false;
            var targetPropertyControl = new TargetPropertyControl(targetPropertyDataModel, __target_manager_data_model, __target_manager_data_model.SelectedTarget.Name);
            if (targetPropertyControl.ShowDialog() == true)
            {
                __target_manager_data_model.Modify(__target_manager_data_model.SelectedTarget, targetPropertyDataModel);
                __main_wnd_data_model.IsDirty = true;
            }
        }



        private void OpenProject_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = __main_wnd_data_model.IsOnline == false;
        }

        private void OpenProject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (__main_wnd_data_model.IsDirty == true || __docking_layout_comparison(_ToolboxContainer, __docking_documents_layout) == false)
            {
                if (HandyControl.Controls.MessageBox.Show(this, $"Discard the changes you have made ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
            }

            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.Multiselect = false;
            open.Filter = "JavaScript Object Notation(*.json)|*.json";
            open.AddExtension = true;
            open.DefaultExt = "json";
            
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<LayoutDocument> floatingLayout = null;
                try
                {
                    __cabinets_navigation_data_model.ClearToolbox((doc) => doc.Close());
                    MainDataModel.RESTORE_FROM_JSON(open.FileName, out __target_manager_data_model, __cabinets_navigation_data_model, out __docking_documents_layout, out floatingLayout) ;
                }
                catch (Exception ex)
                {
                    HandyControl.Controls.MessageBox.Show(this, $"At least one unexpected error occured while opening the project.\n{ex.Message}", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (__target_manager_data_model == null)
                    __target_manager_data_model = new TargetManagerDataModel();
                _ConnectionTargetsContainer.Content = new TargetManagerControl(__target_manager_data_model);

                if (__docking_documents_layout == null)
                    __docking_documents_layout = __docking_documents_layout_null;
                if (floatingLayout != null)
                    __restore_documents_floating_layout(floatingLayout);
                _ToolboxContainer.Children.Clear();
                if (__docking_documents_layout != null)
                {
                    _ToolboxContainer.Orientation = (__docking_documents_layout as LayoutGroup).Orientation == TOOL_LAYOUT_ORIENTATION_T.HORIZONTAL ? Orientation.Horizontal : Orientation.Vertical;
                    __restore_documents_docking_layout((__docking_documents_layout as LayoutGroup).SubLayout, _ToolboxContainer);
                }

                __main_wnd_data_model.ProjectPath = open.FileName;
                __main_wnd_data_model.IsDirty = false;
            }     
        }

        private void NewProject_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = __main_wnd_data_model.IsOnline == false;
        }

        private void NewProject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (__main_wnd_data_model.IsDirty == true || __docking_layout_comparison(_ToolboxContainer, __docking_documents_layout) == false)
            {
                if (HandyControl.Controls.MessageBox.Show(this, $"Discard the changes you have made ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
            }
            __target_manager_data_model = new TargetManagerDataModel();
            _ConnectionTargetsContainer.Content = new TargetManagerControl(__target_manager_data_model);
            __cabinets_navigation_data_model.ClearToolbox((doc) => doc.Close());
            _ToolboxContainer.Children.Clear();

            __main_wnd_data_model.IsDirty = true;
            __main_wnd_data_model.ProjectPath = null;
        }

        private void SaveProject_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = __main_wnd_data_model.ProjectPath != null && (__main_wnd_data_model.IsDirty == true || __docking_layout_comparison(_ToolboxContainer, __docking_documents_layout) == false);
        }

        private void SaveProject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var m = __any_binding_error();
            if(m != null)
            {
                HandyControl.Controls.MessageBox.Show(this, $"At least one data binding error was found in <{m.FriendlyName}>.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                __docking_documents_layout = __save_documents_docking_layout(_ToolboxContainer);
                var floatingLayout = __save_documents_floating_layout();

                MainDataModel.SAVE_TO_JSON(__target_manager_data_model, __cabinets_navigation_data_model, __docking_documents_layout, floatingLayout, __main_wnd_data_model.ProjectPath);
                __main_wnd_data_model.IsDirty = false;
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(this, $"At least one unexpected error occured while saving the project.\n{ex.Message}", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveProjectAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = __target_manager_data_model != null;
        }

        private void SaveProjectAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var m = __any_binding_error();
            if (m != null)
            {
                HandyControl.Controls.MessageBox.Show(this, $"At least one data binding error was found in <{m.FriendlyName}>.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
                save.Filter = "JavaScript Object Notation(*.json)|*.json";
                save.AddExtension = true;
                save.DefaultExt = "json";
                if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    __docking_documents_layout = __save_documents_docking_layout(_ToolboxContainer);
                    var floatingLayout = __save_documents_floating_layout();

                    MainDataModel.SAVE_TO_JSON(__target_manager_data_model, __cabinets_navigation_data_model, __docking_documents_layout, floatingLayout, save.FileName);
                    __main_wnd_data_model.ProjectPath = save.FileName;
                    __main_wnd_data_model.IsDirty = false;
                }
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(this, $"At least one unexpected error occured while saving the project.\n{ex.Message}", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddToolbox_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = __target_manager_data_model != null;
        }

        private void AddToolbox_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UserControl ctrl = null;
            try
            {
                ctrl = (e.Parameter as CabinetShortcut).Cabinet.CreateInstance(Plugin_PropertyChanged) as UserControl;
                (ctrl.DataContext as DataModel).Online = __main_wnd_data_model.IsOnline;
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(this, $"The plug-in ({(e.Parameter as CabinetShortcut).Cabinet.GetType().Assembly.FullName}) is not compatible with this framework.\n{ex.Message}", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LayoutDocument doc = new LayoutDocument();
            __cabinets_navigation_data_model.TryAddToolbox(doc, ctrl);

            doc.Content = ctrl;
            doc.Title = (e.Parameter as CabinetShortcut).Cabinet.Name;
            doc.IconSource = (e.Parameter as CabinetShortcut).Icon;
            doc.Closed += ToolDocument_Closed;

            if (_ToolboxContainer.Children.Count == 0)
                _ToolboxContainer.Children.Add(new LayoutDocumentPane());
            else if (_ToolboxContainer.Children[0] is LayoutDocumentPane == false)
                _ToolboxContainer.Children.Insert(0, new LayoutDocumentPane());

            (_ToolboxContainer.Children[0] as LayoutDocumentPane).Children.Add(doc);
            (_ToolboxContainer.Children[0] as LayoutDocumentPane).SelectedContentIndex = (_ToolboxContainer.Children[0] as LayoutDocumentPane).Children.Count - 1;

            __main_wnd_data_model.IsDirty = true;
        }

        private void Connect_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = __target_manager_data_model != null && __target_manager_data_model.ActivedTarget != null && __main_wnd_data_model.IsOnline == false;
        }

        private async void Connect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IsEnabled = false;
            _BusyIndicator.BusyContent = "Connecting...";
            _BusyIndicator.Visibility = Visibility.Visible;
            var state = await __data_synchronizer.Startup(__target_manager_data_model.ActivedTarget);
            _BusyIndicator.Visibility = Visibility.Hidden;
            IsEnabled = true;

            if (state == DATA_SYNCHRONIZER_STATE_T.EXCEPTION)
                HandyControl.Controls.MessageBox.Show(this, $"At least one unexpected error occured while connecting to target.\n{__data_synchronizer.ExceptionMessage}", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                __main_wnd_data_model.IsOnline = true;
                __user_interface_synchronizer.Startup(100);
                foreach (var m in __cabinets_navigation_data_model.ToolDataCollection)
                    m.Online = true;
            }
        }

        private void Disconnect_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = __target_manager_data_model != null && __main_wnd_data_model.IsOnline == true;
        }

        private async void Disconnect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            __main_wnd_data_model.IsOnline = false;

            __user_interface_synchronizer.Stop();

            IsEnabled = false;
            await __data_synchronizer.Stop();
            IsEnabled = true;
            
            foreach (var m in __cabinets_navigation_data_model.ToolDataCollection)
                m.Online = false;

            __main_wnd_data_model.DataSyncState = __data_synchronizer.State;
            __main_wnd_data_model.DataSyncExceptionMessage = __data_synchronizer.ExceptionMessage;
            __main_wnd_data_model.DataPollingInterval = __data_synchronizer.PollingInterval;
        }

        private void ToolDocument_Closed(object sender, EventArgs e)
        {
            __cabinets_navigation_data_model.TryRemoveToolbox(sender as LayoutDocument);
            __main_wnd_data_model.IsDirty = true;
        }

        private void ToolContainer_ChildrenCollectionChanged(object sender, EventArgs e)
        {
            if (__main_wnd_data_model != null && __target_manager_data_model != null) __main_wnd_data_model.IsDirty = true;
        }

        private void ToolContainer_ChildrenTreeChanged(object sender, ChildrenTreeChangedEventArgs e)
        {
            if (__main_wnd_data_model != null && __target_manager_data_model != null) __main_wnd_data_model.IsDirty = true;
        }

        private void Plugin_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (__main_wnd_data_model != null && __target_manager_data_model != null) __main_wnd_data_model.IsDirty = true;
        }

        private void Plugin_LayoutUpdated(object sender, EventArgs e)
        {
            if (__main_wnd_data_model != null && __target_manager_data_model != null) __main_wnd_data_model.IsDirty = true;
        }

        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            if (__main_wnd_data_model.IsDirty == true || (__docking_documents_layout != null &&__docking_layout_comparison(_ToolboxContainer, __docking_documents_layout) == false))
            {
                if (HandyControl.Controls.MessageBox.Show(this, $"Discard the changes you have made ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            if(__main_wnd_data_model.IsOnline == true)
            {
                __user_interface_synchronizer.Stop();
                await __data_synchronizer.Stop();
                __main_wnd_data_model.IsOnline = false;
            }
        }

        private void __restore_documents_docking_layout(IReadOnlyList<ToolLayout> dockingLayouts, LayoutDocumentPaneGroup root)
        {
            foreach(var layout in dockingLayouts)
            {
                if (layout.Type == TOOL_LAYOUT_TYPE_T.PANEL)
                {
                    var layoutpanel = layout as LayoutPanel<LayoutDocument>;
                    LayoutDocumentPane panel = __restore_documents_docking_layout(layoutpanel.Documents);
                    panel.DockHeight = new GridLength(layoutpanel.Height, GridUnitType.Star);
                    panel.DockWidth = new GridLength(layoutpanel.Width, GridUnitType.Star);
                    root.Children.Add(panel);
                }
                else
                {
                    var layoutgroup = layout as LayoutGroup;
                    var group = new LayoutDocumentPaneGroup() { Orientation = layoutgroup.Orientation == TOOL_LAYOUT_ORIENTATION_T.HORIZONTAL ? Orientation.Horizontal : Orientation.Vertical };
                    group.DockHeight = new GridLength(layoutgroup.Height, GridUnitType.Star);
                    group.DockWidth = new GridLength(layoutgroup.Width, GridUnitType.Star);
                    __restore_documents_docking_layout(layoutgroup.SubLayout, group);
                    root.Children.Add(group);
                }
            }
        }

        private LayoutDocumentPane __restore_documents_docking_layout(IReadOnlyList<LayoutDocument> dockingDocuments)
        {
            var panel = new LayoutDocumentPane();
            foreach (var doc in dockingDocuments)
            {
                (var cabinet, var ui) = __cabinets_navigation_data_model[doc];
                (ui.DataContext as DataModel).Online = __main_wnd_data_model.IsOnline;
                doc.Content = ui;
                doc.Title = cabinet.Cabinet.Name;
                doc.IconSource = cabinet.Icon;
                doc.Closed += ToolDocument_Closed;
                panel.Children.Add(doc);
            }
            return panel;
        }

        private void __restore_documents_floating_layout(IReadOnlyList<LayoutDocument> floatingLayout)
        {
            if (_ToolboxContainer.Children.Count == 0)
                _ToolboxContainer.Children.Add(new LayoutDocumentPane());
            else if (_ToolboxContainer.Children[0] is LayoutDocumentPane == false)
                _ToolboxContainer.Children.Insert(0, new LayoutDocumentPane());

            foreach (var doc in floatingLayout)
            {
                (var cabinet, var ui) = __cabinets_navigation_data_model[doc];
                (ui.DataContext as DataModel).Online = __main_wnd_data_model.IsOnline;
                doc.Content = ui;
                doc.Title = cabinet.Cabinet.Name;
                doc.IconSource = cabinet.Icon;
                doc.Closed += ToolDocument_Closed;

                (_ToolboxContainer.Children[0] as LayoutDocumentPane).Children.Add(doc);
                
                doc.FloatingHeight = 480;
                doc.FloatingWidth = 640;
                doc.Float();
            }
        }

        private ToolLayout __save_documents_docking_layout(LayoutDocumentPaneGroup root)
        {
            LayoutGroup r = new LayoutGroup();
            r.SubLayout = new List<ToolLayout>();
            r.Orientation = root.Orientation == Orientation.Horizontal ? TOOL_LAYOUT_ORIENTATION_T.HORIZONTAL : TOOL_LAYOUT_ORIENTATION_T.VERTICAL;
            r.Height = root.DockHeight.Value;
            r.Width = root.DockWidth.Value;
            foreach (var sub in root.Children)
            {
                if (sub is LayoutDocumentPaneGroup)
                {
                    r.SubLayout.Add(__save_documents_docking_layout(sub as LayoutDocumentPaneGroup));
                }
                else
                {
                    r.SubLayout.Add(__save_documents_docking_layout(sub as LayoutDocumentPane));
                }
            }
            return r;
        }

        private ToolLayout __save_documents_docking_layout(LayoutDocumentPane root)
        {
            LayoutPanel<LayoutDocument> r = new LayoutPanel<LayoutDocument>();
            r.Documents = new List<LayoutDocument>();
            r.Height = root.DockHeight.Value;
            r.Width = root.DockWidth.Value;
            foreach (var doc in root.Children)
            {
                r.Documents.Add(doc as LayoutDocument);
            }
            return r;
        }

        private IReadOnlyList<LayoutDocument> __save_documents_floating_layout()
        {
            List<LayoutDocument> list = new List<LayoutDocument>();
            foreach(var floating in _AppContainer.FloatingWindows)
            {
                foreach (var doc in floating.Children)
                    list.Add(doc as LayoutDocument);
            }
            return list;
        }

        private bool __docking_layout_comparison(LayoutDocumentPaneGroup root, ToolLayout layout)
        {
            if(layout.Type == TOOL_LAYOUT_TYPE_T.GROUP)
            {
                var layoutgroup = layout as LayoutGroup;
                var orientation = layoutgroup.Orientation == TOOL_LAYOUT_ORIENTATION_T.HORIZONTAL ? Orientation.Horizontal : Orientation.Vertical;
                if (orientation == root.Orientation && root.Children.Count == layoutgroup.SubLayout.Count &&
                    root.DockHeight.Value == layoutgroup.Height && root.DockWidth.Value == layoutgroup.Width)
                {
                    for (int i = 0; i < layoutgroup.SubLayout.Count; ++i)
                    {
                        if (root.Children[i] is LayoutDocumentPaneGroup && layoutgroup.SubLayout[i].Type == TOOL_LAYOUT_TYPE_T.GROUP)
                        {
                            if(__docking_layout_comparison(root.Children[i] as LayoutDocumentPaneGroup, layoutgroup.SubLayout[i]) == false)
                                return false;
                        }
                        if (root.Children[i] is LayoutDocumentPane && layoutgroup.SubLayout[i].Type == TOOL_LAYOUT_TYPE_T.PANEL)
                        {
                            if (__docking_layout_comparison(root.Children[i] as LayoutDocumentPane, layoutgroup.SubLayout[i] as LayoutPanel<LayoutDocument>) == false)
                                return false;
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        private bool __docking_layout_comparison(LayoutDocumentPane root, LayoutPanel<LayoutDocument> layout)
        {
            if(root.DockHeight.Value == layout.Height && root.DockWidth.Value == layout.Width && root.Children.Count == layout.Documents.Count)
            {
                for (int i = 0; i < layout.Documents.Count; ++i)
                {
                    if (root.Children[i] != layout.Documents[i])
                        return false;
                }
                return true;
            }
            else
                return false;
        }


        private bool __docking_layout_comparison(ToolLayout layout0, ToolLayout layout1)
        {
            if (layout0.Type == layout1.Type)
            {
                switch(layout0.Type)
                {
                    case TOOL_LAYOUT_TYPE_T.PANEL:
                        var layoutpanel0 = layout0 as LayoutPanel<LayoutDocument>;
                        var layoutpanel1 = layout1 as LayoutPanel<LayoutDocument>;
                        if (layoutpanel0.Height == layoutpanel1.Height && layoutpanel0.Width == layoutpanel1.Width && layoutpanel0.Documents.Count == layoutpanel1.Documents.Count)
                        {
                            for (int i = 0; i < layoutpanel0.Documents.Count; ++i)
                            {
                                if (layoutpanel0.Documents[i] != layoutpanel1.Documents[i])
                                    return false;
                            }
                            return true;
                        }
                        else
                            return false;
                    case TOOL_LAYOUT_TYPE_T.GROUP:
                        var layoutgroup0 = layout0 as LayoutGroup;
                        var layoutgroup1 = layout0 as LayoutGroup;
                        if (layoutgroup0.Orientation == layoutgroup1.Orientation &&
                            layoutgroup0.SubLayout.Count == layoutgroup1.SubLayout.Count &&
                            layoutgroup0.Height == layoutgroup1.Height && layoutgroup0.Width == layoutgroup1.Width)
                        {
                            for(int i = 0; i < layoutgroup0.SubLayout.Count; ++i)
                            {
                                if (__docking_layout_comparison(layoutgroup0.SubLayout[i], layoutgroup1.SubLayout[i]) == false)
                                    return false;
                            }
                            return true;
                        }
                        else
                            return false;
                    default:
                        return false;
                }
            }
            else
                return false;
        }


        private async void __ui_data_refresh_handler()
        {
            if (__main_wnd_data_model.IsOnline == false)
                return;
            string info = null;
            __main_wnd_data_model.DataSyncState = __data_synchronizer.State;
            __main_wnd_data_model.DataSyncExceptionMessage = __data_synchronizer.ExceptionMessage;
            __main_wnd_data_model.DataPollingInterval = __data_synchronizer.PollingInterval;
            try
            {
                foreach (var tool in __cabinets_navigation_data_model.ToolDataCollection)
                    tool.ExchangeDataWihtUserInterface();
                _SynchronizeCounter.Text = __counter[__data_synchronizer.Counter / 4 % 4];
            }
            catch (Exception ex)
            {
                info = $"At least one unexpected error occured while exchanging data with user interface.\n{ex.Message}";
            }

            if (info == null)
            {
                if (__main_wnd_data_model.DataSyncState == DATA_SYNCHRONIZER_STATE_T.EXCEPTION)
                {    
                    info = $"At least one unexpected error occured while exchanging data with device.\n{__main_wnd_data_model.DataSyncExceptionMessage}";
                }
            }
            if(info != null)
            {
                __main_wnd_data_model.IsOnline = false;

                __user_interface_synchronizer.Stop();

                IsEnabled = false;
                await __data_synchronizer.Stop();
                IsEnabled = true;

                foreach (var m in __cabinets_navigation_data_model.ToolDataCollection)
                    m.Online = false;
                HandyControl.Controls.MessageBox.Show(this, info, "Error Message", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }

    internal class ThemeCheckedIconVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter == value ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class MainWindowTitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null)
                if ((bool)values[1])
                    return "Mcvein - New Project";
                else
                    return "Mcvein";
            else
                return "Mcvein - " + values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class BooleanValueVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == (bool)parameter ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class ConsoleControl
    {
        public static RoutedUICommand SetGenericTheme { get; private set; } = 
            new RoutedUICommand("Generic", "GenericTheme", typeof(ConsoleControl));
        public static RoutedUICommand SetMetroTheme { get; private set; } =
            new RoutedUICommand("Metro", "MetroTheme", typeof(ConsoleControl));
        public static RoutedUICommand SetAeroTheme { get; private set; } =
            new RoutedUICommand("Aero", "AeroTheme", typeof(ConsoleControl));
        public static RoutedUICommand SetVS2010Theme { get; private set; } =
            new RoutedUICommand("VS2010", "VS2010Theme", typeof(ConsoleControl));

        public static RoutedUICommand AddTarget { get; private set; } =
            new RoutedUICommand("Add", "AddTarget", typeof(ConsoleControl),
                new InputGestureCollection { new KeyGesture(Key.A, ModifierKeys.Control| ModifierKeys.Shift, "Ctrl+Shift+A") });
        public static RoutedUICommand RemoveTarget { get; private set; } =
            new RoutedUICommand("Remove", "RemoveTarget", typeof(ConsoleControl),
                new InputGestureCollection { new KeyGesture(Key.R, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift+R") });
        public static RoutedUICommand ActivateTarget { get; private set; } =
            new RoutedUICommand("Activate", "ActivateTarget", typeof(ConsoleControl),
                new InputGestureCollection { new KeyGesture(Key.E, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift+E") });
        public static RoutedUICommand TargetProperty { get; private set; } =
            new RoutedUICommand("Property", "TargetProperty", typeof(ConsoleControl),
                new InputGestureCollection { new KeyGesture(Key.P, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift+P") });

        public static RoutedUICommand NewProject { get; private set; } =
            new RoutedUICommand("New", "NewProject", typeof(ConsoleControl),
                new InputGestureCollection { new KeyGesture(Key.N, ModifierKeys.Control, "Ctrl+N") });
        public static RoutedUICommand OpenProject { get; private set; } =
            new RoutedUICommand("Open", "OpenProject", typeof(ConsoleControl),
                new InputGestureCollection { new KeyGesture(Key.O, ModifierKeys.Control, "Ctrl+O") });
        public static RoutedUICommand SaveProject { get; private set; } =
            new RoutedUICommand("Save", "SaveProject", typeof(ConsoleControl),
                new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Control, "Ctrl+S") });
        public static RoutedUICommand SaveProjectAs { get; private set; } =
            new RoutedUICommand("Save As", "SaveProjectAs", typeof(ConsoleControl),
                new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift+S") });

        public static RoutedUICommand AddTool { get; private set; } =
            new RoutedUICommand("Add Tool", "AddTool", typeof(ConsoleControl));

        public static RoutedUICommand Connect { get; private set; } =
            new RoutedUICommand("Connect", "Connect", typeof(ConsoleControl),
                new InputGestureCollection { new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift+C") });
        public static RoutedUICommand Disconnect { get; private set; } =
            new RoutedUICommand("Disconnect", "Disconnect", typeof(ConsoleControl),
                new InputGestureCollection { new KeyGesture(Key.D, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift+D") });
        public static RoutedUICommand RemoteReset { get; private set; } =
            new RoutedUICommand("Remote Reset", "RemoteReset", typeof(ConsoleControl));
    }
}
