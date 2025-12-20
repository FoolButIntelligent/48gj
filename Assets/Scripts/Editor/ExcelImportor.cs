using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using Core;

public class CSVImporter
{
    [MenuItem("Tools/Import Food Data")]
    public static void ImportCSV()
    {
        // 设置 CSV 文件的路径
        string path = "Assets/Data/FoodData.csv";  // CSV 文件路径
        string[] lines = File.ReadAllLines(path);  // 读取所有行

        // 获取 CSV 文件所在的目录
        string directoryPath = Path.GetDirectoryName(path);

        // 创建 FoodDatabase 实例（这是一个 ScriptableObject）
        FoodDatabase db = AssetDatabase.LoadAssetAtPath<FoodDatabase>("Assets/Resources/FoodDatabase.asset");

        // 如果没有现成的 FoodDatabase，则创建一个新的实例
        if (db == null)
        {
            db = ScriptableObject.CreateInstance<FoodDatabase>();
            AssetDatabase.CreateAsset(db, "Assets/Resources/FoodDatabase.asset");
        }

        db.foodList.Clear();  // 清空原有数据

        // 解析每一行 CSV 数据
        for (int i = 1; i < lines.Length; i++)  // 从第二行开始，跳过标题行
        {
            string[] columns = lines[i].Split(',');  // 按逗号分割每列

            FoodData foodData = new FoodData();
            foodData.foodName = columns[0];  // 第一列是食物名称

            // 第二列是属性（例如 Hunger:28;Health:6），解析每个属性
            foodData.effects = ParseAttributes(columns[1]);

            db.foodList.Add(foodData);
        }

        // 保存修改后的 FoodDatabase.asset
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();

        Debug.Log("Food data imported to FoodDatabase.asset at " + "Assets/Resources/FoodDatabase.asset");
    }

    // 解析属性字段（例如 Hunger:28;Health:6;Mood:4）
    private static List<StatModifier> ParseAttributes(string raw)
    {
        List<StatModifier> list = new List<StatModifier>();

        // 使用分号分隔每个属性
        string[] effects = raw.Split(';');  
        foreach (var effect in effects)
        {
            string[] kv = effect.Split(':');  // 使用冒号分隔属性名和数值
            if (kv.Length == 2)  // 确保是有效的键值对
            {
                StatModifier mod = new StatModifier();
                mod.type = (StatType)System.Enum.Parse(typeof(StatType), kv[0]);  // 解析属性类型（例如 Hunger、Health）
                mod.value = float.Parse(kv[1]);  // 解析属性值
                list.Add(mod);
            }
        }

        return list;
    }
}

