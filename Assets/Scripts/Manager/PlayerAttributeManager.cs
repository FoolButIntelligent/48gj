using System;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Manager
{
    /// <summary>
    /// 玩家属性管理器 - 管理所有玩家属性和状态逻辑
    /// </summary>
    public class PlayerAttributeManager : MonoBehaviour
    {
        [Header("初始属性设置")]
        [SerializeField] private float initialHealth = 70f;
        [SerializeField] private float initialMood = 50f;
        [SerializeField] private float initialHunger = 40f;

        private Dictionary<AttributeType, AttributeBase> attributes;
        private MoodState currentMoodState;
        private HungerState currentHungerState;

        public event Action<AttributeType, float, float> OnAttributeChanged;
        public event Action<MoodState> OnMoodStateChanged;
        public event Action<HungerState> OnHungerStateChanged;

        // 属性访问器
        public AttributeBase Health => attributes[AttributeType.Health];
        public AttributeBase Mood => attributes[AttributeType.Mood];
        public AttributeBase Hunger => attributes[AttributeType.Hunger];
        public MoodState CurrentMoodState => currentMoodState;
        public HungerState CurrentHungerState => currentHungerState;

        private void Awake()
        {
            InitializeAttributes();
        }

        private void Start()
        {
            StartHungerDecay();
        }

        /// <summary>
        /// 初始化属性
        /// </summary>
        private void InitializeAttributes()
        {
            attributes = new Dictionary<AttributeType, AttributeBase>
            {
                { AttributeType.Health, new AttributeBase(AttributeType.Health, 0f, 100f, initialHealth) },
                { AttributeType.Mood, new AttributeBase(AttributeType.Mood, -50f, 50f, initialMood) },
                { AttributeType.Hunger, new AttributeBase(AttributeType.Hunger, 0f, 100f, initialHunger) }
            };

            // 订阅属性变化事件
            foreach (var kvp in attributes)
            {
                kvp.Value.OnValueChanged += (oldVal, newVal) => 
                {
                    OnAttributeChanged?.Invoke(kvp.Key, oldVal, newVal);
                    CheckAttributeStates(kvp.Key);
                };
            }

            UpdateMoodState();
            UpdateHungerState();
        }

        /// <summary>
        /// 应用属性效果列表
        /// </summary>
        public void ApplyEffects(List<AttributeEffect> effects)
        {
            if (effects == null || effects.Count == 0) return;

            foreach (var effect in effects)
            {
                ApplyEffect(effect);
            }
        }

        /// <summary>
        /// 应用单个属性效果
        /// </summary>
        public void ApplyEffect(AttributeEffect effect)
        {
            if (!attributes.ContainsKey(effect.AttributeType)) return;

            var attribute = attributes[effect.AttributeType];
            float value = effect.IsPercentage 
                ? attribute.MaxValue * effect.Value / 100f 
                : effect.Value;

            attribute.ModifyValue(value);
        }

        /// <summary>
        /// 获取属性当前值
        /// </summary>
        public float GetAttributeValue(AttributeType type)
        {
            return attributes.ContainsKey(type) ? attributes[type].CurrentValue : 0f;
        }

        /// <summary>
        /// 检查属性状态变化
        /// </summary>
        private void CheckAttributeStates(AttributeType type)
        {
            switch (type)
            {
                case AttributeType.Mood:
                    UpdateMoodState();
                    break;
                case AttributeType.Hunger:
                    UpdateHungerState();
                    break;
            }
        }

        /// <summary>
        /// 更新心情状态
        /// </summary>
        private void UpdateMoodState()
        {
            MoodState newState;
            float moodValue = Mood.CurrentValue;

            if (moodValue <= -30f)
                newState = MoodState.Upset;
            else if (moodValue < 0f)
                newState = MoodState.Low;
            else if (moodValue < 30f)
                newState = MoodState.Calm;
            else
                newState = MoodState.Happy;

            if (newState != currentMoodState)
            {
                currentMoodState = newState;
                OnMoodStateChanged?.Invoke(currentMoodState);
                Debug.Log($"心情状态变化: {currentMoodState}");
            }
        }

        /// <summary>
        /// 更新饱腹感状态
        /// </summary>
        private void UpdateHungerState()
        {
            HungerState newState;
            float hungerValue = Hunger.CurrentValue;

            if (hungerValue < 30f)
                newState = HungerState.Starving;
            else if (hungerValue < 70f)
                newState = HungerState.Satisfied;
            else if (hungerValue < 90f)
                newState = HungerState.Happy;
            else
                newState = HungerState.Overfed;

            if (newState != currentHungerState)
            {
                HungerState oldState = currentHungerState;
                currentHungerState = newState;
                OnHungerStateChanged?.Invoke(currentHungerState);
                
                // 根据饱腹感状态影响心情
                ApplyHungerStateEffect(oldState, currentHungerState);
                
                Debug.Log($"饱腹感状态变化: {currentHungerState}");
            }
        }

        /// <summary>
        /// 应用饱腹感状态效果到心情和健康
        /// </summary>
        private void ApplyHungerStateEffect(HungerState oldState, HungerState newState)
        {
            // 饥饿状态：心情每10秒-1
            if (newState == HungerState.Starving)
            {
                if (oldState != HungerState.Starving)
                {
                    InvokeRepeating(nameof(ApplyStarvingEffect), 10f, 10f);
                }
            }
            else
            {
                CancelInvoke(nameof(ApplyStarvingEffect));
            }

            // 开心状态：心情每20秒+1
            if (newState == HungerState.Happy)
            {
                if (oldState != HungerState.Happy)
                {
                    InvokeRepeating(nameof(ApplyHappyEffect), 20f, 20f);
                }
            }
            else
            {
                CancelInvoke(nameof(ApplyHappyEffect));
            }

            // 过饱状态：健康-2
            if (newState == HungerState.Overfed && oldState != HungerState.Overfed)
            {
                Health.ModifyValue(-2f);
            }
        }

        private void ApplyStarvingEffect()
        {
            Mood.ModifyValue(-1f);
        }

        private void ApplyHappyEffect()
        {
            Mood.ModifyValue(1f);
        }

        /// <summary>
        /// 开始饱腹感衰减（每10秒-1）
        /// </summary>
        private void StartHungerDecay()
        {
            InvokeRepeating(nameof(DecayHunger), 10f, 10f);
        }

        private void DecayHunger()
        {
            Hunger.ModifyValue(-1f);
        }

        /// <summary>
        /// 重置所有属性
        /// </summary>
        public void ResetAllAttributes()
        {
            foreach (var attr in attributes.Values)
            {
                attr.Reset();
            }
        }

        private void OnDestroy()
        {
            CancelInvoke();
        }
    }
}
