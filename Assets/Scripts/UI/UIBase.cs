using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    /// <summary>
    /// UI面板基类
    /// </summary>
    public abstract class UIBase : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected float fadeDuration = 0.3f;

        protected bool isVisible = false;

        protected virtual void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void Show(bool immediate = false)
        {
            gameObject.SetActive(true);
            isVisible = true;

            if (immediate)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            else
            {
                StartCoroutine(FadeIn());
            }

            OnShow();
        }

        public virtual void Hide(bool immediate = false)
        {
            isVisible = false;

            if (immediate)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(FadeOut());
            }

            OnHide();
        }

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }

        private System.Collections.IEnumerator FadeIn()
        {
            float elapsed = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        private System.Collections.IEnumerator FadeOut()
        {
            float elapsed = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 数据驱动的UI面板基类 - 泛型约束
    /// </summary>
    public abstract class DataDrivenUIPanel<TData> : UIBase where TData : class
    {
        protected TData currentData;

        public void SetData(TData data)
        {
            currentData = data;
            UpdateUI();
        }

        protected abstract void UpdateUI();
    }

    /// <summary>
    /// 属性条UI组件
    /// </summary>
    public class AttributeBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private Color normalColor = Color.green;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color dangerColor = Color.red;

        public void Initialize(string label)
        {
            if (labelText != null)
                labelText.text = label;
        }

        public void UpdateValue(float current, float max, bool showText = true)
        {
            float percentage = current / max;
            
            if (fillImage != null)
            {
                fillImage.fillAmount = percentage;
                
                // 根据百分比改变颜色
                if (percentage > 0.6f)
                    fillImage.color = normalColor;
                else if (percentage > 0.3f)
                    fillImage.color = warningColor;
                else
                    fillImage.color = dangerColor;
            }

            if (valueText != null && showText)
            {
                valueText.text = $"{current:F0}/{max:F0}";
            }
        }

        public void UpdateValue(float percentage, bool showText = true)
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = percentage / 100f;
                
                if (percentage > 60f)
                    fillImage.color = normalColor;
                else if (percentage > 30f)
                    fillImage.color = warningColor;
                else
                    fillImage.color = dangerColor;
            }

            if (valueText != null && showText)
            {
                valueText.text = $"{percentage:F0}%";
            }
        }
    }
}
