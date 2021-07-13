using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks
{
    class MainDataModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        virtual internal protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            //if (object.Equals(storage, value))
                //return;
            storage = value;
            OnPropertyChanged(propertyName);
        }

        public static Theme AERO_THEME { get; } = new AeroTheme();
        public static Theme VS2010_THEME { get; } = new VS2010Theme();
        public static Theme METRO_THEME { get; } = new MetroTheme();
        public static Theme GENERIC_THEME { get; } = new GenericTheme();
        private static ReadOnlySpan<byte> __utf8_bom => new byte[] { 0xEF, 0xBB, 0xBF };

        private Theme __current_theme;
        private string __project_name;
        private bool __is_dirty = false;
        private bool __is_online = false;

        public MainDataModel()
        {
            __current_theme = GENERIC_THEME;
        }

        public MainDataModel(Theme defaultTheme)
        {
            __current_theme = defaultTheme;
        }

        public Theme CurrentTheme
        {
            get { return __current_theme; }
            set
            {
                if (value == AERO_THEME || value == VS2010_THEME || value == METRO_THEME || value == GENERIC_THEME)
                    SetProperty(ref __current_theme, value);
                else
                    throw new ArgumentException();
            }
        }

        public string ProjectPath
        {
            get { return __project_name; }
            set { if (value != __project_name) SetProperty(ref __project_name, value); }
        }
     
        public bool IsDirty
        {
            get { return __is_dirty; }
            set { if (value != __is_dirty) SetProperty(ref __is_dirty, value); }
        }

        public bool IsOnline
        {
            get { return __is_online; }
            set { if (value != __is_online) SetProperty(ref __is_online, value); }
        }

        private DATA_SYNCHRONIZER_STATE_T __data_sync_state = DATA_SYNCHRONIZER_STATE_T.READY;
        public DATA_SYNCHRONIZER_STATE_T DataSyncState
        {
            get { return __data_sync_state; }
            set { if (value != __data_sync_state) SetProperty(ref __data_sync_state, value); }
        }

        private string __data_sync_exception = "";
        public string DataSyncExceptionMessage
        {
            get { return __data_sync_exception; }
            set { if (value != __data_sync_exception) SetProperty(ref __data_sync_exception, value); }
        }

        private int __data_polling_interval = 0;
        public int DataPollingInterval
        {
            get { return __data_polling_interval; }
            set { if (value != __data_polling_interval) SetProperty(ref __data_polling_interval, value); }
        }

        public static string SAVE_TO_JSON(TargetManagerDataModel targets, ToolsNavigationDataModel<LayoutDocument> navigation, double navigationWidth, ToolLayout dockingLayout, IReadOnlyList<LayoutDocument> floatingLayout, string file)
        {
            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms, new JsonWriterOptions() { Indented = true });

            writer.WriteStartObject();

            writer.WritePropertyName("Targets");
            TargetManagerDataModel.SAVE_TO_JSON(writer, targets);

            writer.WriteNumber("Navigation", navigationWidth);

            writer.WritePropertyName("Docking");
            navigation.ExportDockingToolboxToJSON(writer, dockingLayout);

            writer.WritePropertyName("Floating");
            navigation.ExportFloatingToolboxToJSON(writer, floatingLayout);

            writer.WriteEndObject();
            writer.Flush();

            using var fs = new FileStream(file, FileMode.Create, FileAccess.Write);
            ms.Seek(0, SeekOrigin.Begin);
            ms.CopyTo(fs);
            fs.Flush();

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static void RESTORE_FROM_JSON(string file, out TargetManagerDataModel targets, ToolsNavigationDataModel<LayoutDocument> navigation, out double navigationWidth, out ToolLayout dockingLayout, out List<LayoutDocument> floatingLayout)
        {
            targets = null;
            navigationWidth = 0;
            dockingLayout = null;
            floatingLayout = null;
            ReadOnlySpan<byte> fs = File.ReadAllBytes(file);
            if (fs.StartsWith(__utf8_bom)) fs = fs.Slice(__utf8_bom.Length);

            var reader = new Utf8JsonReader(fs, new JsonReaderOptions() { CommentHandling = JsonCommentHandling.Skip });
            try
            {
                while (reader.Read())
                {
                    switch (reader.TokenType, reader.CurrentDepth)
                    {
                        case (JsonTokenType.PropertyName, 1):
                            switch(reader.GetString())
                            {
                                case "Targets":
                                    TargetManagerDataModel.RESTORE_FROM_JSON(ref reader, out targets);break;
                                case "Navigation":
                                    reader.Read();
                                    navigationWidth =  reader.GetDouble();
                                    break;
                                case "Docking":
                                    dockingLayout = navigation.ImportDockingToolboxFromJSON(ref reader, fs);break;
                                case "Floating":
                                    floatingLayout = navigation.ImportFloatingToolboxFromJSON(ref reader, fs);break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                targets = null;
                dockingLayout = null;
                floatingLayout = null;
                throw;
            }
        }
    }
}
