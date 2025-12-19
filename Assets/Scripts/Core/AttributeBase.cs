using System;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// 属性类型枚举
    /// </summary>
    public enum AttributeType
    {
        Health,      // 健康值
        Mood,        // 心情值
        Hunger       // 饱腹感
    }

    /// <summary>
    /// 心情状态
    /// </summary>
    public enum MoodState
    {
        Upset,       // 烦躁 (<= -30)
        Low,         // 低落 (-30 ~ 0)
        Calm,        // 悠闲 (0 ~ 30)
        Happy        // 开心 (>= 30)
    }

    /// <summary>
    /// 饱腹感状态
    /// </summary>
    public enum HungerState
    {
        Starving,    // 饥饿 (0-30)
        Satisfied,   // 满足 (30-70)
        Happy,       // 开心 (70-90)
        Overfed      // 过饱 (90-100)
    }

    /// <summary>
    /// 属性值类 - 包含范围限制和变化事件
    /// </summary>
    [Serializable]
    public class AttributeBase
    {
        [SerializeField] private AttributeType type;
        [SerializeField] private float minValue;
        [SerializeField] private float maxValue;
        [SerializeField] private float currentValue;
        [SerializeField] private float initialValue;

        public event Action<float, float> OnValueChanged; // 参数：旧值, 新值
        public event Action<float> OnMinReached;
        public event Action<float> OnMaxReached;

        public AttributeType Type => type;
        public float MinValue => minValue;
        public float MaxValue => maxValue;
        public float CurrentValue => currentValue;
        public float Percentage => (currentValue - minValue) / (maxValue - minValue) * 100f;

        public AttributeBase(AttributeType type, float min, float max, float initial)
        {
            this.type = type;
            this.minValue = min;
            this.maxValue = max;
            this.initialValue = initial;
            this.currentValue = initial;
        }

        /// <summary>
        /// 修改属性值
        /// </summary>
        public void ModifyValue(float delta)
        {
            float oldValue = currentValue;
            currentValue = Mathf.Clamp(currentValue + delta, minValue, maxValue);

            if (!Mathf.Approximately(oldValue, currentValue))
            {
                OnValueChanged?.Invoke(oldValue, currentValue);

                if (Mathf.Approximately(currentValue, minValue))
                    OnMinReached?.Invoke(currentValue);
                else if (Mathf.Approximately(currentValue, maxValue))
                    OnMaxReached?.Invoke(currentValue);
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        public void SetValue(float value)
        {
            float delta = value - currentValue;
            ModifyValue(delta);
        }

        /// <summary>
        /// 重置到初始值
        /// </summary>
        public void Reset()
        {
            SetValue(initialValue);
        }
    }

    /// <summary>
    /// 属性效果 - 用于描述对属性的影响
    /// </summary>
    [Serializable]
    public class AttributeEffect
    {
        [SerializeField] private AttributeType attributeType;
        [SerializeField] private float value;
        [SerializeField] private bool isPercentage;

        public AttributeType AttributeType => attributeType;
        public float Value => value;
        public bool IsPercentage => isPercentage;

        public AttributeEffect(AttributeType type, float value, bool isPercentage = false)
        {
            this.attributeType = type;
            this.value = value;
            this.isPercentage = isPercentage;
        }
    }
}
