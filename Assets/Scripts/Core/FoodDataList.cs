using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public class FoodData : MonoBehaviour
    {
        public string foodName;
        public List<StatModifier> effects; // 饱腹感+28, 健康+6, 心情+4
    }
    
    public class FoodDataList : MonoBehaviour
    { 
        public List<FoodData> foodList = new List<FoodData>(); 
    }
    
    [CreateAssetMenu(fileName = "FoodDatabase", menuName = "Data/FoodDatabase")]
    public class FoodDatabase : ScriptableObject
    {
        public List<FoodData> foodList = new List<FoodData>();
    }
}