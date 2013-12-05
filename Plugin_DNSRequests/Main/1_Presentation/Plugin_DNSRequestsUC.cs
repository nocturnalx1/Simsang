﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Configuration;


using Simsang.Plugin;
using Plugin.Main.DNSRequest;
using Plugin.Main.DNSRequest.Config;


namespace Plugin.Main
{
  public partial class PluginDNSRequestsUC : UserControl, IPlugin, IObserver
  {

    #region MEMBERS

    private IPluginHost cHost;
    private List<String> cTargetList;
    private BindingList<DNSRequestRecord> cDNSRequests;
    private TaskFacade cTask;
    private DomainFacade cDomain;

    #endregion


    #region PUBLIC

    /// <summary>
    /// 
    /// </summary>
    public PluginDNSRequestsUC()
    {
      InitializeComponent();

      #region DATAGRID HEADERS

      DataGridViewTextBoxColumn cMACCol = new DataGridViewTextBoxColumn();
      cMACCol.DataPropertyName = "SrcMAC";
      cMACCol.Name = "SrcMAC";
      cMACCol.HeaderText = "MAC address";
      cMACCol.ReadOnly = true;
      cMACCol.Width = 140;
      DGV_DNSRequests.Columns.Add(cMACCol);


      DataGridViewTextBoxColumn cSrcIPCol = new DataGridViewTextBoxColumn();
      cSrcIPCol.DataPropertyName = "SrcIP";
      cSrcIPCol.Name = "SrcIP";
      cSrcIPCol.HeaderText = "Source IP";
      cSrcIPCol.ReadOnly = true;
      cSrcIPCol.Width = 120;
      DGV_DNSRequests.Columns.Add(cSrcIPCol);


      DataGridViewTextBoxColumn cTimestampCol = new DataGridViewTextBoxColumn();
      cTimestampCol.DataPropertyName = "Timestamp";
      cTimestampCol.Name = "Timestamp";
      cTimestampCol.HeaderText = "Timestamp";
      cTimestampCol.ReadOnly = true;
      cTimestampCol.Width = 120;
      DGV_DNSRequests.Columns.Add(cTimestampCol);


      DataGridViewTextBoxColumn cRemHostCol = new DataGridViewTextBoxColumn();
      cRemHostCol.DataPropertyName = "DNSHostname";
      cRemHostCol.Name = "DNSHostname";
      cRemHostCol.HeaderText = "DNS request";
      cRemHostCol.ReadOnly = true;
      cRemHostCol.Width = 180;
      cRemHostCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      DGV_DNSRequests.Columns.Add(cRemHostCol);

      DataGridViewTextBoxColumn cPacketTypeCol = new DataGridViewTextBoxColumn();
      cPacketTypeCol.DataPropertyName = "PacketType";
      cPacketTypeCol.Name = "PacketType";
      cPacketTypeCol.HeaderText = "Packet type";
      cPacketTypeCol.ReadOnly = true;
      //cRemHostCol.Width = 280;
      cPacketTypeCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      DGV_DNSRequests.Columns.Add(cPacketTypeCol);

      cDNSRequests = new BindingList<DNSRequestRecord>();
      DGV_DNSRequests.DataSource = cDNSRequests;

      #endregion

      Config = new PluginProperties()
      {
        BaseDir = String.Format(@"{0}\", Directory.GetCurrentDirectory()),
        SessionDir = ConfigurationManager.AppSettings["sessiondir"] ?? @"Sessions\",
        PluginName = "DNS requests",
        PluginDescription = "Listing client systems DNS requests.",
        PluginVersion = "0.5",
        Ports = "UDP:53;",
        IsActive = true
      };


      cTask = TaskFacade.getInstance(this);
      cDomain = DomainFacade.getInstance(this);

      cDomain.addObserver(this);
    }

    #endregion


    #region PROPERTIES

    public Control PluginControl { get { return (this); } }
    public IPluginHost Host { get { return cHost; } set { cHost = value; cHost.Register(this); } }

    #endregion


    #region IPlugin Member

    /// <summary>
    /// 
    /// </summary>
    public PluginProperties Config { set; get; }

    /// <summary>
    /// 
    /// </summary>
    public delegate void onInitDelegate();
    public void onInit()
    {
      if (InvokeRequired)
      {
        BeginInvoke(new onInitDelegate(onInit), new object[] { });
        return;
      } // if (InvokeRequired)


      cHost.PluginSetStatus(this, "grey");
    }



    /// <summary>
    /// 
    /// </summary>
    public delegate void onStartAttackDelegate();
    public void onStartAttack()
    {
      if (Config.IsActive)
      {
        if (InvokeRequired)
        {
          BeginInvoke(new onStartAttackDelegate(onStartAttack), new object[] { });
          return;
        } // if (InvokeRequired)

        cHost.PluginSetStatus(this, "green");
      } // if (cIsActive)
    }



    /// <summary>
    /// 
    /// </summary>
    public delegate void onStopAttackDelegate();
    public void onStopAttack()
    {
      if (InvokeRequired)
      {
        BeginInvoke(new onStopAttackDelegate(onStopAttack), new object[] { });
        return;
      } // if (InvokeRequired)

      cHost.PluginSetStatus(this, "grey");
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSessionName"></param>
    public delegate void onLoadSessionDataFromFileDelegate(String pSessionName);
    public void onLoadSessionDataFromFile(String pSessionName)
    {
      if (InvokeRequired)
      {
        BeginInvoke(new onLoadSessionDataFromFileDelegate(onLoadSessionDataFromFile), new object[] { pSessionName });
        return;
      } // if (InvokeRequired)


      try
      {
        cTask.loadSessionData(pSessionName);
      }
      catch (Exception lEx)
      {
        cHost.LogMessage(lEx.Message);
      }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSessionData"></param>
    public delegate void onLoadSessionDataFromStringDelegate(String pSessionData);
    public void onLoadSessionDataFromString(String pSessionData)
    {
      if (InvokeRequired)
      {
        BeginInvoke(new onLoadSessionDataFromStringDelegate(onLoadSessionDataFromString), new object[] { pSessionData });
        return;
      } // if (InvokeRequired)

      try
      {
        cTask.loadSessionDataFromString(pSessionData);
      }
      catch (Exception lEx)
      {
        cHost.LogMessage(lEx.StackTrace);
      }
    }




    /// <summary>
    /// Remove session file with serialized data.
    /// </summary>
    /// <param name="pSessionFileName"></param>
    public delegate void onDeleteSessionDataDelegate(String pSessionName);
    public void onDeleteSessionData(String pSessionName)
    {
      if (InvokeRequired)
      {
        BeginInvoke(new onDeleteSessionDataDelegate(onDeleteSessionData), new object[] { pSessionName });
        return;
      } // if (InvokeRequired)


      try
      {
        cTask.deleteSession(pSessionName);
      }
      catch (Exception lEx)
      {
        cHost.LogMessage(lEx.StackTrace);
      }
    }



    /// <summary>
    /// Serialize session data
    /// </summary>
    /// <param name="pSessionName"></param>
    public delegate void onSaveSessionDataDelegate(string pSessionName);
    public void onSaveSessionData(string pSessionName)
    {
      if (Config.IsActive)
      {
        if (InvokeRequired)
        {
          BeginInvoke(new onSaveSessionDataDelegate(onSaveSessionData), new object[] { pSessionName });
          return;
        } // if (InvokeRequired)

        try
        {
          cTask.saveSession(pSessionName);
        }
        catch (Exception lEx)
        {
          cHost.LogMessage(lEx.Message);
        }
      } // if (cIsActive)
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate String getDataDelegate();
    public String getData()
    {
      if (InvokeRequired)
      {
        BeginInvoke(new getDataDelegate(getData), new object[] { });
        return ("");
      } // if (InvokeRequired)
      return ("");
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSessionID"></param>
    /// <returns></returns>
    public delegate String onGetSessionDataDelegate(String pSessionName);
    public String onGetSessionData(String pSessionName)
    {
      if (InvokeRequired)
      {
        BeginInvoke(new onGetSessionDataDelegate(onGetSessionData), new object[] { pSessionName });
        return (String.Empty);
      } // if (InvokeRequired)

      String lRetVal = String.Empty;

      try
      {
        lRetVal = cTask.getSessionData(pSessionName);
      }
      catch (Exception lEx)
      {
        cHost.LogMessage(lEx.StackTrace);
      }

      return (lRetVal);
    }



    /// <summary>
    /// 
    /// </summary>
    public delegate void onResetPluginDelegate();
    public void onResetPlugin()
    {
      if (InvokeRequired)
      {
        BeginInvoke(new onResetPluginDelegate(onResetPlugin), new object[] { });
        return;
      } // if (InvokeRequired)

      TB_Filter.Text = String.Empty;

      cTask.clearRecordList();

      cHost.PluginSetStatus(this, "grey");
    }


    /// <summary>
    /// 
    /// </summary>
    public void onShutDown()
    {
    }



    /// <summary>
    /// New input data arrived
    /// </summary>
    /// <param name="pData"></param>
    public delegate void onNewDataDelegate(String pData);
    public void onNewData(String pData)
        {
          if (Config.IsActive)
          {
            if (InvokeRequired)
            {
              BeginInvoke(new onNewDataDelegate(onNewData), new object[] { pData });
              return;
            } // if (InvokeRequired)



            try
            {
              if (pData != null && pData.Length > 0)
              {
                String[] lSplitter = Regex.Split(pData, @"\|\|");
                if (lSplitter.Length == 7)
                {
                  String lProto = lSplitter[0];
                  String lSMAC = lSplitter[1];
                  String lSIP = lSplitter[2];
                  String lSrcPort = lSplitter[3];
                  String lDstIP = lSplitter[4];
                  String lDstPort = lSplitter[5];
                  String lHostName = lSplitter[6];

                 
                  if (lDstPort != null && lDstPort == "53")
                  {
                    lock (this)
                    {
                      cTask.addRecord(new DNSRequestRecord(lSMAC, lSIP, lHostName, lProto));
                    } // lock (this)
                  } // if (lDstPort != null ...
                }
              } // if (pData.Le... 
            }
            catch (Exception lEx)
            {
              if (cHost != null)
                cHost.LogMessage(String.Format("Input data : {0}\r\nStackTrace : {1}", pData, lEx.StackTrace));
            }
          } // if (cIsActive)
        }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="pTargetList"></param>
    public void SetTargets(List<String> pTargetList)
    {
      cTargetList = pTargetList;
    }


    #endregion


    #region PRIVATE

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pInputData"></param>
    /// <returns></returns>
    private bool CompareToFilter(String pInputData)
    {
      bool lRetVal = false;

      if (Regex.Match(pInputData, @TB_Filter.Text, RegexOptions.IgnoreCase).Success)
        lRetVal = true;

      return (lRetVal);
    }


    /// <summary>
    /// 
    /// </summary>
    private void UseFilter()
    {
      // without this line we will get an exception :/ da fuq!
      DGV_DNSRequests.CurrentCell = null;
      for (int lCounter = 0; lCounter < DGV_DNSRequests.Rows.Count; lCounter++)
      {
        if (TB_Filter.Text.Length <= 0)
        {
          DGV_DNSRequests.Rows[lCounter].Visible = true;
        }
        else
        {
          try
          {
            String lData = DGV_DNSRequests.Rows[lCounter].Cells["DNSHostname"].Value.ToString();
            if (!Regex.Match(lData, Regex.Escape(TB_Filter.Text), RegexOptions.IgnoreCase).Success)
              DGV_DNSRequests.Rows[lCounter].Visible = false;
            else
              DGV_DNSRequests.Rows[lCounter].Visible = true;
          }
          catch (Exception) { }
        }
      }

      DGV_DNSRequests.Refresh();
    }

    #endregion


    #region EVENTS

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BT_Set_Click(object sender, EventArgs e)
    {
      UseFilter();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TSMI_Clear_Click(object sender, EventArgs e)
    {
      lock (this)
      {
        cTask.clearRecordList();
      } // lock (this) ...
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DGV_DNSRequests_MouseUp(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        try
        {
          DataGridView.HitTestInfo hti = DGV_DNSRequests.HitTest(e.X, e.Y);
          if (hti.RowIndex >= 0)
            CMS_DNSRequests.Show(DGV_DNSRequests, e.Location);
        }
        catch (Exception) { }
      }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TB_Filter_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        UseFilter();
      }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
    {

      try
      {
        int lCurIndex = DGV_DNSRequests.CurrentCell.RowIndex;
        cTask.removeRecordAt(lCurIndex);
      }
      catch (Exception) { }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void copyHostNameToolStripMenuItem_Click(object sender, EventArgs e)
    {
      try
      {
        BindingList<DNSRequestRecord> lTmpHosts = new BindingList<DNSRequestRecord>();
        int lCurIndex = DGV_DNSRequests.CurrentCell.RowIndex;
        String lHostName = DGV_DNSRequests.Rows[lCurIndex].Cells["DNSHostname"].Value.ToString();
        Clipboard.SetText(lHostName);
      }
      catch (Exception lEx)
      {
        cHost.LogMessage(lEx.StackTrace);
      }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DGV_DNSRequests_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      try
      {
        BindingList<DNSRequestRecord> lTmpHosts = new BindingList<DNSRequestRecord>();
        int lCurIndex = DGV_DNSRequests.CurrentCell.RowIndex;
        String lHostName = DGV_DNSRequests.Rows[lCurIndex].Cells["DNSHostname"].Value.ToString();
        Clipboard.SetText(lHostName);
      }
      catch (Exception lEx)
      {
        cHost.LogMessage(lEx.StackTrace);
      }
    }




    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DGV_DNSRequests_MouseDown(object sender, MouseEventArgs e)
    {
      try
      {
        DataGridView.HitTestInfo hti = DGV_DNSRequests.HitTest(e.X, e.Y);

        if (hti.RowIndex >= 0)
        {
          DGV_DNSRequests.ClearSelection();
          DGV_DNSRequests.Rows[hti.RowIndex].Selected = true;
          DGV_DNSRequests.CurrentCell = DGV_DNSRequests.Rows[hti.RowIndex].Cells[0];
        }
      }
      catch (Exception)
      {
        DGV_DNSRequests.ClearSelection();
      }
    }
    #endregion


    #region OBSERVER INTERFACE METHODS

    public void update(BindingList<DNSRequestRecord> pDNSReqList)
    {
      int lLastPosition = -1;
      int lLastRowIndex = -1;
      bool lIsLastLine = false;

      if (pDNSReqList != null && pDNSReqList.Count > 0)
      {
        if (DGV_DNSRequests.CurrentRow != null && DGV_DNSRequests.CurrentRow == DGV_DNSRequests.Rows[DGV_DNSRequests.Rows.Count - 1])
          lIsLastLine = true;

        lLastPosition = DGV_DNSRequests.FirstDisplayedScrollingRowIndex;
        lLastRowIndex = DGV_DNSRequests.Rows.Count - 1;

        cDNSRequests.Clear();

        foreach (DNSRequestRecord lTmp in pDNSReqList)
        {
          cDNSRequests.Add(lTmp);
        } // foreach (DNSReq...

        // Filter
        try
        {
          if (!CompareToFilter(DGV_DNSRequests.Rows[lLastRowIndex + 1].Cells["DNSHostname"].Value.ToString()))
            DGV_DNSRequests.Rows[lLastRowIndex + 1].Visible = false;
        }
        catch (Exception) { }
      } // if (pDNSReqL...
    }


    #endregion

  }
}