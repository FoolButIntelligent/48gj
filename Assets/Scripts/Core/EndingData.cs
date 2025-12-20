namespace Core
{
    [System.Serializable]
    public class EndingData 
    {
        public string title;
        public string content;     // 结算文案
        // 触发条件
        public float minHealth;    // 健康下限
        public float maxHealth;    // 健康上限
        public float minMood;      // 心情下限
        public float maxMood;      // 心情上限
    }
}