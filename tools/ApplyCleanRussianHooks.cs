using System;
using System.IO;
using System.Text;

class Program
{
    static int Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string sourceInitPath = @"d:\gameDev\NewBild\source_en\Init_English.lua";
        string destInitPath = @"d:\gameDev\NewBild\data\Init.lua";

        if (!File.Exists(sourceInitPath))
        {
            Console.WriteLine("Source Init_English.lua not found at: " + sourceInitPath);
            return 1;
        }

        string text = File.ReadAllText(sourceInitPath, Encoding.UTF8);

        // 1. Remove duplicate runtimeFixes declaration near line 913
        string tOldRfWin = "CaptureDataAssignmentsEnabled = false,\r\n}\r\nlocal runtimeFixes = {}\r\n";
        string tOldRfUnix = "CaptureDataAssignmentsEnabled = false,\n}\nlocal runtimeFixes = {}\n";
        if (text.Contains(tOldRfWin))
            text = text.Replace(tOldRfWin, "CaptureDataAssignmentsEnabled = false,\r\n}\r\n-- runtimeFixes declared above\r\n");
        else if (text.Contains(tOldRfUnix))
            text = text.Replace(tOldRfUnix, "CaptureDataAssignmentsEnabled = false,\n}\n-- runtimeFixes declared above\n");
        else
            Console.WriteLine("Warning: Could not find tOldRf");

        // 2. Define runtimeFixes early and wrap RussianMod in do ... end
        string t1Win = "    QuitGame = \"Exit\",\r\n}";
        string t1Unix = "    QuitGame = \"Exit\",\n}";
        string r1Block =
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
end";

        if (text.Contains(t1Win))
            text = text.Replace(t1Win, t1Win + "\r\n\r\n" + r1Block);
        else if (text.Contains(t1Unix))
            text = text.Replace(t1Unix, t1Unix + "\n\n" + r1Block);
        else
            Console.WriteLine("Warning: Could not find t1");

        // 3. rawLookupGeminiText & lookupGeminiText facade hook
        string tLookupWin = "local function lookupGeminiText(value)\r\n    if type(value) ~= \"string\" then return nil end";
        string tLookupUnix = "local function lookupGeminiText(value)\n    if type(value) ~= \"string\" then return nil end";
        if (text.Contains(tLookupWin))
            text = text.Replace(tLookupWin, "local function rawLookupGeminiText(value)\r\n    if type(value) ~= \"string\" then return nil end");
        else if (text.Contains(tLookupUnix))
            text = text.Replace(tLookupUnix, "local function rawLookupGeminiText(value)\n    if type(value) ~= \"string\" then return nil end");
        else
            Console.WriteLine("Warning: Could not find tLookup");

        string tShardEndWin = "    touchGeminiShard(prefix)\r\n    local translated = shard[value]\r\n    cacheGeminiLookup(value, translated)\r\n    return translated\r\nend";
        string tShardEndUnix = "    touchGeminiShard(prefix)\n    local translated = shard[value]\n    cacheGeminiLookup(value, translated)\n    return translated\nend";
        string rLookupFacade =
@"local function lookupGeminiText(value)
    local RussianMod = runtimeFixes.RussianMod
    if RussianMod and RussianMod.lookupRussianText then
        local ru = RussianMod.lookupRussianText(value)
        if ru ~= nil then
            return ru
        end
    end
    return rawLookupGeminiText(value)
end

runtimeFixes.rawLookupGeminiText = rawLookupGeminiText
runtimeFixes.lookupGeminiText = lookupGeminiText
if runtimeFixes.RussianMod then
    runtimeFixes.RussianMod.lookupGeminiText = rawLookupGeminiText
end";

        if (text.Contains(tShardEndWin))
            text = text.Replace(tShardEndWin, tShardEndWin + "\r\n\r\n" + rLookupFacade);
        else if (text.Contains(tShardEndUnix))
            text = text.Replace(tShardEndUnix, tShardEndUnix + "\n\n" + rLookupFacade);
        else
            Console.WriteLine("Warning: Could not find tShardEnd");

        // 4. translateVisibleText hook
        string tVisExactWin = "    local reviewedExact = visibleTextExactOverrides[value]\r\n    if reviewedExact ~= nil then\r\n        visibleTextCache[value] = reviewedExact\r\n        return reviewedExact\r\n    end";
        string tVisExactUnix = "    local reviewedExact = visibleTextExactOverrides[value]\n    if reviewedExact ~= nil then\n        visibleTextCache[value] = reviewedExact\n        return reviewedExact\n    end";
        string rVisHook =
@"    local RussianMod = runtimeFixes.RussianMod
    if RussianMod and RussianMod.lookupRussianText then
        local ru = RussianMod.lookupRussianText(value)
        if ru ~= nil then
            visibleTextCache[value] = ru
            return ru
        end
    end";

        if (text.Contains(tVisExactWin))
            text = text.Replace(tVisExactWin, tVisExactWin + "\r\n" + rVisHook);
        else if (text.Contains(tVisExactUnix))
            text = text.Replace(tVisExactUnix, tVisExactUnix + "\n" + rVisHook);
        else
            Console.WriteLine("Warning: Could not find tVisExact");

        // 5. repairLiveString hook
        string tLiveWin = "repairLiveString = function(tableName, rowKey, fieldPath, value)\r\n    local enterWorldShortened = shortenEnterWorldLabel(value)";
        string tLiveUnix = "repairLiveString = function(tableName, rowKey, fieldPath, value)\n    local enterWorldShortened = shortenEnterWorldLabel(value)";
        string rLiveHook =
@"repairLiveString = function(tableName, rowKey, fieldPath, value)
    local RussianMod = runtimeFixes.RussianMod
    if RussianMod and RussianMod.lookupRussianText then
        local ru = RussianMod.lookupRussianText(value)
        if ru ~= nil then
            return ru
        end
    end
    local enterWorldShortened = shortenEnterWorldLabel(value)";

        if (text.Contains(tLiveWin))
            text = text.Replace(tLiveWin, rLiveHook);
        else if (text.Contains(tLiveUnix))
            text = text.Replace(tLiveUnix, rLiveHook.Replace("\r\n", "\n"));
        else
            Console.WriteLine("Warning: Could not find tLive");

        // 6. AssembleDescString hook
        string tDescWin = "    function utils:AssembleDescString(inString, values, rtbOverWrite, id, level, descType, originalType, descContext)\r\n        local original = originalAssembleDescString(";
        string tDescUnix = "    function utils:AssembleDescString(inString, values, rtbOverWrite, id, level, descType, originalType, descContext)\n        local original = originalAssembleDescString(";
        string rDescHook =
@"    function utils:AssembleDescString(inString, values, rtbOverWrite, id, level, descType, originalType, descContext)
        local RussianMod = runtimeFixes.RussianMod
        if type(inString) == ""string"" and RussianMod and RussianMod.lookupRussianText then
            local ruIn = RussianMod.lookupRussianText(inString)
            if ruIn ~= nil then
                inString = ruIn
            end
        end
        local original = originalAssembleDescString(";

        if (text.Contains(tDescWin))
            text = text.Replace(tDescWin, rDescHook);
        else if (text.Contains(tDescUnix))
            text = text.Replace(tDescUnix, rDescHook.Replace("\r\n", "\n"));
        else
            Console.WriteLine("Warning: Could not find tDesc");

        // 7. creatorChoiceLabels hook
        string tChoice = "local creatorChoiceLabels = {\r\n    [1] = { \"Madness\", \"Sanity\" },\r\n    [2] = { \"Wisdom\", \"Power\" },\r\n    [3] = { \"Glory\", \"Emotion\" },\r\n}";
        if (!text.Contains(tChoice)) tChoice = tChoice.Replace("\r\n", "\n");
        string rChoice = "local creatorChoiceLabels = (runtimeFixes.RussianMod and runtimeFixes.RussianMod.creatorChoiceLabels) or {\r\n    [1] = { \"Madness\", \"Sanity\" },\r\n    [2] = { \"Wisdom\", \"Power\" },\r\n    [3] = { \"Glory\", \"Emotion\" },\r\n}";
        if (!tChoice.Contains("\r\n")) rChoice = rChoice.Replace("\r\n", "\n");
        if (text.Contains(tChoice)) text = text.Replace(tChoice, rChoice);

        // 8. TaskBoard encapsulation for LJ_MAX_LOCALS
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
        if (text.Contains(targetTaskBoardStart) && text.Contains(targetTaskBoardEnd))
        {
            text = text.Replace(targetTaskBoardStart, "do\nlocal taskBoardWidgetNames = {");
            string replTaskBoardEnd = targetTaskBoardEnd + "\n" +
@"runtimeFixes.repairTaskInfoLabels = repairTaskInfoLabels
runtimeFixes.repairTaskListItemLabels = repairTaskListItemLabels
runtimeFixes.repairTaskBoardLabels = repairTaskBoardLabels
end";
            text = text.Replace(targetTaskBoardEnd, replTaskBoardEnd);
            text = text.Replace("        repairTaskListItemLabels,\r\n        true,", "        runtimeFixes.repairTaskListItemLabels,\r\n        true,");
            text = text.Replace("        repairTaskListItemLabels,\n        true,", "        runtimeFixes.repairTaskListItemLabels,\n        true,");
            text = text.Replace("        repairTaskInfoLabels,\r\n        true,", "        runtimeFixes.repairTaskInfoLabels,\r\n        true,");
            text = text.Replace("        repairTaskInfoLabels,\n        true,", "        runtimeFixes.repairTaskInfoLabels,\n        true,");
        }

        // 9. installShortMenuLabels encapsulation
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
        if (text.Contains(targetShortMenuStart) && text.Contains(targetShortMenuEnd))
        {
            text = text.Replace(targetShortMenuStart, "do\nlocal function installShortMenuLabels(value, environment)");
            text = text.Replace(targetShortMenuEnd, targetShortMenuEnd + "\nend");
        }

        // 10. Event-Driven Panel Repair encapsulation
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
        if (text.Contains(targetPanelStart) && text.Contains(targetPanelEnd))
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

        // 11. Statistics Everywhere encapsulation
        string targetStatsStart = "local function statisticsEverywhereEnabled()";
        string targetStatsEnd = @"Loader.AfterLoad(
    ""Gameplay.LogicSystem.HUD.HUD_MiddleBtnContent.HUDMiddleMenuCheck"",
    installStatisticsEverywhere,
    1000000,
    ""cpdd.runtime-fix.statistics-everywhere""
)";
        if (!text.Contains(targetStatsEnd)) targetStatsEnd = targetStatsEnd.Replace("\r\n", "\n");
        if (text.Contains(targetStatsStart) && text.Contains(targetStatsEnd))
        {
            text = text.Replace(targetStatsStart, "do\nlocal function statisticsEverywhereEnabled()");
            string replStatsEnd = @"runtimeFixes.setStatisticsEverywhere = setStatisticsEverywhere
runtimeFixes.statisticsEverywhereEnabled = statisticsEverywhereEnabled
" + targetStatsEnd + "\nend";
            text = text.Replace(targetStatsEnd, replStatsEnd);
            text = text.Replace("SetStatisticsEverywhere = setStatisticsEverywhere,", "SetStatisticsEverywhere = runtimeFixes.setStatisticsEverywhere,");
            text = text.Replace("IsStatisticsEverywhereEnabled = statisticsEverywhereEnabled,", "IsStatisticsEverywhereEnabled = runtimeFixes.statisticsEverywhereEnabled,");
        }

        File.WriteAllText(destInitPath, text, new UTF8Encoding(false));
        Console.WriteLine("Clean Russian hooks successfully written to: " + destInitPath);
        return 0;
    }
}
