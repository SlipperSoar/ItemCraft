using System.Collections.Generic;
using System.Linq;
using SodaCraft.Localizations;

namespace ItemCraft
{
    /// <summary>
    /// 本地化工具
    /// </summary>
    public static class LocalizeUtil
    {
        #region Properties

        public static IDictionary<int, string> ItemNameTable => itemNameTable;
        
        private static Dictionary<int, string> itemNameTable;

        // 英语
        private static Dictionary<string, string> enLocalizeDictionary = new Dictionary<string, string>()
        {
            { "ItemCraftRecipeNotFound", "Can't Craft Item: {0}" },
        };
        
        // 简中
        private static Dictionary<string, string> cnLocalizeDictionary = new Dictionary<string, string>()
        {
            { "ItemCraftRecipeNotFound", "无法合成道具：{0}" },
        };
        
        // 繁中
        private static Dictionary<string, string> tcLocalizeDictionary = new Dictionary<string, string>()
        {
            { "ItemCraftRecipeNotFound", "無法合成道具：{0}" },
        };
        
        // 日语
        private static Dictionary<string, string> jpLocalizeDictionary = new Dictionary<string, string>()
        {
            { "ItemCraftRecipeNotFound", "アイテム（{0}）を合成できません" },
        };

        #endregion
        
        #region Public Methods

        /// <summary>
        /// 初始化本地化数据
        /// </summary>
        public static void Initialize()
        {
            UnityEngine.Debug.Log("【本地化工具】初始化道具名称");
            // 由于无法直接通过道具id获取其本地化名称，故而采取此下策：预先缓存所有道具的本地化名称
            itemNameTable = new Dictionary<int, string>();
            // var allItemIds = ItemStatsSystem.ItemAssetsCollection.GetAllTypeIds(new ItemStatsSystem.ItemFilter());
            var allItemIds = ItemStatsSystem.ItemAssetsCollection.Instance.entries.Select(x => x.typeID);
            foreach (var itemId in allItemIds)
            {
                var metaData = ItemStatsSystem.ItemAssetsCollection.GetMetaData(itemId);
                itemNameTable.Add(itemId, metaData.DisplayName);
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
                    dict = cnLocalizeDictionary;
                    break;
                case UnityEngine.SystemLanguage.ChineseTraditional:
                    dict = tcLocalizeDictionary;
                    break;
                // 未列出的语言默认采用英语
                default:
                    dict = enLocalizeDictionary;
                    break;
            }

            if (dict.TryGetValue(key, out var value))
            {
                return string.Format(value, args);
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    }
}