using System;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Manager
{
    /// <summary>
    /// 游戏阶段
    /// </summary>
    public enum GamePhase
    {
        DayStart,       // 一天开始
        ViewStateCard,  // 查看状态卡片
        OrderFood,      // 点外卖
        CheckResult,    // 反馈结果
        DayEnd          // 一天结束
    }

    /// <summary>
    /// 游戏流程管理器 - 管理3天循环和游戏结算
    /// </summary>
    public class GameFlowManager : MonoBehaviour
    {
        [Header("依赖")]
        [SerializeField] private PlayerAttributeManager attributeManager;

        [Header("状态卡片池")]
        [SerializeField] private List<StateCardConfig> stateCardPool = new List<StateCardConfig>();

        [Header("游戏设置")]
        [SerializeField] private int totalDays = 3;

        private int currentDay = 1;
        private GamePhase currentPhase;
        private StateCardConfig currentStateCard;
        private List<FoodItemConfig> orderedFood = new List<FoodItemConfig>();

        public event Action<int> OnDayStarted;
        public event Action<StateCardConfig> OnStateCardPresented;
        public event Action<GamePhase> OnPhaseChanged;
        public event Action<bool, string> OnDayEnded; // 参数：是否完成任务, 结果文本
        public event Action<GameEndType, string> OnGameEnded; // 游戏结束

        public int CurrentDay => currentDay;
        public GamePhase CurrentPhase => currentPhase;
        public StateCardConfig CurrentStateCard => currentStateCard;

        private void Start()
        {
            if (attributeManager == null)
            {
                attributeManager = FindObjectOfType<PlayerAttributeManager>();
            }

            StartNewDay();
        }

        /// <summary>
        /// 开始新的一天
        /// </summary>
        public void StartNewDay()
        {
            orderedFood.Clear();
            ChangePhase(GamePhase.DayStart);
            OnDayStarted?.Invoke(currentDay);

            Debug.Log($"=== 第 {currentDay} 天开始 ===");

            // 抽取状态卡片
            DrawStateCard();
        }

        /// <summary>
        /// 抽取状态卡片
        /// </summary>
        private void DrawStateCard()
        {
            if (stateCardPool.Count == 0)
            {
                Debug.LogError("状态卡片池为空!");
                return;
            }

            // 随机抽取
            currentStateCard = stateCardPool[UnityEngine.Random.Range(0, stateCardPool.Count)];
            
            ChangePhase(GamePhase.ViewStateCard);
            OnStateCardPresented?.Invoke(currentStateCard);

            Debug.Log($"今日状态: {currentStateCard.DisplayName}");
            Debug.Log($"描述: {currentStateCard.Description}");
            Debug.Log($"目标: {currentStateCard.TodayGoal}");
        }

        /// <summary>
        /// 确认状态卡片，进入点餐阶段
        /// </summary>
        public void ConfirmStateCard()
        {
            ChangePhase(GamePhase.OrderFood);
        }

        /// <summary>
        /// 点餐 - 选择食物
        /// </summary>
        public void OrderFood(FoodItemConfig food)
        {
            if (currentPhase != GamePhase.OrderFood)
            {
                Debug.LogWarning("当前不在点餐阶段!");
                return;
            }

            orderedFood.Add(food);
            
            // 应用食物效果
            attributeManager.ApplyEffects(food.AttributeEffects);

            Debug.Log($"点了: {food.DisplayName}");
        }

        /// <summary>
        /// 确认点餐，进入结算阶段
        /// </summary>
        public void ConfirmOrder()
        {
            ChangePhase(GamePhase.CheckResult);
            CheckDayResult();
        }

        /// <summary>
        /// 检查当天结果
        /// </summary>
        private void CheckDayResult()
        {
            bool taskCompleted = CheckTaskCompletion();
            string resultText = "";

            if (taskCompleted)
            {
                // 应用完成奖励
                attributeManager.ApplyEffects(currentStateCard.CompletionRewards);
                resultText = "任务完成！获得奖励。";
                Debug.Log("✓ 任务完成!");
            }
            else
            {
                // 应用失败惩罚
                attributeManager.ApplyEffects(currentStateCard.FailurePenalties);
                resultText = "任务失败...";
                Debug.Log("✗ 任务失败");
            }

            OnDayEnded?.Invoke(taskCompleted, resultText);
        }

        /// <summary>
        /// 检查任务是否完成
        /// </summary>
        private bool CheckTaskCompletion()
        {
            foreach (var requirement in currentStateCard.TaskRequirements)
            {
                float currentValue = attributeManager.GetAttributeValue(requirement.AttributeType);
                
                // 检查是否满足要求
                if (requirement.Value >= 0) // 正值表示需要达到的最小值
                {
                    if (currentValue < requirement.Value)
                        return false;
                }
                else // 负值表示需要达到的变化量
                {
                    // 这里可以根据需求实现变化量检测逻辑
                }
            }

            return true;
        }

        /// <summary>
        /// 进入下一天或结束游戏
        /// </summary>
        public void ProceedToNextDay()
        {
            ChangePhase(GamePhase.DayEnd);

            currentDay++;

            if (currentDay > totalDays)
            {
                EndGame();
            }
            else
            {
                StartNewDay();
            }
        }

        /// <summary>
        /// 游戏结束检查
        /// </summary>
        private void EndGame()
        {
            GameEndType endType = DetermineGameEnding();
            string endMessage = GetEndingMessage(endType);

            OnGameEnded?.Invoke(endType, endMessage);
            Debug.Log($"=== 游戏结束 ===");
            Debug.Log($"结局: {endType}");
            Debug.Log(endMessage);
        }

        /// <summary>
        /// 判断游戏结局
        /// </summary>
        private GameEndType DetermineGameEnding()
        {
            float health = attributeManager.Health.CurrentValue;
            float mood = attributeManager.Mood.CurrentValue;

            // 结局一：养生"医"家上线
            if (health >= 60 && mood <= 0)
            {
                return GameEndType.HealthyDoctor;
            }
            // 结局二：暴吃太溺差
            else if (health < 50 && mood >= 20)
            {
                return GameEndType.Overeater;
            }
            // 其他结局...
            else
            {
                return GameEndType.Default;
            }
        }

        /// <summary>
        /// 获取结局消息
        /// </summary>
        private string GetEndingMessage(GameEndType endType)
        {
            switch (endType)
            {
                case GameEndType.HealthyDoctor:
                    return "小U最近日子过得很淡，健康在线，心情待机~";
                case GameEndType.Overeater:
                    return "小U暴吃太溺差，身体健康堪忧...";
                default:
                    return "游戏结束";
            }
        }

        /// <summary>
        /// 改变游戏阶段
        /// </summary>
        private void ChangePhase(GamePhase newPhase)
        {
            currentPhase = newPhase;
            OnPhaseChanged?.Invoke(currentPhase);
        }

        /// <summary>
        /// 添加状态卡片到卡池
        /// </summary>
        public void AddStateCardToPool(StateCardConfig card)
        {
            if (!stateCardPool.Contains(card))
            {
                stateCardPool.Add(card);
            }
        }
    }

    /// <summary>
    /// 游戏结局类型
    /// </summary>
    public enum GameEndType
    {
        HealthyDoctor,  // 养生"医"家上线
        Overeater,      // 暴吃太溺差
        Default         // 默认结局
    }
}
