using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SocketFileTransfer
{
    public class CustomButton : Button
    {
        private Color _normalColor = Color.Black;
        private Color _disabledColor = Color.Gray;

        public CustomButton()
        {
            SetStyle(ControlStyles.UserPaint, true);
            UpdateForeColor();
        }

        public new Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                _normalColor = value;
                if (Enabled) base.ForeColor = value;
            }
        }

        public Color DisabledForeColor
        {
            get => _disabledColor;
            set
            {
                _disabledColor = value;
                if (!Enabled) base.ForeColor = value;
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            UpdateForeColor();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            TextRenderer.DrawText(pevent.Graphics, Text, Font, ClientRectangle,
                Enabled ? _normalColor : _disabledColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }

        private void UpdateForeColor()
        {
            base.ForeColor = Enabled ? _normalColor : _disabledColor;
        }
    }

    public partial class Form1 : Form
    {
        private TcpListener? serverListener;
        private Thread? serverThread;
        private bool serverRunning = false;

        private TcpClient? client;
        private NetworkStream? clientStream;
        private bool clientConnected = false;

        private string currentPath = @"C:\";

        private GroupBox groupBoxLeft;
        private ComboBox comboBoxDrives;
        private ListBox listBoxFiles;
        private Label labelIp;
        private TextBox txtIpAddress;
        private CustomButton btnServerStop;
        private CustomButton btnConnect;
        private CustomButton btnDisconnect;
        private CustomButton btnExit;
        private CustomButton btnSendToServer;
        private CustomButton btnSendToClient;
        private GroupBox groupBoxClient;
        private RichTextBox richTextBoxClient;
        private GroupBox groupBoxServer;
        private RichTextBox richTextBoxServer;

        public Form1()
        {
            InitializeComponent();
            LoadDrives();
            LoadFilesAndFolders(currentPath);
        }

        private void InitializeComponent()
        {
            groupBoxLeft = new GroupBox();
            comboBoxDrives = new ComboBox();
            listBoxFiles = new ListBox();
            labelIp = new Label();
            txtIpAddress = new TextBox();
            btnServerStop = new CustomButton();
            btnConnect = new CustomButton();
            btnDisconnect = new CustomButton();
            btnExit = new CustomButton();
            btnSendToServer = new CustomButton();
            btnSendToClient = new CustomButton();
            groupBoxClient = new GroupBox();
            richTextBoxClient = new RichTextBox();
            groupBoxServer = new GroupBox();
            richTextBoxServer = new RichTextBox();

            // groupBoxLeft
            groupBoxLeft.BackColor = Color.FromArgb(255, 248, 225);
            groupBoxLeft.Location = new Point(12, 12);
            groupBoxLeft.Size = new Size(320, 550);
            groupBoxLeft.TabIndex = 0;
            groupBoxLeft.TabStop = false;
            groupBoxLeft.Text = "";

            // comboBoxDrives
            comboBoxDrives.DropDownStyle = ComboBoxStyle.DropDown;
            comboBoxDrives.Location = new Point(6, 12);
            comboBoxDrives.Size = new Size(308, 23);
            comboBoxDrives.SelectionChangeCommitted += comboBoxDrives_SelectionChangeCommitted;
            comboBoxDrives.KeyPress += comboBoxDrives_KeyPress;

            // listBoxFiles
            listBoxFiles.Location = new Point(6, 40);
            listBoxFiles.Size = new Size(308, 160);
            listBoxFiles.DoubleClick += listBoxFiles_DoubleClick;

            // labelIp
            labelIp.Text = "IP-адрес:";
            labelIp.ForeColor = Color.Black;
            labelIp.Location = new Point(6, 210);
            labelIp.Size = new Size(65, 20);
            labelIp.TextAlign = ContentAlignment.MiddleLeft;

            // txtIpAddress
            txtIpAddress.Location = new Point(75, 210);
            txtIpAddress.Size = new Size(100, 20);
            txtIpAddress.Text = "127.0.0.1";

            // btnServerStop
            btnServerStop.Text = "Сервер отключить";
            btnServerStop.ForeColor = Color.Black;
            btnServerStop.Location = new Point(180, 208);
            btnServerStop.Size = new Size(134, 23);
            btnServerStop.Enabled = false;
            btnServerStop.Click += btnServerStop_Click;

            // btnConnect
            btnConnect.Text = "Соединиться";
            btnConnect.ForeColor = Color.Black;
            btnConnect.Location = new Point(6, 240);
            btnConnect.Size = new Size(95, 23);
            btnConnect.Click += btnConnect_Click;

            // btnDisconnect
            btnDisconnect.Text = "Отключиться";
            btnDisconnect.ForeColor = Color.Black;
            btnDisconnect.Location = new Point(107, 240);
            btnDisconnect.Size = new Size(95, 23);
            btnDisconnect.Enabled = false;
            btnDisconnect.Click += btnDisconnect_Click;

            // btnExit
            btnExit.Text = "Выход";
            btnExit.ForeColor = Color.Black;
            btnExit.Location = new Point(208, 240);
            btnExit.Size = new Size(106, 23);
            btnExit.Click += btnExit_Click;

            // btnSendToServer
            btnSendToServer.Text = "Передать серверу";
            btnSendToServer.ForeColor = Color.Black;
            btnSendToServer.Location = new Point(6, 275);
            btnSendToServer.Size = new Size(150, 30);
            btnSendToServer.Click += btnSendToServer_Click;

            // btnSendToClient
            btnSendToClient.Text = "Передать клиенту";
            btnSendToClient.ForeColor = Color.Black;
            btnSendToClient.Location = new Point(162, 275);
            btnSendToClient.Size = new Size(152, 30);
            btnSendToClient.Click += btnSendToClient_Click;

            groupBoxLeft.Controls.Add(comboBoxDrives);
            groupBoxLeft.Controls.Add(listBoxFiles);
            groupBoxLeft.Controls.Add(labelIp);
            groupBoxLeft.Controls.Add(txtIpAddress);
            groupBoxLeft.Controls.Add(btnServerStop);
            groupBoxLeft.Controls.Add(btnConnect);
            groupBoxLeft.Controls.Add(btnDisconnect);
            groupBoxLeft.Controls.Add(btnExit);
            groupBoxLeft.Controls.Add(btnSendToServer);
            groupBoxLeft.Controls.Add(btnSendToClient);

            // groupBoxClient
            groupBoxClient.BackColor = Color.FromArgb(255, 248, 225);
            groupBoxClient.Location = new Point(338, 12);
            groupBoxClient.Size = new Size(330, 550);
            groupBoxClient.TabIndex = 1;
            groupBoxClient.TabStop = false;
            groupBoxClient.Text = "Клиентская сторона";
            groupBoxClient.ForeColor = Color.Black;

            richTextBoxClient.BackColor = Color.White;
            richTextBoxClient.ForeColor = Color.Black;
            richTextBoxClient.Font = new Font("Consolas", 9F);
            richTextBoxClient.Location = new Point(6, 19);
            richTextBoxClient.Size = new Size(318, 525);
            richTextBoxClient.ReadOnly = true;
            richTextBoxClient.TabIndex = 0;

            groupBoxClient.Controls.Add(richTextBoxClient);

            // groupBoxServer
            groupBoxServer.BackColor = Color.FromArgb(255, 248, 225);
            groupBoxServer.Location = new Point(674, 12);
            groupBoxServer.Size = new Size(330, 550);
            groupBoxServer.TabIndex = 2;
            groupBoxServer.TabStop = false;
            groupBoxServer.Text = "Серверная сторона";
            groupBoxServer.ForeColor = Color.Black;

            richTextBoxServer.BackColor = Color.White;
            richTextBoxServer.ForeColor = Color.Black;
            richTextBoxServer.Font = new Font("Consolas", 9F);
            richTextBoxServer.Location = new Point(6, 19);
            richTextBoxServer.Size = new Size(318, 525);
            richTextBoxServer.ReadOnly = true;
            richTextBoxServer.TabIndex = 0;

            groupBoxServer.Controls.Add(richTextBoxServer);

            // Form1
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 248, 225);
            ClientSize = new Size(1016, 574);
            Controls.Add(groupBoxLeft);
            Controls.Add(groupBoxClient);
            Controls.Add(groupBoxServer);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Программа для обмена данными между компьютерами";
            FormClosing += Form1_FormClosing;
        }

        private void LoadDrives()
        {
            comboBoxDrives.Items.Clear();
            foreach (var drive in DriveInfo.GetDrives())
                comboBoxDrives.Items.Add(drive.Name);
            if (comboBoxDrives.Items.Count > 0)
                comboBoxDrives.SelectedIndex = 0;
        }

        private void LoadFilesAndFolders(string path)
        {
            listBoxFiles.Items.Clear();
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);

                if (dirInfo.Parent != null)
                {
                    listBoxFiles.Items.Add(".");
                    listBoxFiles.Items.Add("..");
                }

                foreach (var dir in dirInfo.EnumerateDirectories())
                    try { listBoxFiles.Items.Add(dir.Name); } catch { }

                foreach (var file in dirInfo.EnumerateFiles())
                    try { listBoxFiles.Items.Add(file.Name); } catch { }

                currentPath = path;
                UpdateComboBoxPath();
            }
            catch (Exception ex)
            {
                listBoxFiles.Items.Clear();
                string parent = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(parent))
                    listBoxFiles.Items.Add("..");

                MessageBox.Show($"Ошибка доступа: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateComboBoxPath()
        {
            comboBoxDrives.Text = GetElidedPath(currentPath);
        }

        private string GetElidedPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;

            int availableWidth = comboBoxDrives.Width - SystemInformation.VerticalScrollBarWidth - 4;
            using (var g = comboBoxDrives.CreateGraphics())
            {
                SizeF fullSize = TextRenderer.MeasureText(g, path, comboBoxDrives.Font);
                if (fullSize.Width <= availableWidth)
                    return path;

                string ellipsis = "...";
                int maxSuffixLen = path.Length;
                for (int i = 1; i <= path.Length; i++)
                {
                    string candidate = ellipsis + path.Substring(path.Length - i);
                    if (TextRenderer.MeasureText(g, candidate, comboBoxDrives.Font).Width > availableWidth)
                        break;
                    maxSuffixLen = i;
                }
                return ellipsis + path.Substring(path.Length - maxSuffixLen);
            }
        }

        private void comboBoxDrives_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboBoxDrives.SelectedItem != null)
            {
                currentPath = comboBoxDrives.SelectedItem.ToString();
                LoadFilesAndFolders(currentPath);
            }
        }

        private void comboBoxDrives_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void listBoxFiles_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem == null) return;
            string selected = listBoxFiles.SelectedItem.ToString();

            if (selected == ".")
            {
                string root = Path.GetPathRoot(currentPath);
                if (!string.IsNullOrEmpty(root))
                    LoadFilesAndFolders(root);
                return;
            }

            if (selected == "..")
            {
                DirectoryInfo parent = Directory.GetParent(currentPath);
                if (parent != null)
                    LoadFilesAndFolders(parent.FullName);
                return;
            }

            string newPath = Path.Combine(currentPath, selected);
            if (Directory.Exists(newPath))
            {
                try
                {
                    Directory.EnumerateFileSystemEntries(newPath).FirstOrDefault();
                    LoadFilesAndFolders(newPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Нет доступа к каталогу \"{selected}\": {ex.Message}",
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AppendLogClient(string msg)
        {
            if (richTextBoxClient.InvokeRequired)
            {
                richTextBoxClient.Invoke(new Action(() => AppendLogClient(msg)));
                return;
            }
            richTextBoxClient.AppendText(msg + Environment.NewLine);
            richTextBoxClient.ScrollToCaret();
        }

        private void AppendLogServer(string msg)
        {
            if (richTextBoxServer.InvokeRequired)
            {
                richTextBoxServer.Invoke(new Action(() => AppendLogServer(msg)));
                return;
            }
            richTextBoxServer.AppendText(msg + Environment.NewLine);
            richTextBoxServer.ScrollToCaret();
        }

        private void StartServer()
        {
            if (serverRunning) return;
            serverRunning = true;
            serverThread = new Thread(ServerThreadFunc);
            serverThread.IsBackground = true;
            serverThread.Start();
            btnServerStop.Enabled = true;
            AppendLogServer($"Сервер включен {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
        }

        private void StopServer()
        {
            serverRunning = false;
            serverListener?.Stop();
            btnServerStop.Enabled = false;
            AppendLogServer($"Сервер остановлен {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
        }

        private void ServerThreadFunc()
        {
            try
            {
                serverListener = new TcpListener(IPAddress.Any, 8888);
                serverListener.Start();

                while (serverRunning)
                {
                    if (serverListener.Pending())
                    {
                        TcpClient clientSocket = serverListener.AcceptTcpClient();
                        AppendLogServer($"Клиент соединился {DateTime.Now:dd.MM.yyyy HH:mm:ss} с адреса {((IPEndPoint)clientSocket.Client.RemoteEndPoint).Address}");
                        ThreadPool.QueueUserWorkItem(_ => HandleClient(clientSocket));
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                if (serverRunning)
                    AppendLogServer($"Ошибка сервера: {ex.Message}");
            }
        }

        private void HandleClient(TcpClient clientSocket)
        {
            try
            {
                using (clientSocket)
                using (NetworkStream stream = clientSocket.GetStream())
                {
                    string drives = string.Join(",", DriveInfo.GetDrives().Select(d => d.Name.TrimEnd('\\')));
                    byte[] drivesData = Encoding.UTF8.GetBytes(drives);
                    stream.Write(drivesData, 0, drivesData.Length);
                    stream.Flush();

                    byte[] buffer = new byte[4096];
                    int bytesRead;

                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        string path = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                        AppendLogServer($"Сервер получил {DateTime.Now:dd.MM.yyyy HH:mm:ss}: {path}");
                        string response = ProcessServerRequest(path);
                        byte[] responseData = Encoding.UTF8.GetBytes(response);
                        stream.Write(responseData, 0, responseData.Length);
                        stream.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is IOException || ex is ObjectDisposedException) return;
                AppendLogServer($"Ошибка при обработке клиента: {ex.Message}");
            }
        }

        private string ProcessServerRequest(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    var dirs = Directory.GetDirectories(path);
                    var files = Directory.GetFiles(path);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("-- КАТАЛОГИ --");
                    foreach (var d in dirs)
                        sb.AppendLine(Path.GetFileName(d));
                    sb.AppendLine("-- ФАЙЛЫ --");
                    foreach (var f in files)
                        sb.AppendLine(Path.GetFileName(f));
                    return sb.ToString();
                }
                else if (File.Exists(path))
                {
                    string content = File.ReadAllText(path, Encoding.UTF8);
                    if (content.Length > 10000)
                        content = content.Substring(0, 10000) + "\n... (файл обрезан)";
                    return content;
                }
                else
                {
                    return $"Ошибка: путь не существует: {path}";
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (clientConnected)
            {
                MessageBox.Show("Клиент уже подключен!");
                return;
            }

            if (!serverRunning)
            {
                StartServer();
                Thread.Sleep(500);
            }

            string serverIp = txtIpAddress.Text.Trim();
            if (string.IsNullOrEmpty(serverIp))
            {
                MessageBox.Show("Введите IP-адрес сервера");
                return;
            }

            try
            {
                client = new TcpClient();
                await client.ConnectAsync(IPAddress.Parse(serverIp), 8888);
                clientStream = client.GetStream();
                clientConnected = true;

                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;

                byte[] buffer = new byte[4096];
                int bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
                string drivesList = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                AppendLogClient($"Клиент получил {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                AppendLogClient(drivesList);

                if (comboBoxDrives.InvokeRequired)
                {
                    comboBoxDrives.Invoke(new Action(() =>
                    {
                        comboBoxDrives.Items.Clear();
                        foreach (var d in drivesList.Split(','))
                            comboBoxDrives.Items.Add(d + "\\");

                        if (comboBoxDrives.Items.Count > 0)
                        {
                            currentPath = comboBoxDrives.Items[0].ToString();
                            LoadFilesAndFolders(currentPath);
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                AppendLogClient($"Ошибка подключения: {ex.Message}");
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                clientConnected = false;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            DisconnectClient();
        }

        private void DisconnectClient()
        {
            if (clientConnected)
            {
                clientStream?.Close();
                client?.Close();
                clientConnected = false;
                AppendLogClient($"Соединение закрыто {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            }
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
        }

        private async void btnSendToServer_Click(object sender, EventArgs e)
        {
            if (!clientConnected)
            {
                MessageBox.Show("Клиент не подключен к серверу!");
                return;
            }

            if (listBoxFiles.SelectedItem == null)
            {
                MessageBox.Show("Выберите файл или папку для отправки!");
                return;
            }

            string selected = listBoxFiles.SelectedItem.ToString();
            string fullPath;

            if (selected == "..")
                fullPath = Directory.GetParent(currentPath)?.FullName ?? currentPath;
            else
                fullPath = Path.Combine(currentPath, selected);

            try
            {
                byte[] pathData = Encoding.UTF8.GetBytes(fullPath);
                await clientStream!.WriteAsync(pathData, 0, pathData.Length);
                await clientStream!.FlushAsync();

                AppendLogClient($"Отправлено серверу: {fullPath}");

                byte[] buffer = new byte[65536];
                int bytesRead = await clientStream!.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    AppendLogClient("Соединение разорвано сервером.");
                    DisconnectClient();
                    return;
                }
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                AppendLogClient($"Клиент получил {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                AppendLogClient(response);
            }
            catch (Exception ex)
            {
                AppendLogClient($"Ошибка отправки: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}");
                DisconnectClient();
            }
        }

        private void btnSendToClient_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem == null)
            {
                MessageBox.Show("Выберите файл для отправки клиенту!");
                return;
            }

            string selected = listBoxFiles.SelectedItem.ToString();
            if (selected == "..")
            {
                MessageBox.Show("Пожалуйста, выберите файл (не папку) для отправки клиенту!");
                return;
            }

            string fullPath = Path.Combine(currentPath, selected);

            if (Directory.Exists(fullPath))
            {
                MessageBox.Show("Пожалуйста, выберите файл (не папку) для отправки клиенту!");
                return;
            }

            try
            {
                string content = File.ReadAllText(fullPath, Encoding.UTF8);
                if (content.Length > 10000)
                    content = content.Substring(0, 10000) + "\n... (файл обрезан)";

                AppendLogClient($"Содержимое файла {selected}");
                AppendLogClient(content);
            }
            catch (Exception ex)
            {
                AppendLogClient($"Ошибка чтения файла: {ex.Message}");
                MessageBox.Show($"Ошибка чтения файла: {ex.Message}");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            StopServer();
            DisconnectClient();
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopServer();
            DisconnectClient();
        }

        private void btnServerStop_Click(object sender, EventArgs e)
        {
            StopServer();
        }
    }
}