using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Manager;

namespace Manager
{
    /// <summary>
    /// 存档数据结构
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int currentDay;
        public GamePhase currentPhase;
        
        // 玩家属性
        public float healthValue;
        public float moodValue;
        public float hungerValue;
        
        // 当前状态卡片ID
        public string currentStateCardId;
        
        // 游戏统计
        public int tasksCompleted;
        public int tasksFailed;
        
        // 时间戳
        public string saveTime;
        
        public SaveData()
        {
            saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    /// <summary>
    /// 存档管理器 - 使用Newtonsoft.Json序列化
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager instance;
        public static SaveManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("SaveManager");
                    instance = go.AddComponent<SaveManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        [Header("存档设置")]
        [SerializeField] private string saveFileName = "savegame.json";
        [SerializeField] private bool prettyPrint = true;

        private string SaveFilePath => Path.Combine(Application.persistentDataPath, saveFileName);

        public event Action<SaveData> OnGameLoaded;
        public event Action OnGameSaved;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 保存游戏
        /// </summary>
        public void SaveGame(SaveData data)
        {
            try
            {
                // 使用Newtonsoft.Json序列化
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    Formatting = prettyPrint ? Formatting.Indented : Formatting.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                string json = JsonConvert.SerializeObject(data, settings);
                File.WriteAllText(SaveFilePath, json);

                Debug.Log($"游戏已保存到: {SaveFilePath}");
                OnGameSaved?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"保存游戏失败: {e.Message}");
            }
        }

        /// <summary>
        /// 加载游戏
        /// </summary>
        public SaveData LoadGame()
        {
            try
            {
                if (!File.Exists(SaveFilePath))
                {
                    Debug.LogWarning("存档文件不存在");
                    return null;
                }

                string json = File.ReadAllText(SaveFilePath);
                SaveData data = JsonConvert.DeserializeObject<SaveData>(json);

                Debug.Log($"游戏已加载: {data.saveTime}");
                OnGameLoaded?.Invoke(data);
                
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"加载游戏失败: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 检查是否存在存档
        /// </summary>
        public bool HasSaveFile()
        {
            return File.Exists(SaveFilePath);
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public void DeleteSave()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath);
                    Debug.Log("存档已删除");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"删除存档失败: {e.Message}");
            }
        }

        /// <summary>
        /// 获取存档信息（不完全加载）
        /// </summary>
        public SaveData GetSaveInfo()
        {
            try
            {
                if (!File.Exists(SaveFilePath))
                    return null;

                string json = File.ReadAllText(SaveFilePath);
                return JsonConvert.DeserializeObject<SaveData>(json);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 导出存档到指定路径
        /// </summary>
        public void ExportSave(string exportPath)
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    File.Copy(SaveFilePath, exportPath, true);
                    Debug.Log($"存档已导出到: {exportPath}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"导出存档失败: {e.Message}");
            }
        }

        /// <summary>
        /// 从指定路径导入存档
        /// </summary>
        public void ImportSave(string importPath)
        {
            try
            {
                if (File.Exists(importPath))
                {
                    File.Copy(importPath, SaveFilePath, true);
                    Debug.Log("存档已导入");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"导入存档失败: {e.Message}");
            }
        }
    }

    /// <summary>
    /// 游戏存档助手 - 连接游戏系统和存档系统
    /// </summary>
    public class GameSaveHelper : MonoBehaviour
    {
        [Header("依赖")]
        [SerializeField] private GameFlowManager flowManager;
        [SerializeField] private PlayerAttributeManager attributeManager;

        [Header("自动保存")]
        [SerializeField] private bool autoSave = true;
        [SerializeField] private float autoSaveInterval = 300f; // 5分钟

        private float autoSaveTimer = 0f;

        private void Start()
        {
            if (flowManager == null)
                flowManager = FindFirstObjectByType<GameFlowManager>();
            
            if (attributeManager == null)
                attributeManager = FindFirstObjectByType<PlayerAttributeManager>();
        }

        private void Update()
        {
            if (autoSave)
            {
                autoSaveTimer += Time.deltaTime;
                if (autoSaveTimer >= autoSaveInterval)
                {
                    autoSaveTimer = 0f;
                    QuickSave();
                }
            }
        }

        /// <summary>
        /// 快速保存
        /// </summary>
        public void QuickSave()
        {
            SaveData data = CreateSaveData();
            SaveManager.Instance.SaveGame(data);
        }

        /// <summary>
        /// 快速加载
        /// </summary>
        public void QuickLoad()
        {
            SaveData data = SaveManager.Instance.LoadGame();
            if (data != null)
            {
                ApplySaveData(data);
            }
        }

        /// <summary>
        /// 创建存档数据
        /// </summary>
        private SaveData CreateSaveData()
        {
            SaveData data = new SaveData
            {
                currentDay = flowManager.CurrentDay,
                currentPhase = flowManager.CurrentPhase,
                healthValue = attributeManager.Health.CurrentValue,
                moodValue = attributeManager.Mood.CurrentValue,
                hungerValue = attributeManager.Hunger.CurrentValue,
                currentStateCardId = flowManager.CurrentStateCard?.Id ?? "",
            };

            return data;
        }

        /// <summary>
        /// 应用存档数据
        /// </summary>
        private void ApplySaveData(SaveData data)
        {
            // 恢复属性值
            attributeManager.Health.SetValue(data.healthValue);
            attributeManager.Mood.SetValue(data.moodValue);
            attributeManager.Hunger.SetValue(data.hungerValue);

            // TODO: 恢复游戏流程状态
            // 这需要在GameFlowManager中添加状态恢复方法

            Debug.Log($"存档已加载 - 第{data.currentDay}天，{data.saveTime}");
        }

        /// <summary>
        /// 检查是否有存档
        /// </summary>
        public bool HasSaveGame()
        {
            return SaveManager.Instance.HasSaveFile();
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public void DeleteSaveGame()
        {
            SaveManager.Instance.DeleteSave();
        }
    }
}