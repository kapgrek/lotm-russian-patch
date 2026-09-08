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
        private const string GITHUB_DIRECT_DOWNLOAD_URL = "https://github.com/" + GITHUB_REPO + "/releases/latest/download/lom-russian-patch-data.zip";
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

                    // 1. Попытка прямой загрузки актуального пакета с GitHub Releases
                    bool downloadedFromGitHub = false;
                    string tempZip = Path.Combine(Path.GetTempPath(), "lom-russian-patch-data.zip");

                    try
                    {
                        Log("Загрузка актуального пакета данных с GitHub...");
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        using (WebClient wc = new WebClient())
                        {
                            wc.Headers.Add("User-Agent", "Lotm-Russian-Patcher");
                            if (File.Exists(tempZip)) File.Delete(tempZip);
                            wc.DownloadFile(GITHUB_DIRECT_DOWNLOAD_URL, tempZip);

                            FileInfo fi = new FileInfo(tempZip);
                            if (fi.Exists && fi.Length > 5 * 1024 * 1024)
                            {
                                downloadedFromGitHub = true;
                                Log(string.Format("Пакет успешно загружен ({0:F1} МБ).", fi.Length / (1024.0 * 1024.0)));
                            }
                            else
                            {
                                if (File.Exists(tempZip)) File.Delete(tempZip);
                                Log("Прямая загрузка вернула некорректный размер, пробуем API...");
                            }
                        }
                    }
                    catch (Exception exDirect)
                    {
                        Log("Прямая загрузка недоступна (" + exDirect.Message + "), опрос GitHub API...");
                    }

                    // 2. Резерв: если прямой URL не сработал, опрашиваем GitHub API
                    if (!downloadedFromGitHub)
                    {
                        try
                        {
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            using (WebClient wc = new WebClient())
                            {
                                wc.Headers.Add("User-Agent", "Lotm-Russian-Patcher");
                                string json = wc.DownloadString(GITHUB_API_URL);

                                string assetName = "lom-russian-patch-data.zip";
                                string targetKey = "\"browser_download_url\":";
                                string downloadUrl = null;
                                int bdlIdx = 0;
                                while ((bdlIdx = json.IndexOf(targetKey, bdlIdx)) != -1)
                                {
                                    int urlStart = json.IndexOf("https://", bdlIdx);
                                    int urlEnd = json.IndexOf("\"", urlStart);
                                    if (urlStart > 0 && urlEnd > urlStart)
                                    {
                                        string url = json.Substring(urlStart, urlEnd - urlStart);
                                        if (url.EndsWith("/" + assetName, StringComparison.OrdinalIgnoreCase) || url.IndexOf(assetName, StringComparison.OrdinalIgnoreCase) != -1)
                                        {
                                            downloadUrl = url;
                                            break;
                                        }
                                    }
                                    bdlIdx += targetKey.Length;
                                }

                                if (!string.IsNullOrEmpty(downloadUrl))
                                {
                                    Log("Скачивание пакета через API URL: " + downloadUrl);
                                    if (File.Exists(tempZip)) File.Delete(tempZip);
                                    wc.DownloadFile(downloadUrl, tempZip);

                                    FileInfo fi = new FileInfo(tempZip);
                                    if (fi.Exists && fi.Length > 5 * 1024 * 1024)
                                    {
                                        downloadedFromGitHub = true;
                                        Log(string.Format("Пакет успешно загружен через API ({0:F1} МБ).", fi.Length / (1024.0 * 1024.0)));
                                    }
                                }
                            }
                        }
                        catch (Exception exApi)
                        {
                            Log("Загрузка через GitHub API не удалась: " + exApi.Message);
                        }
                    }

                    // Распаковка скачанного с GitHub архива
                    if (downloadedFromGitHub)
                    {
                        try
                        {
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
                            Log("Файлы русификатора успешно распакованы в игру!");
                        }
                        catch (Exception exZip)
                        {
                            downloadedFromGitHub = false;
                            Log("Ошибка распаковки архива: " + exZip.Message);
                        }
                    }

                    bool installedLocally = false;

                    // 3. Проверка локального архива пакета данных (если запуск без интернета)
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
                                installedLocally = true;
                                Log("Локальный пакет данных успешно распакован!");
                            }
                            catch (Exception ex)
                            {
                                Log("Ошибка распаковки локального архива: " + ex.Message);
                            }
                        }
                    }

                    // 4. Если архив не найден, копируем отдельные локальные файлы мода
                    if (!downloadedFromGitHub && !installedLocally)
                    {
                        string localSource = AppDomain.CurrentDomain.BaseDirectory;
                        string[] filesToCopy = new string[] { "RussianLocalization.lua", "RuntimeTextRussian.lua", "Init.lua", "bootstrap.lua", "manifest.lua", "translation-overrides.lua", "CPDDTranslation.lua", "EnglishToRussian.lua" };
                        int copiedCount = 0;
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
                                copiedCount++;
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
                                copiedCount++;
                            }
                            Log("Скопированы локальные шарды базы перевода.");
                        }

                        if (copiedCount > 0)
                        {
                            installedLocally = true;
                        }
                    }

                    // 5. Проверка: были ли вообще установлены файлы
                    if (!downloadedFromGitHub && !installedLocally)
                    {
                        Log("ОШИБКА: Не удалось загрузить файлы с GitHub и не найдены локальные файлы русификатора!");
                        MessageBox.Show(
                            "Не удалось загрузить файлы русификатора с GitHub, и локальные файлы не найдены рядом с установщиком.\n\n" +
                            "Пожалуйста, проверьте подключение к интернету или скачайте архив вручную из релизов GitHub.",
                            "Ошибка установки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 6. Проверка и гарантия наличия хука в CPDDTranslation.lua
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
                        else
                        {
                            string cpddContent = "local original = require(\"Launch.Base.LaunchStringExt\")\r\n\r\nlocal File = import(\"LuaFunctionLibrary\")\r\nlocal path = File.GetFilePath(import(\"BlueprintPathsLibrary\").ProjectSavedDir()) .. \"/Mods/bootstrap.lua\"\r\nlocal source = File.LoadFile(path)\r\nLaunchLog.Info(\"[LOMModLoader] bootstrap path=\" .. path .. \" bytes=\" .. tostring(source and #source or 0))\r\nif source and source ~= \"\" then\r\n    local chunk, message = load(source, \"@\" .. path)\r\n    if chunk then xpcall(chunk, LaunchLog.Error) else LaunchLog.Error(message) end\r\nend\r\n\r\nreturn original\r\n";
                            EnsureWritable(cpddLua);
                            File.WriteAllText(cpddLua, cpddContent, System.Text.Encoding.UTF8);
                            Log("Создан файл загрузчика CPDDTranslation.lua в Binaries");
                        }
                    }
                    else
                    {
                        Log("Хук загрузчика проверен в CPDDTranslation.lua");
                    }

                    
// 4. Проверка и установка нативного хука в pakchunk0-Windows.pak
                    PatchPakLaunchHook(gamePath);

                    // 5. Установка нормализованного шрифта Aleo_TitleNew.ttf
                    InstallPatchedFont(gamePath);

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
            RestoreOriginalFont(gamePath);
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
        private const string EmbeddedPakHookBase64 = "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";

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

                // 1. Встроенный Base64 пейлоад (гарантирует автономную работу без внешних файлов)
                if (!string.IsNullOrEmpty(EmbeddedPakHookBase64))
                {
                    try
                    {
                        byte[] b = Convert.FromBase64String(EmbeddedPakHookBase64);
                        if (b.Length == PAK_HOOK_SIZE) hookPayload = b;
                    }
                    catch { }
                }

                // 2. Резервный поиск внешнего файла хука
                if (hookPayload == null)
                {
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

                    bool alreadyActive = true;
                    for (int i = 0; i < PAK_HOOK_SIZE; i++)
                    {
                        if (currentBytes[i] != hookPayload[i]) { alreadyActive = false; break; }
                    }

                    if (alreadyActive)
                    {
                        Log("Хук загрузчика в pakchunk0-Windows.pak уже активен.");
                    }
                    else
                    {
                        string backupBlock = Path.Combine(gamePath, "Content", "Paks", "pakchunk0-Windows.pak.orig_block");
                        if (!File.Exists(backupBlock))
                        {
                            EnsureWritable(backupBlock);
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

        private bool InstallPatchedFont(string gamePath)
        {
            try
            {
                string fontDir = Path.Combine(gamePath, "Binaries", "Win64", "allin_data", "font");
                string targetFont = Path.Combine(fontDir, "Aleo_TitleNew.ttf");
                string backupFont = Path.Combine(fontDir, "Aleo_TitleNew.ttf.orig_bak");

                // 1. Резервное копирование оригинального шрифта
                if (File.Exists(targetFont) && !File.Exists(backupFont))
                {
                    EnsureWritable(backupFont);
                    File.Copy(targetFont, backupFont, true);
                    Log("Создана резервная копия оригинального шрифта: Aleo_TitleNew.ttf.orig_bak");
                }

                // 2. Поиск пропатченного шрифта из пакета мода
                string patchedFont = null;
                string[] candidates = new string[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "font", "Aleo_TitleNew.ttf"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "font", "Aleo_TitleNew.ttf"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Binaries", "Win64", "allin_data", "font", "Aleo_TitleNew.ttf"),
                    Path.Combine(gamePath, "font", "Aleo_TitleNew.ttf"),
                    Path.Combine(gamePath, "Binaries", "Win64", "allin_data", "font", "Aleo_TitleNew.ttf")
                };
                foreach (var p in candidates)
                {
                    if (File.Exists(p) && p != targetFont)
                    {
                        patchedFont = p;
                        break;
                    }
                }

                if (patchedFont != null)
                {
                    if (!Directory.Exists(fontDir)) Directory.CreateDirectory(fontDir);
                    EnsureWritable(targetFont);
                    File.Copy(patchedFont, targetFont, true);
                    Log("✔ Нормализованный шрифт Aleo_TitleNew.ttf успешно установлен в allin_data\\font!");
                    return true;
                }
                else
                {
                    Log("Внимание: Пропатченный шрифт Aleo_TitleNew.ttf не найден в пакете установщика.");
                }
            }
            catch (Exception ex)
            {
                Log("Ошибка установки шрифта: " + ex.Message);
            }
            return false;
        }

        private bool RestoreOriginalFont(string gamePath)
        {
            try
            {
                string fontDir = Path.Combine(gamePath, "Binaries", "Win64", "allin_data", "font");
                string targetFont = Path.Combine(fontDir, "Aleo_TitleNew.ttf");
                string backupFont = Path.Combine(fontDir, "Aleo_TitleNew.ttf.orig_bak");

                if (File.Exists(backupFont))
                {
                    EnsureWritable(targetFont);
                    File.Copy(backupFont, targetFont, true);
                    File.Delete(backupFont);
                    Log("Оригинальный шрифт Aleo_TitleNew.ttf успешно восстановлен из бэкапа!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log("Ошибка восстановления оригинального шрифта: " + ex.Message);
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
