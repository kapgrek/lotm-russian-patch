param (
    [int]$BatchCount = 100
)

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$gameMods = "D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes"
$geminiFile = "$gameMods\RuntimeTextGemini.lua"
$russianFile = "$gameMods\RuntimeTextRussian.lua"

Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host "   Lord of the Mysteries - Пакетный Переводчик Текста" -ForegroundColor Yellow
Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host "Пакет для перевода: $BatchCount строк" -ForegroundColor Gray

# Загружаем уже существующие русские переводы
$existingRu = @{}
if (Test-Path $russianFile) {
    Write-Host "Чтение существующего RuntimeTextRussian.lua..." -ForegroundColor Gray
    $lines = [System.IO.File]::ReadAllLines($russianFile, [System.Text.Encoding]::UTF8)
    foreach ($l in $lines) {
        $l = $l.Trim()
        if ($l.StartsWith('["') -and ($l.EndsWith('",') -or $l.EndsWith('"'))) {
            $delim = $l.IndexOf('"] = "')
            if ($delim -gt 0) {
                $k = $l.Substring(2, $delim - 2)
                $valStart = $delim + 6
                $valEnd = if ($l.EndsWith('",')) { $l.Length - 2 } else { $l.Length - 1 }
                if ($valEnd -ge $valStart) {
                    $v = $l.Substring($valStart, $valEnd - $valStart)
                    $existingRu[$k] = $v
                }
            }
        }
    }
}
Write-Host "Уже переведено: $($existingRu.Count) строк" -ForegroundColor Green

# Функция онлайн-перевода с сохранением тегов
function Translate-EnToRu([string]$text) {
    if ([string]::IsNullOrWhiteSpace($text)) { return $text }

    # Сохраняем теги разметки <...> и {{...}}
    $tagIndex = 0
    $tagMap = @{}
    $protected = [System.Text.RegularExpressions.Regex]::Replace($text, "<[^>]+>|\{\{[^}]+\}\}", {
        param($m)
        $placeholder = "__TAG_$tagIndex`__"
        $tagMap[$placeholder] = $m.Value
        $script:tagIndex++
        return $placeholder
    })

    try {
        $url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=ru&dt=t&q=" + [System.Uri]::EscapeDataString($protected)
        $client = New-Object System.Net.WebClient
        $client.Headers.Add("User-Agent", "Mozilla/5.0")
        $client.Encoding = [System.Text.Encoding]::UTF8
        $response = $client.DownloadString($url)

        # Парсим JSON-массив google translate
        $json = $response | ConvertFrom-Json
        $result = ""
        foreach ($part in $json[0]) {
            if ($part[0]) { $result += $part[0] }
        }

        # Восстанавливаем теги
        foreach ($k in $tagMap.Keys) {
            $result = $result.Replace($k, $tagMap[$k])
        }

        # Экранируем кавычки для Lua
        $result = $result.Replace('"', '\"')
        return $result
    } catch {
        return $null
    }
}

# Ищем непереведенные строки в RuntimeTextGemini.lua
Write-Host "Поиск кандидатов на перевод..." -ForegroundColor Gray
$translatedCount = 0
$reader = New-Object System.IO.StreamReader($geminiFile, [System.Text.Encoding]::UTF8)

$candidates = @()
while (($line = $reader.ReadLine()) -ne $null -and $candidates.Count -lt $BatchCount) {
    $line = $line.Trim()
    if ($line.StartsWith('["') -and ($line.EndsWith('",') -or $line.EndsWith('"'))) {
        $delim = $line.IndexOf('"] = "')
        if ($delim -gt 0) {
            $k = $line.Substring(2, $delim - 2)
            if (-not $existingRu.ContainsKey($k)) {
                $valStart = $delim + 6
                $valEnd = if ($line.EndsWith('",')) { $line.Length - 2 } else { $line.Length - 1 }
                if ($valEnd -ge $valStart) {
                    $en = $line.Substring($valStart, $valEnd - $valStart)
                    $candidates += @{ Key = $k; English = $en }
                }
            }
        }
    }
}
$reader.Close()

Write-Host "Найдено кандидатов в этом пакете: $($candidates.Count)" -ForegroundColor Yellow

$i = 0
foreach ($item in $candidates) {
    $i++
    Write-Host ("[{0}/{1}] Перевод: {2}" -f $i, $candidates.Count, $item.English.Substring(0, [Math]::Min(50, $item.English.Length))) -NoNewline
    $ru = Translate-EnToRu $item.English
    if ($ru) {
        $existingRu[$item.Key] = $ru
        $translatedCount++
        Write-Host " -> OK" -ForegroundColor Green
    } else {
        Write-Host " -> Пропущено (ошибка сети)" -ForegroundColor Red
    }
    Start-Sleep -Milliseconds 100
}

# Перезаписываем RuntimeTextRussian.lua
Write-Host "`nСохранение обновленного словаря RuntimeTextRussian.lua..." -ForegroundColor Gray
$writer = New-Object System.IO.StreamWriter($russianFile, $false, (New-Object System.Text.UTF8Encoding($false)))
$writer.WriteLine("-- Generated Russian Translation Dictionary for Lord of Mysteries")
$writer.WriteLine("-- Entries: " + $existingRu.Count)
$writer.WriteLine("return {")
foreach ($k in $existingRu.Keys) {
    $writer.WriteLine(("    [`"{0}`"] = `"{1}`"," -f $k, $existingRu[$k]))
}
$writer.WriteLine("}")
$writer.Close()

Write-Host "Готово! Успешно переведено и добавлено: $translatedCount строк. Всего в словаре: $($existingRu.Count)." -ForegroundColor Green
