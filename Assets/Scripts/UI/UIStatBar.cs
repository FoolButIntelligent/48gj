using UnityEngine;
using UnityEngine.UI;
using Manager;

namespace UI
{
    public class UIStatBar : MonoBehaviour
    {
        public Slider slider; // 进度条组件
        public Text valueText; // 显示数值的文本
        public Image fillImage; // 进度条填充图（用于变色）

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
}