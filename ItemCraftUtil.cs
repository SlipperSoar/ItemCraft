using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Duckov.Economy;
using ItemStatsSystem;

namespace ItemCraft
{
    /// <summary>
    /// 道具合成工具类
    /// </summary>
    public static class ItemCraftUtil
    {
        #region Public Methods

        public static void Initialize()
        {
            UnityEngine.Debug.Log(LocalizeUtil.Localize(LocalizeUtil.InitCraftTables));
            
            // 首先保证合成配方列表已初始化完毕
            var formulaCollectionInstance = CraftingFormulaCollection.Instance;
            var formulas = formulaCollectionInstance.Entries;
            
            var additiveCraftFormulas = new Dictionary<string, CraftingFormula>();

            // 加载所有合成表
            var currentDir = GetModPath();
            LogItemTable2File(currentDir);
            LogFormulaInfos2File(currentDir);
            var configFiles = GetAllCraftTableConfigFiles(currentDir);
            foreach (var configFile in configFiles)
            {
                LoadCraftTableFromFile(configFile, additiveCraftFormulas);
            }

            // 修改合成表列表
            var listField = typeof(CraftingFormulaCollection).GetField("list", BindingFlags.Instance | BindingFlags.NonPublic);
            if (listField == null)
            {
                UnityEngine.Debug.LogError("【合成工具】无法找到 CraftingFormulaCollection.list 字段");
                return;
            }

            var formulaList = (List<CraftingFormula>)listField.GetValue(formulaCollectionInstance);
            if (formulaList == null)
            {
                UnityEngine.Debug.LogError("【合成工具】formulaList 为 null");
                return;
            }

            formulaList.Capacity = formulaList.Count + additiveCraftFormulas.Count;
            foreach (var additiveCraftFormula in additiveCraftFormulas)
            {
                if (formulaList.Any(x => x.id == additiveCraftFormula.Key))
                {
                    // 与原有配方出现id重复
                    var errorMsg = LocalizeUtil.Localize(LocalizeUtil.FormulaIdAlreadyExistInRawList,
                        additiveCraftFormula.Key);
                    UnityEngine.Debug.LogWarning(errorMsg);
                }
                else
                {
                    formulaList.Add(additiveCraftFormula.Value);
                }
            }
        }

        public static void OnItemCrafted(CraftingFormula formula, Item item)
        {
            UnityEngine.Debug.Log($"【合成工具】合成成功：{item.DisplayName} by 配方id: {formula.id}");
        }

        public static void OnFormulaUnlocked(string formulaId)
        {
            UnityEngine.Debug.Log($"【合成工具】解锁配方：{formulaId}");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 从文件加载合成表
        /// </summary>
        /// <param name="filePath">合成表文件路径</param>
        private static void LoadCraftTableFromFile(string filePath, Dictionary<string, CraftingFormula> craftTable)
        {
            var csvContents = CsvUtil.ReadCSV(filePath);
            if (csvContents == null)
            {
                return;
            }

            foreach (var content in csvContents)
            {
                // 按照既定格式处理
                // 不符合格式导致处理出现异常的行会跳过
                try
                {
                    // 配方格式：
                    // 官方消耗道具格式：以‘,’分割，每个道具是 name:num，即 名字:数量，这里为初始化方便将名字改为id，即id:num
                    // 示例：
                    // 1:2,2:1,3:1,4:1
                    var materials = content["materials"].Split(',');
                    var targetItem = content["target"].Split(':');
                    var targetItemId = int.Parse(targetItem[0]);
                    var targetItemAmount = int.Parse(targetItem[1]);
                    var costMoney = int.Parse(content["money"]);
                    var materialItems = materials.Select(str =>
                    {
                        var split = str.Split(':');
                        return (int.Parse(split[0]), long.Parse(split[1]));
                    }).ToArray();
                    var tags = content["tags"].Split(',');
                    var formulaId = content["id"];

                    var notExistItems = new List<int>();
                    // 检查配方中的物品是否存在
                    if (!ItemStatsSystem.ItemAssetsCollection.GetPrefab(targetItemId))
                    {
                        notExistItems.Add(targetItemId);
                    }

                    foreach (var valueTuple in materialItems)
                    {
                        if (!ItemStatsSystem.ItemAssetsCollection.GetPrefab(valueTuple.Item1))
                        {
                            notExistItems.Add(valueTuple.Item1);
                        }
                    }

                    // 如果存在物品不存在的，跳过该配方
                    if (notExistItems.Any())
                    {
                        UnityEngine.Debug.LogWarning(LocalizeUtil.Localize(LocalizeUtil.ItemDontExist, formulaId, string.Join(",", notExistItems)));
                        continue;
                    }
                    
                    // UnityEngine.Debug.Log($"【合成工具】初始化合成配方：{string.Join("+", materialItems.Select(x => $"{LocalizeUtil.LocalizeItem(x.Item1)}*{x.Item2}"))} + ￥{costMoney} => {LocalizeUtil.LocalizeItem(targetItemId)}*{targetItemAmount}");
                    var recipe = new CraftingFormula
                    {
                        id = formulaId,
                        cost = new Cost(costMoney, materialItems),
                        result = new CraftingFormula.ItemEntry
                        {
                            id = targetItemId,
                            amount = targetItemAmount
                        },
                        unlockByDefault = bool.Parse(content["unlockByDefault"].ToLower()),
                        tags = tags
                    };
                    
                    // 避免配置配方出现id重复的情况
                    if (!craftTable.TryAdd(formulaId, recipe))
                    {
                        var errorMsg = LocalizeUtil.Localize(LocalizeUtil.FormulaIdAlreadyExistInOtherCsv, formulaId, filePath);
                        UnityEngine.Debug.LogWarning(errorMsg);
                    }
                }
                catch (Exception e)
                {
                    // 不暴露调用栈，只显示错误信息
                    var errorMsg = LocalizeUtil.Localize(LocalizeUtil.InvalidFormulaConfig,
                        string.Join(",", content.Select(kvp => $"{kvp.Key}={kvp.Value}")), filePath);
                    UnityEngine.Debug.LogWarning(errorMsg);
                    // UnityEngine.Debug.LogException(e);
                    continue;
                }
            }
        }
        
        private static string[] GetAllCraftTableConfigFiles(string path)
        {
            // 所有的 合成配方 的配置文件都需要以 “.craft.csv” 结尾，即结尾为".craft"的 csv 文件
            var configFiles = Directory.GetFiles(path, "*.craft.csv");
            return configFiles;
        }

        private static string GetModPath()
        {
            // 路径是到游戏根目录为止
            var currentDir = Directory.GetCurrentDirectory();
            currentDir = Path.Combine(currentDir, @"Duckov_Data\Mods\ItemCraft");
            // 确保目录都存在
            UnityEngine.Debug.Log(LocalizeUtil.Localize(LocalizeUtil.EnsureDirectoryExist, currentDir));
            if (!Directory.Exists(currentDir))
            {
                // 创建目录时顺带创建一个示例的csv
                Directory.CreateDirectory(currentDir);
                UnityEngine.Debug.Log(LocalizeUtil.Localize(LocalizeUtil.CreateSampleCsvFile));
                var sampleCsvFilePath = Path.Combine(currentDir, "sample.craft.csv");
                var sampleCsvFileContent = @"
id	target	money	materials	unlockByDefault	tags	_note
Food_CocoMilk	105:1	0	938:1,60:1	true	WorkBenchAdvanced	可可奶
Large_HealBox	15:1	0	16:3,136:1,89:1	true	MedicStation	大急救箱
Item_UPhone	63:1	0	114:1,339:1	true	WorkBenchAdvanced	uPhone手机
Item_TOMSUNG	114:1	0	63:1,339:1	true	WorkBenchAdvanced	TOMSUNG手机
Food_Cake	132:1	0	84:1,105:1,68:1	true	Cook	烹饪蛋糕";
                File.Create(sampleCsvFilePath).Close();
                File.WriteAllText(sampleCsvFilePath, sampleCsvFileContent.TrimStart());
            }

            return currentDir;
        }

        private static void LogItemTable2File(string folderPath)
        {
            // 将所有的道具信息输出到本地文件以便设计合成表
            var itemInfoContents = new System.Text.StringBuilder();
            itemInfoContents.AppendLine("id\tname");
            foreach (var kvp in LocalizeUtil.ItemNameTable)
            {
                itemInfoContents.AppendLine($"{kvp.Key}\t{kvp.Value}");
            }
            
            System.IO.File.WriteAllText(Path.Combine(folderPath, "ItemInfo.txt"), itemInfoContents.ToString());
        }
        
        private static void LogFormulaInfos2File(string folderPath)
        {
            // 将所有的配方信息输出到本地文件以便设计合成表
            var contents = new System.Text.StringBuilder();
            contents.AppendLine("id\ttags");
            foreach (var formula in CraftingFormulaCollection.Instance.Entries)
            {
                contents.AppendLine($"{formula.id}\t{string.Join(",", formula.tags)}");
            }
            
            System.IO.File.WriteAllText(Path.Combine(folderPath, "FormulaInfomations.txt"), contents.ToString());
        }

        #endregion
    }
}