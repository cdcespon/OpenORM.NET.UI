using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

public class SyncfusionJsControlsBuilder : IPlugin
{
    private List<ITemplate> _templates = new List<ITemplate>();
    const string PluginName = "Syncfusion Js Crud Builder";
    const string PluginDescription = "Syncfusion Js Crud Builder";
    public SyncfusionJsControlsBuilder()
    {
        _templates.Add(new SyncfusionJsCrudBuilder());
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

    public class SyncfusionJsCrudBuilder : ITemplate
    {
        const string TemplateName = "Syncfusion Js CrudBuilder";
        const string TemplateDescription = "Syncfusion Js Crud Builder";
        const string TemplateoutputLanguage = "C#";
        private ArrayList _arraylist = new ArrayList();
        private string _workingdir = String.Empty;
        private string _languageMappingFileName;
        private string _dbTargetMappingFileName;
        private string _resourceUrl = string.Empty;
        private const string resourceUrl = "Resource Url";

        private ArrayList _refkeyList = new ArrayList();
        public SyncfusionJsCrudBuilder()
        { }
        public SyncfusionJsCrudBuilder(GenerationProject generationProject)
        {

            TemplateConfigurationEntry resourceUrlEntry = generationProject.TemplateConfigurationEntries.Find(e => e.Template.Equals(TemplateName) && e.Name.Equals(resourceUrl));

            if (resourceUrlEntry == null)
                generationProject.TemplateConfigurationEntries.Add(new TemplateConfigurationEntry(TemplateName, resourceUrl, _resourceUrl));
            else
                _resourceUrl = resourceUrlEntry.Value;
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
        public bool Execute(MyMeta.IDatabase db, string workingDir, GenerationProject generationProject)
        {
            try
            {
                _workingdir = workingDir;

                MyMeta.IDatabase _dataBase = db;

                System.Text.StringBuilder output = null;

                db.Root.DbTarget = GetDbTarget(db.Root.Driver);
                db.Root.Language = GetLanguage(db.Root.Driver);//"C# Types";


                TemplateConfigurationEntry resourceUrlEntry = generationProject.TemplateConfigurationEntries.Find(e => e.Template.Equals(TemplateName) && e.Name.Equals(resourceUrl));

                if (resourceUrlEntry != null)
                    _resourceUrl = resourceUrlEntry.Value;


                foreach (MyMeta.ITable refTable in db.Tables)
                {
                    output = new System.Text.StringBuilder();
                    if (refTable.Selected)
                    {
                        output.AppendLine("<%@ Page Language=" + System.Convert.ToChar(34) + "C#" + System.Convert.ToChar(34) + " ValidateRequest=" + System.Convert.ToChar(34) + "false" + System.Convert.ToChar(34) + " %>");
                        output.AppendLine("<!DOCTYPE html>");
                        output.AppendLine("<html xmlns=" + System.Convert.ToChar(34) + "http://www.w3.org/1999/xhtml" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("<head>");
                        output.AppendLine("    <title>ABM de " + refTable.Name + "</title>");
                        output.AppendLine("    <meta name=" + System.Convert.ToChar(34) + "viewport" + System.Convert.ToChar(34) + " content=" + System.Convert.ToChar(34) + "width=device-width, initial-scale=1.0" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <meta charset=" + System.Convert.ToChar(34) + "utf-8" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <link href=" + System.Convert.ToChar(34) + _resourceUrl + "/bootstrap/css/bootstrap.min.css" + System.Convert.ToChar(34) + " rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <link href=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/content/ejthemes/default-theme/ej.widgets.all.min.css" + System.Convert.ToChar(34) + " rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <link href=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/content/default.css" + System.Convert.ToChar(34) + " rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <link href=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/content/default-responsive.css" + System.Convert.ToChar(34) + " rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <link href=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/content/ejthemes/responsive-css/ej.responsive.css" + System.Convert.ToChar(34) + " rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <link href=" + System.Convert.ToChar(34) + "../../../../Content/Site.css" + System.Convert.ToChar(34) + " rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " />");

                        output.AppendLine("</head>");
                        output.AppendLine("<body>");
                        output.AppendLine("    <div id=" + System.Convert.ToChar(34) + "loading" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("        <img id=" + System.Convert.ToChar(34) + "loading-image" + System.Convert.ToChar(34) + " src=" + System.Convert.ToChar(34) + _resourceUrl + "/imagenes/gif/loadingGears.gif" + System.Convert.ToChar(34) + " alt=" + System.Convert.ToChar(34) + "Cargando..." + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    </div>");

                        output.AppendLine("");
                        output.AppendLine("    <div class=" + System.Convert.ToChar(34) + "content-container-fluid" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("        <div class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("            <div class=" + System.Convert.ToChar(34) + "cols-sample-area" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("");
                        output.AppendLine("                <script id=" + System.Convert.ToChar(34) + "menuselect" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "text/x-jsrender" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                    <table>");
                        output.AppendLine("                        <tr>");
                        output.AppendLine("                            <td> Tipo de filtro </td><td>:</td>");
                        output.AppendLine("                            <td>");
                        output.AppendLine("");
                        output.AppendLine("                                <select id=" + System.Convert.ToChar(34) + "filtermenu" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "e-ddl" + System.Convert.ToChar(34) + " data-bind=" + System.Convert.ToChar(34) + "value: field" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                    <option value=" + System.Convert.ToChar(34) + "excel" + System.Convert.ToChar(34) + " selected=" + System.Convert.ToChar(34) + "selected" + System.Convert.ToChar(34) + ">Excel</option>");
                        output.AppendLine("                                    <option value=" + System.Convert.ToChar(34) + "menu" + System.Convert.ToChar(34) + ">Menu</option>");
                        output.AppendLine("                                </select>");
                        output.AppendLine("                            </td>");
                        output.AppendLine("                        </tr>");
                        output.AppendLine("                    </table>");
                        output.AppendLine("                </script>");
                        output.AppendLine("");


                        output.AppendLine("                <div id=" + System.Convert.ToChar(34) + "Grid" + System.Convert.ToChar(34) + "></div>");
                        output.AppendLine("                <div id=" + System.Convert.ToChar(34) + "template" + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "display: none" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("");

                        /////////////////////// TABS /////////////////
                        output.AppendLine("                    <div id=" + System.Convert.ToChar(34) + "tabEdit" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                        <ul>");
                        string tableDesc = refTable.Description.Equals(String.Empty) ? refTable.Name : refTable.Description;
                        output.AppendLine("                            <li><a href=" + System.Convert.ToChar(34) + "#editPanel" + System.Convert.ToChar(34) + ">Detalle de " + tableDesc + "</a></li>");

                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {
                            if (column.IsInForeignKey) //&& !column.IsInPrimaryKey)
                                foreach (MyMeta.IForeignKey fk in column.ForeignKeys)
                                {
                                    if (!refTable.Name.Equals(fk.ForeignTable.Name))
                                    {
                                        output.AppendLine("                            <li><a href=" + System.Convert.ToChar(34) + "#panel" + fk.ForeignTable.Name + System.Convert.ToChar(34) + "> " + fk.ForeignTable.Name + " </a></li>");
                                    }
                                }
                        }


                        //output.AppendLine("                            <li><a href=" + System.Convert.ToChar(34) + "#localidadPanel" + System.Convert.ToChar(34) + ">Localidades </a></li>");
                        output.AppendLine("");
                        output.AppendLine("                        </ul>");
                        output.AppendLine("");
                        output.AppendLine("                        <div id=" + System.Convert.ToChar(34) + "editPanel" + System.Convert.ToChar(34) + ">");


                        /////////////////////// END TABS /////////////////
                        output.AppendLine("                    <b>" + refTable.Name + "</b>");
                        output.AppendLine("                    <table id=" + System.Convert.ToChar(34) + "templatetable" + System.Convert.ToChar(34) + ">");

                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {

                            string fieldDesc = column.Description.Equals(String.Empty) ? column.Name : column.Description;
                            switch (column.LanguageType)
                            {
                                //case "String":
                                case "Date":
                                case "SmallDateTime":
                                case "DateTime":
                                    output.AppendLine("                        <tr>");
                                    output.AppendLine("                            <td style=" + System.Convert.ToChar(34) + "text-align: left;" + System.Convert.ToChar(34) + ">");
                                    output.AppendLine("                                " + fieldDesc);
                                    output.AppendLine("                            </td>");
                                    output.AppendLine("                            <td style=" + System.Convert.ToChar(34) + "text-align: left" + System.Convert.ToChar(34) + ">");
                                    output.AppendLine("                                <input id=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " value=" + System.Convert.ToChar(34) + "{{: " + column.Name + "}}" + System.Convert.ToChar(34) + " />");
                                    output.AppendLine("                                ");
                                    output.AppendLine("                            </td>");
                                    output.AppendLine("                        </tr>");

                                    break;
                                case "Boolean":
                                case "Bool":
                                    output.AppendLine("                        <tr>");
                                    output.AppendLine("                            <td style=" + System.Convert.ToChar(34) + "text-align: left;" + System.Convert.ToChar(34) + ">");
                                    output.AppendLine("                                " + fieldDesc);
                                    output.AppendLine("                            </td>");
                                    output.AppendLine("                            <td style=" + System.Convert.ToChar(34) + "text-align: left" + System.Convert.ToChar(34) + ">");
                                    output.AppendLine("                                <input id=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " value=" + System.Convert.ToChar(34) + "{{: " + column.Name + "}}" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "checkbox" + System.Convert.ToChar(34) + " />");
                                    output.AppendLine("                            </td>");
                                    output.AppendLine("                        </tr>");
                                    break;
                                default:
                                    if (column.IsInPrimaryKey && !column.IsAutoKey) // Editable only in Add
                                    {
                                        output.AppendLine("                        <tr>");
                                        output.AppendLine("                            <td style=" + System.Convert.ToChar(34) + "text-align: left;" + System.Convert.ToChar(34) + ">");
                                        output.AppendLine("                                " + fieldDesc);
                                        output.AppendLine("                            </td>");
                                        output.AppendLine("                            <td style=" + System.Convert.ToChar(34) + "text-align: left" + System.Convert.ToChar(34) + ">");
                                        output.AppendLine("                                <input id=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " value=" + System.Convert.ToChar(34) + "{{: " + column.Name + "}}" + System.Convert.ToChar(34) + " disabled=" + System.Convert.ToChar(34) + "disabled" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "e-field e-ejinputtext valid  e-disable" + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "text-align: right; width: 100%; height: 28px" + System.Convert.ToChar(34) + " />");
                                        output.AppendLine("                            </td>");
                                        output.AppendLine("                        </tr>");
                                    }
                                    else if (column.IsAutoKey) // Never editable 
                                    {
                                        output.AppendLine("                        <tr>");
                                        output.AppendLine("                            <td style=" + System.Convert.ToChar(34) + "text-align: left;" + System.Convert.ToChar(34) + ">");
                                        output.AppendLine("                                " + fieldDesc);
                                        output.AppendLine("                            </td>");
                                        output.AppendLine("                            <td style=" + System.Convert.ToChar(34) + "text-align: left" + System.Convert.ToChar(34) + ">");
                                        output.AppendLine("                                <input id=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " value=" + System.Convert.ToChar(34) + "{{: " + column.Name + "}}" + System.Convert.ToChar(34) + " disabled=" + System.Convert.ToChar(34) + "disabled" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "e-field e-ejinputtext valid  e-disable" + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "text-align: right; width: 100%; height: 28px" + System.Convert.ToChar(34) + " readonly = " + System.Convert.ToChar(34) + "readonly" + System.Convert.ToChar(34) + " />");
                                        output.AppendLine("                            </td>");
                                        output.AppendLine("                        </tr>");

                                    }
                                    else // Always editable
                                    {
                                        output.AppendLine("                        <tr>");
                                        output.AppendLine("                            <td style=" + System.Convert.ToChar(34) + "text-align: left;" + System.Convert.ToChar(34) + ">");
                                        output.AppendLine("                                " + fieldDesc);
                                        output.AppendLine("                            </td>");
                                        output.AppendLine("                            <td style=" + System.Convert.ToChar(34) + "text-align: left" + System.Convert.ToChar(34) + ">");
                                        output.AppendLine("                                <input id=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " name=" + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + " value=" + System.Convert.ToChar(34) + "{{: " + column.Name + "}}" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "e-field e-ejinputtext valid " + System.Convert.ToChar(34) + " style=" + System.Convert.ToChar(34) + "text-align: right; width: 100%; height: 28px" + System.Convert.ToChar(34) + " />");
                                        output.AppendLine("                            </td>");
                                        output.AppendLine("                        </tr>");

                                    }
                                    break;
                            }


                        }

                        output.AppendLine("                    </table>");
                        /////////////////////// TABS /////////////////

                        output.AppendLine("                      </div>");
                        //output.AppendLine("                        <div id=" + System.Convert.ToChar(34) + "localidadPanel" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                            <iframe id=" + System.Convert.ToChar(34) + "iframeLocalidad" + System.Convert.ToChar(34) + " frameborder=" + System.Convert.ToChar(34) + "0" + System.Convert.ToChar(34) + "></iframe>");

                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {
                            if (column.IsInForeignKey) //&& !column.IsInPrimaryKey)
                                foreach (MyMeta.IForeignKey fk in column.ForeignKeys)
                                {
                                    if (!refTable.Name.Equals(fk.ForeignTable.Name))
                                    {
                                        //output.AppendLine("                            <li><a href=" + System.Convert.ToChar(34) + "#iframe" + fk.ForeignTable.Name + System.Convert.ToChar(34) + "> " + fk.ForeignTable.Name + " </a></li>");
                                        output.AppendLine("                        <div id=" + System.Convert.ToChar(34) + "panel" + fk.ForeignTable.Name + System.Convert.ToChar(34) + ">");
                                        output.AppendLine("                            <iframe id=" + System.Convert.ToChar(34) + "iframe" + fk.ForeignTable.Name + System.Convert.ToChar(34) + " frameborder=" + System.Convert.ToChar(34) + "0" + System.Convert.ToChar(34) + "></iframe>");
                                        output.AppendLine("                        </div>");


                                    }
                                }
                        }                        

                        output.AppendLine("                        </div>");
                        output.AppendLine("                        </div>");
                        output.AppendLine("                    </div>");
                        
                        
                        /////////////////////// END TABS /////////////////
                        output.AppendLine("             </div>");
                        output.AppendLine("          </div>");

                        output.AppendLine("    <!--[if lt IE 9]>");
                        output.AppendLine("     <script type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + " src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/jquery-1.11.3.min.js" + System.Convert.ToChar(34) + " ></script>");
                        output.AppendLine("    <![endif]-->");
                        output.AppendLine("    <!--[if gte IE 9]><!-->");
                        output.AppendLine("    <script type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + " src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/jquery-2.1.4.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <!--<![endif]-->");
                        output.AppendLine("    <script src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/jquery.easing.1.3.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <script src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/ej.web.all.min.js" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <script src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/jsrender.min.js" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <script src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/properties.js" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <script src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/cultures/ej.culture.es-AR.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("");
                        output.AppendLine("<script src=" + System.Convert.ToChar(34) + "<%= ResolveUrl(" + System.Convert.ToChar(34) + "~/config.js" + System.Convert.ToChar(34) + ") %>" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("<script src=" + System.Convert.ToChar(34) + "<%= ResolveUrl(" + System.Convert.ToChar(34) + "~/Scripts/Site.js" + System.Convert.ToChar(34) + ") %>" + System.Convert.ToChar(34) + "></script>");
                        //output.AppendLine("<script src=" + System.Convert.ToChar(34) + "<%= ResolveUrl(" + System.Convert.ToChar(34) + "~/Scripts/validateExtension.js" + System.Convert.ToChar(34) + ") %>" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("");
                        output.AppendLine("    <script type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("");


                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {
                            if (column.IsInForeignKey)// && !column.IsInPrimaryKey)
                                foreach (MyMeta.IForeignKey fk in column.ForeignKeys)
                                {
                                    if (!refTable.Name.Equals(fk.PrimaryTable.Name))
                                    {
                                        output.AppendLine("        var fk" + fk.PrimaryTable.Name + "Data;");
                                    }
                                }
                        }

                        output.AppendLine("");
                        string varinit = string.Empty;
                        string varassign = string.Empty;
                        string pageparams = string.Empty;
                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("            var " + column.Name.ToLower() + "_ ;");
                                varinit += "            " + column.Name.ToLower() + "_ = getURLParameter('" + column.Name + "');" + Environment.NewLine;
                                pageparams += column.Name + "=" + System.Convert.ToChar(34) + " + " + column.Name.ToLower() + "_&";
                                varassign += column.Name.ToLower() + "_ = args.rowData." + column.Name + ";" + Environment.NewLine;

                            }
                        }


                        string varinitFk = string.Empty;
                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {
                            if (column.IsInForeignKey && !column.IsInPrimaryKey)
                                foreach (MyMeta.IForeignKey fk in column.ForeignKeys)
                                {
                                    //if (!refTable.Name.Equals(fk.PrimaryTable.Name))
                                    //{
                                    output.AppendLine("        var fk" + column.Name + "_ ;");
                                    varinitFk += "            fk" + column.Name + "_ = getURLParameter('" + column.Name + "');" + Environment.NewLine;
                                    //}
                                }
                        }

                        output.AppendLine(varinitFk);
                        output.AppendLine("");
                        output.AppendLine("        $(function () {");
                        output.AppendLine("");
                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {
                            if (column.IsInForeignKey)// && !column.IsInPrimaryKey)
                                foreach (MyMeta.IForeignKey fk in column.ForeignKeys)
                                {
                                    if (!refTable.Name.Equals(fk.PrimaryTable.Name))
                                    {
                                        output.AppendLine("            load" + fk.PrimaryTable.Name + "FkData();");
                                    }
                                }
                        }
                        output.AppendLine("            loadMainData();");
                        output.AppendLine("");
                        output.AppendLine("        });");
                        /////////////////////// TABS /////////////////

                        output.AppendLine("     function onClientBeforeActive(e) {");
                        output.AppendLine("            cargarIFrame(e.activeIndex);");
                        output.AppendLine("        }");
                        output.AppendLine("        function cargarIFrame(indice) {");
                        output.AppendLine("            var height = $(window).height();");
                        output.AppendLine("            switch (indice) {");

                        string relativepath = db.Name.ToLower() + "/" + refTable.Schema + "/";



                        int index = 1;
                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {
                            if (column.IsInForeignKey) //&& !column.IsInPrimaryKey)
                                foreach (MyMeta.IForeignKey fk in column.ForeignKeys)
                                {
                                    if (!refTable.Name.Equals(fk.ForeignTable.Name))
                                    {
                                        output.AppendLine("                case " + index.ToString() + ":");

                                        string prms = string.Empty;
                                        if (pageparams.Length > 0)
                                            prms = pageparams.Substring(0, pageparams.Length - 1);

                                        string relativepath2 = "/pages/general/general/";

                                        output.AppendLine("                    $('#iframe" + fk.ForeignTable.Name + "').attr(" + System.Convert.ToChar(34) + "src" + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + relativepath2 + db.Name + "." + refTable.Schema + "." + fk.ForeignTable.Name + ".aspx?" + prms + ");");
                                        output.AppendLine("                    $('#iframe" + fk.ForeignTable.Name + "').css('height', height * 0.75 | 0);");
                                        output.AppendLine("                    break;");
                                        index += 1;
                                    }
                                }
                        }

                        output.AppendLine("                default:");
                        output.AppendLine("                    break;");
                        output.AppendLine("            }");
                        output.AppendLine("        }");


                        /////////////////////// END TABS /////////////////

                        output.AppendLine("function getURLParameter(name) {");
                        output.AppendLine(@"  return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [null, ''])[1].replace(/\+/g, '%20')) || null;");
                        output.AppendLine("}");


                        output.AppendLine("        function loadMainData() {");


                        output.AppendLine("            var dataparameters = null; ");

                        String parameters = String.Empty;
                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {
                            if (column.IsInForeignKey)// && !column.IsInPrimaryKey)
                                foreach (MyMeta.IForeignKey fk in column.ForeignKeys)
                                {
                                    if (!refTable.Name.Equals(fk.PrimaryTable.Name))
                                    {
                                        parameters += "fk" + column.Name + "_ != null && ";
                                    }
                                }
                        }


                        if (parameters.Length > 0)
                        {

                            //output.AppendLine("            dataparameters = { sessionId: localStorage.getItem(" + System.Convert.ToChar(34) + "session_id" + System.Convert.ToChar(34) + "),");
                            foreach (MyMeta.IColumn column in refTable.Columns)
                            {
                                if (column.IsInForeignKey)// && !column.IsInPrimaryKey)
                                    foreach (MyMeta.IForeignKey fk in column.ForeignKeys)
                                    {
                                        if (!refTable.Name.Equals(fk.PrimaryTable.Name))
                                        {
                                            //output.AppendLine("            " + column.Name + " : fk" + column.Name + "_  != null ? fk" + column.Name + "_ : undefined,");

                                        }
                                    }
                            }
                            //output.AppendLine("                 };");

                        }

   
                        output.AppendLine("            $.ajax({");
                        output.AppendLine("                url: getwebservicesurl() + " + System.Convert.ToChar(34) + "/" + relativepath + db.Name.ToLower() + "." + refTable.Schema + "." + refTable.Name.ToLower() + ".asmx/items" + System.Convert.ToChar(34) + ",");
                        output.AppendLine("                data: { sessionId: localStorage.getItem(" + System.Convert.ToChar(34) + "session_id" + System.Convert.ToChar(34) + ")},");
                        output.AppendLine("                type: 'POST',");
                        output.AppendLine("                dataType: 'json',");
                        output.AppendLine("                async: true,");
                        output.AppendLine("                crossdomain: true,");
                        output.AppendLine("                success: function (data) {");
                        output.AppendLine("                    switch (data.NumeroInformacion) {");
                        output.AppendLine("                        case 0:");
                        output.AppendLine("                            var resultado = JSON.parse(data.Dato);");
                        output.AppendLine("                            loadGrid(resultado);");
                        output.AppendLine("                            break;");
                        output.AppendLine("                        case 1001:");
                        output.AppendLine("                            raiseEventMessageToParent('SessionError', data.Dato);");
                        output.AppendLine("                            break;");
                        output.AppendLine("                        default:");
                        output.AppendLine("                            raiseEventMessageToParent('Error', data.Dato);");
                        output.AppendLine("                            break;");
                        output.AppendLine("                    }");
                        output.AppendLine("                },");
                        output.AppendLine("                error: function (result) {");
                        output.AppendLine("                    raiseEventMessageToParent('Error', 'ERROR: ' + JSON.stringify(result));");
                        output.AppendLine("                }");
                        output.AppendLine("            });");
                        output.AppendLine("         ocultarImagenEspera();");
                        output.AppendLine("");
                        output.AppendLine("        }");
                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {
                            if (column.IsInForeignKey)
                            {
                                foreach (MyMeta.IForeignKey fk in column.ForeignKeys)
                                {

                                    if (!refTable.Name.Equals(fk.PrimaryTable.Name))
                                    {
                                        output.AppendLine("        function load" + fk.PrimaryTable.Name + "FkData() {");
                                        output.AppendLine("            // Carga los datos del servicio web");
                                        relativepath = db.Name.ToLower() + "/" + fk.PrimaryTable.Schema + "/";
                                        output.AppendLine("            $.ajax({");
                                        output.AppendLine("                url: getwebservicesurl() + " + System.Convert.ToChar(34) + "/" + relativepath + db.Name.ToLower() + "." + fk.PrimaryTable.Schema + "." + fk.PrimaryTable.Name.ToLower() + ".asmx/items" + System.Convert.ToChar(34) + ",");
                                        output.AppendLine("                data: { sessionId: localStorage.getItem(" + System.Convert.ToChar(34) + "session_id" + System.Convert.ToChar(34) + ")},");
                                        output.AppendLine("                type: 'POST',");
                                        output.AppendLine("                dataType: 'json',");
                                        output.AppendLine("                async: false,");
                                        output.AppendLine("                crossdomain: true,");
                                        output.AppendLine("                success: function (data) {");
                                        output.AppendLine("                    switch (data.NumeroInformacion) {");
                                        output.AppendLine("                        case 0:");
                                        output.AppendLine("                            var resultado = JSON.parse(data.Dato);");
                                        output.AppendLine("                             fk" + fk.PrimaryTable.Name + "Data = resultado;");
                                        output.AppendLine("                            break;");
                                        output.AppendLine("                        case 1001:");
                                        output.AppendLine("                            raiseEventMessageToParent('SessionError', data.Dato);");
                                        output.AppendLine("                            break;");
                                        output.AppendLine("                        default:");
                                        output.AppendLine("                            raiseEventMessageToParent('Error', data.Dato);");
                                        output.AppendLine("                            break;");
                                        output.AppendLine("                    }");


                                        output.AppendLine("                },");
                                        output.AppendLine("                error: function (result) {");
                                        //output.AppendLine("                    alert('error ' + result.toString());");
                                        output.AppendLine("                    raiseEventMessageToParent('Error', 'ERROR: ' + JSON.stringify(result));");
                                        output.AppendLine("                }");
                                        output.AppendLine("            });");
                                        output.AppendLine("");
                                        output.AppendLine("        }");
                                    }
                                }
                            }
                        }
                        output.AppendLine("        function complete(args) {");
                        output.AppendLine("");
                        output.AppendLine("            if (args.action == 'edit')");
                        output.AppendLine("                args.action = 'update';");
                        output.AppendLine("");
                        output.AppendLine("            switch (args.requestType) {");
                        output.AppendLine("                case " + System.Convert.ToChar(34) + "beginedit" + System.Convert.ToChar(34) + ":");
                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#templatetable" + System.Convert.ToChar(34) + ").attr('style', " + System.Convert.ToChar(34) + "width:100%" + System.Convert.ToChar(34) + ");");
                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {

                            if (column.IsInForeignKey) //&& !column.IsInPrimaryKey)
                            {
                                foreach (MyMeta.IForeignKey fk in column.ForeignKeys)
                                {
                                    if (!refTable.Name.Equals(fk.PrimaryTable.Name))
                                    {
                                        string name = FindFirstVarcharColumnName(fk.PrimaryTable);
                                        string value = FindKeyColumnName(fk.PrimaryTable);

                                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejDropDownList({ dataSource: fk" + fk.PrimaryTable.Name + "Data, width: '100%', fields: { text: " + System.Convert.ToChar(34) + name + System.Convert.ToChar(34) + ", value: " + System.Convert.ToChar(34) + value + System.Convert.ToChar(34) + " } });");
                                        //output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejDropDownList(" + System.Convert.ToChar(34) + "setSelectedValue" + System.Convert.ToChar(34) + ", args.model.currentViewData[args.rowIndex]." + column.Name + ");");
                                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejDropDownList(" + System.Convert.ToChar(34) + "setSelectedValue" + System.Convert.ToChar(34) + ", $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").val());");
                                    }
                                }
                            }
                            else
                            {
                                switch (column.LanguageType)
                                {
                                    case "String":
                                        break;
                                    case "Date":
                                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejDatePicker({ width: '100%', locale: " + System.Convert.ToChar(34) + "es-AR" + System.Convert.ToChar(34) + "});");
                                        break;
                                    case "SmallDateTime":
                                    case "DateTime":
                                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejDateTimePicker({ width: '100%', locale: " + System.Convert.ToChar(34) + "es-AR" + System.Convert.ToChar(34) + ", buttonText: { today: " + System.Convert.ToChar(34) + "Hoy" + System.Convert.ToChar(34) + ", timeNow: " + System.Convert.ToChar(34) + "Ahora" + System.Convert.ToChar(34) + ", done: " + System.Convert.ToChar(34) + "Aceptar" + System.Convert.ToChar(34) + ", timeTitle: " + System.Convert.ToChar(34) + "Hora" + System.Convert.ToChar(34) + " } });");
                                        break;
                                    case "Boolean":
                                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejCheckBox({ checked: args.model.currentViewData[args.rowIndex]." + column.Name + " });");
                                        break;
                                    case "Decimal":
                                    case "Float":
                                    case "Double":
                                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejNumericTextbox({ width: " + System.Convert.ToChar(34) + "100%" + System.Convert.ToChar(34) + ", height: " + System.Convert.ToChar(34) + "34px" + System.Convert.ToChar(34) + ", decimalPlaces: 2 });");
                                        break;
                                    case "Int16":
                                    case "Int32":
                                    case "Int64":
                                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejNumericTextbox({ width: " + System.Convert.ToChar(34) + "100%" + System.Convert.ToChar(34) + ", height: " + System.Convert.ToChar(34) + "34px" + System.Convert.ToChar(34) + ", decimalPlaces: 0 });");
                                        break;
                                }
                            }
                        }
                        output.AppendLine("");

                        /////////////////////////////////////////////////

                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#tabEdit" + System.Convert.ToChar(34) + ").ejTab({");
                        output.AppendLine("                        beforeActive: " + System.Convert.ToChar(34) + "onClientBeforeActive" + System.Convert.ToChar(34) + ",");
                        output.AppendLine("                        heightAdjustMode: ej.Tab.HeightAdjustMode.Content,");
                        output.AppendLine("                        headerPosition: " + System.Convert.ToChar(34) + "top" + System.Convert.ToChar(34) + ",");
                        output.AppendLine("                        showReloadIcon: true");
                        output.AppendLine("                    });");


                        /////////////////////////////////////////////////


                        output.AppendLine("                    " + varassign);


                        output.AppendLine("                    break;");
                        output.AppendLine("                case " + System.Convert.ToChar(34) + "add" + System.Convert.ToChar(34) + ":");
                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {

                            if (column.IsInForeignKey)// && !column.IsInPrimaryKey)
                            {
                                foreach (MyMeta.IForeignKey fk in column.ForeignKeys)
                                {
                                    if (!refTable.Name.Equals(fk.PrimaryTable.Name))
                                    {
                                        string name = FindFirstVarcharColumnName(fk.PrimaryTable);
                                        string value = FindKeyColumnName(fk.PrimaryTable);
                                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejDropDownList({ dataSource: fk" + fk.PrimaryTable.Name + "Data, width: '100%', fields: { text: " + System.Convert.ToChar(34) + name + System.Convert.ToChar(34) + ", value: " + System.Convert.ToChar(34) + value + System.Convert.ToChar(34) + " } });");
                                    }
                                }
                            }
                            else
                            {
                                switch (column.LanguageType)
                                {
                                    case "String":
                                        break;
                                    case "Date":
                                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejDatePicker({ width: '100%', locale: " + System.Convert.ToChar(34) + "es-AR" + System.Convert.ToChar(34) + "});");
                                        break;
                                    case "SmallDateTime":
                                    case "DateTime":
                                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejDateTimePicker({ width: '100%', locale: " + System.Convert.ToChar(34) + "es-AR" + System.Convert.ToChar(34) + ", buttonText: { today: " + System.Convert.ToChar(34) + "Hoy" + System.Convert.ToChar(34) + ", timeNow: " + System.Convert.ToChar(34) + "Ahora" + System.Convert.ToChar(34) + ", done: " + System.Convert.ToChar(34) + "Aceptar" + System.Convert.ToChar(34) + ", timeTitle: " + System.Convert.ToChar(34) + "Hora" + System.Convert.ToChar(34) + " } });");
                                        break;
                                    case "Boolean":
                                    case "Bool":
                                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejCheckBox();");
                                        break;
                                    case "Decimal":
                                    case "Double":
                                    case "Float":
                                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejNumericTextbox({ width: " + System.Convert.ToChar(34) + "100%" + System.Convert.ToChar(34) + ", height: " + System.Convert.ToChar(34) + "34px" + System.Convert.ToChar(34) + ", decimalPlaces: 2 });");
                                        break;
                                    case "Int16":
                                    case "Int32":
                                    case "Int64":
                                        output.AppendLine("                    $(" + System.Convert.ToChar(34) + "#" + column.Name + System.Convert.ToChar(34) + ").ejNumericTextbox({ width: " + System.Convert.ToChar(34) + "100%" + System.Convert.ToChar(34) + ", height: " + System.Convert.ToChar(34) + "34px" + System.Convert.ToChar(34) + ", decimalPlaces: 0 });");
                                        break;
                                }
                            }
                        }


                        output.AppendLine("                    var tabObj = $(" + System.Convert.ToChar(34) + "#tabEdit" + System.Convert.ToChar(34) + ").ejTab({}).data(" + System.Convert.ToChar(34) + "ejTab" + System.Convert.ToChar(34) + ");");
                        output.AppendLine("");
                        output.AppendLine("                    if (tabObj.model) {");
                        output.AppendLine("                        for (var i = 1; i < length-1; i++) {");
                        output.AppendLine("                            tabObj.option({ disabledItemIndex: [i] });");
                        output.AppendLine("                        }");
                        output.AppendLine("                        ");
                        output.AppendLine("                    }");
                        output.AppendLine("");

                        output.AppendLine("");
                        output.AppendLine("                    break;");
                        output.AppendLine("                case " + System.Convert.ToChar(34) + "save" + System.Convert.ToChar(34) + ":");
                        output.AppendLine("                    save(args.data, args.action)");
                        output.AppendLine("                    break;");
                        output.AppendLine("                case " + System.Convert.ToChar(34) + "cancel" + System.Convert.ToChar(34) + ":");
                        output.AppendLine("                    break;");
                        output.AppendLine("                case " + System.Convert.ToChar(34) + "refresh" + System.Convert.ToChar(34) + ":");
                        output.AppendLine("                    break;");
                        output.AppendLine("                case " + System.Convert.ToChar(34) + "delete" + System.Convert.ToChar(34) + ":");
                        output.AppendLine("                    save(args.data, 'delete')");
                        output.AppendLine("                    break;");
                        output.AppendLine("");
                        output.AppendLine("            }");
                        output.AppendLine("");
                        output.AppendLine("        }");
                        output.AppendLine("        function save(rowData, action) {");
                        output.AppendLine("            mostrarImagenEspera(); ");
                        output.AppendLine("            $.ajax({");
                        output.AppendLine("");
                        relativepath = db.Name.ToLower() + "/" + refTable.Schema + "/";
                        output.AppendLine("                url: getwebservicesurl() + " + System.Convert.ToChar(34) + "/" + relativepath + db.Name.ToLower() + "." + refTable.Schema + "." + refTable.Name.ToLower() + ".asmx/" + System.Convert.ToChar(34) + " + action,");
                        output.AppendLine("                data: { sessionId: localStorage.getItem(" + System.Convert.ToChar(34) + "session_id" + System.Convert.ToChar(34) + "),");

                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {
                            switch (column.LanguageType)
                            {
                                case "Date":
                                case "SmallDateTime":
                                case "DateTime":
                                    output.AppendLine("                    " + column.Name + ": ej.format(new Date(rowData." + column.Name + "), " + System.Convert.ToChar(34) + "dd/MM/yyyy" + System.Convert.ToChar(34) + "),");
                                    break;
                                case "Int16":
                                    output.AppendLine("                    " + column.Name + ": parseInt(rowData." + column.Name + " || 0),");
                                    break;

                                case "Int32":
                                    output.AppendLine("                    " + column.Name + ": parseInt(rowData." + column.Name + " || 0),");
                                    break;

                                case "Int64":
                                    output.AppendLine("                    " + column.Name + ": parseInt(rowData." + column.Name + " || 0),");
                                    break;
                                case "Decimal":
                                case "Float":
                                case "Double":
                                    output.AppendLine("                    " + column.Name + ": parseInt(rowData." + column.Name + " || 0),");
                                    break;
                                default:
                                    output.AppendLine("                    " + column.Name + ": rowData." + column.Name + ",");

                                    break;
                            }
                        }
                        output.AppendLine("                },");
                        output.AppendLine("");
                        output.AppendLine("                type: 'POST',");
                        output.AppendLine("                dataType: 'json',");
                        output.AppendLine("                async: true,");
                        output.AppendLine("                crossdomain: true,");
                        output.AppendLine("                success: function (data) {");
                        output.AppendLine("                    switch (data.NumeroInformacion) {");
                        output.AppendLine("                        case 0:");
                        output.AppendLine("                            var resultado = JSON.parse(data.Dato);");
                        output.AppendLine("                             loadMainData()");
                        output.AppendLine("                            break;");
                        output.AppendLine("                        case 1001:");
                        output.AppendLine("                            raiseEventMessageToParent('SessionError', data.Dato);");
                        output.AppendLine("                            break;");
                        output.AppendLine("                        default:");
                        output.AppendLine("                            raiseEventMessageToParent('Error', data.Dato);");
                        output.AppendLine("                            break;");
                        output.AppendLine("                    }");
                        output.AppendLine("                },");
                        output.AppendLine("                error: function (result) {");
                        output.AppendLine("                    raiseEventMessageToParent('Error', 'ERROR: ' + JSON.stringify(result));");
                        output.AppendLine("                    loadMainData();");
                        output.AppendLine("                }");
                        output.AppendLine("            });");
                        output.AppendLine("        }");
                        output.AppendLine("        function loadGrid(datasource) {");
                        
                        output.AppendLine("             var grid = $(" + System.Convert.ToChar(34) + "#Grid" + System.Convert.ToChar(34) + ").data(" + System.Convert.ToChar(34) + "ejGrid" + System.Convert.ToChar(34) + ");");
                        output.AppendLine("             if (grid != undefined) {");
                        output.AppendLine("                 var filtros = grid.model.filterSettings.filteredColumns;");
                        output.AppendLine("                 var paginador = grid.model.pageSettings.currentPage;");
                        output.AppendLine("             }");
                        output.AppendLine("");

                        output.AppendLine("            $(" + System.Convert.ToChar(34) + "#Grid" + System.Convert.ToChar(34) + ").ejGrid({");
                        output.AppendLine("");
                        output.AppendLine("                dataSource: datasource,");
                        output.AppendLine("                allowPaging: true,");
                        output.AppendLine("                enablelAltRow: true,");
                        output.AppendLine("                locale: 'es-AR',");
                        output.AppendLine("                dateFormat: " + System.Convert.ToChar(34) + "dd/MM/yyyy" + System.Convert.ToChar(34) + ",");
                        output.AppendLine("                actionComplete: " + System.Convert.ToChar(34) + "complete" + System.Convert.ToChar(34) + ",");
                        output.AppendLine("                editSettings: { allowEditing: true, allowAdding: true, allowDeleting: true, editMode: ej.Grid.EditMode.DialogTemplate, dialogEditorTemplateID: " + System.Convert.ToChar(34) + "#template" + System.Convert.ToChar(34) + ", showDeleteConfirmDialog: true },");
                        output.AppendLine("                toolbarSettings: { showToolbar: true, toolbarItems: [ej.Grid.ToolBarItems.Add, ej.Grid.ToolBarItems.Edit, ej.Grid.ToolBarItems.Delete,ej.Grid.ToolBarItems.PrintGrid], customToolbarItems: [{ templateID: " + System.Convert.ToChar(34) + "#menuselect" + System.Convert.ToChar(34) + " }] },");

                        output.AppendLine("                allowFiltering: true,");
                        output.AppendLine("                enableHeaderHover: true,");
                        output.AppendLine("                allowGrouping: true,");
                        output.AppendLine("                allowSorting: true,");
                        output.AppendLine("                filterSettings: { filterType: " + System.Convert.ToChar(34) + "excel" + System.Convert.ToChar(34) + " },");
                        output.AppendLine("                contextMenuSettings: { enableContextMenu: true },");
                        output.AppendLine("                showColumnChooser: true,");
                        output.AppendLine("");
                        output.AppendLine("                columns: [");
                        int keysCount = 0;
                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {
                            if (column.IsInPrimaryKey)
                                keysCount += 1;
                        }

                        string pkInfo = string.Empty;
                        
                        foreach (MyMeta.IColumn column in refTable.Columns)
                        {
                            if (column.IsInForeignKey)// && !column.IsInPrimaryKey)
                            {
                                foreach (MyMeta.IForeignKey fk in column.ForeignKeys)
                                {
                                    if (!refTable.Name.Equals(fk.PrimaryTable.Name))
                                    {
                                        string name = FindFirstVarcharColumnName(fk.PrimaryTable);
                                        string value = FindKeyColumnName(fk.PrimaryTable);

                                        if (column.IsInPrimaryKey)
                                            pkInfo = ", isPrimaryKey: true";
                                        string fieldDesc = column.Description.Equals(String.Empty) ? column.Name : column.Description;
                                        output.AppendLine("                        { field: " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + pkInfo + ", foreignKeyField: " + System.Convert.ToChar(34) + value + System.Convert.ToChar(34) + ", foreignKeyValue: " + System.Convert.ToChar(34) + name + System.Convert.ToChar(34) + ", dataSource: fk" + fk.PrimaryTable.Name + "Data, width: 75, headerText: " + System.Convert.ToChar(34) + fieldDesc + System.Convert.ToChar(34) + " },");
                                    }
                                    else
                                    {
                                        // Only one ke
                                        if (keysCount == 1)
                                        {
                                            if (column.IsInPrimaryKey)
                                                pkInfo = ", isPrimaryKey: true";
                                            string fieldDesc = column.Description.Equals(String.Empty) ? column.Name : column.Description;
                                            String sline = "                        { field: " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + pkInfo + ", width: 75, headerText: " + System.Convert.ToChar(34) + fieldDesc + System.Convert.ToChar(34) + " },";
                                            if (output.ToString().IndexOf(sline) == -1)
                                                output.AppendLine(sline);

                                        }
                                    }

                                }
                            }

                            else
                            {
                                string fieldDesc = column.Description.Equals(String.Empty) ? column.Name : column.Description;
                                switch (column.LanguageType)
                                {

                                    case "Boolean":
                                        //{ field: "Principal", headerText: 'Principal', width: 50, editType: ej.Grid.EditingType.Boolean, displayAsCheckBox: true }
                                        output.AppendLine("                        { field: " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ", headerText: " + System.Convert.ToChar(34) + fieldDesc + System.Convert.ToChar(34) + ", editType: ej.Grid.EditingType.Boolean, width: 50, displayAsCheckBox: true },");
                                        break;
                                    case "Date":
                                    case "SmallDateTime":
                                    case "DateTime":
                                        output.AppendLine("                        { field: " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ", headerText: " + System.Convert.ToChar(34) + fieldDesc + System.Convert.ToChar(34) + ", editType: ej.Grid.EditingType.DateTimePicker, width: 90, dateTimeFormat: " + System.Convert.ToChar(34) + "dd/MM/yyyy" + System.Convert.ToChar(34) + ", editParams: {}},");
                                        break;
                                    case "String":
                                        if (column.IsInPrimaryKey)
                                            output.AppendLine("                        { field: " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ", isPrimaryKey: true, headerText: '" + fieldDesc + "', width: 150 },");
                                        else
                                            output.AppendLine("                        { field: " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ", headerText: '" + fieldDesc + "', width: 150 },");
                                        break;
                                    case "Int16":
                                    case "Int32":
                                    case "Int64":
                                        if (column.IsInPrimaryKey)
                                            output.AppendLine("                        { field: " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ", isPrimaryKey: true, headerText: " + System.Convert.ToChar(34) + fieldDesc + System.Convert.ToChar(34) + ", textAlign: ej.TextAlign.Right, validationRules: { required: true, number: true }, width: 90 },");
                                        else
                                            output.AppendLine("                        { field: " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ", headerText: '" + fieldDesc + "', textAlign: ej.TextAlign.Right, width: 100 },");
                                        break;
                                    case "Decimal":
                                    case "Float":
                                    case "Double":
                                        output.AppendLine("                        { field: " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ", headerText: '" + fieldDesc + "', format: " + System.Convert.ToChar(34) + "{0:C}" + System.Convert.ToChar(34) + ", textAlign: ej.TextAlign.Right, editType: ej.Grid.EditingType.Numeric, editParams: { decimalPlaces: 2 }, width: 75 },");
                                        break;
                                    default:
                                        if (column.IsInPrimaryKey)
                                            output.AppendLine("                        { field: " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ", isPrimaryKey: true, headerText: '" + fieldDesc + "', width: 150 },");
                                        else
                                            output.AppendLine("                        { field: " + System.Convert.ToChar(34) + column.Name + System.Convert.ToChar(34) + ", headerText: '" + fieldDesc + "', width: 150 },");
                                        break;
                                }
                            }
                        }

                        output.AppendLine("");
                        output.AppendLine("                ]");
                        output.AppendLine("            });");
                        output.AppendLine("         $(" + System.Convert.ToChar(34) + "#filtermenu" + System.Convert.ToChar(34) + ").ejDropDownList({ " + System.Convert.ToChar(34) + "change" + System.Convert.ToChar(34) + ": " + System.Convert.ToChar(34) + "selectChange" + System.Convert.ToChar(34) + ", width: " + System.Convert.ToChar(34) + "120px" + System.Convert.ToChar(34) + ", selectedItemIndex: 0 })");


                        output.AppendLine("         if (grid != undefined) {");
                        output.AppendLine("             grid.model.filterSettings.filteredColumns = filtros;");
                        output.AppendLine("             grid.model.pageSettings.currentPage = paginador;");
                        output.AppendLine("             grid.refreshContent();");
                        output.AppendLine("         }");
                        
                        output.AppendLine("         ocultarImagenEspera();");
                        output.AppendLine("        }");
                        output.AppendLine("");
                        output.AppendLine("        function selectChange(args) {");
                        output.AppendLine("            $(" + System.Convert.ToChar(34) + "#Grid" + System.Convert.ToChar(34) + ").ejGrid(" + System.Convert.ToChar(34) + "option" + System.Convert.ToChar(34) + ", { " + System.Convert.ToChar(34) + "filterSettings" + System.Convert.ToChar(34) + ": { " + System.Convert.ToChar(34) + "filterType" + System.Convert.ToChar(34) + ": this.getSelectedValue() } });");
                        output.AppendLine("        }");
                        output.AppendLine("");
                        output.AppendLine("    </script>");

                        output.AppendLine("</body>");
                        output.AppendLine("</html>");

                        relativepath = db.Name.ToLower() + "/" + refTable.Schema + "/";
                        SaveOutputToFile(db.Name + "." + refTable.Schema + "." + refTable.Name + ".aspx", output, relativepath, true);
                    }
                }
                // Main Crud navigator
                


                List<String> schemas = new List<string>();

                foreach (MyMeta.ITable refTable in db.Tables)
                {
                    if (refTable.Selected)
                    {
                        if (!schemas.Contains(refTable.Schema))
                            schemas.Add(refTable.Schema);
                    }
                }


                foreach (string schema in schemas)
                {
                    output = new System.Text.StringBuilder();

                    output.AppendLine("<%@ Page Language=" + System.Convert.ToChar(34) + "C#" + System.Convert.ToChar(34) + " ValidateRequest=" + System.Convert.ToChar(34) + "false" + System.Convert.ToChar(34) + " %>");

                    output.AppendLine("");
                    output.AppendLine("");
                    output.AppendLine("<!DOCTYPE html>");
                    output.AppendLine("");
                    output.AppendLine("<html xmlns=" + System.Convert.ToChar(34) + "http://www.w3.org/1999/xhtml" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("<head>");
                    output.AppendLine("    <title>Maestros</title>");
                    output.AppendLine("        <meta charset=" + System.Convert.ToChar(34) + "utf-8" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("    <meta name=" + System.Convert.ToChar(34) + "viewport" + System.Convert.ToChar(34) + " content=" + System.Convert.ToChar(34) + "width=device-width, initial-scale=1.0" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("    <meta http-equiv=" + System.Convert.ToChar(34) + "cache-control" + System.Convert.ToChar(34) + " content=" + System.Convert.ToChar(34) + "max-age=0" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("    <meta http-equiv=" + System.Convert.ToChar(34) + "cache-control" + System.Convert.ToChar(34) + " content=" + System.Convert.ToChar(34) + "no-cache" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("    <meta http-equiv=" + System.Convert.ToChar(34) + "expires" + System.Convert.ToChar(34) + " content=" + System.Convert.ToChar(34) + "0" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("    <meta http-equiv=" + System.Convert.ToChar(34) + "expires" + System.Convert.ToChar(34) + " content=" + System.Convert.ToChar(34) + "Tue, 01 Jan 1980 1:00:00 GMT" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("    <meta http-equiv=" + System.Convert.ToChar(34) + "pragma" + System.Convert.ToChar(34) + " content=" + System.Convert.ToChar(34) + "no-cache" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("");
                    output.AppendLine("    <link href=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/content/bootstrap.min.css" + System.Convert.ToChar(34) + " rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("    <link href=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/content/ejthemes/default-theme/ej.widgets.all.min.css" + System.Convert.ToChar(34) + " rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("    <link href=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/content/default.css" + System.Convert.ToChar(34) + " rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("    <link href=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/content/default-responsive.css" + System.Convert.ToChar(34) + " rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("    <link href=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/content/ejthemes/responsive-css/ej.responsive.css" + System.Convert.ToChar(34) + " rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("");
                    output.AppendLine("    <link href=" + System.Convert.ToChar(34) + "/Content/Site.css" + System.Convert.ToChar(34) + " rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("    <link href=" + System.Convert.ToChar(34) + "/favicon.ico" + System.Convert.ToChar(34) + " rel=" + System.Convert.ToChar(34) + "shortcut icon" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "image/x-icon" + System.Convert.ToChar(34) + " />");
                    output.AppendLine("");
                    output.AppendLine("</head>");
                    output.AppendLine("");
                    output.AppendLine("<body>");
                    output.AppendLine("    <div id=" + System.Convert.ToChar(34) + "loading" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("        <img id=" + System.Convert.ToChar(34) + "loading-image" + System.Convert.ToChar(34) + " src=" + System.Convert.ToChar(34) + _resourceUrl + "/imagenes/gif/loadingGears.gif" + System.Convert.ToChar(34) + " alt=" + System.Convert.ToChar(34) + "Cargando..." + System.Convert.ToChar(34) + " />");
                    output.AppendLine("    </div>");
                    output.AppendLine("");
                    output.AppendLine("    <div class=" + System.Convert.ToChar(34) + "content-container-fluid" + System.Convert.ToChar(34) + " id=" + System.Convert.ToChar(34) + "divPpal" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("        <div class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("            <div class=" + System.Convert.ToChar(34) + "cols-sample-area" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("                <div id=" + System.Convert.ToChar(34) + "tabMaestros" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("                    <ul>");

                    string tableDesc = String.Empty;

                    foreach (MyMeta.ITable refTable in db.Tables)
                    {
                        if (refTable.Selected && refTable.Schema.Equals(schema))
                        {
                            tableDesc = refTable.Name.Equals(String.Empty) ? refTable.Description : refTable.Name;
                            output.AppendLine("                        <li><a href=" + System.Convert.ToChar(34) + "#" + refTable.Name + System.Convert.ToChar(34) + ">" + tableDesc.Replace("_"," ") + "</a></li>");
                        }
                    }


                    output.AppendLine("                    </ul>");
                    output.AppendLine("");



                    foreach (MyMeta.ITable refTable in db.Tables)
                    {
                        if (refTable.Selected && refTable.Schema.Equals(schema))
                        {
                            tableDesc = refTable.Name.Equals(String.Empty) ? refTable.Description : refTable.Name;
                            output.AppendLine("                    <div id=" + System.Convert.ToChar(34) + tableDesc + System.Convert.ToChar(34) + ">");
                            output.AppendLine("                        <iframe id=" + System.Convert.ToChar(34) + "iframe" + tableDesc + System.Convert.ToChar(34) + "></iframe>");
                            output.AppendLine("                    </div>");
                        }
                    }

                    output.AppendLine("                </div>");
                    output.AppendLine("            </div>");
                    output.AppendLine("        </div>");
                    output.AppendLine("    </div>");
                    output.AppendLine("<!--[if lt IE 9]>");
                    output.AppendLine("        <script type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + " src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/jquery-1.11.3.min.js" + System.Convert.ToChar(34) + " ></script>");
                    output.AppendLine("    <![endif]-->");
                    output.AppendLine("<!--[if gte IE 9]><!-->");
                    output.AppendLine("<script src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/jquery-2.1.4.min.js" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + "></script>");
                    output.AppendLine("<!--<![endif]-->");
                    output.AppendLine("<script src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/jquery.easing.1.3.min.js" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + "></script>");
                    output.AppendLine("<script src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/jquery.validate.min.js" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + "></script>");
                    output.AppendLine("<script src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/jquery.validate.unobtrusive.min.js" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + "></script>");
                    output.AppendLine("<script src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/jsrender.min.js" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + "></script>");
                    output.AppendLine("<script src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/ej.web.all.min.js" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + "></script>");
                    output.AppendLine("<script src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/properties.js" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + "></script>");
                    output.AppendLine("<script src=" + System.Convert.ToChar(34) + _resourceUrl + "/Ejs/scripts/cultures/ej.culture.es-AR.min.js" + System.Convert.ToChar(34) + " type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + "></script>");
                    output.AppendLine("");
                    output.AppendLine("<script src=" + System.Convert.ToChar(34) + "<%= ResolveUrl(" + System.Convert.ToChar(34) + "~/config.js" + System.Convert.ToChar(34) + ") %>" + System.Convert.ToChar(34) + "></script>");
                    output.AppendLine("<script src=" + System.Convert.ToChar(34) + "<%= ResolveUrl(" + System.Convert.ToChar(34) + "~/Scripts/Site.js" + System.Convert.ToChar(34) + ") %>" + System.Convert.ToChar(34) + "></script>");
                    //output.AppendLine("<script src=" + System.Convert.ToChar(34) + "<%= ResolveUrl(" + System.Convert.ToChar(34) + "~/Scripts/validateExtension.js" + System.Convert.ToChar(34) + ") %>" + System.Convert.ToChar(34) + "></script>");
                    
                    output.AppendLine("");
                    output.AppendLine("    <script type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("        (function ($, window, document) {");
                    output.AppendLine("            $(function () {");
                    output.AppendLine("                cargarIFrame(0);");
                    output.AppendLine("");
                    output.AppendLine("                $(" + System.Convert.ToChar(34) + "#tabMaestros" + System.Convert.ToChar(34) + ").ejTab({");
                    output.AppendLine("                    beforeActive: " + System.Convert.ToChar(34) + "onClientBeforeActive" + System.Convert.ToChar(34) + ",");
                    output.AppendLine("                    heightAdjustMode: ej.Tab.HeightAdjustMode.Content,");
                    output.AppendLine("                    headerPosition: " + System.Convert.ToChar(34) + "left" + System.Convert.ToChar(34) + ",");
                    output.AppendLine("                    showReloadIcon: true");
                    output.AppendLine("                });");
                    output.AppendLine("");
                    output.AppendLine("                ocultarImagenEspera();");
                    output.AppendLine("            });");
                    output.AppendLine("        }(window.jQuery, window, document));");
                    output.AppendLine("");
                    output.AppendLine("");
                    output.AppendLine("        function onClientBeforeActive(e) {");
                    output.AppendLine("            cargarIFrame(e.activeIndex);");
                    output.AppendLine("        }");
                    output.AppendLine("");
                    output.AppendLine("        function cargarIFrame(indice) {");
                    output.AppendLine("            var height = $(window).height();");
                    output.AppendLine("            switch (indice) {");

                    Int16 caseIndex = 0;


                    foreach (MyMeta.ITable refTable in db.Tables)
                    {
                        if (refTable.Selected && refTable.Schema.Equals(schema))
                        {
                            tableDesc = refTable.Description.Equals(String.Empty) ? refTable.Name : refTable.Description;
                            output.AppendLine("                case " + caseIndex.ToString() + ":");
                            output.AppendLine("                    $('#iframe" + tableDesc + "').attr('src', '" + db.Name.ToLower() + "." + refTable.Schema + "." + tableDesc + ".aspx');");
                            output.AppendLine("                    $('#iframe" + tableDesc + "').css('height', height * 0.75 | 0);");
                            output.AppendLine("                    break;");
                            caseIndex += 1;
                        }
                    }



                    output.AppendLine("                default:");
                    output.AppendLine("                    break;");
                    output.AppendLine("            }");
                    output.AppendLine("        }");
                    output.AppendLine("    </script>");
                    output.AppendLine("");
                    //output.AppendLine("<!-- Visual Studio Browser Link -->");
                    //output.AppendLine("<script type=" + System.Convert.ToChar(34) + "application/json" + System.Convert.ToChar(34) + " id=" + System.Convert.ToChar(34) + "__browserLink_initializationData" + System.Convert.ToChar(34) + ">");
                    //output.AppendLine("    {" + System.Convert.ToChar(34) + "appName" + System.Convert.ToChar(34) + ":" + System.Convert.ToChar(34) + "Internet Explorer" + System.Convert.ToChar(34) + "," + System.Convert.ToChar(34) + "requestId" + System.Convert.ToChar(34) + ":" + System.Convert.ToChar(34) + "790812fe244943ff8b8afb815b6ecfcb" + System.Convert.ToChar(34) + "}");
                    //output.AppendLine("</script>");
                    //output.AppendLine("<script type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + " src=" + System.Convert.ToChar(34) + "http://localhost:6391/23cfe249a4a3442a810173c05724332c/browserLink" + System.Convert.ToChar(34) + " async=" + System.Convert.ToChar(34) + "async" + System.Convert.ToChar(34) + "></script>");
                    //output.AppendLine("<!-- End Browser Link -->");
                    output.AppendLine("");
                    output.AppendLine("</body>");
                    output.AppendLine("</html>");
                    output.AppendLine("");

                    string relativepath = db.Name.ToLower() + "/" + schema + "/";

                    SaveOutputToFile(db.Name + "." + schema + ".Crud.aspx", output, relativepath, true);
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
            string returnvalue = string.Empty;

            foreach (MyMeta.IColumn item in table.Columns)
            {
                if (item.DataTypeName.StartsWith("varchar"))
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

        #region Save File Method
        private void SaveOutputToFile(string fileName, System.Text.StringBuilder output, string relativepath, bool overWrite)
        {
            if (!_workingdir.EndsWith("\\"))
                _workingdir += "\\";
            string filePath = _workingdir + "SyncfusionCrud\\" + relativepath + fileName;
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
            get { return "c6884693-f5e6-404c-bc2d-240cd6d215e2"; }
        }
        #region ITemplate Members

        public string OutputLanguage
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
    // End class