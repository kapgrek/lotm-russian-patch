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

        // 1. RussianMod and EnglishToRussian load
        string t1 = "    QuitGame = \"Exit\",\r\n}";
        if (!text.Contains(t1)) t1 = "    QuitGame = \"Exit\",\n}";
        if (!text.Contains(t1))
        {
            Console.WriteLine("Target 1 not found!");
            return 1;
        }
        string r1 = t1 + "\r\n\r\n" +
@"local okRussian, RussianMod = pcall(require, ""mods.cpdd_runtime_fixes.RussianLocalization"")
if okRussian and type(RussianMod) == ""table"" and RussianMod.Enabled then
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
    for k, v in pairs(EnglishMod.exact) do
        if visibleTextExactOverrides[k] == nil then
            visibleTextExactOverrides[k] = v
        end
    end
end";
        text = text.Replace(t1, r1);

        // 2. adjustWidgetLetterSpacing function
        string t2 = "Loader.Telemetry = Loader.Telemetry or {}\r\nLoader.Telemetry.Runtime = runtimeMetrics";
        if (!text.Contains(t2)) t2 = "Loader.Telemetry = Loader.Telemetry or {}\nLoader.Telemetry.Runtime = runtimeMetrics";
        if (!text.Contains(t2))
        {
            Console.WriteLine("Target 2 not found!");
            return 1;
        }
        string r2 = @"local function adjustWidgetLetterSpacing(widget, targetSize)
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
runtimeFixes.adjustWidgetLetterSpacing = adjustWidgetLetterSpacing

" + t2;
        text = text.Replace(t2, r2);

        // 3. lookupGeminiText hook
        string t3 = "local function lookupGeminiText(value)\r\n    if type(value) ~= \"string\" then return nil end";
        if (!text.Contains(t3)) t3 = "local function lookupGeminiText(value)\n    if type(value) ~= \"string\" then return nil end";
        if (!text.Contains(t3))
        {
            Console.WriteLine("Target 3 not found!");
            return 1;
        }
        string r3 = @"local function lookupGeminiText(value)
    if RussianMod and RussianMod.lookupRussianText then
        local ru = RussianMod.lookupRussianText(value)
        if ru ~= nil then
            return ru
        end
    end
    if type(value) ~= ""string"" then return nil end";
        text = text.Replace(t3, r3);

        // 4. translateVisibleText hook
        string t4 = @"    local reviewedExact = visibleTextExactOverrides[value]
    if reviewedExact ~= nil then
        visibleTextCache[value] = reviewedExact
        return reviewedExact
    end";
        if (!text.Contains(t4))
        {
            Console.WriteLine("Target 4 not found!");
            return 1;
        }
        string r4 = t4 + @"
    if RussianMod and RussianMod.lookupRussianText then
        local ru = RussianMod.lookupRussianText(value)
        if ru ~= nil then
            visibleTextCache[value] = ru
            return ru
        end
    end
    if EnglishMod and EnglishMod.translate then
        local ruEng = EnglishMod.translate(value)
        if ruEng ~= nil then
            visibleTextCache[value] = ruEng
            return ruEng
        end
    end";
        text = text.Replace(t4, r4);

        // 5. Letter spacing in setSingleWidgetText (both places)
        string t5a = "        pcall(function()\r\n            if widget.SynchronizeProperties ~= nil then\r\n                widget:SynchronizeProperties()\r\n            end\r\n        end)\r\n        pcall(function()\r\n            if widget.InvalidateLayoutAndVolatility ~= nil then";
        if (!text.Contains(t5a)) t5a = "        pcall(function()\n            if widget.SynchronizeProperties ~= nil then\n                widget:SynchronizeProperties()\n            end\n        end)\n        pcall(function()\n            if widget.InvalidateLayoutAndVolatility ~= nil then";
        string r5a = "        pcall(function()\r\n            if widget.SynchronizeProperties ~= nil then\r\n                widget:SynchronizeProperties()\r\n            end\r\n        end)\r\n        adjustWidgetLetterSpacing(widget)\r\n        pcall(function()\r\n            if widget.InvalidateLayoutAndVolatility ~= nil then";
        if (text.Contains(t5a))
        {
            text = text.Replace(t5a, r5a);
        }

        string t5b = "    pcall(function()\r\n        if widget.SynchronizeProperties ~= nil then\r\n            widget:SynchronizeProperties()\r\n        end\r\n    end)\r\n    pcall(function()\r\n        if widget.InvalidateLayoutAndVolatility ~= nil then";
        if (!text.Contains(t5b)) t5b = "    pcall(function()\n        if widget.SynchronizeProperties ~= nil then\n            widget:SynchronizeProperties()\n        end\n    end)\n    pcall(function()\n        if widget.InvalidateLayoutAndVolatility ~= nil then";
        string r5b = "    pcall(function()\r\n        if widget.SynchronizeProperties ~= nil then\r\n            widget:SynchronizeProperties()\r\n        end\r\n    end)\r\n    adjustWidgetLetterSpacing(widget)\r\n    pcall(function()\r\n        if widget.InvalidateLayoutAndVolatility ~= nil then";
        if (text.Contains(t5b))
        {
            text = text.Replace(t5b, r5b);
        }

        // 6. repairLiveString hook
        string t6 = "repairLiveString = function(tableName, rowKey, fieldPath, value)\r\n    local enterWorldShortened = shortenEnterWorldLabel(value)";
        if (!text.Contains(t6)) t6 = "repairLiveString = function(tableName, rowKey, fieldPath, value)\n    local enterWorldShortened = shortenEnterWorldLabel(value)";
        if (!text.Contains(t6))
        {
            Console.WriteLine("Target 6 not found!");
            return 1;
        }
        string r6 = @"repairLiveString = function(tableName, rowKey, fieldPath, value)
    if RussianMod and RussianMod.lookupRussianText then
        local ru = RussianMod.lookupRussianText(value)
        if ru ~= nil then
            return ru
        end
    end
    if EnglishMod and EnglishMod.translate then
        local ruEng = EnglishMod.translate(value)
        if ruEng ~= nil then
            return ruEng
        end
    end
    local enterWorldShortened = shortenEnterWorldLabel(value)";
        text = text.Replace(t6, r6);

        // 7. AssembleDescString hook
        string t7 = "    function utils:AssembleDescString(inString, values, rtbOverWrite, id, level, descType, originalType, descContext)\r\n        local original = originalAssembleDescString(";
        if (!text.Contains(t7)) t7 = "    function utils:AssembleDescString(inString, values, rtbOverWrite, id, level, descType, originalType, descContext)\n        local original = originalAssembleDescString(";
        if (!text.Contains(t7))
        {
            Console.WriteLine("Target 7 not found!");
            return 1;
        }
        string r7 = @"    function utils:AssembleDescString(inString, values, rtbOverWrite, id, level, descType, originalType, descContext)
        if type(inString) == ""string"" and RussianMod and RussianMod.lookupRussianText then
            local ruIn = RussianMod.lookupRussianText(inString)
            if ruIn ~= nil then
                inString = ruIn
            end
        end
        local original = originalAssembleDescString(";
        text = text.Replace(t7, r7);

        // 8. DescFormulaHelper hook
        string t8 = @"    helper.GenerateTipsDesc = function(tipsString, markTag)
        local original = originalGenerateTipsDesc(tipsString, markTag)
        if type(original) ~= ""string"" then
            return original
        end
        local translated = repairLiveString(
            ""DescFormulaHelper"", ""GenerateTipsDesc"",
            ""GenerateTipsDesc.return"", original
        )
        runtimeMetrics.CaptureTranslationAssignment(
            nil, ""DescFormulaHelper"", ""DescFormulaHelper"",
            ""TipsDescription"", original, translated
        )
        return translated
    end
    helper.__cpddGeneratedTipsRepair = VERSION";
        if (!text.Contains(t8))
        {
            Console.WriteLine("Target 8 not found!");
            return 1;
        }
        string r8 = @"    helper.GenerateTipsDesc = function(tipsString, markTag)
        local original = originalGenerateTipsDesc(tipsString, markTag)
        if type(original) ~= ""string"" then
            return original
        end
        local translated = repairLiveString(
            ""DescFormulaHelper"", ""GenerateTipsDesc"",
            ""GenerateTipsDesc.return"", original
        )
        runtimeMetrics.CaptureTranslationAssignment(
            nil, ""DescFormulaHelper"", ""DescFormulaHelper"",
            ""TipsDescription"", original, translated
        )
        return translated
    end

    local originalGenerateDesc = helper.GenerateDesc
    if type(originalGenerateDesc) == ""function"" then
        helper.GenerateDesc = function(...)
            local original = originalGenerateDesc(...)
            if type(original) ~= ""string"" then
                return original
            end
            return repairLiveString(
                ""DescFormulaHelper"", select(1, ...),
                ""GenerateDesc.return"", original
            )
        end
    end
    helper.__cpddGeneratedTipsRepair = VERSION";
        text = text.Replace(t8, r8);

        // 9. installSkillDescriptionRepair
        string t9 = @"    local wrapped = 0
    for _, methodName in ipairs({
        ""GenerateSkillDescNoRichText"",
        ""GenerateSkillBriefDesc"",
        ""GenerateSkillDecoText"",
    }) do
        local original = skillSystem[methodName]
        if type(original) == ""function"" then
            skillSystem[methodName] = function(self, ...)
                local results = { original(self, ...) }
                if type(results[1]) == ""string"" then
                    results[1] = repairLiveString(""SkillCustomSystem"", select(1, ...), methodName, results[1])
                end
                return unpack(results)
            end
            wrapped = wrapped + 1
        end
    end";
        if (!text.Contains(t9))
        {
            Console.WriteLine("Target 9 not found!");
            return 1;
        }
        string r9 = @"    local wrapped = 0
    local targetMethods = {
        ""GenerateSkillDesc"",
        ""GenerateSkillDescNoRichText"",
        ""GenerateSkillBriefDesc"",
        ""GenerateSkillDecoText"",
        ""GenerateSkillDetailDesc"",
        ""GenerateSkillNextDesc"",
        ""GetSkillDesc"",
        ""GetSkillBriefDesc"",
        ""GetSkillDetailDesc"",
        ""GenerateNextLevelDesc"",
        ""GetNextLevelDesc"",
    }
    local seen = {}
    for _, methodName in ipairs(targetMethods) do
        seen[methodName] = true
        local original = skillSystem[methodName]
        if type(original) == ""function"" then
            skillSystem[methodName] = function(self, ...)
                local results = { original(self, ...) }
                if type(results[1]) == ""string"" then
                    results[1] = repairLiveString(""SkillCustomSystem"", select(1, ...), methodName, results[1])
                end
                return unpack(results)
            end
            wrapped = wrapped + 1
        end
    end

    for k, v in pairs(skillSystem) do
        if not seen[k] and type(k) == ""string"" and type(v) == ""function"" and (
            k:find(""SkillDesc"") or k:find(""SkillBrief"") or k:find(""SkillDeco"") or k:find(""SkillDetail"") or k:find(""Desc"")
        ) then
            local original = v
            skillSystem[k] = function(self, ...)
                local results = { original(self, ...) }
                if type(results[1]) == ""string"" then
                    results[1] = repairLiveString(""SkillCustomSystem"", select(1, ...), k, results[1])
                end
                return unpack(results)
            end
            wrapped = wrapped + 1
        end
    end";
        text = text.Replace(t9, r9);

        // 10. repairSkillCommonLabels
        string t10 = @"    runtimeFixes.setNamedWidgetText(view, ""Text_WoodenPost"", ""Training Dummy"")
    local oneClickPage = nil
    pcall(function()
        oneClickPage = view.WBP_Skill_OneClick_Page
    end)
    runtimeFixes.setNamedWidgetText(oneClickPage, ""Text_Content"", ""One-Click Assist"")

    -- BP_SetType on the embedded header can refresh all three captions after
    -- its Lua component returns. Repair the nested UserWidget from the parent
    -- as the final owner as well as through the component hook.
    runtimeFixes.repairEmbeddedSkillHeaderLabels(self)
    runtimeFixes.repairSkillHeaderLabels(self and self.WBP_Skill_BeStrong_BtnCom)
end";
        if (!text.Contains(t10))
        {
            Console.WriteLine("Target 10 not found!");
            return 1;
        }
        string r10 = @"    runtimeFixes.setNamedWidgetText(view, ""Text_WoodenPost"", ""Манекен"")
    local oneClickPage = nil
    pcall(function()
        oneClickPage = view.WBP_Skill_OneClick_Page
    end)
    runtimeFixes.setNamedWidgetText(oneClickPage, ""Text_Content"", ""Помощник"")

    -- BP_SetType on the embedded header can refresh all three captions after
    -- its Lua component returns. Repair the nested UserWidget from the parent
    -- as the final owner as well as through the component hook.
    runtimeFixes.repairEmbeddedSkillHeaderLabels(self)
    runtimeFixes.repairSkillHeaderLabels(self and self.WBP_Skill_BeStrong_BtnCom)
    translateViewTextWidgets(view, self.userWidget or self.widget)
end";
        text = text.Replace(t10, r10);

        // 11. creatorChoiceLabels
        string t11 = @"local creatorChoiceLabels = {
    [1] = { ""Madness"", ""Sanity"" },
    [2] = { ""Wisdom"", ""Power"" },
    [3] = { ""Glory"", ""Emotion"" },
}";
        if (!text.Contains(t11))
        {
            Console.WriteLine("Target 11 not found!");
            return 1;
        }
        string r11 = @"local creatorChoiceLabels = (RussianMod and RussianMod.creatorChoiceLabels) or {
    [1] = { ""Madness"", ""Sanity"" },
    [2] = { ""Wisdom"", ""Power"" },
    [3] = { ""Glory"", ""Emotion"" },
}";
        text = text.Replace(t11, r11);

        // 12. installShortMenuLabels
        string t12 = @"        local label = menuData and shortMenuLabels[menuData.ButtonEnum]
        if label and self.view then
            -- KGTextBlock can repaint its serialized long translation after
            -- OnRefresh. Persist the compact value in both the widget property
            -- and the live Slate text so later menu refreshes cannot restore it.
            runtimeFixes.setNamedWidgetText(self.view, ""Text_Name"", label)
        end
        return unpack(results)
    end
    class.__cpddShortMenuLabels = true
    report(""installed compact English menu labels"")";
        if (!text.Contains(t12))
        {
            Console.WriteLine("Target 12 not found!");
            return 1;
        }
        string r12 = @"        local label = menuData and shortMenuLabels[menuData.ButtonEnum]
        if label and self.view then
            -- KGTextBlock can repaint its serialized long translation after
            -- OnRefresh. Persist the compact value in both the widget property
            -- and the live Slate text so later menu refreshes cannot restore it.
            runtimeFixes.setNamedWidgetText(self.view, ""Text_Name"", label)
            local textWidget = getNamedWidget(self.view, ""Text_Name"")
            if textWidget then
                adjustWidgetLetterSpacing(textWidget, 13)
            end
        end
        return unpack(results)
    end
    class.__cpddShortMenuLabels = true
    report(""installed compact Russian menu labels"")";
        text = text.Replace(t12, r12);

        // Write destinations
        string dest1 = @"D:\gameDev\translate lotm\data\Init.lua";
        string dest2 = @"D:\Games\GMZZLauncher\Game\C7\Saved\Mods\lua\mods\cpdd_runtime_fixes\Init.lua";

        File.WriteAllText(dest1, text, new UTF8Encoding(false));
        File.WriteAllText(dest2, text, new UTF8Encoding(false));

        Console.WriteLine("Successfully applied Russian hooks to 0.9.71 Init.lua!");
        Console.WriteLine("Updated: " + dest1);
        Console.WriteLine("Updated: " + dest2);
        return 0;
    }
}