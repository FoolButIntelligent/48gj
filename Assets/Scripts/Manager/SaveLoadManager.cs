 using UnityEngine;
 using System.IO;
 using Core;

 namespace Manager
{
    public class SaveLoadManager : MonoBehaviour
    {
        public FoodData foodData;  // 你要保存的 ScriptableObject 实例
        public MissionData missionData;
        
        // 保存数据为 JSON 文件
        public void SaveData(ScriptableObject gameData)
        {
            // 将 gameData 转换为 JSON 字符串
            string json = JsonUtility.ToJson(gameData);

            // 保存 JSON 字符串到文件
            string path = Path.Combine(Application.persistentDataPath, "gameData.json");
            File.WriteAllText(path, json);

            Debug.Log("Game data saved to " + path);
        }

        // 从 JSON 文件加载数据
        public void LoadData(ScriptableObject gameData)
        {
            string path = Path.Combine(Application.persistentDataPath, "gameData.json");

            if (File.Exists(path))
            {
                // 读取 JSON 文件内容
                string json = File.ReadAllText(path);

                // 使用 FromJsonOverwrite 将数据加载到 gameData 实例
                JsonUtility.FromJsonOverwrite(json, gameData);

                Debug.Log("Game data loaded from " + path);
            }
            else
            {
                Debug.LogWarning("No saved data found.");
            }
        }
    }

}