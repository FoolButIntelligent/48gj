using System.Collections.Generic;

namespace Core
{
    [System.Serializable]
    public class FoodData : EntityBase
    {
        public string foodName;
        public List<StatModifier> effects; // 饱腹感+28, 健康+6, 心情+4
    }
}