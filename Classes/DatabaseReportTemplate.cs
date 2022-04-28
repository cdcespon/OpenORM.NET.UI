
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using MyMeta;
using System.IO;
using System.Text;
using System.Xml;

//public delegate void OnExceptionEventHandler(System.Exception Iex);
//public delegate void OnInfoEventHandler(string Description);
//public delegate void OnFileGeneratedEventHandler(string File);
//public delegate void OnPercentDoneEventHandler(double Percent);

public class ReportingPlugin : IPlugin
{
	private List<ITemplate> _templates = new List<ITemplate>();
	const string PluginName = "Database Report Plugin";
	const string PluginDescription = "Database Report Generator Plugin";
	public ReportingPlugin()
	{
		_templates.Add(new HtmlDatabaseReport());
	}
	public string Description {
		get { return PluginName; }
	}

	public string Name {
		get { return PluginDescription; }
	}

	public System.Collections.Generic.List<ITemplate> Templates {
		get { return _templates; }
		set { _templates = value; }
	}

	public class HtmlDatabaseReport : ITemplate
	{
		const string TemplateName = "HTML Database Report Template";
		const string TemplateDescription = "HTML Database Report Generator Template";
		const string TemplateOutputLanguage = "HTML";
		private ArrayList _arraylist = new ArrayList();
		private string _workingdir;
		private string _languageMappingFileName;
		private string _dbTargetMappingFileName;
		private const string GENERATED_FILE_NAME = ".DBReport.html";
		private GenerationProject _generationProject;

        private const string GENERATED_FOLDER_NAME = ".Reports\\";
        bool ITemplate.Execute(MyMeta.IDatabase db, string workingDir, GenerationProject generationProject)
        {
            _workingdir = workingDir;
			_generationProject = generationProject;


			return this.Execute(ref db, workingDir, generationProject);
        }
        public bool Execute(ref MyMeta.IDatabase db, string workingDir, GenerationProject generationProject)
		{
			string databaseName = db.Name;
			MyMeta.ITables tables = db.Tables;
			MyMeta.IViews views = db.Views;
			MyMeta.IProcedures procs = db.Procedures;
			IDatabase _database = db;
			Dictionary<string, string> _tablesDescriptions = new Dictionary<string, string>();


			try {

				_tablesDescriptions = GetTablesDescriptions(tables, _generationProject.ConnectionString);

				System.Text.StringBuilder Output = default(System.Text.StringBuilder);
				Output = new System.Text.StringBuilder();

				Output.AppendLine("");
				Output.AppendLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\"> ");
				Output.AppendLine("<HTML> ");
				Output.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"> ");
				Output.AppendLine("<HEAD> ");
				Output.AppendLine("<TITLE>Tables</TITLE> ");
				//output.Append(output.getPreserveBlock("customHeaders"))
				Output.AppendLine("");
				Output.AppendLine("<STYLE><!-- ");
				Output.AppendLine("body, td, th");
				Output.AppendLine("{");
				Output.AppendLine("" + Convert.ToChar(9) + "font-family:arial;");
				Output.AppendLine("" + Convert.ToChar(9) + "font-size:9pt;");
				Output.AppendLine("}");
				Output.AppendLine("");
				Output.AppendLine("p");
				Output.AppendLine("{");
				Output.AppendLine("" + Convert.ToChar(9) + "margin-top:8px;");
				Output.AppendLine("" + Convert.ToChar(9) + "margin-bottom:8px;");
				Output.AppendLine("");
				Output.AppendLine("}");
				Output.AppendLine("");
				Output.AppendLine("p.authorinfo");
				Output.AppendLine("{");
				Output.AppendLine("" + Convert.ToChar(9) + "color:darkblue;");
				Output.AppendLine("" + Convert.ToChar(9) + "font-size:10pt;");
				Output.AppendLine("" + Convert.ToChar(9) + "font-style:italic;");
				Output.AppendLine("}");
				Output.AppendLine("");
				Output.AppendLine("h1, h2, h3, h4");
				Output.AppendLine("{");
				Output.AppendLine("" + Convert.ToChar(9) + "color:blue;");
				Output.AppendLine("}");
				Output.AppendLine("");
				Output.AppendLine("table ");
				Output.AppendLine("{");
				Output.AppendLine("" + Convert.ToChar(9) + "border: black 1px solid;");
				Output.AppendLine("}");
				Output.AppendLine("");
				Output.AppendLine("th ");
				Output.AppendLine("{");
				Output.AppendLine("" + Convert.ToChar(9) + "color: dark blue;");
				Output.AppendLine("" + Convert.ToChar(9) + "background-color: #9999ff;");
				Output.AppendLine("" + Convert.ToChar(9) + "font-weight: bold;");
				Output.AppendLine("}");
				Output.AppendLine("");
				Output.AppendLine("td");
				Output.AppendLine("{");
				Output.AppendLine("" + Convert.ToChar(9) + "color: dark blue;");
				Output.AppendLine("" + Convert.ToChar(9) + "background-color: #ccccff;");
				Output.AppendLine("}");
				Output.AppendLine("");
				Output.AppendLine("h1");
				Output.AppendLine("{");
				Output.AppendLine("" + Convert.ToChar(9) + "font-size:20pt;");
				Output.AppendLine("" + Convert.ToChar(9) + "margin-bottom:4px;");
				Output.AppendLine("}");
				Output.AppendLine("");
				Output.AppendLine("h2");
				Output.AppendLine("{");
				Output.AppendLine("" + Convert.ToChar(9) + "font-size:16pt;");
				Output.AppendLine("" + Convert.ToChar(9) + "font-style:italic;");
				Output.AppendLine("" + Convert.ToChar(9) + "margin-bottom:2px;");
				Output.AppendLine("}");
				Output.AppendLine("");
				Output.AppendLine("h3");
				Output.AppendLine("{");
				Output.AppendLine("" + Convert.ToChar(9) + "font-size:12pt;");
				Output.AppendLine("" + Convert.ToChar(9) + "font-style:italic;");
				Output.AppendLine("" + Convert.ToChar(9) + "margin-bottom:1px;");
				Output.AppendLine("}");
				Output.AppendLine("");
				Output.AppendLine("h4");
				Output.AppendLine("{");
				Output.AppendLine("" + Convert.ToChar(9) + "font-size:10pt;");
				Output.AppendLine("" + Convert.ToChar(9) + "margin-bottom:1px;");
				Output.AppendLine("}");
				Output.AppendLine("--></STYLE> ");
				Output.AppendLine("</HEAD> ");
				Output.AppendLine("<BODY>");
				Output.Append("<h1>Database Report: ");
				Output.Append(databaseName);
				Output.AppendLine("</h1>");
				Output.Append(DateTime.Now.ToString());
				Output.AppendLine("");
				Output.AppendLine("Generated by OpenORM.NET");
				Output.AppendLine("");
				Output.AppendLine("<a name=\"TOC\"></a>");
				Output.AppendLine("<h3>Table Of Contents</h3>");
				Output.AppendLine("<ul>");
				Output.AppendLine("" + Convert.ToChar(9) + "<li>Tables:");
				Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<ol>");

				if (OnInfo != null) {
					OnInfo("Generating table info...");
				}

				foreach (MyMeta.ITable table in tables) {
					if (table.Selected) {
						Output.AppendLine("");
						Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<li><a href=\"#");
						Output.Append(table.Name);
						Output.Append("\">");
						Output.Append(table.Name);
						Output.AppendLine("</a></li>");
					}
				}

				Output.AppendLine("");
				Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "</ol>");
				Output.AppendLine("" + Convert.ToChar(9) + "</li>");
				Output.AppendLine("" + Convert.ToChar(9) + "<li>Views:");
				Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<ol>");


				foreach (MyMeta.IView view in views) {
					if (view.Selected) {
						Output.AppendLine("");
						Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<li><a href=\"#");
						Output.Append(view.Name);
						Output.Append("\">");
						Output.Append(view.Name);
						Output.AppendLine("</a></li>");
					}
				}

				Output.AppendLine("");
				Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "</ol>");
				Output.AppendLine("" + Convert.ToChar(9) + "</li>");
				Output.AppendLine("" + Convert.ToChar(9) + "<li>Stored Procedures:");
				Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<ol>");

				foreach (MyMeta.Procedure proc in procs) {
					if (proc.Selected) {
						Output.AppendLine("");
						Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<li><a href=\"#PROC_");
						Output.Append(proc.Name);
						Output.Append("\">");
						Output.Append(proc.Name);
						Output.AppendLine("</a></li>");
					}
				}

				Output.AppendLine("");
				Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "</ol>");
				Output.AppendLine("" + Convert.ToChar(9) + "</li>");
				Output.AppendLine("</ul>");

				if (OnInfo != null) {
					OnInfo("Generating table info...");
				}
				
				foreach (MyMeta.ITable tableName in tables) {
					ITable table = _database.Tables[tableName.Name];
					if (table.Selected) {
						Output.AppendLine("");
						Output.AppendLine("<HR width=\"100%\" height=1 color=blue>");
						Output.Append("<a name=\"");
						Output.Append(table.Name);
						Output.AppendLine("\"></a>");
						Output.AppendLine("<a href=\"#TOC\">Table of Contents</a>");
						Output.Append("<H3>Table: ");
						Output.Append(table.Name);
						Output.AppendLine("</H3>");
						Output.Append("<H4>Description: ");
						//Output.Append(table.Description);
						Output.Append(GetTableDescription(table,_generationProject.ConnectionString));
						Output.AppendLine("</H4>");
						Output.AppendLine("<H4>Columns</H4> ");
						Output.AppendLine("<table>");
						Output.AppendLine("" + Convert.ToChar(9) + "<tr>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Column Name</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Data Type</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Is in Key?</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Is Nullable?</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Is Computed?</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Is AutoKey?</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Default</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Description</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "</tr>");


						foreach (IColumn column in table.Columns) {
							Output.AppendLine("");
							Output.AppendLine("" + Convert.ToChar(9) + "<tr>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(column.Name);
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(column.DataTypeNameComplete.ToString());
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append((column.IsInPrimaryKey ? "Yes" : "&nbsp;"));
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append((column.IsNullable ? "Yes" : "&nbsp;"));
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append((column.IsComputed ? "Yes" : "&nbsp;"));
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append((column.IsAutoKey ? "Yes" : "&nbsp;"));
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td valign=top>");
							Output.Append(column.Default);
							Output.AppendLine("&nbsp;</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td valign=top>");
							Output.Append(column.Description);
							Output.AppendLine("&nbsp;</td>");
							Output.AppendLine("" + Convert.ToChar(9) + "</tr>");

						}

						Output.AppendLine("");
						Output.AppendLine("</table>");
						Output.AppendLine("");
						Output.AppendLine("<H4>Foreign Keys</H4> ");
						Output.AppendLine("<table>");
						Output.AppendLine("" + Convert.ToChar(9) + "<tr>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Name</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Foreign Table</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Primary Key</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "</tr>");


						foreach (IForeignKey fkey in table.ForeignKeys) {
							Output.AppendLine("");
							Output.AppendLine("" + Convert.ToChar(9) + "<tr>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(fkey.Name);
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(fkey.ForeignTable.Name);
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(fkey.PrimaryKeyName);
							Output.AppendLine("</td>");
							Output.AppendLine("" + Convert.ToChar(9) + "</tr>");

						}

						Output.AppendLine("");
						Output.AppendLine("</table>");
						Output.AppendLine("");
						Output.AppendLine("<H4>Indexes</H4> ");
						Output.AppendLine("<table>");
						Output.AppendLine("" + Convert.ToChar(9) + "<tr>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Name</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Unique</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Clustered</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Type</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Collation</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Fill Factor</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "</tr>");


						foreach (IIndex index in table.Indexes) {
							Output.AppendLine("");
							Output.AppendLine("" + Convert.ToChar(9) + "<tr>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(index.Name);
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append((index.Unique ? "Yes" : "&nbsp;"));
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append((index.Clustered ? "Yes" : "&nbsp;"));
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(index.Type);
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(index.Collation.ToString());
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(index.FillFactor.ToString());
							Output.AppendLine("</td>");
							Output.AppendLine("" + Convert.ToChar(9) + "</tr>");

						}

						Output.AppendLine("");
						Output.AppendLine("</table>");
					}
				}

				if (OnInfo != null) {
					OnInfo("Generating views info...");
				}

				foreach (MyMeta.IView view_ in views) {
					if (view_.Selected) {
						IView view = _database.Views[view_.Name];

						Output.AppendLine("");
						Output.AppendLine("<HR width=\"100%\" height=1 color=blue>");
						Output.Append("<a name=\"");
						Output.Append(view.Name);
						Output.AppendLine("\"></a>");
						Output.AppendLine("<a href=\"#TOC\">Table of Contents</a>");
						Output.Append("<H3>View: ");
						Output.Append(view.Name);
						Output.AppendLine("</H3>");
						Output.AppendLine("<H4>Columns</H4> ");
						Output.AppendLine("<table>");
						Output.AppendLine("" + Convert.ToChar(9) + "<tr>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Column Name</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Data Type</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Is in Key?</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Is Nullable?</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Is Computed?</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Is AutoKey?</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Default</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Description</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "</tr>");


						foreach (IColumn column in view.Columns) {
							Output.AppendLine("");
							Output.AppendLine("" + Convert.ToChar(9) + "<tr>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(column.Name);
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(column.DataTypeNameComplete.ToString());
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append((column.IsInPrimaryKey ? "Yes" : "&nbsp;"));
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append((column.IsNullable ? "Yes" : "&nbsp;"));
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append((column.IsComputed ? "Yes" : "&nbsp;"));
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append((column.IsAutoKey ? "Yes" : "&nbsp;"));
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td valign=top>");
							Output.Append(column.Default);
							Output.AppendLine("&nbsp;</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td valign=top>");
							Output.Append(column.Description);
							Output.AppendLine("&nbsp;</td>");
							Output.AppendLine("" + Convert.ToChar(9) + "</tr>");

						}

						Output.AppendLine("");
						Output.AppendLine("</table>");
						Output.Append("<pre>");
						Output.Append(view.ViewText);
						Output.AppendLine("</pre>");
					}
				}

				if (OnInfo != null) {
					OnInfo("Generating procedure info...");
				}

				foreach (MyMeta.IProcedure procName in procs) {
					if (procName.Selected) {
						IProcedure proc = _database.Procedures[procName.Name];

						Output.AppendLine("");
						Output.AppendLine("<HR width=\"100%\" height=1 color=blue>");
						Output.Append("<a name=\"PROC_");
						Output.Append(proc.Name);
						Output.AppendLine("\"></a>");
						Output.AppendLine("<a href=\"#TOC\">Table of Contents</a>");
						Output.Append("<H3>Stored Procedure: ");
						Output.Append(proc.Name);
						Output.AppendLine("</H3>");
						Output.AppendLine("<H4>Parameters</H4> ");
						Output.AppendLine("<table>");
						Output.AppendLine("" + Convert.ToChar(9) + "<tr>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Name</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Data Type</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Direction?</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Is Nullable?</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Default</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<th nowrap>Description</th>");
						Output.AppendLine("" + Convert.ToChar(9) + "</tr>");


						foreach (IParameter parm in proc.Parameters) {
							Output.AppendLine("");
							Output.AppendLine("" + Convert.ToChar(9) + "<tr>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(parm.Name);
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(parm.DataTypeNameComplete.ToString());
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append(parm.Direction.ToString());
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td nowrap valign=top>");
							Output.Append((parm.IsNullable ? "Yes" : "&nbsp;"));
							Output.AppendLine("</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td valign=top>");
							Output.Append((parm.Default == null ? "" : parm.Default.ToString()));
							Output.AppendLine("&nbsp;</td>");
							Output.Append("" + Convert.ToChar(9) + "" + Convert.ToChar(9) + "<td valign=top>");
							Output.Append(parm.Description);
							Output.AppendLine("&nbsp;</td>");
							Output.AppendLine("" + Convert.ToChar(9) + "</tr>");

						}

						Output.AppendLine("");
						Output.AppendLine("</table>");
						Output.Append("<pre>");
						Output.Append(proc.ProcedureText);
						Output.AppendLine("</pre>");
					}
				}

				Output.AppendLine("");
				Output.AppendLine("</BODY>");
				Output.AppendLine("</HTML>");
				if (OnInfo != null) {
					OnInfo("Writing to file...");
				}

				if (!_workingdir.EndsWith("\\"))
					_workingdir += "\\";
				System.IO.Directory.CreateDirectory(_workingdir);
				System.IO.StreamWriter sw = new System.IO.StreamWriter(_workingdir + _database.Name + GENERATED_FILE_NAME);
				sw.Write(Output.ToString());
				sw.Close();

				if (OnFileGenerated != null) {
					OnFileGenerated(_workingdir + _database.Name.ToUpper() + GENERATED_FOLDER_NAME + _database.Name.ToUpper() + GENERATED_FILE_NAME);
				}

				if (OnInfo != null) {
					OnInfo(GENERATED_FILE_NAME + " finished.");
				}
			} catch (Exception ex) {
				if (OnException != null) {
					OnException(ex);
				}
			}
            return true;
		}

		public string Name {
			get { return TemplateName; }
		}


		public event OnExceptionEventHandler OnException;

        public string WorkingDir {
			get { return _workingdir; }
			set { _workingdir = value; }
		}

		public string Description {
			get { return TemplateDescription; }
		}

		public event OnInfoEventHandler OnInfo;

		public string OutputLanguage {
			get { return TemplateOutputLanguage; }
		}

		public event OnFileGeneratedEventHandler OnFileGenerated;
		public event OnPercentDoneEventHandler OnPercentDone;
		

		public string DbTargetMappingFileName {
			get { return _dbTargetMappingFileName; }
			set { _dbTargetMappingFileName = value; }
		}

		public string LanguageMappingFileName {
			get { return _languageMappingFileName; }
			set { _languageMappingFileName = value; }
		}

		public string GUID {
			get { return "353e226c-6bf1-4079-a822-ba12163b71c9"; }
		}
		private string GetTableDescription(ITable table, string connectionString)
		{
			string sql = @"SELECT value FROM fn_listextendedproperty(NULL, 'schema', '" + table.Schema + "', 'table', '" + table.Name + "', default, default)";

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
		private Dictionary<string, string> GetTablesDescriptions(ITables tables, string connectionString)
		{
			Dictionary<string, string> returnValue = new Dictionary<string, string>();
			foreach (var table in tables)
			{
				ADODB.Recordset recordset = null;
				ADODB.Connection connection = new ADODB.Connection();
				connection.Open(connectionString);

				if (table.Selected)
				{
					string sql = @"SELECT value FROM fn_listextendedproperty(NULL, 'schema', '" + table.Schema + "', 'table', '" + table.Name + "', default, default)";

					ADODB.Command command = new ADODB.Command();
					command.ActiveConnection = connection;
					command.CommandText = sql;
					command.CommandType = ADODB.CommandTypeEnum.adCmdText;
					command.CommandTimeout = 30;
					object rows;
					recordset = command.Execute(out rows);
					string res = string.Empty;
					while (!recordset.EOF)
					{
						returnValue.Add(table.Schema + "." + table.Name, recordset.Fields["Value"].Value.ToString());
						recordset.MoveNext();
					}
				}
			}
			return returnValue;
		}
	}

}

