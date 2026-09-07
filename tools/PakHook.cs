using System;
using System.IO;

namespace LotmPakHook
{
    class Program
    {
        // SHA256: c031726986e09358bb18ff8a2b8ee5f0b4e65ce8ae8331eed2d7575c80b7efa9
        // 4,660 bytes native bridge payload for pakchunk0-Windows.pak
        private const long PAK_HOOK_OFFSET = 427225161L;
        private const int PAK_HOOK_SIZE = 4660;

        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: PakHook.exe <install|restore> <GamePath>");
                return 1;
            }

            string action = args[0].ToLowerInvariant();
            string gamePath = args[1].Trim('"');

            // Handle possible nested paths
            if (!gamePath.EndsWith("C7", StringComparison.OrdinalIgnoreCase))
            {
                if (Directory.Exists(Path.Combine(gamePath, "C7")))
                    gamePath = Path.Combine(gamePath, "C7");
                else if (Directory.Exists(Path.Combine(gamePath, "Game", "C7")))
                    gamePath = Path.Combine(gamePath, "Game", "C7");
            }

            string pakPath = Path.Combine(gamePath, "Content", "Paks", "pakchunk0-Windows.pak");
            string bakPath = Path.Combine(gamePath, "Content", "Paks", "pakchunk0-Windows.pak.orig_block");

            if (!File.Exists(pakPath))
            {
                Console.WriteLine("[!] Ошибка: pakchunk0-Windows.pak не найден в " + pakPath);
                return 2;
            }

            // Load hook payload from embedded base64 or loose file
            byte[] hookBytes = GetHookPayload(gamePath);
            if (hookBytes == null || hookBytes.Length != PAK_HOOK_SIZE)
            {
                Console.WriteLine("[!] Ошибка: Не удалось загрузить байты хука (размер должен быть 4660 байт).");
                return 3;
            }

            EnsureWritable(pakPath);

            if (action == "install")
            {
                return InstallHook(pakPath, bakPath, hookBytes);
            }
            else if (action == "restore")
            {
                return RestoreHook(pakPath, bakPath);
            }
            else
            {
                Console.WriteLine("[!] Неизвестное действие: " + action);
                return 1;
            }
        }

        private static void EnsureWritable(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    FileAttributes attr = File.GetAttributes(file);
                    if ((attr & FileAttributes.ReadOnly) != 0)
                    {
                        File.SetAttributes(file, attr & ~FileAttributes.ReadOnly);
                    }
                }
            }
            catch { }
        }

        private static byte[] GetHookPayload(string gamePath)
        {
            // 1. Check base64 embedded payload
            if (!string.IsNullOrEmpty(EmbeddedPayloadBase64))
            {
                try
                {
                    byte[] b = Convert.FromBase64String(EmbeddedPayloadBase64);
                    if (b.Length == PAK_HOOK_SIZE) return b;
                }
                catch { }
            }

            // 2. Check loose candidates
            string[] candidates = new string[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LaunchInstance.native-bridge.padded.oodle"),
                Path.Combine(gamePath, "LaunchInstance.native-bridge.padded.oodle"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "LaunchInstance.native-bridge.padded.oodle")
            };
            foreach (var c in candidates)
            {
                if (File.Exists(c) && new FileInfo(c).Length == PAK_HOOK_SIZE)
                {
                    return File.ReadAllBytes(c);
                }
            }
            return null;
        }

        private static int InstallHook(string pakPath, string bakPath, byte[] hookBytes)
        {
            try
            {
                using (var fs = new FileStream(pakPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    if (fs.Length < PAK_HOOK_OFFSET + PAK_HOOK_SIZE)
                    {
                        Console.WriteLine("[!] Ошибка: pakchunk0-Windows.pak имеет неподдерживаемый размер (" + fs.Length + " байт).");
                        return 4;
                    }

                    fs.Seek(PAK_HOOK_OFFSET, SeekOrigin.Begin);
                    byte[] cur = new byte[PAK_HOOK_SIZE];
                    fs.Read(cur, 0, PAK_HOOK_SIZE);

                    bool alreadyActive = true;
                    for (int i = 0; i < PAK_HOOK_SIZE; i++)
                    {
                        if (cur[i] != hookBytes[i]) { alreadyActive = false; break; }
                    }

                    if (alreadyActive)
                    {
                        Console.WriteLine("      ✔ Нативный хук загрузчика уже активен.");
                        return 0;
                    }

                    if (!File.Exists(bakPath))
                    {
                        EnsureWritable(bakPath);
                        File.WriteAllBytes(bakPath, cur);
                        Console.WriteLine("      [*] Создана резервная копия оригинального блока игры: pakchunk0-Windows.pak.orig_block");
                    }

                    fs.Seek(PAK_HOOK_OFFSET, SeekOrigin.Begin);
                    fs.Write(hookBytes, 0, PAK_HOOK_SIZE);
                    fs.Flush();
                    Console.WriteLine("      ✔ Нативный хук загрузчика успешно активирован!");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] Ошибка активации хука: " + ex.Message);
                return 5;
            }
        }

        private static int RestoreHook(string pakPath, string bakPath)
        {
            try
            {
                if (!File.Exists(bakPath))
                {
                    Console.WriteLine("[*] Резервная копия блока не найдена или уже была восстановлена.");
                    return 0;
                }

                byte[] origBytes = File.ReadAllBytes(bakPath);
                if (origBytes.Length != PAK_HOOK_SIZE)
                {
                    Console.WriteLine("[!] Ошибка: Неверный размер файла резервной копии.");
                    return 6;
                }

                using (var fs = new FileStream(pakPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    fs.Seek(PAK_HOOK_OFFSET, SeekOrigin.Begin);
                    fs.Write(origBytes, 0, PAK_HOOK_SIZE);
                    fs.Flush();
                }

                try { File.Delete(bakPath); } catch { }
                Console.WriteLine("      ✔ Оригинальный блок pakchunk0-Windows.pak успешно восстановлен.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[!] Ошибка восстановления: " + ex.Message);
                return 7;
            }
        }

        // Will be populated with Convert.ToBase64String(File.ReadAllBytes("data/LaunchInstance.native-bridge.padded.oodle"))
        public static string EmbeddedPayloadBase64 = "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";
    }
}
