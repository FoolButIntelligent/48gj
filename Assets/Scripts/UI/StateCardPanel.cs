using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Core;

namespace UI
{
    /// <summary>
    /// 状态卡片UI面板
    /// </summary>
    public class StateCardPanel : DataDrivenUIPanel<StateCardConfig>
    {
        [Header("UI组件")]
        [SerializeField] private Image cardIcon;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI goalText;
        [SerializeField] private TextMeshProUGUI requirementText;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private Button confirmButton;

        public event Action OnConfirmed;

        protected override void Awake()
        {
            base.Awake();

            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(() => OnConfirmed?.Invoke());
            }
        }

        protected override void UpdateUI()
        {
            if (currentData == null)
            {
                Debug.LogWarning("StateCardPanel: 数据为空!");
                return;
            }

            // 更新图标
            if (cardIcon != null && currentData.Icon != null)
            {
                cardIcon.sprite = currentData.Icon;
            }

            // 更新标题
            if (titleText != null)
            {
                titleText.text = currentData.DisplayName;
            }

            // 更新描述
            if (descriptionText != null)
            {
                descriptionText.text = currentData.Description;
            }

            // 更新今日目标
            if (goalText != null)
            {
                goalText.text = $"今日倾向：{currentData.TodayGoal}";
            }

            // 更新任务要求
            if (requirementText != null)
            {
                requirementText.text = FormatRequirements(currentData.TaskRequirements);
            }

            // 更新奖励
            if (rewardText != null)
            {
                rewardText.text = FormatRewards(currentData.CompletionRewards);
            }
        }

        private string FormatRequirements(System.Collections.Generic.List<AttributeEffect> requirements)
        {
            if (requirements == null || requirements.Count == 0)
                return "无特殊要求";

            string result = "任务：";
            foreach (var req in requirements)
            {
                string attrName = GetAttributeName(req.AttributeType);
                string change = req.Value >= 0 ? $"≥{req.Value}" : $"{req.Value}";
                result += $"{attrName}变化{change} ";
            }
            return result;
        }

        private string FormatRewards(System.Collections.Generic.List<AttributeEffect> rewards)
        {
            if (rewards == null || rewards.Count == 0)
                return "无奖励";

            string result = "达成奖励：";
            foreach (var reward in rewards)
            {
                string attrName = GetAttributeName(reward.AttributeType);
                string change = reward.Value >= 0 ? $"+{reward.Value}" : $"{reward.Value}";
                result += $"{attrName}{change} ";
            }
            return result;
        }

        private string GetAttributeName(AttributeType type)
        {
            switch (type)
            {
                case AttributeType.Health: return "健康";
                case AttributeType.Mood: return "心情";
                case AttributeType.Hunger: return "饱腹感";
                default: return type.ToString();
            }
        }
    }
}
