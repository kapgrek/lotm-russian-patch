using System;
using System.IO;
using System.Text;

class Program
{
    static int Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string sourceInitPath = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\Init.lua";
        if (!File.Exists(sourceInitPath))
        {
            Console.WriteLine("Source Init.lua not found!");
            return 1;
        }

        string text = File.ReadAllText(sourceInitPath, Encoding.UTF8);

        // 1. Remove any old standalone runtimeFixes declaration (near line 940) to prevent duplicate locals
        string tOldRf = "CaptureDataAssignmentsEnabled = false,\n}\nlocal runtimeFixes = {}\n";
        if (text.Contains(tOldRf)) text = text.Replace(tOldRf, "CaptureDataAssignmentsEnabled = false,\n}\n-- runtimeFixes declared above\n");
        string tOldRfWin = "CaptureDataAssignmentsEnabled = false,\r\n}\r\nlocal runtimeFixes = {}\r\n";
        if (text.Contains(tOldRfWin)) text = text.Replace(tOldRfWin, "CaptureDataAssignmentsEnabled = false,\r\n}\r\n-- runtimeFixes declared above\r\n");

        // 2. Define runtimeFixes early and wrap RussianMod / EnglishToRussian load in do ... end
        string t1 = "    QuitGame = \"Exit\",\r\n}";
        if (!text.Contains(t1)) t1 = "    QuitGame = \"Exit\",\n}";
        if (text.Contains(t1) && !text.Contains("runtimeFixes.RussianMod"))
        {
            string r1 = t1 + "\r\n\r\n" +
@"local runtimeFixes = {}

do
    local okRussian, RussianMod = pcall(require, ""mods.cpdd_runtime_fixes.RussianLocalization"")
    if okRussian and type(RussianMod) == ""table"" and RussianMod.Enabled then
        runtimeFixes.RussianMod = RussianMod
        if RussianMod.stringConstOverrides then
            for k, v in pairs(RussianMod.stringConstOverrides) do stringConstOverrides[k] = v end
        end
        if RussianMod.englishToRussian then
            for k, v in pairs(RussianMod.englishToRussian) do visibleTextExactOverrides[k] = v end
        end
        if RussianMod.chineseToRussian then
            for k, v in pairs(RussianMod.chineseToRussian) do visibleTextExactOverrides[k] = v end
        end
        if RussianMod.visibleTextExactOverrides then
            for k, v in pairs(RussianMod.visibleTextExactOverrides) do visibleTextExactOverrides[k] = v end
        end
        if RussianMod.shortMenuLabels then
            for k, v in pairs(RussianMod.shortMenuLabels) do shortMenuLabels[k] = v end
        end
        if RussianMod.marionetteEnglishNames then
            for k, v in pairs(RussianMod.marionetteEnglishNames) do marionetteEnglishNames[k] = v end
        end
        if RussianMod.visibleTextReplacements then
            for _, rep in ipairs(RussianMod.visibleTextReplacements) do
                table.insert(visibleTextReplacements, 1, rep)
            end
        end
    end

    local okEng, EnglishMod = pcall(require, ""mods.cpdd_runtime_fixes.EnglishToRussian"")
    if okEng and type(EnglishMod) == ""table"" and type(EnglishMod.exact) == ""table"" then
        runtimeFixes.EnglishMod = EnglishMod
        for k, v in pairs(EnglishMod.exact) do
            if visibleTextExactOverrides[k] == nil then
                visibleTextExactOverrides[k] = v
            end
        end
    end
end";
            text = text.Replace(t1, r1);
        }

        // 3. adjustWidgetLetterSpacing attached to runtimeFixes
        string t2 = "Loader.Telemetry = Loader.Telemetry or {}\r\nLoader.Telemetry.Runtime = runtimeMetrics";
        if (!text.Contains(t2)) t2 = "Loader.Telemetry = Loader.Telemetry or {}\nLoader.Telemetry.Runtime = runtimeMetrics";
        if (text.Contains(t2) && !text.Contains("runtimeFixes.adjustWidgetLetterSpacing = function"))
        {
            string r2 = @"runtimeFixes.adjustWidgetLetterSpacing = function(widget, targetSize)
    if widget == nil then return end
    pcall(function()
        if widget.SetLetterSpacing ~= nil then
            widget:SetLetterSpacing(0)
        end
    end)
    pcall(function()
        local font = widget.Font or (widget.GetFont and widget:GetFont())
        if font ~= nil then
            font.LetterSpacing = -50
            if targetSize ~= nil and font.Size ~= nil and font.Size > targetSize then
                font.Size = targetSize
            end
            if widget.SetFont ~= nil then
                widget:SetFont(font)
            else
                widget.Font = font
            end
        end
    end)
end

" + t2;
            text = text.Replace(t2, r2);
        }

        // 4. lookupGeminiText hook
        string t3 = "local function lookupGeminiText(value)\r\n    if type(value) ~= \"string\" then return nil end";
        if (!text.Contains(t3)) t3 = "local function lookupGeminiText(value)\n    if type(value) ~= \"string\" then return nil end";
        if (text.Contains(t3))
        {
            string r3 = @"local function lookupGeminiText(value)
    local RussianMod = runtimeFixes.RussianMod
    if RussianMod and RussianMod.lookupRussianText then
        local ru = RussianMod.lookupRussianText(value)
        if ru ~= nil then
            return ru
        end
    end
    if type(value) ~= ""string"" then return nil end";
            text = text.Replace(t3, r3);
        }

        // 5. translateVisibleText hook
        string t4 = @"    local reviewedExact = visibleTextExactOverrides[value]
    if reviewedExact ~= nil then
        visibleTextCache[value] = reviewedExact
        return reviewedExact
    end";
        if (text.Contains(t4) && !text.Contains("local RussianMod = runtimeFixes.RussianMod\n    if RussianMod and RussianMod.lookupRussianText"))
        {
            string r4 = t4 + @"
    local RussianMod = runtimeFixes.RussianMod
    if RussianMod and RussianMod.lookupRussianText then
        local ru = RussianMod.lookupRussianText(value)
        if ru ~= nil then
            visibleTextCache[value] = ru
            return ru
        end
    end
    local EnglishMod = runtimeFixes.EnglishMod
    if EnglishMod and EnglishMod.translate then
        local ruEng = EnglishMod.translate(value)
        if ruEng ~= nil then
            visibleTextCache[value] = ruEng
            return ruEng
        end
    end";
            text = text.Replace(t4, r4);
        }

        // 6. Letter spacing in setSingleWidgetText
        text = text.Replace("adjustWidgetLetterSpacing(widget)", "runtimeFixes.adjustWidgetLetterSpacing(widget)");
        text = text.Replace("adjustWidgetLetterSpacing(textWidget, 13)", "runtimeFixes.adjustWidgetLetterSpacing(textWidget, 13)");

        // 7. repairLiveString hook
        string t6 = "repairLiveString = function(tableName, rowKey, fieldPath, value)\r\n    local enterWorldShortened = shortenEnterWorldLabel(value)";
        if (!text.Contains(t6)) t6 = "repairLiveString = function(tableName, rowKey, fieldPath, value)\n    local enterWorldShortened = shortenEnterWorldLabel(value)";
        if (text.Contains(t6))
        {
            string r6 = @"repairLiveString = function(tableName, rowKey, fieldPath, value)
    local RussianMod = runtimeFixes.RussianMod
    if RussianMod and RussianMod.lookupRussianText then
        local ru = RussianMod.lookupRussianText(value)
        if ru ~= nil then
            return ru
        end
    end
    local EnglishMod = runtimeFixes.EnglishMod
    if EnglishMod and EnglishMod.translate then
        local ruEng = EnglishMod.translate(value)
        if ruEng ~= nil then
            return ruEng
        end
    end
    local enterWorldShortened = shortenEnterWorldLabel(value)";
            text = text.Replace(t6, r6);
        }

        // 8. AssembleDescString hook
        string t7 = "    function utils:AssembleDescString(inString, values, rtbOverWrite, id, level, descType, originalType, descContext)\r\n        local original = originalAssembleDescString(";
        if (!text.Contains(t7)) t7 = "    function utils:AssembleDescString(inString, values, rtbOverWrite, id, level, descType, originalType, descContext)\n        local original = originalAssembleDescString(";
        if (text.Contains(t7))
        {
            string r7 = @"    function utils:AssembleDescString(inString, values, rtbOverWrite, id, level, descType, originalType, descContext)
        local RussianMod = runtimeFixes.RussianMod
        if type(inString) == ""string"" and RussianMod and RussianMod.lookupRussianText then
            local ruIn = RussianMod.lookupRussianText(inString)
            if ruIn ~= nil then
                inString = ruIn
            end
        end
        local original = originalAssembleDescString(";
            text = text.Replace(t7, r7);
        }

        // 9. creatorChoiceLabels
        string t11 = @"local creatorChoiceLabels = {
    [1] = { ""Madness"", ""Sanity"" },
    [2] = { ""Wisdom"", ""Power"" },
    [3] = { ""Glory"", ""Emotion"" },
}";
        if (text.Contains(t11))
        {
            string r11 = @"local creatorChoiceLabels = (runtimeFixes.RussianMod and runtimeFixes.RussianMod.creatorChoiceLabels) or {
    [1] = { ""Madness"", ""Sanity"" },
    [2] = { ""Wisdom"", ""Power"" },
    [3] = { ""Glory"", ""Emotion"" },
}";
            text = text.Replace(t11, r11);
        }

        // 10. TaskBoard block encapsulation
        string targetTaskBoardStart = "local taskBoardWidgetNames = {";
        string targetTaskBoardEnd = @"    local elapsed = nowMilliseconds() - started
    if elapsed >= 8 then
        runtimeMetrics.SlowTargetedRepairs = runtimeMetrics.SlowTargetedRepairs + 1
        report(""slow targeted Task Board repair elapsed_ms=""
            .. string.format(""%.2f"", elapsed)
            .. "" labels="" .. tostring(repaired))
    end
    return repaired
end";
        if (!text.Contains(targetTaskBoardEnd)) targetTaskBoardEnd = targetTaskBoardEnd.Replace("\r\n", "\n");
        if (text.Contains(targetTaskBoardStart) && text.Contains(targetTaskBoardEnd) && !text.Contains("do\nlocal taskBoardWidgetNames"))
        {
            text = text.Replace(targetTaskBoardStart, "do\nlocal taskBoardWidgetNames = {");
            string replTaskBoardEnd = targetTaskBoardEnd + @"
runtimeFixes.repairTaskInfoLabels = repairTaskInfoLabels
runtimeFixes.repairTaskListItemLabels = repairTaskListItemLabels
runtimeFixes.repairTaskBoardLabels = repairTaskBoardLabels
end";
            text = text.Replace(targetTaskBoardEnd, replTaskBoardEnd);
            text = text.Replace("        repairTaskListItemLabels,\r\n        true,", "        runtimeFixes.repairTaskListItemLabels,\r\n        true,");
            text = text.Replace("        repairTaskListItemLabels,\n        true,", "        runtimeFixes.repairTaskListItemLabels,\n        true,");
            text = text.Replace("        repairTaskInfoLabels,\r\n        true,", "        runtimeFixes.repairTaskInfoLabels,\r\n        true,");
            text = text.Replace("        repairTaskInfoLabels,\n        true,", "        runtimeFixes.repairTaskInfoLabels,\n        true,");
        }

        // 11. installShortMenuLabels encapsulation
        string targetShortMenuStart = "local function installShortMenuLabels(value, environment)";
        string targetShortMenuEnd = @"Loader.AfterLoad(
    ""Gameplay.LogicSystem.Menu.MenuBtn_Item"",
    function(value, environment)
        installShortMenuLabels(value, environment)
        return value
    end,
    1000000,
    ""cpdd.runtime-fix.short-menu-labels""
)";
        if (!text.Contains(targetShortMenuEnd)) targetShortMenuEnd = targetShortMenuEnd.Replace("\r\n", "\n");
        if (text.Contains(targetShortMenuStart) && text.Contains(targetShortMenuEnd) && !text.Contains("do\nlocal function installShortMenuLabels"))
        {
            text = text.Replace(targetShortMenuStart, "do\nlocal function installShortMenuLabels(value, environment)");
            text = text.Replace(targetShortMenuEnd, targetShortMenuEnd + "\nend");
        }

        // 12. Event-Driven Panel Repair encapsulation
        string targetPanelStart = "local dynamicPanelRescanUids = {";
        string targetPanelEnd = @"Loader.AfterLoad(
    ""Framework.KGFramework.KGUI.Core.UIComponent"",
    function(value, environment)
        installEventDrivenPanelRepair(value, environment)
        return value
    end,
    1000000,
    ""cpdd.runtime-fix.event-driven-panels""
)";
        if (!text.Contains(targetPanelEnd)) targetPanelEnd = targetPanelEnd.Replace("\r\n", "\n");
        if (text.Contains(targetPanelStart) && text.Contains(targetPanelEnd) && !text.Contains("do\nlocal dynamicPanelRescanUids"))
        {
            text = text.Replace(targetPanelStart, "do\nlocal dynamicPanelRescanUids = {");
            string targetPanelDef = @"local panelTextRepair = {
    States = setmetatable({}, { __mode = ""k"" }),
    Reports = {},
}";
            string replPanelDef = @"local panelTextRepair = {
    States = setmetatable({}, { __mode = ""k"" }),
    Reports = {},
}
runtimeFixes.panelTextRepair = panelTextRepair";
            if (!text.Contains(targetPanelDef)) targetPanelDef = targetPanelDef.Replace("\r\n", "\n");
            if (text.Contains(targetPanelDef)) text = text.Replace(targetPanelDef, replPanelDef);
            text = text.Replace(targetPanelEnd, targetPanelEnd + "\nend");
            text = text.Replace("RepairPanel = function(component) return panelTextRepair:Repair(component, \"manual\") end,",
                                "RepairPanel = function(component) return runtimeFixes.panelTextRepair and runtimeFixes.panelTextRepair:Repair(component, \"manual\") or 0 end,");
        }

        // 13. Statistics Everywhere encapsulation
        string targetStatsStart = "local function statisticsEverywhereEnabled()";
        string targetStatsEnd = @"Loader.AfterLoad(
    ""Gameplay.LogicSystem.HUD.HUD_MiddleBtnContent.HUDMiddleMenuCheck"",
    installStatisticsEverywhere,
    1000000,
    ""cpdd.runtime-fix.statistics-everywhere""
)";
        if (!text.Contains(targetStatsEnd)) targetStatsEnd = targetStatsEnd.Replace("\r\n", "\n");
        if (text.Contains(targetStatsStart) && text.Contains(targetStatsEnd) && !text.Contains("do\nlocal function statisticsEverywhereEnabled"))
        {
            text = text.Replace(targetStatsStart, "do\nlocal function statisticsEverywhereEnabled()");
            string replStatsEnd = @"runtimeFixes.setStatisticsEverywhere = setStatisticsEverywhere
runtimeFixes.statisticsEverywhereEnabled = statisticsEverywhereEnabled
" + targetStatsEnd + "\nend";
            text = text.Replace(targetStatsEnd, replStatsEnd);
            text = text.Replace("SetStatisticsEverywhere = setStatisticsEverywhere,", "SetStatisticsEverywhere = runtimeFixes.setStatisticsEverywhere,");
            text = text.Replace("IsStatisticsEverywhereEnabled = statisticsEverywhereEnabled,", "IsStatisticsEverywhereEnabled = runtimeFixes.statisticsEverywhereEnabled,");
        }

        // Write destinations
        string dest1 = @"D:\gameDev\translate lotm\data\Init.lua";
        string dest2 = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\Init.lua";

        File.WriteAllText(dest1, text, new UTF8Encoding(false));
        File.WriteAllText(dest2, text, new UTF8Encoding(false));

        Console.WriteLine("Successfully applied Russian hooks to Init.lua with encapsulated local scopes!");
        Console.WriteLine("Updated: " + dest1);
        Console.WriteLine("Updated: " + dest2);
        return 0;
    }
}