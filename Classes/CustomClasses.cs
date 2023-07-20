
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using MyMeta;
using System.Collections.ObjectModel;
/// <summary>
/// 
/// </summary>
/// <remarks></remarks>
/// 
[Serializable()]
public class GenerationItem
{
    public string File;
    public string ParentPlugin;
}

/// <summary>
/// 
/// </summary>
/// <remarks></remarks>
/// 
[Serializable(), System.Xml.Serialization.XmlInclude(typeof(GenerationItem))]
public class GenerationProject
{
    private string _name;
    private string _location = string.Empty;
    private string _datasourceServer;
    private string _selectedDatabase;
    private string _description;
    private ArrayList _itemCollection = new ArrayList();
    private List<string> _selectedTemplates = new List<string>();
    private List<string> _selectedTables = new List<string>();
    private List<string> _selectedStoredProcedures = new List<string>();
    private List<string> _unSelectedStoredProcedures = new List<string>();
    private ObservableCollection<MappedStoredProcedure> _mappedStoredProcedures = new ObservableCollection<MappedStoredProcedure>();


    private List<string> _selectedViews = new List<string>();
    private String _connectionString = String.Empty;
    private bool _showSystemData = false;
    private bool _buildRelations = true;
    private String _namespace = String.Empty;
    private bool _disableNamespaceSchema = false;
    private bool _multiQuery = true;
    private bool _useWithNolock = false;
    private bool _includeGenerationInformation = false;
    private bool _serializeEntitiesClases = true;
    private String _childrenPrefix = String.Empty;
    private String _entityDisplayName = String.Empty;
    private String _connectionStringName = String.Empty;
    private String _supportedCharacters = String.Empty;
    private String _entity_Namimg_Enclosing_Left = String.Empty;
    private String _entity_Namimg_Enclosing_Right = String.Empty;
    private bool _forceNullableDatetime = false;
    private bool _overrideEntityDisplayName = false;
    private bool _forceDatetimeToDatetimeNow = false;
    private bool _deletePreviousGeneratedFiles = false;

    private MyMeta.dbDriver _dbDriver = dbDriver.SQL;

    public enum NetVersionEnum
    {
        Net40,
        Net45,
        NetCore
    }
    [Description(".NET Frameworks generation option.")]
    [Category(".NET Framework option")]
    [DisplayName(".NET Version")]
    public NetVersionEnum NetVersion { get; set; }

    public GenerationProject()
    {
        _mappedStoredProcedures.CollectionChanged += _mappedStoredProcedures_CollectionChanged;
    }

    void _mappedStoredProcedures_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        System.Diagnostics.Debug.Print("");
    }

    private string _garbageCollectionCode = string.Empty;

    [Description("If Explicit garbage collection is true,this C# code is added.")]
    [Category("Garbage Collection")]
    [DisplayName("Garbage Collection explicit call")]
    [TypeConverter(typeof(StringListStringConverter))]
    public string GarbageCollectionCode
    {
        get { return _garbageCollectionCode; }
        set { _garbageCollectionCode = value; }
    }

    private bool _usesExplicitGarbageCollection = false;

    [Description("Uses explicit garbage collection.")]
    [Category("Garbage Collection")]
    [DisplayName("Uses explicit garbage collection.")]
    public bool UsesExplicitGarbageCollection
    {
        get { return _usesExplicitGarbageCollection; }
        set { _usesExplicitGarbageCollection = value; }
    }

    [Description("Characters that will be replaced to fit C# Naming convention.")]
    [Category("Character Support")]
    [DisplayName("Supported Characters")] 
    public String SupportedCharacters
    {
        get { return _supportedCharacters; }
        set { _supportedCharacters = value; }
    }
    
    [Description("ConnectionString Name that will be used in configuration file.")]
    [Category("Configuration")]
    [DisplayName("ConnectionString Name")] 
    public String ConnectionStringName
    {
        get { return _connectionStringName; }
        set { _connectionStringName = value; }
    }
    [Description("Prefix that will be used in Children Relations classes.")]
    [Category("Generation Options")]
    [DisplayName("Children Prefix")] 
    public String ChildrenPrefix
    {
        get { return _childrenPrefix; }
        set { _childrenPrefix = value; }
    }
    [Description("Entity attribute name to use as display field.")]
    [Category("Generation Options")]
    [DisplayName("Entity to display attribute name")]
    public String EntityDisplayName
    {
        get { return _entityDisplayName; }
        set { _entityDisplayName = value; }
    }

    [Description("Disables Namespace based on database schemas.")]
    [Category("Generation Options")]
    [DisplayName("Disable Namespace Schema")]
    public bool DisableNamespaceSchema
    {
        get { return _disableNamespaceSchema; }
        set { _disableNamespaceSchema = value; }
    }

    [Description("Allows Multiple Query senteces execution (Mainly for retrieving new Id´s).")]
    [Category("Generation Options")]
    [DisplayName("Multiple Query")]
    public bool MultiQuery
    {
        get { return _multiQuery; }
        set { _multiQuery = value; }
    }

    [Description("Indicates if 'WITH (NOLOCK)' Option is required.")]
    [Category("Generation Options")]
    [DisplayName("Use WITH (NOLOCK)")]
    public bool UseWithNolock
    {
        get { return _useWithNolock; }
        set { _useWithNolock = value; }
    }

    [Description("Generates ORM.NET Version, User & Workstation information.")]
    [Category("Generation Options")]
    [DisplayName("Include Generation information")]
    public bool IncludeGenerationInformation
    {
        get { return _includeGenerationInformation; }
        set { _includeGenerationInformation = value; }
    }

    [Description("Serialize Entities clases.")]
    [Category("Generation Options")]
    [DisplayName("Serialize Entities clases")]
    public bool SerializeEntitiesClases
    {
        get { return _serializeEntitiesClases; }
        set { _serializeEntitiesClases = value; }
    }
    [Description("Global Namespace for Library.")]
    [Category("Generation Options")]
    [DisplayName("Library Namespace")] 

    public String Namespace
    {
        get { return _namespace; }
        set { _namespace = value; }
    }

    [Description("Shows Database System Data.")]
    [Category("Generation Options")]
    [DisplayName("Show System Data")] 
    public bool ShowSystemData
    {
        get { return _showSystemData; }
        set { _showSystemData = value; }
    }

    [Description("Build Relations Namespace (Only Optimized for Surrogate Key Pattern).")]
    [Category("Generation Options")]
    [DisplayName("Build Relations Namespace")]
    public bool BuildRelations
    {
        get { return _buildRelations; }
        set { _buildRelations = value; }
    }
    
    [Description("Forces Datetime Type to Nullable  (As required by MudBlazor components such as MudTable)")]
    [Category("Generation Options")]
    [DisplayName("Force Datetime Type to Nullable")]
    public bool ForceNullableDatetime
    {
        get { return _forceNullableDatetime; }
        set { _forceNullableDatetime = value; }
    }

    [Description("Overrides EntityDisplayName (ToString() and GetHashCode() methods)")]
    [Category("Generation Options")]
    [DisplayName("Override EntityDisplayName ")]
    public bool OverrideEntityDisplayName
    {
        get { return _overrideEntityDisplayName; }
        set { _overrideEntityDisplayName = value; }
    }

    [Description("Forces Datetime Type to DateTime.Now for avoiding out of range exception")]
    [Category("Generation Options")]
    [DisplayName("Force Datetime Type to DateTime.Now")]
    public bool ForceDatetimeToDatetimeNow
    {
        get { return _forceDatetimeToDatetimeNow; }
        set { _forceDatetimeToDatetimeNow = value; }
    }


    [Description("Deletes previously generated files in target directory, prior to current generation.")]
    [Category("Generation Options")]
    [DisplayName("Delete previous generated files")]
    public bool DeletePreviousGeneratedFiles
    {
        get { return _deletePreviousGeneratedFiles; }
        set { _deletePreviousGeneratedFiles = value; }
    }
    
    [Description("MyMeta driver list.")]
    [Category("Generation Options")]
    [DisplayName("DbDriver")] 
    public MyMeta.dbDriver DbDriver
    {
        get { return _dbDriver; }
        set { _dbDriver = value; }
    }
    //[Description("Manages ConnectionString for this project.")]
    [Category("Generation Options")]
    [DisplayName("ConnectionString")] 
    [System.ComponentModel.Editor(typeof(ConnectionStringUITypeEditor), typeof(System.Drawing.Design.UITypeEditor)), System.ComponentModel.Description("Gets or Sets then Connection String for Data Source")]
    public String ConnectionString
    {
        get { return _connectionString; }
        set { _connectionString = value; }
    }

    [Description("Database Views selected for this project.")]
    [Category("Project database objects selection")]
    [DisplayName("Selected Views")] 
    public List<string> SelectedViews
    {
        get { return _selectedViews; }
        set { _selectedViews = value; }
    }

    [Description("Database Stored Procedures selected for this project.")]
    [Category("Project database objects selection")]
    [DisplayName("Selected Stored Procedures")]
    public List<string> SelectedStoredProcedures
    {
        get { return _selectedStoredProcedures; }
        set { _selectedStoredProcedures = value; }
    }

    [Description("Database Stored Procedures unselected for this project.")]
    [Category("Project database objects selection")]
    [DisplayName("Unselected Stored Procedures")]
    public List<string> UnselectedStoredProcedures
    {
        get { return _unSelectedStoredProcedures; }
        set { _unSelectedStoredProcedures = value; }
    }

    [Description("Database Stored Procedures mapped for this project.")]
    [Category("Project database mapped Stored Procedures selection")]
    [DisplayName("Mapped Stored Procedures")]
    [Editor(typeof(CustomCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public ObservableCollection<MappedStoredProcedure> MappedStoredProcedures
    {
        get { return _mappedStoredProcedures; }
        set { _mappedStoredProcedures = value; }
    }

    [Description("Database Tables selected for this project.")]
    [Category("Project database objects selection")]
    [DisplayName("Selected Tables")]
    public List<string> SelectedTables
    {
        get { return _selectedTables; }
        set { _selectedTables = value; }
    }

    [ReadOnly(true)]
    [Description("Database selected for this project.")]
    [Category("Project database selection")]
    [DisplayName("Selected Database")]
    public string SelectedDatabase
    {
        get { return _selectedDatabase; }
        set { _selectedDatabase = value; }
    }

    [ReadOnly(true)]
    [Description("Templates selected for this project.")]
    [Category("Project Template selection")]
    [DisplayName("Selected Templates")]
    public List<string> SelectedTemplates
    {
        get { return _selectedTemplates; }
        set { _selectedTemplates = value; }
    }

    [Description("Project Name.")]
    [Category("Generation Options")]
    [DisplayName("Project Name")] 
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    [ReadOnly(true)]
    [Description("Project Location.")]
    [Category("Generation Options")]
    [DisplayName("Project Location")]
    public string Location
    {
        get { return _location; }
        set
        {
            if (!value.EndsWith("\\"))
            {
                value += "\\";
            }
            _location = value;
        }
    }
    [Description("Datasource Server.")]
    [Category("Generation Options")]
    [DisplayName("Datasource Server")]
    public string DatasourceServer
    {
        get { return _datasourceServer; }
        set { _datasourceServer = value; }
    }
    [Description("Project Description.")]
    [Category("Generation Options")]
    [DisplayName("Project Description")]
    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    private List<TemplateConfigurationEntry> _templateConfigurationEntries;

    [Description("Template configuration entries used to specify individual configuration for plugins.")]
    [Category("Plugin Configuration")]
    [DisplayName("Template Configuration Entry.")]
    public List<TemplateConfigurationEntry> TemplateConfigurationEntries
    {
        get { return _templateConfigurationEntries; }
        set { _templateConfigurationEntries = value; }
    }

    bool _usesEncription  = false;
    [Description("Uses ecription for connectionstrings.")]
    [Category("Generation Options")]
    [DisplayName("Uses Encription")]
    public bool UsesEncription
    {
        get { return _usesEncription; }
        set { _usesEncription = value; }
    }

    bool _lazyLoadingValueHolderVisible = false;
    [Description("Shows lazy loading value holder variable (_[VARIABLE]) is visible.")]
    [Category("Generation Options")]
    [DisplayName("Lazy Loading Value Holder Visible")]
    public bool LazyLoadingValueHolderVisible
    {
        get { return _lazyLoadingValueHolderVisible; }
        set { _lazyLoadingValueHolderVisible = value; }
    }

}
/// <summary>
/// 
/// </summary>
/// <remarks></remarks>
public class MyMetaWrapper
{
    public void connect()
    {

        try
        {

            MyMeta.dbRoot dbroot = new MyMeta.dbRoot();
            IDbConnection db = default(IDbConnection);
            //db = dbroot.BuildConnection("Microsoft SQL Server", "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=CasepromPortal;Data Source=NOK\SQL2005")
            db = dbroot.BuildConnection("Microsoft SQL Server", "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Data Source=NOK\\SQL2005");

            dbroot.DbTarget = "SqlClient";
            db.Open();
            MessageBox.Show(dbroot.Databases.Count.ToString());


        }
        catch (Exception)
        {
            
        }
    }
}
[Serializable()]
[TypeConverter(typeof(ServerItemConverter)), DefaultProperty("Name"), Browsable(true)]
public class ServerItem
{
    private string _name;
    private string _connectionString;
    private bool _selected;
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
    [System.ComponentModel.Editor(typeof(ConnectionStringUITypeEditor), typeof(System.Drawing.Design.UITypeEditor)), System.ComponentModel.Description("Gets or Sets then Connection String for Data Source")]
    public string ConnectionString
    {
        get { return _connectionString; }
        set { _connectionString = value; }
    }
    public bool Selected
    {
        get { return _selected; }
        set { _selected = value; }
    }
}
public static class Bilder
{
    public static ServerItem Bild;
}

public class ServerItemConverter : ExpandableObjectConverter
{

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (object.ReferenceEquals(sourceType, typeof(string)))
        {
            return true;
        }
        return base.CanConvertFrom(context, sourceType);
    }
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        if (object.ReferenceEquals(destinationType, typeof(ServerItem)))
        {
            return true;
        }
        return base.CanConvertFrom(context, destinationType);
    }


    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (object.ReferenceEquals(destinationType, typeof(string)) && value is ServerItem)
        {
            //Esta función obtiene el objeto  
            //editado en el PropertyGrid del editor de colecciones

            //Lo almacenamos en una variable pública
            Bilder.Bild = (ServerItem)value;
            //Y escribimos en la línea del ListBox el pie de foto para distinguirlo
            return Bilder.Bild.Name;
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string)
        {
            //Esta función se ejecuta cuando seleccionamos un objeto en el ListBox
            //y lo muestra el PropertyGrid del editor de colecciones

            //Simplemente recuperamos el objeto 
            //que antes habíamos metido en la variable pública
            return new List<ServerItem>();
        }
        return base.ConvertFrom(context, culture, value);
    }

}


[Serializable(), System.Xml.Serialization.XmlInclude(typeof(ServerItem))]
public class ServersConfigurationSetting
{
    public event OnPropertyChangedEventHandler OnPropertyChanged;
    public delegate void OnPropertyChangedEventHandler(string PropertyName);
    private List<ServerItem> _servers = new List<ServerItem>();
    [Category("Data Sources"), Description("Datasource Collection"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
    public List<ServerItem> Servers
    {
        get { return _servers; }
        set
        {
            _servers = value;
            if (OnPropertyChanged != null)
            {
                OnPropertyChanged("Servers");
            }
        }
    }
}


[Serializable(), System.Xml.Serialization.XmlInclude(typeof(ServerItem))]
public class ConfigurationSetting
{
    private string _pluginDirectory;
    private List<ServerItem> _servers = new List<ServerItem>();
    private string _languageMappingFileName;
    private string _dbTargetMappingFileName;
    private bool _showSystemData;
    public event OnPropertyChangedEventHandler OnPropertyChanged;
    public delegate void OnPropertyChangedEventHandler(string PropertyName);
    //<Category("Culture"), Description("Application Culture")> _
    //Public Property Culture() As String
    //    Get
    //        Return _culture
    //    End Get
    //    Set(ByVal value As String)
    //        If value <> _culture Then
    //            _culture = value
    //            RaiseEvent OnPropertyChanged("Culture")
    //        End If
    //    End Set
    //End Property
    [Category("General"), Description("Indicates if System data will be shown")]
    public bool ShowSystemData
    {
        get { return _showSystemData; }
        set
        {
            if (value != _showSystemData)
            {
                _showSystemData = value;
                if (OnPropertyChanged != null)
                {
                    OnPropertyChanged("ShowSystemData");
                }
            }

        }
    }

    [Category("Mapping"), System.ComponentModel.Editor(typeof(OpenFileUITypeEditor), typeof(System.Drawing.Design.UITypeEditor)), System.ComponentModel.Description("Gets or Sets the Db Target Mapping FileName")]
    public string DbTargetMappingFileName
    {

        get { return _dbTargetMappingFileName; }
        set
        {
            if (value != _dbTargetMappingFileName)
            {
                _dbTargetMappingFileName = value;
                if (OnPropertyChanged != null)
                {
                    OnPropertyChanged("DbTargetMappingFileName");
                }
            }

        }
    }
    [Category("Mapping"), Description("Gets or Sets then Language Mapping FileName"), System.ComponentModel.Editor(typeof(OpenFileUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public string LanguageMappingFileName
    {
        get { return _languageMappingFileName; }
        set
        {
            if (value != _languageMappingFileName)
            {
                _languageMappingFileName = value;
                if (OnPropertyChanged != null)
                {
                    OnPropertyChanged("LanguageMappingFileName");
                }
            }

        }
    }
    [Category("Data Sources"), Description("Datasource Collection"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
    public List<ServerItem> Servers
    {
        get { return _servers; }
        set
        {
            _servers = value;
            if (OnPropertyChanged != null)
            {
                OnPropertyChanged("Servers");
            }
        }
    }

    //<System.ComponentModel.Category("Workspace"), System.ComponentModel.Description("Gets or Sets the generator working directory"), _
    //System.ComponentModel.Editor(GetType(OpenFolderUITypeEditor), GetType(System.Drawing.Design.UITypeEditor))> _
    //Public Property WorkingDirectory() As String
    //    Get
    //        Return _workingDirectory
    //    End Get
    //    Set(ByVal value As String)
    //        If value <> _workingDirectory Then
    //            _workingDirectory = value
    //            RaiseEvent OnPropertyChanged("WorkingDirectory")
    //        End If

    //    End Set
    //End Property
    [System.ComponentModel.Category("Plugin"), System.ComponentModel.Description("Gets or Sets the Plugin resource Location"), System.ComponentModel.Editor(typeof(OpenFolderUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public string PluginDirectory
    {
        get { return _pluginDirectory; }
        set
        {
            if (value != _pluginDirectory)
            {
                _pluginDirectory = value;
                if (OnPropertyChanged != null)
                {
                    OnPropertyChanged("PluginDirectory");
                }
            }

        }
    }
    //friend property Location a
}

//****************************************************************
public class PasswordUITypeEditor : System.Drawing.Design.UITypeEditor
{
    private TextBox _textbox = new TextBox();
    public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
    {
        //Este procedimiento llama al cuadro de diálogo
        //OpenFileDialog y devuelve la ruta del archivo seleccionado
        //respetando siempre el tipo String de la propiedad
        //Dim pwdEditor As TextBox = New TextBox()
        var _with1 = _textbox;
        _with1.PasswordChar = Convert.ToChar("*");
        return null;
    }

    public new object GetEditStyle()
    {
        return _textbox;
    }

    public virtual bool GetPaintValueSupported()
    {
        return true;
    }
}
//****************************************************************

public class OpenFileUITypeEditor : System.Drawing.Design.UITypeEditor
{

    public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
    {
        //Este procedimiento llama al cuadro de diálogo
        //OpenFileDialog y devuelve la ruta del archivo seleccionado
        //respetando siempre el tipo String de la propiedad
        System.Windows.Forms.OpenFileDialog openf = new System.Windows.Forms.OpenFileDialog();
        var _with2 = openf;
        _with2.InitialDirectory = value.ToString();
        _with2.Filter = "Configuration files(*.xml,*.config)|*.xml;*.congig|All Files(*.*)|*.*";
        _with2.ShowReadOnly = false;
        _with2.CheckFileExists = true;
        System.Windows.Forms.DialogResult r = openf.ShowDialog();
        switch (r)
        {
            case DialogResult.OK:
                return openf.FileName;
            case DialogResult.Cancel:
                return value;
        }
        return null;

    }

    public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
    {
        return System.Drawing.Design.UITypeEditorEditStyle.Modal;
    }

    public virtual bool GetPaintValueSupported()
    {
        return true;
    }
}

public class OpenFolderUITypeEditor : System.Drawing.Design.UITypeEditor
{

    private ConfigurationSetting _config = ConfigurationSettingSerializer.Load(null);
    public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
    {
        //Este procedimiento llama al cuadro de diálogo
        //OpenFileDialog y devuelve la ruta del archivo seleccionado
        //respetando siempre el tipo String de la propiedad

        try
        {
            System.Windows.Forms.FolderBrowserDialog openf = new System.Windows.Forms.FolderBrowserDialog();
            var _with3 = openf;
            _with3.ShowNewFolderButton = true;
            //If _config.WorkingDirectory = Nothing Then
            _with3.RootFolder = Environment.SpecialFolder.Desktop;
            //Else
            //    .RootFolder = system.IAsyncR _config.WorkingDirectory
            //End If

            System.Windows.Forms.DialogResult r = openf.ShowDialog();
            //GenerationProjectSerializer.CurrentProject = MainForm.GenerationProjectSerializer.CurrentProject
            GenerationProject _projectConfig = GenerationProjectSerializer.Load();
            if (openf.SelectedPath != null)
            {
                if (openf.SelectedPath.EndsWith("\\"))
                {
                    _projectConfig.Location = openf.SelectedPath;
                }
                else
                {
                    _projectConfig.Location = openf.SelectedPath + "\\";
                }

            }
            switch (r)
            {
                case DialogResult.OK:
                    return openf.SelectedPath;
                case DialogResult.Cancel:
                    return value;
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
    {
        return System.Drawing.Design.UITypeEditorEditStyle.Modal;
    }

    public virtual bool GetPaintValueSupported()
    {
        return true;
    }
}



/// <summary>
/// A type editor for db connections
/// </summary>
public class ConnectionStringUITypeEditor : System.Drawing.Design.UITypeEditor
{

    /// <summary>
    /// constructor
    /// </summary>
    public ConnectionStringUITypeEditor()
    {
    }

    /// <summary>
    /// display a modal form 
    /// </summary>
    /// <param name="context">
    /// see documentation on ITypeDescriptorContext
    /// </param>
    /// <returns>
    /// the style of the editor
    /// </returns>
    public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
    {

        return System.Drawing.Design.UITypeEditorEditStyle.Modal;
    }

    /// <summary>
    /// used to show the connection
    /// </summary>
    /// <param name="context">
    /// see documentation on ITypeDescriptorContext
    /// </param>
    /// <param name="provider">
    /// see documentation on IServiceProvider
    /// </param>
    /// <param name="value">
    /// the value prior to editing
    /// </param>
    /// <returns>
    /// the new connection string after editing
    /// </returns>
    public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
    {
        return this.EditValue(value as string);
    }

    /// <summary>
    /// show the form for the new connection string
    /// </summary>
    /// <returns>
    /// the new connection string after editing
    /// </returns>
    public string EditValue()
    {
        return this.EditValue(string.Empty);
    }

    /// <summary>
    /// show the form for the new connection 
    /// string based on an an existing one
    /// </summary>
    /// <param name="value">
    /// the value prior to editing
    /// </param>
    /// <returns>
    /// the new connection string after editing
    /// </returns>
    public string EditValue(string value)
    {

        string newValue = string.Empty;

        ConnectionString connectionString = new ConnectionString();

        return connectionString.ShowDialog(value);
    }

}


public class CustomCollectionEditor : System.ComponentModel.Design.CollectionEditor
{
    public delegate void MyFormClosedEventHandler(object sender,
                                        FormClosedEventArgs e);

    public static event MyFormClosedEventHandler FormClosed;

    public CustomCollectionEditor(Type type) : base(type) { }
    protected override CollectionForm CreateCollectionForm()
    {
        CollectionForm collectionForm = base.CreateCollectionForm();
        collectionForm.FormClosed += new FormClosedEventHandler(collection_FormClosed);
        return collectionForm;
    }

    void collection_FormClosed(object sender, FormClosedEventArgs e)
    {
        if (FormClosed != null)
        {
            FormClosed(this, e);
        }
    }
}

/// 
/// contains the application entry point
/// 
public class MainClass
{

    /// 
    /// not to be instantiated
    /// 
    private MainClass()
    {
        // no initialization
    }

    /// 
    /// entry point for the application
    /// 
    /// contains demo code to obtain a connection
    /// string from a ConnectionString instance
    /// 
    [System.STAThread()]
    private static void Main(string[] args)
    {
        TestConnectionString();
    }

    /// 
    /// test / demo code for ConnectionString
    /// 

    public static void TestConnectionString()
    {
        System.Console.WriteLine("creating new connection string");

        ConnectionString connectionString = new ConnectionString();

        string newConnectionString = connectionString.ShowDialog();

        System.Console.WriteLine(newConnectionString);
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.WriteLine("press enter to continue");
        System.Console.ReadLine();
        System.Console.WriteLine();
        System.Console.WriteLine("editing previous connection string");

        string editedConnectionString = connectionString.ShowDialog(newConnectionString);

        System.Console.WriteLine(editedConnectionString);
        System.Console.WriteLine();
        System.Console.WriteLine("press enter to close window");
        System.Console.ReadLine();
    }
}

/// 
/// utility class to build the connection string 
/// 
///  
/// required references: //* NOTE 01
///  [COM] oledb32.dll
///  [NET] adodb (interop assembly)
/// 
public class ConnectionString
{

    /// 
    /// constructor
    /// 
    public ConnectionString()
    {
        // no initialization
    }

    /// 
    /// use if there is no existing connection string 
    /// 
    /// 
    /// an oledb connection string suitable for .Net
    /// or a zero length string on exception 
    ///  //* NOTE 02
    public string ShowDialog()
    {
        return ShowDialog(null);
    }

    /// 
    /// use if there is an existing connection string 
    /// 
    /// "connectionString">
    /// the existing oledb connection string 
    /// zero length or null imply no existing connection string
    /// 
    /// 
    /// an oledb connection string suitable for .Net
    /// string.Empty on exception
    ///  //* NOTE 02
    public string ShowDialog(string connectionString)
    {

        string retVal = string.Empty;

        try
        {
            ADODB.Connection connection = null;
            MSDASC.DataLinks links = new MSDASC.DataLinksClass();

            if ((connectionString == null) || (connectionString.Length == 0))
            {
                object connectionObject = links.PromptNew();
                connection = (ADODB.Connection)connectionObject;
                retVal = connection.ConnectionString;
            }
            else
            {
                connection = new ADODB.ConnectionClass();
                connection.ConnectionString = connectionString;

                // required for ref object argument
                object connectionObject = connection;
                bool success = links.PromptEdit(connectionObject);
                if (success)
                {
                    connection = (ADODB.Connection)connectionObject;
                    retVal = connection.ConnectionString;
                }
                else
                {
                    retVal = connectionString;
                }
            }
        }
        catch
        {
            retVal = string.Empty;
        }
        return retVal;
    }
}


public class EventObject
{
    public enum EventTypeEnum
    {
        InfoEvent = 0,
        ErrorEvent = 1,
        WarningEvent = 2
    }
    private string _eventText;
    private EventTypeEnum _eventType;
    public EventObject(string EventText, EventTypeEnum EventType)
    {
        _eventText = EventText;
        _eventType = EventType;
    }
    public string EventText
    {
        get { return _eventText; }
        set { _eventText = value; }
    }
    public EventTypeEnum EventType
    {
        get { return _eventType; }
        set { _eventType = value; }
    }
}


public class SelectedProceduresConverter : StringConverter
{
    public override bool GetStandardValuesSupported(
                           ITypeDescriptorContext context)
    {
        return true;
    }
    public override StandardValuesCollection
                      GetStandardValues(ITypeDescriptorContext context)
    {
        return new StandardValuesCollection(OpenORM.UI.Classes.Global.CurrentProject.UnselectedStoredProcedures.ToArray());
    } 
}

[Serializable]
public class TemplateConfigurationEntry
{
    
    public TemplateConfigurationEntry()
    { }
    public TemplateConfigurationEntry(string template,string name,string value)
    {
        this.Template = template;
        this.Name = name;
        this.Value = value;
    }

    public String Template { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }


}


public class StringListStringConverter : StringConverter
{
    public override Boolean GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
    public override Boolean GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }
    public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        List<String> list = new List<String>();
        list.Add("GC.SuppressFinalize(this);");

        list.Add("GC.Collect(0, GCCollectionMode.Default);");
        list.Add("GC.Collect(0, GCCollectionMode.Optimized);");
        list.Add("GC.Collect(0, GCCollectionMode.Forced);");

        list.Add("GC.Collect(1, GCCollectionMode.Default);");
        list.Add("GC.Collect(1, GCCollectionMode.Optimized);");
        list.Add("GC.Collect(1, GCCollectionMode.Forced);");

        list.Add("GC.Collect(2, GCCollectionMode.Default);");
        list.Add("GC.Collect(2, GCCollectionMode.Optimized);");
        list.Add("GC.Collect(2, GCCollectionMode.Forced);");

        return new StandardValuesCollection(list);
    }
    
}