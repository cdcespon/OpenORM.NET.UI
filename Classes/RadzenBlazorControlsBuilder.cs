using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

public class RadzenBlazorControlsBuilder : IPlugin
{
    private List<ITemplate> _templates = new List<ITemplate>();
    const string PluginName = "RadzenBlazor Crud Builder";
    const string PluginDescription = "RadzenBlazor Crud Builder";
    public RadzenBlazorControlsBuilder()
    {
        _templates.Add(new RadzenBlazorCrudBuilder());
        _templates.Add(new RadzenBlazorLeftMenuBuilder());
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

    public class RadzenBlazorCrudBuilder : ITemplate
    {
        const string TemplateName = "RadzenBlazor CrudBuilder";
        const string TemplateDescription = "RadzenBlazor Crud Builder";
        const string TemplateoutputLanguage = "C#";
        private ArrayList _arraylist = new ArrayList();
        private string _workingdir = String.Empty;
        private string _languageMappingFileName;
        private string _dbTargetMappingFileName;
        private string _generationFolder = string.Empty;
        private string _treatFieldAsImage = string.Empty;
        private const string generationFolder = "Radzen Blazor Generation folder";
        private const string treatFieldAsImage = "Radzen Blazor Treat Field as Image";

        private ArrayList _refkeyList = new ArrayList();
        public RadzenBlazorCrudBuilder()
        { }
        public RadzenBlazorCrudBuilder(GenerationProject generationProject)
        {
            // Generation Folder
            TemplateConfigurationEntry generationFolderEntry = generationProject.TemplateConfigurationEntries.Find(e => e.Template.Equals(TemplateName) && e.Name.Equals(generationFolder));

            if (generationFolderEntry == null)
                generationProject.TemplateConfigurationEntries.Add(new TemplateConfigurationEntry(TemplateName, generationFolder, _generationFolder));
            else
                _generationFolder = generationFolderEntry.Value;

            // Treat Field As Image
            TemplateConfigurationEntry treatFieldAsImageEntry = generationProject.TemplateConfigurationEntries.Find(e => e.Template.Equals(TemplateName) && e.Name.Equals(treatFieldAsImage));

            if (generationFolderEntry == null)
                generationProject.TemplateConfigurationEntries.Add(new TemplateConfigurationEntry(TemplateName, treatFieldAsImage, _treatFieldAsImage));
            else
                _treatFieldAsImage = treatFieldAsImageEntry.Value;

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

                string fieldsToTreatAsImage = string.Empty;
                var entry = generationProject.TemplateConfigurationEntries.Find(e => e.Template.Equals(TemplateName) && e.Name.Equals(treatFieldAsImage));
                if (entry != null)
                    fieldsToTreatAsImage = entry.Value;

                foreach (MyMeta.ITable table in db.Tables)
                {
                    output = new System.Text.StringBuilder();
                    if (table.Selected)
                    {
                        string gridCustomProperties = " AllowColumnReorder=@CustomizadorController.ObtenerPropiedadGrilla(" + System.Convert.ToChar(34) + "HabilitarMovilidad" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosTabla) ";
                        gridCustomProperties += "AllowColumnPicking=@CustomizadorController.ObtenerPropiedadGrilla(" + System.Convert.ToChar(34) + "HabilitarColumnasOpcionales" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosTabla) ";
                        gridCustomProperties += "AllowFiltering=@CustomizadorController.ObtenerPropiedadGrilla(" + System.Convert.ToChar(34) + "HabilitarFiltrado" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosTabla) ";
                        gridCustomProperties += "AllowColumnResize=@CustomizadorController.ObtenerPropiedadGrilla(" + System.Convert.ToChar(34) + "HabilitarRedimensionamiento" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosTabla) ";
                        gridCustomProperties += "AllowSorting=@CustomizadorController.ObtenerPropiedadGrilla(" + System.Convert.ToChar(34) + "HabilitarOrdenamiento" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosTabla) ";
                        gridCustomProperties += "AllowPaging=@CustomizadorController.ObtenerPropiedadGrilla(" + System.Convert.ToChar(34) + "HabilitarPaginado" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosTabla) ";
                        gridCustomProperties += "AllowGrouping=@CustomizadorController.ObtenerPropiedadGrilla(" + System.Convert.ToChar(34) + "HabilitarAgrupamiento" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosTabla) ";

                        string columnCustomProperties = " Filterable=@CustomizadorController.ObtenerPropiedadColumnaFiltrable(" + System.Convert.ToChar(34) + "TemplateABM" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                        columnCustomProperties += "Groupable=@CustomizadorController.ObtenerPropiedadColumnaAgrupable(" + System.Convert.ToChar(34) + "TemplateABM" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                        columnCustomProperties += "Resizable=@CustomizadorController.ObtenerPropiedadColumnaRedimensionable(" + System.Convert.ToChar(34) + "TemplateABM" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                        columnCustomProperties += "Reorderable=@CustomizadorController.ObtenerPropiedadColumnaMovible(" + System.Convert.ToChar(34) + "TemplateABM" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                        columnCustomProperties += "MinWidth=@CustomizadorController.ObtenerPropiedadAnchoMinimoColumnaEnGrilla(" + System.Convert.ToChar(34) + "TemplateABM" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                        columnCustomProperties += "Width=@CustomizadorController.ObtenerPropiedadAnchoColumnaEnGrilla(" + System.Convert.ToChar(34) + "TemplateABM" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                        columnCustomProperties += "Pickable=@CustomizadorController.ObtenerPropiedadVisibilidadColumnaEnSelector(" + System.Convert.ToChar(34) + "TemplateABM" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                        columnCustomProperties += "OrderIndex=@CustomizadorController.ObtenerPropiedadPosicionColumnaEnGrilla(" + System.Convert.ToChar(34) + "TemplateABM" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                        columnCustomProperties += "Sortable=@CustomizadorController.ObtenerPropiedadColumnaOrdenable(" + System.Convert.ToChar(34) + "TemplateABM" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                        columnCustomProperties += "Visible=@CustomizadorController.ObtenerPropiedadVisibilidadColumna(" + System.Convert.ToChar(34) + "TemplateABM" + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";

                        output.AppendLine("@page " + System.Convert.ToChar(34) + "/" + table.Schema + table.Name + "Crud" + System.Convert.ToChar(34) + "");
                        output.AppendLine("@using Permaquim." + generationProject.Namespace + ".Web.Administration.Controllers;");
                        output.AppendLine("@inject NotificationService NotificationService");
                        output.AppendLine("@inject Blazored.SessionStorage.ISessionStorageService sessionStorage");
                        output.AppendLine("@inject NavigationManager NavManager");

                        output.AppendLine(" ");
                        output.AppendLine("@if (estaProcesando)");
                        output.AppendLine("{");
                        output.AppendLine("     <div class=" + System.Convert.ToChar(34) + "spinner" + System.Convert.ToChar(34) + "></div>");
                        output.AppendLine("}");
                        output.AppendLine(" ");
                        output.AppendLine("@if(dataFunciones != null)");
                        output.AppendLine("{");
                        output.AppendLine("@if (SeguridadController.VerificarPermisoFuncion(" + System.Convert.ToChar(34) + table.Schema + table.Name + System.Convert.ToChar(34) + ",dataFunciones," + System.Convert.ToChar(34) + "PuedeVisualizar" + System.Convert.ToChar(34) + "))");
                        output.AppendLine("{");
                        output.AppendLine(" @if (" + table.Name + "_entities == null)");
                        output.AppendLine(" {");
                        output.AppendLine("     <div class=" + System.Convert.ToChar(34) + "spinner" + System.Convert.ToChar(34) + " />");
                        output.AppendLine(" }");
                        output.AppendLine(" else");
                        output.AppendLine(" {");
                        output.AppendLine("     switch (crudMode)");
                        output.AppendLine("     {");
                        output.AppendLine("         case CrudMode.Add:");
                        output.AppendLine("             <RadzenBadge Style=@EstiloController.ObtenerItemEstilo(dataEsquemaDetalle, " + System.Convert.ToChar(34) + "ColorFondoTituloBadge" + System.Convert.ToChar(34) + ", false) BadgeStyle =" + System.Convert.ToChar(34) + "BadgeStyle.Secondary" + System.Convert.ToChar(34) + " >");
                        output.AppendLine("                 <ChildContent>");
                        output.AppendLine("                         <h4 style=" + System.Convert.ToChar(34) + "color:white;" + System.Convert.ToChar(34) + ">" + "@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "ABM_A_NAME_" + table.Schema + "." + table.Name + System.Convert.ToChar(34) + ",dataLenguaje)</h4>");
                        output.AppendLine("                 </ChildContent>");
                        output.AppendLine("             </RadzenBadge>");
                        output.AppendLine("             break;");
                        output.AppendLine("         case CrudMode.Delete:");
                        output.AppendLine("             <RadzenBadge Style=@EstiloController.ObtenerItemEstilo(dataEsquemaDetalle, " + System.Convert.ToChar(34) + "ColorFondoTituloBadgeBorrar" + System.Convert.ToChar(34) + ", false)  BadgeStyle=" + System.Convert.ToChar(34) + "BadgeStyle.Warning" + System.Convert.ToChar(34) + " >");
                        output.AppendLine("                 <ChildContent>");
                        output.AppendLine("                         <h4 style=" + System.Convert.ToChar(34) + "color:white;" + System.Convert.ToChar(34) + ">" + "@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "ABM_D_NAME_" + table.Schema + "." + table.Name + System.Convert.ToChar(34) + ",dataLenguaje)</h4>");
                        output.AppendLine("                 </ChildContent>");
                        output.AppendLine("             </RadzenBadge>");
                        output.AppendLine("             break;");
                        output.AppendLine("             case CrudMode.Edit:");
                        output.AppendLine("             <RadzenBadge Style=@EstiloController.ObtenerItemEstilo(dataEsquemaDetalle, " + System.Convert.ToChar(34) + "ColorFondoTituloBadge" + System.Convert.ToChar(34) + ", false)  BadgeStyle=" + System.Convert.ToChar(34) + "BadgeStyle.Secondary" + System.Convert.ToChar(34) + " >");
                        output.AppendLine("                 <ChildContent>");
                        output.AppendLine("                         <h4 style=" + System.Convert.ToChar(34) + "color:white;" + System.Convert.ToChar(34) + ">" + "@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "ABM_M_NAME_" + table.Schema + "." + table.Name + System.Convert.ToChar(34) + ",dataLenguaje)</h4>");
                        output.AppendLine("                 </ChildContent>");
                        output.AppendLine("             </RadzenBadge>");
                        output.AppendLine("             break;         ");
                        output.AppendLine("             case CrudMode.List:");
                        output.AppendLine("             <RadzenBadge Style=@EstiloController.ObtenerItemEstilo(dataEsquemaDetalle, " + System.Convert.ToChar(34) + "ColorFondoTituloBadge" + System.Convert.ToChar(34) + ", false)  BadgeStyle=" + System.Convert.ToChar(34) + "BadgeStyle.Secondary" + System.Convert.ToChar(34) + " >");
                        output.AppendLine("                 <ChildContent>");
                        output.AppendLine("                     <div>");
                        output.AppendLine("                         <h4 style=" + System.Convert.ToChar(34) + "color:white;" + System.Convert.ToChar(34) + ">" + "@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "ABM_L_NAME_" + table.Schema + "." + table.Name + System.Convert.ToChar(34) + ",dataLenguaje)</h4>");
                        output.AppendLine("                     </div>");
                        output.AppendLine("                 </ChildContent>");
                        output.AppendLine("             </RadzenBadge>");
                        output.AppendLine("             break;  ");
                        output.AppendLine("     }");
                        output.AppendLine("     <hr>");
                        output.AppendLine("     if (crudMode == CrudMode.List)");
                        output.AppendLine("     {");
                        output.AppendLine("         <RadzenDataGrid @bind-Settings=" + System.Convert.ToChar(34) + "@GridSettings" + System.Convert.ToChar(34) + " FilterCaseSensitivity= " + System.Convert.ToChar(34) + "FilterCaseSensitivity.CaseInsensitive" + System.Convert.ToChar(34) + (table.Columns.Where(x => x.Name == "Habilitado").Count() > 0 ? " CellRender=@" + table.Name + "CellRender" : "") + " PageSizeOptions=" + System.Convert.ToChar(34) + "@(new int[]{10,20,50,100})" + System.Convert.ToChar(34) + " FilterMode=" + System.Convert.ToChar(34) + "FilterMode.Advanced" + System.Convert.ToChar(34) + " PageSize=" + System.Convert.ToChar(34) + "10" + System.Convert.ToChar(34) + " PagerHorizontalAlign=" + System.Convert.ToChar(34) + "HorizontalAlign.Left" + System.Convert.ToChar(34) + " ShowPagingSummary=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34));
                        output.AppendLine("         " + gridCustomProperties);
                        output.AppendLine("         FilterText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "CABECERA_FILTRO" + System.Convert.ToChar(34) + ", dataLenguaje) GroupPanelText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "INDICADOR_AGRUPACION" + System.Convert.ToChar(34) + ", dataLenguaje) IsEmptyText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_ES_VACIO" + System.Convert.ToChar(34) + ", dataLenguaje) IsNotNullText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_NO_NULO" + System.Convert.ToChar(34) + ", dataLenguaje) IsNullText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_ES_NULO" + System.Convert.ToChar(34) + ", dataLenguaje) LessThanText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_MENOR_A" + System.Convert.ToChar(34) + ", dataLenguaje) LessThanOrEqualsText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_MENOR_A_O_IGUAL" + System.Convert.ToChar(34) + ", dataLenguaje) NotEqualsText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_NO_EQUIVALE" + System.Convert.ToChar(34) + ", dataLenguaje) IsNotEmptyText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_NO_VACIO" + System.Convert.ToChar(34) + ", dataLenguaje) OrOperatorText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_OPERADOR_OR" + System.Convert.ToChar(34) + ", dataLenguaje) GreaterThanText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_MAYOR_A" + System.Convert.ToChar(34) + ", dataLenguaje) GreaterThanOrEqualsText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_MAYOR_A_O_IGUAL" + System.Convert.ToChar(34) + ", dataLenguaje) AndOperatorText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_OPERADOR_AND" + System.Convert.ToChar(34) + ", dataLenguaje) AllColumnsText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "MOSTRAR_TODAS_COLUMNAS" + System.Convert.ToChar(34) + ", dataLenguaje) ContainsText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_CONTIENE" + System.Convert.ToChar(34) + ", dataLenguaje) DoesNotContainText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_NO_CONTIENE" + System.Convert.ToChar(34) + ", dataLenguaje) ClearFilterText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "LIMPIAR_FILTRO" + System.Convert.ToChar(34) + ", dataLenguaje) ApplyFilterText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "APLICAR_FILTRO" + System.Convert.ToChar(34) + ", dataLenguaje) EqualsText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_EXACTO" + System.Convert.ToChar(34) + ", dataLenguaje) EndsWithText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "FILTRO_TERMINA_CON" + System.Convert.ToChar(34) + ", dataLenguaje) PageSizeText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "REGISTROS_POR_PAGINA" + System.Convert.ToChar(34) + ", dataLenguaje) StartsWithText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "EMPIEZA_CON" + System.Convert.ToChar(34) + ", dataLenguaje) ColumnsShowingText=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "COLUMNAS_OPCIONALES" + System.Convert.ToChar(34) + ", dataLenguaje)");
                        output.AppendLine("         Data=" + System.Convert.ToChar(34) + "@" + table.Name + "_entities" + System.Convert.ToChar(34) + " TItem=" + System.Convert.ToChar(34) + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "" + System.Convert.ToChar(34) + " ColumnWidth=" + System.Convert.ToChar(34) + "300px" + System.Convert.ToChar(34) + " LogicalFilterOperator=" + System.Convert.ToChar(34) + "LogicalFilterOperator.And" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("         <EmptyTemplate>");
                        output.AppendLine("             <p style=" + System.Convert.ToChar(34) + "color: lightgrey; font-size: 24px; text-align: center; margin: 2rem;" + System.Convert.ToChar(34) + ">No existen registros.</p>");
                        output.AppendLine("         </EmptyTemplate>        ");
                        output.AppendLine("         <Columns>");
                        output.AppendLine("             <RadzenDataGridColumn TItem=" + System.Convert.ToChar(34) + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "" + System.Convert.ToChar(34) + " TextAlign=" + System.Convert.ToChar(34) + "TextAlign.Center" + System.Convert.ToChar(34) + columnCustomProperties + " >");
                        output.AppendLine("                 <HeaderTemplate>");
                        output.AppendLine("                     <RadzenButton Icon=" + System.Convert.ToChar(34) + "add_circle_outline" + System.Convert.ToChar(34) + " ButtonStyle=" + System.Convert.ToChar(34) + "ButtonStyle.Success" + System.Convert.ToChar(34) + " Visible=@SeguridadController.VerificarPermisoFuncion(" + System.Convert.ToChar(34) + table.Schema + table.Name + System.Convert.ToChar(34) + ",dataFunciones," + System.Convert.ToChar(34) + "PuedeAgregar" + System.Convert.ToChar(34) + ")" + " Text =@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "BOTON_NUEVO" + System.Convert.ToChar(34) + ",dataLenguaje)" + " Click=" + System.Convert.ToChar(34) + "Add" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                     </RadzenButton>");
                        output.AppendLine("                     </HeaderTemplate>");
                        output.AppendLine("                     <Template Context=" + System.Convert.ToChar(34) + "element" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                     <RadzenButton Icon=" + System.Convert.ToChar(34) + "edit" + System.Convert.ToChar(34) + " ButtonStyle=" + System.Convert.ToChar(34) + "ButtonStyle.Info" + System.Convert.ToChar(34) + " Visible=@SeguridadController.VerificarPermisoFuncion(" + System.Convert.ToChar(34) + table.Schema + table.Name + System.Convert.ToChar(34) + ",dataFunciones," + System.Convert.ToChar(34) + "PuedeModificar" + System.Convert.ToChar(34) + ")" + " Class=" + System.Convert.ToChar(34) + "m-1" + System.Convert.ToChar(34) + " Click=" + System.Convert.ToChar(34) + "@(args => Edit(element,false))" + System.Convert.ToChar(34) + " @onclick:stopPropagation=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                     </RadzenButton>");
                        output.AppendLine("                     <RadzenButton Icon=" + System.Convert.ToChar(34) + "delete" + System.Convert.ToChar(34) + " ButtonStyle=" + System.Convert.ToChar(34) + "ButtonStyle.Danger" + System.Convert.ToChar(34) + " Visible=@SeguridadController.VerificarPermisoFuncion(" + System.Convert.ToChar(34) + table.Schema + table.Name + System.Convert.ToChar(34) + ",dataFunciones," + System.Convert.ToChar(34) + "PuedeEliminar" + System.Convert.ToChar(34) + ")" + " Class=" + System.Convert.ToChar(34) + "m-1" + System.Convert.ToChar(34) + " Click=" + System.Convert.ToChar(34) + "@(args => Edit(element,true))" + System.Convert.ToChar(34) + " @onclick:stopPropagation=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                     </RadzenButton>");
                        output.AppendLine("                 </Template>");
                        output.AppendLine("                 </RadzenDataGridColumn>");
                        foreach (var column in table.Columns)
                        {
                            columnCustomProperties = " Filterable=@CustomizadorController.ObtenerPropiedadColumnaFiltrable(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                            columnCustomProperties += "Groupable=@CustomizadorController.ObtenerPropiedadColumnaAgrupable(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                            columnCustomProperties += "Resizable=@CustomizadorController.ObtenerPropiedadColumnaRedimensionable(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                            columnCustomProperties += "Reorderable=@CustomizadorController.ObtenerPropiedadColumnaMovible(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                            columnCustomProperties += "MinWidth=@CustomizadorController.ObtenerPropiedadAnchoMinimoColumnaEnGrilla(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                            columnCustomProperties += "Width=@CustomizadorController.ObtenerPropiedadAnchoColumnaEnGrilla(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                            columnCustomProperties += "Pickable=@CustomizadorController.ObtenerPropiedadVisibilidadColumnaEnSelector(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                            columnCustomProperties += "OrderIndex=@CustomizadorController.ObtenerPropiedadPosicionColumnaEnGrilla(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                            columnCustomProperties += "Sortable=@CustomizadorController.ObtenerPropiedadColumnaOrdenable(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";
                            columnCustomProperties += "Visible=@CustomizadorController.ObtenerPropiedadVisibilidadColumna(" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ",dataCustomizacionPagina.AtributosColumnas) ";

                            if (!column.IsComputed)
                            {
                                if (column.IsInPrimaryKey)
                                {
                                    output.AppendLine("             <RadzenDataGridColumn TItem=" + System.Convert.ToChar(34) + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "" + System.Convert.ToChar(34) + " Property=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + columnCustomProperties + " Title=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Frozen=" + System.Convert.ToChar(34) + "false" + System.Convert.ToChar(34) + " TextAlign=" + System.Convert.ToChar(34) + "TextAlign.Center" + System.Convert.ToChar(34) + " />");
                                }
                                else
                                {
                                    if (column.IsInForeignKey)
                                    {
                                        var foreignKey = column.ForeignKeys.FirstOrDefault();
                                        MyMeta.ITable fkTable = foreignKey.PrimaryTable;

                                        if (column.Name.Contains("Usuario"))
                                        {
                                            output.AppendLine("             <RadzenDataGridColumn TItem=" + System.Convert.ToChar(34) + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "" + System.Convert.ToChar(34) + columnCustomProperties + " Title=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Property=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Type=" + System.Convert.ToChar(34) + "typeof(IEnumerable<Int64>)" + System.Convert.ToChar(34) + " FilterValue=" + System.Convert.ToChar(34) + "@selectedFilter_" + column.Name + System.Convert.ToChar(34) + " FilterOperator=" + System.Convert.ToChar(34) + "FilterOperator.Contains" + System.Convert.ToChar(34) + " LogicalFilterOperator=" + System.Convert.ToChar(34) + "LogicalFilterOperator.And" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                 <Template>");
                                            output.AppendLine("                     @{");
                                            output.AppendLine("                         var usuario = " + fkTable.Name + "_" + column.Name + "_entities.FirstOrDefault( c => c.Id == @context." + column.Name + ");");
                                            output.AppendLine("                         if(usuario != null)");
                                            output.AppendLine("                         {");
                                            output.AppendLine("                             <RadzenLabel Text=@(usuario.NombreApellido) Component=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "width: 100%" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                             </RadzenLabel>");
                                            output.AppendLine("                         }");
                                            output.AppendLine("                         else");
                                            output.AppendLine("                         {");
                                            output.AppendLine("                             <RadzenLabel Text=" + System.Convert.ToChar(34) + "No data available" + System.Convert.ToChar(34) + " Component=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "width: 100%" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                             </RadzenLabel>                        ");
                                            output.AppendLine("                         }");
                                            output.AppendLine("                     }");
                                            output.AppendLine("                 </Template>");
                                            output.AppendLine("                 <FilterTemplate>");
                                            output.AppendLine("                     <div style=" + System.Convert.ToChar(34) + "margin-top: 1rem;font-weight: 600" + System.Convert.ToChar(34) + ">Filter</div>");
                                            output.AppendLine("                     <RadzenDropDown @bind-Value=@selectedFilter_" + column.Name + " Data=" + System.Convert.ToChar(34) + "@" + fkTable.Name + "_" + column.Name + "_filter" + System.Convert.ToChar(34) + " Change=" + System.Convert.ToChar(34) + "OnSelectedFilter_" + column.Name + "_Changed" + System.Convert.ToChar(34) + " AllowFiltering =" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " FilterCaseSensitivity=" + System.Convert.ToChar(34) + "FilterCaseSensitivity.CaseInsensitive" + System.Convert.ToChar(34) + " ValueProperty=" + System.Convert.ToChar(34) + "Id" + System.Convert.ToChar(34) + " TextProperty=" + System.Convert.ToChar(34) + "NombreApellido" + System.Convert.ToChar(34) + " AllowClear =" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " Multiple=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + "/>");
                                            output.AppendLine("                 </FilterTemplate>");
                                        }
                                        else
                                        {
                                            output.AppendLine("             <RadzenDataGridColumn TItem=" + System.Convert.ToChar(34) + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "" + System.Convert.ToChar(34) + columnCustomProperties + " Title=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Property=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Type=" + System.Convert.ToChar(34) + "typeof(IEnumerable<Int64>)" + System.Convert.ToChar(34) + " FilterValue=" + System.Convert.ToChar(34) + "@selectedFilter_" + column.Name + System.Convert.ToChar(34) + " FilterOperator=" + System.Convert.ToChar(34) + "FilterOperator.Contains" + System.Convert.ToChar(34) + " LogicalFilterOperator=" + System.Convert.ToChar(34) + "LogicalFilterOperator.And" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                 <Template>");
                                            output.AppendLine("                     @if(" + fkTable.Name + "_" + column.Name + "_entities.FirstOrDefault( c => c.Id == @context." + column.Name + ") != null){");
                                            output.AppendLine("                         <RadzenLabel Text=" + System.Convert.ToChar(34) + "@" + fkTable.Name + "_" + column.Name + "_entities.FirstOrDefault( c => c.Id == @context." + column.Name + ")." + displayColumn + System.Convert.ToChar(34) + " Component=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "width: 100%" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                             </RadzenLabel>");
                                            output.AppendLine("                         }");
                                            output.AppendLine("                         else");
                                            output.AppendLine("                         {");
                                            output.AppendLine("                         <RadzenLabel Text=" + System.Convert.ToChar(34) + "No data available" + System.Convert.ToChar(34) + " Component=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "width: 100%" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                             </RadzenLabel>                        ");
                                            output.AppendLine("                         }");
                                            output.AppendLine("                 </Template>");
                                            output.AppendLine("                 <FilterTemplate>");
                                            output.AppendLine("                     <div style=" + System.Convert.ToChar(34) + "margin-top: 1rem;font-weight: 600" + System.Convert.ToChar(34) + ">Filter</div>");
                                            output.AppendLine("                     <RadzenDropDown @bind-Value=@selectedFilter_" + column.Name + " Data=" + System.Convert.ToChar(34) + "@" + fkTable.Name + "_" + column.Name + "_filter" + System.Convert.ToChar(34) + " Change=" + System.Convert.ToChar(34) + "OnSelectedFilter_" + column.Name + "_Changed" + System.Convert.ToChar(34) + " AllowFiltering =" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " FilterCaseSensitivity=" + System.Convert.ToChar(34) + "FilterCaseSensitivity.CaseInsensitive" + System.Convert.ToChar(34) + " ValueProperty=" + System.Convert.ToChar(34) + "Id" + System.Convert.ToChar(34) + " TextProperty=" + System.Convert.ToChar(34) + displayColumn + System.Convert.ToChar(34) + " AllowClear=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " Multiple=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + "/>");
                                            output.AppendLine("                 </FilterTemplate>");
                                        }
                                        output.AppendLine("             </RadzenDataGridColumn>");

                                    }
                                    else
                                    {
                                        if (fieldsToTreatAsImage.IndexOf(column.Name) < 0)
                                        {

                                            switch (column.DataTypeName)
                                            {
                                                case "bit":
                                                    output.AppendLine("             <RadzenDataGridColumn TItem=" + System.Convert.ToChar(34) + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + System.Convert.ToChar(34) + " Property=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " FilterValue=" + System.Convert.ToChar(34) + "@" + column.Name + "_filter" + System.Convert.ToChar(34) + columnCustomProperties + " Title=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Frozen=" + System.Convert.ToChar(34) + "false" + System.Convert.ToChar(34) + " TextAlign=" + System.Convert.ToChar(34) + "TextAlign.Center" + System.Convert.ToChar(34) + " >");
                                                    output.AppendLine("             <Template Context=" + System.Convert.ToChar(34) + "data" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                 <RadzenCheckBox TValue =" + System.Convert.ToChar(34) + (column.IsNullable ? "bool?" : "bool") + System.Convert.ToChar(34) + " Value=@data." + column.Name + " Disabled=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                 </RadzenCheckBox>");
                                                    output.AppendLine("             </Template>");
                                                    output.AppendLine("             <FilterTemplate>");
                                                    output.AppendLine("                 <div style=" + System.Convert.ToChar(34) + "margin-top: 1rem;font-weight: 600" + System.Convert.ToChar(34) + ">Filter</div>");
                                                    output.AppendLine("                 <div>");
                                                    output.AppendLine("                     @MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)");
                                                    output.AppendLine("                     <RadzenCheckBox Style=" + System.Convert.ToChar(34) + "margin-bottom: 2%;" + System.Convert.ToChar(34) + " @bind-Value=" + System.Convert.ToChar(34) + column.Name + "_filter" + System.Convert.ToChar(34) + " TriState=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + "/>");
                                                    output.AppendLine("                 </div>");
                                                    output.AppendLine("             </FilterTemplate>");

                                                    break;
                                                case "ntext":
                                                    output.AppendLine("             <RadzenDataGridColumn TItem=" + System.Convert.ToChar(34) + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + System.Convert.ToChar(34) + " Property=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + columnCustomProperties + "Title=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Frozen=" + System.Convert.ToChar(34) + "false" + System.Convert.ToChar(34) + " TextAlign=" + System.Convert.ToChar(34) + "TextAlign.Center" + System.Convert.ToChar(34) + " >");
                                                    output.AppendLine("             <Template Context=" + System.Convert.ToChar(34) + "data" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                     <RadzenImage Path=" + System.Convert.ToChar(34) + "@data." + column.Name + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "width: 40px; height: 40px; border-radius: 8px;" + System.Convert.ToChar(34) + " >");
                                                    output.AppendLine("                 </RadzenImage>");
                                                    output.AppendLine("             </Template>");
                                                    break;
                                                default:
                                                    output.AppendLine("             <RadzenDataGridColumn TItem=" + System.Convert.ToChar(34) + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + System.Convert.ToChar(34) + " Property=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + columnCustomProperties + " Title=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Frozen=" + System.Convert.ToChar(34) + "false" + System.Convert.ToChar(34) + " TextAlign=" + System.Convert.ToChar(34) + "TextAlign.Center" + System.Convert.ToChar(34) + " >");
                                                    break;
                                            }
                                            output.AppendLine("     </RadzenDataGridColumn>");
                                        }
                                        else
                                        {
                                            output.AppendLine("             <RadzenDataGridColumn TItem=" + System.Convert.ToChar(34) + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + System.Convert.ToChar(34) + columnCustomProperties + " Title=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Frozen=" + System.Convert.ToChar(34) + "false" + System.Convert.ToChar(34) + " >");
                                            output.AppendLine("                 <Template Context=" + System.Convert.ToChar(34) + "data" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                     <RadzenImage Path=" + System.Convert.ToChar(34) + "@data." + column.Name + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "width: 40px; height: 40px; border-radius: 8px;" + System.Convert.ToChar(34) + " />");
                                            output.AppendLine("                 </Template>");
                                            output.AppendLine("             </RadzenDataGridColumn>");

                                        }
                                    }
                                }
                            }
                        }


                        output.AppendLine("         </Columns>");
                        output.AppendLine("         </RadzenDataGrid>");
                        output.AppendLine("     <hr>");
                        output.AppendLine("     }");
                        output.AppendLine(" ");
                        output.AppendLine("     if (crudMode == CrudMode.Add || crudMode == CrudMode.Edit || crudMode == CrudMode.Delete)");
                        output.AppendLine("     {");
                        output.AppendLine("             <RadzenTemplateForm TItem=" + System.Convert.ToChar(34) + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "" + System.Convert.ToChar(34) + " Data=" + System.Convert.ToChar(34) + "@" + table.Name + "_entity" + System.Convert.ToChar(34) + "  @bind-Value=" + System.Convert.ToChar(34) + table.Name + "_entity" + System.Convert.ToChar(34) + " Submit =" + System.Convert.ToChar(34) + "Save" + System.Convert.ToChar(34) + " Visible =" + System.Convert.ToChar(34) + "@(" + table.Name + "_entity != null)" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("             <ChildContent>");

                        foreach (var column in table.Columns)
                        {
                            //string Disabled = (column.IsAutoKey || column.Name == "FechaModificacion" || column.Name == "FechaCreacion") ? "true" : "@(crudMode==CrudMode.Delete)";
                            string Disabled = column.IsAutoKey ? "true" : "@(crudMode==CrudMode.Delete || estaProcesando)";

                            if (!column.IsComputed)
                            {
                                if (column.Name != "UsuarioCreacion" && column.Name != "UsuarioModificacion")
                                {
                                    if (column.IsInForeignKey && !column.IsInPrimaryKey)
                                    {
                                        var foreignKey = column.ForeignKeys.FirstOrDefault();
                                        MyMeta.ITable fkTable = foreignKey.PrimaryTable;
                                        string nullable = column.IsNullable ? "?" : "";

                                        if (!column.Name.Contains("Usuario"))
                                        {
                                            output.AppendLine("             <div style=" + System.Convert.ToChar(34) + "margin-bottom: 1rem" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                 <div class=" + System.Convert.ToChar(34) + "col-md-3" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                 <RadzenLabel Text=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Component =" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "width: 100%" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                 </RadzenLabel>");
                                            output.AppendLine("                 </div>");
                                            output.AppendLine("                 <div class=" + System.Convert.ToChar(34) + "col-md-4" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                     @if((@" + fkTable.Name + "_" + column.Name + "_entities.Count > 0 && @" + fkTable.Name + "_" + column.Name + "_entities.FirstOrDefault( c => c.Id == @" + table.Name + "_entity." + column.Name + ") != null) || crudMode != CrudMode.Delete)");
                                            output.AppendLine("                     {");
                                            output.AppendLine("                         var dataSet = crudMode == CrudMode.Add ? " + fkTable.Name + "_" + column.Name + "_entities.Where(x => x.Habilitado == true) : " + fkTable.Name + "_" + column.Name + "_entities.Where(x => x.Habilitado == true || x.Id == " + table.Name + "_entity." + column.Name + ");");
                                            output.AppendLine("                         <RadzenDropDown  Name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " AllowFiltering=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " FilterOperator=" + System.Convert.ToChar(34) + "StringFilterOperator.Contains" + System.Convert.ToChar(34) + " FilterCaseSensitivity=" + System.Convert.ToChar(34) + "FilterCaseSensitivity.CaseInsensitive" + System.Convert.ToChar(34) + " AllowClear=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " TValue=" + System.Convert.ToChar(34) + column.LanguageType + nullable + System.Convert.ToChar(34) + " Class=" + System.Convert.ToChar(34) + "w-100" + System.Convert.ToChar(34) + " @bind-Value=" + System.Convert.ToChar(34) + "@(" + table.Name + "_entity." + column.Name + ")" + System.Convert.ToChar(34));
                                            output.AppendLine("                                     Data=@dataSet TextProperty = " + System.Convert.ToChar(34) + displayColumn + System.Convert.ToChar(34) + " ValueProperty = " + System.Convert.ToChar(34) + foreignKey.PrimaryColumns.FirstOrDefault().Name + System.Convert.ToChar(34));
                                            output.AppendLine("                                     Change=@(args => Select" + fkTable.Name + "_" + column.Name + "ValueChanged(args))");
                                            output.AppendLine("                                     Disabled=" + System.Convert.ToChar(34) + Disabled + System.Convert.ToChar(34) + " />");
                                            output.AppendLine("                     }");
                                            output.AppendLine("                     else");
                                            output.AppendLine("                     {");
                                            output.AppendLine("                         <RadzenDropDown  Name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " AllowClear=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " TValue=" + System.Convert.ToChar(34) + column.LanguageType + nullable + System.Convert.ToChar(34) + " Class=" + System.Convert.ToChar(34) + "w-100" + System.Convert.ToChar(34) + " ");
                                            output.AppendLine("                                     Data=@" + fkTable.Name + "_" + column.Name + "_entities TextProperty = " + System.Convert.ToChar(34) + displayColumn + System.Convert.ToChar(34) + " ValueProperty = " + System.Convert.ToChar(34) + foreignKey.PrimaryColumns.FirstOrDefault().Name + System.Convert.ToChar(34));
                                            output.AppendLine("                                     Change=@(args => Select" + fkTable.Name + "_" + column.Name + "ValueChanged(args))");
                                            output.AppendLine("                                     Disabled=" + System.Convert.ToChar(34) + Disabled + System.Convert.ToChar(34) + " />");
                                            output.AppendLine("                     }");
                                            if (!column.IsNullable)
                                            {
                                                output.AppendLine("                    @if (@crudMode != CrudMode.Delete)");
                                                output.AppendLine("                    {");
                                                output.AppendLine("                        <RadzenNumericRangeValidator Min=" + System.Convert.ToChar(34) + "@(" + fkTable.Name + "_" + column.Name + "_entities.Count > 0 ? " + fkTable.Name + "_" + column.Name + "_entities.Min(x => x.Id) : 1)" + System.Convert.ToChar(34) + " Component =" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + column.Name + " is required" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "position: relative" + System.Convert.ToChar(34) + ">");
                                                output.AppendLine("                        </RadzenNumericRangeValidator>");
                                                output.AppendLine("                    }");

                                            }
                                            output.AppendLine("                </div>");
                                            output.AppendLine("              </div>");
                                        }
                                        else
                                        {
                                            output.AppendLine("             <div style=" + System.Convert.ToChar(34) + "margin-bottom: 1rem" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                 <div class=" + System.Convert.ToChar(34) + "col-md-3" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                 <RadzenLabel Text=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Component =" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "width: 100%" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                 </RadzenLabel>");
                                            output.AppendLine("                 </div>");
                                            output.AppendLine("                 <div class=" + System.Convert.ToChar(34) + "col-md-4" + System.Convert.ToChar(34) + ">");
                                            output.AppendLine("                     @if((@" + fkTable.Name + "_" + column.Name + "_entities.Count > 0 && @" + fkTable.Name + "_" + column.Name + "_entities.FirstOrDefault( c => c.Id == @" + table.Name + "_entity." + column.Name + ") != null) || crudMode != CrudMode.Delete)");
                                            output.AppendLine("                     {");
                                            output.AppendLine("                         var dataSet = crudMode == CrudMode.Add ? " + fkTable.Name + "_" + column.Name + "_entities.Where(x => x.Habilitado == true) : " + fkTable.Name + "_" + column.Name + "_entities.Where(x => x.Habilitado == true || x.Id == " + table.Name + "_entity." + column.Name + ");");
                                            output.AppendLine("                         <RadzenDropDown Name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " AllowClear=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " TValue=" + System.Convert.ToChar(34) + column.LanguageType + nullable + System.Convert.ToChar(34) + " Class=" + System.Convert.ToChar(34) + "w-100" + System.Convert.ToChar(34) + " @bind-Value=" + System.Convert.ToChar(34) + "@(" + table.Name + "_entity." + column.Name + ")" + System.Convert.ToChar(34));
                                            output.AppendLine("                                     Data=@dataSet TextProperty = " + System.Convert.ToChar(34) + "NombreApellido" + System.Convert.ToChar(34) + " ValueProperty = " + System.Convert.ToChar(34) + foreignKey.PrimaryColumns.FirstOrDefault().Name + System.Convert.ToChar(34));
                                            output.AppendLine("                                     Change=@(args => Select" + fkTable.Name + "_" + column.Name + "ValueChanged(args))");
                                            output.AppendLine("                                     Disabled=" + System.Convert.ToChar(34) + Disabled + System.Convert.ToChar(34) + " />");
                                            output.AppendLine("                     }");
                                            output.AppendLine("                     else");
                                            output.AppendLine("                     {");
                                            output.AppendLine("                         <RadzenDropDown Name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " AllowClear =" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " TValue=" + System.Convert.ToChar(34) + column.LanguageType + nullable + System.Convert.ToChar(34) + " Class=" + System.Convert.ToChar(34) + "w-100" + System.Convert.ToChar(34) + " ");
                                            output.AppendLine("                                     Data=@" + fkTable.Name + "_" + column.Name + "_entities TextProperty = " + System.Convert.ToChar(34) + "NombreApellido" + System.Convert.ToChar(34) + " ValueProperty = " + System.Convert.ToChar(34) + foreignKey.PrimaryColumns.FirstOrDefault().Name + System.Convert.ToChar(34));
                                            output.AppendLine("                                     Change=@(args => Select" + fkTable.Name + "_" + column.Name + "ValueChanged(args))");
                                            output.AppendLine("                                     Disabled=" + System.Convert.ToChar(34) + Disabled + System.Convert.ToChar(34) + " />");
                                            output.AppendLine("                    }");
                                            if (!column.IsNullable)
                                            {
                                                output.AppendLine("                    @if (@crudMode != CrudMode.Delete)");
                                                output.AppendLine("                    {");
                                                output.AppendLine("                        <RadzenNumericRangeValidator Min=" + System.Convert.ToChar(34) + "@(" + fkTable.Name + "_" + column.Name + "_entities.Count > 0 ? " + fkTable.Name + "_" + column.Name + "_entities.Min(x => x.Id) : 1)" + System.Convert.ToChar(34) + " Component =" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + column.Name + " is required" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "position: relative" + System.Convert.ToChar(34) + ">");
                                                output.AppendLine("                        </RadzenNumericRangeValidator>");
                                                output.AppendLine("                    }");
                                            }
                                            output.AppendLine("                </div>");
                                            output.AppendLine("              </div>");
                                        }
                                    }
                                    else
                                    {
                                        string fieldWidth = "col-md-auto";
                                        if (column.IsInPrimaryKey && column.IsAutoKey)
                                        {
                                            output.AppendLine("                @if(crudMode != CrudMode.Add)");
                                            output.AppendLine("                {");
                                        }
                                        switch (column.DataTypeName.ToLower())
                                        {
                                            case "tinyint":
                                            case "int":
                                            case "smallint":
                                            case "bigint":
                                            case "decimal":
                                            case "float":
                                                string numericFormat = string.Empty;
                                                //fieldWidth = "col-md-2";
                                                switch (column.DataTypeName.ToLower())
                                                {
                                                    case "int":
                                                        numericFormat = " TValue=" + System.Convert.ToChar(34) + (column.IsNullable ? "int?" : "int") + System.Convert.ToChar(34);
                                                        break;
                                                    case "smallint":
                                                        numericFormat = " TValue=" + System.Convert.ToChar(34) + (column.IsNullable ? "short?" : "short") + System.Convert.ToChar(34);
                                                        break;
                                                    case "bigint":
                                                        numericFormat = " TValue=" + System.Convert.ToChar(34) + (column.IsNullable ? "long?" : "long") + System.Convert.ToChar(34);
                                                        break;
                                                    case "decimal":
                                                        numericFormat = " Format=" + System.Convert.ToChar(34) + "c" + System.Convert.ToChar(34);
                                                        numericFormat = " TValue=" + System.Convert.ToChar(34) + "decimal" + System.Convert.ToChar(34);
                                                        break;
                                                    case "float":
                                                        numericFormat = " Format=" + System.Convert.ToChar(34) + "c" + System.Convert.ToChar(34);
                                                        numericFormat = " TValue=" + System.Convert.ToChar(34) + "float" + System.Convert.ToChar(34);
                                                        break;
                                                    default:
                                                        numericFormat = " TValue=" + System.Convert.ToChar(34) + "int" + System.Convert.ToChar(34);
                                                        break;
                                                }

                                                if (column.Name != "UsuarioModificacion" && column.Name != "UsuarioCreacion")
                                                {
                                                    output.AppendLine("                 <div style=" + System.Convert.ToChar(34) + "margin-bottom: 1rem" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                     <div class=" + System.Convert.ToChar(34) + "col-md-3" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                         <RadzenLabel Text=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Component=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "width: 100%" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                         </RadzenLabel>");
                                                    output.AppendLine("                     </div>");
                                                    output.AppendLine("                     <div class=" + System.Convert.ToChar(34) + fieldWidth + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                         <RadzenNumeric " + numericFormat + " style=" + System.Convert.ToChar(34) + "display: block; width: 100%" + System.Convert.ToChar(34) + " @bind-Value=" + System.Convert.ToChar(34) + "@(" + table.Name + "_entity." + column.Name + ")" + System.Convert.ToChar(34) + " Name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Disabled=" + System.Convert.ToChar(34) + Disabled + System.Convert.ToChar(34) + " >");
                                                    output.AppendLine("                     </RadzenNumeric>");
                                                    output.AppendLine("                     </div>");
                                                    output.AppendLine("                 </div>");
                                                }

                                                break;
                                            case "date":
                                                if (column.Name != "FechaModificacion" && column.Name != "FechaCreacion")
                                                {
                                                    output.AppendLine("                 <div style=" + System.Convert.ToChar(34) + "margin-bottom: 1rem" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                     <div class=" + System.Convert.ToChar(34) + "col-md-3" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                         <RadzenLabel Text=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Component=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "width: 100%" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                         </RadzenLabel>");
                                                    output.AppendLine("                     </div>");
                                                    output.AppendLine("                 <div class=" + System.Convert.ToChar(34) + fieldWidth + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                     <RadzenDatePicker style=" + System.Convert.ToChar(34) + "width: 25%" + System.Convert.ToChar(34) + " @bind-Value=" + System.Convert.ToChar(34) + "@(" + table.Name + "_entity." + column.Name + ")" + System.Convert.ToChar(34) + " Name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Disabled=" + System.Convert.ToChar(34) + Disabled + System.Convert.ToChar(34) + " >");
                                                    output.AppendLine("                     </RadzenDatePicker>");
                                                    output.AppendLine("                 </div>");
                                                    output.AppendLine("             </div>");
                                                }
                                                break;
                                            case "datetime":
                                            case "smalldatetime":
                                                if (column.Name != "FechaModificacion" && column.Name != "FechaCreacion")
                                                {
                                                    output.AppendLine("                 <div style=" + System.Convert.ToChar(34) + "margin-bottom: 1rem" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                     <div class=" + System.Convert.ToChar(34) + "col-md-3" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                         <RadzenLabel Text=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Component=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "width: 100%" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                         </RadzenLabel>");
                                                    output.AppendLine("                     </div>");
                                                    output.AppendLine("                 <div class=" + System.Convert.ToChar(34) + fieldWidth + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                     <RadzenDatePicker ShowTime=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " ShowSeconds =" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " style =" + System.Convert.ToChar(34) + "width: 25%" + System.Convert.ToChar(34) + " @bind-Value=" + System.Convert.ToChar(34) + "@(" + table.Name + "_entity." + column.Name + ")" + System.Convert.ToChar(34) + " Name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Disabled=" + System.Convert.ToChar(34) + Disabled + System.Convert.ToChar(34) + " >");
                                                    output.AppendLine("                     </RadzenDatePicker>");
                                                    output.AppendLine("                 </div>");
                                                    output.AppendLine("             </div>");
                                                }
                                                break;
                                            case "bit":
                                                output.AppendLine("                 <div style=" + System.Convert.ToChar(34) + "margin-bottom: 1rem" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                                                output.AppendLine("                     <div class=" + System.Convert.ToChar(34) + "col-md-3" + System.Convert.ToChar(34) + ">");
                                                output.AppendLine("                 <RadzenLabel Text=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Component=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "margin-left: 0px; vertical-align: middle;" + System.Convert.ToChar(34) + " />");
                                                output.AppendLine("                     </div>");
                                                output.AppendLine("                     <div class=" + System.Convert.ToChar(34) + fieldWidth + System.Convert.ToChar(34) + ">");
                                                output.AppendLine("                 <RadzenCheckBox @bind-Value=@" + table.Name + "_entity." + column.Name + "  Name=" + System.Convert.ToChar(34) + column.Name + "CheckBox" + System.Convert.ToChar(34) + " TValue=" + System.Convert.ToChar(34) + (column.IsNullable ? "bool?" : "bool") + System.Convert.ToChar(34) + " Change=@(args => " + column.Name + "Change(args, " + "@" + table.Name + "_entity" + "))" + " Disabled = " + System.Convert.ToChar(34) + "@(crudMode == CrudMode.Delete)" + System.Convert.ToChar(34) + " />");
                                                output.AppendLine("                     </div>");
                                                output.AppendLine("                 </div>");
                                                break;
                                            case "image":
                                            case "varbinary":
                                                break;
                                            case "ntext":
                                                output.AppendLine("                 <div style=" + System.Convert.ToChar(34) + "margin-bottom: 1rem" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                                                output.AppendLine("                     <div class=" + System.Convert.ToChar(34) + "col-md-3" + System.Convert.ToChar(34) + ">");
                                                output.AppendLine("                 <RadzenLabel Text=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Component=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "margin-left: 0px; vertical-align: middle;" + System.Convert.ToChar(34) + " />");
                                                output.AppendLine("                     </div>");
                                                output.AppendLine("                     <div class=" + System.Convert.ToChar(34) + fieldWidth + System.Convert.ToChar(34) + ">");
                                                output.AppendLine("                         <RadzenFileInput @bind-Value=" + System.Convert.ToChar(34) + "@" + table.Name + "_entity." + column.Name + System.Convert.ToChar(34) + " Error =" + System.Convert.ToChar(34) + "@ErrorFileInput_" + column.Name + System.Convert.ToChar(34) + " TValue=" + System.Convert.ToChar(34) + "string" + System.Convert.ToChar(34) + " Class=" + System.Convert.ToChar(34) + "w-100" + System.Convert.ToChar(34) + " />  ");
                                                output.AppendLine("                     </div>");
                                                output.AppendLine("                 </div>");

                                                break;
                                            case "text":
                                            default:

                                                if (fieldsToTreatAsImage.IndexOf(column.Name) < 0)
                                                {
                                                    output.AppendLine("                 <div style=" + System.Convert.ToChar(34) + "margin-bottom: 1rem" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                     <div class=" + System.Convert.ToChar(34) + "col-md-3" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                         <RadzenLabel Text=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Component=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "width: 100%" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                         </RadzenLabel>");
                                                    output.AppendLine("                     </div>");
                                                    output.AppendLine("                     <div class=" + System.Convert.ToChar(34) + fieldWidth + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                         <RadzenTextBox  style=" + System.Convert.ToChar(34) + "display: block" + System.Convert.ToChar(34) + " @bind-Value=" + System.Convert.ToChar(34) + "@(" + table.Name + "_entity." + column.Name + ")" + System.Convert.ToChar(34) + " Name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Disabled=" + System.Convert.ToChar(34) + Disabled + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                         </RadzenTextBox>");
                                                    if (!column.IsNullable)
                                                    {
                                                        output.AppendLine("                         @if(@crudMode!=CrudMode.Delete)");
                                                        output.AppendLine("                         {");
                                                        output.AppendLine("                             <RadzenRequiredValidator Component=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + column.Name + " is required" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "position: absolute" + System.Convert.ToChar(34) + ">");
                                                        output.AppendLine("                             </RadzenRequiredValidator>");
                                                        output.AppendLine("                         }");
                                                    }
                                                    output.AppendLine("                     </div>");
                                                    output.AppendLine("                 </div>");
                                                }
                                                else
                                                {
                                                    output.AppendLine("                 <div style=" + System.Convert.ToChar(34) + "margin-bottom: 1rem" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                     <div class=" + System.Convert.ToChar(34) + "col-md-3" + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                 <RadzenLabel Text=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + column.Name.ToUpper() + System.Convert.ToChar(34) + ",dataLenguaje)" + " Component=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "margin-left: 0px; vertical-align: middle;" + System.Convert.ToChar(34) + " />");
                                                    output.AppendLine("                     </div>");
                                                    output.AppendLine("                     <div class=" + System.Convert.ToChar(34) + fieldWidth + System.Convert.ToChar(34) + ">");
                                                    output.AppendLine("                         <RadzenFileInput @bind-Value=" + System.Convert.ToChar(34) + "@" + table.Name + "_entity." + column.Name + System.Convert.ToChar(34) + " TValue=" + System.Convert.ToChar(34) + "string" + System.Convert.ToChar(34) + " Class=" + System.Convert.ToChar(34) + "w-100" + System.Convert.ToChar(34) + " />  ");
                                                    output.AppendLine("                     </div>");
                                                    output.AppendLine("                 </div>");

                                                }
                                                break;
                                        }
                                        if (column.IsInPrimaryKey && column.IsAutoKey)
                                        {
                                            output.AppendLine("                }");
                                        }
                                    }
                                }
                            }
                        }
                        output.AppendLine("             <div class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                 <div class=" + System.Convert.ToChar(34) + "col offset-sm-3" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                 <RadzenButton Disabled=" + System.Convert.ToChar(34) + "@estaProcesando" + System.Convert.ToChar(34) + " ButtonType=" + System.Convert.ToChar(34) + "ButtonType.Submit" + System.Convert.ToChar(34) + " Icon=" + System.Convert.ToChar(34) + "save" + System.Convert.ToChar(34) + " Text=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "BOTON_ACEPTAR" + System.Convert.ToChar(34) + ",dataLenguaje)" + " ButtonStyle =" + System.Convert.ToChar(34) + "ButtonStyle.Primary" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                 </RadzenButton>");
                        output.AppendLine("                 <RadzenButton Disabled=" + System.Convert.ToChar(34) + "@estaProcesando" + System.Convert.ToChar(34) + " ButtonStyle =" + System.Convert.ToChar(34) + "ButtonStyle.Secondary" + System.Convert.ToChar(34) + " Icon = " + System.Convert.ToChar(34) + "save" + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "margin-left: 1rem" + System.Convert.ToChar(34) + " Text=@MultilenguajeController.ObtenerTextoPorClave(" + System.Convert.ToChar(34) + "BOTON_CANCELAR" + System.Convert.ToChar(34) + ",dataLenguaje)" + " Click=" + System.Convert.ToChar(34) + "Cancel" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                 </RadzenButton>");
                        output.AppendLine("                  </div>");
                        output.AppendLine("             </div>");
                        output.AppendLine("             </ChildContent>");
                        output.AppendLine("             </RadzenTemplateForm>");
                        output.AppendLine("     <hr>");
                        output.AppendLine("     }");
                        output.AppendLine(" ");
                        output.AppendLine("     <RadzenNotification /> ");
                        output.AppendLine(" }");
                        output.AppendLine("}");
                        output.AppendLine("else");
                        output.AppendLine("{");
                        output.AppendLine("     <p>No tiene permisos para visualizar los registros.</p>");
                        output.AppendLine("}");
                        output.AppendLine("}");
                        output.AppendLine("else");
                        output.AppendLine("{");
                        output.AppendLine("     <div class=" + System.Convert.ToChar(34) + "spinner" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("}");

                        output.AppendLine(" ");
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
                        output.AppendLine("    /// Variable to save grid state");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private DataGridSettings GridSettings;");
                        output.AppendLine(" ");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Variables to handle main entity");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " " + table.Name + "_entity = new();");
                        output.AppendLine("    private List<" + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "> " + table.Name + "_entities = null;");
                        output.AppendLine(" ");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Variables to store multilanguaje");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private List<Entities.TextoLenguaje> dataLenguaje;");
                        output.AppendLine(" ");
                        output.AppendLine("    private List<" + generationProject.Namespace + ".Entities.Tables.Estilo.EsquemaDetalle> dataEsquemaDetalle = new();");
                        output.AppendLine(" ");
                        output.AppendLine("    private bool estaProcesando = false ;");
                        output.AppendLine(" ");
                        output.AppendLine("    private " + generationProject.Namespace + ".Entities.Tables.Seguridad.Rol? usuarioRol = new();");
                        output.AppendLine(" ");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// List which stores definitions to customize grid columns");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private CustomizadorEntities.CustomizacionPagina dataCustomizacionPagina = new();");
                        output.AppendLine(" ");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Variables to store permissions");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private List<SeguridadEntities.FuncionRol> dataFunciones;");
                        output.AppendLine(" ");
                        foreach (var column in table.Columns)
                        {
                            if (!column.IsInPrimaryKey)
                            {
                                if (column.IsInForeignKey)
                                {
                                    var foreignKey = column.ForeignKeys.FirstOrDefault();
                                    MyMeta.ITable fkTable = foreignKey.PrimaryTable;

                                    //if (!fkTable.Equals(table))
                                    //{
                                    output.AppendLine("    /// <summary>");
                                    output.AppendLine("    /// Variables to handle Relation between " + table.Name + "." + column.Name + " and " + fkTable.Name + "." + foreignKey.PrimaryColumns.FirstOrDefault().Name);
                                    output.AppendLine("    /// </summary>");
                                    output.AppendLine("    private " + generationProject.Namespace + ".Entities.Tables." + fkTable.Schema + "." + fkTable.Name + " " + fkTable.Name + "_" + column.Name + "_entity = new();");
                                    output.AppendLine("    private List<" + generationProject.Namespace + ".Entities.Tables." + fkTable.Schema + "." + fkTable.Name + ">? " + fkTable.Name + "_" + column.Name + "_entities;");
                                    output.AppendLine("    private List<" + generationProject.Namespace + ".Entities.Tables." + fkTable.Schema + "." + fkTable.Name + "> " + fkTable.Name + "_" + column.Name + "_filter;");
                                    output.AppendLine("    private IEnumerable<Int64> selectedFilter_" + column.Name + ";");

                                    output.AppendLine(" ");
                                    //}
                                }
                                else
                                {
                                    if (column.DataTypeName == "bit")
                                    {
                                        output.AppendLine("    /// <summary>");
                                        output.AppendLine("    /// Variables to handle column filter for field " + table.Name + "." + column.Name);
                                        output.AppendLine("    private bool? " + column.Name + "_filter;");
                                        output.AppendLine(" ");
                                    }
                                }
                            }
                        }


                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    ///  Page crud mode {List,Add,Edit,Delete}");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private CrudMode crudMode = CrudMode.List;");
                        output.AppendLine(" ");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    ///  Variable to store the user id");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private " + generationProject.Namespace + ".Entities.Tables.Seguridad.Usuario? dataUsuario = null;");
                        output.AppendLine(" ");
                        output.AppendLine("    protected override async Task OnInitializedAsync()");
                        output.AppendLine("    {");
                        output.AppendLine("        dataUsuario = await sessionStorage.GetItemAsync<" + generationProject.Namespace + ".Entities.Tables.Seguridad.Usuario?>(" + System.Convert.ToChar(34) + "DataUsuario" + System.Convert.ToChar(34) + ");");
                        output.AppendLine("        if (dataUsuario == null)");
                        output.AppendLine("             NavManager.NavigateTo(" + System.Convert.ToChar(34) + "login" + System.Convert.ToChar(34) + ", true);");
                        output.AppendLine("        dataLenguaje = await sessionStorage.GetItemAsync<List<Entities.TextoLenguaje>>(" + System.Convert.ToChar(34) + "DataLenguaje" + System.Convert.ToChar(34) + ");");
                        output.AppendLine("        await sessionStorage.RemoveItemAsync(" + System.Convert.ToChar(34) + "DataFunciones" + System.Convert.ToChar(34) + ");");
                        output.AppendLine("        await sessionStorage.RemoveItemAsync(" + System.Convert.ToChar(34) + "RolId" + System.Convert.ToChar(34) + ");");
                        output.AppendLine("        dataCustomizacionPagina = CustomizadorController.ObtenerCustomizacionPagina(" + System.Convert.ToChar(34) + table.Schema + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + table.Name + System.Convert.ToChar(34) + ");");
                        output.AppendLine("        dataEsquemaDetalle = await sessionStorage.GetItemAsync<List<" + generationProject.Namespace + ".Entities.Tables.Estilo.EsquemaDetalle>>(" + System.Convert.ToChar(34) + "DataEsquemaDetalle" + System.Convert.ToChar(34) + ");");
                        output.AppendLine("        usuarioRol = SeguridadController.ObtenerRolesPorUsuario(dataUsuario.Id);");
                        output.AppendLine("        dataFunciones = SeguridadController.ObtenerFuncionesPorRol(usuarioRol.Id);");
                        output.AppendLine("        await sessionStorage.SetItemAsync(" + System.Convert.ToChar(34) + "RolId" + System.Convert.ToChar(34) + ", usuarioRol.Id);");
                        output.AppendLine("        await sessionStorage.SetItemAsync(" + System.Convert.ToChar(34) + "DataFunciones" + System.Convert.ToChar(34) + ", dataFunciones);");
                        output.AppendLine("        await Task.Run(LoadMainEntityData);");
                        output.AppendLine("        await Task.Run(LoadTypesData);");
                        output.AppendLine("    }");
                        output.AppendLine("    #region Loading entities data");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Loads main entity data");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void LoadMainEntityData()");
                        output.AppendLine("    {");
                        output.AppendLine("        if (dataCustomizacionPagina.AtributosTabla.HabilitarAuditoria)");
                        output.AppendLine("             AuditController.Log(AuditController.LogTypeEnum.Navigation, " + System.Convert.ToChar(34) + "Acceso a " + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + table.Schema + table.Name + "Crud" + System.Convert.ToChar(34) + ", crudMode.ToString(), dataUsuario.Id);");
                        output.AppendLine("        estaProcesando = false;");
                        output.AppendLine("        " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " entity = new();");
                        output.AppendLine("        " + table.Name + "_entities = entity.Items();");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Loads Type Tables data");
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

                                    output.AppendLine("        " + generationProject.Namespace + ".Business.Tables." + fkTable.Schema + "." + fkTable.Name + " " + fkTable.Name + "_" + column.Name + "_local = new();");
                                    output.AppendLine("        " + fkTable.Name + "_" + column.Name + "_entities = " + fkTable.Name + "_" + column.Name + "_local.Items();");
                                    output.AppendLine("        " + fkTable.Name + "_" + column.Name + "_filter = " + fkTable.Name + "_" + column.Name + "_entities.Where(x => " + table.Name + "_entities.Select(x => x." + column.Name + ").Distinct().Contains(x.Id)).ToList();");
                                    output.AppendLine(" ");
                                }
                            }
                        }
                        output.AppendLine("    }");
                        output.AppendLine(" ");
                        output.AppendLine("    #endregion");
                        output.AppendLine(" ");
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
                                    //Si es autoreferenciada no generamos el codigo
                                    if (fkTable.Name != table.Name)
                                        output.AppendLine("        " + fkTable.Name + "_" + column.Name + "_entity = new(); ");
                                }
                            }
                        }

                        output.AppendLine("        crudMode = CrudMode.Add;");
                        output.AppendLine("        if (dataCustomizacionPagina.AtributosTabla.HabilitarAuditoria)");
                        output.AppendLine("             AuditController.Log(AuditController.LogTypeEnum.Navigation, " + System.Convert.ToChar(34) + "Acceso a " + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + table.Schema + table.Name + "Crud" + System.Convert.ToChar(34) + ", crudMode.ToString(), dataUsuario.Id);");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Initializations for editing an entity");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void Edit(" + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " item, bool delete)");
                        output.AppendLine("    {");
                        output.AppendLine("             " + table.Name + "_entity = item;");

                        foreach (var column in table.Columns)
                        {
                            if (!column.IsInPrimaryKey)
                            {
                                if (column.IsInForeignKey)
                                {
                                    var foreignKey = column.ForeignKeys.FirstOrDefault();
                                    MyMeta.ITable fkTable = foreignKey.PrimaryTable;

                                    //Si es autoreferenciada no generamos el codigo
                                    if (fkTable.Name != table.Name)
                                    {
                                        if (column.IsNullable)
                                        {
                                            output.AppendLine("            if(" + fkTable.Name + "_" + column.Name + "_entities.Exists(c => c.Id == " + table.Name + "_entity." + column.Name + "))");
                                            output.AppendLine("            {");
                                            output.AppendLine("                 " + fkTable.Name + "_" + column.Name + "_entity = new() { ");
                                            output.AppendLine("                 Id = " + fkTable.Name + "_" + column.Name + "_entities.FirstOrDefault(c => c.Id == " + table.Name + "_entity." + column.Name + ").Id, ");
                                            output.AppendLine("                 " + displayColumn + " = " + fkTable.Name + "_" + column.Name + "_entities.FirstOrDefault(c => c.Id == " + table.Name + "_entity." + column.Name + ")." + displayColumn);
                                            output.AppendLine("                 };");
                                            output.AppendLine("            }");
                                        }
                                        else
                                        {
                                            output.AppendLine("            " + fkTable.Name + "_" + column.Name + "_entity = new() { ");
                                            output.AppendLine("            Id = " + fkTable.Name + "_" + column.Name + "_entities.FirstOrDefault(c => c.Id == " + table.Name + "_entity." + column.Name + ").Id, ");
                                            output.AppendLine("            " + displayColumn + " = " + fkTable.Name + "_" + column.Name + "_entities.FirstOrDefault(c => c.Id == " + table.Name + "_entity." + column.Name + ")." + displayColumn);
                                            output.AppendLine("            };");
                                        }
                                    }

                                }
                            }
                        }


                        output.AppendLine("        if (delete)");
                        output.AppendLine("            crudMode = CrudMode.Delete;");
                        output.AppendLine("        else");
                        output.AppendLine("            crudMode = CrudMode.Edit;");
                        output.AppendLine("");
                        output.AppendLine("        if (dataCustomizacionPagina.AtributosTabla.HabilitarAuditoria)");
                        output.AppendLine("             AuditController.Log(AuditController.LogTypeEnum.Navigation, " + System.Convert.ToChar(34) + "Acceso a " + System.Convert.ToChar(34) + " + " + System.Convert.ToChar(34) + table.Schema + table.Name + "Crud" + System.Convert.ToChar(34) + ", crudMode.ToString() + " + System.Convert.ToChar(34) + " Id: " + System.Convert.ToChar(34) + " + item.Id, dataUsuario.Id);");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Operation depending on Crud mode");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private async Task Save()");
                        output.AppendLine("    {");
                        output.AppendLine("        estaProcesando = true;");
                        output.AppendLine("        await Task.Run(Procesar);");
                        output.AppendLine("        estaProcesando = false;");
                        output.AppendLine("    }");
                        output.AppendLine("");
                        output.AppendLine("    private async Task Procesar()");
                        output.AppendLine("    {");
                        output.AppendLine("        string actionInformation = string.Empty;");
                        output.AppendLine("        NotificationSeverity severityType = NotificationSeverity.Error;");
                        output.AppendLine("        string summary = string.Empty;");
                        output.AppendLine("        " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " crud = new();");
                        output.AppendLine("        foreach(var property in " + table.Name + "_entity.GetType().GetProperties())");
                        output.AppendLine("        {");
                        output.AppendLine("             switch(property.Name)");
                        output.AppendLine("             {");
                        if (table.Columns.Where(x => x.Name == "FechaModificacion").FirstOrDefault() != null)
                        {
                            output.AppendLine("                 case " + System.Convert.ToChar(34) + "FechaModificacion" + System.Convert.ToChar(34) + ":");
                            output.AppendLine("                 if(crudMode == CrudMode.Add)");
                            output.AppendLine("                 {");
                            output.AppendLine("                     " + table.Name + "_entity.FechaModificacion=null;");
                            output.AppendLine("                 }");
                            output.AppendLine("                 else");
                            output.AppendLine("                 {");
                            output.AppendLine("                     " + table.Name + "_entity.FechaModificacion=DateTime.Now;");
                            output.AppendLine("                 }");
                            output.AppendLine("                 break;");
                        }
                        if (table.Columns.Where(x => x.Name == "FechaCreacion").FirstOrDefault() != null)
                        {
                            output.AppendLine("                 case " + System.Convert.ToChar(34) + "FechaCreacion" + System.Convert.ToChar(34) + ":");
                            output.AppendLine("                 if(crudMode == CrudMode.Add)");
                            output.AppendLine("                 {");
                            output.AppendLine("                     " + table.Name + "_entity.FechaCreacion=DateTime.Now;");
                            output.AppendLine("                 }");
                            output.AppendLine("                 break;");
                        }
                        if (table.Columns.Where(x => x.Name == "UsuarioCreacion").FirstOrDefault() != null)
                        {
                            output.AppendLine("                 case " + System.Convert.ToChar(34) + "UsuarioCreacion" + System.Convert.ToChar(34) + ":");
                            output.AppendLine("                 if(crudMode == CrudMode.Add)");
                            output.AppendLine("                 {");
                            output.AppendLine("                     " + table.Name + "_entity.UsuarioCreacion=dataUsuario.Id;");
                            output.AppendLine("                 }");
                            output.AppendLine("                 break;");
                        }
                        if (table.Columns.Where(x => x.Name == "UsuarioCreacion").FirstOrDefault() != null)
                        {
                            output.AppendLine("                 case " + System.Convert.ToChar(34) + "UsuarioModificacion" + System.Convert.ToChar(34) + ":");
                            output.AppendLine("                 if(crudMode == CrudMode.Add)");
                            output.AppendLine("                 {");
                            output.AppendLine("                     " + table.Name + "_entity.UsuarioModificacion=null;");
                            output.AppendLine("                 }");
                            output.AppendLine("                 else");
                            output.AppendLine("                 {");
                            output.AppendLine("                     " + table.Name + "_entity.UsuarioModificacion=dataUsuario.Id;");
                            output.AppendLine("                 }");
                            output.AppendLine("                 break;");
                        }
                        output.AppendLine("             }");
                        output.AppendLine("        }");


                        output.AppendLine("        switch (crudMode)");
                        output.AppendLine("        {");
                        output.AppendLine("            case CrudMode.Add:");
                        output.AppendLine("                try");
                        output.AppendLine("                {");
                        output.AppendLine("                     crud.Add(" + table.Name + "_entity, (long)SeguridadEntities.Aplicacion.AdministradorWeb);");
                        output.AppendLine("                     summary = " + System.Convert.ToChar(34) + "El registro se ha " + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                     actionInformation = " + System.Convert.ToChar(34) + "agregado." + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                     severityType = NotificationSeverity.Success;");
                        output.AppendLine("                }");
                        output.AppendLine("                catch(Exception ex)");
                        output.AppendLine("                {");
                        output.AppendLine("                     AuditController.Log(ex, dataUsuario.Id);");
                        output.AppendLine("                     summary = " + System.Convert.ToChar(34) + "Error al procesar." + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                     actionInformation = ex.Message;");
                        output.AppendLine("                     severityType = NotificationSeverity.Error;");
                        output.AppendLine("                }");
                        output.AppendLine("            break;");
                        output.AppendLine("            case CrudMode.Edit:");
                        output.AppendLine("                try");
                        output.AppendLine("                {");
                        output.AppendLine("                     crud.Update(" + table.Name + "_entity, (long)SeguridadEntities.Aplicacion.AdministradorWeb);");
                        output.AppendLine("                     summary = " + System.Convert.ToChar(34) + "El registro se ha " + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                     actionInformation = " + System.Convert.ToChar(34) + "modificado." + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                     severityType = NotificationSeverity.Success;");
                        output.AppendLine("                }");
                        output.AppendLine("                catch(Exception ex)");
                        output.AppendLine("                {");
                        output.AppendLine("                     AuditController.Log(ex, dataUsuario.Id);");
                        output.AppendLine("                     summary = " + System.Convert.ToChar(34) + "Error al procesar." + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                     actionInformation = ex.Message;");
                        output.AppendLine("                     severityType = NotificationSeverity.Error;");
                        output.AppendLine("                }");
                        output.AppendLine("            break;");
                        output.AppendLine("            case CrudMode.Delete:");
                        output.AppendLine("                try");
                        output.AppendLine("                {");
                        if (table.Columns.Where(x => x.Name == "Habilitado").FirstOrDefault() != null)
                        {
                            output.AppendLine("                     " + table.Name + "_entity.Habilitado=false;");
                            output.AppendLine("                     crud.Update(" + table.Name + "_entity, (long)SeguridadEntities.Aplicacion.AdministradorWeb);");
                        }
                        else
                            output.AppendLine("                     crud.Delete(" + table.Name + "_entity, (long)SeguridadEntities.Aplicacion.AdministradorWeb);");
                        output.AppendLine("                     summary = " + System.Convert.ToChar(34) + "El registro se ha " + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                     actionInformation = " + System.Convert.ToChar(34) + "eliminado." + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                     severityType = NotificationSeverity.Success;");
                        output.AppendLine("                }");
                        output.AppendLine("                catch(Exception ex)");
                        output.AppendLine("                {");
                        output.AppendLine("                     AuditController.Log(ex, dataUsuario.Id);");
                        output.AppendLine("                     summary = " + System.Convert.ToChar(34) + "Error al procesar." + System.Convert.ToChar(34) + ";");
                        output.AppendLine("                     actionInformation = ex.Message;");
                        output.AppendLine("                     severityType = NotificationSeverity.Error;");
                        output.AppendLine("                }");
                        output.AppendLine("                break;");
                        output.AppendLine("        }");
                        output.AppendLine(" ");
                        output.AppendLine("        // Sets Crud mode to List");
                        output.AppendLine("        crudMode = CrudMode.List;");
                        output.AppendLine(" ");
                        output.AppendLine("        ShowNotification(new NotificationMessage { ");
                        output.AppendLine("            Severity = severityType, ");
                        output.AppendLine("            Summary = summary, ");
                        output.AppendLine("            Detail = actionInformation, ");
                        output.AppendLine("            Duration = 4000 });");
                        output.AppendLine("       ");
                        output.AppendLine("            await Task.Run(LoadMainEntityData);");
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
                        output.AppendLine("    #region Notifications");
                        output.AppendLine("    private void ShowNotification(NotificationMessage message)");
                        output.AppendLine("    {");
                        output.AppendLine("        NotificationService.Notify(message);");
                        output.AppendLine(" ");
                        output.AppendLine("    }");
                        output.AppendLine(" ");
                        output.AppendLine("    #endregion");
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
                                    output.AppendLine("    /// " + fkTable.Name + "_" + column.Name + " value changed");
                                    output.AppendLine("    /// </summary>");
                                    output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "item" + System.Convert.ToChar(34) + "></param>");
                                    output.AppendLine("    private void Select" + fkTable.Name + "_" + column.Name + "ValueChanged(object item)");
                                    output.AppendLine("    {");


                                    //output.AppendLine("        " + table.Name + "_entity." + column.Name + " = " + fkTable.Name + "_entities.FirstOrDefault(c => c." + displayColumn + " == item).Id;");
                                    output.AppendLine("    }");
                                    output.AppendLine(" ");

                                    output.AppendLine("    /// <summary>");
                                    output.AppendLine("    /// selectedFilter_" + column.Name + " value changed");
                                    output.AppendLine("    /// </summary>");
                                    output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "item" + System.Convert.ToChar(34) + "></param>");
                                    output.AppendLine("    private void OnSelectedFilter_" + column.Name + "_Changed(object value)");
                                    output.AppendLine("    {");
                                    output.AppendLine("         if(selectedFilter_" + column.Name + "!=null && !selectedFilter_" + column.Name + ".Any())");
                                    output.AppendLine("         {");
                                    output.AppendLine("             selectedFilter_" + column.Name + "=null;");
                                    output.AppendLine("         }");
                                    output.AppendLine("    }");
                                    output.AppendLine(" ");
                                }
                            }
                            if (column.DataTypeName.ToLower() == "ntext")
                            {
                                output.AppendLine("    /// <summary>");
                                output.AppendLine("    /// " + table.Name + "_entity." + column.Name + " input error");
                                output.AppendLine("    /// </summary>");
                                output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "args" + System.Convert.ToChar(34) + "></param>");
                                output.AppendLine("    private void ErrorFileInput_" + column.Name + "(UploadErrorEventArgs args)");
                                output.AppendLine("    {");
                                output.AppendLine("         ShowNotification(new NotificationMessage");
                                output.AppendLine("         {");
                                output.AppendLine("             Severity = NotificationSeverity.Error,");
                                output.AppendLine("             Summary = " + System.Convert.ToChar(34) + "Error al procesar archivo." + System.Convert.ToChar(34) + ",");
                                output.AppendLine("             Detail = args.Message,");
                                output.AppendLine("             Duration = 4000");
                                output.AppendLine("         });");
                                output.AppendLine("    }");
                                output.AppendLine(" ");
                            }
                        }
                        foreach (var column in table.Columns)
                        {
                            if (column.IsInForeignKey && !column.IsInPrimaryKey)
                            {
                                var foreignKey = column.ForeignKeys.FirstOrDefault();
                                MyMeta.ITable fkTable = foreignKey.PrimaryTable;

                            }
                            else
                            {
                                switch (column.DataTypeName.ToLower())
                                {
                                    case "tinyint":
                                    case "int":
                                    case "smallint":
                                    case "bigint":
                                        break;
                                    case "decimal":
                                    case "float":
                                        break;
                                    case "datetime":
                                    case "smalldatetime":
                                        break;
                                    case "bit":
                                        output.AppendLine("    /// <summary>");
                                        output.AppendLine("    /// Entity is binded, however it sets anyway.");
                                        output.AppendLine("    /// </summary>");
                                        output.AppendLine("    /// <returns></returns>");
                                        output.AppendLine("    void " + column.Name + "Change(bool? value," + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " item)");
                                        output.AppendLine("    {");
                                        output.AppendLine("         " + table.Name + "_entity = item;");
                                        output.AppendLine("    }");


                                        break;
                                    case "text":
                                    default:
                                        break;
                                }
                            }
                        }

                        if (table.Columns.Where(x => x.Name == "Habilitado").FirstOrDefault() != null)
                        {
                            output.AppendLine("    /// <summary>");
                            output.AppendLine("    /// " + table.Name + "CellRender event");
                            output.AppendLine("    /// </summary>");
                            output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "args" + System.Convert.ToChar(34) + "></param>");
                            output.AppendLine("    private void " + table.Name + "CellRender(DataGridCellRenderEventArgs<" + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "> args)");
                            output.AppendLine("    {");
                            output.AppendLine("         if(!args.Data.Habilitado)");
                            output.AppendLine("             args.Attributes.Add(" + System.Convert.ToChar(34) + "class" + System.Convert.ToChar(34) + ", " + System.Convert.ToChar(34) + "row-highlight-disabled" + System.Convert.ToChar(34) + ");");
                            output.AppendLine("         else");
                            output.AppendLine("             args.Attributes.Add(" + System.Convert.ToChar(34) + "class" + System.Convert.ToChar(34) + ", " + System.Convert.ToChar(34) + "rz-datatable-even" + System.Convert.ToChar(34) + ");");
                            output.AppendLine("    }");
                            output.AppendLine(" ");
                        }

                        output.AppendLine("    #endregion");
                        output.AppendLine(" ");
                        output.AppendLine("    #region Export");
                        output.AppendLine(" ");
                        output.AppendLine("    #endregion");
                        output.AppendLine(" ");
                        output.AppendLine("}");


                        SaveOutputToFile(table.Schema + "\\" + table.Name + ".Crud.razor", output, relativepath, true);
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
            string filePath = _workingdir + _generationFolder.Trim() + "\\" + relativepath + fileName;
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
                if (file.ToLower().EndsWith(".razor"))
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
            get { return "c6884699-f5e7-506c-bc3d-480cd6d425a2"; }
        }
        #region ITemplate Members

        public string OutputLanguage
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    public class RadzenBlazorLeftMenuBuilder : ITemplate
    {
        const string TemplateName = "RadzenBlazor Left Menu Builder";
        const string TemplateDescription = "RadzenBlazor Left Menu Builder";
        const string TemplateoutputLanguage = "C#";
        private ArrayList _arraylist = new ArrayList();
        private string _workingdir = String.Empty;
        private string _languageMappingFileName;
        private string _dbTargetMappingFileName;
        private string _generationFolder = string.Empty;
        private string _treatFieldAsImage = string.Empty;
        private const string generationFolder = "Radzen Blazor Generation folder";
        private const string treatFieldAsImage = "Radzen Blazor Treat Field as Image";

        private ArrayList _refkeyList = new ArrayList();
        public RadzenBlazorLeftMenuBuilder()
        { }
        public RadzenBlazorLeftMenuBuilder(GenerationProject generationProject)
        {
            // Generation Folder
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

                //if (generationProject.DeletePreviousGeneratedFiles)
                //{
                //    DeletePreviousGeneratedFiles(relativepath);
                //}

                string fieldsToTreatAsImage = string.Empty;
                var entry = generationProject.TemplateConfigurationEntries.Find(e => e.Template.Equals(TemplateName) && e.Name.Equals(treatFieldAsImage));
                if (entry != null)
                    fieldsToTreatAsImage = entry.Value;

                string currentTableSchema = String.Empty;
                output = new System.Text.StringBuilder();

                output.AppendLine("  <RadzenPanelMenu style=" + System.Convert.ToChar(34) + "height: 100%" + System.Convert.ToChar(34) + ">");
                output.AppendLine("      <ChildContent>");

                output.AppendLine("      <RadzenPanelMenuItem Icon=" + System.Convert.ToChar(34) + "settings" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + generationProject.Namespace + System.Convert.ToChar(34) + " Visible=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "padding-left:5%;" + System.Convert.ToChar(34) + ">");
                output.AppendLine("      <ChildContent>");

                foreach (MyMeta.ITable table in db.Tables)
                {
                    if (table.Selected)
                    {
                        if (!currentTableSchema.Equals(table.Schema))
                        {
                            currentTableSchema = table.Schema;
                            output.AppendLine("        <RadzenPanelMenuItem Icon=" + System.Convert.ToChar(34) + "settings" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + table.Schema + System.Convert.ToChar(34) + " Visible=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + ">");
                            output.AppendLine("          <ChildContent>");
                            AppendTablesToSchema(ref db, table.Schema, ref output);
                            AppendViewsToSchema(ref db, table.Schema, ref output);
                            output.AppendLine("          </ChildContent>");
                            output.AppendLine("        </RadzenPanelMenuItem>");
                        }
                    }
                }
                output.AppendLine("       </ChildContent>");
                output.AppendLine("      </ChildContent>");
                output.AppendLine("    </RadzenPanelMenu>");
                SaveOutputToFile(@"\RadzenNavMenu.razor", output, relativepath, true);

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

        private void AppendTablesToSchema(ref MyMeta.IDatabase db, string schema, ref System.Text.StringBuilder output)
        {
            foreach (MyMeta.ITable table in db.Tables)
            {
                if (table.Selected && table.Schema.Equals(schema))
                {
                    output.AppendLine("        <RadzenPanelMenuItem Icon=" + System.Convert.ToChar(34) + "home" + System.Convert.ToChar(34) + " Path=" + System.Convert.ToChar(34) + "/" + schema + table.Name + "Crud" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + table.Name + System.Convert.ToChar(34) + " Visible=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("        </RadzenPanelMenuItem>");
                }
            }
        }
        private void AppendViewsToSchema(ref MyMeta.IDatabase db, string schema, ref System.Text.StringBuilder output)
        {
            foreach (MyMeta.IView view in db.Views)
            {
                if (view.Selected && view.Schema.Equals(schema))
                {
                    output.AppendLine("        <RadzenPanelMenuItem Icon=" + System.Convert.ToChar(34) + "home" + System.Convert.ToChar(34) + " Path=" + System.Convert.ToChar(34) + "/" + schema + view.Name + "view" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + view.Name + System.Convert.ToChar(34) + " Visible=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("        </RadzenPanelMenuItem>");
                }
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
            string filePath = _workingdir + _generationFolder.Trim() + "\\" + relativepath + fileName;
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
                if (file.ToLower().EndsWith(".razor"))
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
            get { return "c7984600-f6e8-506c-bc4d-490cd6d535a3"; }
        }
        #region ITemplate Members

        public string OutputLanguage
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }


    public class RadzenBlazorViewReportBuilder : ITemplate
    {
        const string TemplateName = "RadzenBlazor View ReportBuilder";
        const string TemplateDescription = "RadzenBlazor View ReportBuilder";
        const string TemplateoutputLanguage = "C#";
        private ArrayList _arraylist = new ArrayList();
        private string _workingdir = String.Empty;
        private string _languageMappingFileName;
        private string _dbTargetMappingFileName;
        private string _generationFolder = string.Empty;
        private string _treatFieldAsImage = string.Empty;
        private const string generationFolder = "Radzen Blazor Generation folder";
        private const string treatFieldAsImage = "Radzen Blazor Treat Field as Image";

        private ArrayList _refkeyList = new ArrayList();
        public RadzenBlazorViewReportBuilder()
        { }
        public RadzenBlazorViewReportBuilder(GenerationProject generationProject)
        {
            // Generation Folder
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

                string fieldsToTreatAsImage = string.Empty;
                var entry = generationProject.TemplateConfigurationEntries.Find(e => e.Template.Equals(TemplateName) && e.Name.Equals(treatFieldAsImage));
                if (entry != null)
                    fieldsToTreatAsImage = entry.Value;

                foreach (MyMeta.IView view in db.Views)
                {
                    output = new System.Text.StringBuilder();
                    if (view.Selected)
                    {
                        output.AppendLine("@page " + System.Convert.ToChar(34) + "/" + view.Schema + view.Name + "View" + System.Convert.ToChar(34) + "");
                        output.AppendLine("@using  Permaquim." + generationProject.Namespace + ".Web.Administration.Controllers;");
                        output.AppendLine(" ");
                        output.AppendLine("@inject NotificationService NotificationService");
                        output.AppendLine("@inject Blazored.SessionStorage.ISessionStorageService sessionStorage");
                        output.AppendLine("@inject NavigationManager NavManager");
                        output.AppendLine(" ");
                        output.AppendLine("@if (" + view.Name + "_entities != null)");
                        output.AppendLine("{");
                        output.AppendLine("            <RadzenBadge BadgeStyle=" + System.Convert.ToChar(34) + "BadgeStyle.Secondary" + System.Convert.ToChar(34) + " >");
                        output.AppendLine("                <ChildContent>");
                        output.AppendLine("                    <div>");
                        output.AppendLine("                        <h4 style=" + System.Convert.ToChar(34) + "color:white;" + System.Convert.ToChar(34) + ">" + view.Name + "</h4>");
                        output.AppendLine("                    </div>");
                        output.AppendLine("                </ChildContent>");
                        output.AppendLine("            </RadzenBadge>");
                        output.AppendLine(" <hr>");
                        output.AppendLine("        <RadzenDataGrid AllowColumnResize=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " AllowSorting=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " PageSize=" + System.Convert.ToChar(34) + "10" + System.Convert.ToChar(34) + " AllowPaging=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " PagerHorizontalAlign=" + System.Convert.ToChar(34) + "HorizontalAlign.Left" + System.Convert.ToChar(34) + " ShowPagingSummary=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + "");
                        output.AppendLine("        Data=" + System.Convert.ToChar(34) + "@" + view.Name + "_entities" + System.Convert.ToChar(34) + " TItem=" + System.Convert.ToChar(34) + generationProject.Namespace + ".Entities.Views." + view.Schema + "." + view.Name + "" + System.Convert.ToChar(34) + " ColumnWidth=" + System.Convert.ToChar(34) + "300px" + System.Convert.ToChar(34) + " LogicalFilterOperator=" + System.Convert.ToChar(34) + "LogicalFilterOperator.Or" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("       <EmptyTemplate>");
                        output.AppendLine("            <p style=" + System.Convert.ToChar(34) + "color: lightgrey; font-size: 24px; text-align: center; margin: 2rem;" + System.Convert.ToChar(34) + ">No existen registros.</p>");
                        output.AppendLine("        </EmptyTemplate>        ");
                        output.AppendLine("        <Columns>");
                        foreach (var column in view.Columns)
                        {
                            output.AppendLine("            <RadzenDataGridColumn TItem=" + System.Convert.ToChar(34) + generationProject.Namespace + ".Entities.Views." + view.Schema + "." + view.Name + System.Convert.ToChar(34) + " Property=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Title=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " Frozen=" + System.Convert.ToChar(34) + "false" + System.Convert.ToChar(34) + " Sortable=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " Width=" + System.Convert.ToChar(34) + "60px" + System.Convert.ToChar(34) + " >");
                            output.AppendLine("            </RadzenDataGridColumn>");

                        }
                        output.AppendLine("        </Columns>");
                        output.AppendLine("        </RadzenDataGrid>");
                        output.AppendLine("    }");
                        output.AppendLine(" <hr>");
                        output.AppendLine(" ");
                        output.AppendLine("    <RadzenNotification /> ");
                        output.AppendLine(" ");
                        output.AppendLine("@code {");

                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Variables to handle main entity");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private " + generationProject.Namespace + ".Entities.Views." + view.Schema + "." + view.Name + " " + view.Name + "_entity = new();");
                        output.AppendLine("    private List<" + generationProject.Namespace + ".Entities.Views." + view.Schema + "." + view.Name + ">? " + view.Name + "_entities;");
                        output.AppendLine(" ");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Variables to store multilanguaje");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private List<Entities.TextoLenguaje> dataLenguaje;");
                        output.AppendLine(" ");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Variables to store user data");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private " + generationProject.Namespace + ".Entities.Tables.Seguridad.Usuario? dataUsuario = null;");
                        output.AppendLine(" ");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Variables to store style data");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private List<" + generationProject.Namespace + ".Entities.Tables.Estilo.EsquemaDetalle> dataEsquemaDetalle = new();");
                        output.AppendLine(" ");
                        output.AppendLine("    protected override async Task OnInitializedAsync()");
                        output.AppendLine("    {");
                        output.AppendLine("        dataLenguaje = await sessionStorage.GetItemAsync<List<Entities.TextoLenguaje>>(" + System.Convert.ToChar(34) + "DataLenguaje" + System.Convert.ToChar(34) + ");");
                        output.AppendLine("        dataUsuario = await sessionStorage.GetItemAsync<" + generationProject.Namespace + ".Entities.Tables.Seguridad.Usuario?>(" + System.Convert.ToChar(34) + "DataUsuario" + System.Convert.ToChar(34) + ");");
                        output.AppendLine("        if (dataUsuario == null)");
                        output.AppendLine("             NavManager.NavigateTo(" + System.Convert.ToChar(34) + "login" + System.Convert.ToChar(34) + ", true);");
                        output.AppendLine("        dataEsquemaDetalle = await sessionStorage.GetItemAsync<List<" + generationProject.Namespace + ".Entities.Tables.Estilo.EsquemaDetalle>>(" + System.Convert.ToChar(34) + "DataEsquemaDetalle" + System.Convert.ToChar(34) + ");");
                        output.AppendLine("        await Task.Run(LoadMainEntityData);");
                        output.AppendLine("    }");
                        output.AppendLine("    #region Loading entities data");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Loads main entity data");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void LoadMainEntityData()");
                        output.AppendLine("    {");
                        output.AppendLine("        " + generationProject.Namespace + ".Business.Views." + view.Schema + "." + view.Name + " entity = new();");
                        output.AppendLine("        " + view.Name + "_entities = entity.Items();");
                        output.AppendLine("    }");
                        output.AppendLine("    #endregion");

                        output.AppendLine("}");


                        SaveOutputToFile(view.Schema + "\\" + view.Name + ".View.razor", output, relativepath, true);
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
            string filePath = _workingdir + _generationFolder.Trim() + "\\" + relativepath + fileName;
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
                if (file.ToLower().EndsWith(".razor"))
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
            get { return "c6885891-a6g8-906c-bc3d-123cd6d452a3"; }
        }
        #region ITemplate Members

        public string OutputLanguage
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
