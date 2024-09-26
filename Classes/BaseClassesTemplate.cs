using MyMeta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.Text;


public delegate void OnPercentDoneEventHandler(double Percent);
public delegate void OnExceptionEventHandler(System.Exception Iex);
public delegate void OnInfoEventHandler(string Description);
public delegate void OnFileGeneratedEventHandler(string File);
public delegate void OnPropertyChangedEventHandler(string PropertyChanged);


public class BaseClassesPlugin : IPlugin
{
    private List<ITemplate> _templates = new List<ITemplate>();
    const string PluginName = "Database Abstraction Classes Plugin (5G)";
    const string PluginDescription = "Database Abstraction Classes Generator Plugin (5G)";


    public BaseClassesPlugin()
    {
        _templates.Add(new BusinessLogicLayerTemplate_5G_CSHARP());
        _templates.Add(new WebFormsWebGridCrudTemplate_5G_CSHARP());
    }

    string IPlugin.Description
    {
        get { return PluginName; }
    }

    string IPlugin.Name
    {
        get { return PluginDescription; }
    }

    System.Collections.Generic.List<ITemplate> IPlugin.Templates
    {
        get { return _templates; }
        set { _templates = value; }
    }

    public static string GetEntityName(MyMeta.ITable entity)
    {
        return entity.Name.Replace(" ", "_").Replace("$", "$_");

    }
    public static string GetEntityNameWithSchema(MyMeta.ITable entity)
    {
        return entity.Schema + "." + entity.Name.Replace(" ", "_").Replace("$", "$_");
    }

    public static string GetEntityName(MyMeta.IView entity)
    {
        return entity.Name.Replace(" ", "_").Replace("$", "$_");
    }
    public static string GetEntityNameWithSchema(MyMeta.IView entity)
    {
        return entity.Schema + "." + entity.Name.Replace(" ", "_").Replace("$", "$_");
    }

    public static string GetEntityName(MyMeta.IProcedure entity)
    {
        return entity.Name.Replace(" ", "_").Replace("$", "$_");
    }
    public static string GetEntityNameWithSchema(MyMeta.IProcedure entity)
    {
        return entity.Schema + "." + entity.Name.Replace(" ", "_").Replace("$", "$_");
    }
    public static string GetEntityName(MyMeta.IColumn entity)
    {
        return entity.Name.Replace(" ", "_").Replace("$", "$_");
    }
}


/// /////////////////////////////////////////////////////

public class BusinessLogicLayerTemplate_5G_CSHARP : ITemplate
{
    string _workingdir;
    public const string TemplateName = "Data Abstraction Model Template  (5G-CSHARP)";
    public const string TemplateDescription = "Data Abstraction Model Template for CSHARP";
    const string TemplateOutputLanguage = "CSHARP";
    private string _languageMappingFileName;
    private string _dbTargetMappingFileName;
    private string _databaseName = string.Empty;
    private GenerationProject _generationProject = null;
    private MyMeta.IDatabase _db = null;
    public String CustomDomain = String.Empty;
    public String BaseDirectory { get; set; }
    bool ITemplate.Execute(MyMeta.IDatabase db, string workingDir, GenerationProject generationProject)
    {
        _workingdir = workingDir;
        _db = (MyMeta.IDatabase)db;
        CustomDomain = _db.Name;
        return this.Execute(ref _db, workingDir, generationProject);
    }
    public BusinessLogicLayerTemplate_5G_CSHARP()
    { }
    private string GUID = "{34759F30-8DFA-4E62-B2DC-C00AF2CB9DB8}";

    public BusinessLogicLayerTemplate_5G_CSHARP(GenerationProject generationProject)
    {

    }

    public bool Execute(ref MyMeta.IDatabase db, string workingDir, GenerationProject generationProject)
    {

        try
        {

            db.Root.DbTarget = GetDbTarget(db.Root.Driver);
            db.Root.Language = GetLanguage(db.Root.Driver);//"C# Types";
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            String definedColumnList = String.Empty;
            String columnList = String.Empty;
            _databaseName = db.Name;
            _generationProject = generationProject;

            //Data Abstraction files
            BuildBusinessTables(db, generationProject.Namespace);
            BuildBusinessViews(db, generationProject.Namespace);
            BuildBusinessProgrammability(db, generationProject.Namespace);

            BuildEntitiesTables(db, generationProject.Namespace);
            BuildEntitiesViews(db, generationProject.Namespace);
            BuildEntitiesProgrammability(db, generationProject.Namespace);

            if (generationProject.BuildRelations)
            {
                BuildBusinessRelations(db, generationProject.Namespace);
                BuildEntitiesRelations(db, generationProject.Namespace, generationProject.ChildrenPrefix);
                BuildRelationsDataHandler(generationProject.Namespace);
            }
            //Framework files

            BuildDataField();
            switch (generationProject.NetVersion)
            {
                case GenerationProject.NetVersionEnum.Net40:
                    BuildConfigurationHandler();
                    BuildDataHandlerBaseNET40(generationProject.Namespace);
                    break;
                case GenerationProject.NetVersionEnum.Net45:
                    BuildConfigurationHandler();
                    BuildDataHandlerBaseNET45(generationProject.Namespace);
                    break;
                case GenerationProject.NetVersionEnum.NetCore:
                    BuildConfigurationHandlerNETCore();
                    BuildDataHandlerBaseNETCORE(generationProject.Namespace);
                    break;
                default:
                    break;
            }

            BuildDataHandler();

            BuildInterfaces();
            BuildParameterBase(generationProject.Namespace);
            BuildProcedureDataHandler();
            BuildFunctionDataHandler();
            BuildsqlEnumerations(generationProject.Namespace);
            BuildAttributes();
            BuildConstants(generationProject);
            //BuildProject();
            //BuildSolution();
            //BuildAssemblyInfo();

            //BuildWebGridWebForms(db);

            //if (_generationProject.MappedStoredProcedures.Count > 0)
            BuildEntitiesMappedProcedures();
            BuildBusinessMappedProcedures();
            BuildMappedProcedureDataHandler();
            BuildCore(generationProject.Namespace);
            if (_generationProject.UsesEncription)
                BuildEncriptionFile(generationProject.Namespace);

            return true;
        }
        catch (Exception ex)
        {
            if (OnException != null)
            {
                OnException(ex);
            }
            return false;
        }

    }
    #region Build Methods
    private void BuildBusinessTables(MyMeta.IDatabase db, string _namespace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        String definedColumnList = string.Empty;
        String columnList = string.Empty;

        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Linq;");
        output.AppendLine("using System.Text;");


        string currentEntity = string.Empty;
        try
        {
            String nullable = String.Empty;
            foreach (MyMeta.ITable entity in db.Tables)
            {
                if (entity.Selected)
                {
                    currentEntity = GetFormattedEntityName(entity.Name);
                    output.AppendLine("	namespace " + _namespace + ".Business.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + " {");
                    output.AppendLine("	    /// <summary>");
                    output.AppendLine("	    /// " + entity.Description);
                    output.AppendLine("	    /// </summary>");
                    output.AppendLine("		public partial class " + GetFormattedEntityName(entity.Name) + " : DataHandler");
                    output.AppendLine("		{");
                    output.AppendLine("				public enum ColumnEnum : int");
                    output.AppendLine("                {");
                    int columnCount = 0;
                    foreach (MyMeta.IColumn column in entity.Columns)
                    {
                        columnCount += 1;
                        if (columnCount == entity.Columns.Count)
                        {
                            output.AppendLine("					" + GetFormattedEntityName(column.Name));
                        }
                        else
                        {
                            output.AppendLine("					" + GetFormattedEntityName(column.Name) + ",");
                        }
                    }

                    output.AppendLine("				}");
                    output.AppendLine("         protected List<Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> _entities = new List<Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ">();");
                    output.AppendLine("         protected List<IDataItem> _cacheItemList = new List<IDataItem>();");
                    //output.AppendLine("         public WhereCollection Where { get; set; }");
                    //output.AppendLine("         public OrderByCollection OrderBy { get; set; }");
                    //output.AppendLine("         public GroupByCollection GroupBy { get; set; }");
                    output.AppendLine("         public WhereCollection Where = new WhereCollection();");
                    output.AppendLine("         public OrderByCollection OrderBy = new OrderByCollection();");
                    output.AppendLine("         public GroupByCollection GroupBy = new GroupByCollection();");

                    output.AppendLine("         public AggregateCollection Aggregate { get; set; }");
                    output.AppendLine("            public " + GetFormattedEntityName(entity.Name) + "() : base()");
                    output.AppendLine("            {");
                    output.AppendLine("                base._dataItem = new Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "();");
                    //output.AppendLine("                Where = new WhereCollection();");
                    //output.AppendLine("                OrderBy = new OrderByCollection();");
                    //output.AppendLine("                GroupBy = new GroupByCollection();");
                    output.AppendLine("            }");
                    output.AppendLine("            public " + GetFormattedEntityName(entity.Name) + "(IDataHandler dataHandler)");
                    output.AppendLine("                : base(dataHandler)");
                    output.AppendLine("            {");
                    output.AppendLine("                base._transaction = dataHandler.GetTransaction();");
                    output.AppendLine("                base._connection = dataHandler.GetConnection();");
                    output.AppendLine("                base._dataItem = new Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "();");
                    //output.AppendLine("                Where = new WhereCollection();");
                    //output.AppendLine("                OrderBy = new OrderByCollection();");
                    //output.AppendLine("                GroupBy = new GroupByCollection();");
                    output.AppendLine("            }");

                    output.AppendLine("            public class AggregateCollection : AggregateParameter");
                    output.AppendLine("            {");
                    output.AppendLine("                 internal AggregateParameter aggregateParameter = new AggregateParameter();");
                    output.AppendLine("                 public void Add(" + _namespace + ".sqlEnum.FunctionEnum functionEnum, ColumnEnum column)");
                    output.AppendLine("                     {");
                    output.AppendLine("                         this.aggregateParameter.Add(functionEnum, Enum.GetName(typeof(ColumnEnum), column));");
                    output.AppendLine("                     }");
                    output.AppendLine("            }");

                    output.AppendLine("			// Adds to a memory cache to hold pending transactions");
                    output.AppendLine("			public void AddToCache(Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " item)");
                    output.AppendLine("			{");
                    output.AppendLine("				_cacheItemList.Add(item);");
                    output.AppendLine("			}");
                    output.AppendLine("			public void UpdateCache()");
                    output.AppendLine("			{");
                    output.AppendLine("                this.BeginTransaction();");
                    output.AppendLine("				foreach(IDataItem item in _cacheItemList)");
                    output.AppendLine("					base.Add(item);");
                    output.AppendLine("				this.EndTransaction(true);");
                    output.AppendLine("			}");
                    output.AppendLine("			// Method that accepts arguments corresponding to fields (Those wich aren´t identity.)");
                    output.AppendLine("         /// <summary>");
                    output.AppendLine("         /// " + GetFormattedEntityName(entity.Name) + " Add Method");
                    output.AppendLine("         /// </summary>");
                    definedColumnList = string.Empty; columnList = string.Empty;
                    foreach (MyMeta.IColumn column in entity.Columns)
                    {
                        if (!column.IsAutoKey)
                        {
                            output.AppendLine("         /// <param name='" + GetFormattedEntityName(column.Name) + "'></param>");
                            nullable = column.IsNullable == true && column.LanguageType != "String" ? "?" : String.Empty;
                            definedColumnList += column.LanguageType + nullable + " " + GetFormattedEntityName(column.Name) + ",";
                            columnList += GetFormattedEntityName(column.Name) + ",";
                        }
                    }
                    if (definedColumnList.Length > 0)
                        definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);
                    if (columnList.Length > 0)
                        columnList = columnList.Substring(0, columnList.Length - 1);
                    output.AppendLine("         /// <returns>Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "</returns>");
                    output.AppendLine("			public Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " Add(" + definedColumnList + ") ");
                    output.AppendLine("			{");
                    output.AppendLine("			  return (Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ")base.Add(new Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "(" + columnList + "));");
                    output.AppendLine("			}");
                    output.AppendLine("            public new List<Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> Items()");
                    output.AppendLine("            {");
                    output.AppendLine("                this.WhereParameter = this.Where;");
                    output.AppendLine("                this.OrderByParameter = this.OrderBy;");
                    output.AppendLine("                this.GroupByParameter = this.GroupBy;");
                    output.AppendLine("                this.TopQuantity = this.TopQuantity;");
                    output.AppendLine("                base.AnalizeIDataItem();");
                    output.AppendLine("                _entities = base.Items().Cast<Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ">().ToList<Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ">();");
                    output.AppendLine("                return _entities;");
                    output.AppendLine("            }");
                    ///////////////////////////////////////////////////////////////
                    if (HasPrimaryKey(entity))
                    {
                        if (entity.Columns.Count > GetPrimaryKeyCount(entity))
                        {
                            output.AppendLine("            /// <summary>");
                            output.AppendLine("            /// Gets Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " items by Pk");
                            output.AppendLine("            /// </summary>");
                            definedColumnList = String.Empty;
                            foreach (MyMeta.IColumn column in entity.Columns)
                            {
                                if (column.IsInPrimaryKey)
                                {
                                    output.AppendLine("            /// <param name=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></param>");
                                    definedColumnList += column.LanguageType + " " + GetFormattedEntityName(column.Name) + ",";
                                }
                            }
                            if (definedColumnList.Length > 0)
                                definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);
                            output.AppendLine("            /// <returns></returns>");
                            output.AppendLine("            public List<Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> Items(" + definedColumnList + ")");
                            output.AppendLine("            {");
                            output.AppendLine("                this.Where.Clear();");
                            foreach (MyMeta.IColumn column in entity.Columns)
                            {
                                if (column.IsInPrimaryKey)
                                {
                                    output.AppendLine("                    if (this.Where.Count == 0)");
                                    output.AppendLine("                    {");
                                    output.AppendLine("                         this.Where.Add(ColumnEnum." + GetFormattedEntityName(column.Name) + ", " + _namespace + ".sqlEnum.OperandEnum.Equal, " + GetFormattedEntityName(column.Name) + ");");
                                    output.AppendLine("                    }");
                                    output.AppendLine("                    else");
                                    output.AppendLine("                    {");
                                    output.AppendLine("                         this.Where.Add(" + _namespace + ".sqlEnum.ConjunctionEnum.AND,ColumnEnum." + GetFormattedEntityName(column.Name) + ", " + _namespace + ".sqlEnum.OperandEnum.Equal, " + GetFormattedEntityName(column.Name) + ");");
                                    output.AppendLine("                    }");
                                }
                            }

                            output.AppendLine("                return this.Items();");
                            output.AppendLine("            }");
                        }
                    }
                    output.AppendLine("            /// <summary>");
                    output.AppendLine("            /// Gets Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " items with parameters.");
                    output.AppendLine("            /// </summary>");
                    definedColumnList = string.Empty;
                    foreach (MyMeta.IColumn column in entity.Columns)
                    {
                        output.AppendLine("            /// <param name=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></param>");
                        if (!column.LanguageType.Equals("Byte[]"))
                        {
                            if (column.LanguageType.ToUpper().Equals("STRING"))
                            {
                                definedColumnList += column.LanguageType + " " + GetFormattedEntityName(column.Name) + ",";
                            }
                            else
                            {
                                definedColumnList += column.LanguageType + "? " + GetFormattedEntityName(column.Name) + ",";
                            }
                        }
                    }
                    output.AppendLine("            /// <returns></returns>");
                    if (definedColumnList.Length > 0)
                        definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);
                    output.AppendLine("            public List<Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> Items(" + definedColumnList + ")");
                    output.AppendLine("            {");
                    output.AppendLine("                this.Where.Clear();");
                    foreach (MyMeta.IColumn column in entity.Columns)
                    {
                        if (!column.LanguageType.Equals("Byte[]"))
                        {
                            output.AppendLine("                if (" + GetFormattedEntityName(column.Name) + " != null)");
                            output.AppendLine("                {");
                            output.AppendLine("                    if (this.Where.Count == 0)");
                            output.AppendLine("                    {");
                            output.AppendLine("                        this.Where.Add(ColumnEnum." + GetFormattedEntityName(column.Name) + ", sqlEnum.OperandEnum.Equal, " + GetFormattedEntityName(column.Name) + ");");
                            output.AppendLine("                    }");
                            output.AppendLine("                    else");
                            output.AppendLine("                    {");
                            output.AppendLine("                        this.Where.Add(sqlEnum.ConjunctionEnum.AND,ColumnEnum." + GetFormattedEntityName(column.Name) + ", " + _namespace + ".sqlEnum.OperandEnum.Equal, " + GetFormattedEntityName(column.Name) + ");");
                            output.AppendLine("                    }");
                            output.AppendLine("                   ");
                            output.AppendLine("                }");
                        }
                    }
                    output.AppendLine("                return this.Items();");
                    output.AppendLine("            }");
                    output.AppendLine("            /// <summary>");
                    output.AppendLine("            /// Adds an instance of Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "");
                    output.AppendLine("            /// </summary>");
                    output.AppendLine("            /// <param name=" + System.Convert.ToChar(34) + "item" + System.Convert.ToChar(34) + "></param>");
                    output.AppendLine("            /// <returns></returns>");
                    output.AppendLine("            public Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " Add(Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " item)");
                    output.AppendLine("            {");
                    output.AppendLine("                return (Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ")base.Add((IDataItem)item);");
                    output.AppendLine("            }");
                    String pkValuesList = String.Empty;
                    foreach (MyMeta.IColumn column in entity.Columns)
                    {
                        if (column.IsInPrimaryKey)
                        {
                            pkValuesList += "item." + GetFormattedEntityName(column.Name) + ",";
                        }
                    }
                    if (pkValuesList.Length > 0)
                        pkValuesList = pkValuesList.Substring(0, pkValuesList.Length - 1);

                    output.AppendLine("            /// <summary>");
                    output.AppendLine("            /// Adds or updates an instance of Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "");
                    output.AppendLine("            /// </summary>");
                    output.AppendLine("            /// <param name=" + System.Convert.ToChar(34) + "item" + System.Convert.ToChar(34) + "></param>");
                    output.AppendLine("            /// <returns></returns>");
                    output.AppendLine("            public Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " AddOrUpdate(Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " item)");
                    output.AppendLine("            {");
                    output.AppendLine("                 if (Items(" + pkValuesList + ").Count == 0)");
                    output.AppendLine("                 {");
                    output.AppendLine("                     return (Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ")base.Add((IDataItem)item);");
                    output.AppendLine("                 }");
                    output.AppendLine("                 else");
                    output.AppendLine("                 {");
                    output.AppendLine("                     Update(item);");
                    output.AppendLine("                     return item;");
                    output.AppendLine("                 }");
                    output.AppendLine("             }");


                    output.AppendLine("            /// <summary>");
                    output.AppendLine("            /// Updates an instance of Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "");
                    output.AppendLine("            /// </summary>");
                    output.AppendLine("            /// <param name=" + System.Convert.ToChar(34) + "item" + System.Convert.ToChar(34) + "></param>");
                    output.AppendLine("            /// <returns><Int64/returns>");
                    output.AppendLine("            public Int64 Update(Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " item)");
                    output.AppendLine("            {");
                    output.AppendLine("                return base.Update((IDataItem)item);");
                    output.AppendLine("            }");
                    output.AppendLine("            /// Updates an instance of Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " with parameters");
                    output.AppendLine("            /// </summary>");
                    definedColumnList = string.Empty;
                    columnList = string.Empty;
                    foreach (MyMeta.IColumn column in entity.Columns)
                    {
                        output.AppendLine("            /// <param name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + "></param>");
                        nullable = column.IsNullable == true && column.LanguageType != "String" ? "?" : String.Empty;
                        definedColumnList += column.LanguageType + nullable + " " + GetFormattedEntityName(column.Name).ToLower() + ",";
                        columnList += GetFormattedEntityName(column.Name) + " = " + GetFormattedEntityName(column.Name).ToLower() + ",";
                    }

                    output.AppendLine("            /// <returns>Int64</returns>");
                    if (definedColumnList.Length > 0)
                        definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);
                    if (columnList.Length > 0)
                        columnList = columnList.Substring(0, columnList.Length - 1);

                    output.AppendLine("            public Int64 Update(" + definedColumnList + ")");
                    output.AppendLine("            {");
                    output.AppendLine("                return base.Update((IDataItem) new Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " {" + columnList + "});");
                    output.AppendLine("            }");

                    output.AppendLine("            /// <summary>");
                    output.AppendLine("            /// Deletes an instance of Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "");
                    output.AppendLine("            /// </summary>");
                    output.AppendLine("            /// <param name=" + System.Convert.ToChar(34) + "item" + System.Convert.ToChar(34) + "></param>");
                    output.AppendLine("            /// <returns></returns>");
                    output.AppendLine("            public Int64 Delete(Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " item)");
                    output.AppendLine("            {");
                    output.AppendLine("                return base.DeleteItem((IDataItem)item);");
                    output.AppendLine("            }");
                    output.AppendLine("            /// <summary>");
                    output.AppendLine("            /// Deletes Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " with where conditions");
                    output.AppendLine("            /// </summary>");
                    output.AppendLine("            /// <returns></returns>");
                    output.AppendLine("            public new Int64 Delete()");
                    output.AppendLine("            {");
                    output.AppendLine("                this.WhereParameter = this.Where;");
                    output.AppendLine("                this.OrderByParameter = this.OrderBy;");
                    output.AppendLine("                this.GroupByParameter = this.GroupBy;");

                    output.AppendLine("                return this.Delete();");
                    output.AppendLine("            }");
                    output.AppendLine("            /// <summary>");


                    String pkList = String.Empty;
                    String paramsList = String.Empty;
                    foreach (MyMeta.IColumn column in entity.Columns)
                    {
                        if (column.IsInPrimaryKey)
                        {
                            paramsList += column.LanguageType + " " + GetFormattedEntityName(column.Name).ToLower() + ",";
                            pkList += GetFormattedEntityName(column.Name) + " = " + GetFormattedEntityName(column.Name).ToLower() + ",";
                        }
                    }
                    if (pkList.Length > 0)
                        pkList = pkList.Substring(0, pkList.Length - 1);
                    if (paramsList.Length > 0)
                        paramsList = paramsList.Substring(0, paramsList.Length - 1);
                    if (pkList.Length > 0 && paramsList.Length > 0)
                    {
                        output.AppendLine("            /// Deletes by Pks");
                        output.AppendLine("            /// </summary>");
                        output.AppendLine("            /// <returns></returns>");

                        output.AppendLine("            public Int64 Delete(" + paramsList + ")");
                        output.AppendLine("            {");
                        output.AppendLine("                return base.DeleteItem((IDataItem) new Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " {" + pkList + "});");
                        output.AppendLine("            }");
                    }
                    output.AppendLine("            /// <summary>");
                    output.AppendLine("            /// Holds last Items() executed.");
                    output.AppendLine("            /// </summary>");
                    output.AppendLine("            /// <returns>Last Items()</returns>");
                    output.AppendLine("            public List<Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> Result");
                    output.AppendLine("            {");
                    output.AppendLine("                get{return _entities;}");
                    output.AppendLine("            }");
                    output.AppendLine("            public class WhereCollection : WhereParameter {");
                    //output.AppendLine("                 internal WhereParameter whereParameter = new WhereParameter();");
                    output.AppendLine("                 public void Add(ColumnEnum betweenColumn, " + _namespace + ".sqlEnum.OperandEnum operand, object valueFrom, object valueTo)");
                    output.AppendLine("                 {");
                    //output.AppendLine("                     this.whereParameter.Add(Enum.GetName(typeof(ColumnEnum), betweenColumn), valueFrom, valueTo);");
                    output.AppendLine("                     base.Add(Enum.GetName(typeof(ColumnEnum), betweenColumn), valueFrom, valueTo);");
                    output.AppendLine("                 }");
                    output.AppendLine("                 public void  Add(ColumnEnum column, " + _namespace + ".sqlEnum.OperandEnum operand,object value)");
                    output.AppendLine("                 {");
                    //output.AppendLine("                     this.whereParameter.Add(Enum.GetName(typeof(ColumnEnum), column), operand, value);");
                    output.AppendLine("                     base.Add(Enum.GetName(typeof(ColumnEnum), column), operand, value);");
                    output.AppendLine("                 }");
                    output.AppendLine("                 public void Add(" + _namespace + ".sqlEnum.ConjunctionEnum conjunction,ColumnEnum betweenColumn, " + _namespace + ".sqlEnum.OperandEnum operand, object valueFrom, object valueTo)");
                    output.AppendLine("                 {");
                    //output.AppendLine("                     this.whereParameter.Add(conjunction, Enum.GetName(typeof(ColumnEnum), betweenColumn), valueFrom, valueTo);");
                    output.AppendLine("                     base.Add(conjunction, Enum.GetName(typeof(ColumnEnum), betweenColumn), valueFrom, valueTo);");
                    output.AppendLine("                 }");
                    output.AppendLine("                 public void AddOperand(" + _namespace + ".sqlEnum.ConjunctionEnum Conjunction)");
                    output.AppendLine("                 {");
                    output.AppendLine("                     base.AddConjunction(Conjunction);");
                    output.AppendLine("                 }");
                    output.AppendLine("                 public void OpenParentheses()");
                    output.AppendLine("                 {");
                    output.AppendLine("                     base.OpenParentheses();");
                    output.AppendLine("                 }");
                    output.AppendLine("                 public void CloseParentheses()");
                    output.AppendLine("                 {");
                    output.AppendLine("                     base.CloseParentheses();");
                    output.AppendLine("                 }");
                    output.AppendLine("                 public void Add(" + _namespace + ".sqlEnum.ConjunctionEnum conjunction,ColumnEnum column, " + _namespace + ".sqlEnum.OperandEnum operand, object value)");
                    output.AppendLine("                 {");
                    //output.AppendLine("                     this.whereParameter.Add(conjunction, Enum.GetName(typeof(ColumnEnum), column), operand, value);");
                    output.AppendLine("                     base.Add(conjunction, Enum.GetName(typeof(ColumnEnum), column), operand, value);");
                    output.AppendLine("                 }");
                    output.AppendLine("                 public new void Clear()"); // MODIF- 2-03-2016 
                    output.AppendLine("                 {");
                    //output.AppendLine("                     this.whereParameter.Clear();");
                    output.AppendLine("                     base.Clear();");
                    output.AppendLine("                 }");
                    output.AppendLine("                 public new long Count");
                    output.AppendLine("                 {");
                    output.AppendLine("                     get {");
                    //output.AppendLine("                         return this.whereParameter.Count;");
                    output.AppendLine("                         return base.Count;");
                    output.AppendLine("                     }");
                    output.AppendLine("                 }");
                    output.AppendLine("            }");
                    output.AppendLine("            public class OrderByCollection : OrderByParameter {");
                    //output.AppendLine("                 internal OrderByParameter orderByParameter = new OrderByParameter();");
                    output.AppendLine("                 public void Add(ColumnEnum column, " + _namespace + ".sqlEnum.DirEnum direction = " + _namespace + ".sqlEnum.DirEnum.ASC)");
                    output.AppendLine("                 {");
                    //output.AppendLine("                     this.orderByParameter.Add(Enum.GetName(typeof(ColumnEnum), column), direction);");
                    output.AppendLine("                     base.Add(Enum.GetName(typeof(ColumnEnum), column), direction);");
                    output.AppendLine("                 }");
                    output.AppendLine("            }");
                    output.AppendLine("            public class GroupByCollection : GroupByParameter {");
                    //output.AppendLine("                 internal GroupByParameter groupByParameter = new GroupByParameter();");
                    output.AppendLine("                 public void Add(ColumnEnum column)");
                    output.AppendLine("                 {");
                    //output.AppendLine("                     this.groupByParameter.Add(Enum.GetName(typeof(ColumnEnum), column));");
                    output.AppendLine("                     base.Add(Enum.GetName(typeof(ColumnEnum), column));");
                    output.AppendLine("                 }");
                    output.AppendLine("            }");
                    output.AppendLine("             public void Dispose()");
                    output.AppendLine("             {");
                    output.AppendLine("                 _entities = null;");
                    output.AppendLine("                 _cacheItemList = null;");
                    output.AppendLine("                 Where = null;");
                    output.AppendLine("                 OrderBy = null;");
                    output.AppendLine("                 GroupBy = null;");
                    output.AppendLine("                 Aggregate = null;");
                    output.AppendLine("				");
                    output.AppendLine("                 base.Dispose(true);");
                    output.AppendLine("             }");
                    output.AppendLine("        } // class " + GetFormattedEntityName(entity.Name));
                    output.AppendLine("	} //namespace " + _namespace + ".Business.Tables." + GetSchemaName(GetSchemaName(entity.Schema)));
                    System.Diagnostics.Debug.Print(GetFormattedEntityName(entity.Name));
                }
            }

            SaveOutputToFile("Business.Tables.cs", output, true);
            definedColumnList = string.Empty;
            columnList = string.Empty;
            output = new StringBuilder();
        }
        catch (Exception ex)
        {
            currentEntity = currentEntity + "";
            if (OnException != null)
            {
                OnException(ex);
            }
        }

    }
    private void BuildBusinessViews(MyMeta.IDatabase db, string _namespace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        String definedColumnList = string.Empty;
        String columnList = string.Empty;

        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Linq;");
        output.AppendLine("using System.Text;");

        foreach (MyMeta.IView entity in db.Views)
        {
            if (entity.Selected)
            {

                output.AppendLine("	namespace " + _namespace + ".Business.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + " {");
                output.AppendLine("	    /// <summary>");
                output.AppendLine("	    /// " + entity.Description);
                output.AppendLine("	    /// </summary>");
                output.AppendLine("		public class " + GetFormattedEntityName(entity.Name) + " : DataHandler");
                output.AppendLine("		{");
                output.AppendLine("				public enum ColumnEnum : int");
                output.AppendLine("                {");
                int columnCount = 0;
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    columnCount += 1;
                    if (columnCount == entity.Columns.Count)
                    {
                        output.AppendLine("					" + GetFormattedEntityName(column.Name));
                    }
                    else
                    {
                        output.AppendLine("					" + GetFormattedEntityName(column.Name) + ",");
                    }
                }

                output.AppendLine("				}");


                output.AppendLine("			protected List<IDataItem> _cacheItemList = new List<IDataItem>();");
                output.AppendLine("         public WhereCollection Where = new WhereCollection();");
                output.AppendLine("         public OrderByCollection OrderBy = new OrderByCollection();");
                output.AppendLine("         public GroupByCollection GroupBy = new GroupByCollection();");
                output.AppendLine("         public AggregateCollection Aggregate { get; set; }");
                output.AppendLine("         private List<Entities.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> _entities = new List<Entities.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ">();");

                output.AppendLine("            public " + GetFormattedEntityName(entity.Name) + "() : base()");
                output.AppendLine("            {");
                output.AppendLine("                base._dataItem = new Entities.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "();");
                output.AppendLine("                Where = new WhereCollection();");
                output.AppendLine("                OrderBy = new OrderByCollection();");
                output.AppendLine("                GroupBy = new GroupByCollection();");
                output.AppendLine("            }");
                output.AppendLine("            public " + GetFormattedEntityName(entity.Name) + "(IDataHandler dataHandler)");
                output.AppendLine("                : base(dataHandler)");
                output.AppendLine("            {");
                output.AppendLine("                base._transaction = dataHandler.GetTransaction();");
                output.AppendLine("                base._connection = dataHandler.GetConnection();");
                output.AppendLine("                base._dataItem = new Entities.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "();");
                output.AppendLine("                Where = new WhereCollection();");
                output.AppendLine("                OrderBy = new OrderByCollection();");
                output.AppendLine("                GroupBy = new GroupByCollection();");
                output.AppendLine("            }");

                output.AppendLine("            public class AggregateCollection : AggregateParameter");
                output.AppendLine("            {");
                output.AppendLine("                 internal AggregateParameter aggregateParameter = new AggregateParameter();");
                output.AppendLine("                 public void Add(" + _namespace + ".sqlEnum.FunctionEnum functionEnum, ColumnEnum column)");
                output.AppendLine("                     {");
                output.AppendLine("                         this.aggregateParameter.Add(functionEnum, Enum.GetName(typeof(ColumnEnum), column));");
                output.AppendLine("                     }");
                output.AppendLine("            }");


                output.AppendLine("			// Adds to a memory cache to hold pending transactions");
                output.AppendLine("			public void AddToCache(Entities.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " item)");
                output.AppendLine("			{");
                output.AppendLine("				_cacheItemList.Add(item);");
                output.AppendLine("			}");
                output.AppendLine("			public void UpdateCache()");
                output.AppendLine("			{");
                output.AppendLine("                this.BeginTransaction();");
                output.AppendLine("				foreach(IDataItem item in _cacheItemList)");
                output.AppendLine("					base.Add(item);");
                output.AppendLine("				this.EndTransaction(true);");
                output.AppendLine("			}");
                output.AppendLine("			// Method that accepts arguments corresponding to fields (Those wich aren´t identity.)");
                definedColumnList = string.Empty; columnList = string.Empty;
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    definedColumnList += column.LanguageType + " " + GetFormattedEntityName(column.Name) + ",";
                    columnList += GetFormattedEntityName(column.Name) + ",";
                }
                if (definedColumnList.Length > 0)
                    definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);
                if (columnList.Length > 0)
                    columnList = columnList.Substring(0, columnList.Length - 1);

                output.AppendLine("			public Entities.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " Add(" + definedColumnList + ") ");
                output.AppendLine("			{");
                output.AppendLine("			  return (Entities.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ")base.Add(new Entities.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "(" + columnList + "));");
                output.AppendLine("			}");
                output.AppendLine("            public new List<Entities.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> Items()");
                output.AppendLine("            {");
                output.AppendLine("                this.WhereParameter = this.Where;");
                output.AppendLine("                this.OrderByParameter = this.OrderBy.orderByParameter;");
                output.AppendLine("                this.GroupByParameter = this.GroupBy.groupByParameter;");
                output.AppendLine("                this.TopQuantity = this.TopQuantity;");
                output.AppendLine("                base.AnalizeIDataItem();");
                output.AppendLine("                _entities = base.Items().Cast<Entities.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ">().ToList<Entities.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ">();");
                output.AppendLine("                return _entities;");
                output.AppendLine("            }");
                output.AppendLine("            /// <summary>");
                output.AppendLine("            /// Holds last Items() executed.");
                output.AppendLine("            /// </summary>");
                output.AppendLine("            /// <returns>Last Items()</returns>");
                output.AppendLine("            public List<Entities.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> Result");
                output.AppendLine("            {");
                output.AppendLine("                get{return _entities;}");
                output.AppendLine("            }");

                output.AppendLine("            /// <summary>");
                output.AppendLine("            /// Gets ");
                output.AppendLine("            /// </summary>");
                definedColumnList = string.Empty;
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    output.AppendLine("            /// <param name=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></param>");
                    if (!column.LanguageType.Equals("Byte[]"))
                    {
                        if (column.LanguageType.ToUpper().Equals("STRING"))
                        {
                            definedColumnList += column.LanguageType + " " + GetFormattedEntityName(column.Name) + ",";
                        }
                        else
                        {
                            definedColumnList += column.LanguageType + "? " + GetFormattedEntityName(column.Name) + ",";
                        }
                    }
                }
                output.AppendLine("            /// <returns></returns>");
                if (definedColumnList.Length > 0)
                    definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);
                output.AppendLine("            public List<Entities.Views." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> Items(" + definedColumnList + ")");
                output.AppendLine("            {");
                output.AppendLine("                this.Where.Clear();");

                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    if (!column.LanguageType.Equals("Byte[]"))
                    {
                        output.AppendLine("                if (" + GetFormattedEntityName(column.Name) + " != null)");
                        output.AppendLine("                {");
                        output.AppendLine("                    if (this.Where.Count == 0)");
                        output.AppendLine("                    {");
                        output.AppendLine("                        this.Where.Add(ColumnEnum." + GetFormattedEntityName(column.Name) + ", " + _namespace + ".sqlEnum.OperandEnum.Equal, " + GetFormattedEntityName(column.Name) + ");");
                        output.AppendLine("                    }");
                        output.AppendLine("                    else");
                        output.AppendLine("                    {");
                        output.AppendLine("                        this.Where.Add(" + _namespace + ".sqlEnum.ConjunctionEnum.AND, ColumnEnum." + GetFormattedEntityName(column.Name) + ", " + _namespace + ".sqlEnum.OperandEnum.Equal, " + GetFormattedEntityName(column.Name) + ");");
                        output.AppendLine("                    }");
                        output.AppendLine("                   ");
                        output.AppendLine("                }");
                    }
                }

                output.AppendLine("                return this.Items();");
                output.AppendLine("            }");

                output.AppendLine("            public new IDataItem Add(IDataItem item)");
                output.AppendLine("            {");
                output.AppendLine("                DataHandler dh = new DataHandler(this._dataItem);");
                output.AppendLine("                return base.Add(item);");
                output.AppendLine("            }");

                output.AppendLine("            public class WhereCollection : WhereParameter {");

                output.AppendLine("                 public void Add(ColumnEnum betweenColumn, " + _namespace + ".sqlEnum.OperandEnum operand, object valueFrom, object valueTo)");
                output.AppendLine("                 {");
                output.AppendLine("                     base.Add(Enum.GetName(typeof(ColumnEnum), betweenColumn), valueFrom, valueTo);");
                output.AppendLine("                 }");
                output.AppendLine("                 public void  Add(ColumnEnum column, " + _namespace + ".sqlEnum.OperandEnum operand,object value)");
                output.AppendLine("                 {");
                output.AppendLine("                     base.Add(Enum.GetName(typeof(ColumnEnum), column), operand, value);");
                output.AppendLine("                 }");
                output.AppendLine("                 public void Add(" + _namespace + ".sqlEnum.ConjunctionEnum conjunction,ColumnEnum betweenColumn, " + _namespace + ".sqlEnum.OperandEnum operand, object valueFrom, object valueTo)");
                output.AppendLine("                 {");
                output.AppendLine("                     base.Add(conjunction, Enum.GetName(typeof(ColumnEnum), betweenColumn), valueFrom, valueTo);");
                output.AppendLine("                 }");
                output.AppendLine("                 public void Add(" + _namespace + ".sqlEnum.ConjunctionEnum conjunction,ColumnEnum column, " + _namespace + ".sqlEnum.OperandEnum operand, object value)");
                output.AppendLine("                 {");
                output.AppendLine("                     base.Add(conjunction, Enum.GetName(typeof(ColumnEnum), column), operand, value);");
                output.AppendLine("                 }");
                output.AppendLine("                 public void AddOperand(" + _namespace + ".sqlEnum.ConjunctionEnum Conjunction)");
                output.AppendLine("                 {");
                output.AppendLine("                     base.AddConjunction(Conjunction);");
                output.AppendLine("                 }");
                output.AppendLine("                 public void OpenParentheses()");
                output.AppendLine("                 {");
                output.AppendLine("                     base.OpenParentheses();");
                output.AppendLine("                 }");
                output.AppendLine("                 public void CloseParentheses()");
                output.AppendLine("                 {");
                output.AppendLine("                     base.CloseParentheses();");
                output.AppendLine("                 }");
                output.AppendLine("            }");
                output.AppendLine("            public class OrderByCollection : OrderByParameter {");
                output.AppendLine("                 internal OrderByParameter orderByParameter = new OrderByParameter();");
                output.AppendLine("                 public void Add(ColumnEnum column, " + _namespace + ".sqlEnum.DirEnum direction = " + _namespace + ".sqlEnum.DirEnum.ASC)");
                output.AppendLine("                 {");
                output.AppendLine("                     this.orderByParameter.Add(Enum.GetName(typeof(ColumnEnum), column), direction);");
                output.AppendLine("                 }");
                output.AppendLine("            }");
                output.AppendLine("            public class GroupByCollection : GroupByParameter {");
                output.AppendLine("                 internal GroupByParameter groupByParameter = new GroupByParameter();");
                output.AppendLine("                 public void Add(ColumnEnum column)");
                output.AppendLine("                 {");
                output.AppendLine("                     this.groupByParameter.Add(Enum.GetName(typeof(ColumnEnum), column));");
                output.AppendLine("                 }");
                output.AppendLine("            }");
                output.AppendLine("             public void Dispose()");
                output.AppendLine("             {");
                output.AppendLine("                 _cacheItemList = null;");
                output.AppendLine("                 Where = null;");
                output.AppendLine("                 OrderBy = null;");
                output.AppendLine("                 GroupBy = null;");
                output.AppendLine("                 Aggregate = null;");
                output.AppendLine("				");
                output.AppendLine("                 base.Dispose(true);");
                output.AppendLine("             }");
                output.AppendLine("        } // class " + GetFormattedEntityName(entity.Name));
                output.AppendLine("	} //namespace " + _namespace + ".Business.Views." + GetSchemaName(GetSchemaName(entity.Schema)));



            }
        }

        SaveOutputToFile("Business.Views.cs", output, true);
        definedColumnList = string.Empty;
        columnList = string.Empty;
        output = new StringBuilder();
    }
    private void BuildBusinessProgrammability(MyMeta.IDatabase db, string _namespace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        String definedColumnList = string.Empty;
        String columnList = string.Empty;
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Linq;");
        output.AppendLine("using System.Text;");

        string _entityType = String.Empty;
        string _entityNamespace = String.Empty;

        foreach (MyMeta.IProcedure entity in db.Procedures)
        {
            if (entity.Selected)
            {

                _entityType = GetEntityType(entity);
                _entityNamespace = _entityType.Substring(0, _entityType.Length - 1);

                output.AppendLine("	namespace " + _namespace + ".Business." + _entityType + "." + GetSchemaName(GetSchemaName(entity.Schema)) + " {");
                output.AppendLine("	    /// <summary>");
                output.AppendLine("	    /// " + entity.Description);
                output.AppendLine("	    /// </summary>");
                output.AppendLine("		public class " + GetFormattedEntityName(entity.Name) + " : " + _entityNamespace + "DataHandler");
                output.AppendLine("		{");
                output.AppendLine("         private List<Entities." + _entityType + "." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> _result = new List<Entities." + _entityType + "." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ">();");
                output.AppendLine("			protected List<IDataItem> _cacheItemList = new List<IDataItem>();");

                output.AppendLine("            public " + GetFormattedEntityName(entity.Name) + "() : base()");
                output.AppendLine("            {");
                output.AppendLine("                base._dataItem = new Entities." + _entityType + "." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "();");
                output.AppendLine("            }");


                string nullable = string.Empty;
                definedColumnList = string.Empty; columnList = string.Empty;
                foreach (MyMeta.IParameter column in entity.Parameters)
                {
                    if (!GetFormattedEntityName(column.Name).Equals("@RETURN_VALUE"))
                    {
                        nullable = column.IsNullable == true && column.LanguageType != "String" ? "?" : String.Empty;
                        //definedColumnList += column.LanguageType + " " + GetFormattedEntityName(column.Name).Replace("@", "") + ",";
                        definedColumnList += column.LanguageType + nullable + " " + GetFormattedEntityName(column.Name).Replace("@", "") + ",";
                        columnList += " new ParameterItemValue(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name).Replace("@", "") + System.Convert.ToChar(34) + ", " + GetFormattedEntityName(column.Name).Replace("@", "") + "),";
                    }
                }
                if (definedColumnList.Length > 0)
                    definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);
                if (columnList.Length > 0)
                    columnList = columnList.Substring(0, columnList.Length - 1);

                string newKeyword = string.Empty;
                if (entity.Parameters.Count != 1)
                    newKeyword = string.Empty;//"new";

                output.AppendLine("            public " + newKeyword + " List<Entities." + _entityType + "." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> Items(" + definedColumnList + ")");
                output.AppendLine("            {");
                output.AppendLine("                " + _entityNamespace + "DataHandler dh =  new " + _entityNamespace + "DataHandler(this._dataItem);");
                output.AppendLine("                List<Entities." + _entityType + "." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> _entities = new List<Entities." + _entityType + "." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ">();");
                output.AppendLine("                _result = dh.Items(new List<ParameterItemValue> {" + columnList + "}).Cast<Entities." + _entityType + "." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ">().ToList<Entities." + _entityType + "." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ">();");
                output.AppendLine("                return _result;");
                output.AppendLine("             }");
                output.AppendLine("             public List<Entities." + _entityType + "." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> Resultset");
                output.AppendLine("             {");
                output.AppendLine("                 get { return _result; }");
                output.AppendLine("             }");
                output.AppendLine("             public void Dispose()");
                output.AppendLine("             {");
                output.AppendLine("                 _cacheItemList = null;");

                output.AppendLine("				");
                output.AppendLine("                 base.Dispose(true);");
                output.AppendLine("             }");
                output.AppendLine("        }// class " + GetSchemaName(GetSchemaName(entity.Schema)));
                output.AppendLine("	} // namespace " + _namespace + ".Business." + _entityType + "." + GetSchemaName(GetSchemaName(entity.Schema)));


            }
        }

        SaveOutputToFile("Business.Programmability.cs", output, true);
        definedColumnList = string.Empty;
        columnList = string.Empty;
        output = new StringBuilder();
    }

    private void BuildEntitiesTables(MyMeta.IDatabase db, string _namespace)
    {
        string nullable = string.Empty;
        string dateTimeNow = string.Empty;
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Linq;");
        output.AppendLine("using System.Text;");


        foreach (MyMeta.ITable entity in db.Tables)
        {
            if (entity.Selected)
            {
                String definedColumnList = string.Empty;
                String columnList = string.Empty;

                output.AppendLine("		namespace " + _namespace + ".Entities.Tables." + GetSchemaName(entity.Schema) + " {");
                if (_generationProject.SerializeEntitiesClases)
                    output.AppendLine("			[Serializable()]                         //");
                output.AppendLine("			[DataItemAttributeSchemaName(" + System.Convert.ToChar(34) + GetSchemaName(entity.Schema) + System.Convert.ToChar(34) + ")]  // Database Schema Name");
                output.AppendLine("			[DataItemAttributeObjectName(" + System.Convert.ToChar(34) + entity.Name + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + entity.Alias + System.Convert.ToChar(34) + ")]    // Object name  and alias in Database");
                output.AppendLine("			[DataItemAttributeObjectType(DataItemAttributeObjectType.ObjectTypeEnum.Table)] // Table, View,StoredProcedure,Function");
                output.AppendLine("			public class " + GetFormattedEntityName(entity.Name) + " : IDataItem");
                output.AppendLine("			{");
                output.AppendLine("				        ");
                output.AppendLine("				public class ColumnNames");
                output.AppendLine("				{");
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    output.AppendLine("					public const string " + GetFormattedEntityName(column.Name) + " = " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ";");
                }
                output.AppendLine("				}");

                output.AppendLine("				public enum FieldEnum : int");
                output.AppendLine("                {");
                int columnCount = 0;
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    columnCount += 1;
                    if (columnCount == entity.Columns.Count)
                    {
                        output.AppendLine("					" + GetFormattedEntityName(column.Name));
                    }
                    else
                    {
                        output.AppendLine("					" + GetFormattedEntityName(column.Name) + ",");
                    }
                }

                output.AppendLine("				}");

                output.AppendLine("	               /// <summary>");
                output.AppendLine("                /// Parameterless Constructor");
                output.AppendLine("	               /// <summary>");
                output.AppendLine("                public " + GetFormattedEntityName(entity.Name) + "()");
                output.AppendLine("                {");
                output.AppendLine("                }");
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    if (!column.IsAutoKey)
                    {
                        nullable = column.IsNullable == true && column.LanguageType != "String" ? "?" : String.Empty;
                        definedColumnList += column.LanguageType + nullable + " " + GetFormattedEntityName(column.Name) + ",";
                        columnList += GetFormattedEntityName(column.Name) + ",";
                    }
                }
                if (definedColumnList.Length > 0)
                    definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);
                if (columnList.Length > 0)
                    columnList = columnList.Substring(0, columnList.Length - 1);
                if (definedColumnList.Length > 0) // No parameter constructor is built if all columns are autokey : TODO: Verify
                {
                    output.AppendLine("                public  " + GetFormattedEntityName(entity.Name) + "(" + definedColumnList + ")");
                    output.AppendLine("                {");
                    foreach (MyMeta.IColumn column in entity.Columns)
                    {
                        output.AppendLine("                    this." + GetFormattedEntityName(column.Name) + " = " + GetFormattedEntityName(column.Name) + ";");
                    }
                    output.AppendLine("                }");
                }
                string displayColumn = String.Empty;
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    // Original name and framework name
                    output.AppendLine("             [DataItemAttributeFieldName(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ")]");
                    if (column.IsInPrimaryKey)
                        output.AppendLine("             [PropertyAttribute(PropertyAttribute.PropertyAttributeEnum.Pk)] //Is Primary Key");
                    if (column.IsAutoKey)
                        output.AppendLine("             [PropertyAttribute(PropertyAttribute.PropertyAttributeEnum.Auto)] //Is Auto Key");
                    if (column.Name.Equals(_generationProject.EntityDisplayName))
                    {
                        displayColumn = column.Name;
                        output.AppendLine("             [PropertyAttribute(PropertyAttribute.PropertyAttributeEnum.Display)] //Is Display Default");
                    }
                    if (column.IsInForeignKey && !column.IsInPrimaryKey)
                    {
                        output.AppendLine("             [PropertyAttribute(PropertyAttribute.PropertyAttributeEnum.Fk)] //Is Foreign Key");
                        output.AppendLine("             [PropertyAttributeForeignKeyObjectName(" + System.Convert.ToChar(34) + column.ForeignKeys[0].PrimaryTable.Name + System.Convert.ToChar(34) + ")]// Object name in Database");
                    }
                    if (column.IsComputed)
                        output.AppendLine("             [PropertyAttribute(PropertyAttribute.PropertyAttributeEnum.Computed)] //Is Computed");
                    if (_generationProject.ForceNullableDatetime == true && (column.DataTypeName.Equals("datetime") || column.DataTypeName.Equals("smalldatetime")))
                        nullable = "?";
                    else
                        nullable = column.IsNullable == true && column.LanguageType != "String" ? "?" : String.Empty;

                    // Sets to empty because only applies to DateTime fields
                    dateTimeNow = string.Empty;
                    if (_generationProject.ForceDatetimeToDatetimeNow == true && (column.DataTypeName.Equals("datetime") || column.DataTypeName.Equals("smalldatetime")))
                        dateTimeNow = " = DateTime.Now;";

                    output.AppendLine("             public " + column.LanguageType + nullable + " " + GetFormattedEntityName(column.Name) + " { get; set; }" + dateTimeNow);
                }
                //////////////////////////////// Overrides section ////////////////////////
                if (_generationProject.OverrideEntityDisplayName)
                {

                    output.AppendLine("             public override int GetHashCode() => (" + displayColumn + " == null ? string.Empty : " + displayColumn + ").GetHashCode();");
                    output.AppendLine("             public override string ToString() => " + displayColumn + ";");
                }
                output.AppendLine("				");

                output.AppendLine("			} //Class " + GetFormattedEntityName(entity.Name) + " ");
                output.AppendLine("} //namespace " + _namespace + ".Entities.Tables." + GetSchemaName(entity.Schema));
            }

        }
        SaveOutputToFile("Entities.Tables.cs", output, true);
        output = new StringBuilder();
    }
    private void BuildEntitiesViews(MyMeta.IDatabase db, string _namespace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Linq;");
        output.AppendLine("using System.Text;");


        foreach (MyMeta.IView entity in db.Views)
        {
            if (entity.Selected)
            {
                String definedColumnList = string.Empty;
                String columnList = string.Empty;

                output.AppendLine("		namespace " + _namespace + ".Entities.Views." + GetSchemaName(entity.Schema) + " {");
                if (_generationProject.SerializeEntitiesClases)
                    output.AppendLine("			[Serializable()]                         //");
                output.AppendLine("			[DataItemAttributeSchemaName(" + System.Convert.ToChar(34) + GetSchemaName(entity.Schema) + System.Convert.ToChar(34) + ")]  // Database Schema Name");
                output.AppendLine("			[DataItemAttributeObjectName(" + System.Convert.ToChar(34) + entity.Name + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + entity.Alias + System.Convert.ToChar(34) + ")]    // Object name  and alias in Database");
                output.AppendLine("			[DataItemAttributeObjectType(DataItemAttributeObjectType.ObjectTypeEnum.View)] // Table, View,StoredProcedure,Function");
                output.AppendLine("			public class " + GetFormattedEntityName(entity.Name) + " : IDataItem");
                output.AppendLine("			{");
                output.AppendLine("				        ");
                output.AppendLine("				public class ColumnNames");
                output.AppendLine("				{");
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    output.AppendLine("					public const string " + GetFormattedEntityName(column.Name) + " = " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ";");
                }
                output.AppendLine("				}");

                output.AppendLine("				public enum FieldEnum : int");
                output.AppendLine("                {");
                int columnCount = 0;
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    columnCount += 1;
                    if (columnCount == entity.Columns.Count)
                    {
                        output.AppendLine("					" + GetFormattedEntityName(column.Name));
                    }
                    else
                    {
                        output.AppendLine("					" + GetFormattedEntityName(column.Name) + ",");
                    }
                }

                output.AppendLine("				}");

                output.AppendLine("	               /// <summary>");
                output.AppendLine("                /// Parameterless Constructor");
                output.AppendLine("	               /// </summary>");
                output.AppendLine("                public " + GetFormattedEntityName(entity.Name) + "()");
                output.AppendLine("                {");
                output.AppendLine("                }");

                output.AppendLine("	               /// <summary>");
                output.AppendLine("                /// Constructor with Parameters ");
                output.AppendLine("	               /// </summary>");
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    definedColumnList += column.LanguageType + " " + GetFormattedEntityName(column.Name) + ",";
                    columnList += GetFormattedEntityName(column.Name) + ",";
                }
                if (definedColumnList.Length > 0)
                    definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);
                if (columnList.Length > 0)
                    columnList = columnList.Substring(0, columnList.Length - 1);

                output.AppendLine("                public  " + GetFormattedEntityName(entity.Name) + "(" + definedColumnList + ")");
                output.AppendLine("                {");
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    output.AppendLine("                    this." + GetFormattedEntityName(column.Name) + " = " + GetFormattedEntityName(column.Name) + ";");
                }
                output.AppendLine("                }");

                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    output.AppendLine("             /// <summary>");
                    output.AppendLine("             /// " + GetViewColumnDescription(column, _generationProject.ConnectionString));
                    output.AppendLine("             /// </summary>");
                     // Original name and framework name
                    output.AppendLine("             [DataItemAttributeFieldName(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ")]");
                    if (column.Name.Equals("Name"))
                        output.AppendLine("             [PropertyAttribute(PropertyAttribute.PropertyAttributeEnum.Display)] //Id Display Default");
                    output.AppendLine("             public " + column.LanguageType + " " + GetFormattedEntityName(column.Name) + " { get; set; }");
                }

                output.AppendLine("				");
                output.AppendLine("			} //Class " + GetFormattedEntityName(entity.Name) + " ");
                output.AppendLine("			");
                output.AppendLine("} //namespace " + _namespace + ".Entities.Views." + GetSchemaName(entity.Schema));
                output.AppendLine("//////////////////////////////////////////////////////////////////////////////////");
            }

        }
        SaveOutputToFile("Entities.Views.cs", output, true);
        output = new StringBuilder();
    }
    private void BuildEntitiesProgrammability(MyMeta.IDatabase db, string _namespace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Linq;");
        output.AppendLine("using System.Text;");


        foreach (MyMeta.IProcedure entity in db.Procedures)
        {
            if (entity.Selected)
            {
                String _entityType = String.Empty;
                String _entityNamespace = String.Empty;

                _entityType = GetEntityType(entity);
                _entityNamespace = _entityType.Substring(0, _entityType.Length - 1);

                String definedColumnList = string.Empty;
                String columnList = string.Empty;

                output.AppendLine("		namespace " + _namespace + ".Entities." + _entityType + "." + GetSchemaName(entity.Schema) + " {");
                if (_generationProject.SerializeEntitiesClases)
                    output.AppendLine("			[Serializable()]                         //");
                output.AppendLine("			[DataItemAttributeSchemaName(" + System.Convert.ToChar(34) + GetSchemaName(entity.Schema) + System.Convert.ToChar(34) + ")]  // Database Schema Name");
                output.AppendLine("			[DataItemAttributeObjectName(" + System.Convert.ToChar(34) + entity.Name + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + entity.Alias + System.Convert.ToChar(34) + ")]    // Object name  and alias in Database");
                output.AppendLine("			[DataItemAttributeObjectType(DataItemAttributeObjectType.ObjectTypeEnum." + _entityNamespace + ")] // Table, " + _entityType + ",StoredProcedure,Function");
                output.AppendLine("			public class " + GetFormattedEntityName(entity.Name) + " : IDataItem");
                output.AppendLine("			{");
                output.AppendLine("				        ");
                output.AppendLine("				public class ColumnNames");
                output.AppendLine("				{");
                foreach (MyMeta.IResultColumn column in entity.ResultColumns)
                {
                    output.AppendLine("					public const string " + GetFormattedEntityName(column.Name) + " = " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ";");
                }
                output.AppendLine("				}");

                output.AppendLine("				public enum FieldEnum : int");
                output.AppendLine("                {");
                int columnCount = 0;
                foreach (MyMeta.IResultColumn column in entity.ResultColumns)
                {
                    columnCount += 1;
                    if (columnCount == entity.ResultColumns.Count)
                    {
                        output.AppendLine("					" + GetFormattedEntityName(column.Name));
                    }
                    else
                    {
                        output.AppendLine("					" + GetFormattedEntityName(column.Name) + ",");
                    }
                }

                output.AppendLine("				}");

                output.AppendLine("	               /// <summary>");
                output.AppendLine("                /// Parameterless Constructor");
                output.AppendLine("	               /// <summary>");
                output.AppendLine("                public " + GetFormattedEntityName(entity.Name) + "()");
                output.AppendLine("                {");
                output.AppendLine("                }");
                foreach (MyMeta.IResultColumn column in entity.ResultColumns)
                {
                    // Original name and framework name
                    output.AppendLine("             [DataItemAttributeFieldName(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ")]");
                    if (column.Name.Equals("Name"))
                        output.AppendLine("             [PropertyAttribute(PropertyAttribute.PropertyAttributeEnum.Display)] //Id Display Default");

                    //Corrección de resultados nullables - AIS
                    if (column.LanguageType.ToUpper().Equals("STRING"))
                        output.AppendLine("             public " + column.LanguageType + " " + GetFormattedEntityName(column.Name) + " { get; set; }");
                    else
                        output.AppendLine("             public " + column.LanguageType + "? " + GetFormattedEntityName(column.Name) + " { get; set; }");
                }

                output.AppendLine("				");
                output.AppendLine("			} //Class " + GetFormattedEntityName(entity.Name) + " ");
                output.AppendLine("			");
                output.AppendLine("} //namespace " + _namespace + ".Entities." + _entityType + "." + GetSchemaName(entity.Schema));
            }

        }
        SaveOutputToFile("Entities.Programmability.cs", output, true);
        output = new StringBuilder();
    }

    private void BuildEntitiesMappedProcedures()
    {
        StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Linq;");
        output.AppendLine("using System.Text;");



        foreach (MappedStoredProcedure mappedStoredProcedure in _generationProject.MappedStoredProcedures)
        {
            String _entityName = mappedStoredProcedure.Name.Split('.')[1].ToString();
            String _entitySchemaName = mappedStoredProcedure.Name.Split('.')[0].ToString();



            if (mappedStoredProcedure.ResultSets.Count > 0)
            {
                output.AppendLine("		namespace " + _generationProject.Namespace + ".Entities.Procedures. " + _entitySchemaName + " {");
                if (_generationProject.SerializeEntitiesClases)
                    output.AppendLine("			[Serializable()]                         //");
                output.AppendLine("			[DataItemAttributeSchemaName(" + System.Convert.ToChar(34) + _entitySchemaName + System.Convert.ToChar(34) + ")]  // Database Schema Name");
                output.AppendLine("			[DataItemAttributeObjectName(" + System.Convert.ToChar(34) + _entityName + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + _entityName + System.Convert.ToChar(34) + ")]");
                output.AppendLine("			[DataItemAttributeObjectType(DataItemAttributeObjectType.ObjectTypeEnum.Procedure)]");
                //output.AppendLine("			public class " + _entityName + " : IMappedProcedure // ResultContainer");
                output.AppendLine("			public class " + _entityName + " : IDataItem // ResultContainer");
                output.AppendLine("			{");

                foreach (MappedStoredProcedure.ResultSet mappedProcedureResultset in mappedStoredProcedure.ResultSets)
                {
                    output.AppendLine("			public class " + mappedProcedureResultset.Name + " : IDataItem");
                    output.AppendLine("				        {");
                    output.AppendLine("				public class ColumnNames");
                    output.AppendLine("				{");
                    foreach (MappedStoredProcedure.ProcedureField procedureField in mappedProcedureResultset.ProcedureFields)
                    {
                        output.AppendLine("					public const string " + procedureField.Name + " = " + System.Convert.ToChar(34) + procedureField.Name + System.Convert.ToChar(34) + ";");
                    }
                    output.AppendLine("				}");
                    output.AppendLine("				public enum FieldEnum : int");
                    output.AppendLine("                {");
                    int columnCount = 0;
                    foreach (MappedStoredProcedure.ProcedureField procedureField in mappedProcedureResultset.ProcedureFields)
                    {
                        columnCount += 1;
                        if (columnCount == mappedProcedureResultset.ProcedureFields.Count)
                        {
                            output.AppendLine("					" + procedureField.Name);
                        }
                        else
                        {
                            output.AppendLine("					" + procedureField.Name + ",");
                        }
                    }

                    output.AppendLine("				}");
                    output.AppendLine("	               /// <summary>");
                    output.AppendLine("                /// Parameterless Constructor");
                    output.AppendLine("	               /// <summary>");
                    output.AppendLine("                public " + mappedProcedureResultset.Name + "()");
                    output.AppendLine("                {}");
                    foreach (MappedStoredProcedure.ProcedureField procedureField in mappedProcedureResultset.ProcedureFields)
                    {
                        output.AppendLine("             [DataItemAttributeFieldName(" + System.Convert.ToChar(34) + procedureField.Name + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + GetFormattedEntityName(procedureField.Name) + System.Convert.ToChar(34) + ")]");
                        if (procedureField.Name.Equals("Name"))
                            output.AppendLine("             [PropertyAttribute(PropertyAttribute.PropertyAttributeEnum.Display)] //Id Display Default");
                        output.AppendLine("             public " + procedureField.DataType + (procedureField.IsNullable == true ? "?" : string.Empty)
                            + " " + procedureField.Name + " { get; set; }");
                    }
                    output.AppendLine("     }");
                }
                output.AppendLine("     }");
                output.AppendLine("     }");
            }
            //output.AppendLine("     }");
        }

        SaveOutputToFile("Entities.MappedProcedures.cs", output, true);
        output = new StringBuilder();
    }
    private void BuildBusinessMappedProcedures()
    {
        StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Linq;");
        output.AppendLine("using System.Text;");



        foreach (MappedStoredProcedure mappedStoredProcedure in _generationProject.MappedStoredProcedures)
        {
            String _entityName = mappedStoredProcedure.Name.Split('.')[1].ToString();
            String _entitySchemaName = mappedStoredProcedure.Name.Split('.')[0].ToString();
            if (mappedStoredProcedure.ResultSets.Count > 0)
            {
                output.AppendLine("		namespace " + _generationProject.Namespace + ".Business.Procedures." + _entitySchemaName + " {");
                output.AppendLine("	    /// <summary>");
                output.AppendLine("	    /// " + _entityName);
                output.AppendLine("	    /// </summary>");
                output.AppendLine("		public class " + _entityName + " : " + "MappedProcedureDataHandler");
                output.AppendLine("		{");
                output.AppendLine("         private List<List<IDataItem>> _result = new List<List<IDataItem>>();");
                output.AppendLine("         private List<IDataItem> _dataItems = new List<IDataItem>();");
                output.AppendLine("         public MappedResult MappedResultSet = new MappedResult();");


                output.AppendLine("            public " + _entityName + "() : base()");
                output.AppendLine("            {");
                foreach (MappedStoredProcedure.ResultSet resultSet in mappedStoredProcedure.ResultSets)
                    output.AppendLine("                 _dataItems.Add(new Entities.Procedures." + _entitySchemaName + "." + _entityName + "." + resultSet.Name + "());");
                output.AppendLine("            }");

                string definedColumnList = string.Empty;
                string columnList = string.Empty;
                foreach (MappedStoredProcedure.ProcedureParameter parameter in mappedStoredProcedure.ProcedureParameters)
                {
                    definedColumnList += parameter.DataType.ToString() + (parameter.DataType.ToString() == "String" ? "" : "?") + " " + parameter.Name + ", ";
                    //columnList += parameter.Name + ", ";
                    columnList += " new ParameterItemValue(" + System.Convert.ToChar(34) + GetFormattedEntityName(parameter.Name).Replace("@", "") + System.Convert.ToChar(34) + ", " + GetFormattedEntityName(parameter.Name).Replace("@", "") + "),";
                }
                if (definedColumnList.Length > 0)
                    definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 2);
                if (columnList.Length > 0)
                    columnList = columnList.Substring(0, columnList.Length - 1);


                string newKeyword = string.Empty;
                if (mappedStoredProcedure.ProcedureParameters.Count != 1)
                    newKeyword = string.Empty;//"new";

                output.AppendLine("            public List<List<IDataItem>> Items(" + definedColumnList + ")");
                output.AppendLine("            {");
                output.AppendLine("                MappedProcedureDataHandler dh = new MappedProcedureDataHandler((IDataItem)new Entities.Procedures." + _entitySchemaName + "." + _entityName + "(), _dataItems);");
                //output.AppendLine("                MappedProcedureDataHandler dh =  new  MappedProcedureDataHandler(_result);");
                //output.AppendLine("                _result = dh.Items(_result);");
                //output.AppendLine("                _result = dh.Items(" + columnList + ");");
                output.AppendLine("                _result = dh.Items(new List<ParameterItemValue> {" + columnList + "});");


                int resultIndex = 0;
                foreach (MappedStoredProcedure.ResultSet resultSet in mappedStoredProcedure.ResultSets)
                {
                    output.AppendLine("                 MappedResultSet." + resultSet.Name + " = _result[" + resultIndex.ToString() + "].Cast<Entities.Procedures." + _entitySchemaName + "." + _entityName + "." + resultSet.Name + ">().ToList<Entities.Procedures." + _entitySchemaName + "." + _entityName + "." + resultSet.Name + ">();");
                    resultIndex += 1;
                }

                output.AppendLine("                return _result;");
                output.AppendLine("             }");
                output.AppendLine("             public List<List<IDataItem>> Resultset");
                output.AppendLine("             {");
                output.AppendLine("                 get { return _result; }");
                output.AppendLine("             }");

                output.AppendLine("             public class MappedResult");
                output.AppendLine("             {");

                foreach (MappedStoredProcedure.ResultSet resultSet in mappedStoredProcedure.ResultSets)
                    output.AppendLine("                 public List<Entities.Procedures." + _entitySchemaName + "." + _entityName + "." + resultSet.Name + "> " + resultSet.Name + "= new List<Entities.Procedures." + _entitySchemaName + "." + _entityName + "." + resultSet.Name + ">();");

                output.AppendLine("             }");
                output.AppendLine("        }// class ");

                output.AppendLine("     }");
            }

        }

        SaveOutputToFile("Business.MappedProcedures.cs", output, true);
        output = new StringBuilder();
    }


    #endregion
    #region Private Methods
    private static string GetEntityType(MyMeta.IProcedure entity)
    {
        String returnValue = "Procedures"; // Assumes "Procedures" by now
        String procedureText = entity.ProcedureText.Replace(" ", "");

        if (procedureText.ToUpper().Contains("CREATEPROCEDURE"))
        {
            returnValue = "Procedures";
        }
        if (procedureText.ToUpper().Contains("CREATEFUNCTION"))
        {
            returnValue = "Functions";
        }
        return returnValue;
    }

    private string GetSchemaName(string schemaName)
    {
        if (schemaName.Trim().Equals(String.Empty))
            return "DELETE_SCHEMA";
        else
            return schemaName;
    }

    private bool HasPrimaryKey(MyMeta.ITable table)
    {
        bool returnValue = false;

        foreach (MyMeta.IColumn item in table.Columns)
        {
            if (item.IsInPrimaryKey)
            {
                returnValue = true;
                break;
            }
        }
        return returnValue;
    }
    private string GetPrimaryKeyName(MyMeta.ITable table)
    {
        string returnValue = string.Empty;

        foreach (MyMeta.IColumn item in table.Columns)
        {
            if (item.IsInPrimaryKey)
            {
                returnValue = GetFormattedEntityName(item.Name);
                break;
            }
        }
        return returnValue;
    }
    private int GetPrimaryKeyCount(MyMeta.ITable table)
    {
        int returnValue = 0;

        foreach (MyMeta.IColumn item in table.Columns)
        {
            if (item.IsInPrimaryKey)
            {
                returnValue += 1;
            }
        }
        return returnValue;
    }

    private string GetDbTarget(MyMeta.dbDriver driver)
    {
        string returnvalue = string.Empty;

        //switch (driver)
        //{
        //    case MyMeta.dbDriver.SQL:
        //        returnvalue = "SqlClient";
        //        break;
        //    case MyMeta.dbDriver.SQLite:
        //        returnvalue = "SQLite.NET v3.x";
        //        break;
        //    case MyMeta.dbDriver.Access:
        //        returnvalue = "DbType";
        //        break;
        //    case MyMeta.dbDriver.Firebird:
        //        returnvalue = "FirebirdSql";
        //        break;
        //}
        returnvalue = System.Configuration.ConfigurationSettings.AppSettings[driver.ToString() + "DbTarget"];
        return returnvalue;
    }
    private string GetLanguage(MyMeta.dbDriver driver)
    {
        string returnvalue = string.Empty;

        //switch (driver)
        //{
        //    case MyMeta.dbDriver.SQL:
        //        returnvalue = "C# Types";
        //        break;
        //    case MyMeta.dbDriver.SQLite:
        //        returnvalue = "C# (SQLite v3.x)";
        //        break;
        //    case MyMeta.dbDriver.Access:
        //        returnvalue = "C# Types";
        //        break;
        //    case MyMeta.dbDriver.Firebird:
        //        returnvalue = "C#";
        //        break;
        //}
        returnvalue = System.Configuration.ConfigurationSettings.AppSettings[driver.ToString() + "Language"];
        return returnvalue;
    }



    #endregion
    #region Save File Methods
    private void SaveOutputToFile(string fileName, System.Text.StringBuilder output, bool overWrite)
    {
        try
        {
            if (!_workingdir.EndsWith("\\"))
                _workingdir += "\\";
            string filePath = _workingdir + fileName;
            string fileDirectory = System.IO.Path.GetDirectoryName(filePath);


            if (!System.IO.Directory.Exists(fileDirectory))
                System.IO.Directory.CreateDirectory(fileDirectory);

            if (!System.IO.File.Exists(filePath) || overWrite == true)
            {


                System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath);
                sw.Write(output.ToString().Replace(".DELETE_SCHEMA", "").Replace("DELETE_SCHEMA", ""));
                sw.Flush();
                sw.Close();
                if (OnFileGenerated != null)
                {
                    OnFileGenerated(_workingdir + fileName);
                }
            }
        }
        catch (Exception ex)
        {
            if (OnException != null)
            {
                OnException(ex);
            }
        }

    }
    #endregion
    #region Framework Base Files
    private void BuildDataField()
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");

        output.AppendLine("public class GenericDataField : IDataField");
        output.AppendLine("{");
        output.AppendLine("    public GenericDataField(string name, string value)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("public class DataFieldDefinition");
        output.AppendLine("{");
        output.AppendLine("    public DataFieldDefinition(String name, String frameworkName,String dataTypeName, bool isPk,bool isAuto, bool isKey, bool isFk, bool isDp,bool isNone,bool exclude, bool isComputed)");
        output.AppendLine("    {");
        output.AppendLine("        Name = name;");
        output.AppendLine("        FrameworkName = frameworkName;");
        output.AppendLine("        DataTypeName = dataTypeName;");
        output.AppendLine("        IsPk = isPk;");
        output.AppendLine("        IsAuto = isAuto;");
        output.AppendLine("        IsKey = isKey;");
        output.AppendLine("        IsFk = isFk;");
        output.AppendLine("        IsDp = isDp;");
        output.AppendLine("        IsNone = isNone;");
        output.AppendLine("        Exclude = exclude;");
        output.AppendLine("        IsComputed = isComputed;");
        output.AppendLine("    }");
        output.AppendLine("    public String Name { get; set; }");
        output.AppendLine("    public String FrameworkName { get; set; }");
        output.AppendLine("    public String DataTypeName { get; set; }");
        output.AppendLine("    public bool IsPk { get; set; }");
        output.AppendLine("    public bool IsAuto { get; set; }");
        output.AppendLine("    public bool IsKey { get; set; }");
        output.AppendLine("    public bool IsFk { get; set; }");
        output.AppendLine("    public bool IsDp { get; set; }");
        output.AppendLine("    public bool IsNone { get; set; }");
        output.AppendLine("    public bool Exclude { get; set; }");
        output.AppendLine("    public bool IsComputed { get; set; }");
        output.AppendLine("}");

        SaveOutputToFile("DataField.cs", output, true);
        output = new StringBuilder();
    }

    private void GetHeaderInfo(System.Text.StringBuilder output)
    {
        if (_generationProject.IncludeGenerationInformation)
        {
            output.AppendLine(" // Generated by OpenORM.NET Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            output.AppendLine(" // Generation date: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            string architecture = System.Environment.Is64BitOperatingSystem == true ? "x64" : "x86";
            output.AppendLine(" // User: " + System.Environment.UserName + " - Workstation: " + System.Environment.MachineName + " - OS Version: " + System.Environment.OSVersion + " (" + architecture + ")");
            output.AppendLine(" // Warning: This file should not be edited.");
        }

    }
    private string GetViewColumnDescription(IColumn column, string connectionString)
    {
        string sql = @"SELECT S.name as [SchemaName], O.name AS [ViewName],C.Name As ColumnName, ep.value AS [Value]
                    FROM sys.extended_properties EP
                    INNER JOIN sys.all_objects O ON ep.major_id = O.object_id 
                    INNER JOIN sys.schemas S on O.schema_id = S.schema_id
                    INNER JOIN sys.columns AS c ON ep.major_id = c.object_id AND ep.minor_id = c.column_id
                    WHERE s.name = '" + column.View.Schema + "' AND O.type = 'V' AND ep.name = 'Description'";
        sql += "AND c.Name = '" + column.Name + "'";

        ADODB.Recordset recordset = null;
        ADODB.Connection connection = new ADODB.Connection();
        ADODB.Command command = new ADODB.Command();
        command.CommandText = sql;
        command.CommandType = ADODB.CommandTypeEnum.adCmdText;
        command.CommandTimeout = 30;

        connection.Open(connectionString);
        command.ActiveConnection = connection;
        object rows;
        recordset = command.Execute(out rows);
        string res = string.Empty;
        while (!recordset.EOF)
        {
            res = recordset.Fields["Value"].Value.ToString();
            recordset.MoveNext();
        }
        return res;
    }
    private void BuildDataHandlerBaseNET40(string nameSpace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Data;");
        output.AppendLine("using System.Configuration;");
        output.AppendLine("using System.Collections;");
        output.AppendLine("using System.Reflection;");

        output.AppendLine("public class DataHandlerBase: IDataHandler, IDisposable");
        output.AppendLine("{");
        output.AppendLine("    #region Local Variables");
        output.AppendLine("    protected string _commandText = null;");
        output.AppendLine("    protected string _fieldList = string.Empty;");
        output.AppendLine("    protected string _valueList = string.Empty;");
        output.AppendLine("    protected string _adoNetAssemblyName = string.Empty;");
        output.AppendLine("    protected string _adoNetConnectionTypeName = string.Empty;");
        output.AppendLine("    protected string _parameterPrefix = string.Empty;");
        output.AppendLine("    protected string _connectionString = string.Empty;");
        output.AppendLine("    protected Int32 _commandTimeout = 30;");
        output.AppendLine("    /// List of parameters to hold DataItem Values information");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected List<ParameterItemValue> _parameterizedValues = new List<ParameterItemValue>();");
        output.AppendLine("    #endregion");
        output.AppendLine("    #region Local interfaces");
        output.AppendLine("        protected IDataItem _dataItem = null;");
        output.AppendLine("        protected List<IDataItem> _itemList = new List<IDataItem>();");
        output.AppendLine("    #endregion");
        output.AppendLine("    #region ADO.NET Interfaces");
        output.AppendLine("        protected IDbConnection _connection = null;");
        output.AppendLine("        protected IDbCommand _command = null;");
        output.AppendLine("        protected IDbTransaction _transaction { get; set; }");
        output.AppendLine("        protected IDataReader _datareader { get; set; }");
        output.AppendLine("    #endregion");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Represents Where condition list");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected WhereParameterBase WhereParameter {get;set;}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Represents Order By parameters");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected OrderByParameterBase OrderByParameter  {get;set;}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Represents Group By parameters");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected GroupByParameterBase GroupByParameter  {get;set;}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Represents aggregate parameter");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected AggregateParameterBase AggregateParameter {get;set;}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Reflection cache data");
        output.AppendLine("    /// </summary>");
        output.AppendLine("	#region Reflection data");
        output.AppendLine("		protected Type _dataItemType = null;");
        output.AppendLine("		private PropertyInfo[] _properties = null;");
        output.AppendLine("		private System.Attribute _objectSchemaTypeAttribute = null;");
        output.AppendLine("		private System.Attribute _objectNameTypeAttribute = null;");
        output.AppendLine("	#endregion");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// These are field definitions stores as Custom Attributes in Entities");
        output.AppendLine("    /// and are loaded with Reflection");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected List<DataFieldDefinition> _dataFieldDefinitions = new List<DataFieldDefinition>();");
        output.AppendLine("    private bool disposedValue;");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// No constructor");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public DataHandlerBase()");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        WhereParameter = new WhereParameterBase();");
        output.AppendLine("        OrderByParameter = new OrderByParameterBase();");
        output.AppendLine("        GroupByParameter = new GroupByParameterBase();");
        output.AppendLine("        getAssembliesNames();");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Constructor with IDataItem");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public DataHandlerBase(IDataItem dataItem)");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        WhereParameter = new WhereParameterBase();");
        output.AppendLine("        OrderByParameter = new OrderByParameterBase();");
        output.AppendLine("        GroupByParameter = new GroupByParameterBase();");
        output.AppendLine("        _dataItem = dataItem;");
        output.AppendLine("        AnalizeIDataItem();");
        output.AppendLine("        getAssembliesNames();");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Constructor to handle Transaction by reflection");
        output.AppendLine("    /// IDataHandler has Connection and dataItem intrinsic information");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    public DataHandlerBase(IDataHandler dataHandler)");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("             WhereParameter = new WhereParameterBase();");
        output.AppendLine("			    OrderByParameter = new OrderByParameterBase();");
        output.AppendLine("             GroupByParameter = new GroupByParameterBase();");
        output.AppendLine("            //_transaction = dataHandler.GetTransaction(); // Gets IDataHandler transaction");
        output.AppendLine("			//_dataItem = dataHandler._dataItem; // Gets IDataHandler IdataItem");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception)");
        output.AppendLine("        {");
        output.AppendLine("            throw (new Exception(Constants.ERROR_CONSTRUCTOR));");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Returns working transaction");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    IDbTransaction IDataHandler.GetTransaction()");
        output.AppendLine("    {");
        output.AppendLine("        return _transaction;");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Returns working connection");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    IDbConnection IDataHandler.GetConnection()");
        output.AppendLine("    {");
        output.AppendLine("        return _connection;");
        output.AppendLine("    }");
        output.AppendLine("	/////////////// Este bloque es para analizar y cachear los datos del IDataItem //////////////////////");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Gets IdataItem reflection information and set corresponding variables.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>	");
        output.AppendLine("   private void AnalizeIDataItem()");
        output.AppendLine("    {");
        output.AppendLine("        if (_dataItemType == null)");
        output.AppendLine("        {");
        output.AppendLine("            _dataItemType = _dataItem.GetType();");
        output.AppendLine("            _properties = _dataItemType.GetProperties();");
        output.AppendLine("            //Gets the Schema Name of the IDataItem");
        output.AppendLine("            Type DataItemAttributeSchemaNameType = typeof(DataItemAttributeSchemaName);");
        output.AppendLine("            //Gets the Object Name of the IDataItem");
        output.AppendLine("            Type DataItemAttributeObjectNameType = typeof(DataItemAttributeObjectName);");

        output.AppendLine("            Type PropertyAttribute = typeof(PropertyAttribute);");
        output.AppendLine("            Type PropertyAttributeEnum = typeof(PropertyAttribute.PropertyAttributeEnum);");

        output.AppendLine("            for (int i = 0; i < (((System.Reflection.MemberInfo)(_dataItemType))).GetCustomAttributes(false).Length; i++)");
        output.AppendLine("            {");
        output.AppendLine("                switch ((((System.Reflection.MemberInfo)(_dataItemType))).GetCustomAttributes(false)[i].ToString())");
        output.AppendLine("                {");
        output.AppendLine("                    case Constants.SYSTEM_SERIALIZABLEATTRIBUTE:");
        output.AppendLine("                        break;");
        output.AppendLine("                    case Constants.DATAITEMATTRIBUTESCHEMANAME:");
        output.AppendLine("                        _objectSchemaTypeAttribute = (DataItemAttributeSchemaName)(((System.Reflection.MemberInfo)(_dataItemType))).GetCustomAttributes(false)[i];");
        output.AppendLine("                        break;");
        output.AppendLine("                    case Constants.DATAITEMATTRIBUTEOBJECTTYPE:");
        output.AppendLine("                        break;");
        output.AppendLine("                    case Constants.DATAITEMATTRIBUTEOBJECTNAME:");
        output.AppendLine("                        _objectNameTypeAttribute = (DataItemAttributeObjectName)(((System.Reflection.MemberInfo)(_dataItemType))).GetCustomAttributes(false)[i];");
        output.AppendLine("                        break;");
        output.AppendLine("                    default:");
        output.AppendLine("                        break;");
        output.AppendLine("                }");
        output.AppendLine("            }");

        output.AppendLine("            foreach (PropertyInfo info in _properties)");
        output.AppendLine("            {");
        output.AppendLine("                bool isPk = false;");
        output.AppendLine("                bool isAuto = false;");
        output.AppendLine("                bool isKey = false;");
        output.AppendLine("                bool isFk = false;");
        output.AppendLine("                bool isDp = false;");
        output.AppendLine("                bool isNone = false;");
        output.AppendLine("                bool exclude = false;");
        output.AppendLine("                bool isComputed = false;");
        output.AppendLine("                String fieldName = String.Empty;");
        output.AppendLine("                String fieldFrameworkName = String.Empty;");

        output.AppendLine("                int customAttributesCount = info.GetCustomAttributes(false).GetLength(0);");

        output.AppendLine("                for (int i = 0; i < customAttributesCount; i++)");
        output.AppendLine("                {");
        output.AppendLine("                    if ( info.GetCustomAttributes(false)[i].GetType().Name.Equals(Constants.PROPERTYATTRIBUTE))");
        output.AppendLine("                    {");
        output.AppendLine("                        switch (((PropertyAttribute)(info.GetCustomAttributes(false)[i]))._propertyAttribute)");
        output.AppendLine("                        {");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Pk:");
        output.AppendLine("                                isPk = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Auto:");
        output.AppendLine("                                isAuto = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Key:");
        output.AppendLine("                                isKey = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Fk:");
        output.AppendLine("                                isFk = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Display:");
        output.AppendLine("                                isDp = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.None:");
        output.AppendLine("                                isNone = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Exclude:");
        output.AppendLine("                                exclude = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Computed:");
        output.AppendLine("                                isComputed = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            default:");
        output.AppendLine("                                break;");
        output.AppendLine("                        }");
        output.AppendLine("                    }");
        output.AppendLine("                    fieldName = info.Name;");
        output.AppendLine("                    if (info.GetCustomAttributes(false)[i].GetType().Name.Equals(Constants.DATAITEMATTRIBUTEFIELDNAME))");
        output.AppendLine("                    {");
        output.AppendLine("                        fieldName = ((DataItemAttributeFieldName)(info.GetCustomAttributes(false)[i])).FieldName;");
        output.AppendLine("                        fieldFrameworkName = ((DataItemAttributeFieldName)(info.GetCustomAttributes(false)[i])).FieldFrameworkName;");
        output.AppendLine("                    }");
        output.AppendLine("                }");

        output.AppendLine("                 if(!exclude)");
        output.AppendLine("                     _dataFieldDefinitions.Add(new DataFieldDefinition(fieldName, fieldFrameworkName,(info.PropertyType).FullName, isPk,isAuto, isKey, isFk, isDp,isNone,exclude, isComputed));");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("    }");

        output.AppendLine("	/// <summary>");
        output.AppendLine("    /// Returns [Schema name].[Object name] eg.: [dbo].[User]");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("	protected String GetFullDataEntityName()");
        output.AppendLine("	{");
        output.AppendLine("        AnalizeIDataItem();");
        output.AppendLine("        if (((DataItemAttributeSchemaName)_objectSchemaTypeAttribute).schemaName.Equals(String.Empty))");
        output.AppendLine("		        return  " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + ((DataItemAttributeObjectName)_objectNameTypeAttribute).ObjectName + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + ";");
        output.AppendLine("        else ");
        output.AppendLine("		        return  " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + ((DataItemAttributeSchemaName)_objectSchemaTypeAttribute).schemaName + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " +");
        output.AppendLine("		         " + System.Convert.ToChar(34) + ".[" + System.Convert.ToChar(34) + " + ((DataItemAttributeObjectName)_objectNameTypeAttribute).ObjectName + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + ";");
        output.AppendLine("	}");
        output.AppendLine("	/// <summary>");
        output.AppendLine("    /// Returns field list separated by colon eg.: " + System.Convert.ToChar(34) + "[Id],[Name],[Description]" + System.Convert.ToChar(34) + "");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("	protected String GetFieldList(bool includePk, bool includeComputed)");
        output.AppendLine("	{");
        output.AppendLine("		string result = string.Empty;");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("		{");
        output.AppendLine("            if ((!field.IsPk && !field.IsComputed)) || (field.IsPk && !field.IsAuto) || (includePk && field.IsPk) || (includeComputed && field.IsComputed))");
        output.AppendLine("                result += " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + field.Name + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("		}");
        output.AppendLine("		return result.Substring(0, result.Length - 1);	");
        output.AppendLine("	}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Returns parametrized values list : @Id, @Name,@Description");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "includePk" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    protected String GetParameterizedValuesList(bool includePk, bool includeComputed)");
        output.AppendLine("    {");
        output.AppendLine("        string result = string.Empty;");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("        {");
        output.AppendLine("            if ((!field.IsPk && !field.IsComputed) || (field.IsPk && !field.IsAuto) || (includePk && field.IsPk) || (includeComputed && field.IsComputed))");
        output.AppendLine("                result += _parameterPrefix + field.Name + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("        }");
        output.AppendLine("        return result.Substring(0, result.Length - 1);");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Builds Parameter Values list eg.: " + System.Convert.ToChar(34) + "@Id,@Name'" + System.Convert.ToChar(34) + " for select based on parameters");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    protected void BuildParameterValuesList(WhereParameterBase item)");
        output.AppendLine("    {");
        output.AppendLine("        _parameterizedValues.Clear();");

        output.AppendLine("        foreach (ParameterItem field in item._whereParameters)");
        output.AppendLine("        {");
        output.AppendLine("            if(field.IsFieldParameter)");
        output.AppendLine("            {");
        output.AppendLine("                 foreach (ParameterItemValue value in field.Values)");
        output.AppendLine("                 {");
        output.AppendLine("                     _parameterizedValues.Add(new ParameterItemValue(value.Name.Replace(Constants.INPUT_PARAMETER, _parameterPrefix), value.Value));");
        output.AppendLine("                 }");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("    }	");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Builds Parameter Values list eg.: " + System.Convert.ToChar(34) + "@Id,@Name'" + System.Convert.ToChar(34) + " for select based on IDataItem parameters");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    protected void BuildIDataItemParameterValuesList(IDataItem item,bool includeNonPk = false)");
        output.AppendLine("    {");
        output.AppendLine("        _parameterizedValues.Clear();");
        output.AppendLine("        _dataItem = item;");
        output.AppendLine("        AnalizeIDataItem();");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("        {");
        output.AppendLine("            if(field.IsPk)");
        output.AppendLine("            {");
        output.AppendLine("                var pkValue = item.GetType().GetProperty(field.Name).GetValue(item, null);");
        output.AppendLine("                _parameterizedValues.Add(new ParameterItemValue(field.Name.Replace(Constants.INPUT_PARAMETER, _parameterPrefix), pkValue));");
        output.AppendLine("            }");
        output.AppendLine("            if (!field.IsPk && includeNonPk)");
        output.AppendLine("            {");
        output.AppendLine("                var pkValue = item.GetType().GetProperty(field.Name).GetValue(item, null);");
        output.AppendLine("                _parameterizedValues.Add(new ParameterItemValue(field.Name.Replace(Constants.INPUT_PARAMETER, _parameterPrefix), pkValue));");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("    }	");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Builds Parameter Values list eg.: " + System.Convert.ToChar(34) + "@Id,@Name'" + System.Convert.ToChar(34) + " for Insert");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("	protected void BuildParameterValuesList(IDataItem item)");
        output.AppendLine("	{");
        output.AppendLine("        _parameterizedValues.Clear();");
        output.AppendLine("       ");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("		{");
        output.AppendLine("            if (!field.IsPk || (field.IsPk && !field.IsAuto))");
        output.AppendLine("            {");
        output.AppendLine("                var memberData = item.GetType().GetProperty(field.Name).GetValue(item, null);");
        output.AppendLine("                _parameterizedValues.Add(new ParameterItemValue( _parameterPrefix + field.Name, memberData));");
        output.AppendLine("            }");
        output.AppendLine("		}");
        output.AppendLine("		");
        output.AppendLine("	}	");
        output.AppendLine("	/// <summary>");
        output.AppendLine("    /// Returns set list separated by colon eg.: " + System.Convert.ToChar(34) + "[Id] = @Id,[Name] = @Name,[Description] = @Description" + System.Convert.ToChar(34) + "");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("	");
        output.AppendLine("	protected String GetSetList(IDataItem item)");
        output.AppendLine("	{");
        output.AppendLine("		string result = string.Empty;");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("        {");
        output.AppendLine("            if (!field.IsPk && !field.isComputed)");
        output.AppendLine("            {");
        output.AppendLine("				result += " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + field.Name + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + " = " + System.Convert.ToChar(34) + " + _parameterPrefix + field.Name + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("		return result.Substring(0, result.Length - 1);	");
        output.AppendLine("	}");
        output.AppendLine("	/// <summary>");
        output.AppendLine("    /// Returns keys list separated by colon eg.: " + System.Convert.ToChar(34) + "[Id] = @Id AND [Code] = @Code" + System.Convert.ToChar(34) + "");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    protected String GetKeysList(IDataItem item)");
        output.AppendLine("	{");
        output.AppendLine("		string result = String.Empty;");
        output.AppendLine("		foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("		{");
        output.AppendLine("            if (field.IsPk || field.IsKey)");
        output.AppendLine("			{");
        output.AppendLine("				result += " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + field.Name + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + " = " + System.Convert.ToChar(34) + " + _parameterPrefix + field.Name + Constants.SQL_AND;");
        output.AppendLine("			}");
        output.AppendLine("		}");
        output.AppendLine("		return result.Substring(0, result.Length - 5);	");
        output.AppendLine("	}");

        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Begins a Transaction.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    internal virtual IDbTransaction BeginTransaction()");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            if (_connection == null)");
        output.AppendLine("            {");
        output.AppendLine("                GetConnection();");
        output.AppendLine("                _transaction = _connection.BeginTransaction();");
        output.AppendLine("                return _transaction;");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                throw (new Exception(Constants.ERROR_TRANSACTION_ALREADY_OPENED));");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Ends a Transaction with commit  or rollback passed as boolean.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "commit" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "validateOpenedTransaction" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    internal virtual bool EndTransaction(bool commit, bool validateOpenedTransaction = true)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            if (_connection == null && validateOpenedTransaction == true)");
        output.AppendLine("            {");
        output.AppendLine("                throw (new Exception(Constants.ERROR_TRANSACTION_NOT_OPENED_YET));");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                if (commit)");
        output.AppendLine("                {");
        output.AppendLine("                    _transaction.Commit();");
        output.AppendLine("                }");
        output.AppendLine("                else");
        output.AppendLine("                {");
        output.AppendLine("                    _transaction.Rollback();");
        output.AppendLine("                }");
        output.AppendLine("                _connection.Close();");
        output.AppendLine("                _connection = null;");
        output.AppendLine("                _transaction = null;");
        output.AppendLine("                return true;");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected void GetConnection()");
        output.AppendLine("    {");
        output.AppendLine("        if (_transaction != null)");
        output.AppendLine("        {");
        output.AppendLine("             _connection = _transaction.Connection;");
        output.AppendLine("        }");
        output.AppendLine("        else");
        output.AppendLine("        {");
        output.AppendLine("             if (_connection == null)");
        output.AppendLine("             {");
        output.AppendLine("                 getAssembliesNames();");
        output.AppendLine("                 // Create a  connection of the type");
        output.AppendLine("                 _connection = (IDbConnection)Activator.CreateInstance(_adoNetAssemblyName, _adoNetConnectionTypeName).Unwrap();");
        output.AppendLine("                 // Retrieve the Connection String    ");
        if (_generationProject.UsesEncription)
            output.AppendLine("                 _connection.ConnectionString = " +
                nameSpace + ".Crypto.Decrypt(ConfigurationHandler.ConnectionString ,ConfigurationHandler.PasswordKey);");
        else
            output.AppendLine("                 _connection.ConnectionString = ConfigurationHandler.ConnectionString;");

        output.AppendLine("                 _connection.Open();");
        output.AppendLine("              }");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected IDbCommand GetCommand(string query, IDbConnection connection, IDbTransaction transaction, CommandType commandType = CommandType.Text)");
        output.AppendLine("    {");
        output.AppendLine("        getAssembliesNames();");
        output.AppendLine("        IDbCommand newCommand = connection.CreateCommand();");
        output.AppendLine("        newCommand.CommandText = query;");
        output.AppendLine("        newCommand.Connection = connection;");
        output.AppendLine("        newCommand.Transaction = transaction;");
        output.AppendLine("        newCommand.CommandType = commandType;");
        output.AppendLine("        newCommand.CommandTimeout = _commandTimeout;");
        output.AppendLine("        foreach (ParameterItemValue parameterValue in _parameterizedValues)");
        output.AppendLine("		{");
        output.AppendLine("			    IDbDataParameter parameter = newCommand.CreateParameter();");
        output.AppendLine("			    parameter.ParameterName = parameterValue.Name;");
        output.AppendLine("			    parameter.Value = parameterValue.Value == null ? DBNull.Value : parameterValue.Value;");
        output.AppendLine("			    newCommand.Parameters.Add(parameter);");
        output.AppendLine("		}		");
        output.AppendLine("        return newCommand;");
        output.AppendLine("    }");
        output.AppendLine("    protected void getAssembliesNames()");
        output.AppendLine("    {");
        output.AppendLine("        if (_adoNetAssemblyName.Equals(String.Empty) || _adoNetConnectionTypeName.Equals(String.Empty))");
        output.AppendLine("        {");
        output.AppendLine("             _adoNetAssemblyName = ConfigurationHandler.AdoNetAssemblyName;");
        output.AppendLine("             _adoNetConnectionTypeName = ConfigurationHandler.AdoNetConnectionTypeName;");
        output.AppendLine("             _commandTimeout = ConfigurationHandler.AdoNetCommandTimeout;");
        output.AppendLine("             _parameterPrefix = ConfigurationHandler.ParameterPrefix;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected IDataReader ExecuteReader(String query, IDbTransaction transaction, CommandType commandType = CommandType.Text)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            GetConnection();");
        output.AppendLine("            _command = GetCommand(query, _connection, transaction,commandType);");
        output.AppendLine("		");
        output.AppendLine("            return _command.ExecuteReader();");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception sqlExeption)");
        output.AppendLine("        {");
        output.AppendLine("            throw sqlExeption;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected Int64 ExecuteNonQuery(String query, IDbTransaction transaction,bool addOutputParameter)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            GetConnection();");
        output.AppendLine("            _command = GetCommand(query, _connection, transaction);");
        output.AppendLine("            if (addOutputParameter)");
        output.AppendLine("            {");
        output.AppendLine("                IDbDataParameter outputParam = _command.CreateParameter();");
        output.AppendLine("                outputParam.ParameterName = Constants.NEWID;");
        output.AppendLine("                outputParam.Direction = ParameterDirection.Output;");
        output.AppendLine("                outputParam.Size = Int32.MaxValue;");
        output.AppendLine("                _command.Parameters.Add(outputParam); ");
        output.AppendLine("            }");


        output.AppendLine("            return _command.ExecuteNonQuery();");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception sqlExeption)");
        output.AppendLine("        {");
        output.AppendLine("            throw sqlExeption;");
        output.AppendLine("        }");
        output.AppendLine("        finally");
        output.AppendLine("        {");
        output.AppendLine("            if(_connection.State != ConnectionState.Closed && transaction == null)");
        output.AppendLine("                 _connection.Close();");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected string GetPkName()");
        output.AppendLine("    {");
        output.AppendLine("        string returnValue = string.Empty;");
        output.AppendLine("        foreach (var item in _dataFieldDefinitions)");
        output.AppendLine("        {");
        output.AppendLine("            if (item.IsPk)");
        output.AppendLine("                returnValue =  item.Name;");
        output.AppendLine("        }");
        output.AppendLine("        return returnValue;");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Maps DataReader data to DataItem entity with reflection.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "dr" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    protected virtual List<IDataItem> MapDataReaderToDataItem<IDataItem>(IDataReader dr, IDataItem dataItem) //where IDataItem : new()");
        output.AppendLine("    {");
        output.AppendLine("        Type dataItemType = dataItem.GetType();");
        output.AppendLine("        PropertyInfo[] properties = dataItemType.GetProperties(); ");
        output.AppendLine("        Hashtable hashtable = new Hashtable();");
        output.AppendLine("        List<IDataItem> entities = new List<IDataItem>();");
        output.AppendLine("        foreach (PropertyInfo info in properties)");
        output.AppendLine("        {");
        output.AppendLine("            hashtable[info.Name] = info;");
        output.AppendLine("        }");
        output.AppendLine("        while (dr.Read())");
        output.AppendLine("        {");
        output.AppendLine("            IDataItem newObject = (IDataItem)Activator.CreateInstance(dataItemType, false);");
        output.AppendLine("            for (int index = 0; index < dr.FieldCount; index++)");
        output.AppendLine("            {");
        output.AppendLine("                PropertyInfo info = (PropertyInfo)hashtable[_dataFieldDefinitions[index].Name];");
        output.AppendLine("                if ((info != null) && info.CanWrite)");
        output.AppendLine("                {");
        output.AppendLine("                     if (!dr.GetValue(index).Equals(System.DBNull.Value))");
        output.AppendLine("                         info.SetValue(newObject, dr.GetValue(index), null);");
        output.AppendLine("                }");
        output.AppendLine("            }");
        output.AppendLine("            entities.Add(newObject);");
        output.AppendLine("        }");
        output.AppendLine("        dr.Close();");
        output.AppendLine("        return entities;");
        output.AppendLine("    }");
        output.AppendLine(" ");
        output.AppendLine("    protected virtual void Dispose(bool disposing)");
        output.AppendLine("    {");
        output.AppendLine("        if (!disposedValue)");
        output.AppendLine("        {");
        output.AppendLine("            if (disposing)");
        output.AppendLine("            {");
        output.AppendLine("                // TODO: dispose managed state (managed objects)");
        output.AppendLine("                _itemList = null;");
        output.AppendLine("                _fieldList = null;");
        output.AppendLine("                _dataItem = null;");
        output.AppendLine("            }");
        output.AppendLine(" ");
        output.AppendLine("            // TODO: free unmanaged resources (unmanaged objects) and override finalizer");
        output.AppendLine("            // TODO: set large fields to null");
        output.AppendLine("            _connection = null;");
        output.AppendLine("            _command = null;");
        output.AppendLine("            _transaction = null;");
        output.AppendLine("            _datareader = null;");
        output.AppendLine(" ");
        output.AppendLine("            disposedValue = true;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine(" ");
        output.AppendLine("    void IDisposable.Dispose()");
        output.AppendLine("    {");
        output.AppendLine("        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method");
        output.AppendLine("        Dispose(disposing: true);");
        if( _generationProject.UsesExplicitGarbageCollection)
        {
            output.AppendLine("        " + _generationProject.GarbageCollectionCode);
        }

        output.AppendLine("    }");
        output.AppendLine("}");


        SaveOutputToFile("DataHandlerBase.cs", output, true);
        output = new StringBuilder();
    }

    private void BuildDataHandlerBaseNET45(string nameSpace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Data;");
        output.AppendLine("using System.Configuration;");
        output.AppendLine("using System.Collections;");
        output.AppendLine("using System.Reflection;");

        output.AppendLine("public class DataHandlerBase: IDataHandler, IDisposable");
        output.AppendLine("{");
        output.AppendLine("    #region Local Variables");
        output.AppendLine("    protected string _commandText = null;");
        output.AppendLine("    protected string _fieldList = string.Empty;");
        output.AppendLine("    protected string _valueList = string.Empty;");
        output.AppendLine("    protected string _adoNetAssemblyName = string.Empty;");
        output.AppendLine("    protected string _adoNetConnectionTypeName = string.Empty;");
        output.AppendLine("    protected string _parameterPrefix = string.Empty;");
        output.AppendLine("    protected string _connectionString = string.Empty;");
        output.AppendLine("    protected Int32 _commandTimeout = 30;");
        output.AppendLine("    /// List of parameters to hold DataItem Values information");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    List<ParameterItemValue> _parameterizedValues = new List<ParameterItemValue>();");
        output.AppendLine("    #endregion");
        output.AppendLine("    #region Local interfaces");
        output.AppendLine("        protected IDataItem _dataItem = null;");
        output.AppendLine("        protected List<IDataItem> _itemList = new List<IDataItem>();");
        output.AppendLine("    #endregion");
        output.AppendLine("    #region ADO.NET Interfaces");
        output.AppendLine("        protected IDbConnection _connection = null;");
        output.AppendLine("        protected IDbCommand _command = null;");
        output.AppendLine("        protected IDbTransaction _transaction { get; set; }");
        output.AppendLine("        protected IDataReader _datareader { get; set; }");
        output.AppendLine("    #endregion");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Represents Where condition list");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected WhereParameterBase WhereParameter {get;set;}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Represents Order By parameters");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected OrderByParameterBase OrderByParameter  {get;set;}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Represents aggregate parameter");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected AggregateParameterBase AggregateParameter {get;set;}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Reflection cache data");
        output.AppendLine("    /// </summary>");
        output.AppendLine("	#region Reflection data");
        output.AppendLine("		protected Type _dataItemType = null;");
        output.AppendLine("		private PropertyInfo[] _properties = null;");
        output.AppendLine("		private System.Attribute _objectSchemaTypeAttribute = null;");
        output.AppendLine("		private System.Attribute _objectNameTypeAttribute = null;");
        output.AppendLine("	#endregion");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// These are field definitions stores as Custom Attributes in Entities");
        output.AppendLine("    /// and are loaded with Reflection");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    private List<DataFieldDefinition> _dataFieldDefinitions = new List<DataFieldDefinition>();");
        output.AppendLine("    private bool disposedValue;");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// No constructor");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public DataHandlerBase()");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        WhereParameter = new WhereParameterBase();");
        output.AppendLine("        OrderByParameter = new OrderByParameterBase();");
        output.AppendLine("        getAssembliesNames();");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Constructor with IDataItem");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public DataHandlerBase(IDataItem dataItem)");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        WhereParameter = new WhereParameterBase();");
        output.AppendLine("        OrderByParameter = new OrderByParameterBase();");
        output.AppendLine("        _dataItem = dataItem;");
        output.AppendLine("        AnalizeIDataItem();");
        output.AppendLine("        getAssembliesNames();");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Constructor to handle Transaction by reflection");
        output.AppendLine("    /// IDataHandler has Connection and dataItem intrinsic information");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    public DataHandlerBase(IDataHandler dataHandler)");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("             WhereParameter = new WhereParameterBase();");
        output.AppendLine("			OrderByParameter = new OrderByParameterBase();");
        output.AppendLine("            //_transaction = dataHandler.GetTransaction(); // Gets IDataHandler transaction");
        output.AppendLine("			//_dataItem =  dataHandler._dataItem; // Gets IDataHandler IdataItem");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception)");
        output.AppendLine("        {");
        output.AppendLine("            throw (new Exception(" + System.Convert.ToChar(34) + "DataHandler (New) : Transaction assignment Error." + System.Convert.ToChar(34) + "));");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Returns working transaction");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    IDbTransaction IDataHandler.GetTransaction()");
        output.AppendLine("    {");
        output.AppendLine("        return _transaction;");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Returns working connection");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    IDbConnection IDataHandler.GetConnection()");
        output.AppendLine("    {");
        output.AppendLine("        return _connection;");
        output.AppendLine("    }");
        output.AppendLine("	/////////////// Este bloque es para analizar y cachear los datos del IDataItem //////////////////////");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Gets IdataItem reflection information and set corresponding variables.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>	");
        output.AppendLine("    private void AnalizeIDataItem()");
        output.AppendLine("    {");
        output.AppendLine("        if (_dataItemType == null)");
        output.AppendLine("        {");
        output.AppendLine("            _dataItemType = _dataItem.GetType();");
        output.AppendLine("            _properties = _dataItemType.GetProperties();");
        output.AppendLine("            //Gets the Schema Name of the IDataItem");
        output.AppendLine("            Type DataItemAttributeSchemaNameType = typeof(DataItemAttributeSchemaName);");
        output.AppendLine("            //Gets the Object Name of the IDataItem");
        output.AppendLine("            Type DataItemAttributeObjectNameType = typeof(DataItemAttributeObjectName);");

        output.AppendLine("            Type PropertyAttribute = typeof(PropertyAttribute);");
        output.AppendLine("            Type PropertyAttributeEnum = typeof(PropertyAttribute.PropertyAttributeEnum);");

        output.AppendLine("            _objectSchemaTypeAttribute = _dataItemType.GetCustomAttribute(DataItemAttributeSchemaNameType);");
        output.AppendLine("            _objectNameTypeAttribute = _dataItemType.GetCustomAttribute(DataItemAttributeObjectNameType);");

        output.AppendLine("            foreach (PropertyInfo info in _properties)");
        output.AppendLine("            {");
        output.AppendLine("                bool isPk = false;");
        output.AppendLine("                bool isAuto = false;");
        output.AppendLine("                bool isKey = false;");
        output.AppendLine("                bool isFk = false;");
        output.AppendLine("                bool isDp = false;");
        output.AppendLine("                bool isNone = false;");

        output.AppendLine("                String typeName = (info.PropertyType).FullName;");


        output.AppendLine("                var p = info.GetCustomAttribute(PropertyAttribute);");
        output.AppendLine("                if (p != null)");
        output.AppendLine("                {");
        output.AppendLine("                    isPk = ((PropertyAttribute)(p))._propertyAttribute == global::PropertyAttribute.PropertyAttributeEnum.Pk;");
        output.AppendLine("                    isAuto = ((PropertyAttribute)(p))._propertyAttribute == global::PropertyAttribute.PropertyAttributeEnum.Auto;");
        output.AppendLine("                    isKey = ((PropertyAttribute)(p))._propertyAttribute == global::PropertyAttribute.PropertyAttributeEnum.Key;");
        output.AppendLine("                    isFk = ((PropertyAttribute)(p))._propertyAttribute == global::PropertyAttribute.PropertyAttributeEnum.Fk;");
        output.AppendLine("                    isDp = ((PropertyAttribute)(p))._propertyAttribute == global::PropertyAttribute.PropertyAttributeEnum.Display;");
        output.AppendLine("                    isNone = ((PropertyAttribute)(p))._propertyAttribute == global::PropertyAttribute.PropertyAttributeEnum.None;");

        output.AppendLine("                }");
        output.AppendLine("                _dataFieldDefinitions.Add(new DataFieldDefinition(info.Name, typeName, ispk, isKey, isFk, isDp));");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("	/// <summary>");
        output.AppendLine("    /// Returns [Schema name].[Object name] eg.: [dbo].[User]");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("	protected String GetFullDataEntityName()");
        output.AppendLine("	{");
        output.AppendLine("        AnalizeIDataItem();");
        output.AppendLine("        if (((DataItemAttributeSchemaName)_objectSchemaTypeAttribute).schemaName.Equals(String.Empty))");
        output.AppendLine("		        return  " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + ((DataItemAttributeObjectName)_objectNameTypeAttribute).ObjectName + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + ";");
        output.AppendLine("        else ");
        output.AppendLine("		        return  " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + ((DataItemAttributeSchemaName)_objectSchemaTypeAttribute).schemaName + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " +");
        output.AppendLine("		        " + System.Convert.ToChar(34) + ".[" + System.Convert.ToChar(34) + " + ((DataItemAttributeObjectName)_objectNameTypeAttribute).ObjectName + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + ";");
        output.AppendLine("	}");
        output.AppendLine("	/// <summary>");
        output.AppendLine("    /// Returns field list separated by colon eg.: " + System.Convert.ToChar(34) + "[Id],[Name],[Description]" + System.Convert.ToChar(34) + "");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("	protected String GetFieldList(bool includePk, bool includeComputed)");
        output.AppendLine("	{");
        output.AppendLine("		string result = string.Empty;");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("		{");
        output.AppendLine("            if (!field.IsPk && !field.IsComputed)");
        output.AppendLine("            {");
        output.AppendLine("                result += " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + field.Name + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("            }");
        output.AppendLine("            if (field.IsPk && !field.IsAuto)");
        output.AppendLine("            {");
        output.AppendLine("                result += " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + field.Name + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("            }");
        output.AppendLine("            if (includePk && field.IsPk)");
        output.AppendLine("            {");
        output.AppendLine("                result += " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + field.Name + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("            }");
        output.AppendLine("            if (includeComputed && field.IsComputed)");
        output.AppendLine("            {");
        output.AppendLine("                result += " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + field.Name + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("            }");
        output.AppendLine("		}");
        output.AppendLine("		return result.Substring(0, result.Length - 1);	");
        output.AppendLine("	}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Returns parametrized values list : @Id, @Name,@Description");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "includePk" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    protected String GetParameterizedValuesList(bool includePk, bool includeComputed)");
        output.AppendLine("    {");
        output.AppendLine("        string result = string.Empty;");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("        {");
        output.AppendLine("            if (!field.IsPk && !field.IsComputed)");
        output.AppendLine("            {");
        output.AppendLine("                result += _parameterPrefix + field.Name + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("            }");
        output.AppendLine("            if (field.IsPk && !field.IsAuto)");
        output.AppendLine("            {");
        output.AppendLine("                result += _parameterPrefix + field.Name + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("            }");
        output.AppendLine("            if (includePk && field.IsPk)");
        output.AppendLine("            {");
        output.AppendLine("                result += _parameterPrefix + field.Name + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("            }");
        output.AppendLine("            if (includeComputed && field.IsComputed)");
        output.AppendLine("            {");
        output.AppendLine("                result += _parameterPrefix + field.Name + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        return result.Substring(0, result.Length - 1);");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Builds Parameter Values list eg.: " + System.Convert.ToChar(34) + "@Id,@Name'" + System.Convert.ToChar(34) + " for select based on parameters");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    protected void BuildParameterValuesList(WhereParameterBase item)");
        output.AppendLine("    {");
        output.AppendLine("        _parameterizedValues.Clear();");

        output.AppendLine("        foreach (ParameterItem field in item._whereParameters)");
        output.AppendLine("        {");
        output.AppendLine("            if(field.IsFieldParameter)");
        output.AppendLine("            {");
        output.AppendLine("                 foreach (ParameterItemValue value in field.Values)");
        output.AppendLine("                 {");
        output.AppendLine("                     _parameterizedValues.Add(new ParameterItemValue(value.Name.Replace(Constants.INPUT_PARAMETER, _parameterPrefix), value.Value));");
        output.AppendLine("                 }");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("    }	");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Builds Parameter Values list eg.: " + System.Convert.ToChar(34) + "@Id,@Name'" + System.Convert.ToChar(34) + " for Insert");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("	");
        output.AppendLine("	protected void BuildParameterValuesList(IDataItem item)");
        output.AppendLine("	{");
        output.AppendLine("        _parameterizedValues.Clear();");
        output.AppendLine("       ");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("		{");
        output.AppendLine("            if (!field.IsPk)");
        output.AppendLine("            {");
        output.AppendLine("                var memberData = item.GetType().GetProperty(field.Name).GetValue(item, null);");
        output.AppendLine("                _parameterizedValues.Add(new ParameterItemValue( _parameterPrefix + field.Name, memberData));");
        output.AppendLine("            }");
        output.AppendLine("		}");
        output.AppendLine("		");
        output.AppendLine("	}	");
        output.AppendLine("	/// <summary>");
        output.AppendLine("    /// Returns set list separated by colon eg.: " + System.Convert.ToChar(34) + "[Id] = @Id,[Name] = @Name,[Description] = @Description" + System.Convert.ToChar(34) + "");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("	");
        output.AppendLine("	protected String GetSetList(IDataItem item)");
        output.AppendLine("	{");
        output.AppendLine("		string result = string.Empty;");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("        {");
        output.AppendLine("            if (!field.IsPk && !field.IsComputed)");
        output.AppendLine("            {");
        output.AppendLine("				result += " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + field.Name + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + " = " + System.Convert.ToChar(34) + " + _parameterPrefix + item.GetType().GetProperty(field.Name).ToString() + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("		return result.Substring(0, result.Length - 1);	");
        output.AppendLine("	}");
        output.AppendLine("	/// <summary>");
        output.AppendLine("    /// Returns keys list separated by colon eg.: " + System.Convert.ToChar(34) + "[Id] = @Id AND [Code] = @Code" + System.Convert.ToChar(34) + "");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    protected String GetKeysList(IDataItem item)");
        output.AppendLine("	{");
        output.AppendLine("		string result = String.Empty;");
        output.AppendLine("		foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("		{");
        output.AppendLine("            if (field.IsPk || field.IsKey)");
        output.AppendLine("			{");
        output.AppendLine("				result += " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + field.Name + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + " = " + System.Convert.ToChar(34) + " + _parameterPrefix + item.GetType().GetProperty(field.Name).ToString() + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("			}");
        output.AppendLine("		}");
        output.AppendLine("		return result.Substring(0, result.Length - 1);	");
        output.AppendLine("	}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Returns a List<IDataItem> for a SQL query.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <remarks> </remarks>");
        output.AppendLine("    protected List<IDataItem> Items(string query)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            //Clears previous result");
        output.AppendLine("            _itemList.Clear();");
        output.AppendLine("            //Transaction evaluation");
        output.AppendLine("            if (_transaction == null)");
        output.AppendLine("            {");
        output.AppendLine("                _datareader = ExecuteReader(query,_transaction);");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                _datareader = ExecuteReader(_commandText, _transaction);");
        output.AppendLine("            }");

        output.AppendLine("            _itemList = MapDataReaderToDataItem(_datareader, _dataItem);");

        output.AppendLine("            return _itemList;");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("        finally");
        output.AppendLine("        {");
        output.AppendLine("            if (_datareader != null)");
        output.AppendLine("                _datareader.Close();");
        output.AppendLine("            if (_connection.State == ConnectionState.Open && _transaction == null)");
        output.AppendLine("                _connection.Close();");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Begins a Transaction.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    internal virtual IDbTransaction BeginTransaction()");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            if (_connection == null)");
        output.AppendLine("            {");
        output.AppendLine("                GetConnection();");
        output.AppendLine("                _transaction = _connection.BeginTransaction();");
        output.AppendLine("                return _transaction;");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                throw (new Exception(" + System.Convert.ToChar(34) + "Transaction already opened" + System.Convert.ToChar(34) + "));");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Ends a Transaction with commit  or rollback passed as boolean.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "commit" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "validateOpenedTransaction" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    internal virtual bool EndTransaction(bool commit,bool validateOpenedTransaction = true");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            if (_connection == null && validateOpenedTransaction == true)");
        output.AppendLine("            {");
        output.AppendLine("                throw (new Exception(" + System.Convert.ToChar(34) + "Transaction not opened yet" + System.Convert.ToChar(34) + "));");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                if (commit)");
        output.AppendLine("                {");
        output.AppendLine("                    _transaction.Commit();");
        output.AppendLine("                }");
        output.AppendLine("                else");
        output.AppendLine("                {");
        output.AppendLine("                    _transaction.Rollback();");
        output.AppendLine("                }");
        output.AppendLine("                _connection.Close();");
        output.AppendLine("                _connection = null;");
        output.AppendLine("                _transaction = null;");
        output.AppendLine("                return true;");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected void GetConnection()");
        output.AppendLine("    {");
        output.AppendLine("        if (_connection == null)");
        output.AppendLine("        {");
        output.AppendLine("            getAssembliesNames();");
        output.AppendLine("            // Create a  connection of the type");
        output.AppendLine("            _connection = (IDbConnection)Activator.CreateInstance(_adoNetAssemblyName, _adoNetConnectionTypeName).Unwrap();");
        output.AppendLine("            // Retrieve the Connection String    ");
        output.AppendLine("            _connection.ConnectionString = ConfigurationManager.ConnectionStrings[Constants.CONNECTIONSTRING].ConnectionString;");
        output.AppendLine("            _connection.Open();");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected IDbCommand GetCommand(string query, IDbConnection connection, IDbTransaction transaction, CommandType commandType = CommandType.Text)");
        output.AppendLine("    {");
        output.AppendLine("        getAssembliesNames();");
        output.AppendLine("        IDbCommand newCommand = connection.CreateCommand();");
        output.AppendLine("        newCommand.CommandText = query;");
        output.AppendLine("        newCommand.Connection = connection;");
        output.AppendLine("        newCommand.Transaction = transaction;");
        output.AppendLine("        newCommand.CommandType = commandType;");
        output.AppendLine("        newCommand.CommandTimeout = _commandTimeout;");
        output.AppendLine("        foreach (ParameterItemValue parameterValue in _parameterizedValues)");
        output.AppendLine("		{");
        output.AppendLine("			    IDbDataParameter parameter = newCommand.CreateParameter();");
        output.AppendLine("			    parameter.ParameterName = parameterValue.Name;");
        output.AppendLine("			    parameter.Value = parameterValue.Value;");
        output.AppendLine("			    newCommand.Parameters.Add(parameter);");
        output.AppendLine("		}		");
        output.AppendLine("        return newCommand;");
        output.AppendLine("    }");
        output.AppendLine("    protected void getAssembliesNames()");
        output.AppendLine("    {");
        output.AppendLine("        if (_adoNetAssemblyName.Equals(String.Empty) || _adoNetConnectionTypeName.Equals(String.Empty))");
        output.AppendLine("        {");
        output.AppendLine("             _adoNetAssemblyName = Configuration.AdoNetAssemblyName;");
        output.AppendLine("             _adoNetConnectionTypeName = Configuration.AdoNetConnectionTypeName;");
        output.AppendLine("             _commandTimeout = Convert.ToInt32(Configuration.AdoNetCommandTimeout);");
        output.AppendLine("             _parameterPrefix = Configuration.ParameterPrefix;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected IDataReader ExecuteReader(String query, IDbTransaction transaction)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            GetConnection();");
        output.AppendLine("            _command = GetCommand(query, _connection, transaction);");
        output.AppendLine("		");
        output.AppendLine("            return _command.ExecuteReader();");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception sqlExeption)");
        output.AppendLine("        {");
        output.AppendLine("            throw sqlExeption;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected Int64 ExecuteNonQuery(String query, IDbTransaction transaction)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            GetConnection();");
        output.AppendLine("            _command = GetCommand(query, _connection, transaction);");
        output.AppendLine("            return _command.ExecuteNonQuery();");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception sqlExeption)");
        output.AppendLine("        {");
        output.AppendLine("            throw sqlExeption;");
        output.AppendLine("        }");
        output.AppendLine("        finally");
        output.AppendLine("        {");
        output.AppendLine("            if(_connection.State != ConnectionState.Closed && transaction == null)");
        output.AppendLine("                 _connection.Close();");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Maps DataReader data to DataItem entity with reflection.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "dr" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    protected virtual List<IDataItem> MapDataReaderToDataItem<IDataItem>(IDataReader dr, IDataItem dataItem) //where IDataItem : new()");
        output.AppendLine("    {");
        output.AppendLine("        Type dataItemType = dataItem.GetType();");

        output.AppendLine("        PropertyInfo[] properties = dataItemType.GetProperties(); ");

        output.AppendLine("        Hashtable hashtable = new Hashtable();");

        output.AppendLine("        List<IDataItem> entities = new List<IDataItem>();");

        output.AppendLine("        foreach (PropertyInfo info in properties)");
        output.AppendLine("        {");
        output.AppendLine("            hashtable[info.Name.ToUpper()] = info;");
        output.AppendLine("        }");
        output.AppendLine("        while (dr.Read())");
        output.AppendLine("        {");
        output.AppendLine("            IDataItem newObject = (IDataItem)Activator.CreateInstance(dataItemType, false);");
        output.AppendLine("            for (int index = 0; index < dr.FieldCount; index++)");
        output.AppendLine("            {");
        output.AppendLine("                PropertyInfo info = (PropertyInfo)hashtable[dr.GetName(index).ToUpper()];");
        output.AppendLine("                if ((info != null) && info.CanWrite)");
        output.AppendLine("                {");
        output.AppendLine("                     if (!dr.GetValue(index).Equals(System.DBNull.Value))");
        output.AppendLine("                         info.SetValue(newObject, dr.GetValue(index), null);");
        output.AppendLine("                }");
        output.AppendLine("            }");
        output.AppendLine("            entities.Add(newObject);");
        output.AppendLine("        }");
        output.AppendLine("        dr.Close();");
        output.AppendLine("        return entities;");
        output.AppendLine("    }");
        output.AppendLine(" ");
        output.AppendLine("    protected virtual void Dispose(bool disposing)");
        output.AppendLine("    {");
        output.AppendLine("        if (!disposedValue)");
        output.AppendLine("        {");
        output.AppendLine("            if (disposing)");
        output.AppendLine("            {");
        output.AppendLine("                // TODO: dispose managed state (managed objects)");
        output.AppendLine("                _itemList = null;");
        output.AppendLine("                _fieldList = null;");
        output.AppendLine("                _dataItem = null;");
        output.AppendLine("            }");
        output.AppendLine(" ");
        output.AppendLine("            // TODO: free unmanaged resources (unmanaged objects) and override finalizer");
        output.AppendLine("            // TODO: set large fields to null");
        output.AppendLine("            _connection = null;");
        output.AppendLine("            _command = null;");
        output.AppendLine("            _transaction = null;");
        output.AppendLine("            _datareader = null;");
        output.AppendLine(" ");
        output.AppendLine("            disposedValue = true;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine(" ");
        output.AppendLine("    void IDisposable.Dispose()");
        output.AppendLine("    {");
        output.AppendLine("        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method");
        output.AppendLine("        Dispose(disposing: true);");
        if (_generationProject.UsesExplicitGarbageCollection)
        {
            output.AppendLine("        " + _generationProject.GarbageCollectionCode);
        }
        output.AppendLine("    }");
        output.AppendLine("}");

        SaveOutputToFile("DataHandlerBase.cs", output, true);
        output = new StringBuilder();
    }

    private void BuildDataHandlerBaseNETCORE(string nameSpace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Data;");
        output.AppendLine("using System.Configuration;");
        output.AppendLine("using System.Collections;");
        output.AppendLine("using System.Reflection;");

        output.AppendLine("public class DataHandlerBase: IDataHandler, IDisposable");
        output.AppendLine("{");
        output.AppendLine("    #region Local Variables");
        output.AppendLine("    protected string _commandText = null;");
        output.AppendLine("    protected string _fieldList = string.Empty;");
        output.AppendLine("    protected string _valueList = string.Empty;");
        output.AppendLine("    protected string _adoNetAssemblyName = string.Empty;");
        output.AppendLine("    protected string _adoNetConnectionTypeName = string.Empty;");
        output.AppendLine("    protected string _parameterPrefix = string.Empty;");
        output.AppendLine("    protected string _connectionString = string.Empty;");
        output.AppendLine("    protected Int32 _commandTimeout = 30;");
        output.AppendLine("    /// List of parameters to hold DataItem Values information");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected List<ParameterItemValue> _parameterizedValues = new List<ParameterItemValue>();");
        output.AppendLine("    #endregion");
        output.AppendLine("    #region Local interfaces");
        output.AppendLine("        protected IDataItem _dataItem = null;");
        output.AppendLine("        protected List<IDataItem> _itemList = new List<IDataItem>();");
        output.AppendLine("    #endregion");
        output.AppendLine("    #region ADO.NET Interfaces");
        output.AppendLine("        protected IDbConnection _connection = null;");
        output.AppendLine("        protected IDbCommand _command = null;");
        output.AppendLine("        protected IDbTransaction _transaction { get; set; }");
        output.AppendLine("        protected IDataReader _datareader { get; set; }");
        output.AppendLine("    #endregion");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Represents Where condition list");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected WhereParameterBase WhereParameter {get;set;}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Represents Order By parameters");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected OrderByParameterBase OrderByParameter  {get;set;}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Represents Group By parameters");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected GroupByParameterBase GroupByParameter  {get;set;}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Represents aggregate parameter");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected AggregateParameterBase AggregateParameter {get;set;}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Reflection cache data");
        output.AppendLine("    /// </summary>");
        output.AppendLine("	#region Reflection data");
        output.AppendLine("		protected Type _dataItemType = null;");
        output.AppendLine("		private PropertyInfo[] _properties = null;");
        output.AppendLine("		private System.Attribute _objectSchemaTypeAttribute = null;");
        output.AppendLine("		private System.Attribute _objectNameTypeAttribute = null;");
        output.AppendLine("	#endregion");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// These are field definitions stores as Custom Attributes in Entities");
        output.AppendLine("    /// and are loaded with Reflection");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    protected List<DataFieldDefinition> _dataFieldDefinitions = new List<DataFieldDefinition>();");
        output.AppendLine("    private bool disposedValue;");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// No constructor");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public DataHandlerBase()");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        WhereParameter = new WhereParameterBase();");
        output.AppendLine("        OrderByParameter = new OrderByParameterBase();");
        output.AppendLine("        GroupByParameter = new GroupByParameterBase();");
        output.AppendLine("        getAssembliesNames();");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Constructor with IDataItem");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public DataHandlerBase(IDataItem dataItem)");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        WhereParameter = new WhereParameterBase();");
        output.AppendLine("        OrderByParameter = new OrderByParameterBase();");
        output.AppendLine("        GroupByParameter = new GroupByParameterBase();");
        output.AppendLine("        _dataItem = dataItem;");
        output.AppendLine("        AnalizeIDataItem();");
        output.AppendLine("        getAssembliesNames();");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Constructor to handle Transaction by reflection");
        output.AppendLine("    /// IDataHandler has Connection and dataItem intrinsic information");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    public DataHandlerBase(IDataHandler dataHandler)");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("             WhereParameter = new WhereParameterBase();");
        output.AppendLine("			    OrderByParameter = new OrderByParameterBase();");
        output.AppendLine("             GroupByParameter = new GroupByParameterBase();");
        output.AppendLine("            //_transaction = dataHandler.GetTransaction(); // Gets IDataHandler transaction");
        output.AppendLine("			//_dataItem = dataHandler._dataItem; // Gets IDataHandler IdataItem");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception)");
        output.AppendLine("        {");
        output.AppendLine("            throw (new Exception(Constants.ERROR_CONSTRUCTOR));");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Returns working transaction");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    IDbTransaction IDataHandler.GetTransaction()");
        output.AppendLine("    {");
        output.AppendLine("        return _transaction;");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Returns working connection");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    IDbConnection IDataHandler.GetConnection()");
        output.AppendLine("    {");
        output.AppendLine("        return _connection;");
        output.AppendLine("    }");
        output.AppendLine("	/////////////// Este bloque es para analizar y cachear los datos del IDataItem //////////////////////");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Gets IdataItem reflection information and set corresponding variables.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>	");

        output.AppendLine("   protected void AnalizeIDataItem()");
        output.AppendLine("    {");
        output.AppendLine("        if (_dataItemType == null)");
        output.AppendLine("        {");
        output.AppendLine("            _dataItemType = _dataItem.GetType();");
        output.AppendLine("            _properties = _dataItemType.GetProperties();");
        output.AppendLine("            //Gets the Schema Name of the IDataItem");
        output.AppendLine("            Type DataItemAttributeSchemaNameType = typeof(DataItemAttributeSchemaName);");
        output.AppendLine("            //Gets the Object Name of the IDataItem");
        output.AppendLine("            Type DataItemAttributeObjectNameType = typeof(DataItemAttributeObjectName);");

        output.AppendLine("            Type PropertyAttribute = typeof(PropertyAttribute);");
        output.AppendLine("            Type PropertyAttributeEnum = typeof(PropertyAttribute.PropertyAttributeEnum);");

        output.AppendLine("            for (int i = 0; i < (((System.Reflection.MemberInfo)(_dataItemType))).GetCustomAttributes(false).Length; i++)");
        output.AppendLine("            {");
        output.AppendLine("                switch ((((System.Reflection.MemberInfo)(_dataItemType))).GetCustomAttributes(false)[i].ToString())");
        output.AppendLine("                {");
        output.AppendLine("                    case Constants.SYSTEM_SERIALIZABLEATTRIBUTE:");
        output.AppendLine("                        break;");
        output.AppendLine("                    case Constants.DATAITEMATTRIBUTESCHEMANAME:");
        output.AppendLine("                        _objectSchemaTypeAttribute = (DataItemAttributeSchemaName)(((System.Reflection.MemberInfo)(_dataItemType))).GetCustomAttributes(false)[i];");
        output.AppendLine("                        break;");
        output.AppendLine("                    case Constants.DATAITEMATTRIBUTEOBJECTTYPE:");
        output.AppendLine("                        break;");
        output.AppendLine("                    case Constants.DATAITEMATTRIBUTEOBJECTNAME:");
        output.AppendLine("                        _objectNameTypeAttribute = (DataItemAttributeObjectName)(((System.Reflection.MemberInfo)(_dataItemType))).GetCustomAttributes(false)[i];");
        output.AppendLine("                        break;");
        output.AppendLine("                    default:");
        output.AppendLine("                        break;");
        output.AppendLine("                }");
        output.AppendLine("            }");

        output.AppendLine("            foreach (PropertyInfo info in _properties)");
        output.AppendLine("            {");
        output.AppendLine("                bool isPk = false;");
        output.AppendLine("                bool isAuto = false;");
        output.AppendLine("                bool isKey = false;");
        output.AppendLine("                bool isFk = false;");
        output.AppendLine("                bool isDp = false;");
        output.AppendLine("                bool isNone = false;");
        output.AppendLine("                bool exclude = false;");
        output.AppendLine("                bool isComputed = false;");
        output.AppendLine("                String fieldName = String.Empty;");
        output.AppendLine("                String fieldFrameworkName = String.Empty;");

        output.AppendLine("                int customAttributesCount = info.GetCustomAttributes(false).GetLength(0);");
        output.AppendLine("                for (int i = 0; i < customAttributesCount; i++)");
        output.AppendLine("                {");
        output.AppendLine("                    if ( info.GetCustomAttributes(false)[i].GetType().Name.Equals(Constants.PROPERTYATTRIBUTE))");
        output.AppendLine("                    {");
        output.AppendLine("                        switch (((PropertyAttribute)(info.GetCustomAttributes(false)[i]))._propertyAttribute)");
        output.AppendLine("                        {");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Pk:");
        output.AppendLine("                                isPk = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Auto:");
        output.AppendLine("                                isAuto = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Key:");
        output.AppendLine("                                isKey = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Fk:");
        output.AppendLine("                                isFk = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Display:");
        output.AppendLine("                                isDp = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.None:");
        output.AppendLine("                                isNone = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Exclude:");
        output.AppendLine("                                exclude = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            case global::PropertyAttribute.PropertyAttributeEnum.Computed:");
        output.AppendLine("                                isComputed = true;");
        output.AppendLine("                                break;");
        output.AppendLine("                            default:");
        output.AppendLine("                                break;");
        output.AppendLine("                        }");
        output.AppendLine("                    }");
        output.AppendLine("                    fieldName = info.Name;");
        output.AppendLine("                    if (info.GetCustomAttributes(false)[i].GetType().Name.Equals(Constants.DATAITEMATTRIBUTEFIELDNAME))");
        output.AppendLine("                    {");
        output.AppendLine("                        fieldName = ((DataItemAttributeFieldName)(info.GetCustomAttributes(false)[i])).FieldName;");
        output.AppendLine("                        fieldFrameworkName = ((DataItemAttributeFieldName)(info.GetCustomAttributes(false)[i])).FieldFrameworkName;");
        output.AppendLine("                    }");
        output.AppendLine("                }");

        output.AppendLine("                 if(!exclude)");
        output.AppendLine("                     _dataFieldDefinitions.Add(new DataFieldDefinition(fieldName, fieldFrameworkName,(info.PropertyType).FullName, isPk,isAuto, isKey, isFk, isDp,isNone,exclude, isComputed));");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("    }");

        output.AppendLine("	/// <summary>");
        output.AppendLine("    /// Returns [Schema name].[Object name] eg.: [dbo].[User]");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("	protected String GetFullDataEntityName()");
        output.AppendLine("	{");
        output.AppendLine("        AnalizeIDataItem();");
        output.AppendLine("        if (((DataItemAttributeSchemaName)_objectSchemaTypeAttribute).schemaName.Equals(String.Empty))");
        output.AppendLine("		        return  " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + ((DataItemAttributeObjectName)_objectNameTypeAttribute).ObjectName + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + ";");
        output.AppendLine("        else ");
        output.AppendLine("		        return  " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + ((DataItemAttributeSchemaName)_objectSchemaTypeAttribute).schemaName + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " +");
        output.AppendLine("		         " + System.Convert.ToChar(34) + ".[" + System.Convert.ToChar(34) + " + ((DataItemAttributeObjectName)_objectNameTypeAttribute).ObjectName + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + ";");
        output.AppendLine("	}");
        output.AppendLine("	/// <summary>");
        output.AppendLine("    /// Returns field list separated by colon eg.: " + System.Convert.ToChar(34) + "[Id],[Name],[Description]" + System.Convert.ToChar(34) + "");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("	protected String GetFieldList(bool includePk, bool includeComputed)");
        output.AppendLine("	{");
        output.AppendLine("		string result = string.Empty;");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("		{");

        output.AppendLine("            if ((!field.IsPk && !field.IsComputed) || (field.IsPk && !field.IsAuto) || (includePk && field.IsPk) || (includeComputed && field.IsComputed))");
        output.AppendLine("                result += " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + field.Name + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("		}");
        output.AppendLine("		return result.Substring(0, result.Length - 1);	");
        output.AppendLine("	}");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Returns parametrized values list : @Id, @Name,@Description");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "includePk" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    protected String GetParameterizedValuesList(bool includePk, bool includeComputed)");
        output.AppendLine("    {");
        output.AppendLine("        string result = string.Empty;");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("        {");
        output.AppendLine("            if ((!field.IsPk && !field.IsComputed) || (field.IsPk && !field.IsAuto) || (includePk && field.IsPk) || (includeComputed && field.IsComputed))");
        output.AppendLine("                result += _parameterPrefix + field.Name + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("        }");
        output.AppendLine("        return result.Substring(0, result.Length - 1);");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Builds Parameter Values list eg.: " + System.Convert.ToChar(34) + "@Id,@Name'" + System.Convert.ToChar(34) + " for select based on parameters");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    protected void BuildParameterValuesList(WhereParameterBase item)");
        output.AppendLine("    {");
        output.AppendLine("        _parameterizedValues.Clear();");

        output.AppendLine("        foreach (ParameterItem field in item._whereParameters)");
        output.AppendLine("        {");
        output.AppendLine("            if(field.IsFieldParameter)");
        output.AppendLine("            {");
        output.AppendLine("                 foreach (ParameterItemValue value in field.Values)");
        output.AppendLine("                 {");
        output.AppendLine("                     _parameterizedValues.Add(new ParameterItemValue(value.Name.Replace(Constants.INPUT_PARAMETER, _parameterPrefix), value.Value));");
        output.AppendLine("                 }");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("    }	");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Builds Parameter Values list eg.: " + System.Convert.ToChar(34) + "@Id,@Name'" + System.Convert.ToChar(34) + " for select based on IDataItem parameters");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    protected void BuildIDataItemParameterValuesList(IDataItem item,bool includeNonPk = false)");
        output.AppendLine("    {");
        output.AppendLine("        _parameterizedValues.Clear();");
        output.AppendLine("        _dataItem = item;");
        output.AppendLine("        AnalizeIDataItem();");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("        {");
        output.AppendLine("            if(field.IsPk)");
        output.AppendLine("            {");
        output.AppendLine("                var pkValue = item.GetType().GetProperty(field.Name).GetValue(item, null);");
        output.AppendLine("                _parameterizedValues.Add(new ParameterItemValue(field.Name.Replace(Constants.INPUT_PARAMETER, _parameterPrefix), pkValue));");
        output.AppendLine("            }");
        output.AppendLine("            if (!field.IsPk && includeNonPk)");
        output.AppendLine("            {");
        output.AppendLine("                var pkValue = item.GetType().GetProperty(field.Name).GetValue(item, null);");
        output.AppendLine("                _parameterizedValues.Add(new ParameterItemValue(field.Name.Replace(Constants.INPUT_PARAMETER, _parameterPrefix), pkValue));");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("    }	");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Builds Parameter Values list eg.: " + System.Convert.ToChar(34) + "@Id,@Name'" + System.Convert.ToChar(34) + " for Insert");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("	protected void BuildParameterValuesList(IDataItem item)");
        output.AppendLine("	{");
        output.AppendLine("        _parameterizedValues.Clear();");
        output.AppendLine("       ");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("		{");
        output.AppendLine("            if (!field.IsPk || (field.IsPk && !field.IsAuto))");
        output.AppendLine("            {");
        output.AppendLine("                var memberData = item.GetType().GetProperty(field.Name).GetValue(item, null);");
        output.AppendLine("                _parameterizedValues.Add(new ParameterItemValue( _parameterPrefix + field.Name, memberData));");
        output.AppendLine("            }");
        output.AppendLine("		}");
        output.AppendLine("		");
        output.AppendLine("	}	");
        output.AppendLine("	/// <summary>");
        output.AppendLine("    /// Returns set list separated by colon eg.: " + System.Convert.ToChar(34) + "[Id] = @Id,[Name] = @Name,[Description] = @Description" + System.Convert.ToChar(34) + "");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("	");
        output.AppendLine("	protected String GetSetList(IDataItem item)");
        output.AppendLine("	{");
        output.AppendLine("		string result = string.Empty;");
        output.AppendLine("        foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("        {");
        output.AppendLine("            if (!field.IsPk && !field.IsComputed)");
        output.AppendLine("            {");
        output.AppendLine("				result += " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + field.Name + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + " = " + System.Convert.ToChar(34) + " + _parameterPrefix + field.Name + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("		return result.Substring(0, result.Length - 1);	");
        output.AppendLine("	}");
        output.AppendLine("	/// <summary>");
        output.AppendLine("    /// Returns keys list separated by colon eg.: " + System.Convert.ToChar(34) + "[Id] = @Id AND [Code] = @Code" + System.Convert.ToChar(34) + "");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    protected String GetKeysList(IDataItem item)");
        output.AppendLine("	{");
        output.AppendLine("		string result = String.Empty;");
        output.AppendLine("		foreach (DataFieldDefinition field in _dataFieldDefinitions)");
        output.AppendLine("		{");
        output.AppendLine("            if (field.IsPk || field.IsKey)");
        output.AppendLine("			{");
        output.AppendLine("				result += " + System.Convert.ToChar(34) + "[" + System.Convert.ToChar(34) + " + field.Name + " + System.Convert.ToChar(34) + "]" + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + " = " + System.Convert.ToChar(34) + " + _parameterPrefix + field.Name + Constants.SQL_AND;");
        output.AppendLine("			}");
        output.AppendLine("		}");
        output.AppendLine("		return result.Substring(0, result.Length - 5);	");
        output.AppendLine("	}");

        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Begins a Transaction.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    internal virtual IDbTransaction BeginTransaction()");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            if (_connection == null)");
        output.AppendLine("            {");
        output.AppendLine("                GetConnection();");
        output.AppendLine("                _transaction = _connection.BeginTransaction();");
        output.AppendLine("                return _transaction;");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                throw (new Exception(Constants.ERROR_TRANSACTION_ALREADY_OPENED));");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Ends a Transaction with commit  or rollback passed as boolean.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "commit" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "validateOpenedTransaction" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    internal virtual bool EndTransaction(bool commit,bool validateOpenedTransaction = true)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            if (_connection == null && validateOpenedTransaction == true)");
        output.AppendLine("            {");
        output.AppendLine("                throw (new Exception(Constants.ERROR_TRANSACTION_NOT_OPENED_YET));");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                if (commit)");
        output.AppendLine("                {");
        output.AppendLine("                    _transaction.Commit();");
        output.AppendLine("                }");
        output.AppendLine("                else");
        output.AppendLine("                {");
        output.AppendLine("                    _transaction.Rollback();");
        output.AppendLine("                }");
        output.AppendLine("                _connection.Close();");
        output.AppendLine("                _connection = null;");
        output.AppendLine("                _transaction = null;");
        output.AppendLine("                return true;");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected void GetConnection()");
        output.AppendLine("    {");
        output.AppendLine("        if (_transaction != null)");
        output.AppendLine("        {");
        output.AppendLine("             _connection = _transaction.Connection;");
        output.AppendLine("        }");
        output.AppendLine("        else");
        output.AppendLine("        {");
        output.AppendLine("             if (_connection == null)");
        output.AppendLine("             {");
        output.AppendLine("                 getAssembliesNames();");
        output.AppendLine("                 // Create a  connection of the type");
        output.AppendLine("                 _connection = (IDbConnection)Activator.CreateInstance(_adoNetAssemblyName, _adoNetConnectionTypeName).Unwrap();");
        output.AppendLine("                 // Retrieve the Connection String    ");
        if (_generationProject.UsesEncription)
            output.AppendLine("                 _connection.ConnectionString = " +
            nameSpace + ".Crypto.Decrypt(ConfigurationHandler.ConnectionString ,ConfigurationHandler.PasswordKey);");
        else
            output.AppendLine("                 _connection.ConnectionString = ConfigurationHandler.ConnectionString;");

        output.AppendLine("                 _connection.Open();");
        output.AppendLine("              }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                 if (_connection.State == ConnectionState.Closed)");
        output.AppendLine("                 _connection.Open(); ");
        output.AppendLine("            }");

        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected IDbCommand GetCommand(string query, IDbConnection connection, IDbTransaction transaction, CommandType commandType = CommandType.Text)");
        output.AppendLine("    {");
        output.AppendLine("        getAssembliesNames();");
        output.AppendLine("        IDbCommand newCommand = connection.CreateCommand();");
        output.AppendLine("        newCommand.CommandText = query;");
        output.AppendLine("        newCommand.Connection = connection;");
        output.AppendLine("        newCommand.Transaction = transaction;");
        output.AppendLine("        newCommand.CommandType = commandType;");
        output.AppendLine("        newCommand.CommandTimeout = _commandTimeout;");
        output.AppendLine("        foreach (ParameterItemValue parameterValue in _parameterizedValues)");
        output.AppendLine("		{");
        output.AppendLine("			    IDbDataParameter parameter = newCommand.CreateParameter();");
        output.AppendLine("			    parameter.ParameterName = parameterValue.Name;");
        output.AppendLine("			    parameter.Value = parameterValue.Value == null ? DBNull.Value : parameterValue.Value;");
        output.AppendLine("			    newCommand.Parameters.Add(parameter);");
        output.AppendLine("		}		");
        output.AppendLine("        return newCommand;");
        output.AppendLine("    }");
        output.AppendLine("    protected void getAssembliesNames()");
        output.AppendLine("    {");
        output.AppendLine("        if (_adoNetAssemblyName.Equals(String.Empty) || _adoNetConnectionTypeName.Equals(String.Empty))");
        output.AppendLine("        {");
        output.AppendLine("             _adoNetAssemblyName = ConfigurationHandler.AdoNetAssemblyName;");
        output.AppendLine("             _adoNetConnectionTypeName = ConfigurationHandler.AdoNetConnectionTypeName;");
        output.AppendLine("             _commandTimeout = ConfigurationHandler.AdoNetCommandTimeout;");
        output.AppendLine("             _parameterPrefix = ConfigurationHandler.ParameterPrefix;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected IDataReader ExecuteReader(String query, IDbTransaction transaction, CommandType commandType = CommandType.Text)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            GetConnection();");
        output.AppendLine("            _command = GetCommand(query, _connection, transaction,commandType);");
        output.AppendLine("		");
        output.AppendLine("            return _command.ExecuteReader();");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception sqlExeption)");
        output.AppendLine("        {");
        output.AppendLine("            throw sqlExeption;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected Int64 ExecuteNonQuery(String query, IDbTransaction transaction,bool addOutputParameter)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            GetConnection();");
        output.AppendLine("            _command = GetCommand(query, _connection, transaction);");
        output.AppendLine("            if (addOutputParameter)");
        output.AppendLine("            {");
        output.AppendLine("                IDbDataParameter outputParam = _command.CreateParameter();");
        output.AppendLine("                outputParam.ParameterName = Constants.NEWID;");
        output.AppendLine("                outputParam.Direction = ParameterDirection.Output;");
        output.AppendLine("                outputParam.Size = Int32.MaxValue;");
        output.AppendLine("                _command.Parameters.Add(outputParam); ");
        output.AppendLine("            }");
        output.AppendLine("            if(_connection.State == ConnectionState.Closed)");
        output.AppendLine("                 _connection.Open();");
        output.AppendLine("            return _command.ExecuteNonQuery();");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception sqlExeption)");
        output.AppendLine("        {");
        output.AppendLine("            throw sqlExeption;");
        output.AppendLine("        }");
        output.AppendLine("        finally");
        output.AppendLine("        {");
        output.AppendLine("            if(_connection.State != ConnectionState.Closed && transaction == null)");
        output.AppendLine("                 _connection.Close();");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected string GetPkName()");
        output.AppendLine("    {");
        output.AppendLine("        string returnValue = string.Empty;");
        output.AppendLine("        foreach (var item in _dataFieldDefinitions)");
        output.AppendLine("        {");
        output.AppendLine("            if (item.IsPk)");
        output.AppendLine("                returnValue =  item.Name;");
        output.AppendLine("        }");
        output.AppendLine("        return returnValue;");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Maps DataReader data to DataItem entity with reflection.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "dr" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    protected virtual List<IDataItem> MapDataReaderToDataItem<IDataItem>(IDataReader dr, IDataItem dataItem) //where IDataItem : new()");
        output.AppendLine("    {");
        output.AppendLine("        Type dataItemType = dataItem.GetType();");
        output.AppendLine("        PropertyInfo[] properties = dataItemType.GetProperties(); ");
        output.AppendLine("        Hashtable hashtable = new Hashtable();");
        output.AppendLine("        List<IDataItem> entities = new List<IDataItem>();");
        output.AppendLine("        foreach (PropertyInfo info in properties)");
        output.AppendLine("        {");
        output.AppendLine("            hashtable[info.Name] = info;");
        output.AppendLine("        }");
        output.AppendLine("        while (dr.Read())");
        output.AppendLine("        {");
        output.AppendLine("            IDataItem newObject = (IDataItem)Activator.CreateInstance(dataItemType, false);");
        output.AppendLine("            for (int index = 0; index < dr.FieldCount; index++)");
        output.AppendLine("            {");
        //output.AppendLine("                PropertyInfo info = (PropertyInfo)hashtable[dr.GetName(index).Replace(" + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + "_" + System.Convert.ToChar(34) + ")];");
        output.AppendLine("                PropertyInfo info = (PropertyInfo)hashtable[_dataFieldDefinitions[index].Name];");
        output.AppendLine("                if ((info != null) && info.CanWrite)");
        output.AppendLine("                {");
        output.AppendLine("                     if (!dr.GetValue(index).Equals(System.DBNull.Value))");
        output.AppendLine("                         info.SetValue(newObject, dr.GetValue(index), null);");
        output.AppendLine("                }");
        output.AppendLine("            }");
        output.AppendLine("            entities.Add(newObject);");
        output.AppendLine("        }");
        output.AppendLine("        dr.Close();");
        output.AppendLine("        return entities;");
        output.AppendLine("    }");
        output.AppendLine(" ");
        output.AppendLine("    protected virtual void Dispose(bool disposing)");
        output.AppendLine("    {");
        output.AppendLine("        if (!disposedValue)");
        output.AppendLine("        {");
        output.AppendLine("            if (disposing)");
        output.AppendLine("            {");
        output.AppendLine("                // TODO: dispose managed state (managed objects)");
        output.AppendLine("                _itemList = null;");
        output.AppendLine("                _fieldList = null;");
        output.AppendLine("                _dataItem = null;");
        output.AppendLine("            }");
        output.AppendLine(" ");
        output.AppendLine("            // TODO: free unmanaged resources (unmanaged objects) and override finalizer");
        output.AppendLine("            // TODO: set large fields to null");
        output.AppendLine("            _connection = null;");
        output.AppendLine("            _command = null;");
        output.AppendLine("            _transaction = null;");
        output.AppendLine("            _datareader = null;");
        output.AppendLine(" ");
        output.AppendLine("            disposedValue = true;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine(" ");
        output.AppendLine("    void IDisposable.Dispose()");
        output.AppendLine("    {");
        output.AppendLine("        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method");
        output.AppendLine("        Dispose(disposing: true);");
        if (_generationProject.UsesExplicitGarbageCollection)
        {
            output.AppendLine("        " + _generationProject.GarbageCollectionCode);
        }
        output.AppendLine("    }");
        output.AppendLine("}");


        SaveOutputToFile("DataHandlerBase.cs", output, true);
        output = new StringBuilder();
    }
    /// <summary>
    /// Builds classes to hold User-Defined Data Types 
    /// </summary>
    private void BuildUserDefinedDataTypeClasses()
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Data;");
        output.AppendLine("using System.Collections;");
        output.AppendLine("using System.Reflection;");
        output.AppendLine("using System.Collections.Generic;");
        foreach (MyMeta.IDomain item in _db.Domains)
        {
            output.AppendLine("public class " + item.Name);
            output.AppendLine("{");
            output.AppendLine("     public " + item.DataTypeName + " " + item.Name + "{get;set;}");
            output.AppendLine("}");
        }
        if (_db.Domains.Count > 0)
            SaveOutputToFile("UserDefinedDataTypeClasses.cs", output, true);
        output = new StringBuilder();


    }
    private void BuildDataHandler()
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Data;");
        output.AppendLine("using System.Collections;");
        output.AppendLine("using System.Reflection;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("public class DataHandler : DataHandlerBase");
        output.AppendLine("{");
        output.AppendLine("    protected string _pkFunction = ConfigurationHandler.PkFunction;");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// No argument constructor");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public DataHandler()");
        output.AppendLine("        : base()");
        output.AppendLine("    { }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Constructor with IDataItem");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public DataHandler(IDataItem dataItem)");
        output.AppendLine("        : base(dataItem)");
        output.AppendLine("    {");
        output.AppendLine("        _dataItem = dataItem;");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Constructor to handle Transaction by reflection");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    public DataHandler(IDataHandler dataHandler)");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            _transaction = dataHandler.GetTransaction();");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception)");
        output.AppendLine("        {");
        output.AppendLine("            throw (new Exception(" + System.Convert.ToChar(34) + "DataHandler (New) : Transaction assignment Error." + System.Convert.ToChar(34) + "));");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Adds a record in table");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "Iemm" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <remarks> </remarks>");
        output.AppendLine("    internal virtual IDataItem Add(IDataItem item)");
        output.AppendLine("    {");
        output.AppendLine("        long result = 0;");
        output.AppendLine("        try");
        output.AppendLine("        {");

        output.AppendLine("            _commandText = Constants.SQL_INSERT_INTO ;");
        output.AppendLine("            _commandText += GetFullDataEntityName();");
        output.AppendLine("            _commandText += " + System.Convert.ToChar(34) + " ( " + System.Convert.ToChar(34) + " + GetFieldList(false, false);");
        output.AppendLine("            _commandText += " + System.Convert.ToChar(34) + " ) " + System.Convert.ToChar(34) + " + Constants.SQL_VALUES;");
        output.AppendLine("            _commandText += " + System.Convert.ToChar(34) + " ( " + System.Convert.ToChar(34) + " + GetParameterizedValuesList(false,false);");
        output.AppendLine("            _commandText += " + System.Convert.ToChar(34) + " )" + System.Convert.ToChar(34) + ";");
        output.AppendLine("            BuildParameterValuesList(item);");

        if (_generationProject.MultiQuery)
            output.AppendLine("            _commandText += _pkFunction;");

        output.AppendLine("            result = ExecuteNonQuery(_commandText, _transaction,true);");

        if (!_generationProject.MultiQuery)
            output.AppendLine("            ExecuteNonQuery(_pkFunction, _transaction, true);");


        output.AppendLine("            if (result > 0)");
        output.AppendLine("            {");
        output.AppendLine("                var newIdValue =((IDbDataParameter)base._command.Parameters[Constants.NEWID]).Value;");
        output.AppendLine("                string pkName = base.GetPkName();");
        output.AppendLine("                Type dataItemType = item.GetType();");
        output.AppendLine("                PropertyInfo[] properties = dataItemType.GetProperties();");
        output.AppendLine("                foreach (PropertyInfo info in properties)");
        output.AppendLine("                {");

        output.AppendLine("                    if (info.Name.Equals(pkName) && newIdValue != DBNull.Value)");
        output.AppendLine("                    {");
        output.AppendLine("                        info.SetValue(item, Convert.ChangeType(newIdValue, info.PropertyType), null);");
        output.AppendLine("                        break;");
        output.AppendLine("                    }");
        output.AppendLine("                }");
        output.AppendLine("                return item;");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                return null;");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Adds or Updates referenced IDataItem");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "item" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    internal virtual IDataItem AddOrUpdate(IDataItem item)");
        output.AppendLine("    {");
        output.AppendLine("        throw new NotImplementedException();");
        output.AppendLine("    }");
        output.AppendLine("");
        output.AppendLine("    public int TopQuantity { get; set; }");
        output.AppendLine("");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Returns a List<IDataItem> for a SQL query.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <remarks> </remarks>");
        output.AppendLine("    internal virtual List<IDataItem> Items()");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            //Clears previous result");
        output.AppendLine("            _itemList.Clear();");
        output.AppendLine("            _commandText = Constants.SQL_SELECT ;");
        output.AppendLine("            if (TopQuantity > 0)");
        output.AppendLine("                 _commandText += Constants.SQL_TOP + TopQuantity.ToString() +" + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + ";");
        output.AppendLine("            _commandText += GetFieldList(true, true);");
        output.AppendLine("            _commandText += Constants.SQL_FROM ;");
        output.AppendLine("            _commandText += GetFullDataEntityName();");
        if (_generationProject.UseWithNolock)
            output.AppendLine("            _commandText += Constants.SQL_WITH_NOLOCK;");

        output.AppendLine("            _commandText += WhereParameter.ToString();");
        output.AppendLine("            _commandText += GroupByParameter.ToString();");
        output.AppendLine("            BuildParameterValuesList(WhereParameter);");
        output.AppendLine("            _commandText += OrderByParameter.ToString();");
        output.AppendLine("            _commandText = _commandText.Replace(Constants.INPUT_PARAMETER, _parameterPrefix);");
        output.AppendLine("            _datareader = base.ExecuteReader(_commandText, _transaction);");

        output.AppendLine("            _itemList = MapDataReaderToDataItem(_datareader, _dataItem);");

        output.AppendLine("            return _itemList;");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("        finally");
        output.AppendLine("        {");
        output.AppendLine("            if (_datareader != null)");
        output.AppendLine("                _datareader.Close();");
        output.AppendLine("            if (_connection.State == ConnectionState.Open && _transaction == null)");
        output.AppendLine("                _connection.Close();");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Deletes records from table");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    internal virtual Int64 Delete()");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            _commandText = Constants.SQL_DELETE + Constants.SQL_FROM;");
        output.AppendLine("            _commandText += GetFullDataEntityName();");
        output.AppendLine("            _commandText += WhereParameter.ToString();");
        output.AppendLine("            BuildParameterValuesList(WhereParameter);");
        output.AppendLine("            _commandText = _commandText.Replace(Constants.INPUT_PARAMETER, _parameterPrefix);");

        output.AppendLine("            return ExecuteNonQuery(_commandText, _transaction,false);");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");

        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Deletes a record represented by IDataItem from Table");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "Item" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    internal virtual Int64 DeleteItem(IDataItem Item)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            _commandText = Constants.SQL_DELETE + Constants.SQL_FROM;");
        output.AppendLine("            _commandText += GetFullDataEntityName();");
        output.AppendLine("            _commandText += Constants.SQL_WHERE; ");
        output.AppendLine("            _commandText += GetKeysList(Item);");
        output.AppendLine("            BuildIDataItemParameterValuesList(Item);");
        output.AppendLine("            return ExecuteNonQuery(_commandText, _transaction,false);");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");

        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Elimina todos los registros de la base, con opcion a ejecutar un 'Truncate'.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "Truncate" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    internal virtual Int64 Clear(bool truncate)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            if (truncate)");
        output.AppendLine("            {");
        output.AppendLine("                 _commandText = Constants.SQL_TRUNCATE_TABLE;");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                 _commandText = Constants.SQL_DELETE + Constants.SQL_FROM;");
        output.AppendLine("            }");

        output.AppendLine("            _commandText += GetFullDataEntityName();");

        output.AppendLine("            return ExecuteNonQuery(_commandText, _transaction,false);");

        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw ex;");
        output.AppendLine("        }");
        output.AppendLine("    }");

        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Updates a record in Table represented by IDataItem DataEntity");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "Item" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    internal virtual Int64 Update(IDataItem Item)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            _commandText = Constants.SQL_UPDATE;");
        output.AppendLine("            _commandText += GetFullDataEntityName();");
        output.AppendLine("            _commandText += Constants.SQL_SET + GetSetList(Item);");
        output.AppendLine("            _commandText += Constants.SQL_WHERE;");
        output.AppendLine("            _commandText += GetKeysList(Item);");
        output.AppendLine("            BuildIDataItemParameterValuesList(Item, true);");
        output.AppendLine("            return ExecuteNonQuery(_commandText, _transaction,false);");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Begins a Transaction.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    public new IDbTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel = IsolationLevel.Unspecified)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            if (_connection == null)");
        output.AppendLine("            {");
        output.AppendLine("                GetConnection();");
        output.AppendLine("                _transaction = _connection.BeginTransaction(isolationLevel);");
        output.AppendLine("                return _transaction;");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                throw (new Exception(" + System.Convert.ToChar(34) + "Transaction already opened" + System.Convert.ToChar(34) + "));");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Ends a Transaction with commit  or rollback passed as boolean.");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "commit" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "validateOpenedTransaction" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    public new bool EndTransaction(bool commit, bool validateOpenedTransaction = true)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            if (_connection == null && validateOpenedTransaction == true)");
        output.AppendLine("            {");
        output.AppendLine("                throw (new Exception(" + System.Convert.ToChar(34) + "Transaction not opened yet" + System.Convert.ToChar(34) + "));");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                if (commit)");
        output.AppendLine("                {");
        output.AppendLine("                    _transaction.Commit();");
        output.AppendLine("                }");
        output.AppendLine("                else");
        output.AppendLine("                {");
        output.AppendLine("                    _transaction.Rollback();");
        output.AppendLine("                }");
        output.AppendLine("                _connection.Close();");
        output.AppendLine("                _connection = null;");
        output.AppendLine("                _transaction = null;");
        output.AppendLine("                return true;");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("}");

        SaveOutputToFile("DataHandler.cs", output, true);
        output = new StringBuilder();
    }
    private void BuildCore(string nameSpace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);

        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Linq;");
        output.AppendLine("using System.Reflection;");
        output.AppendLine("using System.Text;");
        output.AppendLine("using System.Threading.Tasks;");
        output.AppendLine("namespace " + nameSpace);
        output.AppendLine("{");
        output.AppendLine("    public static class Utilities");
        output.AppendLine("    {");
        output.AppendLine("        public static List<IDataItem> GetDataItems()");
        output.AppendLine("        {");
        output.AppendLine("            List<IDataItem> returnValue = new List<IDataItem>();");
        output.AppendLine("");
        output.AppendLine("            var instances = from t in Assembly.GetExecutingAssembly().GetTypes()");
        output.AppendLine("                            where t.GetInterfaces().Contains(typeof(IDataItem))");
        output.AppendLine("                                     && t.GetConstructor(Type.EmptyTypes) != null");
        output.AppendLine("                            select Activator.CreateInstance(t) as IDataItem;");
        output.AppendLine("");
        output.AppendLine("            returnValue.AddRange(instances);");
        output.AppendLine("");
        output.AppendLine("            return returnValue ;");
        output.AppendLine("        }");
        output.AppendLine("");
        output.AppendLine("        public static List<IDataHandler> GetDataHandlers()");
        output.AppendLine("        {");
        output.AppendLine("            List<IDataHandler> returnValue = new List<IDataHandler>();");
        output.AppendLine("");
        output.AppendLine("            var instances = from t in Assembly.GetExecutingAssembly().GetTypes()");
        output.AppendLine("                            where t.GetInterfaces().Contains(typeof(IDataHandler))");
        output.AppendLine("                                     && t.GetConstructor(Type.EmptyTypes) != null");
        output.AppendLine("                            select Activator.CreateInstance(t) as IDataHandler;");
        output.AppendLine("");
        output.AppendLine("            returnValue.AddRange(instances);");
        output.AppendLine("            ");
        output.AppendLine("            return returnValue ;");
        output.AppendLine("        }");
        output.AppendLine("        public static IDataHandler GetDataHandlerByName(string name)");
        output.AppendLine("        {");
        output.AppendLine("            return GetDataHandlers().Where(x => x.ToString().Equals(name)).FirstOrDefault();");
        output.AppendLine("        }");
        output.AppendLine("");
        output.AppendLine("        /// <summary>");
        output.AppendLine("        /// Usage : ExecuteWithTimeMeasurement(() => m.Items()).ToString(); ");
        output.AppendLine("        /// </summary>");
        output.AppendLine("        /// <param name=" + System.Convert.ToChar(34) + "action" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("        /// <returns></returns>");
        output.AppendLine("        public static long ExecuteWithTimeMeasurement(Action action)");
        output.AppendLine("        {");
        output.AppendLine("            if (action == null) throw new ArgumentNullException();");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                var sw = new System.Diagnostics.Stopwatch();");
        output.AppendLine("                sw.Start();");
        output.AppendLine("                action();");
        output.AppendLine("                sw.Stop();");
        output.AppendLine("                return sw.ElapsedMilliseconds;");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("");
        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("");


        SaveOutputToFile("Core.cs", output, true);
    }
    private void BuildInterfaces()
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System.Data;");
        output.AppendLine("public interface IDataHandler{ IDbTransaction GetTransaction(); IDbConnection GetConnection();}");
        output.AppendLine("public interface IDataItem  { }");
        output.AppendLine("public interface IDataField  { }");
        output.AppendLine("public interface IRelationsDataITem : IDataItem { }");
        output.AppendLine("public interface IMappedProcedure { }");
        SaveOutputToFile("Interfaces.cs", output, true);
        output = new StringBuilder();
    }
    private void BuildParameterBase(string _namespace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("public class ParameterItem");
        output.AppendLine("{");
        output.AppendLine("	public string Column  {get;set;}");
        output.AppendLine("	public string Operand {get;set;}");
        output.AppendLine("    public string Name { get; set; }");
        output.AppendLine("    public List<ParameterItemValue> Values {get;set;}");
        output.AppendLine("	public string FullQuery  {get;set;}");
        output.AppendLine("	public bool IsFieldParameter  {get;set;} = true;");

    output.AppendLine("    public ParameterItem(String fullQuery, bool isFieldParameter = true)");
        output.AppendLine("	{");
        output.AppendLine("		FullQuery = fullQuery;");
        output.AppendLine("		IsFieldParameter = isFieldParameter;");
        output.AppendLine("	}");
        output.AppendLine("    public ParameterItem(string fullQuery, string column, string operand, string name, List<ParameterItemValue> values, bool isFieldParameter = true)");
        output.AppendLine("	{");
        output.AppendLine("		Operand   = operand;");
        output.AppendLine("		Values     = values;");
        output.AppendLine("        Name = name;");
        output.AppendLine("		FullQuery = fullQuery;");
        output.AppendLine("		IsFieldParameter = isFieldParameter;");
        output.AppendLine("	}");
        output.AppendLine("}");
        output.AppendLine("public class ParameterItemValue");
        output.AppendLine("{");
        output.AppendLine("    public string Name { get; set; }");
        output.AppendLine("    public object Value { get; set; }");
        output.AppendLine("    public ParameterItemValue()");
        output.AppendLine("    {");
        output.AppendLine("    }");
        output.AppendLine("    public ParameterItemValue(string name, object value)");
        output.AppendLine("    {");
        output.AppendLine("        Value = value;");
        output.AppendLine("        Name = name;");
        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("public class WhereParameterBase");
        output.AppendLine("{");
        output.AppendLine("    public WhereParameterBase()");
        output.AppendLine("    {");
        output.AppendLine("        _whereParameters = new List<ParameterItem>();");
        output.AppendLine("    }");
        output.AppendLine("    internal List<ParameterItem> _whereParameters = new List<ParameterItem>();");
        output.AppendLine("    private const string _WHERE = " + System.Convert.ToChar(34) + " WHERE " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _BETWEEN = " + System.Convert.ToChar(34) + " BETWEEN " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _EQUAL = " + System.Convert.ToChar(34) + " = " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _GREATER_THAN = " + System.Convert.ToChar(34) + " > " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _GREATER_THAN_OR_EQUAL = " + System.Convert.ToChar(34) + " >= " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _IN_ = " + System.Convert.ToChar(34) + " IN " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _IS_NOT_NULL = " + System.Convert.ToChar(34) + " IS NOT NULL " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _IS_NULL = " + System.Convert.ToChar(34) + " IS NULL " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _LESS_THAN = " + System.Convert.ToChar(34) + " < " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _LESS_THAN_OR_EQUAL = " + System.Convert.ToChar(34) + " <= " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _LIKE = " + System.Convert.ToChar(34) + " LIKE " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _NOT_EQUAL = " + System.Convert.ToChar(34) + " <> " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _NOT_IN = " + System.Convert.ToChar(34) + " NOT IN " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _NOT_LIKE = " + System.Convert.ToChar(34) + " NOT LIKE " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private int counter = -1;");
        output.AppendLine("    protected int GetNextParameterIndex()");
        output.AppendLine("    {");
        output.AppendLine("        counter++;");
        output.AppendLine("        return counter;");
        output.AppendLine("    }");
        output.AppendLine("    protected int GetCurrentParameterIndex()");
        output.AppendLine("    {");
        output.AppendLine("        return counter;");
        output.AppendLine("    }");
        output.AppendLine("    internal string GetSQL()");
        output.AppendLine("    {");
        output.AppendLine("        string _buff = String.Empty;");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            foreach (ParameterItem parameterItem in _whereParameters)");
        output.AppendLine("            {");
        output.AppendLine("                _buff += parameterItem.FullQuery;");
        output.AppendLine("            }");
        output.AppendLine("            if (_buff.Length > 0)");
        output.AppendLine("            {");
        output.AppendLine("                return _WHERE + _buff;");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                return null;");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw ex;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected string GetOperand(" + _namespace + ".sqlEnum.OperandEnum Operand)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            switch (Operand)");
        output.AppendLine("            {");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.Between:");
        output.AppendLine("                    return _BETWEEN;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.Equal:");
        output.AppendLine("                    return _EQUAL;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.GreaterThan:");
        output.AppendLine("                    return _GREATER_THAN;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.GreaterThanOrEqual:");
        output.AppendLine("                    return _GREATER_THAN_OR_EQUAL;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.In:");
        output.AppendLine("                    return _IN_;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.IsNotNull:");
        output.AppendLine("                    return _IS_NOT_NULL;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.IsNull:");
        output.AppendLine("                    return _IS_NULL;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.LessThan:");
        output.AppendLine("                    return _LESS_THAN;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.LessThanOrEqual:");
        output.AppendLine("                    return _LESS_THAN_OR_EQUAL;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.Like:");
        output.AppendLine("                    return _LIKE;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.NotEqual:");
        output.AppendLine("                    return _NOT_EQUAL;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.NotIn:");
        output.AppendLine("                    return _NOT_IN;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.NotLike:");
        output.AppendLine("                    return _NOT_LIKE;");
        output.AppendLine("            }");
        output.AppendLine("            return null;");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw ex;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    public void Clear()");
        output.AppendLine("    {");
        output.AppendLine("        _whereParameters.Clear();");
        output.AppendLine("    }");
        output.AppendLine("    public Int64 Count");
        output.AppendLine("    {");
        output.AppendLine("        get { return _whereParameters.Count; }");
        output.AppendLine("    }");
        output.AppendLine("    public override string ToString()");
        output.AppendLine("    {");
        output.AppendLine("        return GetSQL();");
        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("public class OrderByParameterBase");
        output.AppendLine("{");
        output.AppendLine("    private const string _COLON = " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _ORDER_BY = " + System.Convert.ToChar(34) + " ORDER BY " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    protected List<ParameterItem> _orderByParameters = new List<ParameterItem>();");
        output.AppendLine("    internal string GetSQL() // Esto cambia por la iteración de los ParameterItem");
        output.AppendLine("    {");
        output.AppendLine("        string _buff = String.Empty;");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            foreach (ParameterItem param in _orderByParameters)");
        output.AppendLine("            {");
        output.AppendLine("                _buff += param.FullQuery + _COLON;");
        output.AppendLine("            }");
        output.AppendLine("            if (_buff != null && _buff.Length > 0)");
        output.AppendLine("            {");
        output.AppendLine("                return _ORDER_BY + _buff.Substring(0, _buff.Length - 1);");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                return null;");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw ex;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    public void Clear()");
        output.AppendLine("    {");
        output.AppendLine("        _orderByParameters.Clear();");
        output.AppendLine("    }");
        output.AppendLine("    public long Count");
        output.AppendLine("    {");
        output.AppendLine("        get { return _orderByParameters.Count; }");
        output.AppendLine("    }");
        output.AppendLine("    public override string ToString()");
        output.AppendLine("    {");
        output.AppendLine("        return GetSQL();");
        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("public class GroupByParameterBase");
        output.AppendLine("{");
        output.AppendLine("    private const string _COLON = " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _GROUP_BY = " + System.Convert.ToChar(34) + " GROUP BY " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    protected List<ParameterItem> _groupByParameters = new List<ParameterItem>();");
        output.AppendLine("    internal string GetSQL() // Esto cambia por la iteración de los ParameterItem");
        output.AppendLine("    {");
        output.AppendLine("        string _buff = String.Empty;");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            foreach (ParameterItem param in _groupByParameters)");
        output.AppendLine("            {");
        output.AppendLine("                _buff += param.FullQuery + _COLON;");
        output.AppendLine("            }");
        output.AppendLine("            if (_buff != null && _buff.Length > 0)");
        output.AppendLine("            {");
        output.AppendLine("                return _GROUP_BY + _buff.Substring(0, _buff.Length - 1);");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                return null;");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw ex;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    public void Clear()");
        output.AppendLine("    {");
        output.AppendLine("        _groupByParameters.Clear();");
        output.AppendLine("    }");
        output.AppendLine("    public long Count");
        output.AppendLine("    {");
        output.AppendLine("        get { return _groupByParameters.Count; }");
        output.AppendLine("    }");
        output.AppendLine("    public override string ToString()");
        output.AppendLine("    {");
        output.AppendLine("        return GetSQL();");
        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("public  class AggregateParameterBase");
        output.AppendLine("{");
        output.AppendLine("    private const string _AVG = " + System.Convert.ToChar(34) + " AVG " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _COUNT = " + System.Convert.ToChar(34) + " COUNT " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _MAX = " + System.Convert.ToChar(34) + " MAX " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _MIN = " + System.Convert.ToChar(34) + " MIN " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _STDEV = " + System.Convert.ToChar(34) + " STDEV " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _SUM = " + System.Convert.ToChar(34) + " SUM " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _VAR = " + System.Convert.ToChar(34) + " VAR " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    private const string _COLON = " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("    protected List<ParameterItem> _aggregateParameters = new List<ParameterItem>();");
        output.AppendLine("    protected string GetOperand(" + _namespace + ".sqlEnum.FunctionEnum Operand)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            switch (Operand)");
        output.AppendLine("            {");
        output.AppendLine("                case " + _namespace + ".sqlEnum.FunctionEnum.Avg:");
        output.AppendLine("                    return _AVG;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.FunctionEnum.Count:");
        output.AppendLine("                    return _COUNT;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.FunctionEnum.Max:");
        output.AppendLine("                    return _MAX;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.FunctionEnum.Min:");
        output.AppendLine("                    return _MIN;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.FunctionEnum.StdDev:");
        output.AppendLine("                    return _STDEV;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.FunctionEnum.Sum:");
        output.AppendLine("                    return _SUM;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.FunctionEnum.Var:");
        output.AppendLine("                    return _VAR;");
        output.AppendLine("            }");
        output.AppendLine("            return null;");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw ex;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    internal string GetSQL() ");
        output.AppendLine("    {");
        output.AppendLine("        string _buff = String.Empty;");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            foreach (ParameterItem param in _aggregateParameters)");
        output.AppendLine("            {");
        output.AppendLine("                _buff += param.FullQuery + _COLON;");
        output.AppendLine("            }");
        output.AppendLine("            if (_buff != null && _buff.Length > 0)");
        output.AppendLine("            {");
        output.AppendLine("                return _buff.Substring(0, _buff.Length - 1);");
        output.AppendLine("            }");
        output.AppendLine("            else");
        output.AppendLine("            {");
        output.AppendLine("                return null;");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw ex;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    public void Clear()");
        output.AppendLine("    {");
        output.AppendLine("        _aggregateParameters.Clear();");
        output.AppendLine("    }");
        output.AppendLine("    public long Count");
        output.AppendLine("    {");
        output.AppendLine("        get { return _aggregateParameters.Count; }");
        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("public class WhereParameter : WhereParameterBase");
        output.AppendLine("{");
        output.AppendLine("    public WhereParameter()");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("    }");
        output.AppendLine("    internal void Add(String column, " + _namespace + ".sqlEnum.OperandEnum operand, dynamic values)");
        output.AppendLine("    {");
        output.AppendLine("        string operandString = base.GetOperand(operand);");
        output.AppendLine("        List<ParameterItemValue> _values = new List<ParameterItemValue>();");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            string _buff = String.Empty;");
        output.AppendLine("            if (values != null)");
        output.AppendLine("            {");
        output.AppendLine("                if(values.GetType().ToString().StartsWith(Constants.SYSTEM_COLLECTIONS)) ");
        output.AppendLine("                {");
        output.AppendLine("                        foreach (object value in values)");
        output.AppendLine("                        {");
        output.AppendLine("                            _buff += Constants.INPUT_PARAMETER + GetNextParameterIndex().ToString() + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("                            _values.Add(new ParameterItemValue(Constants.INPUT_PARAMETER + GetCurrentParameterIndex().ToString(), value));");

        output.AppendLine("                        }");
        output.AppendLine("                 }");
        output.AppendLine("                 else ");
        output.AppendLine("                 {");
        output.AppendLine("                        _buff += Constants.INPUT_PARAMETER + GetNextParameterIndex().ToString() + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("                        _values.Add(new ParameterItemValue(Constants.INPUT_PARAMETER + GetCurrentParameterIndex().ToString(), values));");
        output.AppendLine("                }");
        output.AppendLine("            }");
        output.AppendLine("            switch (operand)");
        output.AppendLine("            {");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.In:");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.NotIn:");
        output.AppendLine("                      if( _buff.Length > 0)");
        output.AppendLine("                       _whereParameters.Add(new ParameterItem(column + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + operandString + " + System.Convert.ToChar(34) + " (" + System.Convert.ToChar(34) + " + _buff.Substring(0, _buff.Length - 1) + " + System.Convert.ToChar(34) + ")" + System.Convert.ToChar(34) + ", column, operandString, column, _values));");
        output.AppendLine("                    break;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.IsNull:");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.IsNotNull:");
        output.AppendLine("                    _whereParameters.Add(new ParameterItem(column + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + operandString, column, operandString, column, _values));");
        output.AppendLine("                    break;");
        output.AppendLine("                default:");
        output.AppendLine("                      if( _buff.Length > 0)");
        output.AppendLine("                    _whereParameters.Add(new ParameterItem(column + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + operandString + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + _buff.Substring(0, _buff.Length - 1), column, operandString, column, _values));");
        output.AppendLine("                    break;");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw ex;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    internal void Add(" + _namespace + ".sqlEnum.ConjunctionEnum Conjunction, String column, " + _namespace + ".sqlEnum.OperandEnum operand, dynamic values)");
        output.AppendLine("    {");
        output.AppendLine("        string operandString = base.GetOperand(operand);");
        output.AppendLine("        List<ParameterItemValue> _values = new List<ParameterItemValue>();");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            string _buff = String.Empty;");
        output.AppendLine("            if (values != null)");
        output.AppendLine("            {");

        output.AppendLine("                if(values.GetType().ToString().StartsWith(Constants.SYSTEM_COLLECTIONS)) ");
        output.AppendLine("                {");

        output.AppendLine("                        foreach (object value in values)");
        output.AppendLine("                        {");
        output.AppendLine("                            _buff += Constants.INPUT_PARAMETER + GetNextParameterIndex().ToString() + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("                            _values.Add(new ParameterItemValue(Constants.INPUT_PARAMETER + GetCurrentParameterIndex().ToString(), value));");
        output.AppendLine("                        }");

        output.AppendLine("                 }");
        output.AppendLine("                 else ");
        output.AppendLine("                 {");
        output.AppendLine("                        _buff += Constants.INPUT_PARAMETER + GetNextParameterIndex().ToString() + " + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + ";");
        output.AppendLine("                        _values.Add(new ParameterItemValue(Constants.INPUT_PARAMETER + GetCurrentParameterIndex().ToString(), values));");
        output.AppendLine("                }");
        output.AppendLine("            }");
        output.AppendLine("            switch (operand)");
        output.AppendLine("            {");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.In:");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.NotIn:");
        output.AppendLine("                      if( _buff.Length > 0)");
        output.AppendLine("                         _whereParameters.Add(new ParameterItem(" + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + Enum.GetName(typeof(" + _namespace + ".sqlEnum.ConjunctionEnum), Conjunction) + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + column + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + operandString + " + System.Convert.ToChar(34) + " (" + System.Convert.ToChar(34) + " + _buff.Substring(0, _buff.Length - 1) + " + System.Convert.ToChar(34) + ")" + System.Convert.ToChar(34) + ", column, operandString, column, _values));");

        output.AppendLine("                    break;");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.IsNull:");
        output.AppendLine("                case " + _namespace + ".sqlEnum.OperandEnum.IsNotNull:");
        output.AppendLine("                    _whereParameters.Add(new ParameterItem(" + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + Enum.GetName(typeof(" + _namespace + ".sqlEnum.ConjunctionEnum), Conjunction) + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + column + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + operandString, column, operandString, column, _values));");
        output.AppendLine("                    break;");
        output.AppendLine("                default:");
        output.AppendLine("                      if( _buff.Length > 0)");
        output.AppendLine("                         _whereParameters.Add(new ParameterItem(" + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + Enum.GetName(typeof(" + _namespace + ".sqlEnum.ConjunctionEnum), Conjunction) + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + column + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + operandString + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + _buff.Substring(0, _buff.Length - 1), column, operandString, column, _values));");

        output.AppendLine("                    break;");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw ex;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    internal void Add(String betweenColumn, object fromValue, object toValue)");
        output.AppendLine("    {");
        output.AppendLine("        List<ParameterItemValue> _values = new List<ParameterItemValue>();");
        output.AppendLine("        string firstParameter = GetNextParameterIndex().ToString();");
        output.AppendLine("        string secondParameter = GetNextParameterIndex().ToString();");
        output.AppendLine("        _values.Add(new ParameterItemValue(Constants.INPUT_PARAMETER + firstParameter, fromValue));");
        output.AppendLine("        _values.Add(new ParameterItemValue(Constants.INPUT_PARAMETER + secondParameter, toValue));");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            _whereParameters.Add(new ParameterItem(betweenColumn + " + System.Convert.ToChar(34) + " BETWEEN " + System.Convert.ToChar(34) + " + Constants.INPUT_PARAMETER + firstParameter + " + System.Convert.ToChar(34) + " AND " + System.Convert.ToChar(34) + " + Constants.INPUT_PARAMETER + secondParameter, betweenColumn, " + System.Convert.ToChar(34) + "Between" + System.Convert.ToChar(34) + ", betweenColumn, _values));");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw ex;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    internal void Add(" + _namespace + ".sqlEnum.ConjunctionEnum Conjunction, String betweenColumn, object fromValue, object toValue)");
        output.AppendLine("    {");
        output.AppendLine("        if (_whereParameters.Count > 0)");
        output.AppendLine("        {");
        output.AppendLine("        List<ParameterItemValue> _values = new List<ParameterItemValue>();");
        output.AppendLine("        string firstParameter = GetNextParameterIndex().ToString();");
        output.AppendLine("        string secondParameter = GetNextParameterIndex().ToString();");
        output.AppendLine("        _values.Add(new ParameterItemValue(Constants.INPUT_PARAMETER + firstParameter, fromValue));");
        output.AppendLine("        _values.Add(new ParameterItemValue(Constants.INPUT_PARAMETER + secondParameter, toValue));");
        output.AppendLine("        _whereParameters.Add(new ParameterItem(" + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + Enum.GetName(typeof(" + _namespace + ".sqlEnum.ConjunctionEnum), Conjunction) + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + betweenColumn + " + System.Convert.ToChar(34) + " BETWEEN " + System.Convert.ToChar(34) + " + Constants.INPUT_PARAMETER + firstParameter + " + System.Convert.ToChar(34) + " AND " + System.Convert.ToChar(34) + " + Constants.INPUT_PARAMETER + secondParameter, betweenColumn, " + System.Convert.ToChar(34) + "Between" + System.Convert.ToChar(34) + ", betweenColumn, _values));");
        output.AppendLine("        }");
        output.AppendLine("        else");
        output.AppendLine("        {");
        output.AppendLine("            throw new Exception(" + System.Convert.ToChar(34) + "Add: Overload Error." + System.Convert.ToChar(34) + ");");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    internal void OpenParentheses()");
        output.AppendLine("    {");
        output.AppendLine("        _whereParameters.Add(new ParameterItem(Constants.SQL_OPENPARENTHESES, false));");
        output.AppendLine("    }");
        output.AppendLine("    internal void CloseParentheses()");
        output.AppendLine("    {");
        output.AppendLine("        _whereParameters.Add(new ParameterItem(Constants.SQL_CLOSEPARENTHESES, false));");
        output.AppendLine("    }");
        output.AppendLine("    internal void AddConjunction(" + _namespace + ".sqlEnum.ConjunctionEnum Conjunction)");
        output.AppendLine("    {");
        output.AppendLine("        _whereParameters.Add(new ParameterItem(" + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + Enum.GetName(typeof(" + _namespace + ".sqlEnum.ConjunctionEnum), Conjunction) + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + ", false));");
        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("public class OrderByParameter : OrderByParameterBase");
        output.AppendLine("{");
        output.AppendLine("    internal void Add(String Column, " + _namespace + ".sqlEnum.DirEnum Direction = " + _namespace + ".sqlEnum.DirEnum.ASC)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            base._orderByParameters.Add(new ParameterItem(Column + " + System.Convert.ToChar(34) + " " + System.Convert.ToChar(34) + " + Enum.GetName(typeof(" + _namespace + ".sqlEnum.DirEnum), Direction)));");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw ex;");
        output.AppendLine("        }");

        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("public class GroupByParameter : GroupByParameterBase");
        output.AppendLine("{");
        output.AppendLine("    internal void Add(String Column)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            base._groupByParameters.Add(new ParameterItem(Column));");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw ex;");
        output.AppendLine("        }");

        output.AppendLine("    }");
        output.AppendLine("}");

        output.AppendLine("public class AggregateParameter : AggregateParameterBase");
        output.AppendLine("{");
        output.AppendLine("    internal void Add(" + _namespace + ".sqlEnum.FunctionEnum AggregateFunction, String Column)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            _aggregateParameters.Add(new ParameterItem(GetOperand(AggregateFunction) + " + System.Convert.ToChar(34) + "( " + System.Convert.ToChar(34) + " + Column + " + System.Convert.ToChar(34) + " )" + System.Convert.ToChar(34) + "));");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw ex;");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("}");

        SaveOutputToFile("ParameterBase.cs", output, true);
        output = new StringBuilder();
    }
    private void BuildProcedureDataHandler()
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);

        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Data;");
        output.AppendLine("using System.Collections;");
        output.AppendLine("using System.Reflection;");
        output.AppendLine("public class ProcedureDataHandler : DataHandlerBase");
        output.AppendLine("{");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// No constructor");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public ProcedureDataHandler()");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Constructor with IDataItem");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public ProcedureDataHandler(IDataItem dataItem)");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        _dataItem = dataItem;");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// MetFhod for executing Stored Procedure");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "parameters" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    internal virtual List<IDataItem> Items(List<ParameterItemValue> parameters)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            _itemList.Clear();");
        output.AppendLine("            _commandText = GetFullDataEntityName();");
        output.AppendLine("            _parameterizedValues.AddRange(parameters);");
        output.AppendLine("            _datareader = ExecuteReader(_commandText, _transaction, CommandType.StoredProcedure);");
        output.AppendLine("            _itemList = MapDataReaderToDataItem(_datareader, _dataItem);");
        output.AppendLine("");
        output.AppendLine("            return _itemList;");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("        finally");
        output.AppendLine("        {");
        output.AppendLine("            if (_datareader != null)");
        output.AppendLine("                _datareader.Close();");
        output.AppendLine("            if (_connection.State == ConnectionState.Open  && _transaction == null)");
        output.AppendLine("                _connection.Close();");
        output.AppendLine("        }");
        output.AppendLine("    }");

        output.AppendLine("}");
        output.AppendLine("");


        SaveOutputToFile("ProcedureDataHandler.cs", output, true);
        output = new StringBuilder();
    }
    private void BuildMappedProcedureDataHandler()
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);

        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Data;");
        output.AppendLine("using System.Collections;");
        output.AppendLine("using System.Reflection;");
        output.AppendLine("public class MappedProcedureDataHandler : DataHandlerBase");
        output.AppendLine("{");
        output.AppendLine("    private List<IDataItem> _dataItems;");
        output.AppendLine("");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// No constructor");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public MappedProcedureDataHandler()");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Constructor with List of IDataItem");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public MappedProcedureDataHandler(IDataItem dataItem, List<IDataItem> dataItems)");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        _dataItem = dataItem;");
        output.AppendLine("        _dataItems = dataItems;");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Method for executing mapped Stored Procedure");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "parameters" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    internal virtual List<List<IDataItem>> Items(List<ParameterItemValue> parameters)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            _itemList.Clear();");
        output.AppendLine("            _commandText = GetFullDataEntityName();");
        output.AppendLine("            _parameterizedValues.AddRange(parameters);");
        output.AppendLine("            _datareader = ExecuteReader(_commandText, _transaction, CommandType.StoredProcedure);");
        output.AppendLine("            List<List<IDataItem>> results = new List<List<IDataItem>>();");
        output.AppendLine("");
        output.AppendLine("            foreach (IDataItem item in _dataItems)");
        output.AppendLine("            {");
        output.AppendLine("                results.Add(mMapDataReaderToDataItem(_datareader, item));");
        output.AppendLine("");
        output.AppendLine("                _datareader.NextResult();");
        output.AppendLine("            }");
        output.AppendLine("");
        output.AppendLine("            return results;");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("        finally");
        output.AppendLine("        {");
        output.AppendLine("            if (_datareader != null)");
        output.AppendLine("                _datareader.Close();");
        output.AppendLine("            if (_connection.State == ConnectionState.Open  && _transaction == null)");
        output.AppendLine("                _connection.Close();");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("    protected virtual List<IDataItem> mMapDataReaderToDataItem<IDataItem>(IDataReader dr, IDataItem dataItem) //where IDataItem : new()");
        output.AppendLine("    {");
        output.AppendLine("        Type dataItemType = dataItem.GetType();");
        output.AppendLine("        PropertyInfo[] properties = dataItemType.GetProperties();");
        output.AppendLine("        Hashtable hashtable = new Hashtable();");
        output.AppendLine("        List<IDataItem> entities = new List<IDataItem>();");
        output.AppendLine("        foreach (PropertyInfo info in properties)");
        output.AppendLine("        {");
        output.AppendLine("            hashtable[info.Name] = info;");
        output.AppendLine("        }");
        output.AppendLine("        while (dr.Read())");
        output.AppendLine("        {");
        output.AppendLine("            IDataItem newObject = (IDataItem)Activator.CreateInstance(dataItemType, false);");
        output.AppendLine("            for (int index = 0; index < dr.FieldCount; index++)");
        output.AppendLine("            {");
        output.AppendLine("                PropertyInfo info = (PropertyInfo)hashtable[dr.GetName(index)];");
        output.AppendLine("                if ((info != null) && info.CanWrite)");
        output.AppendLine("                {");
        output.AppendLine("                    if (!dr.GetValue(index).Equals(System.DBNull.Value))");
        output.AppendLine("                        info.SetValue(newObject, dr.GetValue(index), null);");
        output.AppendLine("                }");
        output.AppendLine("            }");
        output.AppendLine("            entities.Add(newObject);");
        output.AppendLine("        }");
        output.AppendLine("");
        output.AppendLine("        return entities;");
        output.AppendLine("    }");
        output.AppendLine("}");

        SaveOutputToFile("MappedProcedureDataHandler.cs", output, true);
        output = new StringBuilder();
    }
    private void BuildFunctionDataHandler()
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);

        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Data;");
        output.AppendLine("using System.Collections;");
        output.AppendLine("using System.Reflection;");
        output.AppendLine("public class FunctionDataHandler : DataHandlerBase");
        output.AppendLine("{");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// No constructor");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public FunctionDataHandler()");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Constructor with IDataItem");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public FunctionDataHandler(IDataItem dataItem)");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        _dataItem = dataItem;");
        output.AppendLine("    }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Method for executing Stored Function");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "parameters" + System.Convert.ToChar(34) + "></param>");
        output.AppendLine("    /// <returns></returns>");
        output.AppendLine("    internal virtual List<IDataItem> Items(List<ParameterItemValue> parameters)");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            _itemList.Clear();");
        output.AppendLine("            _commandText = GetFullDataEntityName();");
        output.AppendLine("            _parameterizedValues.AddRange(parameters);");
        output.AppendLine("            _datareader = ExecuteReader(_commandText, _transaction, CommandType.StoredProcedure);");
        output.AppendLine("            _itemList = MapDataReaderToDataItem(_datareader, _dataItem);");
        output.AppendLine("");
        output.AppendLine("            return _itemList;");
        output.AppendLine("        }");
        output.AppendLine("        catch (System.Exception ex)");
        output.AppendLine("        {");
        output.AppendLine("            throw (ex);");
        output.AppendLine("        }");
        output.AppendLine("        finally");
        output.AppendLine("        {");
        output.AppendLine("            if (_datareader != null)");
        output.AppendLine("                _datareader.Close();");
        output.AppendLine("            if (_connection.State == ConnectionState.Open && _transaction == null)");
        output.AppendLine("                _connection.Close();");
        output.AppendLine("        }");
        output.AppendLine("    }");

        output.AppendLine("}");
        output.AppendLine("");


        SaveOutputToFile("FunctionDataHandler.cs", output, true);
        output = new StringBuilder();
    }

    private void BuildsqlEnumerations(string _namespace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Data;");
        output.AppendLine("using System.Diagnostics;");
        output.AppendLine("namespace " + _namespace + " {");
        output.AppendLine("public class sqlEnum");
        output.AppendLine("{");
        output.AppendLine("    public enum OperandEnum");
        output.AppendLine("    {");
        output.AppendLine("        Equal,");
        output.AppendLine("        NotEqual,");
        output.AppendLine("        GreaterThan,");
        output.AppendLine("        GreaterThanOrEqual,");
        output.AppendLine("        LessThan,");
        output.AppendLine("        LessThanOrEqual,");
        output.AppendLine("        Like,");
        output.AppendLine("        NotLike,");
        output.AppendLine("        IsNull,");
        output.AppendLine("        IsNotNull,");
        output.AppendLine("        Between,");
        output.AppendLine("        In,");
        output.AppendLine("        NotIn");
        output.AppendLine("    }");

        output.AppendLine("    public enum DirEnum");
        output.AppendLine("    {");
        output.AppendLine("        ASC,");
        output.AppendLine("        DESC");
        output.AppendLine("    }");

        output.AppendLine("    public enum ConjunctionEnum");
        output.AppendLine("    {");
        output.AppendLine("        AND,");
        output.AppendLine("        OR");
        output.AppendLine("    }");

        output.AppendLine("    public enum FunctionEnum");
        output.AppendLine("    {");
        output.AppendLine("        Avg = 1,");
        output.AppendLine("        Count,");
        output.AppendLine("        Max,");
        output.AppendLine("        Min,");
        output.AppendLine("        StdDev,");
        output.AppendLine("        Var,");
        output.AppendLine("        Sum");
        output.AppendLine("    }");

        output.AppendLine(" }");
        output.AppendLine("}");

        output.AppendLine("/// <summary>");
        output.AppendLine("/// Clase DataField - Fecha de Creación : lunes, 07 de octubre de 2013");
        output.AppendLine("/// </summary>");
        output.AppendLine("/// <remarks> Representa una consulta de Funciones de agregación. </remarks>");
        output.AppendLine("public class DataField");
        output.AppendLine("{");
        output.AppendLine("    private string _name;");

        output.AppendLine("    private object _value;");
        output.AppendLine("    public DataField(string Name, object Value)");
        output.AppendLine("    {");
        output.AppendLine("        _name = Name;");
        output.AppendLine("        _value = Value;");
        output.AppendLine("    }");
        output.AppendLine("    public string Name");
        output.AppendLine("    {");
        output.AppendLine("        get { return _name; }");
        output.AppendLine("        set { _name = value; }");
        output.AppendLine("    }");
        output.AppendLine("    public object Value");
        output.AppendLine("    {");
        output.AppendLine("        get { return _value; }");
        output.AppendLine("        set { _value = value; }");
        output.AppendLine("    }");
        output.AppendLine("}");


        SaveOutputToFile("sqlEnumerations.cs", output, true);
        output = new StringBuilder();
    }
    private void BuildAttributes()
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("/// <summary>");
        output.AppendLine("/// ");
        output.AppendLine("/// </summary>");
        output.AppendLine("[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]");
        output.AppendLine("public class DataItemAttributeSchemaName : Attribute");
        output.AppendLine("{");
        output.AppendLine("     public string schemaName { get; set; }");
        output.AppendLine("        public DataItemAttributeSchemaName(string schemaName)");
        output.AppendLine("        {");
        output.AppendLine("            this.schemaName = schemaName;");
        output.AppendLine("        }");
        output.AppendLine("}");
        output.AppendLine("/// <summary>");
        output.AppendLine("/// ");
        output.AppendLine("/// </summary>");
        output.AppendLine("[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]");
        output.AppendLine("public class DataItemAttributeObjectName : Attribute");
        output.AppendLine("{");

        output.AppendLine("    public string ObjectName { get; set; }");
        output.AppendLine("    public string ObjectAlias { get; set; }");
        output.AppendLine("    public DataItemAttributeObjectName(string objectName, string objectAlias)");
        output.AppendLine("    {");
        output.AppendLine("        this.ObjectName = objectName;");
        output.AppendLine("        this.ObjectAlias = objectAlias;");
        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("/// <summary>");
        output.AppendLine("/// ");
        output.AppendLine("/// </summary>");
        output.AppendLine("[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]");
        output.AppendLine("public class DataItemAttributeObjectType : Attribute");
        output.AppendLine("{");
        output.AppendLine("    public enum ObjectTypeEnum");
        output.AppendLine("    {");
        output.AppendLine("        Table,");
        output.AppendLine("        View,");
        output.AppendLine("        Procedure,");
        output.AppendLine("        Function");
        output.AppendLine("    }");
        output.AppendLine("    public ObjectTypeEnum objectType { get; set; }");
        output.AppendLine("    public DataItemAttributeObjectType(ObjectTypeEnum objectType)");
        output.AppendLine("    {");
        output.AppendLine("        this.objectType = objectType;");
        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("/// <summary>");
        output.AppendLine("/// ");
        output.AppendLine("/// </summary>");
        output.AppendLine("[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]");
        output.AppendLine("public class PropertyAttribute : Attribute");
        output.AppendLine("{");
        output.AppendLine("    public enum PropertyAttributeEnum");
        output.AppendLine("    {");
        output.AppendLine("        Pk,");
        output.AppendLine("        Auto,");
        output.AppendLine("        Fk,");
        output.AppendLine("        Key,");
        output.AppendLine("        Display,");
        output.AppendLine("        Exclude,");
        output.AppendLine("        Computed,");
        output.AppendLine("        None");
        output.AppendLine("    }");

        output.AppendLine("    public PropertyAttributeEnum _propertyAttribute { get; set; }");
        output.AppendLine("    public PropertyAttribute(PropertyAttributeEnum propertyAttribute)");
        output.AppendLine("    {");
        output.AppendLine("        this._propertyAttribute = propertyAttribute;");
        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("/// <summary>");
        output.AppendLine("/// ");
        output.AppendLine("/// </summary>");
        output.AppendLine("[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]");
        output.AppendLine("public class PropertyAttributeForeignKeyObjectName : Attribute");
        output.AppendLine("{");

        output.AppendLine("    public string ForeignKeyObjectName { get; set; }");
        output.AppendLine("    public PropertyAttributeForeignKeyObjectName(string foreignKeyObjectName)");
        output.AppendLine("    {");
        output.AppendLine("        this.ForeignKeyObjectName = foreignKeyObjectName;");
        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("/// <summary>");
        output.AppendLine("///   Usage: [DataItemAttributeFieldName(" + System.Convert.ToChar(34) + "Discontinued,Discontinued" + System.Convert.ToChar(34) + ")]");
        output.AppendLine("/// </summary>");
        output.AppendLine("[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]");
        output.AppendLine("public class DataItemAttributeFieldName : Attribute");
        output.AppendLine("{");
        output.AppendLine("     public string FieldName { get; set; }");
        output.AppendLine("     public string FieldFrameworkName { get; set; }");
        output.AppendLine("     public DataItemAttributeFieldName(string fieldName, string fieldFrameworkName)");
        output.AppendLine("     {");
        output.AppendLine("         this.FieldName = fieldName;");
        output.AppendLine("         this.FieldFrameworkName = fieldFrameworkName;");
        output.AppendLine("     }");
        output.AppendLine("}");


        SaveOutputToFile("Attributes.cs", output, true);
        output = new StringBuilder();
    }
    private void BuildConfigurationHandler()
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Configuration;");
        output.AppendLine("internal static class ConfigurationHandler");
        output.AppendLine("{");
        output.AppendLine("     internal static String ConnectionString");
        output.AppendLine("     {");
        output.AppendLine("         get { return ConfigurationManager.ConnectionStrings[Constants.CONNECTIONSTRING].ConnectionString; }");
        output.AppendLine("     }");
        output.AppendLine("     internal static String AdoNetAssemblyName");
        output.AppendLine("     {");
        output.AppendLine("         get { return ConfigurationManager.AppSettings[Constants.ADONETASSEMBLYNAME].ToString(); }");
        output.AppendLine("     }");
        output.AppendLine("     internal static String AdoNetConnectionTypeName");
        output.AppendLine("     {");
        output.AppendLine("         get { return ConfigurationManager.AppSettings[Constants.ADONETCONNECTIONTYPENAME].ToString(); }");
        output.AppendLine("     }");
        output.AppendLine("     internal static Int32 AdoNetCommandTimeout");
        output.AppendLine("     {");
        output.AppendLine("         get { return Convert.ToInt32(ConfigurationManager.AppSettings[Constants.ADONETCOMMANDTIMEOUT].ToString()); }");
        output.AppendLine("     }");
        output.AppendLine("     internal static String ParameterPrefix");
        output.AppendLine("     {");
        output.AppendLine("         get { return ConfigurationManager.AppSettings[Constants.PARAMETERPREFIX].ToString(); }");
        output.AppendLine("     }");
        output.AppendLine("     internal static String PkFunction");
        output.AppendLine("     {");
        output.AppendLine("         get { return ConfigurationManager.AppSettings[Constants.PKFUNCTION].ToString(); }");
        output.AppendLine("     }");
        output.AppendLine("}");

        SaveOutputToFile("ConfigurationHandler.cs", output, true);
        output = new StringBuilder();


    }

    private void BuildConfigurationHandlerNETCore()
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using Microsoft.Extensions.Configuration;");
        output.AppendLine("internal class ConfigurationHandler");
        output.AppendLine("{");
        output.AppendLine("     private static ConfigurationBuilder _configurationBuilder = null;");
        output.AppendLine("     private static IConfigurationRoot _configuration = null;");
        output.AppendLine("     internal static String PasswordKey");
        output.AppendLine("     {");
        output.AppendLine("         get { return getConfiguration(Constants.PASSWORDKEY); }");
        output.AppendLine("     }");
        output.AppendLine("     internal static String ConnectionString");
        output.AppendLine("     {");
        output.AppendLine("         get { return getConfiguration(Constants.CONNECTIONSTRING); }");
        output.AppendLine("     }");
        output.AppendLine("     internal static String AdoNetAssemblyName");
        output.AppendLine("     {");
        output.AppendLine("         get { return getConfiguration(Constants.ADONETASSEMBLYNAME); }");
        output.AppendLine("    }");
        output.AppendLine("     internal static String AdoNetConnectionTypeName");
        output.AppendLine("     {");
        output.AppendLine("         get { return getConfiguration(Constants.ADONETCONNECTIONTYPENAME); }");
        output.AppendLine("    }");
        output.AppendLine("     internal static Int32 AdoNetCommandTimeout");
        output.AppendLine("     {");
        output.AppendLine("         get { return Convert.ToInt32(getConfiguration(Constants.ADONETCOMMANDTIMEOUT)); }");
        output.AppendLine("    }");
        output.AppendLine("     internal static String ParameterPrefix");
        output.AppendLine("     {");
        output.AppendLine("         get { return getConfiguration(Constants.PARAMETERPREFIX); }");
        output.AppendLine("    }");
        output.AppendLine("     internal static String PkFunction");
        output.AppendLine("     {");
        output.AppendLine("         get { return getConfiguration(Constants.PKFUNCTION); }");
        output.AppendLine("    }");
        output.AppendLine("");
        output.AppendLine("   private static string getConfiguration(string configurationEntry)");
        output.AppendLine("   {");
        output.AppendLine("       string? conf = Environment.GetEnvironmentVariable(" + System.Convert.ToChar(34) + _generationProject.EnvironmentConfigurationVariableName + System.Convert.ToChar(34) + ");");
        output.AppendLine("       if (conf != null && conf.ToUpper().Equals(" + System.Convert.ToChar(34) + "ENVIRONMENT" + System.Convert.ToChar(34) + "))");
        output.AppendLine("       {");
        output.AppendLine("           return Environment.GetEnvironmentVariable(" + System.Convert.ToChar(34) + System.Convert.ToChar(34) + " + configurationEntry);");
        output.AppendLine("       }");
        output.AppendLine("       else");
        output.AppendLine("       {");
        output.AppendLine("           var env = Environment.GetEnvironmentVariable(" + System.Convert.ToChar(34) + "ASPNETCORE_ENVIRONMENT" + System.Convert.ToChar(34) + ") == null ? String.Empty :");
        output.AppendLine("               Environment.GetEnvironmentVariable(" + System.Convert.ToChar(34) + "ASPNETCORE_ENVIRONMENT" + System.Convert.ToChar(34) + ") + " + System.Convert.ToChar(34) + "." + System.Convert.ToChar(34) + ";");
        output.AppendLine("           if (_configurationBuilder == null)");
        output.AppendLine("           {");
        output.AppendLine("               _configurationBuilder = new ConfigurationBuilder();");
        output.AppendLine("               _configurationBuilder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)");
        output.AppendLine("                       .AddJsonFile(" + System.Convert.ToChar(34) + "appsettings." + System.Convert.ToChar(34) + " + env + " + System.Convert.ToChar(34) + "json" + System.Convert.ToChar(34) + ", optional: false, reloadOnChange: true);");
        output.AppendLine("               _configuration = _configurationBuilder.Build();");
        output.AppendLine("           }");
        output.AppendLine("           return _configuration.GetSection(" + System.Convert.ToChar(34) + "AppSettings" + System.Convert.ToChar(34) + ").GetSection(configurationEntry).Value.ToString();");
        output.AppendLine("       }");
        output.AppendLine("   }");
        output.AppendLine("}");
        output.AppendLine("");

        SaveOutputToFile("ConfigurationHandler.cs", output, true);
        output = new StringBuilder();

    }
    private void BuildConstants(GenerationProject generationProject)
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("public class Constants {");
        output.AppendLine("    #region DataType Constants");
        output.AppendLine("         internal const string SYSTEM_STRING = " + System.Convert.ToChar(34) + "System.String" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SYSTEM_DATETIME = " + System.Convert.ToChar(34) + "System.DateTime" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SYSTEM_BOOLEAN = " + System.Convert.ToChar(34) + "System.Boolean" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SYSTEM_OBJECT = " + System.Convert.ToChar(34) + "System.Object" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SYSTEM_COLLECTIONS = " + System.Convert.ToChar(34) + "System.Collections" + System.Convert.ToChar(34) + ";");

        output.AppendLine("    #endregion");
        output.AppendLine("    #region Parameter Constants");
        output.AppendLine("         internal const string INPUT_PARAMETER = " + System.Convert.ToChar(34) + "INPUT_PARAMETER" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string OUTPUT_PARAMETER = " + System.Convert.ToChar(34) + "OUTPUT_PARAMETER" + System.Convert.ToChar(34) + ";");
        output.AppendLine("    #endregion");
        output.AppendLine("    #region Configuration Constants");
        output.AppendLine("         internal const string ADONETASSEMBLYNAME = " + System.Convert.ToChar(34) + "AdoNetAssemblyName" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string PASSWORDKEY = " + System.Convert.ToChar(34) + "PasswordKey" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string ADONETCONNECTIONTYPENAME = " + System.Convert.ToChar(34) + "AdoNetConnectionTypeName" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string ADONETCOMMANDTIMEOUT = " + System.Convert.ToChar(34) + "AdoNetCommandTimeout" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string CONNECTIONSTRING = " + System.Convert.ToChar(34) + generationProject.ConnectionStringName + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string PARAMETERPREFIX = " + System.Convert.ToChar(34) + "ParameterPrefix" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string PKFUNCTION = " + System.Convert.ToChar(34) + "PkFunction" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string NEWID = " + System.Convert.ToChar(34) + "@newId" + System.Convert.ToChar(34) + ";");
        output.AppendLine("    #endregion");
        output.AppendLine("    #region CustomProperties Constants");
        output.AppendLine("         internal const string DATAITEMATTRIBUTESCHEMANAME = " + System.Convert.ToChar(34) + "DataItemAttributeSchemaName" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string DATAITEMATTRIBUTEOBJECTTYPE = " + System.Convert.ToChar(34) + "DataItemAttributeObjectType" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string DATAITEMATTRIBUTEOBJECTNAME = " + System.Convert.ToChar(34) + "DataItemAttributeObjectName" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string DATAITEMATTRIBUTEFIELDNAME = " + System.Convert.ToChar(34) + "DataItemAttributeFieldName" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string PROPERTYATTRIBUTE = " + System.Convert.ToChar(34) + "PropertyAttribute" + System.Convert.ToChar(34) + ";");
        output.AppendLine("    #endregion");
        output.AppendLine("    #region reflection custom attributes Constants");
        output.AppendLine("         internal const string SYSTEM_SERIALIZABLEATTRIBUTE = " + System.Convert.ToChar(34) + "System.SerializableAttribute" + System.Convert.ToChar(34) + ";");
        output.AppendLine("    #endregion");
        output.AppendLine("    #region SQL syntax Constants");
        output.AppendLine("         internal const string SQL_EXEC = " + System.Convert.ToChar(34) + "EXEC " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_SELECT = " + System.Convert.ToChar(34) + " SELECT " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_UPDATE = " + System.Convert.ToChar(34) + " UPDATE " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_DELETE = " + System.Convert.ToChar(34) + " DELETE " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_TOP = " + System.Convert.ToChar(34) + " TOP " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_FROM = " + System.Convert.ToChar(34) + " FROM " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_WHERE = " + System.Convert.ToChar(34) + " WHERE " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_SET = " + System.Convert.ToChar(34) + " SET " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_TRUNCATE_TABLE = " + System.Convert.ToChar(34) + " TRUNCATE TABLE " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_INSERT_INTO = " + System.Convert.ToChar(34) + " INSERT INTO " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_VALUES = " + System.Convert.ToChar(34) + " VALUES " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_AND = " + System.Convert.ToChar(34) + " AND " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_OPENPARENTHESES = " + System.Convert.ToChar(34) + " ( " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_CLOSEPARENTHESES = " + System.Convert.ToChar(34) + " ) " + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string SQL_WITH_NOLOCK = " + System.Convert.ToChar(34) + " WITH (NOLOCK) " + System.Convert.ToChar(34) + ";");
        output.AppendLine("    #endregion");
        output.AppendLine("    #region Error Constants");
        output.AppendLine("         internal const string ERROR_CONSTRUCTOR = " + System.Convert.ToChar(34) + "DataHandler (constructor) : Transaction assignment Error." + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string ERROR_TRANSACTION_ALREADY_OPENED = " + System.Convert.ToChar(34) + "Transaction already opened" + System.Convert.ToChar(34) + ";");
        output.AppendLine("         internal const string ERROR_TRANSACTION_NOT_OPENED_YET = " + System.Convert.ToChar(34) + "Transaction not opened yet" + System.Convert.ToChar(34) + ";");
        output.AppendLine("    #endregion");

        output.AppendLine("}");

        SaveOutputToFile("Constants.cs", output, true);
        output = new StringBuilder();
    }


    ////////////////                RELATIONS                           //////////////////////////////////////////
    private void BuildBusinessRelations(MyMeta.IDatabase db, string _namespace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        String definedColumnList = string.Empty;
        String columnList = string.Empty;

        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Linq;");
        output.AppendLine("using System.Text;");

        foreach (MyMeta.ITable entity in db.Tables)
        {
            if (entity.Selected)
            {
                output.AppendLine("	namespace " + _namespace + ".Business.Relations." + GetSchemaName(GetSchemaName(entity.Schema)) + " {");
                output.AppendLine("	    /// <summary>");
                output.AppendLine("	    /// " + entity.Description);
                output.AppendLine("	    /// </summary>");
                output.AppendLine("		public class " + GetFormattedEntityName(entity.Name) + " : RelationsDataHandler");
                output.AppendLine("		{");

                output.AppendLine("				public enum ColumnEnum : int");
                output.AppendLine("                {");
                int columnCount = 0;
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    columnCount += 1;
                    if (columnCount == entity.Columns.Count)
                    {
                        output.AppendLine("					" + GetFormattedEntityName(column.Name));
                    }
                    else
                    {
                        output.AppendLine("					" + GetFormattedEntityName(column.Name) + ",");
                    }
                }

                output.AppendLine("				}");

                output.AppendLine("			   protected List<Entities.Relations." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> _cacheItemList = new List<Entities.Relations." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + ">();");
                output.AppendLine("			   protected List<Entities.Relations." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> _entities = null;");
                output.AppendLine("            public CustomWhereParameter Where { get; set; }");
                output.AppendLine("            public CustomOrderByParameter OrderBy { get; set; }");
                output.AppendLine("            public CustomGroupByParameter GroupBy { get; set; }");
                output.AppendLine("            public CustomAggregateParameter Aggregate { get; set; }");

                output.AppendLine("            public " + GetFormattedEntityName(entity.Name) + "() : base()");
                output.AppendLine("            {");
                output.AppendLine("                base._dataItem = new Entities.Relations." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "();");
                output.AppendLine("                Where = new CustomWhereParameter();");
                output.AppendLine("                OrderBy = new CustomOrderByParameter();");
                output.AppendLine("                GroupBy = new CustomGroupByParameter();");
                output.AppendLine("                Aggregate = new CustomAggregateParameter();");
                output.AppendLine("            }");

                output.AppendLine("            public class CustomAggregateParameter : AggregateParameter");
                output.AppendLine("            {");
                output.AppendLine("                 internal AggregateParameter aggregateParameter = new AggregateParameter();");
                output.AppendLine("                 public void Add(" + _namespace + ".sqlEnum.FunctionEnum functionEnum, ColumnEnum column)");
                output.AppendLine("                     {");
                output.AppendLine("                         this.aggregateParameter.Add(functionEnum, Enum.GetName(typeof(ColumnEnum), column));");
                output.AppendLine("                     }");
                output.AppendLine("            }");

                output.AppendLine("			// Adds to a memory cache to hold pending transactions");
                output.AppendLine("			public void AddToCache(Entities.Relations." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " item)");
                output.AppendLine("			{");
                output.AppendLine("				_cacheItemList.Add(item);");
                output.AppendLine("			}");
                output.AppendLine("			public void UpdateCache()");
                output.AppendLine("			{");
                output.AppendLine("                this.BeginTransaction();");
                output.AppendLine("				foreach(IDataItem item in _cacheItemList)");
                output.AppendLine("					base.Add(item);");
                output.AppendLine("				this.EndTransaction(true);");
                output.AppendLine("			}");
                output.AppendLine("			// Method that accepts arguments corresponding to fields (Those wich aren´t identity.)");
                output.AppendLine("         /// <summary>");
                output.AppendLine("         /// " + GetFormattedEntityName(entity.Name) + " Add Method");
                output.AppendLine("         /// </summary>");
                definedColumnList = string.Empty; columnList = string.Empty;
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    if (!column.IsAutoKey)
                    {
                        if (column.IsInForeignKey && !column.IsInPrimaryKey)
                        {
                            definedColumnList += _namespace + ".Entities.Relations." + GetSchemaName(GetSchemaName(column.ForeignKeys[0].PrimaryTable.Schema)) + "." + column.ForeignKeys[0].PrimaryTable.Name + " " + GetFormattedEntityName(column.Name) + ",";
                            output.AppendLine("         /// <param name='" + _namespace + ".Entities.Relations." + GetSchemaName(GetSchemaName(column.ForeignKeys[0].PrimaryTable.Schema)) + "." + column.ForeignKeys[0].PrimaryTable.Name + " " + GetFormattedEntityName(column.Name) + "'></param>");
                            columnList += GetFormattedEntityName(column.Name) + ",";
                        }
                        else
                        {
                            output.AppendLine("         /// <param name='" + GetFormattedEntityName(column.Name) + "'></param>");
                            definedColumnList += column.LanguageType + " " + GetFormattedEntityName(column.Name) + ",";
                            columnList += GetFormattedEntityName(column.Name) + ",";
                        }
                    }
                }
                if (definedColumnList.Length > 0)
                    definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);
                if (columnList.Length > 0)
                    columnList = columnList.Substring(0, columnList.Length - 1);
                output.AppendLine("         /// <returns>Entities.Relations." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "</returns>");
                output.AppendLine("			public Entities.Relations." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " Add(" + definedColumnList + ") ");
                output.AppendLine("			{");
                output.AppendLine("			  return (Entities.Relations." + GetSchemaName(entity.Schema) + "." + GetFormattedEntityName(entity.Name) + ")base.Add(new Entities.Relations." + GetSchemaName(entity.Schema) + "." + GetFormattedEntityName(entity.Name) + "(" + columnList + "));");
                output.AppendLine("			}");
                output.AppendLine("            public new List<Entities.Relations." + GetSchemaName(entity.Schema) + "." + GetFormattedEntityName(entity.Name) + "> Items()");
                output.AppendLine("            {");
                output.AppendLine("                base.WhereParameter = this.Where.whereParameter;");
                output.AppendLine("                base.OrderByParameter = this.OrderBy.orderByParameter;");
                output.AppendLine("                base.GroupByParameter = this.GroupBy.groupByParameter;");
                output.AppendLine("                base.TopQuantity = this.TopQuantity;");
                output.AppendLine("                base.AnalizeIDataItem();");
                output.AppendLine("                _entities = base.Items().Cast<Entities.Relations." + GetSchemaName(entity.Schema) + "." + GetFormattedEntityName(entity.Name) + ">().ToList<Entities.Relations." + GetSchemaName(entity.Schema) + "." + GetFormattedEntityName(entity.Name) + ">();");
                output.AppendLine("                return _entities;");
                output.AppendLine("            }");
                if (HasPrimaryKey(entity))
                {
                    if (entity.Columns.Count > GetPrimaryKeyCount(entity))
                    {
                        output.AppendLine("            /// <summary>");
                        output.AppendLine("            /// Gets Entities.Relations." + GetSchemaName(entity.Schema) + "." + GetFormattedEntityName(entity.Name) + " items by Pk");
                        output.AppendLine("            /// </summary>");
                        definedColumnList = String.Empty;
                        foreach (MyMeta.IColumn column in entity.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("            /// <param name=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></param>");
                                definedColumnList += column.LanguageType + " " + GetFormattedEntityName(column.Name) + ",";
                            }
                        }
                        output.AppendLine("            /// <returns></returns>");
                        if (definedColumnList.Length > 0)
                            definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);
                        output.AppendLine("            public List<Entities.Relations." + GetSchemaName(entity.Schema) + "." + GetFormattedEntityName(entity.Name) + "> Items(" + definedColumnList + ")");
                        output.AppendLine("            {");
                        output.AppendLine("                this.Where.Clear();");
                        foreach (MyMeta.IColumn column in entity.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("                    if (this.Where.whereParameter.Count == 0)");
                                output.AppendLine("                    {");
                                output.AppendLine("                         this.Where.Add(ColumnEnum." + GetFormattedEntityName(column.Name) + ", " + _namespace + ".sqlEnum.OperandEnum.Equal, " + GetFormattedEntityName(column.Name) + ");");
                                output.AppendLine("                    }");
                                output.AppendLine("                    else");
                                output.AppendLine("                    {");
                                output.AppendLine("                         this.Where.Add(" + _namespace + ".sqlEnum.ConjunctionEnum.AND,ColumnEnum." + GetFormattedEntityName(column.Name) + ", " + _namespace + ".sqlEnum.OperandEnum.Equal, " + GetFormattedEntityName(column.Name) + ");");
                                output.AppendLine("                    }");
                            }
                        }


                        output.AppendLine("                return this.Items();");
                        output.AppendLine("            }");
                    }
                }
                output.AppendLine("            /// <summary>");
                output.AppendLine("            /// Gets items with all fields");
                output.AppendLine("            /// </summary>");
                definedColumnList = string.Empty;
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    output.AppendLine("            /// <param name=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></param>");
                    if (!column.LanguageType.Equals("Byte[]"))
                    {
                        if (column.LanguageType.ToUpper().Equals("STRING"))
                        {
                            definedColumnList += column.LanguageType + " " + GetFormattedEntityName(column.Name) + ",";
                        }
                        else
                        {
                            definedColumnList += column.LanguageType + "? " + GetFormattedEntityName(column.Name) + ",";
                        }
                    }
                }
                output.AppendLine("            /// <returns></returns>");
                if (definedColumnList.Length > 0)
                    definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);
                output.AppendLine("            public List<Entities.Relations." + GetSchemaName(entity.Schema) + "." + GetFormattedEntityName(entity.Name) + "> Items(" + definedColumnList + ")");
                output.AppendLine("            {");
                output.AppendLine("                this.Where.whereParameter.Clear();");

                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    if (!column.LanguageType.Equals("Byte[]"))
                    {
                        output.AppendLine("                if (" + GetFormattedEntityName(column.Name) + " != null)");
                        output.AppendLine("                {");
                        output.AppendLine("                    if (this.Where.whereParameter.Count == 0)");
                        output.AppendLine("                    {");
                        output.AppendLine("                        this.Where.Add(ColumnEnum." + GetFormattedEntityName(column.Name) + ", " + _namespace + ".sqlEnum.OperandEnum.Equal, " + GetFormattedEntityName(column.Name) + ");");
                        output.AppendLine("                    }");
                        output.AppendLine("                    else");
                        output.AppendLine("                    {");
                        output.AppendLine("                        this.Where.Add(" + _namespace + ".sqlEnum.ConjunctionEnum.AND, ColumnEnum." + GetFormattedEntityName(column.Name) + ", " + _namespace + ".sqlEnum.OperandEnum.Equal, " + GetFormattedEntityName(column.Name) + ");");
                        output.AppendLine("                    }");
                        output.AppendLine("                   ");
                        output.AppendLine("                }");
                    }
                }

                output.AppendLine("                return this.Items();");
                output.AppendLine("            }");

                output.AppendLine("            public List<Entities.Relations." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "> Result ");
                output.AppendLine("            {");
                output.AppendLine("                get { return _entities; }");
                output.AppendLine("            }");

                output.AppendLine("            public Entities.Tables." + GetSchemaName(entity.Schema) + "." + GetFormattedEntityName(entity.Name) + " Add(Entities.Tables." + GetSchemaName(entity.Schema) + "." + GetFormattedEntityName(entity.Name) + " item)");
                output.AppendLine("            {");
                output.AppendLine("                DataHandler dh = new DataHandler(item);");
                output.AppendLine("                return (Entities.Tables." + GetSchemaName(entity.Schema) + "." + GetFormattedEntityName(entity.Name) + ")dh.Add((IDataItem)item);");
                output.AppendLine("            }");

                output.AppendLine("            public Int64 Update(Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " item)");
                output.AppendLine("            {");
                output.AppendLine("                DataHandler dh = new DataHandler(item);");
                output.AppendLine("                return base.Update((IDataItem)item);");
                output.AppendLine("            }");


                output.AppendLine("            public Int64 Delete(Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " item)");
                output.AppendLine("            {");
                output.AppendLine("                DataHandler dh = new DataHandler(item);");
                output.AppendLine("                return dh.DeleteItem((IDataItem)item);");
                output.AppendLine("            }");

                output.AppendLine("            /// Updates an instance of Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " with parameters");
                output.AppendLine("            /// </summary>");
                definedColumnList = string.Empty;
                columnList = string.Empty;
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    output.AppendLine("            /// <param name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + "></param>");
                    definedColumnList += column.LanguageType + " " + GetFormattedEntityName(column.Name) + ",";
                    columnList += "                 item." + GetFormattedEntityName(column.Name) + " = " + GetFormattedEntityName(column.Name) + ";" + Environment.NewLine;

                }

                output.AppendLine("            /// <returns>Int64</returns>");
                if (definedColumnList.Length > 0)
                    definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);

                output.AppendLine("            public Int64 Update(" + definedColumnList + ")");
                output.AppendLine("            {");
                output.AppendLine("                 Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + " item = new Entities.Tables." + GetSchemaName(GetSchemaName(entity.Schema)) + "." + GetFormattedEntityName(entity.Name) + "();");
                output.AppendLine(columnList);
                output.AppendLine("                return base.Update((IDataItem)item);");
                output.AppendLine("            }");


                output.AppendLine("            public class CustomWhereParameter : WhereParameter {");

                output.AppendLine("                 internal WhereParameter whereParameter = new WhereParameter();");
                output.AppendLine("                 public void Add(ColumnEnum betweenColumn, " + _namespace + ".sqlEnum.OperandEnum operand, object valueFrom, object valueTo)");
                output.AppendLine("                 {");
                output.AppendLine("                     this.whereParameter.Add(Enum.GetName(typeof(ColumnEnum), betweenColumn), valueFrom, valueTo);");
                output.AppendLine("                 }");
                output.AppendLine("                 public void  Add(ColumnEnum column, " + _namespace + ".sqlEnum.OperandEnum operand,object value)");
                output.AppendLine("                 {");
                output.AppendLine("                     this.whereParameter.Add(Enum.GetName(typeof(ColumnEnum), column), operand, value);");
                output.AppendLine("                 }");
                output.AppendLine("                 public void Add(" + _namespace + ".sqlEnum.ConjunctionEnum conjunction,ColumnEnum betweenColumn, " + _namespace + ".sqlEnum.OperandEnum operand, object valueFrom, object valueTo)");
                output.AppendLine("                 {");
                output.AppendLine("                     this.whereParameter.Add(conjunction, Enum.GetName(typeof(ColumnEnum), betweenColumn), valueFrom, valueTo);");
                output.AppendLine("                 }");
                output.AppendLine("                 public void Add(" + _namespace + ".sqlEnum.ConjunctionEnum conjunction,ColumnEnum column, " + _namespace + ".sqlEnum.OperandEnum operand, object value)");
                output.AppendLine("                 {");
                output.AppendLine("                     this.whereParameter.Add(conjunction, Enum.GetName(typeof(ColumnEnum), column), operand, value);");
                output.AppendLine("                 }");
                output.AppendLine("                 public void Clear()");
                output.AppendLine("                 {");
                output.AppendLine("                     this.whereParameter.Clear();");
                output.AppendLine("                 }");
                output.AppendLine("                 public long Count");
                output.AppendLine("                 {");
                output.AppendLine("                     get {");
                output.AppendLine("                         return this.whereParameter.Count;");
                output.AppendLine("                     }");
                output.AppendLine("                 }");
                output.AppendLine("            }");
                output.AppendLine("            public class CustomOrderByParameter : OrderByParameter {");
                output.AppendLine("                 internal OrderByParameter orderByParameter = new OrderByParameter();");
                output.AppendLine("                 public void Add(ColumnEnum column, " + _namespace + ".sqlEnum.DirEnum direction = " + _namespace + ".sqlEnum.DirEnum.ASC)");
                output.AppendLine("                 {");
                output.AppendLine("                     this.orderByParameter.Add(Enum.GetName(typeof(ColumnEnum), column), direction);");
                output.AppendLine("                 }");
                output.AppendLine("                 public void Clear()");
                output.AppendLine("                 {");
                output.AppendLine("                     this.orderByParameter.Clear();");
                output.AppendLine("                 }");
                output.AppendLine("                 public long Count");
                output.AppendLine("                 {");
                output.AppendLine("                     get {");
                output.AppendLine("                         return this.orderByParameter.Count;");
                output.AppendLine("                     }");
                output.AppendLine("                 }");
                output.AppendLine("            }");
                output.AppendLine("            public class CustomGroupByParameter : GroupByParameter {");
                output.AppendLine("                 internal GroupByParameter groupByParameter = new GroupByParameter();");
                output.AppendLine("                 public void Add(ColumnEnum column)");
                output.AppendLine("                 {");
                output.AppendLine("                     this.groupByParameter.Add(Enum.GetName(typeof(ColumnEnum), column));");
                output.AppendLine("                 }");
                output.AppendLine("                 public void Clear()");
                output.AppendLine("                 {");
                output.AppendLine("                     this.groupByParameter.Clear();");
                output.AppendLine("                 }");
                output.AppendLine("                 public long Count");
                output.AppendLine("                 {");
                output.AppendLine("                     get {");
                output.AppendLine("                         return this.groupByParameter.Count;");
                output.AppendLine("                     }");
                output.AppendLine("                 }");
                output.AppendLine("            }");
                output.AppendLine("             public void Dispose()");
                output.AppendLine("             {");
                output.AppendLine("                 _entities = null;");
                output.AppendLine("                 _cacheItemList = null;");
                output.AppendLine("                 Where = null;");
                output.AppendLine("                 OrderBy = null;");
                output.AppendLine("                 GroupBy = null;");
                output.AppendLine("                 Aggregate = null;");
                output.AppendLine("				");
                output.AppendLine("                 base.Dispose(true);");
                output.AppendLine("             }");
                output.AppendLine("        } // class " + GetFormattedEntityName(entity.Name));
                output.AppendLine("	} //namespace " + _namespace + ".Business.Relations." + GetSchemaName(entity.Schema));

            }
        }

        SaveOutputToFile("Business.Relations.cs", output, true);
        definedColumnList = string.Empty;
        columnList = string.Empty;
        output = new StringBuilder();
    }

    private void BuildEntitiesRelations(MyMeta.IDatabase db, string _namespace, string childrenPrefix)
    {
        string nullable = string.Empty;
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using System.Linq;");
        output.AppendLine("using System.Text;");


        foreach (MyMeta.ITable entity in db.Tables)
        {
            if (entity.Selected)
            {
                String definedColumnList = string.Empty;
                String columnList = string.Empty;

                output.AppendLine("		namespace " + _namespace + ".Entities.Relations." + GetSchemaName(entity.Schema) + " {");
                if (_generationProject.SerializeEntitiesClases)
                    output.AppendLine("			[Serializable()]                         //");
                output.AppendLine("			[DataItemAttributeSchemaName(" + System.Convert.ToChar(34) + GetSchemaName(entity.Schema) + System.Convert.ToChar(34) + ")]  // Database Schema Name");
                output.AppendLine("			[DataItemAttributeObjectName(" + System.Convert.ToChar(34) + entity.Name + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + entity.Alias + System.Convert.ToChar(34) + ")]    // Object name  and alias in Database");
                output.AppendLine("			[DataItemAttributeObjectType(DataItemAttributeObjectType.ObjectTypeEnum.Table)] // Table, View,StoredProcedure,Function");
                output.AppendLine("			public class " + GetFormattedEntityName(entity.Name) + " : IRelationsDataITem");
                output.AppendLine("			{");
                output.AppendLine("				        ");
                output.AppendLine("				public class ColumnNames");
                output.AppendLine("				{");
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    output.AppendLine("					public const string " + GetFormattedEntityName(column.Name) + " = " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ";");
                }
                output.AppendLine("				}");

                output.AppendLine("				public enum FieldEnum : int");
                output.AppendLine("                {");
                int columnCount = 0;
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    columnCount += 1;
                    if (columnCount == entity.Columns.Count)
                    {
                        output.AppendLine("					" + GetFormattedEntityName(column.Name));
                    }
                    else
                    {
                        output.AppendLine("					" + GetFormattedEntityName(column.Name) + ",");
                    }
                }

                output.AppendLine("				}");

                output.AppendLine("	               /// <summary>");
                output.AppendLine("                /// Parameterless Constructor");
                output.AppendLine("	               /// <summary>");
                output.AppendLine("                public " + GetFormattedEntityName(entity.Name) + "()");
                output.AppendLine("                {");
                output.AppendLine("                }");
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    if (!column.IsAutoKey)
                    {
                        if (column.IsInForeignKey && !column.IsInPrimaryKey)
                        {
                            definedColumnList += _namespace + ".Entities.Relations." + GetSchemaName(column.ForeignKeys[0].PrimaryTable.Schema) + "." + column.ForeignKeys[0].PrimaryTable.Name + " " + column.Name + ",";
                            columnList += column.Name + ",";
                        }
                        else
                        {
                            nullable = column.IsNullable == true && column.LanguageType != "String" ? "?" : String.Empty;
                            definedColumnList += column.LanguageType + nullable + " " + GetFormattedEntityName(column.Name) + ",";
                            columnList += GetFormattedEntityName(column.Name) + ",";
                        }
                    }

                }
                if (definedColumnList.Length > 0)
                    definedColumnList = definedColumnList.Substring(0, definedColumnList.Length - 1);
                if (columnList.Length > 0)
                    columnList = columnList.Substring(0, columnList.Length - 1);
                if (definedColumnList.Length > 0) // No parameter constructor is built if all columns are autokey : TODO: Verify
                {
                    output.AppendLine("                public  " + GetFormattedEntityName(entity.Name) + "(" + definedColumnList + ")");
                    output.AppendLine("                {");
                    foreach (MyMeta.IColumn column in entity.Columns)
                    {
                        output.AppendLine("                    this." + GetFormattedEntityName(column.Name) + " = " + GetFormattedEntityName(column.Name) + ";");
                    }
                    output.AppendLine("                }");
                }
                string displayColumn = string.Empty;
                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    // Original name and framework name
                    output.AppendLine("             [DataItemAttributeFieldName(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ")]");
                    if (column.IsInPrimaryKey)
                        output.AppendLine("             [PropertyAttribute(PropertyAttribute.PropertyAttributeEnum.Pk)] //Is Primary Key");
                    if (column.Name.Equals(_generationProject.EntityDisplayName))
                    {
                        displayColumn = column.Name;
                        output.AppendLine("             [PropertyAttribute(PropertyAttribute.PropertyAttributeEnum.Display)] //Is Display Default");
                    }
                    if (column.IsInForeignKey && !column.IsInPrimaryKey)
                    {
                        nullable = column.IsNullable == true && column.LanguageType != "String" ? "?" : String.Empty;

                        output.AppendLine("             [PropertyAttribute(PropertyAttribute.PropertyAttributeEnum.Exclude)] //Exclude");

                        string lazyLoadingValueHolderVisible = _generationProject.LazyLoadingValueHolderVisible ? "internal " : "private "; 

                        output.AppendLine("             " + lazyLoadingValueHolderVisible + column.LanguageType + nullable + " _" + GetFormattedEntityName(column.Name) + " { get; set; }");

                        output.AppendLine("             [PropertyAttribute(PropertyAttribute.PropertyAttributeEnum.Fk)] //Is Foreign Key");
                        output.AppendLine("             [PropertyAttributeForeignKeyObjectName(" + System.Convert.ToChar(34) + column.ForeignKeys[0].PrimaryTable.Name + System.Convert.ToChar(34) + ")]// Object name in Database");
                        output.AppendLine("             public " + _namespace + ".Entities.Relations." + GetSchemaName(column.ForeignKeys[0].PrimaryTable.Schema) + "." + column.ForeignKeys[0].PrimaryTable.Name + nullable + " " + column.Name);
                        output.AppendLine("             {");
                        output.AppendLine("                 get {");
                        //output.AppendLine("                     if (" + GetFormattedEntityName(column.Name) + "_ == null || " + GetFormattedEntityName(column.Name) + "_." + GetFormattedEntityName(GetPrimaryKeyName(column.ForeignKeys[0].PrimaryTable)) + " != _" + GetFormattedEntityName(column.Name) + ")");
                        //output.AppendLine("                         {");
                        output.AppendLine("                         if(this._" + GetFormattedEntityName(column.Name) +" != null && this." + GetFormattedEntityName(column.Name) + "_EntityHolder == null)");
                        //output.AppendLine("                             this." + GetFormattedEntityName(column.Name) + "_EntityHolder = this." + GetFormattedEntityName(column.Name) + "_BusinessHolder.Items((" + column.LanguageType + ")this._" + GetFormattedEntityName(column.Name) + ").FirstOrDefault();");
                        output.AppendLine("                             this." + GetFormattedEntityName(column.Name) + "_EntityHolder = new " + _namespace + ".Business.Relations." + GetSchemaName(column.ForeignKeys[0].PrimaryTable.Schema) + "." + GetFormattedEntityName(column.ForeignKeys[0].PrimaryTable.Name) + "().Items((" + column.LanguageType + ")this._" + GetFormattedEntityName(column.Name) + ").FirstOrDefault();");
                        //output.AppendLine("                         }");
                        output.AppendLine("                         return this." + GetFormattedEntityName(column.Name) + "_EntityHolder;");
                        output.AppendLine("                     }");
                        output.AppendLine("                 set {" + GetFormattedEntityName(column.Name) + "_EntityHolder  =  value;}");
                        output.AppendLine("             }");

                        output.AppendLine("             private " + _namespace + ".Entities.Relations." + GetSchemaName(column.ForeignKeys[0].PrimaryTable.Schema) + "." + GetFormattedEntityName(column.ForeignKeys[0].PrimaryTable.Name) + " " + GetFormattedEntityName(column.Name) + "_EntityHolder = null;");
                        //output.AppendLine("             private " + _namespace + ".Business.Relations." + GetSchemaName(column.ForeignKeys[0].PrimaryTable.Schema) + "." + GetFormattedEntityName(column.ForeignKeys[0].PrimaryTable.Name) + " " + GetFormattedEntityName(column.Name) + "_BusinessHolder = new();");
                    }
                    else
                    {
                        nullable = column.IsNullable == true && column.LanguageType != "String" ? "?" : String.Empty;
                        output.AppendLine("             public " + column.LanguageType + nullable + " " + GetFormattedEntityName(column.Name) + " { get; set; }");
                    }
                }
                //////////////////////////    Begin Children   ///////////////////////////////////////////////

                string childBuffer = string.Empty;

                foreach (MyMeta.IColumn column in entity.Columns)
                {
                    if (column.IsInPrimaryKey)  // MODIF- 2-03-2016 
                    {
                        foreach (MyMeta.IForeignKey refFk in column.ForeignKeys)
                        {
                            MyMeta.ITable refFkTable = ((MyMeta.IForeignKey)refFk).ForeignTable;
                            if (!refFkTable.Name.Equals(GetFormattedEntityName(entity.Name))) // Exclude same table
                            {

                                if (((MyMeta.IForeignKey)refFk).ForeignColumns != null)
                                {
                                    int indexofChild = childBuffer.IndexOf("                 ///  Represents the child collection of " + GetFormattedEntityName(refFkTable.Name) + " that have this " + GetFormattedEntityName(((MyMeta.IForeignKey)refFk).ForeignColumns[0].Name) + " value.");
                                    if (indexofChild == -1)
                                    {
                                        childBuffer += "                 ///  Represents the child collection of " + GetFormattedEntityName(refFkTable.Name) + " that have this " + GetFormattedEntityName(((MyMeta.IForeignKey)refFk).ForeignColumns[0].Name) + " value.";
                                        output.AppendLine("                 /// <summary>");
                                        output.AppendLine("                 ///  Represents the child collection of " + GetFormattedEntityName(refFkTable.Name) + " that have this " + GetFormattedEntityName(((MyMeta.IForeignKey)refFk).ForeignColumns[0].Name) + " value.");
                                        output.AppendLine("                 /// </summary>");
                                        output.AppendLine("                 [PropertyAttribute(PropertyAttribute.PropertyAttributeEnum.Exclude)] //Exclude");
                                        output.AppendLine("                 public List<" + _namespace + ".Entities.Relations." + GetSchemaName(refFkTable.Schema) + "." + GetFormattedEntityName(refFkTable.Name) + "> " + childrenPrefix + GetFormattedEntityName(refFkTable.Name) + "_" + GetFormattedEntityName(((MyMeta.IForeignKey)refFk).ForeignColumns[0].Name));
                                        output.AppendLine("                {");
                                        output.AppendLine("                     get {");
                                        output.AppendLine("                             " + _namespace + ".Business.Relations." + GetSchemaName(refFkTable.Schema) + "." + GetFormattedEntityName(refFkTable.Name) + " entities = new " + _namespace + ".Business.Relations." + GetSchemaName(refFkTable.Schema) + "." + GetFormattedEntityName(refFkTable.Name) + "();");
                                        output.AppendLine("                             entities.Where.Add(" + _namespace + ".Business.Relations." + GetSchemaName(refFkTable.Schema) + "." + GetFormattedEntityName(refFkTable.Name) + ".ColumnEnum." + GetFormattedEntityName(((MyMeta.IForeignKey)refFk).ForeignColumns[0].Name) + ", " + _namespace + ".sqlEnum.OperandEnum.Equal, " + GetPrimaryKeyName(entity) + ");");
                                        output.AppendLine("                             return entities.Items();");
                                        output.AppendLine("                         }");
                                        output.AppendLine("                }");
                                    }
                                }
                            }
                        }
                    }
                }

                //////////////////////////////// Overrides section ////////////////////////
                if (!displayColumn.Equals(string.Empty))
                {
                    output.AppendLine("             public override int GetHashCode() => (" + displayColumn + " == null ? string.Empty : " + displayColumn + ").GetHashCode();");
                    output.AppendLine("             public override string ToString() => " + displayColumn + ";");
                }
                //////////////////////////    End Children   ///////////////////////////////////////////////
                output.AppendLine("				");
                output.AppendLine("			} //Class " + GetFormattedEntityName(entity.Name) + " ");
                output.AppendLine("		 } //namespace " + _namespace + ".Entities.Relations." + GetSchemaName(entity.Schema));
            }

        }
        SaveOutputToFile("Entities.Relations.cs", output, true);
        output = new StringBuilder();
    }

    private void BuildRelationsDataHandler(string _namespace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System;");
        output.AppendLine("using System.Data;");
        output.AppendLine("using System.Collections;");
        output.AppendLine("using System.Reflection;");
        output.AppendLine("using System.Collections.Generic;");
        output.AppendLine("using " + _namespace + ".Entities;");
        output.AppendLine("public class RelationsDataHandler : DataHandler");
        output.AppendLine("{");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// No argument constructor");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public RelationsDataHandler()");
        output.AppendLine("        : base()");
        output.AppendLine("    { }");
        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Constructor with IDataItem");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    public RelationsDataHandler(IDataItem dataItem)");
        output.AppendLine("        : base(dataItem)");
        output.AppendLine("    {");
        output.AppendLine("        _dataItem = dataItem;");
        output.AppendLine("    }");

        output.AppendLine("    /// <summary>");
        output.AppendLine("    /// Constructor to handle Transaction by reflection");
        output.AppendLine("    /// </summary>");
        output.AppendLine("    /// <remarks></remarks>");
        output.AppendLine("    public RelationsDataHandler(IDataHandler dataHandler)");
        output.AppendLine("        : base()");
        output.AppendLine("    {");
        output.AppendLine("        try");
        output.AppendLine("        {");
        output.AppendLine("            _transaction = dataHandler.GetTransaction();");
        output.AppendLine("        }");
        output.AppendLine("        catch (Exception)");
        output.AppendLine("        {");
        output.AppendLine("            throw (new Exception(" + System.Convert.ToChar(34) + "DataHandler (New) : Transaction assignment Error." + System.Convert.ToChar(34) + "));");
        output.AppendLine("        }");
        output.AppendLine("    }");

        output.AppendLine("    ///////////////////////////////////// Reflection Mapping //////////////////////////////////////////////////////////////");
        output.AppendLine("    protected override List<IDataItem> MapDataReaderToDataItem<IDataItem>(IDataReader dr, IDataItem dataItem) //where IDataItem : new()");
        output.AppendLine("    {");
        output.AppendLine("        Type dataItemType = dataItem.GetType();");

        output.AppendLine("        PropertyInfo[] properties = dataItemType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public); // TODO: Pasar a caché.");

        output.AppendLine("        Hashtable hashtable = new Hashtable();");

        output.AppendLine("        //List<IDataItem> entities = new List<IDataItem>();");
        output.AppendLine("        List<IDataItem> entities = new List<IDataItem>();");

        output.AppendLine("        foreach (PropertyInfo info in properties)");
        output.AppendLine("        {");
        output.AppendLine("            hashtable[info.Name] = info;");
        output.AppendLine("        }");
        output.AppendLine("        while (dr.Read())");
        output.AppendLine("        {");
        output.AppendLine("            IDataItem newObject = (IDataItem)Activator.CreateInstance(dataItemType, false);");
        output.AppendLine("            for (int index = 0; index < dr.FieldCount; index++)");
        output.AppendLine("            {");
        output.AppendLine("                PropertyInfo info = (PropertyInfo)hashtable[_dataFieldDefinitions[index].Name];");
        output.AppendLine("                if ((info != null) && info.CanWrite)");
        output.AppendLine("                {");
        output.AppendLine("                    if (!dr.GetValue(index).Equals(System.DBNull.Value))");
        output.AppendLine("                    {");
        output.AppendLine("                        if (info.PropertyType.Namespace.Contains(" + System.Convert.ToChar(34) + "Entities.Relations" + System.Convert.ToChar(34) + "))");
        output.AppendLine("                        {");
        output.AppendLine("                            info = (PropertyInfo)hashtable[ " + System.Convert.ToChar(34) + "_" + System.Convert.ToChar(34) + " + _dataFieldDefinitions[index].Name];");
        output.AppendLine("                        }");
        output.AppendLine("                            info.SetValue(newObject, dr.GetValue(index), null);");
        output.AppendLine("                    }");
        output.AppendLine("                }");
        output.AppendLine("            }");
        output.AppendLine("            entities.Add(newObject);");
        output.AppendLine("        }");
        output.AppendLine("        dr.Close();");
        output.AppendLine("        return entities;");
        output.AppendLine("    }");
        output.AppendLine("}");

        SaveOutputToFile("RelationsDataHandler.cs", output, true);
        output = new StringBuilder();

    }
    ////////////////                END RELATIONS                        ///////////////////////////////////////// 




    #endregion
    #region Encription      
    private void BuildEncriptionFile(string nameSpace)
    {
        StringBuilder output = new StringBuilder();

        output.AppendLine("using System.Security.Cryptography;");
        output.AppendLine("using System.Text;");
        output.AppendLine("");
        output.AppendLine("namespace " + nameSpace + "");
        output.AppendLine("{");
        output.AppendLine("    internal static partial class Crypto");
        output.AppendLine("    {");
        output.AppendLine("        public static string Encrypt(string textToEncrypt,string key)");
        output.AppendLine("        {");
        output.AppendLine("            if (string.IsNullOrEmpty(key))");
        output.AppendLine("                throw new ArgumentException(" + System.Convert.ToChar(34) + "Key must have valid value." + System.Convert.ToChar(34) + ", nameof(key));");
        output.AppendLine("            if (string.IsNullOrEmpty(textToEncrypt))");
        output.AppendLine("                throw new ArgumentException(" + System.Convert.ToChar(34) + "The text must have valid value." + System.Convert.ToChar(34) + ", nameof(textToEncrypt));");
        output.AppendLine("");
        output.AppendLine("            var buffer = Encoding.UTF8.GetBytes(textToEncrypt);");
        output.AppendLine("            var hash = SHA512.Create();");
        output.AppendLine("            var aesKey = new byte[24];");
        output.AppendLine("            Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);");
        output.AppendLine("");
        output.AppendLine("            using (var aes = Aes.Create())");
        output.AppendLine("            {");
        output.AppendLine("                if (aes == null)");
        output.AppendLine("                    throw new ArgumentException(" + System.Convert.ToChar(34) + "Parameter must not be null." + System.Convert.ToChar(34) + ", nameof(aes));");
        output.AppendLine("");
        output.AppendLine("                aes.Key = aesKey;");
        output.AppendLine("");
        output.AppendLine("                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))");
        output.AppendLine("                using (var resultStream = new MemoryStream())");
        output.AppendLine("                {");
        output.AppendLine("                    using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))");
        output.AppendLine("                    using (var plainStream = new MemoryStream(buffer))");
        output.AppendLine("                    {");
        output.AppendLine("                        plainStream.CopyTo(aesStream);");
        output.AppendLine("                    }");
        output.AppendLine("");
        output.AppendLine("                    var result = resultStream.ToArray();");
        output.AppendLine("                    var combined = new byte[aes.IV.Length + result.Length];");
        output.AppendLine("                    Array.ConstrainedCopy(aes.IV, 0, combined, 0, aes.IV.Length);");
        output.AppendLine("                    Array.ConstrainedCopy(result, 0, combined, aes.IV.Length, result.Length);");
        output.AppendLine("");
        output.AppendLine("                    return Convert.ToBase64String(combined);");
        output.AppendLine("                }");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("");
        output.AppendLine("");
        output.AppendLine("        public static string Decrypt(this string textToDecrypt,string key)");
        output.AppendLine("        {");
        output.AppendLine("            if (string.IsNullOrEmpty(key))");
        output.AppendLine("                throw new ArgumentException(" + System.Convert.ToChar(34) + "Key must have valid value." + System.Convert.ToChar(34) + ", nameof(key));");
        output.AppendLine("            if (string.IsNullOrEmpty(textToDecrypt))");
        output.AppendLine("                throw new ArgumentException(" + System.Convert.ToChar(34) + "The encrypted text must have valid value." + System.Convert.ToChar(34) + ", nameof(textToDecrypt));");
        output.AppendLine("");
        output.AppendLine("            var combined = Convert.FromBase64String(textToDecrypt);");
        output.AppendLine("            var buffer = new byte[combined.Length];");
        output.AppendLine("            var hash = SHA512.Create();");
        output.AppendLine("            var aesKey = new byte[24];");
        output.AppendLine("            Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);");
        output.AppendLine("");
        output.AppendLine("            using (var aes = Aes.Create())");
        output.AppendLine("            {");
        output.AppendLine("                if (aes == null)");
        output.AppendLine("                    throw new ArgumentException(" + System.Convert.ToChar(34) + "Parameter must not be null." + System.Convert.ToChar(34) + ", nameof(aes));");
        output.AppendLine("");
        output.AppendLine("                aes.Key = aesKey;");
        output.AppendLine("");
        output.AppendLine("                var iv = new byte[aes.IV.Length];");
        output.AppendLine("                var ciphertext = new byte[buffer.Length - iv.Length];");
        output.AppendLine("");
        output.AppendLine("                Array.ConstrainedCopy(combined, 0, iv, 0, iv.Length);");
        output.AppendLine("                Array.ConstrainedCopy(combined, iv.Length, ciphertext, 0, ciphertext.Length);");
        output.AppendLine("");
        output.AppendLine("                aes.IV = iv;");
        output.AppendLine("");
        output.AppendLine("                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))");
        output.AppendLine("                using (var resultStream = new MemoryStream())");
        output.AppendLine("                {");
        output.AppendLine("                    using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))");
        output.AppendLine("                    using (var plainStream = new MemoryStream(ciphertext))");
        output.AppendLine("                    {");
        output.AppendLine("                        plainStream.CopyTo(aesStream);");
        output.AppendLine("                    }");
        output.AppendLine("");
        output.AppendLine("                    return Encoding.UTF8.GetString(resultStream.ToArray());");
        output.AppendLine("                }");
        output.AppendLine("            }");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine("}");
        output.AppendLine("");

        SaveOutputToFile("Crypto.cs", output, true);

        output = new StringBuilder();

    }
    #endregion

    #region Solution Files
    private void BuildProject(string _namespace)
    {
        System.Text.StringBuilder output = new StringBuilder();
        output.AppendLine("<?xml version=" + System.Convert.ToChar(34) + "1.0" + System.Convert.ToChar(34) + " encoding=" + System.Convert.ToChar(34) + "utf-8" + System.Convert.ToChar(34) + "?>");
        output.AppendLine("<Project ToolsVersion=" + System.Convert.ToChar(34) + "4.0" + System.Convert.ToChar(34) + " DefaultTargets=" + System.Convert.ToChar(34) + "Build" + System.Convert.ToChar(34) + " xmlns=" + System.Convert.ToChar(34) + "http://schemas.microsoft.com/developer/msbuild/2003" + System.Convert.ToChar(34) + ">");
        output.AppendLine("  <Import Project=" + System.Convert.ToChar(34) + "$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props" + System.Convert.ToChar(34) + " Condition=" + System.Convert.ToChar(34) + "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')" + System.Convert.ToChar(34) + " />");
        output.AppendLine("  <PropertyGroup>");
        output.AppendLine("    <Configuration Condition=" + System.Convert.ToChar(34) + " '$(Configuration)' == '' " + System.Convert.ToChar(34) + ">Debug</Configuration>");
        output.AppendLine("    <Platform Condition=" + System.Convert.ToChar(34) + " '$(Platform)' == '' " + System.Convert.ToChar(34) + ">AnyCPU</Platform>");
        output.AppendLine("    <ProjectGuid>{F83A5690-0B0C-4734-88C6-109415784538}</ProjectGuid>");
        output.AppendLine("    <OutputType>Library</OutputType>");
        output.AppendLine("    <AppDesignerFolder>Properties</AppDesignerFolder>");
        output.AppendLine("    <RootNamespace>" + _namespace + "</RootNamespace>");
        output.AppendLine("    <AssemblyName>" + _namespace + "</AssemblyName>");
        output.AppendLine("    <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>");
        output.AppendLine("    <FileAlignment>512</FileAlignment>");
        output.AppendLine("  </PropertyGroup>");
        output.AppendLine("  <PropertyGroup Condition=" + System.Convert.ToChar(34) + " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' " + System.Convert.ToChar(34) + ">");
        output.AppendLine("    <DebugSymbols>true</DebugSymbols>");
        output.AppendLine("    <DebugType>full</DebugType>");
        output.AppendLine("    <Optimize>false</Optimize>");
        output.AppendLine("    <OutputPath>bin\\Debug\\</OutputPath>");
        output.AppendLine("    <DefineConstants>DEBUG;TRACE</DefineConstants>");
        output.AppendLine("    <ErrorReport>prompt</ErrorReport>");
        output.AppendLine("    <WarningLevel>4</WarningLevel>");
        output.AppendLine("  </PropertyGroup>");
        output.AppendLine("  <PropertyGroup Condition=" + System.Convert.ToChar(34) + " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' " + System.Convert.ToChar(34) + ">");
        output.AppendLine("    <DebugType>pdbonly</DebugType>");
        output.AppendLine("    <Optimize>true</Optimize>");
        output.AppendLine("    <OutputPath>bin\\Release\\</OutputPath>");
        output.AppendLine("    <DefineConstants>TRACE</DefineConstants>");
        output.AppendLine("    <ErrorReport>prompt</ErrorReport>");
        output.AppendLine("    <WarningLevel>4</WarningLevel>");
        output.AppendLine("  </PropertyGroup>");
        output.AppendLine("  <ItemGroup>");
        output.AppendLine("    <Reference Include=" + System.Convert.ToChar(34) + "System" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Reference Include=" + System.Convert.ToChar(34) + "System.Configuration" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Reference Include=" + System.Convert.ToChar(34) + "System.Core" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Reference Include=" + System.Convert.ToChar(34) + "System.Xml.Linq" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Reference Include=" + System.Convert.ToChar(34) + "System.Data.DataSetExtensions" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Reference Include=" + System.Convert.ToChar(34) + "Microsoft.CSharp" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Reference Include=" + System.Convert.ToChar(34) + "System.Data" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Reference Include=" + System.Convert.ToChar(34) + "System.Xml" + System.Convert.ToChar(34) + " />");
        output.AppendLine("  </ItemGroup>");
        output.AppendLine("  <ItemGroup>");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "Attributes.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "Business.Programmability.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "Business.Tables.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "Business.Views.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "DataField.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "DataHandler.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "DataHandlerBase.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "Entities.Programmability.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "Entities.Tables.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "Entities.Views.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "ParameterBase.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "ProcedureDataHandler.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "FunctionDataHandler.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "RelationsDataHandler.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "Properties\\AssemblyInfo.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "sqlEnum.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("    <Compile Include=" + System.Convert.ToChar(34) + "Constants.cs" + System.Convert.ToChar(34) + " />");
        output.AppendLine("  </ItemGroup>");
        output.AppendLine("  <Import Project=" + System.Convert.ToChar(34) + "$(MSBuildToolsPath)\\Microsoft.CSharp.targets" + System.Convert.ToChar(34) + " />");
        output.AppendLine("  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. ");
        output.AppendLine("       Other similar extension points exist, see Microsoft.Common.targets.");
        output.AppendLine("  <Target Name=" + System.Convert.ToChar(34) + "BeforeBuild" + System.Convert.ToChar(34) + ">");
        output.AppendLine("  </Target>");
        output.AppendLine("  <Target Name=" + System.Convert.ToChar(34) + "AfterBuild" + System.Convert.ToChar(34) + ">");
        output.AppendLine("  </Target>");
        output.AppendLine("  -->");
        output.AppendLine("</Project>");
        SaveOutputToFile(_databaseName + ".csproj", output, false);
        output = new StringBuilder();

    }
    private void BuildSolution()
    {
        System.Text.StringBuilder output = new StringBuilder();

        output.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
        output.AppendLine("# Visual Studio 2012");
        output.AppendLine("Project(" + System.Convert.ToChar(34) + "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}" + System.Convert.ToChar(34) + ") = " + System.Convert.ToChar(34) + "" + _databaseName + "" + System.Convert.ToChar(34) + ", " + System.Convert.ToChar(34) + "" + _databaseName + ".csproj" + System.Convert.ToChar(34) + ", " + System.Convert.ToChar(34) + "{F83A5690-0B0C-4734-88C6-109415784538}" + System.Convert.ToChar(34) + "");
        output.AppendLine("EndProject");
        output.AppendLine("Global");
        output.AppendLine("	GlobalSection(SolutionConfigurationPlatforms) = preSolution");
        output.AppendLine("		Debug|Any CPU = Debug|Any CPU");
        output.AppendLine("		Release|Any CPU = Release|Any CPU");
        output.AppendLine("	EndGlobalSection");
        output.AppendLine("	GlobalSection(ProjectConfigurationPlatforms) = postSolution");
        output.AppendLine("		{F83A5690-0B0C-4734-88C6-109415784538}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
        output.AppendLine("		{F83A5690-0B0C-4734-88C6-109415784538}.Debug|Any CPU.Build.0 = Debug|Any CPU");
        output.AppendLine("		{F83A5690-0B0C-4734-88C6-109415784538}.Release|Any CPU.ActiveCfg = Release|Any CPU");
        output.AppendLine("		{F83A5690-0B0C-4734-88C6-109415784538}.Release|Any CPU.Build.0 = Release|Any CPU");
        output.AppendLine("	EndGlobalSection");
        output.AppendLine("	GlobalSection(SolutionProperties) = preSolution");
        output.AppendLine("		HideSolutionNode = FALSE");
        output.AppendLine("	EndGlobalSection");
        output.AppendLine("EndGlobal");

        SaveOutputToFile(_databaseName + ".sln", output, false);
        output = new StringBuilder();
    }
    private void BuildAssemblyInfo()
    {
        System.Text.StringBuilder output = new StringBuilder();
        GetHeaderInfo(output);
        output.AppendLine("using System.Reflection;");
        output.AppendLine("using System.Runtime.CompilerServices;");
        output.AppendLine("using System.Runtime.InteropServices;");

        output.AppendLine("// General Information about an assembly is controlled through the following ");
        output.AppendLine("// set of attributes. Change these attribute values to modify the information");
        output.AppendLine("// associated with an assembly.");
        output.AppendLine("[assembly: AssemblyTitle(" + System.Convert.ToChar(34) + _databaseName + System.Convert.ToChar(34) + ")]");
        output.AppendLine("[assembly: AssemblyDescription(" + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + ")]");
        output.AppendLine("[assembly: AssemblyConfiguration(" + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + ")]");
        output.AppendLine("[assembly: AssemblyCompany(" + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + ")]");
        output.AppendLine("[assembly: AssemblyProduct(" + System.Convert.ToChar(34) + _databaseName + System.Convert.ToChar(34) + ")]");
        output.AppendLine("[assembly: AssemblyCopyright(" + System.Convert.ToChar(34) + "Copyright © " + DateTime.Now.Year.ToString() + System.Convert.ToChar(34) + ")]");
        output.AppendLine("[assembly: AssemblyTrademark(" + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + ")]");
        output.AppendLine("[assembly: AssemblyCulture(" + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + ")]");

        output.AppendLine("// Setting ComVisible to false makes the types in this assembly not visible ");
        output.AppendLine("// to COM components.  If you need to access a type in this assembly from ");
        output.AppendLine("// COM, set the ComVisible attribute to true on that type.");
        output.AppendLine("[assembly: ComVisible(false)]");

        output.AppendLine("// The following GUID is for the ID of the typelib if this project is exposed to COM");
        output.AppendLine("[assembly: Guid(" + System.Convert.ToChar(34) + "4c7df185-d7c5-4900-a8ff-95c66d5fe997" + System.Convert.ToChar(34) + ")]");

        output.AppendLine("// Version information for an assembly consists of the following four values:");
        output.AppendLine("//");
        output.AppendLine("//      Major Version");
        output.AppendLine("//      Minor Version ");
        output.AppendLine("//      Build Number");
        output.AppendLine("//      Revision");
        output.AppendLine("//");
        output.AppendLine("// You can specify all the values or you can default the Build and Revision Numbers ");
        output.AppendLine("// by using the '*' as shown below:");
        output.AppendLine("// [assembly: AssemblyVersion(" + System.Convert.ToChar(34) + "1.0.*" + System.Convert.ToChar(34) + ")]");
        output.AppendLine("[assembly: AssemblyVersion(" + System.Convert.ToChar(34) + "1.0.0.0" + System.Convert.ToChar(34) + ")]");
        output.AppendLine("[assembly: AssemblyFileVersion(" + System.Convert.ToChar(34) + "1.0.0.0" + System.Convert.ToChar(34) + ")]");

        SaveOutputToFile("Properties\\AssemblyInfo.cs", output, false);
        output = new StringBuilder();

    }

    private bool BuildWebGridWebForms(MyMeta.IDatabase db)
    {
        try
        {
            System.Text.StringBuilder output = new StringBuilder();

            foreach (MyMeta.ITable entity in db.Tables)
            {

                if (entity.Selected)
                {
                    string entityFormattedName = GetEntityName(entity);
                    int columnCounter = 10;

                    output.AppendLine("<%@ Page Title=" + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " Language=" + System.Convert.ToChar(34) + "C#" + System.Convert.ToChar(34) + " MasterPageFile=" + System.Convert.ToChar(34) + "~/Main.Master" + System.Convert.ToChar(34) + " AutoEventWireup=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " CodeBehind=" + System.Convert.ToChar(34) + entityFormattedName + ".aspx.cs" + System.Convert.ToChar(34) + " Inherits=" + System.Convert.ToChar(34) + db.Name + "." + entityFormattedName + System.Convert.ToChar(34) + " %>");
                    output.AppendLine("<%@ Register TagPrefix=" + System.Convert.ToChar(34) + "WG" + System.Convert.ToChar(34) + " Namespace=" + System.Convert.ToChar(34) + "WebGrid" + System.Convert.ToChar(34) + " Assembly=" + System.Convert.ToChar(34) + "WebGrid" + System.Convert.ToChar(34) + " %>");
                    output.AppendLine("<asp:Content ID=" + System.Convert.ToChar(34) + "MainContent" + System.Convert.ToChar(34) + " ContentPlaceHolderID=" + System.Convert.ToChar(34) + "MainContentPlaceHolder" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("                <WG:Grid ID=" + System.Convert.ToChar(34) + "wg" + entityFormattedName + System.Convert.ToChar(34) + " DataSourceId=" + System.Convert.ToChar(34) + entity.Name + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Width=" + System.Convert.ToChar(34) + "100%" + System.Convert.ToChar(34) + " SystemMessageDataFile=" + System.Convert.ToChar(34) + "~/GridMessages.xml" + System.Convert.ToChar(34) + " CollapsedGrid=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("                    <Columns>");
                    foreach (MyMeta.IColumn column in entity.Columns)
                    {

                        //Chek if is Pk
                        if (column.IsInPrimaryKey)
                        {
                            output.AppendLine("                        <WG:Number ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " EditAlign=" + System.Convert.ToChar(34) + "Right" + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable != true ? "True" : "False") + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " UniqueValueRequired=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "None" + System.Convert.ToChar(34) + " WidthColumnHeaderTitle=" + System.Convert.ToChar(34) + "24px" + System.Convert.ToChar(34) + " />");
                        }
                        else if (column.IsInForeignKey)
                        {
                            output.AppendLine("                        <WG:Foreignkey ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DataSourceId=" + System.Convert.ToChar(34) + column.ForeignKeys[0].PrimaryTable.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " EditAlign=" + System.Convert.ToChar(34) + "Right" + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable != true ? "True" : "False") + System.Convert.ToChar(34) + " SortExpression=" + System.Convert.ToChar(34) + "Name" + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " ValueColumn=" + System.Convert.ToChar(34) + "Name" + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " />");
                        }
                        else
                        {
                            switch (column.DataTypeName.ToLower())
                            {
                                case "tinyint":
                                case "int":
                                case "smallint":
                                case "bigint":
                                    output.AppendLine("                        <WG:Number ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " EditAlign=" + System.Convert.ToChar(34) + "Right" + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable != true ? "True" : "False") + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " WidthColumnHeaderTitle=" + System.Convert.ToChar(34) + "132px" + System.Convert.ToChar(34) + " />");
                                    break;
                                case "decimal":
                                case "float":
                                    output.AppendLine("                        <WG:Decimal ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " EditAlign=" + System.Convert.ToChar(34) + "Right" + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable != true ? "True" : "False") + System.Convert.ToChar(34) + " Searchable=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " WidthColumnHeaderTitle=" + System.Convert.ToChar(34) + "144px" + System.Convert.ToChar(34) + " />");
                                    break;
                                case "datetime":
                                case "smalldatetime":
                                    output.AppendLine("                        <WG:DateTime AddCalendarClientAttributes=" + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " CalendarFormat=" + System.Convert.ToChar(34) + "dd/mm/yy" + System.Convert.ToChar(34) + " ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " EditAlign=" + System.Convert.ToChar(34) + "Right" + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable != true ? "True" : "False") + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " />");
                                    break;
                                case "bit":
                                    output.AppendLine("                       <WG:Checkbox ColumnId=" + System.Convert.ToChar(34) + column.Name + +System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable != true ? "True" : "False") + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " />");
                                    break;
                                case "text":
                                    output.AppendLine("                        <WG:Text ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " MaxSize=" + System.Convert.ToChar(34) + "500" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable != true ? "True" : "False") + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " />");
                                    break;

                                default:

                                    if (column.DataTypeName.ToLower().StartsWith("char") || column.DataTypeName.ToLower().StartsWith("varchar"))
                                    {
                                        output.AppendLine("                        <WG:Text ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " MaxSize=" + System.Convert.ToChar(34) + "500" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable != true ? "True" : "False") + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " />");
                                    }
                                    else
                                    {
                                        output.AppendLine("                         <%--DataType not implemented: " + column.DataTypeName.ToLower() + " --%>");
                                    }
                                    break;
                            }
                        }
                        columnCounter += 10;
                    }

                    output.AppendLine("                    </Columns>");
                    output.AppendLine("                </WG:Grid>");
                    output.AppendLine("</asp:Content>");

                    columnCounter += 10;

                    SaveOutputToFile("WebGridWebForms\\" + entityFormattedName + ".aspx", output, true);

                    output = new StringBuilder();

                }
            }
            return true;
        }

        catch (Exception)
        {

            return false;
        }

    }
    private string GetEntityName(MyMeta.ITable entity)
    {
        string result = string.Empty;
        foreach (string segment in entity.Name.Split('_'))
        {
            result += segment.Substring(0, 1).ToUpper() + segment.Substring(1).ToLower();
        }
        return result;
    }
    public string GetFormattedEntityName(String entityName)
    {
        //return entityName.Replace(" ", "_").Replace(".", "_"); 
        string table = entityName;
        string chars = _generationProject.SupportedCharacters;
        for (int i = 0; i < chars.Length; i++)
        {
            table = table.Replace(chars.Substring(i, 1), "_");
        }

        return table;
    }

    #endregion
    string ITemplate.Name
    {
        get { return TemplateName; }
    }
    public event OnExceptionEventHandler OnException;

    public event OnInfoEventHandler OnInfo;

    string ITemplate.WorkingDir
    {
        get { return _workingdir; }
        set { _workingdir = value; }
    }
    string ITemplate.Description
    {
        get { return TemplateDescription; }
    }
    string ITemplate.OutputLanguage
    {
        get { return TemplateOutputLanguage; }
    }
    public event OnFileGeneratedEventHandler OnFileGenerated;

    public event OnPercentDoneEventHandler OnPercentDone;

    string ITemplate.DbTargetMappingFileName
    {
        get { return _dbTargetMappingFileName; }
        set { _dbTargetMappingFileName = value; }
    }
    string ITemplate.LanguageMappingFileName
    {
        get { return _languageMappingFileName; }
        set { _languageMappingFileName = value; }
    }
    string ITemplate.GUID
    {
        get { return "2014e055-c7dd-4e8b-be94-e71b20110430"; }
    }
}

public class WebFormsWebGridCrudTemplate_5G_CSHARP : ITemplate
{
    string _workingdir;
    const string TemplateName = "WebForms WebGrid Crud Template (CSHARP)";
    const string TemplateDescription = "WebForms WebGrid Crud Template for CSHARP";
    const string TemplateOutputLanguage = "CSHARP";
    private string _languageMappingFileName;
    private string _dbTargetMappingFileName;
    private string _databaseName = string.Empty;
    private const string GENERATED_FOLDER_NAME = ".WebGridWebForms\\";

    bool ITemplate.Execute(MyMeta.IDatabase db, string workingDir, GenerationProject generationProject)
    {
        MyMeta.IDatabase _db = (MyMeta.IDatabase)db;
        _workingdir = workingDir;
        return this.Execute(ref _db, workingDir, generationProject);
    }

    public bool Execute(ref MyMeta.IDatabase db, string workingDir, GenerationProject generationProject)
    {

        try
        {
            db.Root.LanguageMappingFileName = _languageMappingFileName;
            db.Root.DbTargetMappingFileName = _dbTargetMappingFileName;
            db.Root.DbTarget = "SqlClient";
            db.Root.Language = "C# Types";
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            String definedColumnList = String.Empty;
            String columnList = String.Empty;
            _databaseName = db.Name;

            BuildWebGridWebForms(db);
            return true;
        }
        catch (Exception ex)
        {
            if (OnException != null)
            {
                OnException(ex);
            }
            return false;
        }

    }
    #region Build Methods
    private bool BuildWebGridWebForms(MyMeta.IDatabase db)
    {
        try
        {
            System.Text.StringBuilder output = new StringBuilder();

            foreach (MyMeta.ITable entity in db.Tables)
            {

                if (entity.Selected)
                {
                    string entityFormattedName = GetEntityName(entity);
                    int columnCounter = 10;

                    output.AppendLine("<%@ Page Title=" + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " Language=" + System.Convert.ToChar(34) + "C#" + System.Convert.ToChar(34) + " MasterPageFile=" + System.Convert.ToChar(34) + "~/Main.Master" + System.Convert.ToChar(34) + " AutoEventWireup=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " CodeBehind=" + System.Convert.ToChar(34) + entityFormattedName + ".aspx.cs" + System.Convert.ToChar(34) + " Inherits=" + System.Convert.ToChar(34) + db.Name + "." + entityFormattedName + System.Convert.ToChar(34) + " %>");
                    output.AppendLine("<%@ Register TagPrefix=" + System.Convert.ToChar(34) + "WG" + System.Convert.ToChar(34) + " Namespace=" + System.Convert.ToChar(34) + "WebGrid" + System.Convert.ToChar(34) + " Assembly=" + System.Convert.ToChar(34) + "WebGrid" + System.Convert.ToChar(34) + " %>");
                    output.AppendLine("<asp:Content ID=" + System.Convert.ToChar(34) + "MainContent" + System.Convert.ToChar(34) + " ContentPlaceHolderID=" + System.Convert.ToChar(34) + "MainContentPlaceHolder" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("                <WG:Grid ID=" + System.Convert.ToChar(34) + "wg" + entityFormattedName + System.Convert.ToChar(34) + " DataSourceId=" + System.Convert.ToChar(34) + entity.Name + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Width=" + System.Convert.ToChar(34) + "100%" + System.Convert.ToChar(34) + " SystemMessageDataFile=" + System.Convert.ToChar(34) + "~/GridMessages.xml" + System.Convert.ToChar(34) + " CollapsedGrid=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("                    <Columns>");
                    foreach (MyMeta.IColumn column in entity.Columns)
                    {

                        //Chek if is autokey and Pk
                        if (column.IsInPrimaryKey && column.IsAutoKey)
                        {
                            output.AppendLine("                        <WG:Number ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " EditAlign=" + System.Convert.ToChar(34) + "Right" + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable == true ? "True" : "False") + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " UniqueValueRequired=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "None" + System.Convert.ToChar(34) + " WidthColumnHeaderTitle=" + System.Convert.ToChar(34) + "24px" + System.Convert.ToChar(34) + " />");
                        }
                        else if (column.IsInForeignKey)
                        {
                            output.AppendLine("                        <WG:Foreignkey ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DataSourceId=" + System.Convert.ToChar(34) + column.ForeignKeys[0].PrimaryTable.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " EditAlign=" + System.Convert.ToChar(34) + "Right" + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable == true ? "True" : "False") + System.Convert.ToChar(34) + " SortExpression=" + System.Convert.ToChar(34) + "Name" + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " ValueColumn=" + System.Convert.ToChar(34) + "Name" + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " />");
                        }
                        else
                        {
                            switch (column.DataTypeName.ToLower())
                            {
                                case "tinyint":
                                case "int":
                                case "smallint":
                                case "bigint":
                                    output.AppendLine("                        <WG:Number ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " EditAlign=" + System.Convert.ToChar(34) + "Right" + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable == true ? "True" : "False") + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " WidthColumnHeaderTitle=" + System.Convert.ToChar(34) + "132px" + System.Convert.ToChar(34) + " />");
                                    break;
                                case "decimal":
                                case "float":
                                    output.AppendLine("                        <WG:Decimal ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " EditAlign=" + System.Convert.ToChar(34) + "Right" + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable == true ? "True" : "False") + System.Convert.ToChar(34) + " Searchable=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " WidthColumnHeaderTitle=" + System.Convert.ToChar(34) + "144px" + System.Convert.ToChar(34) + " />");
                                    break;
                                case "datetime":
                                case "smalldatetime":
                                    output.AppendLine("                        <WG:DateTime AddCalendarClientAttributes=" + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " CalendarFormat=" + System.Convert.ToChar(34) + "dd/mm/yy" + System.Convert.ToChar(34) + " ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " EditAlign=" + System.Convert.ToChar(34) + "Right" + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable == true ? "True" : "False") + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " />");
                                    break;
                                case "bit":
                                    output.AppendLine("                       <WG:Checkbox ColumnId=" + System.Convert.ToChar(34) + column.Name + +System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable == true ? "True" : "False") + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " />");
                                    break;
                                case "text":
                                    output.AppendLine("                        <WG:Text ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " MaxSize=" + System.Convert.ToChar(34) + "500" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable == true ? "True" : "False") + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " />");
                                    break;

                                default:

                                    if (column.DataTypeName.ToLower().StartsWith("char") || column.DataTypeName.ToLower().StartsWith("varchar"))
                                        output.AppendLine("                        <WG:Text ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " MaxSize=" + System.Convert.ToChar(34) + "500" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable == true ? "True" : "False") + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " />");

                                    else if (column.IsInForeignKey)
                                    {
                                        output.AppendLine("                        <WG:Foreignkey ColumnId=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " DataSourceId=" + System.Convert.ToChar(34) + column.ForeignKeys[0].PrimaryTable.Name + System.Convert.ToChar(34) + " DisplayIndex=" + System.Convert.ToChar(34) + columnCounter.ToString() + System.Convert.ToChar(34) + " EditAlign=" + System.Convert.ToChar(34) + "Right" + System.Convert.ToChar(34) + " GridAlign=" + System.Convert.ToChar(34) + "Left" + System.Convert.ToChar(34) + " IsInDataSource=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " LoadColumnDataSourceData=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Primarykey=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " Required=" + System.Convert.ToChar(34) + (column.IsNullable == true ? "True" : "False") + System.Convert.ToChar(34) + " SortExpression=" + System.Convert.ToChar(34) + "Name" + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " ValueColumn=" + System.Convert.ToChar(34) + "Name" + System.Convert.ToChar(34) + " Visibility=" + System.Convert.ToChar(34) + "Both" + System.Convert.ToChar(34) + " />");
                                    }
                                    else
                                    {
                                        output.AppendLine("                         <%--DataType not implemented: " + column.DataTypeName.ToLower() + " --%>");
                                    }
                                    break;
                            }
                        }
                        columnCounter += 10;
                    }

                    output.AppendLine("                    </Columns>");
                    output.AppendLine("                </WG:Grid>");
                    output.AppendLine("</asp:Content>");

                    columnCounter += 10;

                    SaveOutputToFile("WebGridWebForms\\" + entityFormattedName + ".aspx", output, true);

                    output = new StringBuilder();

                }
            }
            return true;
        }

        catch (Exception)
        {

            return false;
        }

    }
    #region Save File Methods
    private void SaveOutputToFile(string fileName, System.Text.StringBuilder output, bool overWrite)
    {
        if (!_workingdir.EndsWith("\\"))
            _workingdir += "\\";
        string filePath = _workingdir + fileName;
        string fileDirectory = System.IO.Path.GetDirectoryName(filePath);
        if (!System.IO.Directory.Exists(fileDirectory))
            System.IO.Directory.CreateDirectory(fileDirectory);

        if (!System.IO.File.Exists(filePath) || overWrite == true)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath);
            sw.Write(output.ToString());
            sw.Flush();
            sw.Close();
            if (OnFileGenerated != null)
            {
                OnFileGenerated(_workingdir + GENERATED_FOLDER_NAME + fileName);
            }

        }
    }
    #endregion

    private string GetEntityName(MyMeta.ITable entity)
    {
        string result = string.Empty;
        foreach (string segment in entity.Name.Split('_'))
        {
            result += segment.Substring(0, 1).ToUpper() + segment.Substring(1).ToLower();
        }


        return result;
    }
    #endregion
    string ITemplate.Name
    {
        get { return TemplateName; }
    }
    public event OnExceptionEventHandler OnException;

    public event OnInfoEventHandler OnInfo;
    string ITemplate.WorkingDir
    {
        get { return _workingdir; }
        set { _workingdir = value; }
    }
    string ITemplate.Description
    {
        get { return TemplateDescription; }
    }
    string ITemplate.OutputLanguage
    {
        get { return TemplateOutputLanguage; }
    }
    public event OnFileGeneratedEventHandler OnFileGenerated;

    public event OnPercentDoneEventHandler OnPercentDone;
    string ITemplate.DbTargetMappingFileName
    {
        get { return _dbTargetMappingFileName; }
        set { _dbTargetMappingFileName = value; }
    }
    string ITemplate.LanguageMappingFileName
    {
        get { return _languageMappingFileName; }
        set { _languageMappingFileName = value; }
    }
    string ITemplate.GUID
    {
        get { return "2014e550-c7ee-4e8b-be73-e71b20110199"; }
    }

}

