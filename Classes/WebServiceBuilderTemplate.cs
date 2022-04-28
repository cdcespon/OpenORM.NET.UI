using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

public class WebServiceBuilderPlugin : IPlugin
{
    private List<ITemplate> _templates = new List<ITemplate>();
    const string PluginName = "WebServiceBuilder";
    const string PluginDescription = "WebService Builder Plugin";
    public WebServiceBuilderPlugin()
    {
        _templates.Add(new WebServiceBuilderTemplate());
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

    public class WebServiceBuilderTemplate : ITemplate
    {
        const string TemplateName = "WebServiceBuilder Template";
        const string TemplateDescription = "WebService ASP.NET builder Template";
        const string TemplateoutputLanguage = "C#";
        private ArrayList _arraylist = new ArrayList();
        private string _workingdir;
        private string _languageMappingFileName;
        private string _dbTargetMappingFileName;
        private const string  webServiceBase = "Base Web Service";
        private string _webServiceBase = "System.Web.Services.WebService";

        private ArrayList _refkeyList = new ArrayList();
        GenerationProject _generationProject = null;

        public WebServiceBuilderTemplate()
        { }
        public WebServiceBuilderTemplate(GenerationProject generationProject)
        {

            TemplateConfigurationEntry webServiceBaseEntry = generationProject.TemplateConfigurationEntries.Find(e => e.Template.Equals(TemplateName) && e.Name.Equals(webServiceBase));

            if (webServiceBaseEntry == null)
                generationProject.TemplateConfigurationEntries.Add(new TemplateConfigurationEntry(TemplateName, webServiceBase, _webServiceBase));
            else
                _webServiceBase = webServiceBaseEntry.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="workingDir"></param>
        /// <param name="generationProject"></param>
        /// <returns></returns>
        public bool Execute(ref MyMeta.IDatabase db, string workingDir, GenerationProject generationProject)
        {
            try
            {
                _workingdir = workingDir;
                MyMeta.IDatabase _dataBase = db;
                _generationProject = generationProject;
                db.Root.DbTarget = GetDbTarget(db.Root.Driver);
                db.Root.Language = GetLanguage(db.Root.Driver);//"C# Types";

                TemplateConfigurationEntry webServiceBaseEntry = generationProject.TemplateConfigurationEntries.Find(e => e.Template.Equals(TemplateName) && e.Name.Equals(webServiceBase));

                 _webServiceBase = webServiceBaseEntry.Value;

                BuildTablesServices(db, generationProject);
                BuildViewsServices(db, generationProject);
                BuildProceduresServices(db, generationProject);
                BuildMappedProceduresServices(db, generationProject);

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="generationProject"></param>
        private void BuildTablesServices(MyMeta.IDatabase db, GenerationProject generationProject)
        {
            foreach (MyMeta.ITable entity in db.Tables)
            {
                if (entity.Selected)
                {
                    System.Text.StringBuilder output = new System.Text.StringBuilder();

                    output.AppendLine("<%@ WebService Language=" + System.Convert.ToChar(34) + "C#" + System.Convert.ToChar(34) + " Class=" + System.Convert.ToChar(34) + generationProject.Namespace + "." + entity.Schema + "_" + entity.Name + "service" + System.Convert.ToChar(34) + " %>");
                    output.AppendLine("using System;");
                    output.AppendLine("using System.Collections.Generic;");
                    output.AppendLine("using System.Linq;");
                    output.AppendLine("using System.Web;");
                    output.AppendLine("using System.Web.Script.Services;");
                    output.AppendLine("using System.Web.Services;");
                    output.AppendLine("namespace " + generationProject.Namespace);
                    output.AppendLine("{");
                    output.AppendLine("    /// <summary>");
                    output.AppendLine("    /// ");
                    output.AppendLine("    /// </summary>");
                    output.AppendLine("    [WebService(Namespace = " + System.Convert.ToChar(34) + generationProject.Namespace + ".com " + System.Convert.ToChar(34) + ")]");
                    output.AppendLine("    //[System.ComponentModel.ToolboxItem(false)]");
                    output.AppendLine("    [System.Web.Script.Services.ScriptService]");
                    output.AppendLine("    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. ");
                    output.AppendLine("    // [System.Web.Script.Services.ScriptService]");
                    output.AppendLine("    public class " + entity.Schema + "_" + entity.Name + "service : " + _webServiceBase ); 
                    output.AppendLine("    {");
                    output.AppendLine("        [WebMethod]");
                    output.AppendLine("        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]");
                    output.AppendLine("        public void items()");
                    output.AppendLine("        {");
                    //output.AppendLine("            Context.Response.Clear();");
                    //output.AppendLine("            Context.Response.ContentType = " + System.Convert.ToChar(34) + "application/json" + System.Convert.ToChar(34) + ";");
                    //output.AppendLine("            if (Context.Response.Headers[" + System.Convert.ToChar(34) + "Access-Control-Allow-Origin" + System.Convert.ToChar(34) + "] == null)");
                    //output.AppendLine("                Context.Response.AddHeader(" + System.Convert.ToChar(34) + "Access-Control-Allow-Origin" + System.Convert.ToChar(34) + ", " + System.Convert.ToChar(34) + "*" + System.Convert.ToChar(34) + ");");
                    output.AppendLine("           Int64 usuarioID = ValidarUsuario_ObtenerID();"); 
                    output.AppendLine("           try");
                    output.AppendLine("              {");
                    output.AppendLine("");
                    output.AppendLine("               if (usuarioID != 0)");
                    output.AppendLine("               {");
                    
                    
                    string fields = String.Empty;
                    foreach (MyMeta.IColumn item in entity.Columns)
                    {
                        switch (item.LanguageType)
                        {
                            case "String":
                                output.AppendLine("                     " + item.LanguageType + " " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0] : null;");
                                break;

                            default:
                                output.AppendLine("                     " + item.LanguageType + "? " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Convert.To" + item.LanguageType + "(Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0]) : new " + item.LanguageType + "?();");
                                break;
                        }
                        fields += item.Name + ",";
                    }
                    if (fields.Length > 0)
                        fields = fields.Substring(0, fields.Length - 1);

                    output.AppendLine("                     switch (Context.Request.HttpMethod)");
                    output.AppendLine("                     {");
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Connect:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Get:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Head:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.MkCol:") ;
                    //output.AppendLine("                    break;") ;
                    output.AppendLine("                         case System.Net.WebRequestMethods.Http.Post:");
                    output.AppendLine("                             " + generationProject.Namespace + ".Business.Tables." + entity.Schema + "." + entity.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + entity.Schema + "." + entity.Name + "();");
                    //output.AppendLine("                         Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(entities.Items(" + fields + ")));");
                    output.AppendLine("                             InicializarDti(TipoInformacion.Ok, Newtonsoft.Json.JsonConvert.SerializeObject(entities.Items(" + fields + ")));");
                    output.AppendLine("                             break;");
                    output.AppendLine("                         case System.Net.WebRequestMethods.Http.Put:");
                    output.AppendLine("                             break;");
                    output.AppendLine("                         default:");
                    output.AppendLine("                             break;");
                    output.AppendLine("                      }");
                    output.AppendLine("                    }");
                    output.AppendLine("                    else");
                    output.AppendLine("                    {");
                    output.AppendLine("                        InicializarDti(TipoInformacion.SessionIdInvalido, ControladorAuditoria.ObtenerDescripcionError((int)TipoInformacion.SessionIdInvalido));");
                    output.AppendLine("                    }");
                    output.AppendLine("                }");
                    output.AppendLine("                catch (Exception ex)");
                    output.AppendLine("                {");
                    output.AppendLine("                    ControladorAuditoria.RegistrarLog(ControladorAuditoria.TipoLog.Excepcion, ex.Message, (ex.InnerException == null ? " + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " : ex.InnerException.Source.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.TargetSite.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.Message) + " + System.Convert.ToChar(34) + " // " + System.Convert.ToChar(34) + " + ex.StackTrace, usuarioID);");
                    output.AppendLine("                    InicializarDti(TipoInformacion.Excepcion, ex.Message);");
                    output.AppendLine("                }");
                    output.AppendLine("                finally");
                    output.AppendLine("                {");
                    output.AppendLine("                    RetornarDatos();");
                    output.AppendLine("                }");
                    output.AppendLine("          }");
                    output.AppendLine("    /// <summary>");
                    output.AppendLine("    /// ");
                    output.AppendLine("    /// </summary>");
                    output.AppendLine("        [WebMethod]");
                    output.AppendLine("        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]");
                    output.AppendLine("        public void add()");
                    output.AppendLine("        {");
                    output.AppendLine("           Int64 usuarioID = ValidarUsuario_ObtenerID();");
                    output.AppendLine("           try");
                    output.AppendLine("              {");
                    output.AppendLine("");
                    output.AppendLine("                 if (usuarioID != 0)");
                    output.AppendLine("                 {");

                    output.AppendLine("			   //Data passed to ws");

                    fields = String.Empty;
                    foreach (MyMeta.IColumn item in entity.Columns)
                    {
                        if (!item.IsAutoKey)
                        {
                            switch (item.LanguageType)
                            {
                                case "String":
                                    output.AppendLine("             " + item.LanguageType + " " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0] : null;");
                                    fields += item.Name + ",";
                                    break;

                                default:
                                    output.AppendLine("             " + item.LanguageType + "? " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Convert.To" + item.LanguageType + "(Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0]) : new " + item.LanguageType + "?();");
                                    fields += "(" + item.LanguageType + ")"+ item.Name + ",";
                                    break;
                            }
                            
                        }
                    }

                    if (fields.Length > 0)
                        fields = fields.Substring(0, fields.Length - 1);

                    output.AppendLine("             switch (Context.Request.HttpMethod)");
                    output.AppendLine("             {");
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Connect:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Get:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Head:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.MkCol:") ;
                    //output.AppendLine("                    break;") ;
                    output.AppendLine("                 case System.Net.WebRequestMethods.Http.Post:");
                    output.AppendLine("                     " + generationProject.Namespace + ".Business.Tables." + entity.Schema + "." + entity.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + entity.Schema + "." + entity.Name + "();");
                    //output.AppendLine("                    Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(entities.Add(" + fields + ")));");
                    output.AppendLine("                     InicializarDti(TipoInformacion.Ok, Newtonsoft.Json.JsonConvert.SerializeObject(entities.Add(" + fields + ")));");
                    output.AppendLine("                     break;");
                    output.AppendLine("                 case System.Net.WebRequestMethods.Http.Put:");
                    output.AppendLine("                     break;");
                    output.AppendLine("                         default:");
                    output.AppendLine("                             break;");
                    output.AppendLine("                      }");
                    output.AppendLine("                    }");
                    output.AppendLine("                    else");
                    output.AppendLine("                    {");
                    output.AppendLine("                        InicializarDti(TipoInformacion.SessionIdInvalido, ControladorAuditoria.ObtenerDescripcionError((int)TipoInformacion.SessionIdInvalido));");
                    output.AppendLine("                    }");
                    output.AppendLine("                }");
                    output.AppendLine("                catch (Exception ex)");
                    output.AppendLine("                {");
                    output.AppendLine("                    ControladorAuditoria.RegistrarLog(ControladorAuditoria.TipoLog.Excepcion, ex.Message, (ex.InnerException == null ? " + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " : ex.InnerException.Source.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.TargetSite.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.Message) + " + System.Convert.ToChar(34) + " // " + System.Convert.ToChar(34) + " + ex.StackTrace, usuarioID);");
                    output.AppendLine("                    InicializarDti(TipoInformacion.Excepcion, ex.Message);");
                    output.AppendLine("                }");
                    output.AppendLine("                finally");
                    output.AppendLine("                {");
                    output.AppendLine("                    RetornarDatos();");
                    output.AppendLine("                }");
                    output.AppendLine("          }"); ;
                    output.AppendLine("    /// <summary>");
                    output.AppendLine("    /// ");
                    output.AppendLine("    /// </summary>");
                    output.AppendLine("        [WebMethod]");
                    output.AppendLine("        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]");
                    output.AppendLine("        public void update()");
                    output.AppendLine("        {");
                    output.AppendLine("           Int64 usuarioID = ValidarUsuario_ObtenerID();");
                    output.AppendLine("           try");
                    output.AppendLine("              {");
                    output.AppendLine("");
                    output.AppendLine("                 if (usuarioID != 0)");
                    output.AppendLine("                 {");
                    output.AppendLine("			//Data passed to ws");
                    fields = String.Empty;
                    foreach (MyMeta.IColumn item in entity.Columns)
                    {
                        switch (item.LanguageType)
                        {
                            case "String":
                                output.AppendLine("            " + item.LanguageType + " " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0] : null;");
                                fields += item.Name + ",";
                                break;

                            default:
                                output.AppendLine("            " + item.LanguageType + "? " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Convert.To" + item.LanguageType + "(Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0]) : new " + item.LanguageType + "?();");
                                fields += "(" + item.LanguageType + ")" + item.Name + ",";
                                break;
                        }
                        
                    }

                    if (fields.Length > 0)
                        fields = fields.Substring(0, fields.Length - 1);


                    output.AppendLine("            switch (Context.Request.HttpMethod)");
                    output.AppendLine("            {");
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Connect:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Get:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Head:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.MkCol:") ;
                    //output.AppendLine("                    break;") ;
                    output.AppendLine("                case System.Net.WebRequestMethods.Http.Post:");
                    output.AppendLine("                    " + generationProject.Namespace + ".Business.Tables." + entity.Schema + "." + entity.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + entity.Schema + "." + entity.Name + "();");
                    //output.AppendLine("                    Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(entities.Update(" + fields + ")));");
                    output.AppendLine("                     InicializarDti(TipoInformacion.Ok, Newtonsoft.Json.JsonConvert.SerializeObject(entities.Update(" + fields + ")));");
                    output.AppendLine("                    break;");
                    output.AppendLine("                case System.Net.WebRequestMethods.Http.Put:");
                    output.AppendLine("                    break;");
                    output.AppendLine("                         default:");
                    output.AppendLine("                             break;");
                    output.AppendLine("                      }");
                    output.AppendLine("                    }");
                    output.AppendLine("                    else");
                    output.AppendLine("                    {");
                    output.AppendLine("                        InicializarDti(TipoInformacion.SessionIdInvalido, ControladorAuditoria.ObtenerDescripcionError((int)TipoInformacion.SessionIdInvalido));");
                    output.AppendLine("                    }");
                    output.AppendLine("                }");
                    output.AppendLine("                catch (Exception ex)");
                    output.AppendLine("                {");
                    output.AppendLine("                    ControladorAuditoria.RegistrarLog(ControladorAuditoria.TipoLog.Excepcion, ex.Message, (ex.InnerException == null ? " + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " : ex.InnerException.Source.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.TargetSite.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.Message) + " + System.Convert.ToChar(34) + " // " + System.Convert.ToChar(34) + " + ex.StackTrace, usuarioID);");
                    output.AppendLine("                    InicializarDti(TipoInformacion.Excepcion, ex.Message);");
                    output.AppendLine("                }");
                    output.AppendLine("                finally");
                    output.AppendLine("                {");
                    output.AppendLine("                    RetornarDatos();");
                    output.AppendLine("                }");
                    output.AppendLine("          }"); ;
                    output.AppendLine("    /// <summary>");
                    output.AppendLine("    /// ");
                    output.AppendLine("    /// </summary>");
                    output.AppendLine("        [WebMethod]");
                    output.AppendLine("        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]");
                    output.AppendLine("        public void delete()");
                    output.AppendLine("        {");
                    output.AppendLine("           Int64 usuarioID = ValidarUsuario_ObtenerID();");
                    output.AppendLine("           try");
                    output.AppendLine("              {");
                    output.AppendLine("");
                    output.AppendLine("                 if (usuarioID != 0)");
                    output.AppendLine("                 {");

                    output.AppendLine("			//Data passed to ws");
                    fields = string.Empty;
                    foreach (MyMeta.IColumn item in entity.Columns)
                    {
                        if (item.IsInPrimaryKey)
                        {
                            switch (item.LanguageType)
                            {
                                case "String":
                                    output.AppendLine("            " + item.LanguageType + " " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0] : null;");
                                    fields += item.Name + ",";
                                    break;
                                default:
                                    output.AppendLine("            " + item.LanguageType + "? " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Convert.To" + item.LanguageType + "(Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0]) : new " + item.LanguageType + "?();");
                                    fields += "(" + item.LanguageType + ")" + item.Name + ",";
                                    break;
                            }
                        }
                    }
                    if (fields.Length > 0)
                        fields = fields.Substring(0, fields.Length - 1);

                    output.AppendLine("            switch (Context.Request.HttpMethod)");
                    output.AppendLine("            {");
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Connect:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Get:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Head:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.MkCol:") ;
                    //output.AppendLine("                    break;") ;
                    output.AppendLine("                case System.Net.WebRequestMethods.Http.Post:");
                    output.AppendLine("                    " + generationProject.Namespace + ".Business.Tables." + entity.Schema + "." + entity.Name + " entities = new " + generationProject.Namespace + ".Business.Tables." + entity.Schema + "." + entity.Name + "();");
                    //output.AppendLine("                    Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(entities.Delete(" + fields + ")));");
                    output.AppendLine("                     InicializarDti(TipoInformacion.Ok, Newtonsoft.Json.JsonConvert.SerializeObject(entities.Delete(" + fields + ")));");
                    output.AppendLine("                    break;");
                    output.AppendLine("                case System.Net.WebRequestMethods.Http.Put:");
                    output.AppendLine("                    break;");
                    output.AppendLine("                         default:");
                    output.AppendLine("                             break;");
                    output.AppendLine("                      }");
                    output.AppendLine("                    }");
                    output.AppendLine("                    else");
                    output.AppendLine("                    {");
                    output.AppendLine("                        InicializarDti(TipoInformacion.SessionIdInvalido, ControladorAuditoria.ObtenerDescripcionError((int)TipoInformacion.SessionIdInvalido));");
                    output.AppendLine("                    }");
                    output.AppendLine("                }");
                    output.AppendLine("                catch (Exception ex)");
                    output.AppendLine("                {");
                    output.AppendLine("                    ControladorAuditoria.RegistrarLog(ControladorAuditoria.TipoLog.Excepcion, ex.Message, (ex.InnerException == null ? " + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " : ex.InnerException.Source.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.TargetSite.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.Message) + " + System.Convert.ToChar(34) + " // " + System.Convert.ToChar(34) + " + ex.StackTrace, usuarioID);");
                    output.AppendLine("                    InicializarDti(TipoInformacion.Excepcion, ex.Message);");
                    output.AppendLine("                }");
                    output.AppendLine("                finally");
                    output.AppendLine("                {");
                    output.AppendLine("                    RetornarDatos();");
                    output.AppendLine("                }");
                    output.AppendLine("          }"); ;
                    output.AppendLine("    }");
                    output.AppendLine("}");

                    string relativepath = db.Name.ToLower() + "/" + entity.Schema + "/"; 
                    SaveOutputToFile(db.Name.ToLower() + "." + entity.Schema + "." + entity.Name.ToLower() + ".asmx", relativepath,output, true);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="generationProject"></param>
        private void BuildViewsServices(MyMeta.IDatabase db, GenerationProject generationProject)
        {
            foreach (MyMeta.IView entity in db.Views)
            {
                if (entity.Selected)
                {
                    System.Text.StringBuilder output = new System.Text.StringBuilder();

                    output.AppendLine("<%@ WebService Language=" + System.Convert.ToChar(34) + "C#" + System.Convert.ToChar(34) + " Class=" + System.Convert.ToChar(34) + generationProject.Namespace + "." + entity.Name + "service" + System.Convert.ToChar(34) + " %>");
                    output.AppendLine("using System;");
                    output.AppendLine("using System.Collections.Generic;");
                    output.AppendLine("using System.Linq;");
                    output.AppendLine("using System.Web;");
                    output.AppendLine("using System.Web.Script.Services;");
                    output.AppendLine("using System.Web.Services;");
                    output.AppendLine("namespace " + generationProject.Namespace);
                    output.AppendLine("{");
                    output.AppendLine("    /// <summary>");
                    output.AppendLine("    /// ");
                    output.AppendLine("    /// </summary>");
                    output.AppendLine("    [WebService(Namespace = " + System.Convert.ToChar(34) + generationProject.Namespace + ".com " + System.Convert.ToChar(34) + ")]");
                    output.AppendLine("    //[System.ComponentModel.ToolboxItem(false)]");
                    output.AppendLine("    [System.Web.Script.Services.ScriptService]");
                    output.AppendLine("    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. ");
                    output.AppendLine("    // [System.Web.Script.Services.ScriptService]");
                    output.AppendLine("    public class " + entity.Name + "service : " + _webServiceBase);
                    output.AppendLine("    {");
                    output.AppendLine("        [WebMethod]");
                    output.AppendLine("        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]");
                    output.AppendLine("        public void items()");
                    output.AppendLine("        {");
                    //output.AppendLine("            Context.Response.Clear();");
                    //output.AppendLine("            Context.Response.ContentType = " + System.Convert.ToChar(34) + "application/json" + System.Convert.ToChar(34) + ";");
                    //output.AppendLine("            if (Context.Response.Headers[" + System.Convert.ToChar(34) + "Access-Control-Allow-Origin" + System.Convert.ToChar(34) + "] == null)");
                    //output.AppendLine("                Context.Response.AddHeader(" + System.Convert.ToChar(34) + "Access-Control-Allow-Origin" + System.Convert.ToChar(34) + ", " + System.Convert.ToChar(34) + "*" + System.Convert.ToChar(34) + ");");

                    output.AppendLine("             Int64 usuarioID = ValidarUsuario_ObtenerID();");
                    output.AppendLine("             try");
                    output.AppendLine("                 {");
                    output.AppendLine("");
                    output.AppendLine("                 if (usuarioID != 0)");
                    output.AppendLine("                 {");


                    string fields = String.Empty;
                    foreach (MyMeta.IColumn item in entity.Columns)
                    {
                        switch (item.LanguageType)
                        {
                            case "String":
                                output.AppendLine("                 " + item.LanguageType + " " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0] : null;");
                                break;

                            default:
                                output.AppendLine("                 " + item.LanguageType + "? " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Convert.To" + item.LanguageType + "(Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0]) : new " + item.LanguageType + "?();");
                                break;
                        }
                        fields += item.Name + ",";
                    }
                    if (fields.Length > 0)
                        fields = fields.Substring(0, fields.Length - 1);

                    output.AppendLine("                 switch (Context.Request.HttpMethod)");
                    output.AppendLine("                 {");
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Connect:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Get:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Head:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.MkCol:") ;
                    //output.AppendLine("                    break;") ;
                    output.AppendLine("                     case System.Net.WebRequestMethods.Http.Post:");
                    output.AppendLine("                         " + generationProject.Namespace + ".Business.Views." + entity.Schema + "." + entity.Name + " entities = new " + generationProject.Namespace + ".Business.Views." + entity.Schema + "." + entity.Name + "();");
                    output.AppendLine("                             InicializarDti(TipoInformacion.Ok, Newtonsoft.Json.JsonConvert.SerializeObject(entities.Items(" + fields + ")));");
                    //output.AppendLine("                    Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(entities.Items(" + fields + ")));");
                    output.AppendLine("                         break;");
                    output.AppendLine("                     case System.Net.WebRequestMethods.Http.Put:");
                    output.AppendLine("                         break;");
                    output.AppendLine("                                 default:");
                    output.AppendLine("                                 break;");
                    output.AppendLine("                             }");
                    output.AppendLine("                         }");
                    output.AppendLine("                         else");
                    output.AppendLine("                         {");
                    output.AppendLine("                             InicializarDti(TipoInformacion.SessionIdInvalido, ControladorAuditoria.ObtenerDescripcionError((int)TipoInformacion.SessionIdInvalido));");
                    output.AppendLine("                         }");
                    output.AppendLine("                     }");
                    output.AppendLine("                     catch (Exception ex)");
                    output.AppendLine("                     {");
                    output.AppendLine("                         ControladorAuditoria.RegistrarLog(ControladorAuditoria.TipoLog.Excepcion, ex.Message, (ex.InnerException == null ? " + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " : ex.InnerException.Source.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.TargetSite.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.Message) + " + System.Convert.ToChar(34) + " // " + System.Convert.ToChar(34) + " + ex.StackTrace, usuarioID);");
                    output.AppendLine("                         InicializarDti(TipoInformacion.Excepcion, ex.Message);");
                    output.AppendLine("                     }");
                    output.AppendLine("                     finally");
                    output.AppendLine("                     {");
                    output.AppendLine("                         RetornarDatos();");
                    output.AppendLine("                     }");
                    output.AppendLine("                 }");
                    output.AppendLine("             }");
                    output.AppendLine("  }");

                    string relativepath = db.Name.ToLower() + "/" + entity.Schema + "/"; 
                    SaveOutputToFile(db.Name.ToLower() + "." + entity.Name.ToLower() + ".asmx", relativepath,output, true);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="generationProject"></param>
        private void BuildProceduresServices(MyMeta.IDatabase db, GenerationProject generationProject)
        {
            foreach (MyMeta.IProcedure entity in db.Procedures)
            {
                if (entity.Selected)
                {
                    System.Text.StringBuilder output = new System.Text.StringBuilder();

                    output.AppendLine("<%@ WebService Language=" + System.Convert.ToChar(34) + "C#" + System.Convert.ToChar(34) + " Class=" + System.Convert.ToChar(34) + generationProject.Namespace + "." + entity.Name + "service" + System.Convert.ToChar(34) + " %>");
                    output.AppendLine("using System;");
                    output.AppendLine("using System.Collections.Generic;");
                    output.AppendLine("using System.Linq;");
                    output.AppendLine("using System.Web;");
                    output.AppendLine("using System.Web.Script.Services;");
                    output.AppendLine("using System.Web.Services;");
                    output.AppendLine("namespace " + generationProject.Namespace);
                    output.AppendLine("{");
                    output.AppendLine("    /// <summary>");
                    output.AppendLine("    /// ");
                    output.AppendLine("    /// </summary>");
                    output.AppendLine("    [WebService(Namespace = " + System.Convert.ToChar(34) + generationProject.Namespace + ".com " + System.Convert.ToChar(34) + ")]");
                    output.AppendLine("    //[System.ComponentModel.ToolboxItem(false)]");
                    output.AppendLine("    [System.Web.Script.Services.ScriptService]");
                    output.AppendLine("    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. ");
                    output.AppendLine("    // [System.Web.Script.Services.ScriptService]");
                    output.AppendLine("    public class " + entity.Name + "service : " + _webServiceBase);
                    output.AppendLine("    {");
                    output.AppendLine("        [WebMethod]");
                    output.AppendLine("        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]");
                    output.AppendLine("        public void items()");
                    output.AppendLine("        {");
                    //output.AppendLine("            Context.Response.Clear();");
                    //output.AppendLine("            Context.Response.ContentType = " + System.Convert.ToChar(34) + "application/json" + System.Convert.ToChar(34) + ";");
                    //output.AppendLine("            if (Context.Response.Headers[" + System.Convert.ToChar(34) + "Access-Control-Allow-Origin" + System.Convert.ToChar(34) + "] == null)");
                    //output.AppendLine("                Context.Response.AddHeader(" + System.Convert.ToChar(34) + "Access-Control-Allow-Origin" + System.Convert.ToChar(34) + ", " + System.Convert.ToChar(34) + "*" + System.Convert.ToChar(34) + ");");
                    output.AppendLine("           Int64 usuarioID = ValidarUsuario_ObtenerID();");
                    output.AppendLine("           try");
                    output.AppendLine("              {");
                    output.AppendLine("");
                    output.AppendLine("               if (usuarioID != 0)");
                    output.AppendLine("               {");

                    output.AppendLine("");


                    string parameters = String.Empty;
                    foreach (MyMeta.IParameter item in entity.Parameters)
                    {
                        if (!item.Name.Equals("@RETURN_VALUE"))
                        {
                            switch (item.LanguageType)
                            {
                                case "String":
                                    output.AppendLine("            " + item.LanguageType + " " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0] : null;");
                                    break;

                                default:
                                    output.AppendLine("            " + item.LanguageType + "? " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Convert.To" + item.LanguageType + "(Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0]) : new " + item.LanguageType + "?();");
                                    break;
                            }
                            parameters += item.Name.Replace("@", "") + ",";
                        }
                    }

                    if (parameters.Length > 0)
                        parameters = parameters.Substring(0, parameters.Length - 1);


                    output.AppendLine("            switch (Context.Request.HttpMethod)");
                    output.AppendLine("            {");
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Connect:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Get:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Head:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.MkCol:") ;
                    //output.AppendLine("                    break;") ;
                    output.AppendLine("                case System.Net.WebRequestMethods.Http.Post:");
                    output.AppendLine("                    " + generationProject.Namespace + ".Business.Procedures." + entity.Schema + "." + entity.Name + " entities = new " + generationProject.Namespace + ".Business.Procedures." + entity.Schema + "." + entity.Name + "();");
                    //output.AppendLine("                    Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(entities.Items(" + parameters + ")));");
                    output.AppendLine("                     InicializarDti(TipoInformacion.Ok, Newtonsoft.Json.JsonConvert.SerializeObject(entities.Items(" + parameters + ")));");
                    output.AppendLine("                    break;");
                    output.AppendLine("                case System.Net.WebRequestMethods.Http.Put:");
                    output.AppendLine("                    break;");
                    output.AppendLine("                         default:");
                    output.AppendLine("                             break;");
                    output.AppendLine("                      }");
                    output.AppendLine("                    }");
                    output.AppendLine("                    else");
                    output.AppendLine("                    {");
                    output.AppendLine("                        InicializarDti(TipoInformacion.SessionIdInvalido, ControladorAuditoria.ObtenerDescripcionError((int)TipoInformacion.SessionIdInvalido));");
                    output.AppendLine("                    }");
                    output.AppendLine("                }");
                    output.AppendLine("                catch (Exception ex)");
                    output.AppendLine("                {");
                    output.AppendLine("                    ControladorAuditoria.RegistrarLog(ControladorAuditoria.TipoLog.Excepcion, ex.Message, (ex.InnerException == null ? " + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " : ex.InnerException.Source.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.TargetSite.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.Message) + " + System.Convert.ToChar(34) + " // " + System.Convert.ToChar(34) + " + ex.StackTrace, usuarioID);");
                    output.AppendLine("                    InicializarDti(TipoInformacion.Excepcion, ex.Message);");
                    output.AppendLine("                }");
                    output.AppendLine("                finally");
                    output.AppendLine("                {");
                    output.AppendLine("                    RetornarDatos();");
                    output.AppendLine("                }");
                    output.AppendLine("          }");
                    output.AppendLine("    }");
                    output.AppendLine(" }");

                    string relativepath = db.Name.ToLower() + "/" + entity.Schema + "/";
                    SaveOutputToFile(db.Name.ToLower() + "." + entity.Name.ToLower() + ".asmx", relativepath, output, true);
                }
            }
        }
     /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="generationProject"></param>
        private void BuildMappedProceduresServices(MyMeta.IDatabase db, GenerationProject generationProject)
        {
            string schemaName = String.Empty;

            foreach (MappedStoredProcedure entity in _generationProject.MappedStoredProcedures)
            {
                    System.Text.StringBuilder output = new System.Text.StringBuilder();

                    schemaName = GetSchemaName(db, entity.Name);

                    output.AppendLine("<%@ WebService Language=" + System.Convert.ToChar(34) + "C#" + System.Convert.ToChar(34) + " Class=" + System.Convert.ToChar(34) + generationProject.Namespace + "." + entity.Name + "service" + System.Convert.ToChar(34) + " %>");
                    output.AppendLine("using System;");
                    output.AppendLine("using System.Collections.Generic;");
                    output.AppendLine("using System.Linq;");
                    output.AppendLine("using System.Web;");
                    output.AppendLine("using System.Web.Script.Services;");
                    output.AppendLine("using System.Web.Services;");
                    output.AppendLine("namespace " + generationProject.Namespace);
                    output.AppendLine("{");
                    output.AppendLine("    /// <summary>");
                    output.AppendLine("    /// ");
                    output.AppendLine("    /// </summary>");
                    output.AppendLine("    [WebService(Namespace = " + System.Convert.ToChar(34) + generationProject.Namespace + ".com " + System.Convert.ToChar(34) + ")]");
                    output.AppendLine("    //[System.ComponentModel.ToolboxItem(false)]");
                    output.AppendLine("    [System.Web.Script.Services.ScriptService]");
                    output.AppendLine("    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. ");
                    output.AppendLine("    // [System.Web.Script.Services.ScriptService]");
                    output.AppendLine("    public class " + entity.Name.Substring(schemaName.Length + 1) + "service : " + _webServiceBase);
                    output.AppendLine("    {");
                    output.AppendLine("        [WebMethod]");
                    output.AppendLine("        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]");
                    output.AppendLine("        public void items()");
                    output.AppendLine("        {");
                    //output.AppendLine("            Context.Response.Clear();");
                    //output.AppendLine("            Context.Response.ContentType = " + System.Convert.ToChar(34) + "application/json" + System.Convert.ToChar(34) + ";");
                    //output.AppendLine("            if (Context.Response.Headers[" + System.Convert.ToChar(34) + "Access-Control-Allow-Origin" + System.Convert.ToChar(34) + "] == null)");
                    //output.AppendLine("                Context.Response.AddHeader(" + System.Convert.ToChar(34) + "Access-Control-Allow-Origin" + System.Convert.ToChar(34) + ", " + System.Convert.ToChar(34) + "*" + System.Convert.ToChar(34) + ");");
                    output.AppendLine("             Int64 usuarioID = ValidarUsuario_ObtenerID();");
                    output.AppendLine("             try");
                    output.AppendLine("                 {");
                    output.AppendLine("");
                    output.AppendLine("                 if (usuarioID != 0)");
                    output.AppendLine("                 {");

                    output.AppendLine("");


                    string parameters = String.Empty;
                    foreach (MappedStoredProcedure.ProcedureParameter item in entity.ProcedureParameters)
                    {
                        if (!item.Name.Equals("@RETURN_VALUE"))
                        {
                            switch (item.DataType.ToString())
                            {
                                case "String":
                                    output.AppendLine("                 " + item.DataType.ToString() + " " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0] : null;");
                                    break;

                                default:
                                    output.AppendLine("                 " + item.DataType.ToString() + "? " + item.Name + " = Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ") != null ? Convert.To" + item.DataType.ToString() + "(Context.Request.Form.GetValues(" + System.Convert.ToChar(34) + item.Name + System.Convert.ToChar(34) + ")[0]) : new " + item.DataType.ToString() + "?();");
                                    break;
                            }
                            parameters += item.Name.Replace("@", "") + ",";
                        }
                    }

                    if (parameters.Length > 0)
                        parameters = parameters.Substring(0, parameters.Length - 1);


                    output.AppendLine("                 switch (Context.Request.HttpMethod)");
                    output.AppendLine("                 {");
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Connect:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Get:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.Head:") ;
                    //output.AppendLine("                    break;") ;
                    //output.AppendLine("                case System.Net.WebRequestMethods.Http.MkCol:") ;
                    //output.AppendLine("                    break;") ;
                    output.AppendLine("                     case System.Net.WebRequestMethods.Http.Post:");
                    output.AppendLine("                         " + generationProject.Namespace + ".Business.Procedures." + entity.Name + " entities = new " + generationProject.Namespace + ".Business.Procedures." + entity.Name + "();");
                    //output.AppendLine("                    Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(entities.Items(" + parameters + ")));");
                    output.AppendLine("                             InicializarDti(TipoInformacion.Ok, Newtonsoft.Json.JsonConvert.SerializeObject(entities.Items(" + parameters + ")));");
                    output.AppendLine("                         break;");
                    output.AppendLine("                     case System.Net.WebRequestMethods.Http.Put:");
                    output.AppendLine("                         break;");
                    output.AppendLine("                                 default:");
                    output.AppendLine("                                     break;");
                    output.AppendLine("                             }");
                    output.AppendLine("                         }");
                    output.AppendLine("                         else");
                    output.AppendLine("                         {");
                    output.AppendLine("                             InicializarDti(TipoInformacion.SessionIdInvalido, ControladorAuditoria.ObtenerDescripcionError((int)TipoInformacion.SessionIdInvalido));");
                    output.AppendLine("                         }");
                    output.AppendLine("                     }");
                    output.AppendLine("                     catch (Exception ex)");
                    output.AppendLine("                     {");
                    output.AppendLine("                         ControladorAuditoria.RegistrarLog(ControladorAuditoria.TipoLog.Excepcion, ex.Message, (ex.InnerException == null ? " + System.Convert.ToChar(34) + "" + System.Convert.ToChar(34) + " : ex.InnerException.Source.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.TargetSite.ToString() + " + System.Convert.ToChar(34) + " - " + System.Convert.ToChar(34) + " + ex.InnerException.Message) + " + System.Convert.ToChar(34) + " // " + System.Convert.ToChar(34) + " + ex.StackTrace, usuarioID);");
                    output.AppendLine("                         InicializarDti(TipoInformacion.Excepcion, ex.Message);");
                    output.AppendLine("                     }");
                    output.AppendLine("                     finally");
                    output.AppendLine("                     {");
                    output.AppendLine("                         RetornarDatos();");
                    output.AppendLine("                     }");
                    output.AppendLine("                 }");
                    output.AppendLine("         }");
                    output.AppendLine(" }");

                    string relativepath = db.Name.ToLower() + "\\" + schemaName ; 
                    SaveOutputToFile(db.Name.ToLower() + "." + entity.Name.ToLower() + ".asmx", relativepath,output, true);
                
            }
        }

        private string GetSchemaName(MyMeta.IDatabase db,string procName)
        {
            string returnValue = "dbo";
            foreach (MyMeta.IProcedure entity in db.Procedures)
            {
                if (!entity.Selected)
                {
                    if((entity.Schema + "." +  entity.Name).Equals(procName))
                        returnValue = entity.Schema;
                }
                
            }
            return returnValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
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

        #region Save File Method
        private void SaveOutputToFile(string fileName, string relativePath, System.Text.StringBuilder output, bool overWrite)
        {
            if (!_workingdir.EndsWith("\\"))
                _workingdir += "\\";
            string filePath = _workingdir + "WebServices\\" + relativePath + "\\" + fileName;
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
            get { return "1cb9f2b7-e6eb-4866-7bea-55b549b68a3c"; }
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