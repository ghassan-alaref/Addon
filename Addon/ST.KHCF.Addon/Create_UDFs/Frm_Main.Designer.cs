namespace Create_UDFs
{
    partial class Frm_Main
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Main));
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Tab_Database = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Chk_Save_Password = new System.Windows.Forms.CheckBox();
            this.Btn_Connect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Txt_LicenseServer = new System.Windows.Forms.TextBox();
            this.Txt_Server = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Txt_DB_Username = new System.Windows.Forms.TextBox();
            this.Txt_CompanyDB = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Txt_DB_Password = new System.Windows.Forms.TextBox();
            this.Txt_B1_Password = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Txt_B1_Username = new System.Windows.Forms.TextBox();
            this.Tab_Log = new System.Windows.Forms.TabPage();
            this.Txt_Log = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.panel15 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.Stb_Message = new System.Windows.Forms.ToolStripStatusLabel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.saveFileDialog_Info = new System.Windows.Forms.SaveFileDialog();
            this.Tmr_Clear_Message = new System.Windows.Forms.Timer(this.components);
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.Tab_Database.SuspendLayout();
            this.panel1.SuspendLayout();
            this.Tab_Log.SuspendLayout();
            this.panel15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 66);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(913, 519);
            this.panel2.TabIndex = 15;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Tab_Database);
            this.tabControl1.Controls.Add(this.Tab_Log);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(913, 519);
            this.tabControl1.TabIndex = 0;
            // 
            // Tab_Database
            // 
            this.Tab_Database.Controls.Add(this.panel1);
            this.Tab_Database.Location = new System.Drawing.Point(4, 22);
            this.Tab_Database.Name = "Tab_Database";
            this.Tab_Database.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Database.Size = new System.Drawing.Size(905, 493);
            this.Tab_Database.TabIndex = 5;
            this.Tab_Database.Text = "Database";
            this.Tab_Database.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Chk_Save_Password);
            this.panel1.Controls.Add(this.Btn_Connect);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.Txt_LicenseServer);
            this.panel1.Controls.Add(this.Txt_Server);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.Txt_DB_Username);
            this.panel1.Controls.Add(this.Txt_CompanyDB);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.Txt_DB_Password);
            this.panel1.Controls.Add(this.Txt_B1_Password);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.Txt_B1_Username);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(899, 487);
            this.panel1.TabIndex = 14;
            // 
            // Chk_Save_Password
            // 
            this.Chk_Save_Password.AutoSize = true;
            this.Chk_Save_Password.Location = new System.Drawing.Point(353, 142);
            this.Chk_Save_Password.Name = "Chk_Save_Password";
            this.Chk_Save_Password.Size = new System.Drawing.Size(100, 17);
            this.Chk_Save_Password.TabIndex = 15;
            this.Chk_Save_Password.Text = "Save Password";
            this.Chk_Save_Password.UseVisualStyleBackColor = true;
            // 
            // Btn_Connect
            // 
            this.Btn_Connect.Location = new System.Drawing.Point(353, 162);
            this.Btn_Connect.Name = "Btn_Connect";
            this.Btn_Connect.Size = new System.Drawing.Size(117, 23);
            this.Btn_Connect.TabIndex = 14;
            this.Btn_Connect.Text = "Create UDF";
            this.Btn_Connect.UseVisualStyleBackColor = true;
            this.Btn_Connect.Click += new System.EventHandler(this.Btn_Connect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server";
            // 
            // Txt_LicenseServer
            // 
            this.Txt_LicenseServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Txt_LicenseServer.Location = new System.Drawing.Point(128, 35);
            this.Txt_LicenseServer.Name = "Txt_LicenseServer";
            this.Txt_LicenseServer.Size = new System.Drawing.Size(206, 20);
            this.Txt_LicenseServer.TabIndex = 13;
            this.Txt_LicenseServer.Text = "hanab2:40000";
            // 
            // Txt_Server
            // 
            this.Txt_Server.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Txt_Server.Location = new System.Drawing.Point(128, 9);
            this.Txt_Server.Name = "Txt_Server";
            this.Txt_Server.Size = new System.Drawing.Size(206, 20);
            this.Txt_Server.TabIndex = 1;
            this.Txt_Server.Text = "hanab2:30015";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 38);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "License Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Company DB";
            // 
            // Txt_DB_Username
            // 
            this.Txt_DB_Username.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Txt_DB_Username.Location = new System.Drawing.Point(128, 61);
            this.Txt_DB_Username.Name = "Txt_DB_Username";
            this.Txt_DB_Username.Size = new System.Drawing.Size(206, 20);
            this.Txt_DB_Username.TabIndex = 11;
            this.Txt_DB_Username.Text = "SYSTEM";
            // 
            // Txt_CompanyDB
            // 
            this.Txt_CompanyDB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Txt_CompanyDB.Location = new System.Drawing.Point(128, 113);
            this.Txt_CompanyDB.Name = "Txt_CompanyDB";
            this.Txt_CompanyDB.Size = new System.Drawing.Size(206, 20);
            this.Txt_CompanyDB.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 64);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "DB Username";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "B1 Password";
            // 
            // Txt_DB_Password
            // 
            this.Txt_DB_Password.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Txt_DB_Password.Location = new System.Drawing.Point(128, 87);
            this.Txt_DB_Password.Name = "Txt_DB_Password";
            this.Txt_DB_Password.PasswordChar = '*';
            this.Txt_DB_Password.Size = new System.Drawing.Size(206, 20);
            this.Txt_DB_Password.TabIndex = 9;
            // 
            // Txt_B1_Password
            // 
            this.Txt_B1_Password.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Txt_B1_Password.Location = new System.Drawing.Point(128, 165);
            this.Txt_B1_Password.Name = "Txt_B1_Password";
            this.Txt_B1_Password.PasswordChar = '*';
            this.Txt_B1_Password.Size = new System.Drawing.Size(206, 20);
            this.Txt_B1_Password.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "DB Password";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 142);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "B1 Username";
            // 
            // Txt_B1_Username
            // 
            this.Txt_B1_Username.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Txt_B1_Username.Location = new System.Drawing.Point(128, 139);
            this.Txt_B1_Username.Name = "Txt_B1_Username";
            this.Txt_B1_Username.Size = new System.Drawing.Size(206, 20);
            this.Txt_B1_Username.TabIndex = 7;
            this.Txt_B1_Username.Text = "manager";
            // 
            // Tab_Log
            // 
            this.Tab_Log.Controls.Add(this.Txt_Log);
            this.Tab_Log.Location = new System.Drawing.Point(4, 22);
            this.Tab_Log.Name = "Tab_Log";
            this.Tab_Log.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Log.Size = new System.Drawing.Size(905, 493);
            this.Tab_Log.TabIndex = 2;
            this.Tab_Log.Text = "Log";
            this.Tab_Log.UseVisualStyleBackColor = true;
            // 
            // Txt_Log
            // 
            this.Txt_Log.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Txt_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Txt_Log.Location = new System.Drawing.Point(3, 3);
            this.Txt_Log.Multiline = true;
            this.Txt_Log.Name = "Txt_Log";
            this.Txt_Log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Txt_Log.Size = new System.Drawing.Size(899, 487);
            this.Txt_Log.TabIndex = 0;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "SkyTech Metadata (*.stmt)|*.stmt|SkyTech Metadata (*.stmt)|*.stmt";
            // 
            // panel15
            // 
            this.panel15.Controls.Add(this.pictureBox1);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel15.Location = new System.Drawing.Point(0, 0);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(913, 66);
            this.panel15.TabIndex = 16;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(325, 66);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Stb_Message});
            this.statusStrip1.Location = new System.Drawing.Point(0, 585);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(913, 22);
            this.statusStrip1.TabIndex = 17;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // Stb_Message
            // 
            this.Stb_Message.Name = "Stb_Message";
            this.Stb_Message.Size = new System.Drawing.Size(34, 17);
            this.Stb_Message.Text = "         ";
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // saveFileDialog_Info
            // 
            this.saveFileDialog_Info.Filter = "SkyTech Metadata (*.info)|*.info|SkyTech Metadata (*.info)|*.info";
            // 
            // Tmr_Clear_Message
            // 
            this.Tmr_Clear_Message.Interval = 10000;
            this.Tmr_Clear_Message.Tick += new System.EventHandler(this.Tmr_Clear_Message_Tick);
            // 
            // Frm_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 607);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel15);
            this.Name = "Frm_Main";
            this.Text = "Create UDFs";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Frm_Main_FormClosed);
            this.Load += new System.EventHandler(this.Frm_Main_Load);
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.Tab_Database.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.Tab_Log.ResumeLayout(false);
            this.Tab_Log.PerformLayout();
            this.panel15.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel Stb_Message;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog_Info;
        private System.Windows.Forms.Timer Tmr_Clear_Message;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Tab_Database;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox Chk_Save_Password;
        private System.Windows.Forms.Button Btn_Connect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Txt_LicenseServer;
        private System.Windows.Forms.TextBox Txt_Server;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Txt_DB_Username;
        private System.Windows.Forms.TextBox Txt_CompanyDB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Txt_DB_Password;
        private System.Windows.Forms.TextBox Txt_B1_Password;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Txt_B1_Username;
        private System.Windows.Forms.TabPage Tab_Log;
        private System.Windows.Forms.TextBox Txt_Log;
    }
}