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
            data.missonGoalData = ParseModifiers(cols[6]);
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