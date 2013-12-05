﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;

using Simsang.Session.Config;


namespace Simsang.Session
{
  public class InfrastructureFacade
  {

    #region MEMBERS

    private static InfrastructureFacade cInstance;
    private String mSessionDir;

    #endregion


    #region PUBLIC

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static InfrastructureFacade getInstance()
    {
      return cInstance ?? (cInstance = new InfrastructureFacade());
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSessionFileName"></param>
    public void removeSession(String pSessionFileName)
    {
      String lFileName = String.Empty;
      try
      {
        lFileName = String.Format(@"{0}\{1}", mSessionDir, pSessionFileName);
        if (File.Exists(lFileName))
          File.Delete(lFileName);
      }
      catch (Exception lEx)
      {
        LogConsole.Main.LogConsole.pushMsg(lEx.StackTrace);
      }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSessionName"></param>
    public AttackSession loadSession(String pSessionName)
    {
      String lSessionFilePath = String.Empty;

      try
      {
        lSessionFilePath = String.Format(@"{0}\{1}.xml", mSessionDir, pSessionName);
      }
      catch (Exception lEx)
      {
        LogConsole.Main.LogConsole.pushMsg(lEx.StackTrace);
      }

      /*
       * Load main GUI session data.
       */
      FileStream lFS = null;
      XmlSerializer lXMLSerial;
      AttackSession lAttackSession = new AttackSession();
      String lStartIP = String.Empty;
      String lStopIP = String.Empty;

      try
      {
        lFS = new FileStream(lSessionFilePath, FileMode.Open);
        lXMLSerial = new XmlSerializer(typeof(AttackSession));
        lAttackSession = (AttackSession)lXMLSerial.Deserialize(lFS);
      }
      catch (Exception lEx)
      {
        LogConsole.Main.LogConsole.pushMsg(lEx.StackTrace);
      }
      finally
      {
        if (lFS != null)
          lFS.Close();
      }

      return(lAttackSession);
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSessionDir"></param>
    /// <returns></returns>
    public List<AttackSession> getAllSessions()
    {
      List<AttackSession> lRetVal = new List<AttackSession>();
      DirectoryInfo lDirInfo = new DirectoryInfo(mSessionDir);
      FileInfo[] lSessionFiles = lDirInfo.GetFiles("*.xml");


      /*
       * 
       */
      foreach (FileInfo lSessionFile in lSessionFiles)
      {
        FileStream lFS = null;
        XmlSerializer lXMLSerial;

        try
        {
          lFS = new FileStream(lSessionFile.FullName, FileMode.Open);
          lXMLSerial = new XmlSerializer(typeof(AttackSession));
          AttackSession lAttackSession = (AttackSession)lXMLSerial.Deserialize(lFS);
          lRetVal.Add(lAttackSession);
        }
        catch (Exception lEx)
        {
          LogConsole.Main.LogConsole.pushMsg(String.Format("TaskFacade.GetAllSessions(): {0} ({1})", lEx.Message, lSessionFile.FullName));
        }
        finally
        {
          if (lFS != null)
            lFS.Close();
        }
      } // foreach (FileIn...

      return (lRetVal);
    }


    /// <summary>
    /// 
    /// </summary>
    public void SaveSessionData(AttackSession pAttackSession)
    {
      String lSessionFile;
      XmlSerializer lSerializer;
      FileStream lFS = null;

      try
      {
        lSessionFile = String.Format(@"{0}\{1}", mSessionDir, pAttackSession.SessionFileName);
        lSerializer = new XmlSerializer(typeof(AttackSession));
        lFS = new FileStream(lSessionFile, FileMode.Create);
        lSerializer.Serialize(lFS, pAttackSession);
      }
      catch (Exception lEx)
      {
//MessageBox.Show("Can't save session data : " + lEx.ToString());
      }
      finally
      {
        if (lFS != null)
          lFS.Close();
      }
    }


    /// <summary>
    /// 
    /// </summary>
    public String readMainSessionData(String pSessionFileName)
    {
      String lSessionFilePath = String.Format(@"{0}\{1}.xml", mSessionDir, pSessionFileName);
      String lSessionData = File.ReadAllText(lSessionFilePath);

      return (lSessionData);
    }


    /// <summary>
    /// 
    /// </summary>
    public void writeSessionExportFile(String pPathSessionFile, String pDataString)
    {
      try
      {
        if (!String.IsNullOrEmpty(pDataString))
        {
          if (Regex.Match(pDataString, @"<\s*\?\s*xml.*?>", RegexOptions.IgnoreCase).Success)
            pDataString = Regex.Replace(pDataString, @"<\s*\?\s*xml.*?>", String.Empty, RegexOptions.IgnoreCase);

          using (StreamWriter lSW = new StreamWriter(pPathSessionFile))
          {
            lSW.Write(pDataString);
          } // using (Strea...
        } // if (!String....
      }
      catch (Exception lEx)
      {
        LogConsole.Main.LogConsole.pushMsg(String.Format("Export session: ", lEx.Message));
        throw new Exception(String.Format("Unable to export session : {0}", lEx.Message));
      }
    }

    #endregion


    #region PRIVATE

    /// <summary>
    /// 
    /// </summary>
    private InfrastructureFacade()
    {
      mSessionDir = String.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["sessiondir"] ?? @"Sessions\");
    }

    #endregion

  }
}
