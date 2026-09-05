param (
    [int]$Count = 500
)

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$gameMods = "D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes"
$geminiFile = "$gameMods\RuntimeTextGemini.lua"
$russianFile = "$gameMods\RuntimeTextRussian.lua"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "   Lord of the Mysteries — Высокоскоростной Переводчик    " -ForegroundColor Yellow
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "Запрошено строк: $Count" -ForegroundColor Gray

# Каноничные термины для принудительной замены
$canonMap = @{
    "The Fool" = "Шут"
    "Seer" = "Провидец"
    "Clown" = "Клоун"
    "Magician" = "Фокусник"
    "Faceless" = "Безликий"
    "Marionettist" = "Марионеточник"
    "Bizarro Sorcerer" = "Ловкий Маг"
    "Scholar of Yore" = "Учёный Прошлого"
    "Miracle Invoker" = "Творец Чудес"
    "Attendant of Mysteries" = "Слуга Тайн"
    "Lord of Mysteries" = "Повелитель Тайн"
    "Beyonder" = "Потусторонний"
    "Beyonders" = "Потусторонние"
    "Spirit Body Threads" = "Нити духовного тела"
    "Spirit Body" = "Духовное тело"
    "Sealed Artifact" = "Запечатанный артефакт"
    "Tarot Club" = "Клуб Таро"
    "Nighthawks" = "Ночные Ястребы"
    "Tingen" = "Тинген"
    "Backlund" = "Бэкланд"
}

# 1. Загрузка уже имеющихся строк
$existingRu = @{}
if (Test-Path $russianFile) {
    $lines = [System.IO.File]::ReadAllLines($russianFile, [System.Text.Encoding]::UTF8)
    foreach ($l in $lines) {
        $trimmed = $l.Trim()
        if ($trimmed.StartsWith('["') -and ($trimmed.EndsWith('",') -or $trimmed.EndsWith('"'))) {
            $delim = $trimmed.IndexOf('"] = "')
            if ($delim -gt 0) {
                $k = $trimmed.Substring(2, $delim - 2)
                $valStart = $delim + 6
                $valEnd = if ($trimmed.EndsWith('",')) { $trimmed.Length - 2 } else { $trimmed.Length - 1 }
                if ($valEnd -ge $valStart) {
                    $v = $trimmed.Substring($valStart, $valEnd - $valStart)
                    $existingRu[$k] = $v
                }
            }
        }
    }
}
Write-Host "Текущий размер словаря: $($existingRu.Count) ключей" -ForegroundColor Green

# 2. Выбор кандидатов
Write-Host "Поиск кандидатов на перевод..." -ForegroundColor Gray
$candidates = @()
$reader = New-Object System.IO.StreamReader($geminiFile, [System.Text.Encoding]::UTF8)
while (($line = $reader.ReadLine()) -ne $null -and $candidates.Count -lt $Count) {
    $trimmed = $line.Trim()
    if ($trimmed.StartsWith('["') -and ($trimmed.EndsWith('",') -or $trimmed.EndsWith('"'))) {
        $delim = $trimmed.IndexOf('"] = "')
        if ($delim -gt 0) {
            $cnKey = $trimmed.Substring(2, $delim - 2)
            $valStart = $delim + 6
            $valEnd = if ($trimmed.EndsWith('",')) { $trimmed.Length - 2 } else { $trimmed.Length - 1 }
            if ($valEnd -ge $valStart) {
                $enVal = $trimmed.Substring($valStart, $valEnd - $valStart)
                if (-not $existingRu.ContainsKey($cnKey) -and -not $existingRu.ContainsKey($enVal) -and $enVal.Length -gt 1) {
                    $candidates += @{ Chinese = $cnKey; English = $enVal }
                }
            }
        }
    }
}
$reader.Close()

Write-Host "Отобрано кандидатов: $($candidates.Count)" -ForegroundColor Yellow
if ($candidates.Count -eq 0) {
    Write-Host "Все запрошенные строки уже переведены!" -ForegroundColor Green
    return
}

function Clean-LuaString([string]$s) {
    if ([string]::IsNullOrWhiteSpace($s)) { return "" }
    $res = $s -replace '\\\s+', '\n'
    $res = $res.Replace('"', '\"')
    return $res
}

function Translate-BatchList($items) {
    # Собираем строки с разделителем уникального маркера
    $sep = " [SEP] "
    $joined = ($items | ForEach-Object { $_.English }) -join $sep
    if ([string]::IsNullOrWhiteSpace($joined)) { return $null }

    try {
        $url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=ru&dt=t&q=" + [System.Uri]::EscapeDataString($joined)
        $wc = New-Object System.Net.WebClient
        $wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)")
        $wc.Encoding = [System.Text.Encoding]::UTF8
        $json = $wc.DownloadString($url)
        
        # Парсим ответ Google Translate
        # Ответ имеет вид: [[[tr1, ...], [tr2, ...], ...], ...]
        $raw = ""
        $arr = $json | ConvertFrom-Json
        foreach ($sub in $arr[0]) {
            if ($sub -and $sub.Length -gt 0) {
                $raw += $sub[0]
            }
        }
        
        # Разбиваем по маркеру
        $parts = $raw -split "\[SEP\]|\[sep\]"
        if ($parts.Count -eq $items.Count) {
            return $parts
        }
    } catch { }

    # Fallback: поштучный перевод если пакет не сошёлся по числу разделителей
    $singleResults = @()
    foreach ($item in $items) {
        try {
            $u = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=ru&dt=t&q=" + [System.Uri]::EscapeDataString($item.English)
            $wc2 = New-Object System.Net.WebClient
            $wc2.Headers.Add("User-Agent", "Mozilla/5.0")
            $wc2.Encoding = [System.Text.Encoding]::UTF8
            $j = $wc2.DownloadString($u)
            $arr2 = $j | ConvertFrom-Json
            $s = ""
            foreach ($sub in $arr2[0]) { $s += $sub[0] }
            $singleResults += $s
        } catch {
            $singleResults += $item.English
        }
    }
    return $singleResults
}

# Обработка пачками по 25 строк
$chunkSize = 25
$totalBatches = [Math]::Ceiling($candidates.Count / $chunkSize)
$added = 0

for ($b = 0; $b -lt $totalBatches; $b++) {
    $start = $b * $chunkSize
    $len = [Math]::Min($chunkSize, $candidates.Count - $start)
    $batchItems = $candidates[$start..($start + $len - 1)]

    Write-Host ("Пакет [{0}/{1}] ({2} строк)..." -f ($b + 1), $totalBatches, $len) -NoNewline

    $translations = Translate-BatchList $batchItems
    if ($translations -and $translations.Count -eq $batchItems.Count) {
        for ($k = 0; $k -lt $batchItems.Count; $k++) {
            $tr = $translations[$k].Trim()
            
            # Применение канона
            foreach ($canonKey in $canonMap.Keys) {
                $tr = $tr -ireplace [regex]::Escape($canonKey), $canonMap[$canonKey]
            }

            $cleanTr = Clean-LuaString $tr
            $existingRu[$batchItems[$k].Chinese] = $cleanTr
            $existingRu[$batchItems[$k].English] = $cleanTr
            $added += 2
        }
        Write-Host " Готово" -ForegroundColor Green
    } else {
        Write-Host " Ошибка пакета" -ForegroundColor Red
    }
    Start-Sleep -Milliseconds 200
}

# Сохранение обновленного файла
Write-Host "`nСохранение обновленного RuntimeTextRussian.lua..." -ForegroundColor Gray
$writer = New-Object System.IO.StreamWriter($russianFile, $false, (New-Object System.Text.UTF8Encoding($false)))
$writer.WriteLine("-- Generated Russian Translation Dictionary for Lord of Mysteries")
$writer.WriteLine("-- Entries: " + $existingRu.Count)
$writer.WriteLine("return {")
foreach ($key in $existingRu.Keys) {
    $cKey = Clean-LuaString $key
    $cVal = Clean-LuaString $existingRu[$key]
    $writer.WriteLine(("    [`"{0}`"] = `"{1}`"," -f $cKey, $cVal))
}
$writer.WriteLine("}")
$writer.Close()

# Синхронизация в папку загрузок пользователя
Copy-Item $russianFile "C:\Users\yapug\Downloads\lotm translate\RuntimeTextRussian.lua" -Force -ErrorAction SilentlyContinue

Write-Host "УСПЕХ! Добавлено $added новых записей. Всего в словаре: $($existingRu.Count) ключей." -ForegroundColor Green
