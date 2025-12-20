using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public class EndingData 
    {
        public string endTitle;
        public string content;     // 结算文案
    } 
    [CreateAssetMenu(fileName = "EndingDatabase", menuName = "Data/EndingDatabase")]
    public class EndingDataBase : ScriptableObject
    {
        public List<EndingData> endings = new List<EndingData>();
    }
}