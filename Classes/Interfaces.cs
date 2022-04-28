
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
/// <summary>
/// 
/// </summary>
/// <remarks></remarks>
public interface ITemplate
{
     event OnExceptionEventHandler OnException;
     event OnInfoEventHandler OnInfo;
    //public delegate void OnInfoEventHandler(string Description);
     event OnPercentDoneEventHandler OnPercentDone;
    //delegate void OnPercentDoneEventHandler(System.Double Percent);
     event OnFileGeneratedEventHandler OnFileGenerated;
    //public delegate void OnFileGeneratedEventHandler(string File);
    string Name { get; }
    string Description { get; }
    string OutputLanguage { get; }
    string WorkingDir { get; set; }
    string LanguageMappingFileName { get; set; }
    string DbTargetMappingFileName { get; set; }
    bool Execute(MyMeta.IDatabase db, string workingDir, GenerationProject generationProject);

    string GUID { get; }
}

public interface IPlugin
{
    string Name { get; }
    string Description { get; }
    List<ITemplate> Templates { get; set; }
}

public interface IToolPlugin
{
    string Name { get; }
    string Description { get; }
    object RunAtStartup { get; set; }
    object Execute();
}
public interface IDataHandler
{

}


