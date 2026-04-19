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
    // Кастомный класс кнопки 
    public class CustomButton : Button
    {
        private Color _normalColor = Color.Black;
        private Color _disabledColor = Color.Gray;

        public CustomButton()
        {
            // Отключаем системную отрисовку текста для неактивных кнопок
            SetStyle(ControlStyles.UserPaint, true);
            UpdateForeColor();
        }

        public new Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                _normalColor = value;
                if (Enabled)
                    base.ForeColor = value;
            }
        }

        public Color DisabledForeColor
        {
            get => _disabledColor;
            set
            {
                _disabledColor = value;
                if (!Enabled)
                    base.ForeColor = value;
            }
        }

        // Событие при изменении состояния активности кнопки
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            UpdateForeColor();
            Invalidate();
        }

        // переопределённая отрисовка кнопки
        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Сначала рисуем стандартную кнопку
            base.OnPaint(pevent);

            // Потом перерисовываем текст с нужным цветом
            TextRenderer.DrawText(pevent.Graphics, Text, Font, ClientRectangle,
            Enabled ? _normalColor : _disabledColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }

        // Обновление цвета текста
        private void UpdateForeColor()
        {
            if (Enabled)
                base.ForeColor = _normalColor;
            else
                base.ForeColor = _disabledColor;
        }
    }

    public partial class Form1 : Form
    {
        // Сервер
        private TcpListener? serverListener;
        private Thread? serverThread;
        private bool serverRunning = false;

        // Клиент
        private TcpClient? client;
        private NetworkStream? clientStream;
        private bool clientConnected = false;

        // Для навигации по папкам
        private string currentPath = @"C:\";

        // Конструктор формы
        public Form1()
        {
            InitializeComponent();
            LoadDrives();
            LoadFilesAndFolders(currentPath);
        }

        // Загрузка списка логических дисков
        private void LoadDrives()
        {
            comboBoxDrives.Items.Clear();
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                comboBoxDrives.Items.Add(drive.Name);
            }
            if (comboBoxDrives.Items.Count > 0)
                comboBoxDrives.SelectedIndex = 0;
        }

        // Загрузка файлов и папок из указанного пути
        private void LoadFilesAndFolders(string path)
        {
            try
            {
                listBoxFiles.Items.Clear();
                DirectoryInfo dirInfo = new DirectoryInfo(path);

                if (dirInfo.Parent != null)
                {
                    listBoxFiles.Items.Add("..");
                }

                foreach (var dir in dirInfo.GetDirectories())
                {
                    listBoxFiles.Items.Add(dir.Name);
                }

                foreach (var file in dirInfo.GetFiles())
                {
                    listBoxFiles.Items.Add(file.Name);
                }

                currentPath = path;
            }
            catch (Exception ex)
            {
                listBoxFiles.Items.Clear();
                listBoxFiles.Items.Add("Ошибка доступа: " + ex.Message);
            }
        }

        // Обработчик изменения выбранного диска
        private void comboBoxDrives_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDrives.SelectedItem != null)
            {
                currentPath = comboBoxDrives.SelectedItem.ToString();
                LoadFilesAndFolders(currentPath);
            }
        }

        // Обработчик двойного клика по элементу списка (вход в папку)
        private void listBoxFiles_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem == null) return;

            string selected = listBoxFiles.SelectedItem.ToString();

            if (selected == "..")
            {
                DirectoryInfo parent = Directory.GetParent(currentPath);
                if (parent != null)
                    LoadFilesAndFolders(parent.FullName);
            }
            else
            {
                string newPath = Path.Combine(currentPath, selected);
                if (Directory.Exists(newPath))
                {
                    LoadFilesAndFolders(newPath);
                }
            }
        }

        // Добавление сообщения в лог клиента
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

        // Добавление сообщения в лог сервера
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

        // Запуск сервера
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

        // Обработчик кнопки остановки сервера
        private void btnServerStop_Click(object sender, EventArgs e)
        {
            StopServer();
        }

        // Остановка сервера
        private void StopServer()
        {
            serverRunning = false;
            serverListener?.Stop();
            btnServerStop.Enabled = false;
            AppendLogServer($"Сервер остановлен {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
        }

        // Поток сервера (ожидание подключений и обработка запросов)
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

                        using (NetworkStream stream = clientSocket.GetStream())
                        {
                            string drives = string.Join(",", DriveInfo.GetDrives().Select(d => d.Name.TrimEnd('\\')));
                            byte[] drivesData = Encoding.UTF8.GetBytes(drives);
                            stream.Write(drivesData, 0, drivesData.Length);
                            stream.Flush();

                            byte[] buffer = new byte[4096];
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);
                            string path = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                            AppendLogServer($"Сервер получил {DateTime.Now:dd.MM.yyyy HH:mm:ss}: {path}");

                            string response = ProcessServerRequest(path);
                            byte[] responseData = Encoding.UTF8.GetBytes(response);
                            stream.Write(responseData, 0, responseData.Length);
                            stream.Flush();
                        }
                        clientSocket.Close();
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

        // Обработка запроса от клиента (каталог или файл)
        private string ProcessServerRequest(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    var dirs = Directory.GetDirectories(path);
                    var files = Directory.GetFiles(path);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("КАТАЛОГИ");
                    foreach (var d in dirs)
                        sb.AppendLine(Path.GetFileName(d));
                    sb.AppendLine("ФАЙЛЫ");
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

        // Обработчик кнопки подключения к серверу
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
                            comboBoxDrives.SelectedIndex = 0;
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

        // Обработчик кнопки отключения от сервера
        private void btnDisconnect_Click(object sender, EventArgs e)
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

        // Обработчик кнопки отправки данных на сервер
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
            {
                fullPath = Directory.GetParent(currentPath)?.FullName ?? currentPath;
            }
            else
            {
                fullPath = Path.Combine(currentPath, selected);
            }

            try
            {
                byte[] pathData = Encoding.UTF8.GetBytes(fullPath);
                await clientStream!.WriteAsync(pathData, 0, pathData.Length);
                await clientStream!.FlushAsync();

                AppendLogClient($"Отправлено серверу: {fullPath}");

                byte[] buffer = new byte[65536];
                int bytesRead = await clientStream!.ReadAsync(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                AppendLogClient($"Клиент получил {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                AppendLogClient(response);
            }
            catch (Exception ex)
            {
                AppendLogClient($"Ошибка отправки: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        // Обработчик кнопки показа содержимого файла (передать клиенту)
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

        // Обработчик кнопки выхода из приложения
        private void btnExit_Click(object sender, EventArgs e)
        {
            StopServer();
            if (clientConnected)
            {
                clientStream?.Close();
                client?.Close();
            }
            Application.Exit();
        }

        // Обработчик закрытия формы
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopServer();
            clientStream?.Close();
            client?.Close();
        }
    }
}