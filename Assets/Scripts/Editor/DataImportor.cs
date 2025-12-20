using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Core;

public class DataImporter 
{
    [MenuItem("Tools/同步Excel数据")]
    public static void ImportMissions()
    {
        // 1. 定位 CSV 文件 (假设放在 Assets/Data/Missions.csv)
        string path = Application.dataPath + "/Data/Missions.csv";
        string[] lines = File.ReadAllLines(path);

        // 2. 加载或创建资源文件
        MissionDatabase db = AssetDatabase.LoadAssetAtPath<MissionDatabase>("Assets/Resources/MissionDatabase.asset");
        if (db == null) {
            db = ScriptableObject.CreateInstance<MissionDatabase>();
            AssetDatabase.CreateAsset(db, "Assets/Resources/MissionDatabase.asset");
        }
        db.missions.Clear();

        // 3. 解析每一行 (跳过表头)
        for (int i = 1; i < lines.Length; i++) {
            string[] cols = lines[i].Split(',');
            MissionData data = new MissionData();
            data.id = cols[0];
            data.title = cols[1];
            data.subTitle = cols[2];
            data.storyText = cols[3];
            data.trendText = cols[4];
            data.missionGoal = cols[5];
            data.missionGoalData = ParseModifiers(cols[6]);
            data.rewards = ParseModifiers(cols[7]);
            data.penalties = ParseModifiers(cols[8]);
            db.missions.Add(data);
        }

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        Debug.Log("Misson数据已成功同步至 Unity 资源！");
    }

    // 解析格式如 "Health:3;Mood:6" 的字符串
    private static List<StatModifier> ParseModifiers(string raw) {
        List<StatModifier> list = new List<StatModifier>();
        if (string.IsNullOrEmpty(raw)) return list;
        
        string[] pairs = raw.Split(';');
        foreach (var p in pairs) {
            string[] kv = p.Split(':');
            StatModifier mod = new StatModifier();
            mod.type = (StatType)System.Enum.Parse(typeof(StatType), kv[0]);
            mod.value = float.Parse(kv[1]);
            list.Add(mod);
        }
        return list;
    }
}

// public class DataImporter
// {
//     [MenuItem("Tools/Import Missions")]
//     public static void ImportMissions()
//     {
//         // 读取 CSV 文件
//         string path = Application.dataPath + "/Data/Missions.csv";
//         string[] lines = File.ReadAllLines(path); // 读取文件所有行
//
//         // 获取 CSV 文件所在的目录
//         string directoryPath = Path.GetDirectoryName(path); 
//
//         // 创建 MissionDatabase 实例（可以是一个 ScriptableObject）
//         MissionDatabase db = AssetDatabase.LoadAssetAtPath<MissionDatabase>("Assets/Resources/MissionDatabase.asset");
//
//         if (db == null)
//         {
//             db = ScriptableObject.CreateInstance<MissionDatabase>();
//             AssetDatabase.CreateAsset(db, "Assets/Resources/MissionDatabase.asset");
//         }
//
//         db.missions.Clear();
//
//         List<MissionData> missionList = new List<MissionData>();
//
//         // 遍历 CSV 数据
//         for (int i = 1; i < lines.Length; i++) // 跳过标题行
//         {
//             string[] cols = lines[i].Split(',');
//
//             MissionData data = new MissionData();
//              data.id = cols[0];
//              data.title = cols[1];
//              data.subTitle = cols[2];
//              data.storyText = cols[3];
//              data.trendText = cols[4];
//              data.missionGoal = cols[5];
//              data.missionGoalData = ParseModifiers(cols[6]);
//              data.rewards = ParseModifiers(cols[7]);
//              data.penalties = ParseModifiers(cols[8]);
//              missionList.Add(data);
//         }
//
//         // 将数据转换为 JSON 格式
//         string json = JsonUtility.ToJson(new Wrapper<MissionData> { items = missionList });
//
//         // 保存 JSON 到与 CSV 相同的目录
//         string jsonFilePath = Path.Combine(directoryPath, "Missions.json");
//         File.WriteAllText(jsonFilePath, json);
//
//         Debug.Log("Missions have been saved to JSON at " + jsonFilePath);
//
//         // 重新保存 ScriptableObject
//         AssetDatabase.SaveAssets();
//     }
//
//     // 用于包装 List 转换为 JSON 格式
//     [System.Serializable]
//     public class Wrapper<T>
//     {
//         public List<T> items;
//     }
//
//     // 假设这是解析修改器的函数
//     private static List<StatModifier> ParseModifiers(string raw)
//     {
//         List<StatModifier> list = new List<StatModifier>();
//
//         string[] pains = raw.Split(';');
//         foreach (var pain in pains)
//         {
//             string[] kv = pain.Split(':');
//             StatModifier mod = new StatModifier();
//             mod.type = (StatType)System.Enum.Parse(typeof(StatType), kv[0]); // 解析枚举类型
//             mod.value = float.Parse(kv[1]); // 解析浮动值
//             list.Add(mod);
//         }
//
//         return list;
//     }
// }


