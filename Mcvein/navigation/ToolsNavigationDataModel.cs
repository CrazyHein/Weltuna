using AMEC.PCSoftware.CommunicationProtocol.CrazyHein.SLMP.Master;
using AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool;
using HandyControl.Controls;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xceed.Wpf.AvalonDock.Layout;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks
{
    public enum TOOL_LAYOUT_ORIENTATION_T : byte
    {
        HORIZONTAL = 0,
        VERTICAL = 1
    }

    public enum TOOL_LAYOUT_TYPE_T : byte
    {
        PANEL = 0,
        GROUP = 1
    }

    public class LayoutPanel<T> : ToolLayout where T : new()
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public List<T> Documents { get; set; }

        public LayoutPanel()
        {
            Type = TOOL_LAYOUT_TYPE_T.PANEL;
        }
    }

    public class LayoutGroup : ToolLayout
    {
        public TOOL_LAYOUT_ORIENTATION_T Orientation { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public List<ToolLayout> SubLayout { get; set; }

        public LayoutGroup()
        {
            Type = TOOL_LAYOUT_TYPE_T.GROUP;
        }

    }

    public abstract class ToolLayout
    {
        //public LayoutPanel<T> Panel { get; set; }
        //public LayoutGroup<T> Group { get; set; }

        //public TOOL_LAYOUT_TYPE_T Type => Panel != null ? TOOL_LAYOUT_TYPE_T.PANEL : TOOL_LAYOUT_TYPE_T.GROUP;
        public TOOL_LAYOUT_TYPE_T Type { get; protected set; }
    }

    public class ToolsNavigationDataModel<T> where T : new()
    {
        private Dictionary<string, CabinetShortcut> __cabinet_collection = new Dictionary<string, CabinetShortcut>();
        private PropertyChangedEventHandler __property_changed_event_handler = null;
        private ConcurrentDictionary<T, System.Windows.Controls.UserControl> __toolbox_collection = new ConcurrentDictionary<T, System.Windows.Controls.UserControl>();
        private ConcurrentDictionary<T, DataModel> __tooldata_collection = new ConcurrentDictionary<T, DataModel>();

        public ToolsNavigationDataModel(string toolkitLocation = null, string pattern = "Tool.*.dll", PropertyChangedEventHandler propertyChanged = null)
        {
            toolkitLocation ??= Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\toolkit";
            pattern ??= "Tool.*.dll";
            __property_changed_event_handler = propertyChanged;
            DirectoryInfo toolkitDiretory = new DirectoryInfo(toolkitLocation);
            if (toolkitDiretory.Exists)
            {
                foreach (DirectoryInfo cabinetDirectory in toolkitDiretory.EnumerateDirectories())
                {
                    foreach (FileInfo cabinetFile in cabinetDirectory.GetFiles(pattern, SearchOption.TopDirectoryOnly))
                    {
                        var alc = new CabinetLoadContext(cabinetFile.FullName, true);
                        Type cabinetType = null;
                        BitmapSource bs = null;
                        ICabinet cabinet = null;
                        try
                        {
                            Assembly assem = alc.LoadFromAssemblyPath(cabinetFile.FullName);
                            cabinetType = assem.ExportedTypes.Single(t => typeof(ICabinet).IsAssignableFrom(t));
                            string resourceName = assem.GetName().Name + ".g";
                            ResourceManager rm = new ResourceManager(resourceName, assem);
                            using (ResourceSet set = rm.GetResourceSet(CultureInfo.CurrentCulture, true, true))
                            {
                                if (set != null)
                                {
                                    UnmanagedMemoryStream s = (UnmanagedMemoryStream)set.GetObject("icon.png", false);
                                    if (s != null)
                                    {
                                        bs = BitmapFrame.Create(s, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                                    }
                                }
                            }
                            if (bs == null)
                                bs = BitmapFrame.Create(new Uri("pack://application:,,,/../imgs/plugin.png"), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                            cabinet = (ICabinet)Activator.CreateInstance(cabinetType);
                            if (__cabinet_collection.ContainsKey(assem.FullName) == true)
                                //if (__cabinet_collection.Keys.Contains(cabinetFile.FullName) == true)
                                throw new ArgumentException($"The plug-in with the name <{assem.FullName}> already exists.");

                            __cabinet_collection[assem.FullName] = new CabinetShortcut()
                            //__cabinet_collection[cabinetFile.FullName] = new CabinetShortcut()
                            {
                                Icon = bs,
                                Path = cabinetFile.FullName,
                                Cabinet = cabinet
                            };
                            break;
                        }
                        catch
                        {
                            alc.Unload();
                            throw;
                        }
                    }
                }
            }
        }
    
        public IEnumerable<CabinetShortcut> CabinetShortcuts { get { return __cabinet_collection.Select((cabinet) => cabinet.Value); } }
        public IEnumerable<DataModel> ToolDataCollection { get { return __tooldata_collection.Select((data) => data.Value); } }

        public void TryAddToolbox(T key, System.Windows.Controls.UserControl data)
        {
            if (data != null && data.DataContext is DataModel == false)
                throw new ArgumentException($"The plug-in ({data.GetType().Assembly.FullName}) is not compatible with this framework");
            __toolbox_collection.TryAdd(key, data);
            __tooldata_collection.TryAdd(key, data.DataContext as DataModel);
        }

        public void TryRemoveToolbox(T key)
        {
            bool res = __toolbox_collection.TryRemove(key, out System.Windows.Controls.UserControl data);
            if (res == true)
                (data.DataContext as DataModel).Dispose();
            __tooldata_collection.TryRemove(key, out _);
        }

        public void ClearToolbox()
        {
            foreach (var key in __toolbox_collection.Keys)
            {
                __toolbox_collection.TryRemove(key, out System.Windows.Controls.UserControl ctrl);
                __tooldata_collection.TryRemove(key, out _);
                (ctrl.DataContext as DataModel).Dispose();     
            }
        }

        public void ClearToolbox(Action<T> action)
        {
            foreach(var key in __toolbox_collection.Keys)
            {
                __toolbox_collection.TryRemove(key, out System.Windows.Controls.UserControl ctrl);
                __tooldata_collection.TryRemove(key, out _);
                action(key);
                (ctrl.DataContext as DataModel).Dispose();
            }
        }

        public (CabinetShortcut, System.Windows.Controls.UserControl) this[T key]
        {
            get
            {
                System.Windows.Controls.UserControl data;
                CabinetShortcut cabinet;
                try
                {
                    data = __toolbox_collection[key];
                }
                catch(KeyNotFoundException ex)
                {
                    throw new KeyNotFoundException($"Can not find the specified plug-in instance associated with key : {key}.", ex);
                }
                try
                {
                    cabinet = __cabinet_collection[data.GetType().Assembly.FullName];
                }
                catch (KeyNotFoundException ex)
                {
                    throw new KeyNotFoundException($"Can not find the specified plug-in associated with key : {data.GetType().Assembly.FullName}.", ex);
                }
                return (cabinet, data);
            }
        }

        private void __export_docking_toolbox_to_json_group(Utf8JsonWriter writer, IReadOnlyList<ToolLayout> group)
        {
            writer.WriteStartArray();
            foreach(var sub in group)
            {
                if (sub.Type == TOOL_LAYOUT_TYPE_T.PANEL)
                {
                    var layoutpanel = sub as LayoutPanel<T>;
                    writer.WriteStartObject();
                    writer.WritePropertyName("Panel*" +
                        layoutpanel.Height.ToString("F4", CultureInfo.InvariantCulture) 
                        + "*" + layoutpanel.Width.ToString("F4", CultureInfo.InvariantCulture));
                    __export_docking_toolbox_to_json_panel(writer, layoutpanel.Documents);
                    writer.WriteEndObject();
                }
                else if (sub.Type == TOOL_LAYOUT_TYPE_T.GROUP)
                {
                    var layoutgroup = sub as LayoutGroup;
                    writer.WriteStartObject();
                    switch(layoutgroup.Orientation)
                    {
                        case TOOL_LAYOUT_ORIENTATION_T.HORIZONTAL:
                            writer.WritePropertyName("HGroup*" +
                                layoutgroup.Height.ToString("F4", CultureInfo.InvariantCulture)
                                + "*" + layoutgroup.Width.ToString("F4", CultureInfo.InvariantCulture));
                            break;
                        case TOOL_LAYOUT_ORIENTATION_T.VERTICAL:
                            writer.WritePropertyName("VGroup*" +
                                layoutgroup.Height.ToString("F4", CultureInfo.InvariantCulture)
                                + "*" + layoutgroup.Width.ToString("F4", CultureInfo.InvariantCulture));
                            break;
                        default:
                            throw new ArgumentException("The given value of <ToolLayout.Orientation> property is invalid.");
                    }    
                    __export_docking_toolbox_to_json_group(writer, layoutgroup.SubLayout);
                    writer.WriteEndObject();
                }
                else
                    throw new ArgumentException("<ToolLayout> object is invalid.");
            }
            writer.WriteEndArray();
        }

        private void __export_docking_toolbox_to_json_panel(Utf8JsonWriter writer, IReadOnlyList<T> panel)
        {
            writer.WriteStartArray();
            foreach(var key in panel)
            {
                writer.WriteStartObject();
                (var _, var data) = this[key];
                writer.WritePropertyName(data.GetType().Assembly.FullName);
                if((data.DataContext as DataModel).Save(writer) == 0)
                {
                    writer.WriteStartObject();
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        public void ExportDockingToolboxToJSON(Utf8JsonWriter writer, ToolLayout layout)
        {
            writer.WriteStartObject();
            //if(layout.Panel != null)
            //{
            // writer.WritePropertyName("Panel");
            //__export_docking_toolbox_to_json_panel(writer, layout.Panel);
            //}
            //else if(layout.Group != null)
            if (layout.Type == TOOL_LAYOUT_TYPE_T.GROUP)
            {
                var layoutgroup = layout as LayoutGroup;
                switch (layoutgroup.Orientation)
                {
                    case TOOL_LAYOUT_ORIENTATION_T.HORIZONTAL:
                        writer.WritePropertyName("HGroup*" +
                            layoutgroup.Height.ToString("F4", CultureInfo.InvariantCulture)
                            + "*" + layoutgroup.Width.ToString("F4", CultureInfo.InvariantCulture));
                        break;
                    case TOOL_LAYOUT_ORIENTATION_T.VERTICAL:
                        writer.WritePropertyName("VGroup*" +
                            layoutgroup.Height.ToString("F4", CultureInfo.InvariantCulture)
                            + "*" + layoutgroup.Width.ToString("F4", CultureInfo.InvariantCulture));
                        break;
                    default:
                        throw new ArgumentException("The given value of <ToolLayout.Orientation> property is invalid.");
                }
                __export_docking_toolbox_to_json_group(writer, layoutgroup.SubLayout);
            }
            else
                throw new ArgumentException("<ToolLayout> object is invalid.");
            writer.WriteEndObject();
        }

        public void ExportFloatingToolboxToJSON(Utf8JsonWriter writer, IReadOnlyList<T> layout)
        {
            writer.WriteStartArray();
            foreach (var key in layout)
            {
                writer.WriteStartObject();
                (var _, var data) = this[key];
                writer.WritePropertyName(data.GetType().Assembly.FullName);
                if ((data.DataContext as DataModel).Save(writer) == 0)
                {
                    writer.WriteStartObject();
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        private List<T> __import_docking_toolbox_from_json_panel(ref Utf8JsonReader reader, ReadOnlySpan<byte> dataSpan)
        { 
            if(reader.Read() == false || reader.TokenType != JsonTokenType.StartArray)
                throw new ArgumentException("<Panel> property value of <ToolLayout> object in JSON is not a valid array.");
            List<T> list = new List<T>();
            while (reader.Read() && (reader.TokenType != JsonTokenType.EndArray))
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        if (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
                        {
                            string property = reader.GetString();
                            if (__cabinet_collection.ContainsKey(property) == false)
                                throw new KeyNotFoundException($"Can not find plug-in : {property}.");
                            var data = __cabinet_collection[property].Cabinet.CreateInstance(__property_changed_event_handler);
                            T key = new T();
                            TryAddToolbox(key, data as System.Windows.Controls.UserControl);

                            long start = reader.BytesConsumed;
                            reader.Skip();
                            long spanLength = reader.BytesConsumed - start;
                            var subreader = new Utf8JsonReader(dataSpan.Slice((int)start, (int)spanLength));

                            long bytesConsumed = ((data as System.Windows.Controls.UserControl).DataContext as DataModel).Restore(ref subreader);
                            if (bytesConsumed != 0 && bytesConsumed != spanLength)
                                throw new ArgumentException($"The instance of plug-in : {property} comsumes {bytesConsumed} bytes but the JSON object lenght is {spanLength} bytes.");
                            list.Add(key);
                        }
                        else
                            throw new ArgumentException($"There should be one valid property defined in <Document> object in JSON.");
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.EndObject)
                            throw new ArgumentException($"Only one valid property can be defined in <Document> object in JSON or the <Document> object in JSON is corrupted.");
                        break;
                }
            }
            return list;
        }

        private List<ToolLayout> __import_docking_toolbox_from_json_group(ref Utf8JsonReader reader, ReadOnlySpan<byte> dataSpan)
        {
            List<ToolLayout> layout = new List<ToolLayout>();
            if (reader.Read() == false || reader.TokenType != JsonTokenType.StartArray)
                throw new ArgumentException("<Group> property value of <ToolLayout> object in JSON is not a valid array.");
            LayoutGroup group;
            LayoutPanel<T> panel;
            while (reader.Read() && (reader.TokenType != JsonTokenType.EndArray))
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        if(reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
                        {
                            string property = reader.GetString();
                            int index0 = property.IndexOf('*');
                            int index1 = property.LastIndexOf('*');
                            if(index0 == -1 || index1 == -1)
                                throw new ArgumentException($"The property({property}) name of <ToolLayout> object in JSON is invalid.");
                            double height, width;
                            try
                            {
                                height = double.Parse(property.Substring(index0 + 1, index1 - index0 - 1));
                                width = double.Parse(property.Substring(index1 + 1, property.Length - index1 - 1));
                            }
                            catch
                            {
                                throw new ArgumentException($"The property({property}) name of <ToolLayout> object in JSON is invalid.");
                            }

                            if(property.StartsWith("HGroup"))
                            {
                                group = new LayoutGroup() { Orientation = TOOL_LAYOUT_ORIENTATION_T.HORIZONTAL, Height = height, Width = width, SubLayout = __import_docking_toolbox_from_json_group(ref reader, dataSpan) };
                                layout.Add(group);
                            }
                            else if (property.StartsWith("VGroup"))
                            {
                                group = new LayoutGroup() { Orientation = TOOL_LAYOUT_ORIENTATION_T.VERTICAL, Height = height, Width = width, SubLayout = __import_docking_toolbox_from_json_group(ref reader, dataSpan) };
                                layout.Add(group);
                            }
                            else if (property.StartsWith("Panel"))
                            {
                                panel = new LayoutPanel<T>() { Height = height, Width = width };
                                panel.Documents = __import_docking_toolbox_from_json_panel(ref reader, dataSpan);
                                layout.Add(panel);
                            }
                            else
                                throw new ArgumentException($"The property({property}) name of <ToolLayout> object in JSON is invalid.");
                        }
                        else
                            throw new ArgumentException($"There should be one valid property defined in <ToolLayout> object in JSON.");
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.EndObject)
                            throw new ArgumentException($"Only one valid property can be defined in <ToolLayout> object in JSON or the <ToolLayout> object in JSON is corrupted.");
                        break;
                }
            }
            return layout;
        }

        public ToolLayout ImportDockingToolboxFromJSON(ref Utf8JsonReader reader, ReadOnlySpan<byte> dataSpan)
        {
            ToolLayout layout;
            ClearToolbox();
            if (reader.Read() == false || reader.TokenType != JsonTokenType.StartObject)
                throw new ArgumentException("The given JSON is not a valid <ToolLayout> object.");

            int depth = reader.CurrentDepth;
            try
            {
                if(reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
                {
                    string property = reader.GetString();
                    int index0 = property.IndexOf('*');
                    int index1 = property.LastIndexOf('*');
                    if (index0 == -1 || index1 == -1)
                        throw new ArgumentException($"The property({property}) name of root <ToolLayout> object in JSON is invalid.");
                    layout = new LayoutGroup();
                    var layoutgroup = layout as LayoutGroup;
                    try
                    {
                        layoutgroup.Height = double.Parse(property.Substring(index0 + 1, index1 - index0 - 1));
                        layoutgroup.Width = double.Parse(property.Substring(index1 + 1, property.Length - index1 - 1));
                    }
                    catch
                    {
                        throw new ArgumentException($"The property({property}) name of the root <ToolLayout> object in JSON is invalid.");
                    }
                    if (property.StartsWith("HGroup"))
                        layoutgroup.Orientation = TOOL_LAYOUT_ORIENTATION_T.HORIZONTAL;
                    else if (property.StartsWith("VGroup"))
                        layoutgroup.Orientation = TOOL_LAYOUT_ORIENTATION_T.VERTICAL;
                    else
                        throw new ArgumentException($"The property({property}) name of the root <ToolLayout> object in JSON is invalid.");

                    layoutgroup.SubLayout = __import_docking_toolbox_from_json_group(ref reader, dataSpan);
                }
                else
                    throw new ArgumentException($"There should be one valid property defined in root <ToolLayout> object in JSON.");
                if (reader.Read() == false || reader.TokenType != JsonTokenType.EndObject)
                    throw new ArgumentException($"Only one valid property can be defined in root <ToolLayout> object in JSON or the root <ToolLayout> object in JSON is corrupted.");
            }
            catch
            {
                ClearToolbox();
                throw;
            }

            return layout;
        }

        public List<T> ImportFloatingToolboxFromJSON(ref Utf8JsonReader reader, ReadOnlySpan<byte> dataSpan)
        {
            if (reader.Read() == false || reader.TokenType != JsonTokenType.StartArray)
                throw new ArgumentException("The given JSON is not a valid array of floating windowss.");
            List<T> list = new List<T>();
            while (reader.Read() && (reader.TokenType != JsonTokenType.EndArray))
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        if (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
                        {
                            string property = reader.GetString();
                            if (__cabinet_collection.ContainsKey(property) == false)
                                throw new KeyNotFoundException($"Can not find plug-in : {property}.");
                            var data = __cabinet_collection[property].Cabinet.CreateInstance(__property_changed_event_handler);
                            T key = new T();
                            TryAddToolbox(key, data as System.Windows.Controls.UserControl);

                            long start = reader.BytesConsumed;
                            reader.Skip();
                            long spanLength = reader.BytesConsumed - start;
                            var subreader = new Utf8JsonReader(dataSpan.Slice((int)start, (int)spanLength));

                            long bytesConsumed = ((data as System.Windows.Controls.UserControl).DataContext as DataModel).Restore(ref subreader);
                            if (bytesConsumed != 0 && bytesConsumed != spanLength)
                                throw new ArgumentException($"The instance of plug-in : {property} comsumes {bytesConsumed} bytes but the JSON object lenght is {spanLength} bytes.");
                            list.Add(key);
                        }
                        else
                            throw new ArgumentException($"There should be one valid property defined in <Document> object in JSON.");
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.EndObject)
                            throw new ArgumentException($"Only one valid property can be defined in <Document> object in JSON or the <Document> object in JSON is corrupted.");
                        break;
                }
            }
            return list;
        }

    }
    public class CabinetShortcut
    {
        public BitmapSource Icon { get; init; }
        public string Path { get; init; }
        public ICabinet Cabinet { get; init; }
        public override string ToString()
        {
            return Cabinet.GetType().Assembly.FullName + "\n" + Path;
        }
    }

    class CabinetLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver __resolver;
        public CabinetLoadContext(string cabinetPath, bool isCollectible) : base(name: cabinetPath, isCollectible)
        {
            __resolver = new AssemblyDependencyResolver(cabinetPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (assemblyName.Name == typeof(ICabinet).Assembly.GetName().Name)
                return null; 
            if (assemblyName.Name == typeof(DeviceAccessMaster).Assembly.GetName().Name)
                return null;
            if (assemblyName.Name == typeof(LayoutDocument).Assembly.GetName().Name)
                return null;
            if (assemblyName.Name == typeof(Notification).Assembly.GetName().Name)
                return null;
            string target = __resolver.ResolveAssemblyToPath(assemblyName);
            if (target != null)
                return LoadFromAssemblyPath(target);
            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string path = __resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            return path == null ? IntPtr.Zero : LoadUnmanagedDllFromPath(path);
        }
    }
}
