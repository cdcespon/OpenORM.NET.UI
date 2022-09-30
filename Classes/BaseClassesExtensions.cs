using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

public class BaseClassesExtensionsBuilder : IPlugin
{
    private List<ITemplate> _templates = new List<ITemplate>();
    const string PluginName = "BaseClasses Extensions Builder";
    const string PluginDescription = "BaseClasses Extensions Builder";
    public BaseClassesExtensionsBuilder()
    {
        _templates.Add(new LogBuilder());
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

    public class LogBuilder : ITemplate
    {
        const string TemplateName = "BaseClasses Extensions Builder";
        const string TemplateDescription = "BaseClasses Extensions Builder";
        const string TemplateoutputLanguage = "C#";
        private ArrayList _arraylist = new ArrayList();
        private string _workingdir = String.Empty;
        private string _languageMappingFileName;
        private string _dbTargetMappingFileName;
        private string _generationFolder = string.Empty;
        private const string generationFolder = "BaseClasses Extensions Generation folder";

        private ArrayList _refkeyList = new ArrayList();
        public LogBuilder()
        { }
        public LogBuilder(GenerationProject generationProject)
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
                
                output = new System.Text.StringBuilder();

                foreach (MyMeta.ITable table in db.Tables)
                {
                    if (table.Selected)
                    {
                        output.AppendLine("    #region " + table.Name);
                        output.AppendLine("");
                        output.AppendLine("	namespace " + generationProject.Namespace + ".Business.Tables." + table.Schema);
                        output.AppendLine("{");
                        output.AppendLine("	/// <summary>");
                        output.AppendLine("	/// ");
                        output.AppendLine("	/// </summary>");
                        output.AppendLine("	public partial class " + table.Name);
                        output.AppendLine("	{");
                        output.AppendLine("		public enum LogTypeEnum {");
                        output.AppendLine("			Ninguno,");
                        output.AppendLine("			Alta,");
                        output.AppendLine("			Baja,");
                        output.AppendLine("			Modificacion");
                        output.AppendLine("		}");
                        output.AppendLine("");
                        output.AppendLine("		public Entities.Tables." + table.Schema  + "." + table.Name + " Add(Entities.Tables." + table.Schema  + "." + table.Name + " item,long aplicacionId)");
                        output.AppendLine("		{");
                        output.AppendLine("			var refItem =  (Entities.Tables." + table.Schema  + "." + table.Name + ")base.Add((IDataItem)item);");
                        output.AppendLine("			LogEvent(aplicacionId, LogTypeEnum.Alta, refItem);");
                        output.AppendLine("			return refItem;");
                        output.AppendLine("		}");
                        output.AppendLine("		public Int64 Update(Entities.Tables." + table.Schema  + "." + table.Name + " item, long aplicacionId)");
                        output.AppendLine("		{");
                        output.AppendLine("			var refItem = base.Update((IDataItem)item);");
                        output.AppendLine("			LogEvent(aplicacionId, LogTypeEnum.Modificacion, item);");
                        output.AppendLine("			return refItem;");
                        output.AppendLine("		}");
                        output.AppendLine("");
                        output.AppendLine("		public Int64 Delete(Int64 id, long aplicacionId)");
                        output.AppendLine("		{");
                        output.AppendLine("			Business.Tables." + table.Schema  + "." + table.Name + " refentities = new();");
                        output.AppendLine("			refentities.Items(id);");
                        output.AppendLine("			var refentity = refentities.Result.FirstOrDefault();");
                        output.AppendLine("			LogEvent(aplicacionId, LogTypeEnum.Baja, refentity);");
                        output.AppendLine("			return  base.DeleteItem(refentity);");
                        output.AppendLine("");
                        output.AppendLine("		}");
                        output.AppendLine("");
                        output.AppendLine("		private int LogEvent(long applicacionId, LogTypeEnum logType, " + generationProject.Namespace + ".Entities.Tables." + table.Schema  + "." + table.Name + " entity)");
                        output.AppendLine("");
                        output.AppendLine("		{");
                        output.AppendLine("			string descripcion = string.Empty;");
                        output.AppendLine("			string metodo = string.Empty;");
                        output.AppendLine("");
                        output.AppendLine("			switch (logType)");
                        output.AppendLine("			{");
                        output.AppendLine("				case LogTypeEnum.Ninguno:");
                        output.AppendLine("					descripcion = " + System.Convert.ToChar(34) + "No especificado" + System.Convert.ToChar(34) + ";");
                        output.AppendLine("					metodo = Enum.GetName(LogTypeEnum.Ninguno);");
                        output.AppendLine("					break;");
                        output.AppendLine("				case LogTypeEnum.Alta:");
                        output.AppendLine("					descripcion = " + System.Convert.ToChar(34) + "Alta de entidad: " + generationProject.Namespace + ".Business." + table.Schema  + "." + table.Name + "" + System.Convert.ToChar(34) + ";");
                        output.AppendLine("					metodo = Enum.GetName(LogTypeEnum.Alta);");
                        output.AppendLine("					break;");
                        output.AppendLine("				case LogTypeEnum.Baja:");
                        output.AppendLine("					descripcion = " + System.Convert.ToChar(34) + "Baja de entidad: " + generationProject.Namespace + ".Business." + table.Schema  + "." + table.Name + "" + System.Convert.ToChar(34) + ";");
                        output.AppendLine("					metodo = Enum.GetName(LogTypeEnum.Baja);");
                        output.AppendLine("					break;");
                        output.AppendLine("				case LogTypeEnum.Modificacion:");
                        output.AppendLine("					descripcion = " + System.Convert.ToChar(34) + "Modificacion de entidad: " + generationProject.Namespace + ".Business." + table.Schema  + "." + table.Name + "" + System.Convert.ToChar(34) + ";");
                        output.AppendLine("					metodo = Enum.GetName(LogTypeEnum.Modificacion);");
                        output.AppendLine("					break;");
                        output.AppendLine("				default:");
                        output.AppendLine("					break;");
                        output.AppendLine("			}");
                        output.AppendLine("");
                        output.AppendLine("			" + generationProject.Namespace + ".Business.Tables.dbo.Log logEntities = new();");
                        output.AppendLine("			logEntities.Add(new()");
                        output.AppendLine("			{");
                        output.AppendLine("				AplicacionId = applicacionId,");
                        output.AppendLine("				Descripcion = descripcion,");
                        output.AppendLine("				Detalle = descripcion,");
                        output.AppendLine("				Fecha = DateTime.Now,");
                        output.AppendLine("				Metodo = metodo,");
                        output.AppendLine("				Modulo = " + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + ",");
                        output.AppendLine("				TipoId = (long)logType,");
                        output.AppendLine("				UsuarioId = entity.UsuarioCreacion == null ? -1 : (long)entity.UsuarioCreacion");
                        output.AppendLine("			});");
                        output.AppendLine("");
                        output.AppendLine("			return 0;");
                        output.AppendLine("        }");
                        output.AppendLine("    }");
                        output.AppendLine("}");
                        output.AppendLine("");

                        output.AppendLine("    #endregion");
                        output.AppendLine("");


                    }
                    
                }
                
                SaveOutputToFile("Business.Tables.Extensions.cs", output, relativepath, true);
                
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
            get { return "c6885894-f5e6-505c-bc2d-480cd6d456a1"; }
        }
        #region ITemplate Members

        public string OutputLanguage
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
