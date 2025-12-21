using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using Manager;

namespace UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("进度条引用")]
        public UIStatBar healthBar;
        public UIStatBar hungerBar;
        public UIStatBar moodBar;
        

        [Header("状态文本")]
        public Text moodStatusText; // 显示“烦躁”、“悠闲”等

        void Update()
        {
            // 1. 每帧从 StatManager 获取最新数据并刷新 UI
            if (StateManager.Instance != null)
            {
                hungerBar.UpdateDisplay(StateManager.Instance.hunger);
                moodBar.UpdateDisplay(StateManager.Instance.mood);
                healthBar.UpdateDisplay(StateManager.Instance.health);

                // 2. 更新心情状态标签
                moodStatusText.text = StateManager.Instance.GetMoodStatus();
            }
        }
    }
}