using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LotmRussianPatcher
{
    public class MainForm : Form
    {
        private TextBox txtGamePath;
        private Button btnBrowse;
        private Button btnAutoDetect;
        private Button btnInstall;
        private Button btnToggleLang;
        private Button btnRestore;
        private Button btnCheckUpdates;
        private LinkLabel lnkGitHub;
        private Label lblStatus;
        private ProgressBar progressBar;
        private RichTextBox rtbLog;

        private const string GITHUB_REPO = "kapgrek/lotm-russian-patch";
        private const string GITHUB_API_URL = "https://api.github.com/repos/" + GITHUB_REPO + "/releases/latest";

        public MainForm()
        {
            InitializeComponent();
            AutoDetectGamePath();
            CheckCurrentStatus();
        }

        private void InitializeComponent()
        {
            this.Text = "Lord of the Mysteries — Установщик Русификатора v1.2";
            this.Size = new Size(680, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(20, 24, 30);
            this.ForeColor = Color.FromArgb(220, 225, 235);
            this.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            // Header Banner
            Panel pnlHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(680, 65),
                BackColor = Color.FromArgb(28, 33, 42)
            };
            Label lblTitle = new Label
            {
                Text = "Повелитель Тайн — Русская Локализация",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(212, 175, 55),
                Location = new Point(20, 10),
                AutoSize = true
            };
            Label lblSub = new Label
            {
                Text = "Автоматический установщик и менеджер обновлений",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(160, 170, 185),
                Location = new Point(22, 38),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSub);
            this.Controls.Add(pnlHeader);

            // Path Selection Group
            Label lblPathTitle = new Label
            {
                Text = "Папка с игрой (должна оканчиваться на Game\\C7):",
                Location = new Point(20, 80),
                AutoSize = true
            };
            this.Controls.Add(lblPathTitle);

            txtGamePath = new TextBox
            {
                Location = new Point(20, 105),
                Size = new Size(440, 26),
                BackColor = Color.FromArgb(32, 38, 48),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtGamePath.TextChanged += (s, e) => CheckCurrentStatus();
            this.Controls.Add(txtGamePath);

            btnBrowse = new Button
            {
                Text = "Обзор...",
                Location = new Point(470, 104),
                Size = new Size(85, 28),
                BackColor = Color.FromArgb(45, 52, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBrowse.FlatAppearance.BorderColor = Color.FromArgb(70, 80, 98);
            btnBrowse.Click += BtnBrowse_Click;
            this.Controls.Add(btnBrowse);

            btnAutoDetect = new Button
            {
                Text = "Автопоиск",
                Location = new Point(565, 104),
                Size = new Size(85, 28),
                BackColor = Color.FromArgb(45, 52, 65),
                ForeColor = Color.FromArgb(212, 175, 55),
                FlatStyle = FlatStyle.Flat
            };
            btnAutoDetect.FlatAppearance.BorderColor = Color.FromArgb(70, 80, 98);
            btnAutoDetect.Click += (s, e) => AutoDetectGamePath();
            this.Controls.Add(btnAutoDetect);

            // Status Label
            lblStatus = new Label
            {
                Text = "Статус: Проверка игры...",
                Location = new Point(20, 145),
                Size = new Size(630, 22),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(212, 175, 55)
            };
            this.Controls.Add(lblStatus);

            // Progress Bar
            progressBar = new ProgressBar
            {
                Location = new Point(20, 172),
                Size = new Size(630, 8),
                Visible = false
            };
            this.Controls.Add(progressBar);

            // Buttons Bar
            btnInstall = new Button
            {
                Text = "✔ Установить / Обновить",
                Location = new Point(20, 190),
                Size = new Size(200, 38),
                BackColor = Color.FromArgb(34, 139, 34),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnInstall.FlatAppearance.BorderSize = 0;
            btnInstall.Click += BtnInstall_Click;
            this.Controls.Add(btnInstall);

            btnToggleLang = new Button
            {
                Text = "🔄 Переключить язык",
                Location = new Point(230, 190),
                Size = new Size(190, 38),
                BackColor = Color.FromArgb(45, 52, 65),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                FlatStyle = FlatStyle.Flat
            };
            btnToggleLang.FlatAppearance.BorderColor = Color.FromArgb(70, 80, 98);
            btnToggleLang.Click += BtnToggleLang_Click;
            this.Controls.Add(btnToggleLang);

            btnRestore = new Button
            {
                Text = "↩ Исходный (Backup)",
                Location = new Point(430, 190),
                Size = new Size(160, 38),
                BackColor = Color.FromArgb(45, 52, 65),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                FlatStyle = FlatStyle.Flat
            };
            btnRestore.FlatAppearance.BorderColor = Color.FromArgb(70, 80, 98);
            btnRestore.Click += BtnRestore_Click;
            this.Controls.Add(btnRestore);

            btnCheckUpdates = new Button
            {
                Text = "🌐",
                Location = new Point(600, 190),
                Size = new Size(50, 38),
                BackColor = Color.FromArgb(45, 52, 65),
                ForeColor = Color.FromArgb(212, 175, 55),
                Font = new Font("Segoe UI", 11f, FontStyle.Regular),
                FlatStyle = FlatStyle.Flat
            };
            btnCheckUpdates.FlatAppearance.BorderColor = Color.FromArgb(70, 80, 98);
            btnCheckUpdates.Click += BtnCheckUpdates_Click;
            this.Controls.Add(btnCheckUpdates);

            // Log Console
            rtbLog = new RichTextBox
            {
                Location = new Point(20, 240),
                Size = new Size(630, 205),
                BackColor = Color.FromArgb(14, 17, 22),
                ForeColor = Color.FromArgb(180, 190, 205),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 9f)
            };
            this.Controls.Add(rtbLog);

            // Footer Link
            lnkGitHub = new LinkLabel
            {
                Text = "Репозиторий проекта на GitHub: github.com/" + GITHUB_REPO,
                Location = new Point(20, 455),
                AutoSize = true,
                LinkColor = Color.FromArgb(212, 175, 55),
                ActiveLinkColor = Color.White
            };
            lnkGitHub.LinkClicked += (s, e) => {
                try { Process.Start(new ProcessStartInfo("https://github.com/" + GITHUB_REPO) { UseShellExecute = true }); } catch { }
            };
            this.Controls.Add(lnkGitHub);

            Log("Добро пожаловать в установщик русификатора Lord of the Mysteries!");
        }

        private void Log(string msg)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new Action<string>(Log), msg);
                return;
            }
            rtbLog.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + msg + "\n");
            rtbLog.SelectionStart = rtbLog.Text.Length;
            rtbLog.ScrollToCaret();
        }

        private void AutoDetectGamePath()
        {
            string[] candidates = new string[]
            {
                @"D:\Games\GMZZLauncher\Game\C7",
                @"C:\Games\GMZZLauncher\Game\C7",
                @"E:\Games\GMZZLauncher\Game\C7",
                @"F:\Games\GMZZLauncher\Game\C7",
                @"C:\Program Files\GMZZLauncher\Game\C7",
                @"D:\Program Files\GMZZLauncher\Game\C7",
            };

            foreach (var path in candidates)
            {
                if (IsValidGameFolder(path))
                {
                    txtGamePath.Text = path;
                    Log("Автоматически обнаружена игра: " + path);
                    return;
                }
            }
            Log("Не удалось автоматически найти игру. Пожалуйста, укажите папку через кнопку 'Обзор'.");
        }

        private bool IsValidGameFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path)) return false;
            if (File.Exists(Path.Combine(path, "Lord of Mysteries.exe"))) return true;
            if (Directory.Exists(Path.Combine(path, "Binaries", "Win64"))) return true;
            if (File.Exists(Path.Combine(path, "..", "GamePackageConfig.txt"))) return true;
            return false;
        }

        private void CheckCurrentStatus()
        {
            string path = txtGamePath.Text.Trim();
            if (!IsValidGameFolder(path))
            {
                lblStatus.Text = "Статус: Укажите корректную папку Game\\C7";
                lblStatus.ForeColor = Color.OrangeRed;
                btnInstall.Enabled = false;
                btnToggleLang.Enabled = false;
                btnRestore.Enabled = false;
                return;
            }

            btnInstall.Enabled = true;
            string ruFile = Path.Combine(path, "Saved", "Mods", "lua", "mods", "cpdd_runtime_fixes", "RussianLocalization.lua");
            if (File.Exists(ruFile))
            {
                string text = File.ReadAllText(ruFile);
                if (text.Contains("Enabled = true") || text.Contains("Russian.Enabled = true"))
                {
                    lblStatus.Text = "Статус: Русификатор УСТАНОВЛЕН и АКТИВЕН (Русский)";
                    lblStatus.ForeColor = Color.LightGreen;
                    btnToggleLang.Text = "🔄 Переключить на English";
                }
                else
                {
                    lblStatus.Text = "Статус: Русификатор установлен, но ВЫКЛЮЧЕН (English)";
                    lblStatus.ForeColor = Color.Gold;
                    btnToggleLang.Text = "🔄 Переключить на Русский";
                }
                btnToggleLang.Enabled = true;
                btnRestore.Enabled = true;
            }
            else
            {
                lblStatus.Text = "Статус: Игра готова к установке русификатора";
                lblStatus.ForeColor = Color.White;
                btnToggleLang.Enabled = false;
                btnRestore.Enabled = false;
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Выберите папку с игрой Lord of Mysteries (оканчивающуюся на Game\\C7):";
                fbd.ShowNewFolderButton = false;
                if (!string.IsNullOrEmpty(txtGamePath.Text) && Directory.Exists(txtGamePath.Text))
                {
                    fbd.SelectedPath = txtGamePath.Text;
                }
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string selected = fbd.SelectedPath;
                    if (!selected.EndsWith("C7", StringComparison.OrdinalIgnoreCase) && Directory.Exists(Path.Combine(selected, "Game", "C7")))
                    {
                        selected = Path.Combine(selected, "Game", "C7");
                    }
                    txtGamePath.Text = selected;
                }
            }
        }

        private async void BtnInstall_Click(object sender, EventArgs e)
        {
            string gamePath = txtGamePath.Text.Trim();
            if (!IsValidGameFolder(gamePath))
            {
                MessageBox.Show("Укажите правильную папку с игрой!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверка запущенных процессов игры
            var procs = Process.GetProcessesByName("Lord of Mysteries");
            if (procs.Length == 0) procs = Process.GetProcessesByName("C7-Win64-Shipping");
            if (procs.Length > 0)
            {
                MessageBox.Show("Игра сейчас запущена! Пожалуйста, закройте игру перед установкой или обновлением русификатора.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnInstall.Enabled = false;
            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Marquee;

            Log("Начало процесса установки русификатора...");

            await Task.Run(() =>
            {
                try
                {
                    string modsDir = Path.Combine(gamePath, "Saved", "Mods");
                    string luaFixesDir = Path.Combine(modsDir, "lua", "mods", "cpdd_runtime_fixes");
                    if (!Directory.Exists(luaFixesDir)) Directory.CreateDirectory(luaFixesDir);

                    // 1. Попытка загрузить последний релиз с GitHub
                    bool downloadedFromGitHub = false;
                    try
                    {
                        Log("Проверка актуальных файлов на GitHub (" + GITHUB_REPO + ")...");
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        using (WebClient wc = new WebClient())
                        {
                            wc.Headers.Add("User-Agent", "Lotm-Russian-Patcher");
                            string json = wc.DownloadString(GITHUB_API_URL);
                            // Простой парсинг browser_download_url для lom-russian-patch-data.zip
                            int zipIdx = json.IndexOf("lom-russian-patch-data.zip");
                            if (zipIdx > 0)
                            {
                                int urlStart = json.LastIndexOf("https://", zipIdx);
                                int urlEnd = json.IndexOf("\"", urlStart);
                                if (urlStart > 0 && urlEnd > urlStart)
                                {
                                    string downloadUrl = json.Substring(urlStart, urlEnd - urlStart);
                                    Log("Скачивание актуального пакета с GitHub: " + downloadUrl);
                                    string tempZip = Path.Combine(Path.GetTempPath(), "lom-russian-patch-data.zip");
                                    wc.DownloadFile(downloadUrl, tempZip);

                                    Log("Распаковка обновления...");
                                    using (ZipArchive archive = ZipFile.OpenRead(tempZip))
                                    {
                                        foreach (ZipArchiveEntry entry in archive.Entries)
                                        {
                                            string fullPath = Path.Combine(gamePath, entry.FullName);
                                            if (string.IsNullOrEmpty(entry.Name))
                                            {
                                                Directory.CreateDirectory(fullPath);
                                            }
                                            else
                                            {
                                                string parentDir = Path.GetDirectoryName(fullPath);
                                                if (!Directory.Exists(parentDir)) Directory.CreateDirectory(parentDir);
                                                using (Stream entryStream = entry.Open())
                                                using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                                                {
                                                    entryStream.CopyTo(fs);
                                                }
                                            }
                                        }
                                    }
                                    File.Delete(tempZip);
                                    downloadedFromGitHub = true;
                                    Log("Файлы успешно загружены и распакованы с GitHub!");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("GitHub релиз недоступен или еще не создан: " + ex.Message);
                        Log("Используем локальные файлы русификатора...");
                    }

                    // 2. Если GitHub пока пуст, копируем локальные файлы мода
                    if (!downloadedFromGitHub)
                    {
                        string localSource = AppDomain.CurrentDomain.BaseDirectory;
                        string[] filesToCopy = new string[] { "RussianLocalization.lua", "RuntimeTextRussian.lua", "Init.lua", "bootstrap.lua" };
                        foreach (var f in filesToCopy)
                        {
                            string src = Path.Combine(localSource, f);
                            if (File.Exists(src))
                            {
                                string dest = (f == "bootstrap.lua") ? Path.Combine(modsDir, f) : Path.Combine(luaFixesDir, f);
                                File.Copy(src, dest, true);
                                Log("Скопирован локальный файл: " + f);
                            }
                        }
                    }

                    // 3. Подключение хука в CPDDTranslation.lua
                    string binDir = Path.Combine(gamePath, "Binaries", "Win64", "lua", "Launch", "Base");
                    if (!Directory.Exists(binDir)) Directory.CreateDirectory(binDir);
                    string cpddLua = Path.Combine(binDir, "CPDDTranslation.lua");
                    string expectedHook = "local original = require(\"Launch.Base.LaunchStringExt\")\n\n"
                        + "local File = import(\"LuaFunctionLibrary\")\n"
                        + "local path = File.GetFilePath(import(\"BlueprintPathsLibrary\").ProjectSavedDir()) .. \"/Mods/bootstrap.lua\"\n"
                        + "local source = File.LoadFile(path)\n"
                        + "LaunchLog.Info(\"[LOMModLoader] bootstrap path=\" .. path .. \" bytes=\" .. tostring(source and #source or 0))\n"
                        + "if source and source ~= \"\" then\n"
                        + "    local chunk, message = load(source, \"@\" .. path)\n"
                        + "    if chunk then xpcall(chunk, LaunchLog.Error) else LaunchLog.Error(message) end\n"
                        + "end\n\n"
                        + "return original\n";
                    File.WriteAllText(cpddLua, expectedHook, System.Text.Encoding.UTF8);
                    Log("Хук загрузчика успешно прописан в CPDDTranslation.lua");

                    Log("✔ УСТАНОВКА УСПЕШНО ЗАВЕРШЕНА!");
                }
                catch (Exception ex)
                {
                    Log("ОШИБКА установки: " + ex.Message);
                    MessageBox.Show("Ошибка при установке: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });

            progressBar.Visible = false;
            btnInstall.Enabled = true;
            CheckCurrentStatus();
        }

        private void BtnToggleLang_Click(object sender, EventArgs e)
        {
            string gamePath = txtGamePath.Text.Trim();
            string ruFile = Path.Combine(gamePath, "Saved", "Mods", "lua", "mods", "cpdd_runtime_fixes", "RussianLocalization.lua");
            if (!File.Exists(ruFile)) return;

            try
            {
                string text = File.ReadAllText(ruFile);
                if (text.Contains("Russian.Enabled = true") || text.Contains("Enabled = true"))
                {
                    text = text.Replace("Russian.Enabled = true", "Russian.Enabled = false");
                    text = text.Replace("Enabled = true", "Enabled = false");
                    File.WriteAllText(ruFile, text, System.Text.Encoding.UTF8);
                    Log("Язык переключен на АНГЛИЙСКИЙ");
                }
                else
                {
                    text = text.Replace("Russian.Enabled = false", "Russian.Enabled = true");
                    text = text.Replace("Enabled = false", "Enabled = true");
                    File.WriteAllText(ruFile, text, System.Text.Encoding.UTF8);
                    Log("Язык переключен на РУССКИЙ");
                }
                CheckCurrentStatus();
            }
            catch (Exception ex)
            {
                Log("Ошибка переключения: " + ex.Message);
            }
        }

        private void BtnRestore_Click(object sender, EventArgs e)
        {
            string gamePath = txtGamePath.Text.Trim();
            string fixesDir = Path.Combine(gamePath, "Saved", "Mods", "lua", "mods", "cpdd_runtime_fixes");
            string bak = Path.Combine(fixesDir, "Init.lua.bak_orig");
            string init = Path.Combine(fixesDir, "Init.lua");

            if (File.Exists(bak))
            {
                File.Copy(bak, init, true);
                Log("Исходный файл Init.lua восстановлен из бэкапа!");
                MessageBox.Show("Исходный английский Init.lua восстановлен!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CheckCurrentStatus();
            }
            else
            {
                MessageBox.Show("Файл резервной копии не найден.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnCheckUpdates_Click(object sender, EventArgs e)
        {
            Log("Проверка обновлений на GitHub...");
            btnCheckUpdates.Enabled = false;
            await Task.Run(() =>
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers.Add("User-Agent", "Lotm-Russian-Patcher");
                        string json = wc.DownloadString(GITHUB_API_URL);
                        Log("Связь с GitHub установлена успешно!");
                        MessageBox.Show("Репозиторий доступен! Актуальная версия доступна на GitHub.", "Обновления", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    Log("Информация об обновлениях: " + ex.Message);
                }
            });
            btnCheckUpdates.Enabled = true;
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
