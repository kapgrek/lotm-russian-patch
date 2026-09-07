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

        private static string GetAppVersion()
        {
            Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return string.Format("v{0}.{1}.{2}", v.Major, v.Minor, v.Build);
        }

        private void InitializeComponent()
        {
            this.Text = "Lord of the Mysteries — Установщик Русификатора " + GetAppVersion();
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
            if (Directory.Exists(Path.Combine(path, "Binaries", "Win64"))) return true;
            if (File.Exists(Path.Combine(path, "Content", "Paks", "pakchunk0-Windows.pak"))) return true;
            if (File.Exists(Path.Combine(path, "Lord of Mysteries.exe"))) return true;
            if (File.Exists(Path.Combine(path, "..", "Lord of Mysteries.exe"))) return true;
            if (File.Exists(Path.Combine(path, "..", "GamePackageConfig.txt"))) return true;
            return false;
        }

        private void CheckCurrentStatus()
        {
            string path = txtGamePath.Text.Trim();
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                if (!path.EndsWith("C7", StringComparison.OrdinalIgnoreCase))
                {
                    if (Directory.Exists(Path.Combine(path, "C7")))
                    {
                        path = Path.Combine(path, "C7");
                        txtGamePath.Text = path;
                    }
                    else if (Directory.Exists(Path.Combine(path, "Game", "C7")))
                    {
                        path = Path.Combine(path, "Game", "C7");
                        txtGamePath.Text = path;
                    }
                }
            }

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
                    if (!selected.EndsWith("C7", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Directory.Exists(Path.Combine(selected, "C7")))
                        {
                            selected = Path.Combine(selected, "C7");
                        }
                        else if (Directory.Exists(Path.Combine(selected, "Game", "C7")))
                        {
                            selected = Path.Combine(selected, "Game", "C7");
                        }
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
                                                EnsureWritable(fullPath);
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

                    // 2. Проверка локального архива пакета данных (если запуск без интернета)
                    if (!downloadedFromGitHub)
                    {
                        string localZip = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lom-russian-patch-data.zip");
                        if (File.Exists(localZip))
                        {
                            try
                            {
                                Log("Найден локальный архив пакета данных: " + localZip);
                                Log("Распаковка локального пакета...");
                                using (ZipArchive archive = ZipFile.OpenRead(localZip))
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
                                            EnsureWritable(fullPath);
                                            using (Stream entryStream = entry.Open())
                                            using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                                            {
                                                entryStream.CopyTo(fs);
                                            }
                                        }
                                    }
                                }
                                downloadedFromGitHub = true;
                                Log("Локальный пакет данных успешно распакован!");
                            }
                            catch (Exception ex)
                            {
                                Log("Ошибка распаковки локального архива: " + ex.Message);
                            }
                        }
                    }

                    // 3. Если архив не найден, копируем отдельные локальные файлы мода
                    if (!downloadedFromGitHub)
                    {
                        string localSource = AppDomain.CurrentDomain.BaseDirectory;
                        string[] filesToCopy = new string[] { "RussianLocalization.lua", "RuntimeTextRussian.lua", "Init.lua", "bootstrap.lua", "manifest.lua", "translation-overrides.lua", "CPDDTranslation.lua", "EnglishToRussian.lua" };
                        foreach (var f in filesToCopy)
                        {
                            string src = Path.Combine(localSource, f);
                            if (File.Exists(src))
                            {
                                string dest;
                                if (f == "bootstrap.lua" || f == "manifest.lua" || f == "translation-overrides.lua")
                                    dest = Path.Combine(modsDir, f);
                                else if (f == "CPDDTranslation.lua")
                                    dest = Path.Combine(Path.Combine(gamePath, "Binaries", "Win64", "lua", "Launch", "Base"), f);
                                else
                                    dest = Path.Combine(luaFixesDir, f);

                                string destDir = Path.GetDirectoryName(dest);
                                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                                EnsureWritable(dest);
                                File.Copy(src, dest, true);
                                Log("Скопирован локальный файл: " + f);
                            }
                        }

                        // Если рядом есть папка shards, копируем все 1024 шарда
                        string localShards = Path.Combine(localSource, "shards");
                        if (Directory.Exists(localShards))
                        {
                            foreach (var shardFile in Directory.GetFiles(localShards, "RuntimeTextGemini_*.lua"))
                            {
                                string dest = Path.Combine(luaFixesDir, Path.GetFileName(shardFile));
                                EnsureWritable(dest);
                                File.Copy(shardFile, dest, true);
                            }
                            Log("Скопированы локальные шарды базы перевода.");
                        }
                    }

                    // 3. Проверка хука в CPDDTranslation.lua
                    string binDir = Path.Combine(gamePath, "Binaries", "Win64", "lua", "Launch", "Base");
                    if (!Directory.Exists(binDir)) Directory.CreateDirectory(binDir);
                    string cpddLua = Path.Combine(binDir, "CPDDTranslation.lua");
                    if (!File.Exists(cpddLua) || !File.ReadAllText(cpddLua).Contains("LOMModLoader"))
                    {
                        string localCpdd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CPDDTranslation.lua");
                        if (File.Exists(localCpdd))
                        {
                            EnsureWritable(cpddLua);
                            File.Copy(localCpdd, cpddLua, true);
                            Log("Хук загрузчика скопирован из локального CPDDTranslation.lua");
                        }
                    }
                    else
                    {
                        Log("Хук загрузчика проверен в CPDDTranslation.lua");
                    }

                    // 4. Проверка и установка нативного хука в pakchunk0-Windows.pak
                    PatchPakLaunchHook(gamePath);

                    Log("✔ УСТАНОВКА УСПЕШНО ЗАВЕРШЕНА!");
                }
                catch (UnauthorizedAccessException uex)
                {
                    Log("ОШИБКА ДОСТУПА: " + uex.Message);
                    DialogResult res = MessageBox.Show(
                        "Отказано в доступе к папке с игрой:\n" + uex.Message + "\n\n" +
                        "Игра установлена в защищённую системную папку (например, C:\\Program Files).\n" +
                        "Для записи файлов требуются права администратора.\n\n" +
                        "Перезапустить установщик от имени администратора прямо сейчас?",
                        "Требуются права администратора",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (res == DialogResult.Yes)
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = Application.ExecutablePath,
                                UseShellExecute = true,
                                Verb = "runas"
                            });
                            Application.Exit();
                        }
                        catch { }
                    }
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
                EnsureWritable(ruFile);
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
            RestorePakLaunchHook(gamePath);
            string fixesDir = Path.Combine(gamePath, "Saved", "Mods", "lua", "mods", "cpdd_runtime_fixes");
            string bak = Path.Combine(fixesDir, "Init.lua.bak_orig");
            string init = Path.Combine(fixesDir, "Init.lua");

            if (File.Exists(bak))
            {
                try
                {
                    EnsureWritable(init);
                    File.Copy(bak, init, true);
                    Log("Исходный файл Init.lua восстановлен из бэкапа!");
                    MessageBox.Show("Исходный английский Init.lua восстановлен!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CheckCurrentStatus();
                }
                catch (Exception ex)
                {
                    Log("Ошибка восстановления: " + ex.Message);
                    MessageBox.Show("Ошибка восстановления: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        private const long PAK_HOOK_OFFSET = 427225161L;
        private const int PAK_HOOK_SIZE = 4660;
        private const string PAK_HOOK_SHA256 = "c031726986e09358bb18ff8a2b8ee5f0b4e65ce8ae8331eed2d7575c80b7efa9";

        private static string ComputeSha256(byte[] data)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(data);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (byte b in hash) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private bool PatchPakLaunchHook(string gamePath)
        {
            try
            {
                string pakPath = Path.Combine(gamePath, "Content", "Paks", "pakchunk0-Windows.pak");
                if (!File.Exists(pakPath))
                {
                    Log("Внимание: Файл pakchunk0-Windows.pak не найден в Content\\Paks.");
                    return false;
                }

                byte[] hookPayload = null;
                string[] candidates = new string[]
                {
                    Path.Combine(gamePath, "LaunchInstance.native-bridge.padded.oodle"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LaunchInstance.native-bridge.padded.oodle"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "LaunchInstance.native-bridge.padded.oodle"),
                };
                foreach (var p in candidates)
                {
                    if (File.Exists(p) && new FileInfo(p).Length == PAK_HOOK_SIZE)
                    {
                        hookPayload = File.ReadAllBytes(p);
                        break;
                    }
                }

                if (hookPayload == null || hookPayload.Length != PAK_HOOK_SIZE)
                {
                    Log("Внимание: Файл хука LaunchInstance.native-bridge.padded.oodle не найден.");
                    return false;
                }

                EnsureWritable(pakPath);
                using (FileStream fs = new FileStream(pakPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    if (fs.Length < PAK_HOOK_OFFSET + PAK_HOOK_SIZE)
                    {
                        Log("Ошибка: pakchunk0-Windows.pak имеет неподдерживаемый размер.");
                        return false;
                    }

                    fs.Seek(PAK_HOOK_OFFSET, SeekOrigin.Begin);
                    byte[] currentBytes = new byte[PAK_HOOK_SIZE];
                    fs.Read(currentBytes, 0, PAK_HOOK_SIZE);

                    string currentSha = ComputeSha256(currentBytes);
                    if (currentSha.Equals(PAK_HOOK_SHA256, StringComparison.OrdinalIgnoreCase))
                    {
                        Log("Хук загрузчика в pakchunk0-Windows.pak уже активен.");
                    }
                    else
                    {
                        string backupBlock = Path.Combine(gamePath, "Content", "Paks", "pakchunk0-Windows.pak.orig_block");
                        if (!File.Exists(backupBlock))
                        {
                            File.WriteAllBytes(backupBlock, currentBytes);
                            Log("Создана резервная копия оригинального блока игры: pakchunk0-Windows.pak.orig_block");
                        }

                        fs.Seek(PAK_HOOK_OFFSET, SeekOrigin.Begin);
                        fs.Write(hookPayload, 0, PAK_HOOK_SIZE);
                        fs.Flush();
                        Log("✔ Нативный хук успешно внедрён в pakchunk0-Windows.pak! (Автономный режим активен)");
                    }
                }

                string looseHook = Path.Combine(gamePath, "LaunchInstance.native-bridge.padded.oodle");
                if (File.Exists(looseHook))
                {
                    try { File.Delete(looseHook); } catch { }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log("Ошибка при настройке pakchunk0-Windows.pak: " + ex.Message);
                return false;
            }
        }

        private bool RestorePakLaunchHook(string gamePath)
        {
            try
            {
                string pakPath = Path.Combine(gamePath, "Content", "Paks", "pakchunk0-Windows.pak");
                string backupBlock = Path.Combine(gamePath, "Content", "Paks", "pakchunk0-Windows.pak.orig_block");
                if (File.Exists(backupBlock) && File.Exists(pakPath))
                {
                    byte[] origBytes = File.ReadAllBytes(backupBlock);
                    if (origBytes.Length == PAK_HOOK_SIZE)
                    {
                        EnsureWritable(pakPath);
                        using (FileStream fs = new FileStream(pakPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            fs.Seek(PAK_HOOK_OFFSET, SeekOrigin.Begin);
                            fs.Write(origBytes, 0, PAK_HOOK_SIZE);
                            fs.Flush();
                        }
                        File.Delete(backupBlock);
                        Log("Оригинальный блок pakchunk0-Windows.pak успешно восстановлен!");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Ошибка восстановления блока pakchunk0-Windows.pak: " + ex.Message);
            }
            return false;
        }

        private static bool IsAdministrator()
        {
            try
            {
                using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
                {
                    var principal = new System.Security.Principal.WindowsPrincipal(identity);
                    return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                return false;
            }
        }

        private static void EnsureWritable(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var attrs = File.GetAttributes(filePath);
                    if ((attrs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        File.SetAttributes(filePath, attrs & ~FileAttributes.ReadOnly);
                    }
                }
            }
            catch { }
        }

        [STAThread]
        public static void Main()
        {
            if (!IsAdministrator())
            {
                try
                {
                    ProcessStartInfo proc = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        FileName = Application.ExecutablePath,
                        Verb = "runas"
                    };
                    Process.Start(proc);
                    return;
                }
                catch
                {
                    // Пользователь отклонил запрос UAC — запускаем в обычном режиме
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
