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
            Item.onUseStatic += ItemCraftUtil.OnItemUsed;
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
            Item.onUseStatic -= ItemCraftUtil.OnItemUsed;
        }

        #endregion
    }
}