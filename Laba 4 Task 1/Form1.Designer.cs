namespace SocketFileTransfer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // Левая панель
            this.groupBoxLeft = new System.Windows.Forms.GroupBox();
            this.comboBoxDrives = new System.Windows.Forms.ComboBox();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.labelIp = new System.Windows.Forms.Label();
            this.txtIpAddress = new System.Windows.Forms.TextBox();
            this.btnServerStop = new CustomButton();
            this.btnConnect = new CustomButton();
            this.btnDisconnect = new CustomButton();
            this.btnExit = new CustomButton();
            this.btnSendToServer = new CustomButton();
            this.btnSendToClient = new CustomButton();

            // Центральная панель - Клиент
            this.groupBoxClient = new System.Windows.Forms.GroupBox();
            this.richTextBoxClient = new System.Windows.Forms.RichTextBox();

            // Правая панель - Сервер
            this.groupBoxServer = new System.Windows.Forms.GroupBox();
            this.richTextBoxServer = new System.Windows.Forms.RichTextBox();

            // groupBoxLeft
            this.groupBoxLeft.BackColor = System.Drawing.Color.FromArgb(255, 248, 225);
            this.groupBoxLeft.Location = new System.Drawing.Point(12, 12);
            this.groupBoxLeft.Size = new System.Drawing.Size(320, 550);
            this.groupBoxLeft.TabIndex = 0;
            this.groupBoxLeft.TabStop = false;
            this.groupBoxLeft.Text = "";

            // comboBoxDrives
            this.comboBoxDrives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDrives.Location = new System.Drawing.Point(6, 12);
            this.comboBoxDrives.Size = new System.Drawing.Size(308, 21);
            this.comboBoxDrives.SelectedIndexChanged += new System.EventHandler(this.comboBoxDrives_SelectedIndexChanged);

            // listBoxFiles
            this.listBoxFiles.Location = new System.Drawing.Point(6, 39);
            this.listBoxFiles.Size = new System.Drawing.Size(308, 160);
            this.listBoxFiles.DoubleClick += new System.EventHandler(this.listBoxFiles_DoubleClick);

            // labelIp
            this.labelIp.Text = "IP-адрес:";
            this.labelIp.ForeColor = System.Drawing.Color.Black;
            this.labelIp.Location = new System.Drawing.Point(6, 210);
            this.labelIp.Size = new System.Drawing.Size(65, 20);
            this.labelIp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // txtIpAddress
            this.txtIpAddress.Location = new System.Drawing.Point(75, 210);
            this.txtIpAddress.Size = new System.Drawing.Size(100, 20);
            this.txtIpAddress.Text = "127.0.0.1";

            // btnServerStop - Сервер отключить
            this.btnServerStop.Text = "Сервер отключить";
            this.btnServerStop.ForeColor = System.Drawing.Color.Black;
            this.btnServerStop.Location = new System.Drawing.Point(180, 208);
            this.btnServerStop.Size = new System.Drawing.Size(134, 23);
            this.btnServerStop.Enabled = false;
            this.btnServerStop.Click += new System.EventHandler(this.btnServerStop_Click);

            // btnConnect - Соединиться
            this.btnConnect.Text = "Соединиться";
            this.btnConnect.ForeColor = System.Drawing.Color.Black;
            this.btnConnect.Location = new System.Drawing.Point(6, 245);
            this.btnConnect.Size = new System.Drawing.Size(95, 23);
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);

            // btnDisconnect - Отключиться
            this.btnDisconnect.Text = "Отключиться";
            this.btnDisconnect.ForeColor = System.Drawing.Color.Black;
            this.btnDisconnect.Location = new System.Drawing.Point(107, 245);
            this.btnDisconnect.Size = new System.Drawing.Size(95, 23);
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);

            // btnExit - Выход
            this.btnExit.Text = "Выход";
            this.btnExit.ForeColor = System.Drawing.Color.Black;
            this.btnExit.Location = new System.Drawing.Point(208, 245);
            this.btnExit.Size = new System.Drawing.Size(106, 23);
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);

            // btnSendToServer - Передать серверу
            this.btnSendToServer.Text = "Передать серверу";
            this.btnSendToServer.ForeColor = System.Drawing.Color.Black;
            this.btnSendToServer.Location = new System.Drawing.Point(6, 280);
            this.btnSendToServer.Size = new System.Drawing.Size(150, 30);
            this.btnSendToServer.Click += new System.EventHandler(this.btnSendToServer_Click);

            // btnSendToClient - Передать клиенту
            this.btnSendToClient.Text = "Передать клиенту";
            this.btnSendToClient.ForeColor = System.Drawing.Color.Black;
            this.btnSendToClient.Location = new System.Drawing.Point(162, 280);
            this.btnSendToClient.Size = new System.Drawing.Size(152, 30);
            this.btnSendToClient.Click += new System.EventHandler(this.btnSendToClient_Click);

            // Добавляем элементы в groupBoxLeft
            this.groupBoxLeft.Controls.Add(this.comboBoxDrives);
            this.groupBoxLeft.Controls.Add(this.listBoxFiles);
            this.groupBoxLeft.Controls.Add(this.labelIp);
            this.groupBoxLeft.Controls.Add(this.txtIpAddress);
            this.groupBoxLeft.Controls.Add(this.btnServerStop);
            this.groupBoxLeft.Controls.Add(this.btnConnect);
            this.groupBoxLeft.Controls.Add(this.btnDisconnect);
            this.groupBoxLeft.Controls.Add(this.btnExit);
            this.groupBoxLeft.Controls.Add(this.btnSendToServer);
            this.groupBoxLeft.Controls.Add(this.btnSendToClient);

            // groupBoxClient
            this.groupBoxClient.BackColor = System.Drawing.Color.FromArgb(255, 248, 225);
            this.groupBoxClient.Location = new System.Drawing.Point(338, 12);
            this.groupBoxClient.Size = new System.Drawing.Size(330, 550);
            this.groupBoxClient.TabIndex = 1;
            this.groupBoxClient.TabStop = false;
            this.groupBoxClient.Text = "Клиентская сторона";
            this.groupBoxClient.ForeColor = System.Drawing.Color.Black;

            // richTextBoxClient
            this.richTextBoxClient.BackColor = System.Drawing.Color.White;
            this.richTextBoxClient.ForeColor = System.Drawing.Color.Black;
            this.richTextBoxClient.Font = new System.Drawing.Font("Consolas", 9F);
            this.richTextBoxClient.Location = new System.Drawing.Point(6, 19);
            this.richTextBoxClient.Size = new System.Drawing.Size(318, 525);
            this.richTextBoxClient.ReadOnly = true;
            this.richTextBoxClient.TabIndex = 0;

            this.groupBoxClient.Controls.Add(this.richTextBoxClient);

            // groupBoxServer
            this.groupBoxServer.BackColor = System.Drawing.Color.FromArgb(255, 248, 225);
            this.groupBoxServer.Location = new System.Drawing.Point(674, 12);
            this.groupBoxServer.Size = new System.Drawing.Size(330, 550);
            this.groupBoxServer.TabIndex = 2;
            this.groupBoxServer.TabStop = false;
            this.groupBoxServer.Text = "Серверная сторона";
            this.groupBoxServer.ForeColor = System.Drawing.Color.Black;

            // richTextBoxServer
            this.richTextBoxServer.BackColor = System.Drawing.Color.White;
            this.richTextBoxServer.ForeColor = System.Drawing.Color.Black;
            this.richTextBoxServer.Font = new System.Drawing.Font("Consolas", 9F);
            this.richTextBoxServer.Location = new System.Drawing.Point(6, 19);
            this.richTextBoxServer.Size = new System.Drawing.Size(318, 525);
            this.richTextBoxServer.ReadOnly = true;
            this.richTextBoxServer.TabIndex = 0;

            this.groupBoxServer.Controls.Add(this.richTextBoxServer);

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(255, 248, 225);
            this.ClientSize = new System.Drawing.Size(1016, 574);
            this.Controls.Add(this.groupBoxLeft);
            this.Controls.Add(this.groupBoxClient);
            this.Controls.Add(this.groupBoxServer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Программа для обмена данными между компьютерами";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
        }

        private System.Windows.Forms.GroupBox groupBoxLeft;
        private System.Windows.Forms.ComboBox comboBoxDrives;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Label labelIp;
        private System.Windows.Forms.TextBox txtIpAddress;
        private CustomButton btnServerStop;
        private CustomButton btnConnect;
        private CustomButton btnDisconnect;
        private CustomButton btnExit;
        private CustomButton btnSendToServer;
        private CustomButton btnSendToClient;
        private System.Windows.Forms.GroupBox groupBoxClient;
        private System.Windows.Forms.RichTextBox richTextBoxClient;
        private System.Windows.Forms.GroupBox groupBoxServer;
        private System.Windows.Forms.RichTextBox richTextBoxServer;
    }
}