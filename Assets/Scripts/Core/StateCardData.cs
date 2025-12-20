using UnityEngine;
using System;
using System.Collections.Generic;

namespace Core
{
    [Serializable]
    public class DailyStatusData : EntityBase
    {
        [Header("文本信息")]
        public string id;              // 唯一ID，如 "WeightLoss_001"
        public string title;           // 今日状态：(减肥)
        public string subTitle;        // 副标题：我不是大卫戴
        public string storyText;       // 描述文本：站在镜子前...
        public string trendText;       // 今日倾向：不当大卫戴了...

        [Header("任务逻辑")]
        public string missionGoal;     // 任务目标描述：健康值变化 >= 5
        public float targetValue;      // 对应的数值：5 (方便代码做逻辑判断)

        [Header("奖励与后果")]
        public List<StatModifier> rewards;      // 成功后的奖励列表
        public List<StatModifier> penalties;    // 失败后的后果列表
    }
}