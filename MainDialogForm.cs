using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using MyMeta;
namespace OpenORM.UI
{
    public partial class MainDialogForm : Form
    {
        private const string OpenORM_NET = "OpenORM.NET ";

        //Selected Datasource
        public MyMeta.dbRoot _meta;
        public MyMeta.IDatabase _database;
        private TreeNode _rootNode = new TreeNode();
        public IDatabase Database;
        private string DATABASES = "DATABASES";
        private string TABLES = "TABLES";
        private string VIEWS = "VIEWS";
        private string PROCEDURES = "PROCEDURES";
        private string DOMAINS = "DOMAINS";
        private string COLUMNS = "COLUMNS";
        private string FOREIGNKEYS = "FKS";
        private string PRIMARYKEYS = "PKS";
        private string INDEXES = "INDEXES";
        private string PARAMETERS = "PARAMETERS";
        private string RESULTCOLUMNS = "RESULTCOLUMNS";
        private string PRIMARYCOLUMNS = "PRIMARY_COLUMNS";

        private string FOREIGNCOLUMNS = "FOREIGN_COLUMNS";

        private string OPEN_ORM_NET_PROJECT = "OpenORM.NET.Project.xml";
        private string OPEN_ORM_NET_EXECUTABLE = "OpenORM.NET.UI.exe";


        private string _projectFile = String.Empty;
        private GenerationProject _projectConfig = null;
        private string _workingDir = string.Empty;
        public enum NodeType
        {
            BLANK,
            MYMETA,
            DATABASES,
            DATABASE,
            TABLES,
            TABLE,
            VIEWS,
            VIEW,
            SUBVIEWS,
            SUBTABLES,
            PROCEDURES,
            PROCEDURE,
            COLUMNS,
            COLUMN,
            FOREIGNKEYS,
            INDIRECTFOREIGNKEYS,
            FOREIGNKEY,
            INDEXES,
            INDEX,
            PRIMARYKEYS,
            PRIMARKYKEY,
            PARAMETERS,
            PARAMETER,
            RESULTCOLUMNS,
            RESULTCOLUMN,
            DOMAINS,
            DOMAIN,
            PROVIDERTYPE,
            PROVIDERTYPES
        }
        private enum ImageType
        {
            Databases,
            Database,
            Tables,
            Table,
            Function,
            Views,
            View,
            Procedures,
            Procedure,
            Columns,
            Column,
            ForeignKeys,
            ForeignKey,
            Indexes,
            Index,
            PrimaryKeys,
            PrimaryKey,
            Parameters,
            Parameter,
            ResultColumns,
            ResultColumn,
            Domains,
            Domain,
            Providertypes,
            Providertype
        }
        private TreeNode BlankNode
        {
            get
            {
                TreeNode _blankNode = new TreeNode("");
                _blankNode.Tag = "Blank";
                return _blankNode;
            }
        }
        #region Event Window


        private Int32 _errorcount;
        private Int32 _warningcount;
        private Int32 _messagecount;
        private List<EventObject> _eventList = new List<EventObject>();
        #endregion


        private void MainDialogForm_Load(object sender, System.EventArgs e)
        {
            try
            {
                this.SuspendLayout();
                this.StartPosition = FormStartPosition.CenterScreen;
                //this.WindowState = FormWindowState.Maximized;
                Initialize();

                CustomCollectionEditor.FormClosed += CustomCollectionEditor_FormClosed;

            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        void CustomCollectionEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            GenerationProjectSerializer.Save(_projectFile, _projectConfig);
        }
        public void Initialize()
        {
            InitializeComponent();
            try
            {
                var myId = Process.GetCurrentProcess().Id;
                var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", myId);
                var search = new System.Management.ManagementObjectSearcher("root\\CIMV2", query);
                var results = search.Get().GetEnumerator();
                results.MoveNext();
                var queryObj = results.Current;
                var parentId = (uint)queryObj["ParentProcessId"];
                var parent = Process.GetProcessById((int)parentId);
                if (parent.ProcessName.ToLower().Equals("explorer"))
                {
                    string message = "OpenORM.NET must be called as external tool in Visual Studio or SharpDevelop.";
                        message += Environment.NewLine + "Current path is " + Application.StartupPath + @"\" + OPEN_ORM_NET_EXECUTABLE;
                        message += Environment.NewLine + "Press 'OK' if you want to copy it in your clipboard.";

                    DialogResult dialogResult = MessageBox.Show(message, "Configuration error", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    switch (dialogResult)
                    {
                        case DialogResult.Yes:
                            Clipboard.SetText(Application.StartupPath + @"\" + OPEN_ORM_NET_EXECUTABLE);
                            break;
                        case DialogResult.No:
                            break;
                        default:
                            break;
                    }
                    
                    Application.Exit();
                }
                
                String[] arguments = Environment.GetCommandLineArgs();
                UpdateOutputListview();

                foreach (string data in arguments)
                {
                    AddOutput("Argument: " + data , "", EventObject.EventTypeEnum.InfoEvent);
                }
                if (arguments.Length > 1)
                {

                    _projectFile = arguments[1].ToString() + @"\" + OPEN_ORM_NET_PROJECT;
                    _workingDir = arguments[1].ToString();
                    _workingDir = _workingDir.Replace(System.Convert.ToChar(34).ToString(), "");

                    _projectFile = _projectFile.Replace(System.Convert.ToChar(34).ToString(), String.Empty);

                    LoadProject();
                }
                else
                {
                    AddOutput("Not enough arguments where provided.", "Startup", EventObject.EventTypeEnum.WarningEvent);
                }
                
            }
            catch (Exception ex)
            {
                AddOutput("_projectFile: " + _projectFile + " - _workingDir: " + _workingDir,"",EventObject.EventTypeEnum.ErrorEvent);
                AddOutput(ex);
            }
            finally
            {
                //this.ResumeLayout();
            }
        }

        private void LoadProject()
        {
            try
            {
                _projectConfig = GenerationProjectSerializer.Load(_projectFile);
                PropertyGrid.SelectedObject = _projectConfig;
                if (!_projectConfig.ConnectionString.Trim().Equals(String.Empty))
                {
                    DisplayProjectNameAndDescription();
                    Connect(_projectConfig);
                }
                OpenORM.UI.Classes.Global.CurrentProject = _projectConfig;

            }
            catch (Exception ex)
            {

                //AddOutput("_projectFile: " + _projectFile + " - _workingDir: " + _workingDir, "", EventObject.EventTypeEnum.ErrorEvent);
                if(ex.InnerException != null)
                    AddOutput(ex.InnerException);
                else
                    AddOutput(ex);
            }
        }

        private void DisplayProjectNameAndDescription()
        {
            try
            {
                var executingAssembly = Assembly.GetExecutingAssembly();

                if (_projectConfig.Name != null  && _projectConfig.Name.Length > 0)
                    this.Text = OpenORM_NET +
                    " Version: " + executingAssembly.GetName().Version.ToString() + " :.: " +
                     _projectConfig.Name;

                if (_projectConfig.Name != null && _projectConfig.Name.Length == 0 && _projectConfig.Description.Length > 0)
                    this.Text = OpenORM_NET + _projectConfig.Name +
                    " Version: " + executingAssembly.GetName().Version.ToString() + " :.: " +
                    _projectConfig.Description;

                if (_projectConfig.Name != null && _projectConfig.Name.Length == 0 && _projectConfig.Description.Length == 0)
                    this.Text = OpenORM_NET +
                    " Version: " + executingAssembly.GetName().Version.ToString();

            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
         }

        private bool Connect(GenerationProject projectConfig)
        {

            string error = string.Empty;

            try
            {
                _meta = null;
                _rootNode = new TreeNode();
                this.dbObjectsTreeview.Nodes.Clear();
                _meta = new MyMeta.dbRoot();
                ToolStripProgressBar.Visible = true;

                bool connected = _meta.Connect(projectConfig.DbDriver, projectConfig.ConnectionString);
                _meta.UserMetaDataFileName = projectConfig.ConnectionString;
                AddOutput("Loaded " + projectConfig.Name + ", Driver: " + projectConfig.DbDriver.ToString(), "Startup", EventObject.EventTypeEnum.InfoEvent);
                _meta.ShowSystemData = projectConfig.ShowSystemData;

                if(projectConfig.Namespace.Trim().Equals(String.Empty))
                {
                    projectConfig.Namespace = "DefaultNamespace";
                }
                if (projectConfig.ChildrenPrefix.Trim().Equals(String.Empty))
                {
                    projectConfig.ChildrenPrefix = "ListOf_";
                }
                if (projectConfig.EntityDisplayName.Trim().Equals(String.Empty))
                {
                    projectConfig.EntityDisplayName = "Name";
                }

                if (projectConfig.ConnectionStringName.Trim().Equals(String.Empty))
                {
                    projectConfig.ConnectionStringName = "ConnectionString";
                }
                /// SupportedCharacters default
                if (projectConfig.SupportedCharacters.Trim().Equals(String.Empty))
                {
                    projectConfig.SupportedCharacters = " .";
                }

                if (projectConfig.Location.Trim().Equals(String.Empty))
                {
                    projectConfig.Location = AppDomain.CurrentDomain.BaseDirectory;
                }

                SaveProjectFile();
                
                string appFolder = AppDomain.CurrentDomain.BaseDirectory;

                string _dbTargetMappingFileName = appFolder + @"DbTargets.xml";
                string _languageMappingFileName = appFolder + @"Languages.xml";

                _meta.DbTargetMappingFileName = _dbTargetMappingFileName;
                _meta.LanguageMappingFileName = _languageMappingFileName;
                //_meta.DomainOverride = false;

                InitializeTree(ref _meta, _projectConfig.DatasourceServer);
                UpdateSelectedObjects(_projectConfig);
                LoadPlugins();
                return true;
            }
            catch (Exception ex)
            {
                AddOutput(ex);
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                ToolStripProgressBar.Visible = false;
            }


        }
        public void InitializeTree(ref MyMeta.dbRoot meta, string Name)
        {
            this.dbObjectsTreeview.BeginUpdate();

            dbObjectsTreeview.Nodes.Clear();


            if (meta.IsConnected)
            {
                _rootNode.Tag = new NodeData(NodeType.MYMETA, meta);
                _rootNode.ImageIndex = (int)ImageType.Databases;

                _rootNode.SelectedImageIndex = _rootNode.ImageIndex;
                this.dbObjectsTreeview.Nodes.Add(_rootNode);
                _rootNode.Text = Name;
                // This is true for dbDriver = NONE
                if (meta.Databases == null)
                {
                    this.dbObjectsTreeview.EndUpdate();
                    return;
                }

                try
                {
                    TreeNode databasesNode = new TreeNode(DATABASES);
                    databasesNode.Name = databasesNode.Text;
                    //databasesNode.Tag = New NodeData(NodeType.DATABASES, myMeta.Databases)
                    databasesNode.ImageIndex = (int)ImageType.Databases;
                    databasesNode.SelectedImageIndex = databasesNode.ImageIndex;
                    _rootNode.Nodes.Add(databasesNode);

                    foreach (MyMeta.IDatabase database in meta.Databases)
                    {
                        TreeNode dbNode = new TreeNode(database.Name);
                        dbNode.Name = dbNode.Text;
                        dbNode.Tag = new NodeData(NodeType.DATABASE, database);
                        dbNode.ImageIndex = (int)ImageType.Database;
                        dbNode.SelectedImageIndex = dbNode.ImageIndex;
                        dbNode.Nodes.Add(this.BlankNode);
                        databasesNode.Nodes.Add(dbNode);

                    }

                    _rootNode.Expand();
                    databasesNode.Expand();

                    this.dbObjectsTreeview.EndUpdate();
                }
                catch (Exception ex)
                {
                    AddOutput(ex);
                    if ((_rootNode != null))
                    {
                        _rootNode.Text = "ROOT (" + ex.Message + " )";
                    }
                }
            }
        }
 
        private void ExpandDatabase(IDatabase database, TreeNode dbNode)
        {
            try
            {
                if (HasBlankNode(dbNode))
                {
                    IDatabase db = _meta.Databases[database.Name];

                    TreeNode node = default(TreeNode);

                    if (db.Tables.Count > 0)
                    {
                        node = new TreeNode(TABLES);
                        node.Name = node.Text;
                        node.Tag = new NodeData(NodeType.TABLES, database.Tables);
                        node.ImageIndex = (int)ImageType.Tables;
                        node.SelectedImageIndex = node.ImageIndex;
                        dbNode.Nodes.Add(node);
                        node.Nodes.Add(this.BlankNode);
                    }

                    if (db.Views.Count > 0)
                    {
                        node = new TreeNode(VIEWS);
                        node.Name = node.Text;
                        node.Tag = new NodeData(NodeType.VIEWS, database.Views);
                        node.ImageIndex = (int)ImageType.Views;
                        node.SelectedImageIndex = node.ImageIndex;
                        dbNode.Nodes.Add(node);
                        node.Nodes.Add(this.BlankNode);
                    }

                    if (db.Procedures.Count > 0)
                    {
                        node = new TreeNode(PROCEDURES);
                        node.Name = node.Text;
                        node.Tag = new NodeData(NodeType.PROCEDURES, database.Procedures);
                        node.ImageIndex = (int)ImageType.Procedures;
                        node.SelectedImageIndex = node.ImageIndex;
                        dbNode.Nodes.Add(node);
                        node.Nodes.Add(this.BlankNode);
                    }

                    if (db.Domains.Count > 0)
                    {
                        node = new TreeNode(DOMAINS);
                        node.Name = node.Text;
                        node.Tag = new NodeData(NodeType.DOMAINS, database.Domains);
                        node.ImageIndex = (int)ImageType.Domains;
                        node.SelectedImageIndex = node.ImageIndex;
                        dbNode.Nodes.Add(node);
                        node.Nodes.Add(this.BlankNode);
                    }
                }
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }

        }

        private void ExpandTables(ITables tables, TreeNode node)
        {
            try
            {
                if (HasBlankNode(node))
                {
                    foreach (ITable entity in tables)
                    {
                        TreeNode n = new TreeNode(entity.Schema == String.Empty ? entity.Name : entity.Schema + "." + entity.Name);
                        n.Tag = new NodeData(NodeType.TABLE, entity);
                        n.ImageIndex = (int)ImageType.Table;
                        n.SelectedImageIndex = n.ImageIndex;
                        node.Nodes.Add(n);
                        n.Nodes.Add(this.BlankNode);
                    }
                }
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }

        }

        private void ExpandTable(ITable table, TreeNode node)
        {
            try
            {
                if (HasBlankNode(node))
                {
                    TreeNode n = default(TreeNode);

                    if (table.Columns.Count > 0)
                    {
                        n = new TreeNode(COLUMNS);
                        n.Tag = new NodeData(NodeType.COLUMNS, table.Columns);
                        n.ImageIndex = (int)ImageType.Columns;
                        n.SelectedImageIndex = n.ImageIndex;
                        node.Nodes.Add(n);
                        n.Nodes.Add(this.BlankNode);
                    }

                    if (table.ForeignKeys.Count > 0)
                    {
                        n = new TreeNode(FOREIGNKEYS);
                        n.Tag = new NodeData(NodeType.FOREIGNKEYS, table.ForeignKeys);
                        n.ImageIndex = (int)ImageType.ForeignKeys;
                        n.SelectedImageIndex = n.ImageIndex;
                        node.Nodes.Add(n);
                        n.Nodes.Add(this.BlankNode);
                    }

                    if (table.Indexes.Count > 0)
                    {
                        n = new TreeNode(INDEXES);
                        n.Tag = new NodeData(NodeType.INDEXES, table.Indexes);
                        n.ImageIndex = (int)ImageType.Indexes;
                        n.SelectedImageIndex = n.ImageIndex;
                        node.Nodes.Add(n);
                        n.Nodes.Add(this.BlankNode);
                    }

                    if (table.PrimaryKeys.Count > 0)
                    {
                        n = new TreeNode(PRIMARYKEYS);
                        n.Tag = new NodeData(NodeType.PRIMARYKEYS, table.PrimaryKeys);
                        n.ImageIndex = (int)ImageType.PrimaryKeys;
                        n.SelectedImageIndex = n.ImageIndex;
                        node.Nodes.Add(n);
                        n.Nodes.Add(this.BlankNode);
                    }
                }
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }

        }

        private void ExpandViews(IViews views, TreeNode node)
        {
            try
            {
                if (HasBlankNode(node))
                {
                    foreach (IView entity in views)
                    {
                        //TreeNode n = new TreeNode(entity.Schema + "." + entity.Name);
                        TreeNode n = new TreeNode(entity.Schema == String.Empty ? entity.Name : entity.Schema + "." + entity.Name);
                        n.Tag = new NodeData(NodeType.VIEW, entity);
                        n.ImageIndex = (int)ImageType.View;
                        n.SelectedImageIndex = n.ImageIndex;
                        node.Nodes.Add(n);
                        n.Nodes.Add(this.BlankNode);
                    }
                }
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }

        }

        private void ExpandView(IView view, TreeNode node)
        {
            try
            {
                if (HasBlankNode(node))
                {
                    TreeNode n = default(TreeNode);

                    if (view.Columns.Count > 0)
                    {
                        n = new TreeNode(COLUMNS);
                        n.Tag = new NodeData(NodeType.COLUMNS, view.Columns);
                        n.ImageIndex = (int)ImageType.Column;
                        n.SelectedImageIndex = n.ImageIndex;
                        node.Nodes.Add(n);
                        n.Nodes.Add(this.BlankNode);
                    }

                    if (view.SubTables.Count > 0)
                    {
                        n = new TreeNode("SubTables");
                        n.Tag = new NodeData(NodeType.SUBTABLES, view.SubTables);
                        n.ImageIndex = (int)ImageType.Table;
                        n.SelectedImageIndex = n.ImageIndex;
                        node.Nodes.Add(n);
                        n.Nodes.Add(this.BlankNode);
                    }

                    if (view.SubViews.Count > 0)
                    {
                        n = new TreeNode("SubViews");
                        n.Tag = new NodeData(NodeType.SUBVIEWS, view.SubViews);
                        n.ImageIndex = (int)ImageType.View;
                        n.SelectedImageIndex = n.ImageIndex;
                        node.Nodes.Add(n);
                        n.Nodes.Add(this.BlankNode);
                    }
                }
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }

        }

        private void ExpandProcedures(IProcedures procedures, TreeNode node)
        {
            try
            {


                bool show = false;
                if (HasBlankNode(node))
                {
                    foreach (IProcedure entity in procedures)
                    {
                        switch (entity.Schema.ToUpper())
                        {
                            case  // ERROR: Case labels with binary operators are unsupported : Equality
                                "SYS":
                                if (entity.Database.Root.ShowSystemData)
                                {
                                    show = true;
                                }
                                else
                                {
                                    show = false;
                                }
                                break;
                            default:
                                show = true;
                                break;
                        }
                        if (show)
                        {
                            //TreeNode n = new TreeNode(entity.Schema + "." + entity.Name);
                            TreeNode n = new TreeNode(entity.Schema == String.Empty ? entity.Name : entity.Schema + "." + entity.Name);
                            n.Tag = new NodeData(NodeType.PROCEDURE, entity);
                            n.ImageIndex = (int)ImageType.Procedure;
                            n.SelectedImageIndex = n.ImageIndex;
                            n.ToolTipText = entity.Schema + "." + entity.Name;
                            node.Nodes.Add(n);
                            n.Nodes.Add(this.BlankNode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
        }

        private void ExpandProcedure(IProcedure procedure, TreeNode node)
        {
            if (HasBlankNode(node))
            {
                TreeNode n = default(TreeNode);

                if (procedure.Parameters.Count > 0)
                {
                    n = new TreeNode(PARAMETERS);
                    n.Tag = new NodeData(NodeType.PARAMETERS, procedure.Parameters);
                    n.ImageIndex = (int)ImageType.Parameters;
                    n.SelectedImageIndex = n.ImageIndex;
                    node.Nodes.Add(n);
                    n.Nodes.Add(this.BlankNode);
                }

                if (procedure.ResultColumns.Count > 0)
                {
                    n = new TreeNode(RESULTCOLUMNS);
                    n.Tag = new NodeData(NodeType.RESULTCOLUMNS, procedure.ResultColumns);
                    n.ImageIndex = (int)ImageType.ResultColumn;
                    n.SelectedImageIndex = n.ImageIndex;
                    node.Nodes.Add(n);
                    n.Nodes.Add(this.BlankNode);
                }
            }
        }

        private void ExpandColumns(IColumns columns, TreeNode node)
        {
            if (HasBlankNode(node))
            {
                foreach (IColumn column in columns)
                {
                    TreeNode n = new TreeNode(column.Name);
                    n.Tag = new NodeData(NodeType.COLUMN, column);

                    if (!column.IsInPrimaryKey)
                    {
                        n.ImageIndex = (int)ImageType.Column;

                    }
                    else
                    {
                        n.ImageIndex = (int)ImageType.PrimaryKey;
                    }
                    n.SelectedImageIndex = n.ImageIndex;
                    node.Nodes.Add(n);

                    if (column.ForeignKeys.Count > 0)
                    {
                        TreeNode nn = new TreeNode(FOREIGNKEYS);
                        nn.Tag = new NodeData(NodeType.FOREIGNKEYS, column.ForeignKeys);
                        nn.ImageIndex = (int)ImageType.ForeignKeys;
                        nn.SelectedImageIndex = nn.ImageIndex;
                        n.Nodes.Add(nn);
                        nn.Nodes.Add(this.BlankNode);
                    }
                }
            }
        }

        private void ExpandParameters(IParameters parameters, TreeNode node)
        {
            if (HasBlankNode(node))
            {
                foreach (IParameter parameter in parameters)
                {
                    TreeNode n = new TreeNode(parameter.Name);
                    n.Tag = new NodeData(NodeType.PARAMETER, parameter);
                    n.ImageIndex = (int)ImageType.Parameter;
                    n.SelectedImageIndex = n.ImageIndex;
                    node.Nodes.Add(n);
                }
            }
        }

        private void ExpandResultColumns(IResultColumns resultColumns, TreeNode node)
        {
            if (HasBlankNode(node))
            {
                foreach (IResultColumn resultColumn in resultColumns)
                {
                    TreeNode n = new TreeNode(resultColumn.Name);
                    n.Tag = new NodeData(NodeType.RESULTCOLUMN, resultColumn);
                    n.ImageIndex = (int)ImageType.ResultColumn;
                    n.SelectedImageIndex = n.ImageIndex;
                    node.Nodes.Add(n);
                }
            }
        }

        private void ExpandIndexes(IIndexes indexes, TreeNode node)
        {
            if (HasBlankNode(node))
            {
                foreach (IIndex index in indexes)
                {
                    TreeNode indexNode = new TreeNode(index.Name);
                    indexNode.Tag = new NodeData(NodeType.INDEX, index);
                    indexNode.ImageIndex = (int)ImageType.Index;
                    indexNode.SelectedImageIndex = indexNode.ImageIndex;
                    node.Nodes.Add(indexNode);

                    if (index.Columns.Count > 0)
                    {
                        TreeNode n = new TreeNode(COLUMNS);
                        n.Tag = new NodeData(NodeType.COLUMNS, index.Columns);
                        n.ImageIndex = (int)ImageType.Column;
                        n.SelectedImageIndex = n.ImageIndex;
                        indexNode.Nodes.Add(n);
                        n.Nodes.Add(this.BlankNode);
                    }
                }
            }
        }

        private void ExpandForeignKeys(IForeignKeys foreignKeys, TreeNode node)
        {
            if (HasBlankNode(node))
            {
                foreach (IForeignKey foreignKey in foreignKeys)
                {
                    TreeNode n = default(TreeNode);
                    TreeNode fkNode = new TreeNode(foreignKey.Name);
                    fkNode.Tag = new NodeData(NodeType.FOREIGNKEY, foreignKey);
                    fkNode.ImageIndex = (int)ImageType.ForeignKey;
                    fkNode.SelectedImageIndex = fkNode.ImageIndex;
                    node.Nodes.Add(fkNode);

                    if (foreignKey.PrimaryColumns.Count > 0)
                    {
                        n = new TreeNode(PRIMARYCOLUMNS);
                        n.Tag = new NodeData(NodeType.COLUMNS, foreignKey.PrimaryColumns);
                        n.ImageIndex = (int)ImageType.PrimaryKey;
                        n.SelectedImageIndex = n.ImageIndex;
                        fkNode.Nodes.Add(n);
                        n.Nodes.Add(this.BlankNode);
                    }

                    if (foreignKey.ForeignColumns.Count > 0)
                    {
                        n = new TreeNode(FOREIGNCOLUMNS);
                        n.Tag = new NodeData(NodeType.COLUMNS, foreignKey.ForeignColumns);
                        n.ImageIndex = (int)ImageType.ForeignKeys;
                        n.SelectedImageIndex = n.ImageIndex;
                        fkNode.Nodes.Add(n);
                        n.Nodes.Add(this.BlankNode);
                    }
                }
            }
        }
        private bool HasBlankNode(TreeNode node)
        {
            if (node.Nodes.Count == 1 && "Blank" == node.Nodes[0].Tag as string)
            {
                node.Nodes.Clear();
                return true;
            }
            else
            {
                return false;
            }
        }
        private void ExpandDomains(IDomains domains, TreeNode node)
        {
            if (HasBlankNode(node))
            {
                foreach (IDomain domain in domains)
                {
                    TreeNode n = new TreeNode(domain.Name);
                    n.Tag = new NodeData(NodeType.DOMAIN, domain);
                    n.ImageIndex = (int)ImageType.Domains;
                    n.SelectedImageIndex = n.ImageIndex;
                    node.Nodes.Add(n);
                }
            }
        }

        private void dbObjectsTreeview_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            try
            {


                if (e.Node.Checked == true)
                {
                    if (e.Node.Parent.Text.Equals(PROCEDURES))
                    {
                        foreach (var x in _projectConfig.MappedStoredProcedures)
                        {
                            if (x.Name.Equals(e.Node.Text))
                            {
                                MessageBox.Show("This Stored Procedure has been mapped and can´t be selected." + Environment.NewLine + 
                                "Delete mapping to select it.","Mapped SP",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                                e.Node.Checked = false;
                                return;
                            }
                        }
                    }

                }


                bool @checked = false;
                this.Cursor = Cursors.WaitCursor;
                dbObjectsTreeview.SuspendLayout();
                dbObjectsTreeview.AfterCheck -= dbObjectsTreeview_AfterCheck;
                CheckChildNode(e.Node, e.Node.Checked);

                if (e.Node.Checked == true)
                {
                    CheckParentNode(e.Node.Parent, true);
                }

                foreach (TreeNode refItem in _rootNode.Nodes[0].Nodes)
                {
                    if (refItem.Checked)
                    {
                        _database = (IDatabase)((NodeData)refItem.Tag).Meta;
                        _projectConfig.SelectedDatabase = _database.Name;
                        @checked = true;
                    }
                }

                if (!@checked)
                {
                    _database = null;
                }
                UpdateProjectSelection(_projectConfig);
                GenerationProjectSerializer.Save(_projectFile, _projectConfig);
                PropertyGrid.SelectedObject = _projectConfig;

                dbObjectsTreeview.AfterCheck += dbObjectsTreeview_AfterCheck;
                dbObjectsTreeview.ResumeLayout();
                this.Cursor = Cursors.Default;

            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
        }

        private void CheckChildNode(TreeNode refnode, bool Value)
        {

            try
            {
                if (refnode.Tag == null)
                    return;

                refnode.Checked = Value;
                switch (refnode.Tag.GetType().Name.ToUpper())
                {
                    case "NODEDATA":
                        switch (((NodeData)refnode.Tag).Type)
                        {
                            case NodeType.BLANK:

                                break;
                            case NodeType.TABLE:
                                MyMeta.ITable refTable = (ITable)((NodeData)refnode.Tag).Meta;

                                refTable.Selected = Value;
                                break;
                            case NodeType.DATABASE:
                            case NodeType.TABLES:
                            case NodeType.VIEWS:
                            case NodeType.PROCEDURES:

                                if (!refnode.IsExpanded)
                                {
                                    refnode.Expand();
                                }
                                foreach (TreeNode refChildNode in refnode.Nodes)
                                {
                                    CheckChildNode(refChildNode, Value);
                                }


                                break;
                            case NodeType.VIEW:
                                MyMeta.IView refview = (IView)((NodeData)refnode.Tag).Meta;
                                //If Not refnode.IsExpanded Then
                                //    refnode.Expand()
                                //End If
                                foreach (TreeNode refChildNode in refnode.Nodes)
                                {
                                    refChildNode.Checked = Value;
                                }

                                refview.Selected = Value;
                                break;
                            case NodeType.PROCEDURE:
                                MyMeta.IProcedure refProcedure = (IProcedure)((NodeData)refnode.Tag).Meta;
                                refProcedure.Selected = Value;

                                break;
                            default:
                                break;
                            //LOG ERROR

                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
        }

        private void CheckParentNode(TreeNode refnode, bool Value)
        {
            try
            {

                if (refnode == null)
                    return;
                if (refnode.Tag == null)
                    return;
                refnode.Checked = true;
                switch (refnode.Tag.GetType().Name.ToUpper())
                {
                    case "NODEDATA":
                        switch (((NodeData)refnode.Tag).Type)
                        {
                            case NodeType.BLANK:

                                break;
                            case NodeType.DATABASE:
                                break;
                            //_meta = refnode.Tag
                            //RaiseEvent OnInstanceUpdate(Me)

                            case NodeType.TABLES:
                            case NodeType.VIEWS:
                            case NodeType.PROCEDURES:

                                CheckParentNode(refnode.Parent, Value);
                                break;
                            case NodeType.TABLE:
                                MyMeta.ITable refTable = (ITable)((NodeData)refnode.Tag).Meta;
                                refTable.Selected = Value;
                                CheckParentNode(refnode.Parent, Value);
                                break;
                            case NodeType.VIEW:
                                MyMeta.IView refview = (IView)((NodeData)refnode.Tag).Meta;
                                refview.Selected = Value;
                                CheckParentNode(refnode.Parent, Value);
                                break;
                            case NodeType.PROCEDURE:
                                MyMeta.IProcedure refProcedure = (IProcedure)((NodeData)refnode.Tag).Meta;
                                refProcedure.Selected = Value;
                                CheckParentNode(refnode.Parent, Value);
                                break;
                            default:
                                break;
                            //LOG ERROR
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
        }
        private void dbObjectsTreeview_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            try
            {
                NodeData curNode = (NodeData)e.Node.Tag;
                if ((curNode != null))
                {
                    switch (curNode.Type)
                    {
                        case NodeType.BLANK:
                            break;
                        case NodeType.COLUMN:
                            break;
                        case NodeType.COLUMNS:
                            ExpandColumns((IColumns)curNode.Meta, e.Node);
                            break;
                        case NodeType.DATABASE:
                            ExpandDatabase((IDatabase)curNode.Meta, e.Node);
                            break;
                        case NodeType.DATABASES:
                            break;
                        case NodeType.DOMAIN:
                            break;
                        case NodeType.DOMAINS:
                            ExpandDomains((IDomains)curNode.Meta, e.Node);
                            break;
                        case NodeType.FOREIGNKEY:
                            break;
                        case NodeType.FOREIGNKEYS:
                            ExpandForeignKeys((IForeignKeys)curNode.Meta, e.Node);
                            break;
                        case NodeType.INDEX:
                            break;
                        case NodeType.INDEXES:
                            ExpandIndexes((IIndexes)curNode.Meta, e.Node);
                            break;
                        case NodeType.INDIRECTFOREIGNKEYS:
                            break;
                        case NodeType.MYMETA:
                            break;
                        case NodeType.PARAMETER:
                            break;
                        case NodeType.PARAMETERS:
                            ExpandParameters((IParameters)curNode.Meta, e.Node);
                            break;
                        case NodeType.PRIMARKYKEY:

                            break;
                        case NodeType.PRIMARYKEYS:
                            ExpandColumns((IColumns)curNode.Meta, e.Node);
                            break;
                        case NodeType.PROCEDURE:
                            ExpandProcedure((IProcedure)curNode.Meta, e.Node);
                            break;
                        case NodeType.PROCEDURES:
                            ExpandProcedures((IProcedures)curNode.Meta, e.Node);
                            break;
                        case NodeType.PROVIDERTYPE:
                            break;
                        case NodeType.PROVIDERTYPES:
                            break;
                        case NodeType.RESULTCOLUMN:
                            break;
                        case NodeType.RESULTCOLUMNS:
                            ExpandResultColumns((IResultColumns)curNode.Meta, e.Node);
                            break;
                        case NodeType.SUBTABLES:
                            break;
                        case NodeType.SUBVIEWS:
                            break;
                        case NodeType.TABLE:
                            ExpandTable((ITable)curNode.Meta, e.Node);
                            break;
                        case NodeType.TABLES:
                            ExpandTables((ITables)curNode.Meta, e.Node);
                            break;
                        case NodeType.VIEW:
                            ExpandView((IView)curNode.Meta, e.Node);
                            break;
                        case NodeType.VIEWS:
                            ExpandViews((IViews)curNode.Meta, e.Node);
                            break;
                        default:

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
        }

        private class NodeData
        {
            public NodeData(NodeType type, object meta)
            {
                this.Type = type;
                this.Meta = meta;
            }

            public NodeType Type = NodeType.BLANK;
            public object Meta = null;
            public bool Loaded = false;
            public MyMeta.IDatabase ParentDatabase = null;
        }

        public void ConnectDatasource(GenerationProject Project)
        {
            try
            {
                TreeNode refDatabasesNode = null;
                TreeNode refDbNode = null;
                TreeNode refTablesNode = null;
                TreeNode refViewsNode = null;
                TreeNode refSPsNode = null;
                bool connected = false;

                if (!Project.ConnectionString.Trim().Equals(String.Empty))
                    Connect(Project);
                connected = Connect(Project);

                if (connected)
                {

                    refDatabasesNode = _rootNode.Nodes[DATABASES];
                    foreach (TreeNode node in refDatabasesNode.Nodes)
                    {
                        if (node.Text == Project.SelectedDatabase)
                        {
                            //node.Checked = True
                            refDbNode = node;
                            refDbNode.Expand();
                        }
                    }

                    //refViewsNode = refDbNode.Nodes(1)
                    //refSPsNode = refDbNode.Nodes(2)
                    // Loads tables
                    try
                    {
                        refTablesNode = refDbNode.Nodes[TABLES];
                        if (refTablesNode != null)
                        {
                            refTablesNode.Expand();
                            foreach (TreeNode node in refTablesNode.Nodes)
                            {
                                foreach (string refTable in Project.SelectedTables)
                                {
                                    if (refTable == node.Text)
                                    {
                                        node.Checked = true;
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddOutput(ex);
                    }



                    // Loads views
                    try
                    {
                        refViewsNode = refDbNode.Nodes[VIEWS];
                        if (refViewsNode != null)
                        {
                            refViewsNode.Expand();
                            foreach (TreeNode node in refViewsNode.Nodes)
                            {
                                foreach (string refview in Project.SelectedViews)
                                {
                                    if (refview == node.Text)
                                    {
                                        node.Checked = true;
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddOutput(ex);
                    }

                    // Loads procedures
                    try
                    {
                        refSPsNode = refDbNode.Nodes[PROCEDURES];
                        if (refSPsNode == null)
                        {
                            refSPsNode.Expand();
                            foreach (TreeNode node in refSPsNode.Nodes)
                            {
                                foreach (string refSP in Project.SelectedStoredProcedures)
                                {
                                    if (refSP == node.Text)
                                    {
                                        node.Checked = true;
                                        break; // TODO: might not be correct. Was : Exit For
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddOutput(ex);
                    }
                }
                else
                {
                    //if (OnInstanceError != null) {
                    //    OnInstanceError(new Exception("No se pudo conectar a : " + Project.DatasourceServer), this);
                    //}
                }

            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }

        }
        public void UpdateProjectSelection(GenerationProject Project)
        {
            try
            {
                TreeNode refDatabasesNode = null;
                TreeNode refDbNode = null;
                TreeNode refTablesNode = null;
                TreeNode refViewsNode = null;
                TreeNode refSPsNode = null;

                refDatabasesNode = _rootNode.Nodes[DATABASES];
                foreach (TreeNode node in refDatabasesNode.Nodes)
                {
                    if (node.Checked)
                    {
                        Project.SelectedDatabase = node.Text;
                        refDbNode = node;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }


                // Loads tables
                try
                {
                    if(refDbNode == null)
                    {
                        return;
                    }
                    Project.SelectedTables.Clear();
                    refTablesNode = refDbNode.Nodes[TABLES];
                    if (refTablesNode != null)
                    {
                        
                        foreach (TreeNode tablenode in refTablesNode.Nodes)
                        {
                            if (tablenode.Checked)
                            {
                                Project.SelectedTables.Add(tablenode.Text);

                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    AddOutput(ex);
                    //RaiseEvent OnInstanceError(ex, Me)
                }

                // Loads views
                try
                {
                    Project.SelectedViews.Clear();
                    refViewsNode = refDbNode.Nodes[VIEWS];
                    if (refViewsNode != null)
                    {
                        foreach (TreeNode viewnode in refViewsNode.Nodes)
                        {
                            if (viewnode.Checked)
                            {
                                Project.SelectedViews.Add(viewnode.Text);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //RaiseEvent OnInstanceError(ex, Me)
                    AddOutput(ex);
                }

                // Loads procedures
                try
                {
                    Project.SelectedStoredProcedures.Clear();
                    Project.UnselectedStoredProcedures.Clear();
                    refSPsNode = refDbNode.Nodes[PROCEDURES];
                    if (refSPsNode != null)
                    {
                        foreach (TreeNode spnode in refSPsNode.Nodes)
                        {
                            if (spnode.Checked)
                            {
                                Project.SelectedStoredProcedures.Add(spnode.Text);
                                //Project.SelectedStoredProcedures.Add(new MappedStoredProcedure(spnode.Text));
                            }
                            else
                            {
                                Project.UnselectedStoredProcedures.Add(spnode.Text);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddOutput(ex);
                    //RaiseEvent OnInstanceError(ex, Me)
                }
                //

                // Loads mapped procedures


            }
            catch (Exception ex)
            {
                AddOutput(ex);
                //if (OnInstanceError != null) {
                //    OnInstanceError(ex, this);
                //}
            }
        }

        public void UpdateSelectedObjects(GenerationProject Project)
        {

            TreeNode refDatabasesNode = null;
            TreeNode refDbNode = null;
            TreeNode refTablesNode = null;
            TreeNode refViewsNode = null;
            TreeNode refSPsNode = null;
            dbObjectsTreeview.AfterCheck -= dbObjectsTreeview_AfterCheck;

            try
            {
                refDatabasesNode = _rootNode.Nodes[DATABASES];
                foreach (TreeNode node in refDatabasesNode.Nodes)
                {
                    if (node.Text == Project.SelectedDatabase)
                    {
                        node.Checked = true;
                        refDbNode = node;
                        refDbNode.Expand();
                    }
                }
                if (refDbNode == null)
                {
                    dbObjectsTreeview.AfterCheck += dbObjectsTreeview_AfterCheck;
                    return;
                }
                // Loads tables
                refTablesNode = refDbNode.Nodes[TABLES];
                if (refTablesNode != null)
                {
                    refTablesNode.Expand();
                    foreach (TreeNode node in refTablesNode.Nodes)
                    {
                        foreach (string refTable in Project.SelectedTables)
                        {
                            if (refTable == node.Text)
                            {
                                node.Checked = true;
                                CheckParentNode(node, true);
                                break; ;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }

            // Loads views
            try
            {
                refViewsNode = refDbNode.Nodes[VIEWS];
                if (refViewsNode != null)
                {
                    refViewsNode.Expand();
                    foreach (TreeNode node in refViewsNode.Nodes)
                    {
                        foreach (string refview in Project.SelectedViews)
                        {
                            if (refview == node.Text)
                            {
                                node.Checked = true;
                                CheckParentNode(node, true);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }

            // Loads procedures
            try
            {
                refSPsNode = refDbNode.Nodes[PROCEDURES];
                if (refSPsNode != null)
                {
                    refSPsNode.Expand();
                    foreach (TreeNode node in refSPsNode.Nodes)
                    {
                        foreach (string refSP in Project.SelectedStoredProcedures)
                        {
                            if (refSP.Equals(node.Text))
                            {
                                node.Checked = true;
                                CheckParentNode(node, true);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
            dbObjectsTreeview.AfterCheck += dbObjectsTreeview_AfterCheck;
        }

        public void CloseProject()
        {
            try
            {
                _database = null;
                dbObjectsTreeview.Nodes.Clear();

            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
        }

        private void dbObjectsTreeview_NodeMouseDoubleClick(object sender, System.Windows.Forms.TreeNodeMouseClickEventArgs e)
        {

            try
            {
                MainDialogForm.NodeData nodedata = (NodeData)e.Node.Tag;
                switch (nodedata.Type)
                {
                    case NodeType.PROCEDURE:
                        //MainEditorForm refEditor = new MainEditorForm();
                        //refEditor.Tag = "SQL";
                        ////refEditor.MainScintillaControl.Text = CType(nodedata.Meta, MyMeta.IProcedure).ProcedureText
                        //refEditor.Show(MainForm.DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);

                        break;
                }

            }
            catch (Exception ex)
            {
                AddOutput(ex);

            }
        }
        public MainDialogForm()
        {
            Load += MainDialogForm_Load;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                if (_projectFile.Equals(string.Empty))
                {
                    AddOutput("No working directory defined.", "Generate.", EventObject.EventTypeEnum.WarningEvent);
                    return;
                }


                Cursor.Current = Cursors.WaitCursor;

                GenerationProjectSerializer.Save(_projectFile, _projectConfig);

                ToggleControlsEnabling(false);
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                
                foreach (ListViewItem item in pluginsListView.Items)
                {
                    if (item.Checked == true)
                    {
                        AddOutput("Starting " + ((ITemplate)item.Tag).Name, "", EventObject.EventTypeEnum.InfoEvent);
                        ((ITemplate)item.Tag).Execute(_meta.Databases[_projectConfig.SelectedDatabase], _workingDir, _projectConfig);
                        AddOutput(((ITemplate)item.Tag).Name + " finished.", "", EventObject.EventTypeEnum.InfoEvent);
                    }
                }
                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                AddOutput("Generation time : " + elapsedTime, "", EventObject.EventTypeEnum.InfoEvent);
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
            finally
            {
                ToggleControlsEnabling(true);
                Cursor.Current = Cursors.Default;
            }
        }

        private void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            GenerationProjectSerializer.Save(_projectFile, _projectConfig);
            if (e.ChangedItem.Label.Equals("DbDriver")
                || e.ChangedItem.Label.Equals("ConnectionString")
                || e.ChangedItem.Label.Equals("ShowSystemData"))
                Connect(_projectConfig);
            string oldValue = e.OldValue == null ? "[No Value]" : e.OldValue.ToString();
            DisplayProjectNameAndDescription();
            AddOutput("Changed property " + e.ChangedItem.Label  +" from: " + oldValue  + " to: " + e.ChangedItem.Value.ToString(), "", EventObject.EventTypeEnum.InfoEvent);
        }

        public void SaveProjectFile()
        {
            try
            {
                if (dbObjectsTreeview.Nodes.Count > 0)
                {
                    _projectConfig.DatasourceServer = dbObjectsTreeview.Nodes[0].Text;

                    _projectConfig.SelectedTemplates.Clear();
                    foreach (ListViewItem refItem in pluginsListView.Items)
                    {
                        if (refItem.Checked)
                        {
                            _projectConfig.SelectedTemplates.Add(((ITemplate)refItem.Tag).GUID);
                        }
                    }
                    UpdateProjectSelection(_projectConfig);
                    //if (!System.IO.Directory.Exists(GenerationProjectSerializer.CurrentProject.Location))
                    //{
                    //    System.IO.Directory.CreateDirectory(GenerationProjectSerializer.CurrentProject.Location);
                    //}
                    GenerationProjectSerializer.Save(_projectFile, _projectConfig);
                    AddOutput(GenerationProjectSerializer.CurrentProject.Name + " saved correctly.", "", EventObject.EventTypeEnum.InfoEvent);
                }

            }
            catch (Exception ex)
            {
                AddOutput(ex.Message, "SaveProjectFile", EventObject.EventTypeEnum.ErrorEvent);
            }

        }

        private void AddOutput(string Data, string Source, EventObject.EventTypeEnum OutPutType)
        {
            try
            {
                Append(Data.Trim(), Source, OutPutType);
            }
            catch (System.ObjectDisposedException ex)
            {
                // nothing
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddOutput(Exception exept)
        {
            try
            {
                Append(exept.Message, exept.StackTrace, EventObject.EventTypeEnum.ErrorEvent);
            }
            catch (System.ObjectDisposedException ex)
            {
                // nothing
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPlugins()
        {
            pluginsListView.Items.Clear();
            pluginsListView.ItemChecked -=pluginsListView_ItemChecked;
            ITemplate baseClassesTemplate = new BusinessLogicLayerTemplate_5G_CSHARP(_projectConfig);
            
            baseClassesTemplate.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            baseClassesTemplate.OnException += baseClassesTemplate_OnException;
            baseClassesTemplate.OnPercentDone += baseClassesTemplate_OnPercentDone;
            baseClassesTemplate.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItembaseClassesTemplate =  pluginsListView.Items.Add(new ListViewItem(baseClassesTemplate.Description, 7, new ListViewGroup(baseClassesTemplate.Name)));
            lvItembaseClassesTemplate.Tag = baseClassesTemplate;

            ITemplate WebFormsWebGridCrudTemplate = new WebFormsWebGridCrudTemplate_5G_CSHARP();

            WebFormsWebGridCrudTemplate.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            WebFormsWebGridCrudTemplate.OnException += baseClassesTemplate_OnException;
            WebFormsWebGridCrudTemplate.OnPercentDone += baseClassesTemplate_OnPercentDone;
            WebFormsWebGridCrudTemplate.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemWebFormsWebGridCrudTemplate = pluginsListView.Items.Add(new ListViewItem(WebFormsWebGridCrudTemplate.Description, 7, new ListViewGroup(WebFormsWebGridCrudTemplate.Name)));
            lvItemWebFormsWebGridCrudTemplate.Tag = WebFormsWebGridCrudTemplate;

            ITemplate DatabaseReportTemplate = new ReportingPlugin.HtmlDatabaseReport();

            DatabaseReportTemplate.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            DatabaseReportTemplate.OnException += baseClassesTemplate_OnException;
            DatabaseReportTemplate.OnPercentDone += baseClassesTemplate_OnPercentDone;
            DatabaseReportTemplate.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemDatabaseReportTemplate = pluginsListView.Items.Add(new ListViewItem(DatabaseReportTemplate.Description, 7, new ListViewGroup(DatabaseReportTemplate.Name)));
            lvItemDatabaseReportTemplate.Tag = DatabaseReportTemplate;

            ITemplate DatabaseObjectsTemplate = new DatabaseObjectsPlugin.FKViewsBuilder();

            DatabaseObjectsTemplate.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            DatabaseObjectsTemplate.OnException += baseClassesTemplate_OnException;
            DatabaseObjectsTemplate.OnPercentDone += baseClassesTemplate_OnPercentDone;
            DatabaseObjectsTemplate.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemDatabaseObjectsTemplate = pluginsListView.Items.Add(new ListViewItem(DatabaseObjectsTemplate.Description, 7, new ListViewGroup(DatabaseObjectsTemplate.Name)));
            lvItemDatabaseObjectsTemplate.Tag = DatabaseObjectsTemplate;

            ITemplate AppConfigTemplate = new AppConfigTemplatePlugin.AppConfigTemplate();

            AppConfigTemplate.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            AppConfigTemplate.OnException += baseClassesTemplate_OnException;
            AppConfigTemplate.OnPercentDone += baseClassesTemplate_OnPercentDone;
            AppConfigTemplate.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemAppConfigTemplate = pluginsListView.Items.Add(new ListViewItem(AppConfigTemplate.Description, 7, new ListViewGroup(AppConfigTemplate.Name)));
            lvItemAppConfigTemplate.Tag = AppConfigTemplate;

            ITemplate CrudBuilderTemplate = new CrudBuilderPlugin.CrudBuilderTemplate();

            CrudBuilderTemplate.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            CrudBuilderTemplate.OnException += baseClassesTemplate_OnException;
            CrudBuilderTemplate.OnPercentDone += baseClassesTemplate_OnPercentDone;
            CrudBuilderTemplate.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemCrudBuilderTemplate = pluginsListView.Items.Add(new ListViewItem(CrudBuilderTemplate.Description, 7, new ListViewGroup(CrudBuilderTemplate.Name)));
            lvItemCrudBuilderTemplate.Tag = CrudBuilderTemplate;


            ITemplate WebServiceBuilderTemplate = new WebServiceBuilderPlugin.WebServiceBuilderTemplate(_projectConfig);

            WebServiceBuilderTemplate.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            WebServiceBuilderTemplate.OnException += baseClassesTemplate_OnException;
            WebServiceBuilderTemplate.OnPercentDone += baseClassesTemplate_OnPercentDone;
            WebServiceBuilderTemplate.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemWebServiceBuilderTemplate = pluginsListView.Items.Add(new ListViewItem(WebServiceBuilderTemplate.Description, 7, new ListViewGroup(WebServiceBuilderTemplate.Name)));
            lvItemWebServiceBuilderTemplate.Tag = WebServiceBuilderTemplate;


            ITemplate SyncfusionJsCrudBuilderTemplate = new SyncfusionJsControlsBuilder.SyncfusionJsCrudBuilder(_projectConfig);

            SyncfusionJsCrudBuilderTemplate.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            SyncfusionJsCrudBuilderTemplate.OnException += baseClassesTemplate_OnException;
            SyncfusionJsCrudBuilderTemplate.OnPercentDone += baseClassesTemplate_OnPercentDone;
            SyncfusionJsCrudBuilderTemplate.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemSyncfusionJsCrudBuilder = pluginsListView.Items.Add(new ListViewItem(SyncfusionJsCrudBuilderTemplate.Description, 7, new ListViewGroup(SyncfusionJsCrudBuilderTemplate.Name)));
            lvItemSyncfusionJsCrudBuilder.Tag = SyncfusionJsCrudBuilderTemplate;


            ITemplate MudblazorBuilderTemplate = new MudBlazorControlsBuilder.MudBlazorCrudBuilder(_projectConfig);

            MudblazorBuilderTemplate.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            MudblazorBuilderTemplate.OnException += baseClassesTemplate_OnException;
            MudblazorBuilderTemplate.OnPercentDone += baseClassesTemplate_OnPercentDone;
            MudblazorBuilderTemplate.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemMudBlazorCrudBuilder = pluginsListView.Items.Add(new ListViewItem(MudblazorBuilderTemplate.Description, 7, new ListViewGroup(MudblazorBuilderTemplate.Name)));
            lvItemMudBlazorCrudBuilder.Tag = MudblazorBuilderTemplate;

            ITemplate RadzenBlazorCrudBuilderTemplate = new RadzenBlazorControlsBuilder.RadzenBlazorCrudBuilder(_projectConfig);

            RadzenBlazorCrudBuilderTemplate.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            RadzenBlazorCrudBuilderTemplate.OnException += baseClassesTemplate_OnException;
            RadzenBlazorCrudBuilderTemplate.OnPercentDone += baseClassesTemplate_OnPercentDone;
            RadzenBlazorCrudBuilderTemplate.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemRadzenBlazorCrudBuilder = pluginsListView.Items.Add(new ListViewItem(RadzenBlazorCrudBuilderTemplate.Description, 7, new ListViewGroup(RadzenBlazorCrudBuilderTemplate.Name)));
            lvItemRadzenBlazorCrudBuilder.Tag = RadzenBlazorCrudBuilderTemplate;


            ITemplate RadzenBlazorLeftMenuBuilderTemplate = new RadzenBlazorControlsBuilder.RadzenBlazorLeftMenuBuilder(_projectConfig);

            RadzenBlazorLeftMenuBuilderTemplate.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            RadzenBlazorLeftMenuBuilderTemplate.OnException += baseClassesTemplate_OnException;
            RadzenBlazorLeftMenuBuilderTemplate.OnPercentDone += baseClassesTemplate_OnPercentDone;
            RadzenBlazorLeftMenuBuilderTemplate.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemRadzenBlazorLeftMenuBuilder = pluginsListView.Items.Add(new ListViewItem(RadzenBlazorLeftMenuBuilderTemplate.Description, 7, new ListViewGroup(RadzenBlazorLeftMenuBuilderTemplate.Name)));
            lvItemRadzenBlazorLeftMenuBuilder.Tag = RadzenBlazorLeftMenuBuilderTemplate;

            ITemplate RadzenBlazorViewReportBuilderTemplate = new RadzenBlazorControlsBuilder.RadzenBlazorViewReportBuilder(_projectConfig);

            RadzenBlazorViewReportBuilderTemplate.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            RadzenBlazorViewReportBuilderTemplate.OnException += baseClassesTemplate_OnException;
            RadzenBlazorViewReportBuilderTemplate.OnPercentDone += baseClassesTemplate_OnPercentDone;
            RadzenBlazorViewReportBuilderTemplate.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemRadzenBlazorViewReportBuilder = pluginsListView.Items.Add(new ListViewItem(RadzenBlazorViewReportBuilderTemplate.Description, 7, new ListViewGroup(RadzenBlazorViewReportBuilderTemplate.Name)));
            lvItemRadzenBlazorViewReportBuilder.Tag = RadzenBlazorViewReportBuilderTemplate;
            ////////////////////////////////////////////////
            
            ITemplate BaseclassesExtensionsBuilderTemplate = new BaseClassesExtensionsBuilder.LogBuilder(_projectConfig);

            BaseclassesExtensionsBuilderTemplate.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            BaseclassesExtensionsBuilderTemplate.OnException += baseClassesTemplate_OnException;
            BaseclassesExtensionsBuilderTemplate.OnPercentDone += baseClassesTemplate_OnPercentDone;
            BaseclassesExtensionsBuilderTemplate.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemBaseClassesExtensionsBuilder = pluginsListView.Items.Add(new ListViewItem(BaseclassesExtensionsBuilderTemplate.Description, 7, new ListViewGroup(RadzenBlazorViewReportBuilderTemplate.Name)));
            lvItemBaseClassesExtensionsBuilder.Tag = BaseclassesExtensionsBuilderTemplate;

            //////////////////////// RV2
            ITemplate RadzenBlazorCrudBuilderTemplateV2 = new RadzenBlazorControlsBuilderV2.RadzenBlazorCrudBuilderV2(_projectConfig);

            RadzenBlazorCrudBuilderTemplateV2.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            RadzenBlazorCrudBuilderTemplateV2.OnException += baseClassesTemplate_OnException;
            RadzenBlazorCrudBuilderTemplateV2.OnPercentDone += baseClassesTemplate_OnPercentDone;
            RadzenBlazorCrudBuilderTemplateV2.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemRadzenBlazorCrudBuilderV2 = pluginsListView.Items.Add(new ListViewItem(RadzenBlazorCrudBuilderTemplateV2.Description, 7, new ListViewGroup(RadzenBlazorCrudBuilderTemplateV2.Name)));
            lvItemRadzenBlazorCrudBuilderV2.Tag = RadzenBlazorCrudBuilderTemplateV2;


            ITemplate RadzenBlazorLeftMenuBuilderTemplateV2 = new RadzenBlazorControlsBuilderV2.RadzenBlazorLeftMenuBuilderV2(_projectConfig);

            RadzenBlazorLeftMenuBuilderTemplateV2.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            RadzenBlazorLeftMenuBuilderTemplateV2.OnException += baseClassesTemplate_OnException;
            RadzenBlazorLeftMenuBuilderTemplateV2.OnPercentDone += baseClassesTemplate_OnPercentDone;
            RadzenBlazorLeftMenuBuilderTemplateV2.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemRadzenBlazorLeftMenuBuilderV2 = pluginsListView.Items.Add(new ListViewItem(RadzenBlazorLeftMenuBuilderTemplateV2.Description, 7, new ListViewGroup(RadzenBlazorLeftMenuBuilderTemplateV2.Name)));
            lvItemRadzenBlazorLeftMenuBuilderV2.Tag = RadzenBlazorLeftMenuBuilderTemplateV2;

            ITemplate RadzenBlazorViewReportBuilderTemplateV2 = new RadzenBlazorControlsBuilderV2.RadzenBlazorViewReportBuilderV2(_projectConfig);

            RadzenBlazorViewReportBuilderTemplateV2.OnFileGenerated += baseClassesTemplate_OnFileGenerated;
            RadzenBlazorViewReportBuilderTemplateV2.OnException += baseClassesTemplate_OnException;
            RadzenBlazorViewReportBuilderTemplateV2.OnPercentDone += baseClassesTemplate_OnPercentDone;
            RadzenBlazorViewReportBuilderTemplateV2.OnInfo += baseClassesTemplate_OnInfo;

            ListViewItem lvItemRadzenBlazorViewReportBuilderV2 = pluginsListView.Items.Add(new ListViewItem(RadzenBlazorViewReportBuilderTemplateV2.Description, 7, new ListViewGroup(RadzenBlazorViewReportBuilderTemplateV2.Name)));
            lvItemRadzenBlazorViewReportBuilderV2.Tag = RadzenBlazorViewReportBuilderTemplateV2;
            ////////////////////////////////////////////////

            foreach (string item in _projectConfig.SelectedTemplates)
            {
                if (baseClassesTemplate.GUID.Equals(item))
                    lvItembaseClassesTemplate.Checked = true;
                if (WebFormsWebGridCrudTemplate.GUID.Equals(item))
                    lvItemWebFormsWebGridCrudTemplate.Checked = true;
                if (DatabaseReportTemplate.GUID.Equals(item))
                    lvItemDatabaseReportTemplate.Checked = true;
                if (DatabaseObjectsTemplate.GUID.Equals(item))
                    lvItemDatabaseObjectsTemplate.Checked = true;
                if (AppConfigTemplate.GUID.Equals(item))
                    lvItemAppConfigTemplate.Checked = true;
                if (CrudBuilderTemplate.GUID.Equals(item))
                    lvItemCrudBuilderTemplate.Checked = true;
                if (WebServiceBuilderTemplate.GUID.Equals(item))
                    lvItemWebServiceBuilderTemplate.Checked = true;
                if (SyncfusionJsCrudBuilderTemplate.GUID.Equals(item))
                    lvItemSyncfusionJsCrudBuilder.Checked = true;
                if(MudblazorBuilderTemplate.GUID.Equals(item))
                    lvItemMudBlazorCrudBuilder.Checked = true;
                if (RadzenBlazorCrudBuilderTemplate.GUID.Equals(item))
                    lvItemRadzenBlazorCrudBuilder.Checked = true;
                if (RadzenBlazorLeftMenuBuilderTemplate.GUID.Equals(item))
                    lvItemRadzenBlazorLeftMenuBuilder.Checked = true;
                if (RadzenBlazorViewReportBuilderTemplate.GUID.Equals(item))
                    lvItemRadzenBlazorViewReportBuilder.Checked = true;

                if (RadzenBlazorCrudBuilderTemplateV2.GUID.Equals(item))
                    lvItemRadzenBlazorCrudBuilderV2.Checked = true;
                if (RadzenBlazorLeftMenuBuilderTemplateV2.GUID.Equals(item))
                    lvItemRadzenBlazorLeftMenuBuilderV2.Checked = true;
                if (RadzenBlazorViewReportBuilderTemplateV2.GUID.Equals(item))
                    lvItemRadzenBlazorViewReportBuilderV2.Checked = true;



            }
            pluginsListView.ItemChecked += pluginsListView_ItemChecked;
        }

        void baseClassesTemplate_OnInfo(string Description)
        {
            AddOutput(Description, "", EventObject.EventTypeEnum.WarningEvent);
        }

        void baseClassesTemplate_OnPercentDone(double Percent)
        {
            AddOutput(Percent.ToString(), "",EventObject.EventTypeEnum.InfoEvent);
        }

        void baseClassesTemplate_OnException(Exception Iex)
        {
            AddOutput(Iex);
        }

        void baseClassesTemplate_OnFileGenerated(string File)
        {
            AddOutput("Generated: " + File + " - End at: " + DateTime.Now.ToString(), "", EventObject.EventTypeEnum.InfoEvent);
        }

        public bool UpdateProjectSelection2(GenerationProject currentProject)
        {
            try
            {
                TreeNode refDatabasesNode = null;
                TreeNode refDbNode = null;
                TreeNode refTablesNode = null;
                TreeNode refViewsNode = null;
                TreeNode refSPsNode = null;

                refDatabasesNode = _rootNode.Nodes[DATABASES];
                foreach (TreeNode node in refDatabasesNode.Nodes)
                {
                    if (node.Checked)
                    {
                        currentProject.SelectedDatabase = node.Text;
                        refDbNode = node;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }


                // Loads tables
                try
                {
                    currentProject.SelectedTables.Clear();
                    refTablesNode = refDbNode.Nodes[TABLES];
                    if (refTablesNode != null)
                    {
                        foreach (TreeNode tablenode in refTablesNode.Nodes)
                        {
                            if (tablenode.Checked)
                            {
                                currentProject.SelectedTables.Add(tablenode.Text);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    AddOutput(ex);
                }

                // Loads views
                try
                {
                    currentProject.SelectedViews.Clear();
                    refViewsNode = refDbNode.Nodes[VIEWS];
                    if (refViewsNode != null)
                    {

                        foreach (TreeNode viewnode in refViewsNode.Nodes)
                        {
                            if (viewnode.Checked)
                            {
                                currentProject.SelectedViews.Add(viewnode.Text);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddOutput(ex);
                }

                // Loads procedures
                try
                {
                    currentProject.SelectedStoredProcedures.Clear();
                    currentProject.UnselectedStoredProcedures.Clear();
                    refSPsNode = refDbNode.Nodes[PROCEDURES];
                    if (refSPsNode != null)
                    {

                        foreach (TreeNode spnode in refSPsNode.Nodes)
                        {
                            if (spnode.Checked)
                            {
                                currentProject.SelectedStoredProcedures.Add(spnode.Text);
                                //Project.SelectedStoredProcedures.Add(new MappedStoredProcedure(spnode.Text));
                            }
                            else
                            {
                                currentProject.UnselectedStoredProcedures.Add(spnode.Text);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    AddOutput(ex);
                }

                OpenORM.UI.Classes.Global.CurrentProject = currentProject; ;

                return true;
            }
            catch (Exception ex)
            {
                AddOutput(ex);
                return false;
            }
        }

        #region EventWindow Events
        public void Append(string Data, string Source, EventObject.EventTypeEnum EventType)
        {

            try
            {
                _eventList.Add(new EventObject(Data, EventType));
                ListViewItem lItem = default(ListViewItem);
                switch (EventType)
                {
                    case EventObject.EventTypeEnum.ErrorEvent:
                        _errorcount += 1;
                        ErrorsToolStripButton.Text = _errorcount + " Errors";
                        if (ErrorsToolStripButton.Checked)
                        {
                            lItem = OutputListview.Items.Add(Data, (int)EventType);
                            lItem.SubItems.Add(Source);
                        }
                        break;
                    case EventObject.EventTypeEnum.InfoEvent:
                        _messagecount += 1;
                        InfoToolStripButton.Text = _messagecount + " Messages";
                        if (InfoToolStripButton.Checked)
                        {
                            lItem = OutputListview.Items.Add(Data, (int)EventType);
                            lItem.SubItems.Add(Source);
                        }
                        break;
                    case EventObject.EventTypeEnum.WarningEvent:
                        _warningcount += 1;
                        WarningsToolStripButton.Text = _warningcount + " Warnings";
                        if (WarningsToolStripButton.Checked)
                        {
                            lItem = OutputListview.Items.Add(Data, (int)EventType);
                            lItem.SubItems.Add(Source);
                        }

                        break;
                }
                OutputListview.Items[OutputListview.Items.Count - 1].EnsureVisible();

            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
        }
        public void Append(Exception ExceptionObject, string Source, EventObject.EventTypeEnum EventType)
        {

            try
            {
                _eventList.Add(new EventObject(ExceptionObject.Message, EventType));
                ListViewItem lItem = default(ListViewItem);
                switch (EventType)
                {
                    case EventObject.EventTypeEnum.ErrorEvent:
                        _errorcount += 1;
                        ErrorsToolStripButton.Text = _errorcount + " Errors";
                        if (ErrorsToolStripButton.Checked)
                        {
                            lItem = OutputListview.Items.Add(ExceptionObject.Message, (int)EventType);
                            lItem.SubItems.Add(Source);
                        }
                        break;
                    case EventObject.EventTypeEnum.InfoEvent:
                        _messagecount += 1;
                        InfoToolStripButton.Text = _messagecount + " Messages";
                        if (InfoToolStripButton.Checked)
                        {
                            lItem = OutputListview.Items.Add(ExceptionObject.Message, (int)EventType);
                            lItem.SubItems.Add(Source);
                        }
                        break;
                    case EventObject.EventTypeEnum.WarningEvent:
                        _warningcount += 1;
                        WarningsToolStripButton.Text = _warningcount + " Warnings";
                        if (WarningsToolStripButton.Checked)
                        {
                            lItem = OutputListview.Items.Add(ExceptionObject.Message, (int)EventType);
                            lItem.SubItems.Add(Source);
                        }
                        break;

                }
                OutputListview.Items[OutputListview.Items.Count - 1].EnsureVisible();

            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
        }
        private void UpdateOutputListview()
        {
            try
            {
                ListViewItem lItem = new ListViewItem();
                OutputListview.Items.Clear();

                foreach (EventObject evt in _eventList)
                {

                    if (ErrorsToolStripButton.Checked && evt.EventType == EventObject.EventTypeEnum.ErrorEvent)
                    {
                        lItem = OutputListview.Items.Add(evt.EventText, (int)evt.EventType);
                    }

                    if (WarningsToolStripButton.Checked && evt.EventType == EventObject.EventTypeEnum.WarningEvent)
                    {
                        lItem = OutputListview.Items.Add(evt.EventText, (int)evt.EventType);
                    }

                    if (InfoToolStripButton.Checked && evt.EventType == EventObject.EventTypeEnum.InfoEvent)
                    {
                        lItem = OutputListview.Items.Add(evt.EventText, (int)evt.EventType);
                    }
                }

                lItem.EnsureVisible();

            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
        }
        private void ToolStripButton_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                UpdateOutputListview();

            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
        }
        private void DeleteOutputToolStripButton_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                _errorcount = 0;
                _messagecount = 0;
                _warningcount = 0;
                _eventList = new List<EventObject>();
                UpdateOutputListview();
                ErrorsToolStripButton.Text = "0 Errors.";
                InfoToolStripButton.Text = "0 Messages";
                WarningsToolStripButton.Text = "0 Warnings";
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }
        }
        #endregion

        private void pluginsListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            try
            {
                if ((ITemplate)e.Item.Tag != null)
                {
                    SaveProjectFile();
                    PropertyGrid.SelectedObject = _projectConfig;
                }
            }
            catch (Exception ex)
            {
                AddOutput(ex);
            }

        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            if (_projectFile.Equals(string.Empty))
            {
                AddOutput("No working directory defined.", "Reload project.", EventObject.EventTypeEnum.WarningEvent);
                return;
            }
            LoadProject();
        }

        private void OutputEventsToolStripButton_Click(object sender, EventArgs e)
        {
            UpdateOutputListview();
        }

        private void ToggleControlsEnabling(bool enabled)
        {
            dbObjectsTreeview.Enabled = enabled;
            PropertyGrid.Enabled = enabled;
            pluginsListView.Enabled = enabled;
            OutputToolStrip.Enabled = enabled;
            btnAccept.Enabled = enabled;
            btnCancel.Enabled = enabled;
            btnReload.Enabled = enabled;
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            if(!_workingDir.Equals(string.Empty))
                Process.Start(_workingDir);
            else
                AddOutput("No working directorydefined.", "Open Generation folder.", EventObject.EventTypeEnum.WarningEvent);
        }

        private void MainDialogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveProjectFile();
        }

        private void pluginsListView_DoubleClick(object sender, EventArgs e)
        {
            //MessageBox.Show(sender.ToString());
        }
    }
}
