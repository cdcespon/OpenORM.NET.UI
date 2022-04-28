
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

public class ConfigurationSettingSerializer
{
    
    private static string _configfile = AppDomain.CurrentDomain.BaseDirectory + "Configuration.xml";
    public static bool Save(ConfigurationSetting ConfigurationInstance)
    {
        try
        {
            StreamWriter objSW = new StreamWriter(_configfile, false, System.Text.Encoding.Default);
            objSW.Write(Serialize(ConfigurationInstance));
            objSW.Close();

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public static string Serialize(ConfigurationSetting ConfigurationInstance)
    {
        try
        {
            XmlSerializer objXS = new XmlSerializer(ConfigurationInstance.GetType());
            StringWriter objSW = new StringWriter();

            objXS.Serialize(objSW, ConfigurationInstance);

            return objSW.ToString();
        }
        catch (Exception)
        {
            return (null);
        }

    }

    public static ConfigurationSetting Load(string ConfigurationFile)
    {
        try
        {
            if (ConfigurationFile != null)
            {
                if (System.IO.File.Exists(ConfigurationFile))
                {
                    StreamReader objSR = new StreamReader(ConfigurationFile, System.Text.Encoding.Default);
                    ConfigurationSetting objST = DeSerialize(objSR.ReadToEnd());
                    objSR.Close();

                    return objST;
                }
                else
                {
                    ConfigurationSetting newConfig = new ConfigurationSetting();
                    Save(newConfig);
                    return newConfig;
                }
            }
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show("Error deserializing from file: " + ConfigurationFile + " (" + ex.Message + ")" + Environment.NewLine + Environment.NewLine + ex.ToString(), "Deserialization Error");
            throw ex;
        }

        return null;
    }

    public static ConfigurationSetting DeSerialize(string strXML)
    {
        XmlSerializer objXS = new XmlSerializer(typeof(ConfigurationSetting));
        StringReader objSR = new StringReader(strXML);

        ConfigurationSetting objST = (ConfigurationSetting)objXS.Deserialize(objSR);
        //objST.m_blnDeSerializeMode = False

        return objST;
    }

}

