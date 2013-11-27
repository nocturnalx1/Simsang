﻿namespace Plugin.Main
{
  partial class ShowRequest
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowRequest));
      this.TB_Request = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // TB_Request
      // 
      this.TB_Request.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.TB_Request.Location = new System.Drawing.Point(8, 9);
      this.TB_Request.Multiline = true;
      this.TB_Request.Name = "TB_Request";
      this.TB_Request.ReadOnly = true;
      this.TB_Request.Size = new System.Drawing.Size(484, 312);
      this.TB_Request.TabIndex = 0;
      this.TB_Request.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PluginHTTPProxyUC_KeyUp);
      // 
      // ShowRequest
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(502, 329);
      this.Controls.Add(this.TB_Request);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ShowRequest";
      this.Text = "Show HTTP request details";
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PluginHTTPProxyUC_KeyUp);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox TB_Request;
  }
}