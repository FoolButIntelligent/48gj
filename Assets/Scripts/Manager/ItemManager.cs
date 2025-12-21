using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Core;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    public List<FoodData> foods = new List<FoodData>();
    [SerializeField]
    public List<MissionData> missions = new List<MissionData>();
    [SerializeField]
    public List<EndingData> endings = new List<EndingData>();

    public string FoodFilePath = "Assets/Data/FoodData.csv";
    public string MissionFilePath = "Assets/Data/MissionData.csv";
    public string EndingFilePath = "Assets/Data/EndingData.csv";

    // 读取 CSV 文件并填充 FoodData 列表
   public void LoadFoodData()
{
    if (File.Exists(FoodFilePath))
    {
        string[] lines = File.ReadAllLines(FoodFilePath);

        for (int i = 1; i < lines.Length; i++) // 按行读取数据
        {
            string[] columns = lines[i].Split(','); // 每行用逗号分隔（假设 CSV 是逗号分隔）

            if (columns.Length == 2)
            {
                string foodName = columns[0].Trim();  // 第一列为食物名称
                string[] effectsData = columns[1].Split(';');  // 第二列为属性数据

                FoodData newFood = new FoodData
                {
                    foodName = foodName,
                    effects = new List<StatModifier>()
                };

                // 解析属性效果（例如: Hunger:28;Health:6;Mood:4）
                foreach (string effect in effectsData)
                {
                    string[] effectParts = effect.Split(':');
                    if (effectParts.Length == 2)
                    {
                        string effectType = effectParts[0].Trim();  // 获取类型名，例如 Hunger
                        string effectValue = effectParts[1].Trim();  // 获取值，例如 28

                        // 尝试解析类型并添加到 effects 列表
                        StatType statType;
                        if (Enum.TryParse(effectType, true, out statType))
                        {
                            StatModifier stat = new StatModifier
                            {
                                type = statType,
                                value = float.Parse(effectValue)
                            };
                            newFood.effects.Add(stat);
                        }
                        else
                        {
                            Debug.LogWarning($"无法解析属性类型: {effectType}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"数据格式错误: {effect}，应为 '类型:值' 格式。");
                    }
                }

                foods.Add(newFood); // 将解析好的数据添加到 foods 列表
            }
            else
            {
                Debug.LogWarning($"无法解析行数据: {lines[i]}，应该包含两个字段（食物名称和属性数据）。");
            }
        }
    }
    else
    {
        Debug.LogError("Food data file not found at path: " + FoodFilePath);
    }
}



   public void LoadMissionData() 
   {
    if (File.Exists(MissionFilePath))
    {
        string[] lines = File.ReadAllLines(MissionFilePath);

        for (int i = 1; i < lines.Length; i++) // 跳过标题行
        {
            string[] columns = lines[i].Split(',');

            if (columns.Length >= 9) // 确保每行有足够的列
            {
                MissionData newMission = new MissionData
                {
                    id = columns[0].Trim(),  // 获取ID
                    title = columns[1].Trim(),  // 获取标题
                    subTitle = columns[2].Trim(),  // 获取副标题
                    storyText = columns[3].Trim(),  // 获取故事文本
                    trendText = columns[4].Trim(),  // 获取趋势文本
                    missionGoal = columns[5].Trim(),  // 获取任务目标
                    missionGoalData = new List<StatModifier>(), // 任务目标数据
                    rewards = new List<StatModifier>(),  // 奖励数据
                    penalties = new List<StatModifier>()  // 惩罚数据
                };

                // 解析 rewards
                string[] rewardList = columns[7].Split(':');
                if (rewardList.Length > 1)
                {
                    string[] rewardDetails = rewardList[1].Split(',');
                    foreach (string reward in rewardDetails)
                    {
                        string[] rewardParts = reward.Split(':');
                        if (rewardParts.Length == 2)
                        {
                            StatModifier rewardStat = new StatModifier
                            {
                                type = (StatType)Enum.Parse(typeof(StatType), rewardParts[0].Trim(), true),
                                value = float.Parse(rewardParts[1].Trim())
                            };
                            newMission.rewards.Add(rewardStat);
                        }
                    }
                }

                // 解析 penalties
                string[] penaltyList = columns[8].Split(':');
                if (penaltyList.Length > 1)
                {
                    string[] penaltyDetails = penaltyList[1].Split(',');
                    foreach (string penalty in penaltyDetails)
                    {
                        string[] penaltyParts = penalty.Split(':');
                        if (penaltyParts.Length == 2)
                        {
                            StatModifier penaltyStat = new StatModifier
                            {
                                type = (StatType)Enum.Parse(typeof(StatType), penaltyParts[0].Trim(), true),
                                value = float.Parse(penaltyParts[1].Trim())
                            };
                            newMission.penalties.Add(penaltyStat);
                        }
                    }
                }

                // 将新的 MissionData 添加到 missions 列表
                missions.Add(newMission);
            }
            else
            {
                Debug.LogWarning($"数据格式错误: {lines[i]}，请确保每行数据格式正确。");
            }
        }
    }
    else
    {
        Debug.LogError("Mission data file not found at path: " + MissionFilePath);
    }
    }


    // 读取 Ending 数据
    // public void LoadEndingData()
    // {
    //     if (File.Exists(EndingFilePath))
    //     {
    //         string[] lines = File.ReadAllLines(EndingFilePath);
    //         for (int i = 1; i < lines.Length; i++) // Skip header line
    //         {
    //             string[] columns = lines[i].Split(';');
    //             EndingData newEnding = new EndingData
    //             {
    //                 endTitle = columns[0],
    //                 content = columns[1]
    //             };
    //
    //             endings.Add(newEnding);
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("Ending data file not found at path: " + EndingFilePath);
    //     }
    // }

    // 专门打印加载的数据
    public void PrintLoadedData()
    {
        // 打印 FoodData 数据
        Debug.Log("Food Data Loaded:");
        foreach (FoodData food in foods)
        {
            Debug.Log($"Food Name: {food.foodName}");
            foreach (var effect in food.effects)
            {
                Debug.Log($"Effect - {effect.type}: {effect.value}");
            }
        }

        // 打印 MissionData 数据
        Debug.Log("Mission Data Loaded:");
        foreach (MissionData mission in missions)
        {
            Debug.Log($"Mission ID: {mission.id}, Title: {mission.title}");
            Debug.Log($"Goal: {mission.missionGoal}");
            Debug.Log("Rewards:");
            foreach (var reward in mission.rewards)
            {
                Debug.Log($"- {reward.type}: {reward.value}");
            }
            Debug.Log("Penalties:");
            foreach (var penalty in mission.penalties)
            {
                Debug.Log($"- {penalty.type}: {penalty.value}");
            }
        }

        // // 打印 EndingData 数据
        // Debug.Log("Ending Data Loaded:");
        // foreach (EndingData ending in endings)
        // {
        //     Debug.Log($"Ending Title: {ending.endTitle}, Content: {ending.content}");
        // }
    }

    // Start method to load all data at once and print it
    private void Start()
    {
        LoadFoodData();
        LoadMissionData();
        //LoadEndingData();
        PrintLoadedData();  // 打印所有加载的数据
    }
}

