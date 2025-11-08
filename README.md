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
