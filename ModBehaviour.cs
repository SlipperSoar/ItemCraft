using System;
using ItemStatsSystem;

namespace ItemCraft
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        #region Unity Override

        private void Awake()
        {
            UnityEngine.Debug.Log("Mod Item Craft Loaded!");
        }

        private void OnEnable()
        {
            CraftingManager.OnItemCrafted += ItemCraftUtil.OnItemCrafted;
            CraftingManager.OnFormulaUnlocked += ItemCraftUtil.OnFormulaUnlocked;
        }

        private void Start()
        {
            // 本地化初始化
            LocalizeUtil.Initialize();
            // 初始化合成表
            ItemCraftUtil.Initialize();
        }
        
        private void OnDisable()
        {
            CraftingManager.OnItemCrafted -= ItemCraftUtil.OnItemCrafted;
            CraftingManager.OnFormulaUnlocked -= ItemCraftUtil.OnFormulaUnlocked;
        }

        #endregion
    }
}