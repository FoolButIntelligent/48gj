using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Core;

public class ExcelImporter
{
    [MenuItem("Tools/Import Food Data")]
    public static void ImportCSV()
    {
        string path = "Assets/Data/FoodData.csv"; // 你的 CSV 路径
        string[] lines = File.ReadAllLines(path);
        
        // 加载或创建 ScriptableObject
        FoodDatabase db = AssetDatabase.LoadAssetAtPath<FoodDatabase>("Assets/Resources/FoodDatabase.asset");
        if (db == null)
        {
            db = ScriptableObject.CreateInstance<FoodDatabase>();
            AssetDatabase.CreateAsset(db, "Assets/Resources/FoodDatabase.asset");
        }
        db.foodList.Clear();

        // 从第二行开始遍历 (跳过表头)
        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');
            FoodData data = new FoodData();
            data.foodName = columns[0];
            data.effects = ParseEffects(columns[1]); // 解析复杂字段
            db.foodList.Add(data);
        }

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        Debug.Log("Food数据导入完成！");
    }

    // 解析 Hunger:28;Health:6 这种格式的字符串
    private static List<StatModifier> ParseEffects(string raw)
    {
        List<StatModifier> list = new List<StatModifier>();
        string[] parts = raw.Split(';');
        foreach (var p in parts)
        {
            string[] kv = p.Split(':');
            StatModifier mod = new StatModifier();
            mod.type = (StatType)System.Enum.Parse(typeof(StatType), kv[0]);
            mod.value = float.Parse(kv[1]);
            list.Add(mod);
        }
        return list;
    }
}
