using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

public class CrudBuilderPlugin : IPlugin
{
    private List<ITemplate> _templates = new List<ITemplate>();
    const string PluginName = "CrudBuilder";
    const string PluginDescription = "Crud Builder Plugin";
    public CrudBuilderPlugin()
    {
        _templates.Add(new CrudBuilderTemplate());
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

    public class CrudBuilderTemplate : ITemplate
    {
        const string TemplateName = "CrudBuilder Template";
        const string TemplateDescription = "Crud ASP.NET Webforms builder Template";
        const string TemplateoutputLanguage = "C#";
        private ArrayList _arraylist = new ArrayList();
        private string _workingdir;
        private string _languageMappingFileName;
        private string _dbTargetMappingFileName;
        private GenerationProject _generationProject = null;

        private ArrayList _refkeyList = new ArrayList();
        public bool ExecuteAnterior(ref MyMeta.IDatabase db, string workingDir, GenerationProject generationProject)
        {
            try
            {
                _workingdir = workingDir;
                MyMeta.IDatabase _dataBase = db;

                db.Root.DbTarget = GetDbTarget(db.Root.Driver);
                db.Root.Language = GetLanguage(db.Root.Driver);//"C# Types";


                

                foreach (MyMeta.ITable table in db.Tables)
                {
                    if (table.Selected)
                    {
                        System.Text.StringBuilder output = new System.Text.StringBuilder();

                        //if (OnInfo != null)
                        //{
                        //    OnInfo("Building " + table.Name + "...");
                        //}


                        output.AppendLine("<%@ Page Language=" + System.Convert.ToChar(34) + "C#" + System.Convert.ToChar(34) + " %>");
                        output.AppendLine("");
                        output.AppendLine("<!DOCTYPE html>");
                        output.AppendLine("<script runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("");
                        output.AppendLine("    public enum PageState");
                        output.AppendLine("    {");
                        output.AppendLine("        ReadOnly,");
                        output.AppendLine("        Add,");
                        output.AppendLine("        Edit,");
                        output.AppendLine("        Delete");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Maps events to handlers and loads data ");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    protected void Page_Load(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        //output.AppendLine("");
                        //output.AppendLine("        if (Request.UrlReferrer==null)");
                        //output.AppendLine("            Response.Redirect(" + System.Convert.ToChar(34) + "http://localhost:3868/Default.aspx" + System.Convert.ToChar(34) + ");");
                        output.AppendLine("");
                        output.AppendLine("        MainGridView.RowEditing += MainGridView_RowEditing;");
                        output.AppendLine("        MainGridView.RowDeleting += MainGridView_RowDeleting;");
                        output.AppendLine("        MainGridView.PageIndexChanging += MainGridView_PageIndexChanging;");
                        output.AppendLine("        MainGridView.Sorting += MainGridView_Sorting;");
                        //output.AppendLine("        //btnAddNew.Click += btnAddNew_Click;");
                        output.AppendLine("        btnAccept.Click += btnAccept_Click;");
                        output.AppendLine("        btnCancel.Click += btnCancel_Click;");
                        output.AppendLine("        btnAcceptException.Click += btnAcceptException_Click;");
                        output.AppendLine("");
                        output.AppendLine("        if (!IsPostBack)");
                        output.AppendLine("        {");
                        output.AppendLine("            DataPanel.Visible = true;");
                        output.AppendLine("            DetailPanel.Visible = false;");
                        output.AppendLine("            this.hiddenState.Value = ((int)PageState.ReadOnly).ToString();");
                        output.AppendLine("            LoadMainGridViewData();");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInForeignKey && !column.IsInPrimaryKey)
                            {
                                string refKeyName = GetFormattedEntityName(column.ForeignKeys[0].ForeignTable.Name);
                                output.AppendLine("            LoadCombo" + GetFormattedEntityName(column.Name) + "();");
                            }
                        }
                        output.AppendLine("");
                        output.AppendLine("        }");
                        output.AppendLine("        LoadTitles();");
                        output.AppendLine("    }");
                        output.AppendLine("    private void LoadTitles()");
                        output.AppendLine("    {");
                        output.AppendLine("        //Int64 languageId = 1; //Default es-AR");
                        output.AppendLine("        //if (SessionFacade.ActiveUSer != null)");
                        output.AppendLine("        //  languageId = SessionFacade.ActiveUSer.LanguageId;");
                        output.AppendLine("        //lblMainTitle.Text = Functions.GetTitle(Path.GetFileName(Request.PhysicalPath), lblMainTitle.ID, languageId);");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Sorts Gridview data based on column click");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    void MainGridView_Sorting(object sender, GridViewSortEventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Changes Gridview page index");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    void MainGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("        MainGridView.PageIndex = e.NewPageIndex;");
                        output.AppendLine("        LoadMainGridViewData();");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Used to add a new entity");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    protected void btnAddNew_Click(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("        SetMode(PageState.Add);");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Cancels operation");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    void btnCancel_Click(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("        SetMode(PageState.ReadOnly);");
                        output.AppendLine("        ClearControls();");
                        output.AppendLine("        LoadMainGridViewData();");
                        output.AppendLine("        MainGridView.SelectedIndex = -1;");
                        output.AppendLine("    }");
                        //output.AppendLine("    /// <summary>");
                        //output.AppendLine("    /// Accepts current operation");
                        //output.AppendLine("    /// </summary>");
                        //output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        //output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        //output.AppendLine("    void btnAceptar_Click(object sender, EventArgs e)");
                        //output.AppendLine("    {");
                        //output.AppendLine("        pnlModal.Visible = true;");
                        //output.AppendLine("    }");


                        string refParameters = string.Empty;
                        string refArguments = string.Empty;
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                refParameters += column.LanguageType + " " + GetFormattedEntityName(column.Name) + ",";
                                refArguments += GetFormattedEntityName(column.Name) + ",";
                            }
                        }
                        if (refParameters.Length == 0)
                            refParameters += " ";
                        if (refArguments.Length == 0)
                            refArguments += " ";

                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Fires when user clicks " + System.Convert.ToChar(34) + "Delete" + System.Convert.ToChar(34) + "");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    void MainGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("        SetMode(PageState.Delete);");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("         " + column.LanguageType + " " + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(MainGridView.DataKeys[e.RowIndex].Values[" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "].ToString());");
                            }
                        }

                        output.AppendLine("        LoadUIFromEntity(" + refArguments.Substring(0, refArguments.Length - 1) + ");");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Fires when user clicks " + System.Convert.ToChar(34) + "Edit" + System.Convert.ToChar(34) + "");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    void MainGridView_RowEditing(object sender, GridViewEditEventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("        SetMode(PageState.Edit);");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("         " + column.LanguageType + " " + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(MainGridView.DataKeys[e.NewEditIndex].Values[" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "].ToString());");
                            }
                        }

                        output.AppendLine("        LoadUIFromEntity(" + refArguments.Substring(0, refArguments.Length - 1) + ");");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Loads Detail data into edit controls in UI ");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "rowIndex" + System.Convert.ToChar(34) + "></param>");


                        output.AppendLine("    private void LoadUIFromEntity(" + refParameters.Substring(0,refParameters.Length-1) + ")");
                        output.AppendLine("    {");
                        output.AppendLine("    try");
                        output.AppendLine("         {");
                        output.AppendLine("             " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " entity = new " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("             " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("");
                        output.AppendLine("");

                        output.AppendLine("             entity = entities.Items(" + refArguments.Substring(0, refArguments.Length - 1) + ").FirstOrDefault();");

                        foreach (MyMeta.IColumn  column in table.Columns)
                        {
                                if(column.IsInPrimaryKey)
                                    output.AppendLine("             lbl" + GetFormattedEntityName(column.Name) + "Value.Text = entity." + GetFormattedEntityName(column.Name) + ".ToString();");
                                else
                 switch (column.LanguageType)
                                    {
                                        case "String":
                                        case "DateTime":
                                            output.AppendLine("        txt" + GetFormattedEntityName(column.Name) + "Edit.Text = entity." + GetFormattedEntityName(column.Name) + ".ToString();");
                                            break;
                                        case "Boolean":
                                        case "Bool":
                                            output.AppendLine("        chk" + GetFormattedEntityName(column.Name) + "Edit.Checked = entity." + GetFormattedEntityName(column.Name) + ";");
                                            break;
                                    }                        }
                        output.AppendLine("         }");
                        output.AppendLine("         catch (Exception ex)");
                        output.AppendLine("         {");
                        output.AppendLine("             ShowException(ex);");
                        output.AppendLine("         }");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Loads Detail data into edit controls in UI ");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "rowIndex" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    private void LoadEntityFromUI(" + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " entity)");
                        output.AppendLine("    {");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                                output.AppendLine("        entity." + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(lbl" + GetFormattedEntityName(column.Name) + "Value.Text);");
                            else
                                if (column.IsInForeignKey)
                                    output.AppendLine("        entity." + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(comboBox" + GetFormattedEntityName(column.Name) + ".SelectedValue.ToString());");
                                else
                                {
                                    switch (column.LanguageType)
                                    {
                                        case "String":
                                        case "DateTime":
                                            output.AppendLine("        entity." + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(txt" + GetFormattedEntityName(column.Name) + "Edit.Text);");
                                            break;
                                        case "Boolean":
                                        case "Bool":
                                            output.AppendLine("        entity." + GetFormattedEntityName(column.Name) + " = chk" + GetFormattedEntityName(column.Name) + "Edit.Checked;");
                                            break;
                                    }
                                }

                        }
                        output.AppendLine("    }");
                        output.AppendLine("");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Adds new entity.");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void AddEntity()");
                        output.AppendLine("    {");
                        output.AppendLine("     try");
                        output.AppendLine("         {");
                        output.AppendLine("             " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " entity = new " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("             " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                if (!column.IsAutoKey)
                                {
                                    output.AppendLine("             entity." + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(lbl" + GetFormattedEntityName(column.Name) + "Value.Text);");
                                }
                            }
                            else
                                if (column.IsInForeignKey)
                                    output.AppendLine("        entity." + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(comboBox" + GetFormattedEntityName(column.Name) + ".SelectedValue.ToString());");
                                else
                                {
                                    switch (column.LanguageType)
                                    {
                                        case "String":
                                        case "DateTime":
                                            output.AppendLine("        entity." + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(txt" + GetFormattedEntityName(column.Name) + "Edit.Text);");
                                            break;
                                        case "Boolean":
                                        case "Bool":
                                            output.AppendLine("        entity." + GetFormattedEntityName(column.Name) + " = chk" + GetFormattedEntityName(column.Name) + "Edit.Checked;");
                                            break;

                                    }
                                }
                        }
                        
                        output.AppendLine("             entities.Add(entity);");
                        output.AppendLine("");
                        output.AppendLine("             DataPanel.Visible = true;");
                        output.AppendLine("             DetailPanel.Visible = false;");
                        output.AppendLine("             LoadMainGridViewData();");
                        output.AppendLine("         }");
                        output.AppendLine("         catch (Exception ex)");
                        output.AppendLine("         {");
                        output.AppendLine("             ShowException(ex);");
                        output.AppendLine("         }");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Updates current entity");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void UpdateEntity()");
                        output.AppendLine("    {");
                        output.AppendLine("        " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " entity = new " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("        " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("");
                        output.AppendLine("        LoadEntityFromUI(entity);");
                        output.AppendLine("        entities.Update(entity);");
                        output.AppendLine("");
                        output.AppendLine("        DataPanel.Visible = true;");
                        output.AppendLine("        DetailPanel.Visible = false;");
                        output.AppendLine("        LoadMainGridViewData();");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Deletes current entity");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void DeleteEntity()");
                        output.AppendLine("    {");
                        output.AppendLine("        " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " entity = new " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("        " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("        LoadEntityFromUI(entity);");
                        output.AppendLine("        entities.Delete(entity);");
                        output.AppendLine("        DataPanel.Visible = true;");
                        output.AppendLine("        DetailPanel.Visible = false;");
                        output.AppendLine("        LoadMainGridViewData();");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Loads data from datasource into Gridview");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <returns></returns>");
                        output.AppendLine("    private Int32 LoadMainGridViewData()");
                        output.AppendLine("    {");
                        output.AppendLine("        try");
                        output.AppendLine("        {");
                        output.AppendLine("            MainGridView.EditIndex = -1;");
                        output.AppendLine("            MainGridView.SelectedIndex = -1;");
                        output.AppendLine("            " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("            MainGridView.DataSource = entities.Items();");
                        output.AppendLine("            MainGridView.DataBind();");
                        output.AppendLine("            return MainGridView.Rows.Count;");
                        output.AppendLine("        }");
                        output.AppendLine("        catch (Exception ex)");
                        output.AppendLine("        {");
                        output.AppendLine("            throw ex;");
                        output.AppendLine("        }");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Sets current mode operation");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "mode" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    private void SetMode(PageState mode)");
                        output.AppendLine("    {");
                        output.AppendLine("        switch (mode)");
                        output.AppendLine("        {");
                        output.AppendLine("            case PageState.ReadOnly:");
                        output.AppendLine("                DataPanel.Visible = true;");
                        output.AppendLine("                DetailPanel.Visible = false;");
                        output.AppendLine("                EnableControls(false);");
                        output.AppendLine("                this.hiddenState.Value = ((int)PageState.ReadOnly).ToString();");
                        output.AppendLine("                break;");
                        output.AppendLine("            case PageState.Add:");
                        output.AppendLine("                DataPanel.Visible = false;");
                        output.AppendLine("                DetailPanel.Visible = true;");
                        output.AppendLine("                ClearControls();");
                        output.AppendLine("                EnableControls(true);");
                        output.AppendLine("                this.hiddenState.Value = ((int)PageState.Add).ToString();");
                        output.AppendLine("                break;");
                        output.AppendLine("            case PageState.Edit:");
                        output.AppendLine("                DataPanel.Visible = false;");
                        output.AppendLine("                DetailPanel.Visible = true;");
                        output.AppendLine("                EnableControls(true);");
                        output.AppendLine("                this.hiddenState.Value = ((int)PageState.Edit).ToString();");
                        output.AppendLine("                break;");
                        output.AppendLine("            case PageState.Delete:");
                        output.AppendLine("                DataPanel.Visible = false;");
                        output.AppendLine("                DetailPanel.Visible = true;");
                        output.AppendLine("                EnableControls(false);");
                        output.AppendLine("                this.hiddenState.Value = ((int)PageState.Delete).ToString();");
                        output.AppendLine("                break;");
                        output.AppendLine("            default:");
                        output.AppendLine("                break;");
                        output.AppendLine("        }");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Enables or disables data controls");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "value" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    void EnableControls(bool value)");
                        output.AppendLine("    {");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                                output.AppendLine("        lbl" + GetFormattedEntityName(column.Name) + "Value.Enabled = value;");
                            else
                                if (column.IsInForeignKey)
                                    output.AppendLine("        comboBox" + GetFormattedEntityName(column.Name) + ".Enabled = value;");
                                else
                                {
                                    switch (column.LanguageType)
                                    {
                                        case "String":
                                        case "DateTime":
                                            output.AppendLine("        txt" + GetFormattedEntityName(column.Name) + "Edit.Enabled = value;");
                                            break;
                                        case "Boolean":
                                        case "Bool":
                                            output.AppendLine("        chk" + GetFormattedEntityName(column.Name) + "Edit.Enabled = value;");
                                            break;
                                    }
                                }
                        }

                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Clears data controls");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    void ClearControls()");
                        output.AppendLine("    {");

                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                                output.AppendLine("        lbl" + GetFormattedEntityName(column.Name) + "Value.Text = string.Empty;");
                            else
                                if (column.IsInForeignKey)
                                    output.AppendLine("        comboBox" + GetFormattedEntityName(column.Name) + ".SelectedIndex = 0;");
                                else
                                {
                                    switch (column.LanguageType)
                                    {
                                        case "String":
                                        case "DateTime":
                                            output.AppendLine("        txt" + GetFormattedEntityName(column.Name) + "Edit.Text = string.Empty;");
                                            break;
                                        case "Boolean":
                                        case "Bool":
                                            output.AppendLine("        chk" + GetFormattedEntityName(column.Name) + "Edit.Checked = false;");
                                            break;

                                    }
                                }
                        }

                        output.AppendLine("    }");
                        output.AppendLine("");
                        output.AppendLine("    protected void btnEdit_Click(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("        Int16 rowIndex = Convert.ToInt16(((Button)sender).CommandArgument);");
                        string refKeys = string.Empty;
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("         " + column.LanguageType + " " + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(MainGridView.DataKeys[rowIndex].Values[" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "].ToString());");
                            }
                        }
                        //output.AppendLine("        " + refKeys);
                        output.AppendLine("        SetMode(PageState.Edit);");
                        output.AppendLine("        LoadUIFromEntity(" + refArguments.Substring(0, refArguments.Length - 1) + ");");
                        output.AppendLine("    }");
                        output.AppendLine("    protected void btnDelete_Click(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        //output.AppendLine("        " + refKeys);
                        output.AppendLine("        Int16 rowIndex = Convert.ToInt16(((Button)sender).CommandArgument);");
                        
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("         " + column.LanguageType + " " + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(MainGridView.DataKeys[rowIndex].Values[" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "].ToString());");
                            }
                        }
                        output.AppendLine("        SetMode(PageState.Delete);");
                        output.AppendLine("        LoadUIFromEntity(" + refArguments.Substring(0, refArguments.Length - 1) + ");");
                        output.AppendLine("    }");
                        output.AppendLine("");
                        output.AppendLine("    protected void btnAccept_Click(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        //output.AppendLine("        pnlModal.Visible = false;");
                        output.AppendLine("        int value = Convert.ToInt32(this.hiddenState.Value);");
                        output.AppendLine("        switch ((PageState)value)");
                        output.AppendLine("        {");
                        output.AppendLine("            case PageState.ReadOnly:");
                        output.AppendLine("                break;");
                        output.AppendLine("            case PageState.Add:");
                        output.AppendLine("                AddEntity();");
                        output.AppendLine("                break;");
                        output.AppendLine("            case PageState.Edit:");
                        output.AppendLine("                UpdateEntity();");
                        output.AppendLine("                MainGridView.EditIndex = -1;");
                        output.AppendLine("                break;");
                        output.AppendLine("            case PageState.Delete:");
                        output.AppendLine("                DeleteEntity();");
                        output.AppendLine("                break;");
                        output.AppendLine("            default:");
                        output.AppendLine("                break;");
                        output.AppendLine("        }");
                        output.AppendLine("        MainGridView.SelectedIndex = -1;");
                        output.AppendLine("    }");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInForeignKey && !column.IsInPrimaryKey)
                            {
                                string refKeyName = (column.ForeignKeys[0].PrimaryTable).Name;
                                output.AppendLine("    private void LoadCombo" + GetFormattedEntityName(column.Name) + "()");
                                output.AppendLine("    {");
                                output.AppendLine("         " + generationProject.Namespace + ".Business.Tables.dbo." + refKeyName + " entities = new " + generationProject.Namespace + ".Business.Tables.dbo." + refKeyName + "();");
                                output.AppendLine("         comboBox" + GetFormattedEntityName(column.Name) + ".DataValueField = " + generationProject.Namespace + ".Entities.Tables.dbo." + refKeyName + ".ColumnNames." + FindPkAutoNumericColumnName(column.ForeignKeys[0].PrimaryTable) + ";");
                                output.AppendLine("         comboBox" + GetFormattedEntityName(column.Name) + ".DataTextField = " + generationProject.Namespace + ".Entities.Tables.dbo." + refKeyName + ".ColumnNames." + FindFirstVarcharColumnName(column.ForeignKeys[0].PrimaryTable) + ";");
                                output.AppendLine("         comboBox" + GetFormattedEntityName(column.Name) + ".DataSource = entities.Items();");
                                output.AppendLine("         comboBox" + GetFormattedEntityName(column.Name) + ".DataBind();");
                                output.AppendLine("    }");
                            }
                        }
                        output.AppendLine("    protected void ShowException(System.Exception ex)");
                        output.AppendLine("    {");
                        output.AppendLine("        DataPanel.Visible = false;");
                        output.AppendLine("        DetailPanel.Visible = false;");
                        output.AppendLine("        ErrorPanel.Visible = true;");
                        output.AppendLine("        lblError.Text = ex.Message;");
                        output.AppendLine("    }");
                        output.AppendLine("    protected void btnAcceptException_Click(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("        DataPanel.Visible = true;");
                        output.AppendLine("        DetailPanel.Visible = false;");
                        output.AppendLine("        ErrorPanel.Visible = false;");
                        output.AppendLine("    }");
                        output.AppendLine("</script>");
                        output.AppendLine("");
                        output.AppendLine("<html xmlns=" + System.Convert.ToChar(34) + "http://www.w3.org/1999/xhtml" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("<head>");
                        output.AppendLine("    <meta charset=" + System.Convert.ToChar(34) + "utf-8" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <meta http-equiv=" + System.Convert.ToChar(34) + "X-UA-Compatible" + System.Convert.ToChar(34) + " content=" + System.Convert.ToChar(34) + "IE=edge" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <title> " + table.Description.Trim() == null ? table.Name : table.Description.Trim() + " </title>");
                        output.AppendLine("    <!-- Tell the browser to be responsive to screen width -->");
                        output.AppendLine("    <meta content=" + System.Convert.ToChar(34) + "width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" + System.Convert.ToChar(34) + " name=" + System.Convert.ToChar(34) + "viewport" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <!-- Bootstrap 3.3.5 -->");
                        output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.5/css/bootstrap.min.css" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <!-- Font Awesome -->");
                        output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://maxcdn.bootstrapcdn.com/font-awesome/4.4.0/css/font-awesome.min.css" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <!-- Ionicons -->");
                        output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://code.ionicframework.com/ionicons/2.0.1/css/ionicons.min.css" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <!-- Page Controls -->");
                        output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.6.0/css/bootstrap-datepicker3.css" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datetimepicker/4.17.37/css/bootstrap-datetimepicker.min.css" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <script src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/jquery/2.1.4/jquery.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <script src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.0.0-alpha/js/bootstrap.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <script src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.6.0/js/bootstrap-datepicker.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("");
                        output.AppendLine("    <!-- Character restriction (github.com/KevinSheedy/jquery.alphanum) -->");
                        output.AppendLine("    <script src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/jquery.alphanum/1.0.24/jquery.alphanum.min.js" + System.Convert.ToChar(34) + "></script>");


                        output.AppendLine("    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->");
                        output.AppendLine("    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->");
                        output.AppendLine("    <!--[if lt IE 9]>");
                        output.AppendLine("        <script src=" + System.Convert.ToChar(34) + "https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("        <script src=" + System.Convert.ToChar(34) + "https://oss.maxcdn.com/respond/1.4.2/respond.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <![endif]-->");
                        output.AppendLine("</head>");

                        output.AppendLine("<body >");
                        output.AppendLine("    <form id=" + System.Convert.ToChar(34) + "form1" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("        <div>");
                        output.AppendLine("");
                        output.AppendLine("            <asp:HiddenField ID=" + System.Convert.ToChar(34) + "hiddenState" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("            <!--  page-wrapper -->");
                        output.AppendLine("            <div id=" + System.Convert.ToChar(34) + "content" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                <div class=" + System.Convert.ToChar(34) + "content" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                    <div class=" + System.Convert.ToChar(34) + "panel panel-default" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                        <div class=" + System.Convert.ToChar(34) + "panel-heading" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                            " + table.Description);
                        output.AppendLine("                        </div>");
                        output.AppendLine("                        <div class=" + System.Convert.ToChar(34) + "panel-body" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                            <asp:Panel ID=" + System.Convert.ToChar(34) + "ErrorPanel" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "alert-danger" + System.Convert.ToChar(34) + " Visible=" + System.Convert.ToChar(34) + "false" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                <table style=" + System.Convert.ToChar(34) + "width:100%" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                    <tr style=" + System.Convert.ToChar(34) + "width:100%" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                        <td style=" + System.Convert.ToChar(34) + "width:10%" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                            <asp:Button ID=" + System.Convert.ToChar(34) + "btnAcceptException" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "btn btn-success btn-sm" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Aceptar" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                        </td>");
                        output.AppendLine("                                        <td style=" + System.Convert.ToChar(34) + "width:100%" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                            <asp:Label ID=" + System.Convert.ToChar(34) + "lblError" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "control-label col-md-3" + System.Convert.ToChar(34) + "></asp:Label>");
                        output.AppendLine("                                        </td>");
                        output.AppendLine("                                    </tr>");
                        output.AppendLine("                                </table>");
                        output.AppendLine("                            </asp:Panel>");

                        output.AppendLine("                            <asp:Panel ID=" + System.Convert.ToChar(34) + "DataPanel" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("");
                        output.AppendLine("                                <asp:GridView ID=" + System.Convert.ToChar(34) + "MainGridView" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " AutoGenerateColumns=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " AllowSorting=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " AllowPaging=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + "");
                        output.AppendLine("                                    UseAccessibleHeader=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " RowStyle-CssClass=" + System.Convert.ToChar(34) + "td" + System.Convert.ToChar(34) + " HeaderStyle-CssClass=" + System.Convert.ToChar(34) + "th" + System.Convert.ToChar(34) + " ShowFooter=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + "");
                        output.AppendLine("                                    DataKeyNames=" + System.Convert.ToChar(34) + refArguments.Substring(0, refArguments.Length - 1) + System.Convert.ToChar(34) + "");
                        output.AppendLine("                                    CssClass=" + System.Convert.ToChar(34) + "table table-striped table-bordered table-condensed table-hover" + System.Convert.ToChar(34) + " Visible=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                    <Columns>");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("                                        <asp:BoundField DataField=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " SortExpression=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "10%" + System.Convert.ToChar(34) + " ReadOnly=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " />");
                                switch (column.LanguageType)
                                {
                                    case "String":
                                        output.AppendLine("                                        <asp:BoundField DataField=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " SortExpression=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "10%" + System.Convert.ToChar(34) + " ReadOnly=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " />");
                                        break;
                                    case "DateTime":
                                        output.AppendLine("                                        <asp:TemplateField HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "75px" + System.Convert.ToChar(34) + ">");
                                        output.AppendLine("                                            <ItemTemplate>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "label" + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:Label>");
                                        output.AppendLine("                                            </ItemTemplate>");
                                        output.AppendLine("                                        </asp:TemplateField>");
                                        break;
                                }

                            }
                            else
                            {
                                switch (column.LanguageType)
                                {
                                    case "String":
                                        output.AppendLine("                                        <asp:BoundField DataField=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " SortExpression=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "10%" + System.Convert.ToChar(34) + " ReadOnly=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " />");
                                        break;
                                    case "DateTime":
                                        output.AppendLine("                                        <asp:TemplateField HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "75px" + System.Convert.ToChar(34) + ">");
                                        output.AppendLine("                                            <ItemTemplate>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "label" + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:Label>");
                                        output.AppendLine("                                            </ItemTemplate>");
                                        output.AppendLine("                                        </asp:TemplateField>");

                                        break;
                                    case "Boolean":
                                    case "Bool":
                                    case "boolean":
                                        output.AppendLine("                                        <asp:TemplateField HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "75px" + System.Convert.ToChar(34) + ">");
                                        output.AppendLine("                                            <ItemTemplate>");
                                        output.AppendLine("                                                <asp:CheckBox ID=" + System.Convert.ToChar(34) + "chk" + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Checked='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:CheckBox>");
                                        output.AppendLine("                                            </ItemTemplate>");
                                        output.AppendLine("                                        </asp:TemplateField>");
                                    
                                        break;

                                    default:
                                        if (!column.IsInPrimaryKey)
                                            output.AppendLine("                                        <asp:BoundField DataField=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " SortExpression=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "100px" + System.Convert.ToChar(34) + " />");
                                        break;
                                }
                            }
                        }
                        output.AppendLine("                                        <asp:TemplateField HeaderText=" + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "10%" + System.Convert.ToChar(34) + " ItemStyle-HorizontalAlign=" + System.Convert.ToChar(34) + "Center" + System.Convert.ToChar(34) + " HeaderStyle-HorizontalAlign=" + System.Convert.ToChar(34) + "Center" + System.Convert.ToChar(34) + " FooterStyle-HorizontalAlign=" + System.Convert.ToChar(34) + "Center" + System.Convert.ToChar(34) + " HeaderStyle-VerticalAlign=" + System.Convert.ToChar(34) + "Middle" + System.Convert.ToChar(34) + " HeaderStyle-Wrap=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                            <ItemTemplate>");
                        output.AppendLine("                                                <asp:Button class=" + System.Convert.ToChar(34) + "btn btn-primary btn-sm glyphicon-pencil " + System.Convert.ToChar(34) + " ID=" + System.Convert.ToChar(34) + "btnEditar" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Editar" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "text-align: center;" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnEdit_Click" + System.Convert.ToChar(34) + " CommandArgument='<%# Container.DisplayIndex %>' />");
                        output.AppendLine("                                                <asp:Button class=" + System.Convert.ToChar(34) + "btn btn-danger  btn-sm" + System.Convert.ToChar(34) + " ID=" + System.Convert.ToChar(34) + "btnBorrar" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Borrar" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "text-align: center;" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnDelete_Click" + System.Convert.ToChar(34) + " CommandArgument='<%# Container.DisplayIndex %>' />");
                        output.AppendLine("                                            </ItemTemplate>");
                        output.AppendLine("                                            <HeaderTemplate>");
                        output.AppendLine("                                                <asp:LinkButton CssClass=" + System.Convert.ToChar(34) + "btn btn-success btn-default" + System.Convert.ToChar(34) + " ID=" + System.Convert.ToChar(34) + "btnAddNew" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Nuevo registro" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "text-align: center;" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnAddNew_Click" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                                </asp:LinkButton>");
                        output.AppendLine("                                            </HeaderTemplate>");
                        output.AppendLine("                                        </asp:TemplateField>");
                        output.AppendLine("                                    </Columns>");
                        output.AppendLine("                                    <HeaderStyle HorizontalAlign=" + System.Convert.ToChar(34) + "Center" + System.Convert.ToChar(34) + " VerticalAlign=" + System.Convert.ToChar(34) + "Middle" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                    <PagerSettings Mode=" + System.Convert.ToChar(34) + "NumericFirstLast" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                    <PagerStyle CssClass=" + System.Convert.ToChar(34) + "bs-pagination" + System.Convert.ToChar(34) + " HorizontalAlign=" + System.Convert.ToChar(34) + "Center" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                    <EmptyDataRowStyle CssClass=" + System.Convert.ToChar(34) + "table-bordered" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                    <EmptyDataTemplate>");
                        output.AppendLine("                                      <div class=" + System.Convert.ToChar(34) + "callout callout-warning" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                        <h4>No existen datos en esta tabla.</h4>");
                        output.AppendLine("                                            <asp:LinkButton CssClass=" + System.Convert.ToChar(34) + "btn btn-success btn-default" + System.Convert.ToChar(34) + " ID=" + System.Convert.ToChar(34) + "btnEmptyAddNew" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Ingrese un nuevo registro" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "text-align: center;" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnAddNew_Click" + System.Convert.ToChar(34) + "></asp:LinkButton>");
                        output.AppendLine("                                      </div>");
                        output.AppendLine("                                    </EmptyDataTemplate>");

                        output.AppendLine("                                </asp:GridView>");
                        output.AppendLine("                            </asp:Panel>");
                        output.AppendLine("                        </div>");
                        output.AppendLine("");
                        output.AppendLine("                        <div class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                            <asp:Panel ID=" + System.Convert.ToChar(34) + "DetailPanel" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                <div class=" + System.Convert.ToChar(34) + "panel-body" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                    <table class=" + System.Convert.ToChar(34) + "table table-striped table-bordered table-hover" + System.Convert.ToChar(34) + ">");

                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("                                        <tr>");
                                output.AppendLine("                                            <%--Id (Identificador de Registro, campo " + GetFormattedEntityName(column.Name) + " )--%>");
                                output.AppendLine("                                            <td>");
                                output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name)  + System.Convert.ToChar(34) + "></asp:Label>");
                                output.AppendLine("                                            </td>");
                                output.AppendLine("                                            <td>");
                                output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + "Value" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:Label>");
                                output.AppendLine("                                            </td>");
                                output.AppendLine("                                            <td></td>");
                                output.AppendLine("                                        </tr>");
                            }
                            if (column.IsInForeignKey && !column.IsInPrimaryKey)
                            {
                                output.AppendLine("                                        <tr>");
                                output.AppendLine("                                            <%-- Description of:  " +  GetFormattedEntityName(column.Name) + " --%>");
                                output.AppendLine("                                            <td>");
                                output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) +  GetFormattedEntityName(column.Name)  + System.Convert.ToChar(34) + "></asp:Label>");
                                output.AppendLine("                                            </td>");
                                output.AppendLine("                                            <td>");
                                output.AppendLine("                                                <asp:DropDownList ID=" + System.Convert.ToChar(34) + "comboBox" + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " AutoPostBack=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control" + System.Convert.ToChar(34) + "/>");
                                output.AppendLine("                                            </td>");
                                output.AppendLine("                                            <td></td>");
                                output.AppendLine("                                        </tr>");

                            }
                            if (!column.IsInPrimaryKey && !column.IsInForeignKey)
                            {
                                switch (column.LanguageType)
                                {
                                    case "String":
                                        output.AppendLine("                                        <tr>");
                                        output.AppendLine("                                            <%-- Description of:  " + GetFormattedEntityName(column.Name) + " --%>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) +  "Edit" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></asp:Label>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:TextBox ID=" + System.Convert.ToChar(34) + "txt" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:TextBox>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td></td>");
                                        output.AppendLine("                                        </tr>");
                                        break;
                                    case "DateTime":
                                        output.AppendLine("                                        <tr>");
                                        output.AppendLine("                                            <%-- Description of:  " + GetFormattedEntityName(column.Name) + " --%>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ClientIdMode=" + System.Convert.ToChar(34) + "static" + System.Convert.ToChar(34) + "></asp:Label>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                 <div class=" + System.Convert.ToChar(34) + "input-group" + System.Convert.ToChar(34) + "><div class=" + System.Convert.ToChar(34) + "input-group-addon" + System.Convert.ToChar(34) + "><i class=" + System.Convert.ToChar(34) + "fa fa-calendar" + System.Convert.ToChar(34) + "></i></div><div>");
                                        output.AppendLine("                                                     <asp:TextBox ID=" + System.Convert.ToChar(34) + "txt" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "datepicker" + System.Convert.ToChar(34) + "  runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:TextBox>");
                                        output.AppendLine("                                                 </div></div>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                        </tr>");
                                        break;
                                    case "Boolean":
                                    case "Bool":
                                        output.AppendLine("                                        <tr>");
                                        output.AppendLine("                                            <%-- Description of:  " + GetFormattedEntityName(column.Name) + " --%>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ClientIdMode=" + System.Convert.ToChar(34) + "static" + System.Convert.ToChar(34) + "></asp:Label>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:CheckBox ID=" + System.Convert.ToChar(34) + "chk" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control date" + System.Convert.ToChar(34) + "  runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Checked='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:CheckBox>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                        </tr>");
                                        break;
                                    case "int":
                                    case "long":
                                    case "Int16":
                                    case "Int32":
                                    case "Int64":
                                        output.AppendLine("                                        <tr>");
                                        output.AppendLine("                                            <%-- Description of:  " + GetFormattedEntityName(column.Name) + " --%>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></asp:Label>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:TextBox ID=" + System.Convert.ToChar(34) + "txt" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control integer" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:TextBox>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td></td>");
                                        output.AppendLine("                                        </tr>");
                                        break;
                                    case "decimal":
                                    case "float":
                                    case "double":
                                    case "Decimal":
                                    case "Float":
                                    case "Double":

                                        output.AppendLine("                                        <tr>");
                                        output.AppendLine("                                            <%-- Description of:  " + GetFormattedEntityName(column.Name) + " --%>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></asp:Label>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:TextBox ID=" + System.Convert.ToChar(34) + "txt" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control decimal" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:TextBox>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td></td>");
                                        output.AppendLine("                                        </tr>");
                                        break;
                                      
                                    default:
                                        output.AppendLine("                                        <tr>");
                                        output.AppendLine("                                            <%-- Description of:  " + GetFormattedEntityName(column.Name) + " --%>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></asp:Label>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:TextBox ID=" + System.Convert.ToChar(34) + "txt" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:TextBox>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td></td>");
                                        output.AppendLine("                                        </tr>");                                    
                                        break;
                                }
                            }
                        }


                        output.AppendLine("                                    </table>");
                        output.AppendLine("                                    <asp:Button ID=" + System.Convert.ToChar(34) + "btnAccept" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "btn btn-success btn-sm" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Aceptar" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                    <asp:Button ID=" + System.Convert.ToChar(34) + "btnCancel" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "btn btn-danger btn-sm" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Cancelar" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                </div>");
                        output.AppendLine("                            </asp:Panel>");
                        //output.AppendLine("");
                        //output.AppendLine("                            <asp:Panel ID=" + System.Convert.ToChar(34) + "pnlModal" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "modal-dialog-center" + System.Convert.ToChar(34) + " Visible=" + System.Convert.ToChar(34) + "false" + System.Convert.ToChar(34) + "");
                        //output.AppendLine("                                style=" + System.Convert.ToChar(34) + "position:fixed;top:100px;bottom:0px;left:0px;");
                        //output.AppendLine("                                right:0px;overflow:hidden;padding:0;margin:0;z-index:1000;" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                <asp:Panel ID=" + System.Convert.ToChar(34) + "pnlModalDialog" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "modal-dialog" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                    <asp:Panel ID=" + System.Convert.ToChar(34) + "pnlModalContent" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "modal-content" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                        <asp:Panel ID=" + System.Convert.ToChar(34) + "pnlModalHeader" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "modal-header" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                            <button type=" + System.Convert.ToChar(34) + "button" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "close" + System.Convert.ToChar(34) + " data-dismiss=" + System.Convert.ToChar(34) + "modal" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                                <span aria-hidden=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + ">&times;</span><span class=" + System.Convert.ToChar(34) + "sr-only" + System.Convert.ToChar(34) + ">Close</span>");
                        //output.AppendLine("                                            </button>");
                        //output.AppendLine("                                            <asp:Label ID=" + System.Convert.ToChar(34) + "lblTitulo" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "modal-title" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Confirma?" + System.Convert.ToChar(34) + "></asp:Label>");
                        //output.AppendLine("                                            <asp:Panel ID=" + System.Convert.ToChar(34) + "pnlBody" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "modal-body" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                                <div class=" + System.Convert.ToChar(34) + "form-inline" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                                    <div class=" + System.Convert.ToChar(34) + "form-group" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                                        <asp:Label ID=" + System.Convert.ToChar(34) + "lblMensaje" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "control-label" + System.Convert.ToChar(34) + " Width=" + System.Convert.ToChar(34) + "120" + System.Convert.ToChar(34) + ">Confirma operación?</asp:Label>");
                        //output.AppendLine("                                                    </div>");
                        //output.AppendLine("                                                </div>");
                        //output.AppendLine("                                            </asp:Panel>");
                        //output.AppendLine("                                            <asp:Panel ID=" + System.Convert.ToChar(34) + "pnlFooter" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "modal-footer" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                                <asp:Button ID=" + System.Convert.ToChar(34) + "btnAceptar" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "btn btn-success" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnAccept_Click" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Aceptar" + System.Convert.ToChar(34) + "></asp:Button>");
                        //output.AppendLine("                                                <asp:Button ID=" + System.Convert.ToChar(34) + "btnCancelar" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "btn btn-danger" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnCancel_Click" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Cancelar" + System.Convert.ToChar(34) + "></asp:Button>");
                        //output.AppendLine("                                            </asp:Panel>");
                        //output.AppendLine("                                        </asp:Panel>");
                        //output.AppendLine("                                    </asp:Panel>");
                        //output.AppendLine("                                </asp:Panel>");
                        //output.AppendLine("                            </asp:Panel>");
                        //output.AppendLine("");
                        output.AppendLine("                        </div>");
                        output.AppendLine("                    </div>");
                        output.AppendLine("                </div>");
                        output.AppendLine("            </div>");
                        output.AppendLine("            <!-- end page-wrapper -->");
                        output.AppendLine("        </div>");
                        output.AppendLine("    </form>");
                        output.AppendLine("</body>");
                        output.AppendLine("</html>");
                        output.AppendLine("<script type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("    //datepicker");
                        output.AppendLine("    $('.datepicker').datepicker({");
                        output.AppendLine("        autoclose: true,");
                        output.AppendLine("        format: " + System.Convert.ToChar(34) + "yyyy-mm-dd" + System.Convert.ToChar(34) + ",");
                        output.AppendLine("        todayHighlight: true,");
                        //output.AppendLine("        //orientation: " + System.Convert.ToChar(34) + "top auto" + System.Convert.ToChar(34) + ",");
                        output.AppendLine("        todayBtn: true,");
                        output.AppendLine("    });");
                        output.AppendLine("");
                        output.AppendLine("    $(document).ready(function () {");
                        output.AppendLine("        $('.bs-pagination td table').each(function (index, obj) {");
                        output.AppendLine("            convertToPagination(obj)");
                        output.AppendLine("        });");
                        output.AppendLine("    });");
                        output.AppendLine("");
                        output.AppendLine("    function convertToPagination(obj) {");
                        output.AppendLine("        var liststring = '<ul class=" + System.Convert.ToChar(34) + "pagination" + System.Convert.ToChar(34) + ">';");
                        output.AppendLine("");
                        output.AppendLine("        $(obj).find(" + System.Convert.ToChar(34) + "tbody tr" + System.Convert.ToChar(34) + ").each(function () {");
                        output.AppendLine("            $(this).children().map(function () {");
                        output.AppendLine("                liststring = liststring + " + System.Convert.ToChar(34) + "<li>" + System.Convert.ToChar(34) + " + $(this).html() + " + System.Convert.ToChar(34) + "</li>" + System.Convert.ToChar(34) + ";");
                        output.AppendLine("            });");
                        output.AppendLine("        });");
                        output.AppendLine("        liststring = liststring + " + System.Convert.ToChar(34) + "</ul>" + System.Convert.ToChar(34) + ";");
                        output.AppendLine("        var list = $(liststring);");
                        output.AppendLine("        list.find('span').parent().addClass('active');");
                        output.AppendLine("");
                        output.AppendLine("        $(obj).replaceWith(list);");
                        output.AppendLine("    }");
                        output.AppendLine("");
                        output.AppendLine("$(" + System.Convert.ToChar(34) + ".integer" + System.Convert.ToChar(34) + ").numeric(" + System.Convert.ToChar(34) + "integer" + System.Convert.ToChar(34) + ");");

                        output.AppendLine("$(" + System.Convert.ToChar(34) + ".decimal" + System.Convert.ToChar(34) + ").alphanum({allowPlus: false, allowMinus: true,allowThouSep: false,allowDecSep:  true});");


						output.AppendLine("</script>");
                        
                        SaveOutputToFile(table.Name + ".aspx", output, true);
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
        public bool Execute(ref MyMeta.IDatabase db, string workingDir, GenerationProject generationProject)
        {
            try
            {
                _workingdir = workingDir;
                MyMeta.IDatabase _dataBase = db;

                db.Root.DbTarget = GetDbTarget(db.Root.Driver);
                db.Root.Language = GetLanguage(db.Root.Driver);//"C# Types";




                foreach (MyMeta.ITable table in db.Tables)
                {
                    if (table.Selected)
                    {
                        System.Text.StringBuilder output = new System.Text.StringBuilder();

                        //if (OnInfo != null)
                        //{
                        //    OnInfo("Building " + table.Name + "...");
                        //}


                        output.AppendLine("<%@ Page Language=" + System.Convert.ToChar(34) + "C#" + System.Convert.ToChar(34) + " %>");
                        output.AppendLine("");
                        output.AppendLine("<!DOCTYPE html>");
                        output.AppendLine("<script runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("");
                        output.AppendLine("    public enum PageState");
                        output.AppendLine("    {");
                        output.AppendLine("        ReadOnly,");
                        output.AppendLine("        Add,");
                        output.AppendLine("        Edit,");
                        output.AppendLine("        Delete");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Maps events to handlers and loads data ");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    protected void Page_Load(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        //output.AppendLine("");
                        //output.AppendLine("        if (Request.UrlReferrer==null)");
                        //output.AppendLine("            Response.Redirect(" + System.Convert.ToChar(34) + "http://localhost:3868/Default.aspx" + System.Convert.ToChar(34) + ");");
                        output.AppendLine("");
                        output.AppendLine("         " + GetFormattedEntityName(table.Name) + "GridView.RowEditing +=  " + GetFormattedEntityName(table.Name) + "GridView_RowEditing;");
                        output.AppendLine("         " + GetFormattedEntityName(table.Name) + "GridView.RowDeleting +=  " + GetFormattedEntityName(table.Name) + "GridView_RowDeleting;");
                        output.AppendLine("         " + GetFormattedEntityName(table.Name) + "GridView.PageIndexChanging +=  " + GetFormattedEntityName(table.Name) + "GridView_PageIndexChanging;");
                        output.AppendLine("         " + GetFormattedEntityName(table.Name) + "GridView.Sorting +=  " + GetFormattedEntityName(table.Name) + "GridView_Sorting;");
                        //output.AppendLine("        //btnAddNew.Click += btnAddNew_Click;");
                        output.AppendLine("        btnAccept.Click += btnAccept_Click;");
                        output.AppendLine("        btnCancel.Click += btnCancel_Click;");
                        output.AppendLine("        btnAcceptException.Click += btnAcceptException_Click;");
                        output.AppendLine("");
                        output.AppendLine("        if (!IsPostBack)");
                        output.AppendLine("        {");
                        output.AppendLine("            DataPanel.Visible = true;");
                        output.AppendLine("            DetailPanel.Visible = false;");
                        output.AppendLine("            this.hiddenState.Value = ((int)PageState.ReadOnly).ToString();");
                        output.AppendLine("            Load" + GetFormattedEntityName(table.Name) + "GridViewData();");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInForeignKey && !column.IsInPrimaryKey)
                            {
                                string refKeyName = GetFormattedEntityName(column.ForeignKeys[0].ForeignTable.Name);
                                output.AppendLine("            LoadCombo" + GetFormattedEntityName(column.Name) + "();");
                            }
                        }
                        output.AppendLine("");
                        output.AppendLine("        }");
                        output.AppendLine("        LoadTitles();");
                        output.AppendLine("    }");
                        output.AppendLine("    private void LoadTitles()");
                        output.AppendLine("    {");
                        output.AppendLine("        //Int64 languageId = 1; //Default es-AR");
                        output.AppendLine("        //if (SessionFacade.ActiveUSer != null)");
                        output.AppendLine("        //  languageId = SessionFacade.ActiveUSer.LanguageId;");
                        output.AppendLine("        //lblMainTitle.Text = Functions.GetTitle(Path.GetFileName(Request.PhysicalPath), lblMainTitle.ID, languageId);");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Sorts Gridview data based on column click");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    void  " + GetFormattedEntityName(table.Name) + "GridView_Sorting(object sender, GridViewSortEventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Changes Gridview page index");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    void  " + GetFormattedEntityName(table.Name) + "GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("         " + GetFormattedEntityName(table.Name) + "GridView.PageIndex = e.NewPageIndex;");
                        output.AppendLine("        Load" + GetFormattedEntityName(table.Name) + "GridViewData();");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Used to add a new entity");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    protected void btnAddNew_Click(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("        SetMode(PageState.Add);");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Cancels operation");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    void btnCancel_Click(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("        SetMode(PageState.ReadOnly);");
                        output.AppendLine("        ClearControls();");
                        output.AppendLine("        Load" + GetFormattedEntityName(table.Name) + "GridViewData();");
                        output.AppendLine("         " + GetFormattedEntityName(table.Name) + "GridView.SelectedIndex = -1;");
                        output.AppendLine("    }");
                        //output.AppendLine("    /// <summary>");
                        //output.AppendLine("    /// Accepts current operation");
                        //output.AppendLine("    /// </summary>");
                        //output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        //output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        //output.AppendLine("    void btnAceptar_Click(object sender, EventArgs e)");
                        //output.AppendLine("    {");
                        //output.AppendLine("        pnlModal.Visible = true;");
                        //output.AppendLine("    }");


                        string refParameters = string.Empty;
                        string refArguments = string.Empty;
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                refParameters += column.LanguageType + " " + GetFormattedEntityName(column.Name) + ",";
                                refArguments += GetFormattedEntityName(column.Name) + ",";
                            }
                        }
                        if (refParameters.Length == 0)
                            refParameters += " ";
                        if (refArguments.Length == 0)
                            refArguments += " ";

                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Fires when user clicks " + System.Convert.ToChar(34) + "Delete" + System.Convert.ToChar(34) + "");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    void  " + GetFormattedEntityName(table.Name) + "GridView_RowDeleting(object sender, GridViewDeleteEventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("        SetMode(PageState.Delete);");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("         " + column.LanguageType + " " + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "( " + GetFormattedEntityName(table.Name) + "GridView.DataKeys[e.RowIndex].Values[" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "].ToString());");
                            }
                        }

                        output.AppendLine("        LoadUIFromEntity(" + refArguments.Substring(0, refArguments.Length - 1) + ");");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Fires when user clicks " + System.Convert.ToChar(34) + "Edit" + System.Convert.ToChar(34) + "");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "sender" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "e" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    void  " + GetFormattedEntityName(table.Name) + "GridView_RowEditing(object sender, GridViewEditEventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("        SetMode(PageState.Edit);");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("         " + column.LanguageType + " " + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "( " + GetFormattedEntityName(table.Name) + "GridView.DataKeys[e.NewEditIndex].Values[" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "].ToString());");
                            }
                        }

                        output.AppendLine("        LoadUIFromEntity(" + refArguments.Substring(0, refArguments.Length - 1) + ");");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Loads Detail data into edit controls in UI ");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "rowIndex" + System.Convert.ToChar(34) + "></param>");


                        output.AppendLine("    private void LoadUIFromEntity(" + refParameters.Substring(0, refParameters.Length - 1) + ")");
                        output.AppendLine("    {");
                        output.AppendLine("         try");
                        output.AppendLine("         {");
                        output.AppendLine("             " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " entity = new " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("             " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("");

                        output.AppendLine("             entity = entities.Items(" + refArguments.Substring(0, refArguments.Length - 1) + ").FirstOrDefault();");

                        

                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            String columnName = GetFormattedEntityName(column.Name);

                            if (column.IsInPrimaryKey)
                                output.AppendLine("             lbl" + columnName + "Value.Text = entity." + columnName + " == null ? String.Empty : entity." + columnName + ".ToString();");
                            else
                            {
                                if (column.IsInForeignKey)
                                    output.AppendLine("             comboBox" + columnName + ".SelectedValue = entity." + columnName + " == null ? String.Empty : entity." + columnName + ".ToString();");
                                else
                                    switch (column.LanguageType)
                                    {
                                        case "String":
                                        case "DateTime":
                                            output.AppendLine("             txt" + columnName + "Edit.Text = entity." + columnName + " == null ? String.Empty : entity." + columnName + ".ToString();");
                                            break;
                                        case "Boolean":
                                        case "Bool":
                                            output.AppendLine("             chk" + columnName + "Edit.Checked = entity." + columnName + " == null ? false : entity." + columnName + ";");
                                            break;
                                        default:
                                            output.AppendLine("             txt" + columnName + "Edit.Text = entity." + columnName + " == null ? String.Empty : entity." + columnName + ".ToString();");
                                            break;
                                    }
                            }
                        }
                        output.AppendLine("         }");
                        output.AppendLine("         catch (Exception ex)");
                        output.AppendLine("         {");
                        output.AppendLine("             ShowException(ex);");
                        output.AppendLine("         }");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Loads Detail data into edit controls in UI ");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "rowIndex" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    private void LoadEntityFromUI(" + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " entity)");
                        output.AppendLine("    {");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                                output.AppendLine("        entity." + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(lbl" + GetFormattedEntityName(column.Name) + "Value.Text);");
                            else
                                if (column.IsInForeignKey)
                                    output.AppendLine("        entity." + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(comboBox" + GetFormattedEntityName(column.Name) + ".SelectedValue.ToString());");
                                else
                                {
                                    switch (column.LanguageType)
                                    {
                                        case "String":
                                        case "DateTime":
                                            output.AppendLine("        entity." + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(txt" + GetFormattedEntityName(column.Name) + "Edit.Text);");
                                            break;
                                        case "Boolean":
                                        case "Bool":
                                            output.AppendLine("        entity." + GetFormattedEntityName(column.Name) + " = chk" + GetFormattedEntityName(column.Name) + "Edit.Checked;");
                                            break;
                                    }
                                }

                        }
                        output.AppendLine("    }");
                        output.AppendLine("");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Adds new entity.");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void AddEntity()");
                        output.AppendLine("    {");
                        output.AppendLine("     try");
                        output.AppendLine("         {");
                        output.AppendLine("             " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " entity = new " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("             " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                if (!column.IsAutoKey)
                                {
                                    output.AppendLine("             entity." + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(lbl" + GetFormattedEntityName(column.Name) + "Value.Text);");
                                }
                            }
                            else
                                if (column.IsInForeignKey)
                                    output.AppendLine("             entity." + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(comboBox" + GetFormattedEntityName(column.Name) + ".SelectedValue.ToString());");
                                else
                                {
                                    switch (column.LanguageType)
                                    {
                                        case "String":
                                        case "DateTime":
                                            output.AppendLine("        entity." + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "(txt" + GetFormattedEntityName(column.Name) + "Edit.Text);");
                                            break;
                                        case "Boolean":
                                        case "Bool":
                                            output.AppendLine("        entity." + GetFormattedEntityName(column.Name) + " = chk" + GetFormattedEntityName(column.Name) + "Edit.Checked;");
                                            break;

                                    }
                                }
                        }

                        output.AppendLine("             entities.Add(entity);");
                        output.AppendLine("");
                        output.AppendLine("             DataPanel.Visible = true;");
                        output.AppendLine("             DetailPanel.Visible = false;");
                        output.AppendLine("             Load" + GetFormattedEntityName(table.Name) + "GridViewData();");
                        output.AppendLine("         }");
                        output.AppendLine("         catch (Exception ex)");
                        output.AppendLine("         {");
                        output.AppendLine("             ShowException(ex);");
                        output.AppendLine("         }");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Updates current entity");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void UpdateEntity()");
                        output.AppendLine("    {");
                        output.AppendLine("        " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " entity = new " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("        " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("");
                        output.AppendLine("        LoadEntityFromUI(entity);");
                        output.AppendLine("        entities.Update(entity);");
                        output.AppendLine("");
                        output.AppendLine("        DataPanel.Visible = true;");
                        output.AppendLine("        DetailPanel.Visible = false;");
                        output.AppendLine("        Load" + GetFormattedEntityName(table.Name) + "GridViewData();");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Deletes current entity");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    private void DeleteEntity()");
                        output.AppendLine("    {");
                        output.AppendLine("        " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + " entity = new " + generationProject.Namespace + ".Entities.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("        " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("        LoadEntityFromUI(entity);");
                        output.AppendLine("        entities.Delete(entity);");
                        output.AppendLine("        DataPanel.Visible = true;");
                        output.AppendLine("        DetailPanel.Visible = false;");
                        output.AppendLine("        Load" + GetFormattedEntityName(table.Name) + "GridViewData();");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Loads data from datasource into Gridview");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <returns></returns>");
                        output.AppendLine("    private Int32 Load" + GetFormattedEntityName(table.Name) + "GridViewData()");
                        output.AppendLine("    {");
                        output.AppendLine("        try");
                        output.AppendLine("        {");
                        output.AppendLine("             " + GetFormattedEntityName(table.Name) + "GridView.EditIndex = -1;");
                        output.AppendLine("             " + GetFormattedEntityName(table.Name) + "GridView.SelectedIndex = -1;");
                        output.AppendLine("            " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + table.Schema + "." + table.Name + "();");
                        output.AppendLine("             " + GetFormattedEntityName(table.Name) + "GridView.DataSource = entities.Items();");
                        output.AppendLine("             " + GetFormattedEntityName(table.Name) + "GridView.DataBind();");
                        output.AppendLine("            return  " + GetFormattedEntityName(table.Name) + "GridView.Rows.Count;");
                        output.AppendLine("        }");
                        output.AppendLine("        catch (Exception ex)");
                        output.AppendLine("        {");
                        output.AppendLine("            throw ex;");
                        output.AppendLine("        }");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Sets current mode operation");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "mode" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    private void SetMode(PageState mode)");
                        output.AppendLine("    {");
                        output.AppendLine("        switch (mode)");
                        output.AppendLine("        {");
                        output.AppendLine("            case PageState.ReadOnly:");
                        output.AppendLine("                DataPanel.Visible = true;");
                        output.AppendLine("                DetailPanel.Visible = false;");
                        output.AppendLine("                EnableControls(false);");
                        output.AppendLine("                this.hiddenState.Value = ((int)PageState.ReadOnly).ToString();");
                        output.AppendLine("                break;");
                        output.AppendLine("            case PageState.Add:");
                        output.AppendLine("                DataPanel.Visible = false;");
                        output.AppendLine("                DetailPanel.Visible = true;");
                        output.AppendLine("                ClearControls();");
                        output.AppendLine("                EnableControls(true);");
                        output.AppendLine("                this.hiddenState.Value = ((int)PageState.Add).ToString();");
                        output.AppendLine("                break;");
                        output.AppendLine("            case PageState.Edit:");
                        output.AppendLine("                DataPanel.Visible = false;");
                        output.AppendLine("                DetailPanel.Visible = true;");
                        output.AppendLine("                EnableControls(true);");
                        output.AppendLine("                this.hiddenState.Value = ((int)PageState.Edit).ToString();");
                        output.AppendLine("                break;");
                        output.AppendLine("            case PageState.Delete:");
                        output.AppendLine("                DataPanel.Visible = false;");
                        output.AppendLine("                DetailPanel.Visible = true;");
                        output.AppendLine("                EnableControls(false);");
                        output.AppendLine("                this.hiddenState.Value = ((int)PageState.Delete).ToString();");
                        output.AppendLine("                break;");
                        output.AppendLine("            default:");
                        output.AppendLine("                break;");
                        output.AppendLine("        }");
                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Enables or disables data controls");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    /// <param name=" + System.Convert.ToChar(34) + "value" + System.Convert.ToChar(34) + "></param>");
                        output.AppendLine("    void EnableControls(bool value)");
                        output.AppendLine("    {");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                                output.AppendLine("        lbl" + GetFormattedEntityName(column.Name) + "Value.Enabled = value;");
                            else
                                if (column.IsInForeignKey)
                                    output.AppendLine("        comboBox" + GetFormattedEntityName(column.Name) + ".Enabled = value;");
                                else
                                {
                                    switch (column.LanguageType)
                                    {
                                        case "String":
                                        case "DateTime":
                                            output.AppendLine("        txt" + GetFormattedEntityName(column.Name) + "Edit.Enabled = value;");
                                            break;
                                        case "Boolean":
                                        case "Bool":
                                            output.AppendLine("        chk" + GetFormattedEntityName(column.Name) + "Edit.Enabled = value;");
                                            break;
                                    }
                                }
                        }

                        output.AppendLine("    }");
                        output.AppendLine("    /// <summary>");
                        output.AppendLine("    /// Clears data controls");
                        output.AppendLine("    /// </summary>");
                        output.AppendLine("    void ClearControls()");
                        output.AppendLine("    {");

                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                                output.AppendLine("        lbl" + GetFormattedEntityName(column.Name) + "Value.Text = string.Empty;");
                            else
                                if (column.IsInForeignKey)
                                    output.AppendLine("        comboBox" + GetFormattedEntityName(column.Name) + ".SelectedIndex = -1;");
                                else
                                {
                                    switch (column.LanguageType)
                                    {
                                        case "String":
                                        case "DateTime":
                                            output.AppendLine("        txt" + GetFormattedEntityName(column.Name) + "Edit.Text = string.Empty;");
                                            break;
                                        case "Boolean":
                                        case "Bool":
                                            output.AppendLine("        chk" + GetFormattedEntityName(column.Name) + "Edit.Checked = false;");
                                            break;

                                    }
                                }
                        }

                        output.AppendLine("    }");
                        output.AppendLine("");
                        output.AppendLine("    protected void btnEdit_Click(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("        Int16 rowIndex = Convert.ToInt16(((Button)sender).CommandArgument);");
                        string refKeys = string.Empty;
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("         " + column.LanguageType + " " + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "( " + GetFormattedEntityName(table.Name) + "GridView.DataKeys[rowIndex].Values[" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "].ToString());");
                            }
                        }
                        //output.AppendLine("        " + refKeys);
                        output.AppendLine("        SetMode(PageState.Edit);");
                        output.AppendLine("        LoadUIFromEntity(" + refArguments.Substring(0, refArguments.Length - 1) + ");");
                        output.AppendLine("    }");
                        output.AppendLine("    protected void btnDelete_Click(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        //output.AppendLine("        " + refKeys);
                        output.AppendLine("        Int16 rowIndex = Convert.ToInt16(((Button)sender).CommandArgument);");

                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("         " + column.LanguageType + " " + GetFormattedEntityName(column.Name) + " = Convert.To" + column.LanguageType + "( " + GetFormattedEntityName(table.Name) + "GridView.DataKeys[rowIndex].Values[" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "].ToString());");
                            }
                        }
                        output.AppendLine("        SetMode(PageState.Delete);");
                        output.AppendLine("        LoadUIFromEntity(" + refArguments.Substring(0, refArguments.Length - 1) + ");");
                        output.AppendLine("    }");
                        output.AppendLine("");
                        output.AppendLine("    protected void btnAccept_Click(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        //output.AppendLine("        pnlModal.Visible = false;");
                        output.AppendLine("        int value = Convert.ToInt32(this.hiddenState.Value);");
                        output.AppendLine("        switch ((PageState)value)");
                        output.AppendLine("        {");
                        output.AppendLine("            case PageState.ReadOnly:");
                        output.AppendLine("                break;");
                        output.AppendLine("            case PageState.Add:");
                        output.AppendLine("                AddEntity();");
                        output.AppendLine("                break;");
                        output.AppendLine("            case PageState.Edit:");
                        output.AppendLine("                UpdateEntity();");
                        output.AppendLine("                 " + GetFormattedEntityName(table.Name) + "GridView.EditIndex = -1;");
                        output.AppendLine("                break;");
                        output.AppendLine("            case PageState.Delete:");
                        output.AppendLine("                DeleteEntity();");
                        output.AppendLine("                break;");
                        output.AppendLine("            default:");
                        output.AppendLine("                break;");
                        output.AppendLine("        }");
                        output.AppendLine("         " + GetFormattedEntityName(table.Name) + "GridView.SelectedIndex = -1;");
                        output.AppendLine("    }");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInForeignKey && !column.IsInPrimaryKey)
                            {
                                string refKeyName = (column.ForeignKeys[0].PrimaryTable).Name;
                                output.AppendLine("    private void LoadCombo" + GetFormattedEntityName(column.Name) + "()");
                                output.AppendLine("    {");
                                output.AppendLine("         " + generationProject.Namespace + ".Business.Tables.dbo." + refKeyName + " entities = new " + generationProject.Namespace + ".Business.Tables.dbo." + refKeyName + "();");
                                output.AppendLine("         comboBox" + GetFormattedEntityName(column.Name) + ".DataValueField = " + generationProject.Namespace + ".Entities.Tables.dbo." + refKeyName + ".ColumnNames." + FindPkAutoNumericColumnName(column.ForeignKeys[0].PrimaryTable) + ";");
                                output.AppendLine("         comboBox" + GetFormattedEntityName(column.Name) + ".DataTextField = " + generationProject.Namespace + ".Entities.Tables.dbo." + refKeyName + ".ColumnNames." + FindFirstVarcharColumnName(column.ForeignKeys[0].PrimaryTable) + ";");
                                output.AppendLine("         comboBox" + GetFormattedEntityName(column.Name) + ".DataSource = entities.Items();");
                                output.AppendLine("         comboBox" + GetFormattedEntityName(column.Name) + ".DataBind();");
                                output.AppendLine("    }");
                            }
                        }
                        output.AppendLine("    protected void ShowException(System.Exception ex)");
                        output.AppendLine("    {");
                        output.AppendLine("        DataPanel.Visible = false;");
                        output.AppendLine("        DetailPanel.Visible = false;");
                        output.AppendLine("        ErrorPanel.Visible = true;");
                        output.AppendLine("        lblError.Text = ex.Message;");
                        output.AppendLine("    }");
                        output.AppendLine("    protected void btnAcceptException_Click(object sender, EventArgs e)");
                        output.AppendLine("    {");
                        output.AppendLine("        DataPanel.Visible = true;");
                        output.AppendLine("        DetailPanel.Visible = false;");
                        output.AppendLine("        ErrorPanel.Visible = false;");
                        output.AppendLine("    }");
                        output.AppendLine("</script>");
                        output.AppendLine("");
                        output.AppendLine("<html xmlns=" + System.Convert.ToChar(34) + "http://www.w3.org/1999/xhtml" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("<head>");
                        output.AppendLine("    <meta charset=" + System.Convert.ToChar(34) + "utf-8" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <meta http-equiv=" + System.Convert.ToChar(34) + "X-UA-Compatible" + System.Convert.ToChar(34) + " content=" + System.Convert.ToChar(34) + "IE=edge" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <title>| </title>");
                        output.AppendLine("    <!-- Tell the browser to be responsive to screen width -->");
                        output.AppendLine("    <meta content=" + System.Convert.ToChar(34) + "width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" + System.Convert.ToChar(34) + " name=" + System.Convert.ToChar(34) + "viewport" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <!-- Bootstrap 3.3.5 -->");
                        output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.5/css/bootstrap.min.css" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <!-- Font Awesome -->");
                        output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://maxcdn.bootstrapcdn.com/font-awesome/4.4.0/css/font-awesome.min.css" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <!-- Ionicons -->");
                        output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://code.ionicframework.com/ionicons/2.0.1/css/ionicons.min.css" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <!-- Page Controls -->");
                        output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.6.0/css/bootstrap-datepicker3.css" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datetimepicker/4.17.37/css/bootstrap-datetimepicker.min.css" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("    <script src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/jquery/2.1.4/jquery.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <script src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.0.0-alpha/js/bootstrap.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <script src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.6.0/js/bootstrap-datepicker.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("");
                        output.AppendLine("    <!-- Character restriction (github.com/KevinSheedy/jquery.alphanum) -->");
                        output.AppendLine("    <script src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/jquery.alphanum/1.0.24/jquery.alphanum.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <!-- DateRangePicker -->");
                        output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-daterangepicker/2.1.19/daterangepicker.css" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("");
                        output.AppendLine("    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->");
                        output.AppendLine("    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->");
                        output.AppendLine("    <!--[if lt IE 9]>");
                        output.AppendLine("        <script src=" + System.Convert.ToChar(34) + "https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("        <script src=" + System.Convert.ToChar(34) + "https://oss.maxcdn.com/respond/1.4.2/respond.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <![endif]-->");
                        output.AppendLine("    <script type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + " src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.11.2/moment.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <script type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + " src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datetimepicker/4.17.37/js/bootstrap-datetimepicker.min.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("    <!-- DateRangePicker -->");
                        output.AppendLine("    <script type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + " src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-daterangepicker/2.1.19/daterangepicker.js" + System.Convert.ToChar(34) + "></script>");
                        output.AppendLine("");


                        output.AppendLine("</head>");

                        output.AppendLine("<body >");
                        output.AppendLine("    <form id=" + System.Convert.ToChar(34) + "form1" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("        <div>");
                        output.AppendLine("");
                        output.AppendLine("            <asp:HiddenField ID=" + System.Convert.ToChar(34) + "hiddenState" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("            <!--  page-wrapper -->");
                        output.AppendLine("            <div id=" + System.Convert.ToChar(34) + "content" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                <div class=" + System.Convert.ToChar(34) + "content" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                    <div class=" + System.Convert.ToChar(34) + "panel panel-default" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                        <div class=" + System.Convert.ToChar(34) + "panel-heading" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                            " + table.Description);
                        output.AppendLine("                        </div>");
                        output.AppendLine("                        <div class=" + System.Convert.ToChar(34) + "panel-body" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                            <asp:Panel ID=" + System.Convert.ToChar(34) + "ErrorPanel" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "alert-danger" + System.Convert.ToChar(34) + " Visible=" + System.Convert.ToChar(34) + "false" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                <table style=" + System.Convert.ToChar(34) + "width:100%" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                    <tr style=" + System.Convert.ToChar(34) + "width:100%" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                        <td style=" + System.Convert.ToChar(34) + "width:10%" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                            <asp:Button ID=" + System.Convert.ToChar(34) + "btnAcceptException" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "btn btn-success btn-sm" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Aceptar" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                        </td>");
                        output.AppendLine("                                        <td style=" + System.Convert.ToChar(34) + "width:100%" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                            <asp:Label ID=" + System.Convert.ToChar(34) + "lblError" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "control-label col-md-3" + System.Convert.ToChar(34) + "></asp:Label>");
                        output.AppendLine("                                        </td>");
                        output.AppendLine("                                    </tr>");
                        output.AppendLine("                                </table>");
                        output.AppendLine("                            </asp:Panel>");

                        output.AppendLine("                            <asp:Panel ID=" + System.Convert.ToChar(34) + "DataPanel" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("");
                        output.AppendLine("                                <asp:GridView ID=" + System.Convert.ToChar(34) + GetFormattedEntityName(table.Name) + "GridView" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " AutoGenerateColumns=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + " AllowSorting=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " AllowPaging=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + "");
                        output.AppendLine("                                    UseAccessibleHeader=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " RowStyle-CssClass=" + System.Convert.ToChar(34) + "td" + System.Convert.ToChar(34) + " HeaderStyle-CssClass=" + System.Convert.ToChar(34) + "th" + System.Convert.ToChar(34) + " ShowFooter=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + "");
                        output.AppendLine("                                    DataKeyNames=" + System.Convert.ToChar(34) + refArguments.Substring(0, refArguments.Length - 1) + System.Convert.ToChar(34) + "");
                        output.AppendLine("                                    CssClass=" + System.Convert.ToChar(34) + "table table-striped table-bordered table-condensed table-hover" + System.Convert.ToChar(34) + " Visible=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                    <Columns>");
                        //output.AppendLine("                                        <asp:TemplateField HeaderText=" + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "10%" + System.Convert.ToChar(34) + " ItemStyle-HorizontalAlign=" + System.Convert.ToChar(34) + "Center" + System.Convert.ToChar(34) + " HeaderStyle-HorizontalAlign=" + System.Convert.ToChar(34) + "Center" + System.Convert.ToChar(34) + " FooterStyle-HorizontalAlign=" + System.Convert.ToChar(34) + "Center" + System.Convert.ToChar(34) + " HeaderStyle-VerticalAlign=" + System.Convert.ToChar(34) + "Middle" + System.Convert.ToChar(34) + " HeaderStyle-Wrap=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                            <ItemTemplate>");
                        //output.AppendLine("                                                <asp:Button class=" + System.Convert.ToChar(34) + "btn btn-primary btn-sm glyphicon-pencil " + System.Convert.ToChar(34) + " ID=" + System.Convert.ToChar(34) + "btnEditRow" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Edit" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "text-align: center;" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnEdit_Click" + System.Convert.ToChar(34) + " CommandArgument='<%# Container.DisplayIndex %>' />");
                        //output.AppendLine("                                                <asp:Button class=" + System.Convert.ToChar(34) + "btn btn-danger  btn-sm" + System.Convert.ToChar(34) + " ID=" + System.Convert.ToChar(34) + "btnDeleteRow" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Delete" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "text-align: center;" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnDelete_Click" + System.Convert.ToChar(34) + " CommandArgument='<%# Container.DisplayIndex %>' />");
                        //output.AppendLine("                                            </ItemTemplate>");
                        //output.AppendLine("                                            <HeaderTemplate>");
                        //output.AppendLine("                                                <asp:LinkButton CssClass=" + System.Convert.ToChar(34) + "btn btn-success btn-default" + System.Convert.ToChar(34) + " ID=" + System.Convert.ToChar(34) + "btnAddNew" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Nuevo registro" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "text-align: center;" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnAddNew_Click" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                                </asp:LinkButton>");
                        //output.AppendLine("                                            </HeaderTemplate>");
                        //output.AppendLine("                                        </asp:TemplateField>");

                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("                                        <asp:BoundField DataField=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " SortExpression=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "10%" + System.Convert.ToChar(34) + " ReadOnly=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " />");
                                switch (column.LanguageType)
                                {
                                    case "String":
                                        output.AppendLine("                                        <asp:BoundField DataField=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " SortExpression=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "10%" + System.Convert.ToChar(34) + " ReadOnly=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " />");
                                        break;
                                    case "DateTime":
                                        output.AppendLine("                                        <asp:TemplateField HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "75px" + System.Convert.ToChar(34) + ">");
                                        output.AppendLine("                                            <ItemTemplate>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "label" + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:Label>");
                                        output.AppendLine("                                            </ItemTemplate>");
                                        output.AppendLine("                                        </asp:TemplateField>");
                                        break;
                                }

                            }
                            else
                            {
                                switch (column.LanguageType)
                                {
                                    case "String":
                                        output.AppendLine("                                        <asp:BoundField DataField=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " SortExpression=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "10%" + System.Convert.ToChar(34) + " ReadOnly=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + " />");
                                        break;
                                    case "DateTime":
                                        output.AppendLine("                                        <asp:TemplateField HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "75px" + System.Convert.ToChar(34) + ">");
                                        output.AppendLine("                                            <ItemTemplate>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "label" + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:Label>");
                                        output.AppendLine("                                            </ItemTemplate>");
                                        output.AppendLine("                                        </asp:TemplateField>");

                                        break;
                                    case "Boolean":
                                    case "Bool":
                                    case "boolean":
                                        output.AppendLine("                                        <asp:TemplateField HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "75px" + System.Convert.ToChar(34) + ">");
                                        output.AppendLine("                                            <ItemTemplate>");
                                        output.AppendLine("                                                <asp:CheckBox ID=" + System.Convert.ToChar(34) + "chk" + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Checked='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:CheckBox>");
                                        output.AppendLine("                                            </ItemTemplate>");
                                        output.AppendLine("                                        </asp:TemplateField>");

                                        break;

                                    default:
                                        if (!column.IsInPrimaryKey)
                                            output.AppendLine("                                        <asp:BoundField DataField=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " HeaderText=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " SortExpression=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "100px" + System.Convert.ToChar(34) + " />");
                                        break;
                                }
                            }
                        }
                        output.AppendLine("                                        <asp:TemplateField HeaderText=" + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " ItemStyle-Width=" + System.Convert.ToChar(34) + "10%" + System.Convert.ToChar(34) + " ItemStyle-HorizontalAlign=" + System.Convert.ToChar(34) + "Center" + System.Convert.ToChar(34) + " HeaderStyle-HorizontalAlign=" + System.Convert.ToChar(34) + "Center" + System.Convert.ToChar(34) + " FooterStyle-HorizontalAlign=" + System.Convert.ToChar(34) + "Center" + System.Convert.ToChar(34) + " HeaderStyle-VerticalAlign=" + System.Convert.ToChar(34) + "Middle" + System.Convert.ToChar(34) + " HeaderStyle-Wrap=" + System.Convert.ToChar(34) + "False" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                            <ItemTemplate>");
                        output.AppendLine("                                                <asp:Button class=" + System.Convert.ToChar(34) + "btn btn-primary btn-sm glyphicon-pencil " + System.Convert.ToChar(34) + " ID=" + System.Convert.ToChar(34) + "btnEditRow" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Editar" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "text-align: center;" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnEdit_Click" + System.Convert.ToChar(34) + " CommandArgument='<%# Container.DisplayIndex %>' />");
                        output.AppendLine("                                                <asp:Button class=" + System.Convert.ToChar(34) + "btn btn-danger  btn-sm" + System.Convert.ToChar(34) + " ID=" + System.Convert.ToChar(34) + "btnDeleteRow" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Borrar" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "text-align: center;" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnDelete_Click" + System.Convert.ToChar(34) + " CommandArgument='<%# Container.DisplayIndex %>' />");
                        output.AppendLine("                                            </ItemTemplate>");
                        output.AppendLine("                                            <HeaderTemplate>");
                        output.AppendLine("                                                <asp:LinkButton CssClass=" + System.Convert.ToChar(34) + "btn btn-success btn-default" + System.Convert.ToChar(34) + " ID=" + System.Convert.ToChar(34) + "btnAddNew" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Nuevo registro" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "text-align: center;" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnAddNew_Click" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                                </asp:LinkButton>");
                        output.AppendLine("                                            </HeaderTemplate>");
                        output.AppendLine("                                        </asp:TemplateField>");
                        output.AppendLine("                                    </Columns>");
                        output.AppendLine("                                    <HeaderStyle HorizontalAlign=" + System.Convert.ToChar(34) + "Center" + System.Convert.ToChar(34) + " VerticalAlign=" + System.Convert.ToChar(34) + "Middle" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                    <PagerSettings Mode=" + System.Convert.ToChar(34) + "NumericFirstLast" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                    <PagerStyle CssClass=" + System.Convert.ToChar(34) + "bs-pagination" + System.Convert.ToChar(34) + " HorizontalAlign=" + System.Convert.ToChar(34) + "Center" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                    <EmptyDataRowStyle CssClass=" + System.Convert.ToChar(34) + "table-bordered" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                    <EmptyDataTemplate>");
                        output.AppendLine("                                      <div class=" + System.Convert.ToChar(34) + "callout callout-warning" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                        <h4>No existen datos en esta tabla.</h4>");
                        output.AppendLine("                                            <asp:LinkButton CssClass=" + System.Convert.ToChar(34) + "btn btn-success btn-default" + System.Convert.ToChar(34) + " ID=" + System.Convert.ToChar(34) + "btnEmptyAddNew" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Ingrese un nuevo registro" + System.Convert.ToChar(34) + " Style=" + System.Convert.ToChar(34) + "text-align: center;" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnAddNew_Click" + System.Convert.ToChar(34) + "></asp:LinkButton>");
                        output.AppendLine("                                      </div>");
                        output.AppendLine("                                    </EmptyDataTemplate>");

                        output.AppendLine("                                </asp:GridView>");
                        output.AppendLine("                            </asp:Panel>");
                        output.AppendLine("                        </div>");
                        output.AppendLine("");
                        output.AppendLine("                        <div class=" + System.Convert.ToChar(34) + "row" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                            <asp:Panel ID=" + System.Convert.ToChar(34) + "DetailPanel" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                <div class=" + System.Convert.ToChar(34) + "panel-body" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("                                    <table class=" + System.Convert.ToChar(34) + "table table-striped table-bordered table-hover" + System.Convert.ToChar(34) + ">");

                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            if (column.IsInPrimaryKey)
                            {
                                output.AppendLine("                                        <tr>");
                                output.AppendLine("                                            <%--Id (Identificador de Registro, campo " + GetFormattedEntityName(column.Name) + " )--%>");
                                output.AppendLine("                                            <td>");
                                output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></asp:Label>");
                                output.AppendLine("                                            </td>");
                                output.AppendLine("                                            <td>");
                                output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + "Value" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + "></asp:Label>");
                                output.AppendLine("                                            </td>");
                                output.AppendLine("                                            <td></td>");
                                output.AppendLine("                                        </tr>");
                            }
                            if (column.IsInForeignKey && !column.IsInPrimaryKey)
                            {
                                output.AppendLine("                                        <tr>");
                                output.AppendLine("                                            <%-- Description of:  " + GetFormattedEntityName(column.Name) + " --%>");
                                output.AppendLine("                                            <td>");
                                output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></asp:Label>");
                                output.AppendLine("                                            </td>");
                                output.AppendLine("                                            <td>");
                                output.AppendLine("                                                <asp:DropDownList ID=" + System.Convert.ToChar(34) + "comboBox" + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " AutoPostBack=" + System.Convert.ToChar(34) + "True" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control" + System.Convert.ToChar(34) + "/>");
                                output.AppendLine("                                            </td>");
                                output.AppendLine("                                            <td></td>");
                                output.AppendLine("                                        </tr>");

                            }
                            if (!column.IsInPrimaryKey && !column.IsInForeignKey)
                            {
                                switch (column.LanguageType)
                                {
                                    case "String":
                                        output.AppendLine("                                        <tr>");
                                        output.AppendLine("                                            <%-- Description of:  " + GetFormattedEntityName(column.Name) + " --%>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></asp:Label>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:TextBox ID=" + System.Convert.ToChar(34) + "txt" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + "></asp:TextBox>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td></td>");
                                        output.AppendLine("                                        </tr>");
                                        break;
                                    case "DateTime":
                                        output.AppendLine("                                        <tr>");
                                        output.AppendLine("                                            <%-- Description of:  " + GetFormattedEntityName(column.Name) + " --%>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ClientIdMode=" + System.Convert.ToChar(34) + "static" + System.Convert.ToChar(34) + "></asp:Label>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        //output.AppendLine("                                                 <div class=" + System.Convert.ToChar(34) + "input-group" + System.Convert.ToChar(34) + "><div class=" + System.Convert.ToChar(34) + "input-group-addon" + System.Convert.ToChar(34) + "><i class=" + System.Convert.ToChar(34) + "fa fa-calendar" + System.Convert.ToChar(34) + "></i></div><div>");
                                        //output.AppendLine("                                                     <asp:TextBox ID=" + System.Convert.ToChar(34) + "txt" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "datepicker" + System.Convert.ToChar(34) + "  runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text='<%# Bind(" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + ") %>'></asp:TextBox>");
                                        //output.AppendLine("                                                 </div></div>");
                                        output.AppendLine("                                                <div class='input-group date' id=" + System.Convert.ToChar(34) + "txt" + GetFormattedEntityName(column.Name) + "EditContainer" + System.Convert.ToChar(34) + " >") ;
                                        output.AppendLine("                                                    <span class=" + System.Convert.ToChar(34) + "input-group-addon" + System.Convert.ToChar(34) + ">") ;
                                        output.AppendLine("                                                        <span class=" + System.Convert.ToChar(34) + "glyphicon glyphicon-calendar" + System.Convert.ToChar(34) + "></span>") ;
                                        output.AppendLine("                                                    </span>") ;
                                        output.AppendLine("                                                    <asp:TextBox ID=" + System.Convert.ToChar(34) + "txt" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " ></asp:TextBox>");
                                        output.AppendLine("                                                </div>") ;

                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                        </tr>");
                                        break;
                                    case "Boolean":
                                    case "Bool":
                                        output.AppendLine("                                        <tr>");
                                        output.AppendLine("                                            <%-- Description of:  " + GetFormattedEntityName(column.Name) + " --%>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + " ClientIdMode=" + System.Convert.ToChar(34) + "static" + System.Convert.ToChar(34) + "></asp:Label>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:CheckBox ID=" + System.Convert.ToChar(34) + "chk" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control date" + System.Convert.ToChar(34) + "  runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " ></asp:CheckBox>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                        </tr>");
                                        break;
                                    case "int":
                                    case "long":
                                    case "Int16":
                                    case "Int32":
                                    case "Int64":
                                        output.AppendLine("                                        <tr>");
                                        output.AppendLine("                                            <%-- Description of:  " + GetFormattedEntityName(column.Name) + " --%>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></asp:Label>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:TextBox ID=" + System.Convert.ToChar(34) + "txt" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control integer" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " ></asp:TextBox>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td></td>");
                                        output.AppendLine("                                        </tr>");
                                        break;
                                    case "decimal":
                                    case "float":
                                    case "double":
                                    case "Decimal":
                                    case "Float":
                                    case "Double":

                                        output.AppendLine("                                        <tr>");
                                        output.AppendLine("                                            <%-- Description of:  " + GetFormattedEntityName(column.Name) + " --%>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></asp:Label>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:TextBox ID=" + System.Convert.ToChar(34) + "txt" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control decimal" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " ></asp:TextBox>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td></td>");
                                        output.AppendLine("                                        </tr>");
                                        break;

                                    default:
                                        output.AppendLine("                                        <tr>");
                                        output.AppendLine("                                            <%-- Description of:  " + GetFormattedEntityName(column.Name) + " --%>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:Label ID=" + System.Convert.ToChar(34) + "lbl" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + GetFormattedEntityName(column.Name) + System.Convert.ToChar(34) + "></asp:Label>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td>");
                                        output.AppendLine("                                                <asp:TextBox ID=" + System.Convert.ToChar(34) + "txt" + GetFormattedEntityName(column.Name) + "Edit" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "form-control" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " ></asp:TextBox>");
                                        output.AppendLine("                                            </td>");
                                        output.AppendLine("                                            <td></td>");
                                        output.AppendLine("                                        </tr>");
                                        break;
                                }
                            }
                        }


                        output.AppendLine("                                    </table>");
                        output.AppendLine("                                    <asp:Button ID=" + System.Convert.ToChar(34) + "btnAccept" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "btn btn-success btn-sm" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Aceptar" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                    <asp:Button ID=" + System.Convert.ToChar(34) + "btnCancel" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "btn btn-danger btn-sm" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Cancelar" + System.Convert.ToChar(34) + " />");
                        output.AppendLine("                                </div>");
                        output.AppendLine("                            </asp:Panel>");
                        //output.AppendLine("");
                        //output.AppendLine("                            <asp:Panel ID=" + System.Convert.ToChar(34) + "pnlModal" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "modal-dialog-center" + System.Convert.ToChar(34) + " Visible=" + System.Convert.ToChar(34) + "false" + System.Convert.ToChar(34) + "");
                        //output.AppendLine("                                style=" + System.Convert.ToChar(34) + "position:fixed;top:100px;bottom:0px;left:0px;");
                        //output.AppendLine("                                right:0px;overflow:hidden;padding:0;margin:0;z-index:1000;" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                <asp:Panel ID=" + System.Convert.ToChar(34) + "pnlModalDialog" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "modal-dialog" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                    <asp:Panel ID=" + System.Convert.ToChar(34) + "pnlModalContent" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "modal-content" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                        <asp:Panel ID=" + System.Convert.ToChar(34) + "pnlModalHeader" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "modal-header" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                            <button type=" + System.Convert.ToChar(34) + "button" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "close" + System.Convert.ToChar(34) + " data-dismiss=" + System.Convert.ToChar(34) + "modal" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                                <span aria-hidden=" + System.Convert.ToChar(34) + "true" + System.Convert.ToChar(34) + ">&times;</span><span class=" + System.Convert.ToChar(34) + "sr-only" + System.Convert.ToChar(34) + ">Close</span>");
                        //output.AppendLine("                                            </button>");
                        //output.AppendLine("                                            <asp:Label ID=" + System.Convert.ToChar(34) + "lblTitulo" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "modal-title" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Confirma?" + System.Convert.ToChar(34) + "></asp:Label>");
                        //output.AppendLine("                                            <asp:Panel ID=" + System.Convert.ToChar(34) + "pnlBody" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "modal-body" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                                <div class=" + System.Convert.ToChar(34) + "form-inline" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                                    <div class=" + System.Convert.ToChar(34) + "form-group" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                                        <asp:Label ID=" + System.Convert.ToChar(34) + "lblMensaje" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "control-label" + System.Convert.ToChar(34) + " Width=" + System.Convert.ToChar(34) + "120" + System.Convert.ToChar(34) + ">Confirma operación?</asp:Label>");
                        //output.AppendLine("                                                    </div>");
                        //output.AppendLine("                                                </div>");
                        //output.AppendLine("                                            </asp:Panel>");
                        //output.AppendLine("                                            <asp:Panel ID=" + System.Convert.ToChar(34) + "pnlFooter" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "modal-footer" + System.Convert.ToChar(34) + ">");
                        //output.AppendLine("                                                <asp:Button ID=" + System.Convert.ToChar(34) + "btnAceptar" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "btn btn-success" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnAccept_Click" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Aceptar" + System.Convert.ToChar(34) + "></asp:Button>");
                        //output.AppendLine("                                                <asp:Button ID=" + System.Convert.ToChar(34) + "btnCancelar" + System.Convert.ToChar(34) + " CssClass=" + System.Convert.ToChar(34) + "btn btn-danger" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " OnClick=" + System.Convert.ToChar(34) + "btnCancel_Click" + System.Convert.ToChar(34) + " Text=" + System.Convert.ToChar(34) + "Cancelar" + System.Convert.ToChar(34) + "></asp:Button>");
                        //output.AppendLine("                                            </asp:Panel>");
                        //output.AppendLine("                                        </asp:Panel>");
                        //output.AppendLine("                                    </asp:Panel>");
                        //output.AppendLine("                                </asp:Panel>");
                        //output.AppendLine("                            </asp:Panel>");
                        //output.AppendLine("");
                        output.AppendLine("                        </div>");
                        output.AppendLine("                    </div>");
                        output.AppendLine("                </div>");
                        output.AppendLine("            </div>");
                        output.AppendLine("            <!-- end page-wrapper -->");
                        output.AppendLine("        </div>");
                        output.AppendLine("    </form>");
                        output.AppendLine("</body>");
                        output.AppendLine("</html>");
                        output.AppendLine("<script type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + ">");
                        output.AppendLine("    //datepicker");
                        output.AppendLine("    $('.datepicker').datepicker({");
                        output.AppendLine("        autoclose: true,");
                        output.AppendLine("        format: " + System.Convert.ToChar(34) + "yyyy-mm-dd" + System.Convert.ToChar(34) + ",");
                        output.AppendLine("        todayHighlight: true,");
                        //output.AppendLine("        //orientation: " + System.Convert.ToChar(34) + "top auto" + System.Convert.ToChar(34) + ",");
                        output.AppendLine("        todayBtn: true,");
                        output.AppendLine("    });");
                        output.AppendLine("");
                        output.AppendLine("    $(document).ready(function () {");
                        output.AppendLine("        $('.bs-pagination td table').each(function (index, obj) {");
                        output.AppendLine("            convertToPagination(obj)");
                        output.AppendLine("        });");
                        output.AppendLine("    });");
                        output.AppendLine("");
                        output.AppendLine("    function convertToPagination(obj) {");
                        output.AppendLine("        var liststring = '<ul class=" + System.Convert.ToChar(34) + "pagination" + System.Convert.ToChar(34) + ">';");
                        output.AppendLine("");
                        output.AppendLine("        $(obj).find(" + System.Convert.ToChar(34) + "tbody tr" + System.Convert.ToChar(34) + ").each(function () {");
                        output.AppendLine("            $(this).children().map(function () {");
                        output.AppendLine("                liststring = liststring + " + System.Convert.ToChar(34) + "<li>" + System.Convert.ToChar(34) + " + $(this).html() + " + System.Convert.ToChar(34) + "</li>" + System.Convert.ToChar(34) + ";");
                        output.AppendLine("            });");
                        output.AppendLine("        });");
                        output.AppendLine("        liststring = liststring + " + System.Convert.ToChar(34) + "</ul>" + System.Convert.ToChar(34) + ";");
                        output.AppendLine("        var list = $(liststring);");
                        output.AppendLine("        list.find('span').parent().addClass('active');");
                        output.AppendLine("");
                        output.AppendLine("        $(obj).replaceWith(list);");
                        output.AppendLine("    }");
                        output.AppendLine("");
                        output.AppendLine("$(" + System.Convert.ToChar(34) + ".integer" + System.Convert.ToChar(34) + ").numeric(" + System.Convert.ToChar(34) + "integer" + System.Convert.ToChar(34) + ");");

                        output.AppendLine("$(" + System.Convert.ToChar(34) + ".decimal" + System.Convert.ToChar(34) + ").alphanum({allowPlus: false, allowMinus: true,allowThouSep: false,allowDecSep:  true});");


                        output.AppendLine("</script>");

                        output.AppendLine("<script type=" + System.Convert.ToChar(34) + "text/javascript" + System.Convert.ToChar(34) + ">");
                        foreach (MyMeta.IColumn column in table.Columns)
                        {
                            switch (column.LanguageType)
                            {
                                case "DateTime":
                                case "Date":
                                    //output.AppendLine("        //Date range picker with time picker");
                                    //output.AppendLine("        $('#txt" + GetFormattedEntityName(column.Name) + "EditContainer').daterangepicker({ timePicker: true, timePickerIncrement: 1, format: 'DD/MM/YYYY HH:mm' });");
                                    output.AppendLine("        //DateTimepicker");
                                    output.AppendLine("        $('#txt" + GetFormattedEntityName(column.Name) + "EditContainer').datetimepicker({ format: 'DD/MM/YYYY HH:mm:ss' });");
                                    output.AppendLine("");

                                    break;
                                default:
                                    break;
                            }
                        }
                        output.AppendLine("</script>");


                        SaveOutputToFile(table.Name + ".aspx", output, true);
                    }
                }
                BuildMenuPage(db);

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

        private void BuildMenuPage(MyMeta.IDatabase db)
        {

            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.AppendLine("<%@ Page Language=" + System.Convert.ToChar(34) + "C#" + System.Convert.ToChar(34) + " %>");
            output.AppendLine("");
            output.AppendLine("<!DOCTYPE html>");
            output.AppendLine("<script runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
            output.AppendLine(" ");
            output.AppendLine("</script>");
            output.AppendLine("");
            output.AppendLine("<html xmlns=" + System.Convert.ToChar(34) + "http://www.w3.org/1999/xhtml" + System.Convert.ToChar(34) + ">");
            output.AppendLine("<head>");
            output.AppendLine("    <meta charset=" + System.Convert.ToChar(34) + "utf-8" + System.Convert.ToChar(34) + " />");
            output.AppendLine("    <meta http-equiv=" + System.Convert.ToChar(34) + "X-UA-Compatible" + System.Convert.ToChar(34) + " content=" + System.Convert.ToChar(34) + "IE=edge" + System.Convert.ToChar(34) + " />");
            output.AppendLine("    <title> " + " Menu " + " </title>");
            output.AppendLine("    <!-- Tell the browser to be responsive to screen width -->");
            output.AppendLine("    <meta content=" + System.Convert.ToChar(34) + "width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" + System.Convert.ToChar(34) + " name=" + System.Convert.ToChar(34) + "viewport" + System.Convert.ToChar(34) + " />");
            output.AppendLine("    <!-- Bootstrap 3.3.5 -->");
            output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.5/css/bootstrap.min.css" + System.Convert.ToChar(34) + " />");
            output.AppendLine("    <!-- Font Awesome -->");
            output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://maxcdn.bootstrapcdn.com/font-awesome/4.4.0/css/font-awesome.min.css" + System.Convert.ToChar(34) + " />");
            output.AppendLine("    <!-- Ionicons -->");
            output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://code.ionicframework.com/ionicons/2.0.1/css/ionicons.min.css" + System.Convert.ToChar(34) + " />");
            output.AppendLine("    <!-- Page Controls -->");
            output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.6.0/css/bootstrap-datepicker3.css" + System.Convert.ToChar(34) + " />");
            output.AppendLine("    <link rel=" + System.Convert.ToChar(34) + "stylesheet" + System.Convert.ToChar(34) + " href=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datetimepicker/4.17.37/css/bootstrap-datetimepicker.min.css" + System.Convert.ToChar(34) + " />");
            output.AppendLine("    <script src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/jquery/2.1.4/jquery.min.js" + System.Convert.ToChar(34) + "></script>");
            output.AppendLine("    <script src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.0.0-alpha/js/bootstrap.min.js" + System.Convert.ToChar(34) + "></script>");
            output.AppendLine("    <script src=" + System.Convert.ToChar(34) + "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.6.0/js/bootstrap-datepicker.min.js" + System.Convert.ToChar(34) + "></script>");
            output.AppendLine("");
            output.AppendLine("    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->");
            output.AppendLine("    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->");
            output.AppendLine("    <!--[if lt IE 9]>");
            output.AppendLine("        <script src=" + System.Convert.ToChar(34) + "https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js" + System.Convert.ToChar(34) + "></script>");
            output.AppendLine("        <script src=" + System.Convert.ToChar(34) + "https://oss.maxcdn.com/respond/1.4.2/respond.min.js" + System.Convert.ToChar(34) + "></script>");
            output.AppendLine("    <![endif]-->");
            output.AppendLine("</head>");
            output.AppendLine("<body>");
            output.AppendLine("    <form id=" + System.Convert.ToChar(34) + "form1" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + ">");
            output.AppendLine("        <div>");
            output.AppendLine("            <asp:HiddenField ID=" + System.Convert.ToChar(34) + "hiddenState" + System.Convert.ToChar(34) + " runat=" + System.Convert.ToChar(34) + "server" + System.Convert.ToChar(34) + " />");
            output.AppendLine("            <!--  page-wrapper -->");
            output.AppendLine("            <div id=" + System.Convert.ToChar(34) + "content" + System.Convert.ToChar(34) + ">");
            output.AppendLine("                <div class=" + System.Convert.ToChar(34) + "content" + System.Convert.ToChar(34) + ">");
            output.AppendLine("                    <div class=" + System.Convert.ToChar(34) + "panel panel-default" + System.Convert.ToChar(34) + ">");
            output.AppendLine("                        <div class=" + System.Convert.ToChar(34) + "panel-heading" + System.Convert.ToChar(34) + ">");
            output.AppendLine("                        </div>");
            output.AppendLine("                        <div class=" + System.Convert.ToChar(34) + "panel-body" + System.Convert.ToChar(34) + ">");
            output.AppendLine("                            <table id=" + System.Convert.ToChar(34) + "table" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "table table-striped table-bordered" + System.Convert.ToChar(34) + ">");
            output.AppendLine("                                <thead>");
            output.AppendLine("                                    <tr>");
            output.AppendLine("                                        <th style=" + System.Convert.ToChar(34) + "width: 40%;" + System.Convert.ToChar(34) + ">Name</th>");
            output.AppendLine("                                        <th style=" + System.Convert.ToChar(34) + "width: 40%;" + System.Convert.ToChar(34) + ">Descripcion</th>");
            output.AppendLine("                                        <th style=" + System.Convert.ToChar(34) + "width: 20%;" + System.Convert.ToChar(34) + "></th>");
            output.AppendLine("                                    </tr>");
            output.AppendLine("                                </thead>");
            output.AppendLine("                                <tbody>");
            foreach (MyMeta.ITable entity in db.Tables)
            {
                if (entity.Selected)
                {
                    output.AppendLine("                                    <tr>");
                    output.AppendLine("                                        <th>" + entity.Name + "</th>");
                    output.AppendLine("                                        <th>" + entity.Description + "</th>");
                    output.AppendLine("                                        <th style=" + System.Convert.ToChar(34) + "width: 100px;" + System.Convert.ToChar(34) + ">");
                    output.AppendLine("                                            <a href=" + System.Convert.ToChar(34) + entity.Name + ".aspx" + System.Convert.ToChar(34) + " class=" + System.Convert.ToChar(34) + "btn btn-info" + System.Convert.ToChar(34) + " role=" + System.Convert.ToChar(34) + "button" + System.Convert.ToChar(34) + ">Go</a>");
                    output.AppendLine("                                        </th>");
                    output.AppendLine("                                    </tr>");
                }
            }
            output.AppendLine("                                </tbody>");
            output.AppendLine("                                <tfoot>");
            output.AppendLine("                                </tfoot>");
            output.AppendLine("                            </table>");
            output.AppendLine("                        </div>");
            output.AppendLine("                    </div>");
            output.AppendLine("                </div>");
            output.AppendLine("            </div>");
            output.AppendLine("        </div>");
            output.AppendLine("        <!-- end page-wrapper -->");
            output.AppendLine("    </form>");
            output.AppendLine("</body>");
            output.AppendLine("</html>");
            output.AppendLine("");
            output.AppendLine("");
            output.AppendLine("");
            output.AppendLine("");
            SaveOutputToFile("MenuPage.aspx", output, true);
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
        private string FindPkAutoNumericColumnName(MyMeta.ITable table)
        {
            string returnvalue = string.Empty;

            foreach (MyMeta.IColumn item in table.Columns)
            {
                if (item.IsInPrimaryKey && item.IsAutoKey)
                {
                    returnvalue = item.Name;
                    break;
                }
            }

            return returnvalue;
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
        public string GetFormattedEntityName(String entityName)
        {
            string table = entityName;
            try
            {
                //return entityName.Replace(" ", "_").Replace(".", "_"); 
                if (_generationProject.SupportedCharacters.Length > 0)
                {
                    string chars = _generationProject.SupportedCharacters;
                    for (int i = 0; i < chars.Length; i++)
                    {
                        table = table.Replace(chars.Substring(i, 1), "_");
                    }
                }
            }
            catch (Exception)
            {
               
                
            }
            return table;
        }
        #region Save File Method
        private void SaveOutputToFile(string fileName, System.Text.StringBuilder output, bool overWrite)
        {
            if (!_workingdir.EndsWith("\\"))
                _workingdir += "\\";
            string filePath = _workingdir + "\\Pages\\" + fileName;
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
                    //OnFileGenerated(_workingdir + GENERATED_FOLDER_NAME + fileName);
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
            get { return "8cb2f3b7-e8eb-4868-9bea-68b534b75a1c"; }
        }

        #region ITemplate Members


        string ITemplate.Name
        {
            get
            {
                return this.Name;
            }

        }

        string ITemplate.Description
        {
            get
            {
                return this.Description;
            }

        }

        string ITemplate.OutputLanguage
        {
            get
            {
                return this.outputLanguage;
            }

        }

        string ITemplate.WorkingDir
        {
            get
            {
                return this.WorkingDir;
            }
            set
            {
                this.WorkingDir = value;
            }
        }

        string ITemplate.LanguageMappingFileName
        {
            get
            {
                return this.LanguageMappingFileName;
            }
            set
            {
                this.LanguageMappingFileName = value;
            }
        }

        string ITemplate.DbTargetMappingFileName
        {
            get
            {
                return this.DbTargetMappingFileName;
            }
            set
            {
                this.DbTargetMappingFileName = value;
            }
        }

        bool ITemplate.Execute(MyMeta.IDatabase db, string workingDir, GenerationProject generationProject)
        {
            return this.Execute(ref db, workingDir, generationProject);
        }

        string ITemplate.GUID
        {
            get { return this.GUID; }
        }

        #endregion
    }
}
    // End class