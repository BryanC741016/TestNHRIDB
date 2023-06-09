
namespace WinFrmAppNHRIDB
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnSend = new System.Windows.Forms.Button();
            this.BtnEditPassword = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Txtport = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.Txtpassword = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.Txtaccount = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.TxtmailServer = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TxtFromUsr = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.TxtFromMail = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TxtUsr = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TxtMail = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtBody = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtSubject = new System.Windows.Forms.TextBox();
            this.TxtANS = new System.Windows.Forms.TextBox();
            this.ChkEditAllPassword = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnSend
            // 
            this.BtnSend.Location = new System.Drawing.Point(475, 24);
            this.BtnSend.Name = "BtnSend";
            this.BtnSend.Size = new System.Drawing.Size(94, 38);
            this.BtnSend.TabIndex = 0;
            this.BtnSend.Text = "寄單一信";
            this.BtnSend.UseVisualStyleBackColor = true;
            this.BtnSend.Click += new System.EventHandler(this.BtnSend_Click);
            // 
            // BtnEditPassword
            // 
            this.BtnEditPassword.Location = new System.Drawing.Point(759, 47);
            this.BtnEditPassword.Name = "BtnEditPassword";
            this.BtnEditPassword.Size = new System.Drawing.Size(156, 55);
            this.BtnEditPassword.TabIndex = 1;
            this.BtnEditPassword.Text = "手動修改全部密碼";
            this.BtnEditPassword.UseVisualStyleBackColor = true;
            this.BtnEditPassword.Visible = false;
            this.BtnEditPassword.Click += new System.EventHandler(this.BtnEditPassword_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Subject";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Txtport);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.Txtpassword);
            this.groupBox1.Controls.Add(this.BtnSend);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.Txtaccount);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.TxtmailServer);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.TxtFromUsr);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.TxtFromMail);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.TxtUsr);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.TxtMail);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.TxtBody);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.TxtSubject);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(605, 290);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "寄信測試";
            // 
            // Txtport
            // 
            this.Txtport.Location = new System.Drawing.Point(306, 193);
            this.Txtport.Name = "Txtport";
            this.Txtport.Size = new System.Drawing.Size(147, 25);
            this.Txtport.TabIndex = 19;
            this.Txtport.Text = "25";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(270, 196);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(30, 15);
            this.label10.TabIndex = 20;
            this.label10.Text = "port";
            // 
            // Txtpassword
            // 
            this.Txtpassword.Location = new System.Drawing.Point(306, 150);
            this.Txtpassword.Name = "Txtpassword";
            this.Txtpassword.Size = new System.Drawing.Size(147, 25);
            this.Txtpassword.TabIndex = 17;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(241, 160);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 15);
            this.label9.TabIndex = 18;
            this.label9.Text = "password";
            // 
            // Txtaccount
            // 
            this.Txtaccount.Location = new System.Drawing.Point(306, 106);
            this.Txtaccount.Name = "Txtaccount";
            this.Txtaccount.Size = new System.Drawing.Size(147, 25);
            this.Txtaccount.TabIndex = 15;
            this.Txtaccount.Text = "nbctdata@nhri.edu.tw";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(250, 116);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 15);
            this.label8.TabIndex = 16;
            this.label8.Text = "account";
            // 
            // TxtmailServer
            // 
            this.TxtmailServer.Location = new System.Drawing.Point(306, 65);
            this.TxtmailServer.Name = "TxtmailServer";
            this.TxtmailServer.Size = new System.Drawing.Size(147, 25);
            this.TxtmailServer.TabIndex = 13;
            this.TxtmailServer.Text = "sender.nhri.edu.tw";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(231, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 15);
            this.label7.TabIndex = 14;
            this.label7.Text = "mailServer";
            // 
            // TxtFromUsr
            // 
            this.TxtFromUsr.Location = new System.Drawing.Point(306, 24);
            this.TxtFromUsr.Name = "TxtFromUsr";
            this.TxtFromUsr.Size = new System.Drawing.Size(147, 25);
            this.TxtFromUsr.TabIndex = 11;
            this.TxtFromUsr.Text = "nbctdata";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(242, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 15);
            this.label6.TabIndex = 12;
            this.label6.Text = "FromUsr";
            // 
            // TxtFromMail
            // 
            this.TxtFromMail.Location = new System.Drawing.Point(69, 193);
            this.TxtFromMail.Name = "TxtFromMail";
            this.TxtFromMail.Size = new System.Drawing.Size(134, 25);
            this.TxtFromMail.TabIndex = 9;
            this.TxtFromMail.Text = "nbctdata@nhri.edu.tw";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 203);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "FromMail";
            // 
            // TxtUsr
            // 
            this.TxtUsr.Location = new System.Drawing.Point(69, 150);
            this.TxtUsr.Name = "TxtUsr";
            this.TxtUsr.Size = new System.Drawing.Size(134, 25);
            this.TxtUsr.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Usr";
            // 
            // TxtMail
            // 
            this.TxtMail.Location = new System.Drawing.Point(69, 106);
            this.TxtMail.Name = "TxtMail";
            this.TxtMail.Size = new System.Drawing.Size(134, 25);
            this.TxtMail.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Mail";
            // 
            // TxtBody
            // 
            this.TxtBody.Location = new System.Drawing.Point(69, 65);
            this.TxtBody.Name = "TxtBody";
            this.TxtBody.Size = new System.Drawing.Size(134, 25);
            this.TxtBody.TabIndex = 3;
            this.TxtBody.Text = "test1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Body";
            // 
            // TxtSubject
            // 
            this.TxtSubject.Location = new System.Drawing.Point(69, 24);
            this.TxtSubject.Name = "TxtSubject";
            this.TxtSubject.Size = new System.Drawing.Size(134, 25);
            this.TxtSubject.TabIndex = 0;
            this.TxtSubject.Text = "test";
            // 
            // TxtANS
            // 
            this.TxtANS.Location = new System.Drawing.Point(12, 308);
            this.TxtANS.Multiline = true;
            this.TxtANS.Name = "TxtANS";
            this.TxtANS.Size = new System.Drawing.Size(903, 245);
            this.TxtANS.TabIndex = 4;
            // 
            // ChkEditAllPassword
            // 
            this.ChkEditAllPassword.AutoSize = true;
            this.ChkEditAllPassword.Location = new System.Drawing.Point(736, 22);
            this.ChkEditAllPassword.Name = "ChkEditAllPassword";
            this.ChkEditAllPassword.Size = new System.Drawing.Size(179, 19);
            this.ChkEditAllPassword.TabIndex = 5;
            this.ChkEditAllPassword.Text = "啟用手動修改全部密碼";
            this.ChkEditAllPassword.UseVisualStyleBackColor = true;
            this.ChkEditAllPassword.CheckedChanged += new System.EventHandler(this.ChkEditAllPassword_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(927, 565);
            this.Controls.Add(this.ChkEditAllPassword);
            this.Controls.Add(this.TxtANS);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BtnEditPassword);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnSend;
        private System.Windows.Forms.Button BtnEditPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox Txtport;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox Txtpassword;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox Txtaccount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TxtmailServer;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TxtFromUsr;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TxtFromMail;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TxtUsr;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TxtMail;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtBody;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtSubject;
        private System.Windows.Forms.TextBox TxtANS;
        private System.Windows.Forms.CheckBox ChkEditAllPassword;
    }
}

