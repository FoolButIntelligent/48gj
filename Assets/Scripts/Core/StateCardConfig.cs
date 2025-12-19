using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// 状态类型
    /// </summary>
    public enum StateType
    {
        Dieting,        // 减肥
        NoExercise,     // 不想锻炼
        TooTired        // 最近太累了
    }

    /// <summary>
    /// 状态卡片配置 - ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "StateCard", menuName = "TakeoutSimulator/StateCard")]
    public class StateCardConfig : GameDataConfig
    {
        [Header("状态信息")]
        [SerializeField] private StateType stateType;
        [SerializeField] [TextArea(3, 5)] private string description;
        [SerializeField] [TextArea(2, 3)] private string todayGoal;

        [Header("任务要求")]
        [SerializeField] private List<AttributeEffect> taskRequirements = new List<AttributeEffect>();

        [Header("奖励")]
        [SerializeField] private List<AttributeEffect> completionRewards = new List<AttributeEffect>();

        [Header("后果（可选）")]
        [SerializeField] private List<AttributeEffect> failurePenalties = new List<AttributeEffect>();

        public StateType StateType => stateType;
        public string Description => description;
        public string TodayGoal => todayGoal;
        public List<AttributeEffect> TaskRequirements => taskRequirements;
        public List<AttributeEffect> CompletionRewards => completionRewards;
        public List<AttributeEffect> FailurePenalties => failurePenalties;

        public override void Validate()
        {
            if (string.IsNullOrEmpty(id))
                Debug.LogError($"StateCard {name} 缺少 ID!");
            if (string.IsNullOrEmpty(description))
                Debug.LogWarning($"StateCard {name} 缺少描述!");
        }
    }
}
