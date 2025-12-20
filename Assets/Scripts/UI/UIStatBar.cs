using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using Manager;

namespace UI
{
    public class UIStatBar : MonoBehaviour
    {
        public Slider slider;          // 进度条组件
        public TextMeshProUGUI valueText; // 显示数值的文本
        public Image fillImage;        // 进度条填充图（用于变色）

        // 更新显示
        public void UpdateDisplay(float current)
        {
            slider.value = current / 100f; // 假设 Slider 的 Max 为 1
            valueText.text = Mathf.RoundToInt(current).ToString(); // 显示整数
        
            // 可选：根据数值改变颜色
            if (current <= 20) fillImage.color = Color.red;
            //else fillImage.color = Color.white;
        }
    }
    
    public class HUDController : MonoBehaviour
    {
        [Header("进度条引用")]
        public UIStatBar hungerBar;
        public UIStatBar moodBar;
        public UIStatBar healthBar;

        [Header("状态文本")]
        public TextMeshProUGUI moodStatusText; // 显示“烦躁”、“悠闲”等

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