
using Microsoft.VisualBasic;
using OpenORM.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

public class GenerationProjectSerializer
{

    public static GenerationProject CurrentProject;
    public static string CurrentProjectFilename;
    public static bool Save(string strFilename, GenerationProject gpr)
    {
        try
        {
            StreamWriter objSW = new StreamWriter(strFilename, false, System.Text.Encoding.Default);
            objSW.Write(Serialize(gpr));
            objSW.Close();

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }

        //return false;
    }
    public static bool Save(GenerationProject gpr)
    {
        try
        {
            StreamWriter objSW = new StreamWriter(CurrentProjectFilename, false, System.Text.Encoding.Default);
            objSW.Write(Serialize(gpr));
            objSW.Close();

            return true;
        }
        catch (System.IO.IOException ex)
        {
            return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }

        //return false;
    }
    public static string Serialize(GenerationProject gpr)
    {
        try
        {
            XmlSerializer objXS = new XmlSerializer(gpr.GetType());
            StringWriter objSW = new StringWriter();

            objXS.Serialize(objSW, gpr);
            CurrentProject = gpr;

            return objSW.ToString();
        }
        catch (Exception)
        {
            return (null);
        }

    }
    public static GenerationProject Load(string Filename)
    {
        try
        {
            CurrentProjectFilename = Filename;
            if (!System.IO.File.Exists(Filename))
            {
                GenerationProject newFile = new GenerationProject();

                StreamWriter objSW = new StreamWriter(Filename, false, System.Text.Encoding.Default);
                objSW.Write(Serialize(newFile));
                objSW.Close();



            }
            StreamReader objSR = new StreamReader(Filename, System.Text.Encoding.Default);
            GenerationProject objST = DeSerialize(objSR.ReadToEnd());
            objSR.Close();
            CurrentProject = objST;
            return objST;
        }
        catch (Exception ex)
        {
            //System.Windows.Forms.MessageBox.Show("Error deserializing from file: " + Filename + " (" + ex.Message + ")" + Environment.NewLine + Environment.NewLine + ex.ToString(), "Deserialization Error");
            throw ex;
        }
    }
    public static GenerationProject Load()
    {

        try
        {
            if ((CurrentProject != null))
            {
                StreamReader objSR = new StreamReader(CurrentProject.Location, System.Text.Encoding.Default);
                GenerationProject objST = DeSerialize(objSR.ReadToEnd());
                objSR.Close();
                return objST;
            }
            else
            {
                return null;
            }

        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show("Error deserializing from file: " + CurrentProject.Location + " (" + ex.Message + ")" + Environment.NewLine + Environment.NewLine + ex.ToString(), "Deserialization Error");
            throw ex;
        }
    }

    public static GenerationProject DeSerialize(string strXML)
    {
        XmlSerializer objXS = new XmlSerializer(typeof(GenerationProject));
        StringReader objSR = new StringReader(strXML);

        GenerationProject objST = (GenerationProject)objXS.Deserialize(objSR);
        //objST.m_blnDeSerializeMode = False

        return objST;
    }
    public static void CreateDefaultProject()
    {

        try
        {
            //GenerationProjectSerializer.CurrentProject = new GenerationProject();
            //string refProjectname = MainDialogForm._selectedDatabase.Name;
            //var _with1 = GenerationProjectSerializer.CurrentProject;
            ////_with1.Location = My.Computer.FileSystem.SpecialDirectories.MyDocuments + "\\" + refProjectname + "\\";
            //_with1.Name = refProjectname;
            //MainDialogForm.SaveProjectFile();

        }
        catch (Exception ex)
        {
            throw(ex);
        }
    }
}

