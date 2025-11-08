using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SodaCraft.Localizations;

namespace ItemCraft
{
    /// <summary>
    /// 本地化工具
    /// </summary>
    public static class LocalizeUtil
    {
        #region Const

        /// <summary>配方id已存在于其他自定义配方表</summary>
        public const string FormulaIdAlreadyExistInOtherCsv = "FormulaIdAlreadyExistInOtherCsv";
        
        /// <summary>配方id已存在于原有的配方表</summary>
        public const string FormulaIdAlreadyExistInRawList = "FormulaIdAlreadyExistInRawList";

        /// <summary>无效的配方配置</summary>
        public const string InvalidFormulaConfig = "InvalidFormulaConfig";

        /// <summary>确保要使用的目录存在</summary>
        public const string EnsureDirectoryExist = "EnsureDirectoryExist";

        /// <summary>创建示例csv文件</summary>
        public const string CreateSampleCsvFile = "CreateSampleCsvFile";

        public const string InitCraftTables = "InitCraftTables";
        
        public const string InitItemNameTable = "InitItemNameTable";
        
        private const string ModName = "ItemCraft";

        #endregion
        
        #region Properties

        public static IDictionary<int, string> ItemNameTable => itemNameTable;
        
        private static Dictionary<int, string> itemNameTable;

        // 英语
        private static Dictionary<string, string> enLocalizeDictionary = new Dictionary<string, string>()
        {
            { ModName, "[More DIY ItemCraft Mod]" },
            { EnsureDirectoryExist, "Ensure the directory {0} exists" },
            { CreateSampleCsvFile, "Create sample csv file" },
            { InitCraftTables, "Init DIY Item crafting recipes from csv files" },
            { InitItemNameTable, "Init item names" },
            { FormulaIdAlreadyExistInOtherCsv, "The Crafting recipe [{0}] already exist in other csv table (*.craft.csv), this one in {1} invalid" },
            { FormulaIdAlreadyExistInRawList, "The Crafting recipe [{0}] already exist in game recipe list" },
            { InvalidFormulaConfig, "Invalid crafting recipe config: {0}, file: {1}" },
        };
        
        // 简中
        private static Dictionary<string, string> cnLocalizeDictionary = new Dictionary<string, string>()
        {
            { ModName, "[更多DIY配方]" },
            { EnsureDirectoryExist, "确保目录 {0} 存在" },
            { CreateSampleCsvFile, "创建示例csv文件" },
            { InitCraftTables, "从csv文件初始化DIY配方" },
            { InitItemNameTable, "初始化道具名称" },
            { FormulaIdAlreadyExistInOtherCsv, "其他DIY配方表（*.craft.csv）中已存在同名配方：{0}, {1} 中该配方无效" },
            { FormulaIdAlreadyExistInRawList, "游戏原配方列表中已存在同名配方：[{0}], 该自定义配方无效" },
            { InvalidFormulaConfig, "无效的配方配置：{0}，配置文件：{1}" },
        };
        
        // 日语
        private static Dictionary<string, string> jpLocalizeDictionary = new Dictionary<string, string>()
        {
            { ModName, "[合成レシピモジュール]" },
            { EnsureDirectoryExist, "ディレクトリ {0} が存在することを確認する" },
            { CreateSampleCsvFile, "サンプルCSVファイルを作成する" },
            { InitCraftTables, "csvフェイルからDIYレシピを初期化する" },
            { InitItemNameTable, "アイテムの名前を初期化する" },
            { FormulaIdAlreadyExistInOtherCsv, "他のDIYレシピ表（*.craft.csv）にすでに同名のレシピがあります：{0}、{1}ではそのレシピは無効です" },
            { FormulaIdAlreadyExistInRawList, "ゲームの元のレシピリストには同名のレシピが既に存在します：[{0}]、このカスタムレシピは無効です" },
            { InvalidFormulaConfig, "無効なレシピ設定：{0}、フェイル：{1}" },
        };

        #endregion
        
        #region Public Methods

        /// <summary>
        /// 初始化本地化数据
        /// </summary>
        public static void Initialize()
        {
            UnityEngine.Debug.Log(Localize(InitItemNameTable));
            // 由于无法直接通过道具id获取其本地化名称，故而采取此下策：预先缓存所有道具的本地化名称
            // 顺便可以用来保存至本地文件，方便人设计配方
            itemNameTable = new Dictionary<int, string>();
            var allItemIds = ItemStatsSystem.ItemAssetsCollection.Instance.entries.Select(x => x.typeID);
            foreach (var itemId in allItemIds)
            {
                var metaData = ItemStatsSystem.ItemAssetsCollection.GetMetaData(itemId);
                itemNameTable.Add(itemId, metaData.DisplayName);
            }

            // 尝试加载Mod 物品
            var dynamicEntries = typeof(ItemStatsSystem.ItemAssetsCollection).GetField("dynamicDic",
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
            if (dynamicEntries == null)
            {
                UnityEngine.Debug.LogError($"can't find dynamicDic field");
            }
            else
            {
                var dynamicDic = dynamicEntries.GetValue(ItemStatsSystem.ItemAssetsCollection.Instance) as Dictionary<int, ItemStatsSystem.ItemAssetsCollection.DynamicEntry>;
                if (dynamicDic == null)
                {
                    UnityEngine.Debug.LogError($"dynamicDic is null");
                }
                else
                {
                    foreach (var entry in dynamicDic.Select(dynamicEntry => dynamicEntry.Value))
                    {
                        itemNameTable.Add(entry.typeID, entry.MetaData.DisplayName);
                    }
                }
            }
        }

        public static string LocalizeItem(int itemId)
        {
            return itemNameTable.TryGetValue(itemId, out var name) ? name : string.Empty;
        }

        public static string Localize(string key, params object[] args)
        {
            Dictionary<string, string> dict = null;
            switch (LocalizationManager.CurrentLanguage)
            {
                case UnityEngine.SystemLanguage.Japanese:
                    dict = jpLocalizeDictionary;
                    break;
                case UnityEngine.SystemLanguage.Chinese:
                case UnityEngine.SystemLanguage.ChineseSimplified:
                case UnityEngine.SystemLanguage.ChineseTraditional:
                    dict = cnLocalizeDictionary;
                    break;
                // 未列出的语言默认采用英语
                default:
                    dict = enLocalizeDictionary;
                    break;
            }

            if (dict.TryGetValue(key, out var value))
            {
                return $"{dict[ModName]} {string.Format(value, args)}";
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    }
}