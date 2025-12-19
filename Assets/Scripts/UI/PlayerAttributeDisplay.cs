using UnityEngine;
using TMPro;
using Manager;
using Core;

namespace UI
{
    /// <summary>
    /// 玩家属性显示UI
    /// </summary>
    public class PlayerAttributeDisplay : MonoBehaviour
    {
        [Header("属性条")]
        [SerializeField] private AttributeBar healthBar;
        [SerializeField] private AttributeBar moodBar;
        [SerializeField] private AttributeBar hungerBar;

        [Header("状态文本")]
        [SerializeField] private TextMeshProUGUI moodStateText;
        [SerializeField] private TextMeshProUGUI hungerStateText;

        [Header("依赖")]
        [SerializeField] private PlayerAttributeManager attributeManager;

        private void Start()
        {
            if (attributeManager == null)
                attributeManager = FindFirstObjectByType<PlayerAttributeManager>();

            InitializeUI();
            SubscribeToEvents();
            UpdateAllBars();
        }

        private void InitializeUI()
        {
            if (healthBar != null)
                healthBar.Initialize("健康值");

            if (moodBar != null)
                moodBar.Initialize("心情值");

            if (hungerBar != null)
                hungerBar.Initialize("饱腹感");
        }

        private void SubscribeToEvents()
        {
            if (attributeManager == null) return;

            attributeManager.OnAttributeChanged += OnAttributeChanged;
            attributeManager.OnMoodStateChanged += OnMoodStateChanged;
            attributeManager.OnHungerStateChanged += OnHungerStateChanged;
        }

        private void OnAttributeChanged(AttributeType type, float oldValue, float newValue)
        {
            switch (type)
            {
                case AttributeType.Health:
                    UpdateHealthBar();
                    break;
                case AttributeType.Mood:
                    UpdateMoodBar();
                    break;
                case AttributeType.Hunger:
                    UpdateHungerBar();
                    break;
            }
        }

        private void UpdateHealthBar()
        {
            if (healthBar != null)
            {
                healthBar.UpdateValue(
                    attributeManager.Health.CurrentValue,
                    attributeManager.Health.MaxValue
                );
            }
        }

        private void UpdateMoodBar()
        {
            if (moodBar != null)
            {
                // 心情值范围是 -50 到 50，需要转换为 0-100 显示
                float normalizedValue = (attributeManager.Mood.CurrentValue + 50f) / 100f * 100f;
                moodBar.UpdateValue(normalizedValue);
            }
        }

        private void UpdateHungerBar()
        {
            if (hungerBar != null)
            {
                hungerBar.UpdateValue(
                    attributeManager.Hunger.CurrentValue,
                    attributeManager.Hunger.MaxValue
                );
            }
        }

        private void OnMoodStateChanged(MoodState newState)
        {
            if (moodStateText != null)
            {
                moodStateText.text = GetMoodStateText(newState);
            }
        }

        private void OnHungerStateChanged(HungerState newState)
        {
            if (hungerStateText != null)
            {
                hungerStateText.text = GetHungerStateText(newState);
            }
        }

        private string GetMoodStateText(MoodState state)
        {
            switch (state)
            {
                case MoodState.Upset: return "烦躁";
                case MoodState.Low: return "低落";
                case MoodState.Calm: return "悠闲";
                case MoodState.Happy: return "开心";
                default: return "";
            }
        }

        private string GetHungerStateText(HungerState state)
        {
            switch (state)
            {
                case HungerState.Starving: return "饥饿";
                case HungerState.Satisfied: return "满足";
                case HungerState.Happy: return "开心";
                case HungerState.Overfed: return "过饱";
                default: return "";
            }
        }

        private void UpdateAllBars()
        {
            UpdateHealthBar();
            UpdateMoodBar();
            UpdateHungerBar();
            
            if (moodStateText != null)
                moodStateText.text = GetMoodStateText(attributeManager.CurrentMoodState);
            
            if (hungerStateText != null)
                hungerStateText.text = GetHungerStateText(attributeManager.CurrentHungerState);
        }

        private void OnDestroy()
        {
            if (attributeManager != null)
            {
                attributeManager.OnAttributeChanged -= OnAttributeChanged;
                attributeManager.OnMoodStateChanged -= OnMoodStateChanged;
                attributeManager.OnHungerStateChanged -= OnHungerStateChanged;
            }
        }
    }
}
