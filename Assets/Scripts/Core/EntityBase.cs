using System;
using UnityEngine;

namespace Core
{
    public class EntityBase : MonoBehaviour
    {
        [Serializable]
        public class StatModifier
        {
            public StatType type;    // 属性类型：健康、心情、饥饿
            public float value;      // 变化数值：+3, -6 等
        }

        public enum StatType
        {
            Health,     // 健康
            Mood,       // 心情
            Hunger       // 饥饿
        }
    }
}