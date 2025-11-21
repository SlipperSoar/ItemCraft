# 逃离鸭科夫物品合成MOD

## 介绍

- 能够自定义物品的合成配方，并通过制作台/医疗站/厨房进行合成
- 自带部分合成配方（如可可奶的合成）
- 支持模组物品

## 使用方法

将Mod文件放在Duckov_Data/Mods目录下的ItemCraft文件夹中

目录结构：
- ItemCraft/
  - ItemCraft.dll
  - info.ini
  - preview.png
  - *.craft.csv（可选，支持多个配方文件）
  - ItemInfo.txt（**运行后生成**，含所有道具的id和名称信息）
  - FormulaInfomations.txt（**运行后生成**，含所有已有合成配方的id和tags信息）

### 添加自定义合成配方

- 在Duckov_Data/Mods/ItemCraft目录下添加合成配方文件
- 配方文件名按以下格式命名：*.craft.csv
- 配方文件内容格式如下：
  - id: 配方唯一标识字符串
  - target: 要合成的目标物品，格式为“id:amount”
  - money: 要花费的金币数量
  - materials: 配方需要的材料列表，格式为“id:amount”，多个材料之间用“,”分割
  - unlockByDefault: 是否默认解锁，填写true/false（一般true即可）
  - tags: 配方的标签，作用是分配配方所属（以后有多个可以用“,”分割）
    - MedicStation：医疗站
    - WorkBenchAdvanced：制作台
    - Cook：厨房（需要安装mod开启厨房的烹饪交互）

配方文件内容示例：

| id | target | money | materials | unlockByDefault | tags |
| --- | --- | --- | --- | --- | --- |
| Craft_CocoMilk | 105:1 | 0 | 938:1,60:1 | true | WorkBenchAdvanced |

### 启用MOD注意事项

- 当配方中包含模组物品时，请将该Mod顺序放在其后
- 配方的id不可重复，如有重复id，除了第一个配方外均会被忽略
- 如果出现配方未生效问题，请安装控制台mod查看日志信息，其中会输出出错信息
  - 重复配方id：输出具体重复的配方id和对应的文件名
  - 配置错误：会输出具体报错信息和对应内容
- 使用`tab`(`\t`)以外的分隔符
  - 默认以`\t`分割列时，会以`,`分割材料列表（`materials`列），以`:`分割物品数量
  - 改以`,`分割列时，因为会与材料列表的分割**冲突**，所以改用` `(空格)来分割材料列表（`materials`列），以`:`分割物品数量
  - 还可以以` `(空格)或`;`(分号)来分割列，这两种都不会与其他分割逻辑冲突

### 其他MOD内如何通过该MOD添加配方

- 引用该MOD的dll作为依赖
- 调用方法`ItemCraft.ItemCraftUtil.AddCraftFormulas(List<CraftingFormula> formulas)`
  - 结构体`CraftingFormula`是官方的配方结构体
  - `id`不可与游戏内配方重复，也不可与其他mod配方重复（**除非你的MOD先加载**）
  - `result`是合成的目标物品，其中的物品`id`不可不存在
  - `Cost.items`是合成需要的材料，其中的物品`id`不可不存在
- （可选）调用方法`ItemCraft.LocalizeUtil.LoadModItemsInfo()`来（重新）初始化MOD相关物品信息，以便需要输出日志时可以显示其名称而不是id
- （可选）调用方法`ItemCraft.LocalizeUtil.Localize(string key, params object[] args)`来本地化显示一些日志，key可查看`ItemCraft.LocalizeUtil`的常量
- （可选）调用方法`ItemCraft.LocalizeUtil.LocalizeItem(int itemId)`来通过传递物品id从已缓存的物品信息中获取本地化物品名称（当然也可以通过官方的`ItemStatsSystem.ItemAssetsCollection.GetPrefab(int typeId)`来获取整个物品信息）
