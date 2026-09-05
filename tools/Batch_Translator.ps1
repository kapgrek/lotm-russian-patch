param (
    [int]$BatchCount = 20
)

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$gameMods = "D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes"
$geminiFile = "$gameMods\RuntimeTextGemini.lua"
$russianFile = "$gameMods\RuntimeTextRussian.lua"

Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host "   Lord of the Mysteries - Пакетный Переводчик Текста" -ForegroundColor Yellow
Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host "Размер пакета: $BatchCount строк" -ForegroundColor Gray

# Загружаем уже существующие русские переводы
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
                    if ($v -notmatch "QUERY LENGTH LIMIT") {
                        $existingRu[$k] = $v
                    }
                }
            }
        }
    }
}
Write-Host "Уже в словаре: $($existingRu.Count) ключей" -ForegroundColor Green

function Clean-LuaString([string]$s) {
    if ([string]::IsNullOrWhiteSpace($s)) { return "" }
    # Убираем пробелы после \n, которые могли привести к \ 
    $res = $s -replace '\\\s+', '\n'
    # Экранируем кавычки
    $res = $res.Replace('"', '\"')
    return $res
}

function Translate-Chunk([string]$segment) {
    if ([string]::IsNullOrWhiteSpace($segment)) { return $segment }
    try {
        $url = "https://api.mymemory.translated.net/get?q=" + [System.Uri]::EscapeDataString($segment) + "&langpair=en|ru"
        $wc = New-Object System.Net.WebClient
        $wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)")
        $wc.Encoding = [System.Text.Encoding]::UTF8
        $json = $wc.DownloadString($url)
        $obj = $json | ConvertFrom-Json
        if ($obj -and $obj.responseData -and $obj.responseData.translatedText) {
            $t = $obj.responseData.translatedText
            if ($t -match "QUERY LENGTH LIMIT") { return $null }
            return $t
        }
    } catch { }
    return $null
}

function Translate-EnToRu([string]$text) {
    if ([string]::IsNullOrWhiteSpace($text)) { return $text }

    $clean = $text.Replace('\"', '"')

    # Защищаем игровые теги
    $tagIndex = 0
    $tagMap = @{}
    $protected = [System.Text.RegularExpressions.Regex]::Replace($clean, "<[^>]+>|\{\{[^}]+\}\}", {
        param($m)
        $placeholder = "__TAG_$tagIndex`__"
        $tagMap[$placeholder] = $m.Value
        $script:tagIndex++
        return $placeholder
    })

    $result = ""
    if ($protected.Length -le 400) {
        $result = Translate-Chunk $protected
    } else {
        $parts = $protected -split "(\r?\n|\. )"
        $buffer = ""
        foreach ($p in $parts) {
            if (($buffer.Length + $p.Length) -gt 380) {
                $tr = Translate-Chunk $buffer
                $result += if ($tr) { $tr } else { $buffer }
                $buffer = $p
            } else {
                $buffer += $p
            }
        }
        if ($buffer.Length -gt 0) {
            $tr = Translate-Chunk $buffer
            $result += if ($tr) { $tr } else { $buffer }
        }
    }

    if ($result) {
        foreach ($k in $tagMap.Keys) {
            $result = $result.Replace($k, $tagMap[$k])
        }
        return Clean-LuaString $result
    }
    return $null
}

# Поиск непереведенных строк
Write-Host "Поиск кандидатов на перевод..." -ForegroundColor Gray
$translatedCount = 0
$reader = New-Object System.IO.StreamReader($geminiFile, [System.Text.Encoding]::UTF8)

$candidates = @()
while (($line = $reader.ReadLine()) -ne $null -and $candidates.Count -lt $BatchCount) {
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

Write-Host "Кандидатов для перевода: $($candidates.Count)" -ForegroundColor Yellow

$i = 0
foreach ($item in $candidates) {
    $i++
    $preview = $item.English.Substring(0, [Math]::Min(35, $item.English.Length)).Replace("`n", " ")
    Write-Host ("[{0}/{1}] {2}..." -f $i, $candidates.Count, $preview) -NoNewline
    $ru = Translate-EnToRu $item.English
    if ($ru) {
        # Добавляем оба ключа: китайский и английский
        $existingRu[$item.Chinese] = $ru
        $existingRu[$item.English] = $ru
        $translatedCount++
        $ruPreview = $ru.Substring(0, [Math]::Min(35, $ru.Length)).Replace("`n", " ")
        Write-Host (" -> {0}..." -f $ruPreview) -ForegroundColor Green
    } else {
        Write-Host " -> Пропущено" -ForegroundColor Red
    }
    Start-Sleep -Milliseconds 250
}

# Сохраняем обновленный файл
Write-Host "`nСохранение RuntimeTextRussian.lua..." -ForegroundColor Gray
$writer = New-Object System.IO.StreamWriter($russianFile, $false, (New-Object System.Text.UTF8Encoding($false)))
$writer.WriteLine("-- Generated Russian Translation Dictionary for Lord of Mysteries")
$writer.WriteLine("-- Entries: " + $existingRu.Count)
$writer.WriteLine("return {")
foreach ($k in $existingRu.Keys) {
    $cleanKey = Clean-LuaString $k
    $cleanVal = Clean-LuaString $existingRu[$k]
    $writer.WriteLine(("    [`"{0}`"] = `"{1}`"," -f $cleanKey, $cleanVal))
}
$writer.WriteLine("}")
$writer.Close()

# Синхронизируем копию в папку загрузок пользователя
Copy-Item $russianFile "C:\Users\yapug\Downloads\lotm translate\RuntimeTextRussian.lua" -Force -ErrorAction SilentlyContinue

Write-Host "Готово! Добавлено: $translatedCount строк. Всего в словаре: $($existingRu.Count) записей." -ForegroundColor Green
