using System;
using Core;
using UnityEngine;

namespace Manager
{
    public class StateManager :MonoBehaviour
    {
        //单例
        public static StateManager Instance;
        
        private void Awake()
        {
            // 确保管理类在切换场景时不被销毁
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        [Header("实时数值0-100")]
        public float health = 70f;   // 初始值 70
        public float mood = 50f;     // 初始值 50
        public float hunger = 50f;   // 饱腹感初始值 50

        private float fiveSecTimer = 0f;
        private float tenSecTimer = 0f;
        void Update()
        {
            float deltaTime = Time.deltaTime;
            fiveSecTimer += deltaTime;
            tenSecTimer += deltaTime;

            // 每5秒触发：饱腹感自然下降与饥饿效果
            if (fiveSecTimer >= 5f)
            {
                ApplyNaturalDecay();
                CheckFiveSecEffects();
                fiveSecTimer = 0f;
            }

            // 每10秒触发：开心、过饱效果
            if (tenSecTimer >= 10f)
            {
                CheckTenSecEffects();
                tenSecTimer = 0f;
            }
            
            //FoodEffects();
        }
        
        private void ApplyNaturalDecay()
        {
            // 饱腹感每5秒: -1
            ApplyModifier(StatType.Hunger, -1f);
        }

        private void CheckFiveSecEffects()
        {
            // 0-25 饥饿：心情每5秒 -1
            if (hunger <= 25f)
            {
                ApplyModifier(StatType.Mood, -1f);
            }
        }

        private void CheckTenSecEffects()
        {
            // 65-85 开心：心情每10秒 +1
            if (hunger > 65f && hunger <= 85f)
            {
                ApplyModifier(StatType.Mood, 1f);
            }
            // 85-100 过饱：心情每10秒 -2，健康每10秒 -1
            else if (hunger > 85f)
            {
                ApplyModifier(StatType.Mood, -2f);
                ApplyModifier(StatType.Health, -1f);
            }
        }

        public void ApplyModifier(StatType type, float value)
        {
            switch (type)
            {
                case StatType.Health: health = Mathf.Clamp(health + value, 0, 100); break;
                case StatType.Mood:   mood = Mathf.Clamp(mood + value, 0, 100); break;
                case StatType.Hunger: hunger = Mathf.Clamp(hunger + value, 0, 100); break;
            }
        }

        // 获取心情描述文本
        public string GetMoodStatus()
        {
            if (mood <= 20) return "烦躁";
            if (mood <= 40) return "低落";
            if (mood <= 60) return "悠闲";
            if (mood <= 80) return "开心";
            return "狂喜";
        }
    }
}