using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

public class MudBlazorControlsBuilder : IPlugin
{
    private List<ITemplate> _templates = new List<ITemplate>();
    const string PluginName = "MudBlazor Crud Builder";
    const string PluginDescription = "MudBlazor Crud Builder";
    public MudBlazorControlsBuilder()
    {
        _templates.Add(new MudBlazorCrudBuilder());
    }
    public string Description
    {
        get { return PluginName; }
    }

    public string Name
    {
        get { return PluginDescription; }
    }

    public System.Collections.Generic.List<ITemplate> Templates
    {
        get { return _templates; }
        set { _templates = value; }
    }

    public class MudBlazorCrudBuilder : ITemplate
    {
        const string TemplateName = "MudBlazor CrudBuilder";
        const string TemplateDescription = "MudBlazor Crud Builder";
        const string TemplateoutputLanguage = "C#";
        private ArrayList _arraylist = new ArrayList();
        private string _workingdir = String.Empty;
        private string _languageMappingFileName;
        private string _dbTargetMappingFileName;
        private string _generationFolder = string.Empty;
        private const string generationFolder = "MudBlazor Generation folder";

        private ArrayList _refkeyList = new ArrayList();
        public MudBlazorCrudBuilder()
        { }
        public MudBlazorCrudBuilder(GenerationProject generationProject)
        {

            TemplateConfigurationEntry generationFolderEntry = generationProject.TemplateConfigurationEntries.Find(e => e.Template.Equals(TemplateName) && e.Name.Equals(generationFolder));

            if (generationFolderEntry == null)
                generationProject.TemplateConfigurationEntries.Add(new TemplateConfigurationEntry(TemplateName, generationFolder, _generationFolder));
            else
                _generationFolder = generationFolderEntry.Value;
        }
        private string GetDbTarget(MyMeta.dbDriver driver)
        {
            string returnvalue = string.Empty;

            returnvalue = System.Configuration.ConfigurationSettings.AppSettings[driver.ToString() + "DbTarget"];
            return returnvalue;
        }
        private string GetLanguage(MyMeta.dbDriver driver)
        {
            string returnvalue = string.Empty;

            returnvalue = System.Configuration.ConfigurationSettings.AppSettings[driver.ToString() + "Language"];
            return returnvalue;
        }
        public bool Execute(MyMeta.IDatabase db, string workingDir, GenerationProject generationProject)
        {
            try
            {
                _workingdir = workingDir;

                MyMeta.IDatabase _dataBase = db;

                System.Text.StringBuilder output = null;

                db.Root.DbTarget = GetDbTarget(db.Root.Driver);
                db.Root.Language = GetLanguage(db.Root.Driver);//"C# Types";


                TemplateConfigurationEntry generationFolderEntry = generationProject.TemplateConfigurationEntries.Find(e => e.Template.Equals(TemplateName) && e.Name.Equals(generationFolder));

                 string displayColumn = generationProject.EntityDisplayName;
                string relativepath = generationProject.Location;

                if (generationProject.DeletePreviousGeneratedFiles)
                {
                    DeletePreviousGeneratedFiles(relativepath);
                }

                foreach (MyMeta.ITable table in db.Tables)
                {
                    output = new System.Text.StringBuilder();
                    if (table.Selected)
                    {
                        // Display column evaluation if entity_Display_Name is not set
                        if (displayColumn.Equals(String.Empty))
                            displayColumn = FindFirstVarcharColumnName(table);


                        output.AppendLine("@page " + System.Convert.ToChar(34) + "/crud" + table.Schema + table.Name + System.Convert.ToChar(34) + "");
                        output.AppendLine("@inject ISnackbar Snackbar");
                        output.AppendLine("");
                        output.AppendLine("@if (" + table.Name + "_entities == null)");
                        output.AppendLine("{");
                        output.AppendLine("    <MudProgressLinear Color=" + System.Convert.ToChar(34) + "Color.Primary" + System.Convert.ToChar(34) + " Indeterminate=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " Class=" + System.Convert.ToChar(34) + "my-7" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("}");
                        output.AppendLine("else");
                        output.AppendLine("{");
                        output.AppendLine("    if (crudMode == CrudMode.List)");
                        output.AppendLine("    {");
                        output.AppendLine("        <MudButton Variant=" + System.Convert.ToChar(34) + "Variant.Filled" + System.Convert.ToChar(34) + " Color=" + System.Convert.ToChar(34) + "Color.Default" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "Add" + System.Convert.ToChar(34) + " >Nuevo... </MudButton>");
                        output.AppendLine("        <MudTable Items=" + System.Convert.ToChar(34) + "" + table.Name + "_entities" + System.Convert.ToChar(34) + " Hover=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " SortLabel=" + System.Convert.ToChar(34) + "Sort By" + System.Convert.ToChar(34) + " Elevation=" + System.Convert.ToChar(34) + "0" + System.Convert.ToChar(34) + "  Filter=" + System.Convert.ToChar(34) + "new Func<" + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " ,bool>(FilterFunc1)" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("            <ToolBarContent>");
                        output.AppendLine("            <MudText Typo=" + System.Convert.ToChar(34) + "Typo.h6" + System.Convert.ToChar(34) + ">Customers</MudText>");
                        output.AppendLine("            <MudSpacer />");
                        output.AppendLine("            <MudTextField @bind-Value=" + System.Convert.ToChar(34) + "searchString" + System.Convert.ToChar(34) + " Placeholder=" + System.Convert.ToChar(34) + "buscar..." + System.Convert.ToChar(34) + " Adornment=" + System.Convert.ToChar(34) + "Adornment.Start" + System.Convert.ToChar(34) + " AdornmentIcon=" + System.Convert.ToChar(34) + "@Icons.Material.Filled.Search" + System.Convert.ToChar(34) + " IconSize=" + System.Convert.ToChar(34) + "Size.Medium" + System.Convert.ToChar(34) + " Class=" + System.Convert.ToChar(34) + "mt-0" + System.Convert.ToChar(34) + "></MudTextField>");
                        output.AppendLine("        </ToolBarContent> ");
                        output.AppendLine("        <HeaderContent>");
                        output.AppendLine("            <MudTh></MudTh>");
                        foreach (var column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                                output.AppendLine("            <MudTh><MudTableSortLabel InitialDirection=" + System.Convert.ToChar(34) + "SortDirection.Ascending" + System.Convert.ToChar(34) + " SortBy=" + System.Convert.ToChar(34) + "new Func<" + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + ", object>(x=>x." + column.Name + ")" + System.Convert.ToChar(34) + ">" + column.Name + "</MudTableSortLabel></MudTh>");
                            else

                                output.AppendLine("            <MudTh><MudTableSortLabel SortBy=" + System.Convert.ToChar(34) + "new Func<" + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + ", object>(x=>x." + column.Name + ")" + System.Convert.ToChar(34) + ">" + column.Name + "</MudTableSortLabel></MudTh>");

                        }
                        output.AppendLine("        </HeaderContent>");
                        output.AppendLine("        <RowTemplate>");
                        output.AppendLine("            <MudTd DataLabel=" + System.Convert.ToChar(34) + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                <MudFab @onclick=" + System.Convert.ToChar(34) + "@(()=>Edit(@context.Id,false))" + System.Convert.ToChar(34) + " Color=" + System.Convert.ToChar(34) + "Color.Primary" + System.Convert.ToChar(34) + " Icon=" + System.Convert.ToChar(34) + "@Icons.Material.Filled.Edit" + System.Convert.ToChar(34) + " Size=" + System.Convert.ToChar(34) + "Size.Small" + System.Convert.ToChar(34) + " IconSize=" + System.Convert.ToChar(34) + "Size.Small" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                <MudFab @onclick=" + System.Convert.ToChar(34) + "@(()=>Edit(@context.Id,true))" + System.Convert.ToChar(34) + " Color=" + System.Convert.ToChar(34) + "Color.Secondary" + System.Convert.ToChar(34) + " Icon=" + System.Convert.ToChar(34) + "@Icons.Material.Filled.Delete" + System.Convert.ToChar(34) + " Size=" + System.Convert.ToChar(34) + "Size.Small" + System.Convert.ToChar(34) + " IconSize=" + System.Convert.ToChar(34) + "Size.Small" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("            </MudTd>");
                        foreach (var column in table.Columns)
                        {
                            if (column.IsInForeignKey && !column.IsInPrimaryKey)
                            {
                                var foreignKey = column.ForeignKeys.FirstOrDefault();
                                MyMeta.ITable fkTable = foreignKey.PrimaryTable;

                                output.AppendLine("            <MudTd DataLabel=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ">@(" + fkTable.Name + "_entities.FirstOrDefault(c => c." + foreignKey.PrimaryColumns.FirstOrDefault().Name + " == @context." + foreignKey.ForeignColumns.FirstOrDefault().Name + ")." + displayColumn + ")</MudTd>");
                            }
                            else
                                output.AppendLine("            <MudTd DataLabel=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ">@context." + column.Name + "</MudTd>");
                        }
                        output.AppendLine("            <MudTd DataLabel=" + System.Convert.ToChar(34) + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                <MudFab @onclick=" + System.Convert.ToChar(34) + "@(()=>Edit(@context.Id,false))" + System.Convert.ToChar(34) + " Color=" + System.Convert.ToChar(34) + "Color.Primary" + System.Convert.ToChar(34) + " Icon=" + System.Convert.ToChar(34) + "@Icons.Material.Filled.Edit" + System.Convert.ToChar(34) + " Size=" + System.Convert.ToChar(34) + "Size.Small" + System.Convert.ToChar(34) + " IconSize=" + System.Convert.ToChar(34) + "Size.Small" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                <MudFab @onclick=" + System.Convert.ToChar(34) + "@(()=>Edit(@context.Id,true))" + System.Convert.ToChar(34) + " Color=" + System.Convert.ToChar(34) + "Color.Secondary" + System.Convert.ToChar(34) + " Icon=" + System.Convert.ToChar(34) + "@Icons.Material.Filled.Delete" + System.Convert.ToChar(34) + " Size=" + System.Convert.ToChar(34) + "Size.Small" + System.Convert.ToChar(34) + " IconSize=" + System.Convert.ToChar(34) + "Size.Small" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("            </MudTd>");
                        output.AppendLine("        </RowTemplate>");
                        output.AppendLine("        <PagerContent>");
                        output.AppendLine("            <MudTablePager PageSizeOptions=" + System.Convert.ToChar(34) + "new int[]{10,20,50,100}" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("        </PagerContent>");
                        output.AppendLine("    </MudTable>");
                        output.AppendLine("    }");
                        output.AppendLine("");
                        output.AppendLine("    if(crudMode==CrudMode.Add || crudMode==CrudMode.Edit || crudMode==CrudMode.Delete)");
                        output.AppendLine("    {");
                        output.AppendLine("        <MudCard Elevation=" + System.Convert.ToChar(34) + "25" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("        <MudCardHeader>");
                        output.AppendLine("            <CardHeaderContent>");
                        output.AppendLine("                <MudText Typo=" + System.Convert.ToChar(34) + "Typo.inherit" + System.Convert.ToChar(34) + ">" + System.Convert.ToChar(34) + "@(GetDialogTitle())" + System.Convert.ToChar(34) + " </MudText>");
                        output.AppendLine("            </CardHeaderContent>");
                        output.AppendLine("        </MudCardHeader>");
                        output.AppendLine("        <MudCardContent>");

                        foreach (var column in table.Columns)
                        {
                            if (column.IsInForeignKey && !column.IsInPrimaryKey)
                            {
                                var foreignKey = column.ForeignKeys.FirstOrDefault();
                                MyMeta.ITable fkTable = foreignKey.PrimaryTable;

                                output.AppendLine("            <MudSelect HelperText=" + System.Convert.ToChar(34) 
                                    + "Seleccione un elemento" + System.Convert.ToChar(34) + " T=" 
                                    + System.Convert.ToChar(34) + generationProject.Namespace 
                                    + ".Entities.Tables." + fkTable.Schema + "." + fkTable.Name 
                                    + System.Convert.ToChar(34) + " @bind-Value=" + System.Convert.ToChar(34) 
                                    + "@" + fkTable.Name + "_entity" + System.Convert.ToChar(34) 
                                    + " @bind-Text=" + System.Convert.ToChar(34) + "@" + fkTable.Name + "_entity." 
                                    + displayColumn + System.Convert.ToChar(34) + " SelectedValuesChanged=" + System.Convert.ToChar(34) 
                                    + "@(()=>Select" + fkTable.Name + "ValueChanged(" + fkTable.Name + "_entity))" + System.Convert.ToChar(34)
                                    + " Disabled=" + System.Convert.ToChar(34) + "@(crudMode == CrudMode.Edit || crudMode == CrudMode.Add ? false : true)" 
                                    + System.Convert.ToChar(34) + " >");
                                output.AppendLine("                    @foreach(var item in " + fkTable.Name + "_entities)");
                                output.AppendLine("                    {");
                                output.AppendLine("                        <MudSelectItem Value=" + System.Convert.ToChar(34) + "@item" + System.Convert.ToChar(34) + ">@item." + displayColumn + "</MudSelectItem>");
                                output.AppendLine("                    }");
                                output.AppendLine("            </MudSelect>      ");

                            }
                            else
                            {
                                switch (column.DataTypeName.ToLower())
                                {
                                    case "tinyint":
                                    case "int":
                                    case "smallint":
                                    case "bigint":
                                        if (column.IsInPrimaryKey)
                                        {
                                            output.AppendLine("            <MudTextField @bind-Value=" + System.Convert.ToChar(34) + table.Name + "_entity." + column.Name + System.Convert.ToChar(34)
                                            + " Label=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34)
                                            + " Variant=" + System.Convert.ToChar(34) + "Variant.Text" + System.Convert.ToChar(34)
                                            + " Margin=" + System.Convert.ToChar(34) + "Margin.Normal" + System.Convert.ToChar(34)
                                            + " Disabled=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34)
                                            + " ></MudTextField>");
                                        }
                                        else
                                        {
                                            output.AppendLine("            <MudTextField @bind-Value=" + System.Convert.ToChar(34) + table.Name + "_entity." + column.Name + System.Convert.ToChar(34)
                                            + " Label=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34)
                                            + " Variant=" + System.Convert.ToChar(34) + "Variant.Text" + System.Convert.ToChar(34)
                                            + " Margin=" + System.Convert.ToChar(34) + "Margin.Normal" + System.Convert.ToChar(34)
                                            + " Disabled=" + System.Convert.ToChar(34) + "@(crudMode == CrudMode.Edit || crudMode == CrudMode.Add ? false : true)" + System.Convert.ToChar(34)
                                            + " ></MudTextField>");
                                        }
                                        break;
                                    case "decimal":
                                    case "float":
                                        break;
                                    case "datetime":
                                    case "smalldatetime":
                                        output.AppendLine("            <MudDatePicker Label=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " @bind-Date=" + System.Convert.ToChar(34) + "" + table.Name + "_entity." + column.Name + System.Convert.ToChar(34) + " Disabled=" + System.Convert.ToChar(34) + "@(crudMode == CrudMode.Edit || crudMode == CrudMode.Add ? false : true)" + System.Convert.ToChar(34) + "/>");

                                        break;
                                    case "bit":
                                    case "text":
                                    default:
                                        output.AppendLine("            <MudTextField @bind-Value=" + System.Convert.ToChar(34) + "" + table.Name + "_entity." + column.Name + System.Convert.ToChar(34) + " Label=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Variant=" + System.Convert.ToChar(34) + "Variant.Text" + System.Convert.ToChar(34) + " Margin=" + System.Convert.ToChar(34) + "Margin.Normal" + System.Convert.ToChar(34) + " Disabled=" + System.Convert.ToChar(34) + "@(crudMode == CrudMode.Edit || crudMode == CrudMode.Add ? false : true)" + System.Convert.ToChar(34) + "></MudTextField>");
                                        break;
                                }
                            }
                        }

                        output.AppendLine("            ");
                        output.AppendLine("            <br />");
                        output.AppendLine("            <MudButton Variant=" + System.Convert.ToChar(34) + "Variant.Filled" + System.Convert.ToChar(34) + " Color=" + System.Convert.ToChar(34) + "Color.Success" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "Save" + System.Convert.ToChar(34) + ">Aceptar</MudButton>");
                        output.AppendLine("            <MudButton Variant=" + System.Convert.ToChar(34) + "Variant.Filled" + System.Convert.ToChar(34) + " Color=" + System.Convert.ToChar(34) + "Color.Error" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "Cancel" + System.Convert.ToChar(34) + ">Cancelar</MudButton>");
                        output.AppendLine("        </MudCardContent>");
                        output.AppendLine("        </MudCard>");
                        output.AppendLine("    }");
                        output.AppendLine("}");
                        output.AppendLine("");
                        output.AppendLine("@code {");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Crud Mode Enum");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private enum CrudMode");
                        output.AppendLine("    {");
                        output.AppendLine("        List,");
                        output.AppendLine("        Add,");
                        output.AppendLine("        Edit,");
                        output.AppendLine("        Delete");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Variables to handle main entity");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " " + table.Name + "_entity = new();");
                        output.AppendLine("    private List<" + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "> " + table.Name + "_entities = null;");

                        foreach (var column in table.Columns)
                        {
                            if (!column.IsInPrimaryKey)
                            {
                                if (column.IsInForeignKey)
                                {
                                    var foreignKey = column.ForeignKeys.FirstOrDefault();
                                    MyMeta.ITable fkTable = foreignKey.PrimaryTable;

                                    if (!fkTable.Equals(table))
                                    {
                                        output.AppendLine("    /// <summary>");
                                        output.AppendLine("    /// Variables to handle Relation between " + foreignKey.PrimaryColumns.FirstOrDefault().Name + " and " + fkTable.Name);
                                        output.AppendLine("    /// </summary>");
                                        output.AppendLine("    private " + generationProject.Namespace + ".Entities.Tables." + fkTable.Schema + "." + fkTable.Name + " " + fkTable.Name + "_entity = new();");
                                        output.AppendLine("    private List<" + generationProject.Namespace + ".Entities.Tables." + fkTable.Schema + "." + fkTable.Name + "> " + fkTable.Name + "_entities = null;");
                                        output.AppendLine("");
                                    }
                                }
                            }
                        }



                        output.AppendLine("");
                        output.AppendLine("    private string searchString =string.Empty;");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    ///  Page crud mode {List,Add,Edit, Delete,}");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private CrudMode crudMode = CrudMode.List;");
                        output.AppendLine("");
                        output.AppendLine("    protected override async Task OnInitializedAsync()");
                        output.AppendLine("    {");
                        output.AppendLine("        await Task.Run(LoadMainEntityData);");
                        output.AppendLine("        await Task.Run(LoadTypesData);");
                        output.AppendLine("    }");
                        output.AppendLine("    #region Loading entities data");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Loads main entity data");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void LoadMainEntityData()");
                        output.AppendLine("    {");
                        output.AppendLine("        " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " entity = new();");
                        output.AppendLine("        " + table.Name + "_entities = entity.Items();");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Loads TypeTable data");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void LoadTypesData()");
                        output.AppendLine("    {");


                        foreach (var column in table.Columns)
                        {
                            if (!column.IsInPrimaryKey)
                            {
                                if (column.IsInForeignKey)
                                {
                                    var foreignKey = column.ForeignKeys.FirstOrDefault();
                                    MyMeta.ITable fkTable = foreignKey.PrimaryTable;

                                    output.AppendLine("        " + generationProject.Namespace + ".Business.Tables." + fkTable.Schema + "." + fkTable.Name + " " + fkTable.Name + "_local = new();");
                                    output.AppendLine("        " + fkTable.Name + "_entities = " + fkTable.Name + "_local.Items();");
                                    output.AppendLine("");
                                }
                            }
                        }
                        output.AppendLine("    }");
                        output.AppendLine("");
                        output.AppendLine("    #endregion");
                        output.AppendLine("");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Initializations for adding a new entity");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void Add()");
                        output.AppendLine("    {");
                        output.AppendLine("        //Main entity initialization");
                        output.AppendLine("        " + table.Name + "_entity = new();");
                        output.AppendLine("        //Relations entities initialization");


                        foreach (var column in table.Columns)
                        {
                            if (!column.IsInPrimaryKey)
                            {
                                if (column.IsInForeignKey)
                                {
                                    var foreignKey = column.ForeignKeys.FirstOrDefault();
                                    MyMeta.ITable fkTable = foreignKey.PrimaryTable;

                                    output.AppendLine("        " + fkTable.Name + "_entity = new(); ");
                                }
                            }
                        }

                        output.AppendLine("        crudMode = CrudMode.Add;");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Initializations for editing an entity");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void Edit(long id, bool delete)");
                        output.AppendLine("    {");
                        output.AppendLine("        " + table.Name + "_entity = " + table.Name + "_entities.FirstOrDefault(c => c." + ((MyMeta.Column)table.PrimaryKeys.FirstOrDefault()).ItemName + " == id);");
                        output.AppendLine("");
                        foreach (var column in table.Columns)
                        {
                            if (!column.IsInPrimaryKey)
                            {
                                if (column.IsInForeignKey)
                                {
                                    var foreignKey = column.ForeignKeys.FirstOrDefault();
                                    MyMeta.ITable fkTable = foreignKey.PrimaryTable;

                                    output.AppendLine("            " + fkTable.Name + "_entity = new() { ");
                                    output.AppendLine("            Id = " + fkTable.Name + "_entities.FirstOrDefault(c => c.Id == " + table.Name + "_entity." + column.Name + ").Id, ");
                                    output.AppendLine("            " + generationProject.EntityDisplayName + " = " + fkTable.Name + "_entities.FirstOrDefault(c => c.Id == " + table.Name + "_entity." + column.Name + ")." + generationProject.EntityDisplayName);
                                    output.AppendLine("            };");
                                }
                            }
                        }


                        output.AppendLine("        if (delete)");
                        output.AppendLine("            crudMode = CrudMode.Delete;");
                        output.AppendLine("        else");
                        output.AppendLine("            crudMode = CrudMode.Edit;");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Operation depending on Crud mode");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void Save()");
                        output.AppendLine("    {");
                        output.AppendLine("        string actionInformation = string.Empty;");
                        output.AppendLine("        " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " crud = new();");
                        output.AppendLine("        switch (crudMode)");
                        output.AppendLine("        {");
                        output.AppendLine("            case CrudMode.Add:");
                        output.AppendLine("                crud.Add(" + table.Name + "_entity);");
                        output.AppendLine("                actionInformation = " + System.Convert.ToChar(34) + "agregado." + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                break;");
                        output.AppendLine("            case CrudMode.Edit:");
                        output.AppendLine("                crud.Update(" + table.Name + "_entity);");
                        output.AppendLine("                actionInformation = " + System.Convert.ToChar(34) + "modificado." + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                break;");
                        output.AppendLine("            case CrudMode.Delete:");
                        output.AppendLine("                crud.Delete(" + table.Name + "_entity);");
                        output.AppendLine("                actionInformation = " + System.Convert.ToChar(34) + "eliminado." + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                break;");
                        output.AppendLine("        }");
                        output.AppendLine("");
                        output.AppendLine("        Snackbar.Clear();");
                        output.AppendLine("        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;");
                        output.AppendLine("        Snackbar.Add(" + System.Convert.ToChar(34) + "El registro ha sido " + System.Convert.ToChar(34) + " + actionInformation, Severity.Success);");
                        output.AppendLine("");
                        output.AppendLine("        // Sets Crud mode to List");
                        output.AppendLine("        crudMode = CrudMode.List;");
                        output.AppendLine("        ");
                        output.AppendLine("        LoadMainEntityData();");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Crud mode cancellation and default mode");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void Cancel()");
                        output.AppendLine("    {");
                        output.AppendLine("        crudMode = CrudMode.List;  ");
                        output.AppendLine("        LoadMainEntityData();");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Sets detail panel title - TODO: Implement Multilanguage");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <returns></returns>");
                        output.AppendLine("    private string GetDialogTitle()");
                        output.AppendLine("    {");
                        output.AppendLine("        string retValue = string.Empty;");
                        output.AppendLine("        switch (crudMode)");
                        output.AppendLine("        {");
                        output.AppendLine("            case CrudMode.Add:");
                        output.AppendLine("                retValue = " + System.Convert.ToChar(34) + "Nuevo..." + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                break;");
                        output.AppendLine("            case CrudMode.Edit:");
                        output.AppendLine("                retValue = " + System.Convert.ToChar(34) + "Editar..." + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                break;");
                        output.AppendLine("            case CrudMode.Delete:");
                        output.AppendLine("                retValue = " + System.Convert.ToChar(34) + "Borrar..." + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                break;");
                        output.AppendLine("        }");
                        output.AppendLine("        return retValue;");
                        output.AppendLine("    }");
                        output.AppendLine("");
                        output.AppendLine("    #region Selection events");

                        foreach (var column in table.Columns)
                        {
                            if (!column.IsInPrimaryKey)
                            {
                                if (column.IsInForeignKey)
                                {
                                    var foreignKey = column.ForeignKeys.FirstOrDefault();
                                    MyMeta.ITable fkTable = foreignKey.PrimaryTable;

                                    output.AppendLine("    /// <summary>");
                                    output.AppendLine("    /// " + fkTable.Name + " value changed");
                                    output.AppendLine("    /// </summary>");
                                    output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "item" + System.Convert.ToChar(34) + "></param>");
                                    output.AppendLine("    private void Select" + fkTable.Name + "ValueChanged(" + generationProject.Namespace + ".Entities.Tables." + fkTable.Schema + "." + fkTable.Name + " item)");
                                    output.AppendLine("    {");
                                    output.AppendLine("        " + table.Name + "_entity." + column.Name + " = " + fkTable.Name + "_entities.FirstOrDefault(c => c." + generationProject.EntityDisplayName + " == item." + generationProject.EntityDisplayName + ").Id;");
                                    output.AppendLine("    }");
                                    output.AppendLine("");
                                }
                            }
                        }

                        output.AppendLine("    #endregion");
                        output.AppendLine("");
                        output.AppendLine("    #region Table filtering implementation");
                        output.AppendLine("    ");
                        output.AppendLine("    private bool FilterFunc1(" + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " element) => FilterFunc(element, searchString);");
                        output.AppendLine("");
                        output.AppendLine("    private bool FilterFunc(" + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " element, string searchString)");
                        output.AppendLine("    {");
                        output.AppendLine("        if (string.IsNullOrWhiteSpace(searchString))");
                        output.AppendLine("            return true;");

                        string numericFieldList = string.Empty;

                        foreach (var column in table.Columns)
                        {
                            if (!column.IsInPrimaryKey)
                            {
                                switch (column.DataTypeName.ToLower())
                                {
                                    case "tinyint":
                                    case "int":
                                    case "smallint":
                                    case "bigint":
                                    case "decimal":
                                    case "float":
                                        numericFieldList += "{ element." + column.Name + "} ";
                                        break;
                                    case "datetime":
                                    case "smalldatetime":
                                        break;
                                    case "bit":
                                        break;
                                    case "text":
                                    case "varchar":
                                    case "nvarchar":
                                        output.AppendLine("        if (!string.IsNullOrWhiteSpace(element." + column.Name + ") && element." + column.Name + ".Contains(searchString, StringComparison.OrdinalIgnoreCase))");
                                        output.AppendLine("            return true;");
                                        break;
                                    default:

                                        break;
                                }
                            }
                        }

                        if (numericFieldList.Length > 0)
                        {
                            output.AppendLine("        if ($" + System.Convert.ToChar(34) + numericFieldList + System.Convert.ToChar(34) + ".Contains(searchString))");
                            output.AppendLine("            return true;");
                        }
                        output.AppendLine("        return false;");
                        output.AppendLine("    }");
                        output.AppendLine("    #endregion");
                        output.AppendLine("}");
                        output.AppendLine("");
                        

                        SaveOutputToFile(table.Schema + "." + table.Name + ".Crud.razor", output, relativepath, true);
                    }
                }
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
        private string FindFirstVarcharColumnName(MyMeta.ITable table)
        {
            string returnvalue = "[No display column to deliver! Set Display name property or defina a text column]";

            foreach (MyMeta.IColumn item in table.Columns)
            {
                if (item.DataTypeName.ToLower().StartsWith("char") 
                    || item.DataTypeName.ToLower().StartsWith("varchar")
                    || item.DataTypeName.ToLower().StartsWith("nchar")
                    || item.DataTypeName.ToLower().StartsWith("nvarchar")
                    )
                {
                    returnvalue = item.Name;
                    break;
                }
            }

            return returnvalue;
        }
        private string FindKeyColumnName(MyMeta.ITable table)
        {
            string returnvalue = string.Empty;

            foreach (MyMeta.IColumn item in table.Columns)
            {
                if (item.IsInPrimaryKey)
                {
                    returnvalue = item.Name;
                    break;
                }
            }

            return returnvalue;
        }

        #region Output files 
        private void SaveOutputToFile(string fileName, System.Text.StringBuilder output, string relativepath, bool overWrite)
        {
            if (!_workingdir.EndsWith("\\"))
                _workingdir += "\\";
            string filePath = _workingdir +  _generationFolder.Trim() + "\\" + relativepath + fileName;
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
                    OnFileGenerated(_workingdir + fileName);
                }

            }
        }
       
        private void DeletePreviousGeneratedFiles(string relativepath)
        {
            if (!_workingdir.EndsWith("\\"))
                _workingdir += "\\";
  
            string fileDirectory = _workingdir + _generationFolder.Trim() + "\\" + relativepath;

            foreach (var file in System.IO.Directory.GetFiles(fileDirectory))
            {
                if(file.ToLower().EndsWith(".razor"))
                    System.IO.File.Delete(file);
            }

        }

        #endregion
        public string Name
        {
            get { return TemplateName; }
        }
        public event OnExceptionEventHandler OnException;
        public string WorkingDir
        {
            get { return _workingdir; }
            set { _workingdir = value; }
        }
        public string Description
        {
            get { return TemplateDescription; }
        }
        public event OnInfoEventHandler OnInfo;
        public string outputLanguage
        {
            get { return TemplateoutputLanguage; }
        }
        public event OnFileGeneratedEventHandler OnFileGenerated;
        public event OnPercentDoneEventHandler OnPercentDone;
        public string DbTargetMappingFileName
        {
            get { return _dbTargetMappingFileName; }
            set { _dbTargetMappingFileName = value; }
        }
        public string LanguageMappingFileName
        {
            get { return _languageMappingFileName; }
            set { _languageMappingFileName = value; }
        }
        public string GUID
        {
            get { return "c6884693-f5e6-505c-bc2d-480cd6d425a1"; }
        }
        #region ITemplate Members

        public string OutputLanguage
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
  