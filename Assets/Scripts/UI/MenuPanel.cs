using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using Core;

namespace UI
{
    /// <summary>
    /// 菜单UI面板
    /// </summary>
    public class MenuPanel : UIBase
    {
        [Header("分类按钮")]
        [SerializeField] private Button mainDishButton;
        [SerializeField] private Button sideDishButton;
        [SerializeField] private Button beverageButton;

        [Header("菜单项容器")]
        [SerializeField] private Transform menuItemContainer;
        [SerializeField] private GameObject menuItemPrefab;

        [Header("底部按钮")]
        [SerializeField] private Button confirmOrderButton;

        private FoodCategory currentCategory = FoodCategory.MainDish;
        private List<MenuItemUI> currentMenuItems = new List<MenuItemUI>();
        private List<FoodItemConfig> allFoodItems = new List<FoodItemConfig>();

        public event Action<FoodItemConfig> OnFoodSelected;
        public event Action OnOrderConfirmed;

        protected override void Awake()
        {
            base.Awake();

            // 绑定按钮事件
            if (mainDishButton != null)
                mainDishButton.onClick.AddListener(() => ShowCategory(FoodCategory.MainDish));
            
            if (sideDishButton != null)
                sideDishButton.onClick.AddListener(() => ShowCategory(FoodCategory.SideDish));
            
            if (beverageButton != null)
                beverageButton.onClick.AddListener(() => ShowCategory(FoodCategory.Beverage));

            if (confirmOrderButton != null)
                confirmOrderButton.onClick.AddListener(() => OnOrderConfirmed?.Invoke());
        }

        /// <summary>
        /// 初始化菜单（加载所有食物配置）
        /// </summary>
        public void InitializeMenu(List<FoodItemConfig> foodItems)
        {
            allFoodItems = foodItems;
            ShowCategory(currentCategory);
        }

        /// <summary>
        /// 显示指定分类的菜单
        /// </summary>
        public void ShowCategory(FoodCategory category)
        {
            currentCategory = category;
            
            // 清空当前显示
            ClearMenuItems();

            // 筛选并显示该分类的食物
            var filteredItems = allFoodItems.FindAll(item => item.Category == category);
            
            foreach (var foodItem in filteredItems)
            {
                CreateMenuItem(foodItem);
            }

            UpdateCategoryButtonStates();
        }

        /// <summary>
        /// 创建菜单项UI
        /// </summary>
        private void CreateMenuItem(FoodItemConfig foodConfig)
        {
            if (menuItemPrefab == null || menuItemContainer == null)
            {
                Debug.LogError("MenuPanel: 菜单项预制体或容器未设置!");
                return;
            }

            GameObject itemObj = Instantiate(menuItemPrefab, menuItemContainer);
            MenuItemUI menuItem = itemObj.GetComponent<MenuItemUI>();

            if (menuItem != null)
            {
                menuItem.Initialize(foodConfig);
                menuItem.OnClicked += () => OnFoodSelected?.Invoke(foodConfig);
                currentMenuItems.Add(menuItem);
            }
        }

        /// <summary>
        /// 清空菜单项
        /// </summary>
        private void ClearMenuItems()
        {
            foreach (var item in currentMenuItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            currentMenuItems.Clear();
        }

        /// <summary>
        /// 更新分类按钮状态
        /// </summary>
        private void UpdateCategoryButtonStates()
        {
            // 这里可以添加按钮高亮等视觉反馈
        }

        protected override void OnHide()
        {
            base.OnHide();
            ClearMenuItems();
        }
    }

    /// <summary>
    /// 单个菜单项UI
    /// </summary>
    public class MenuItemUI : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private Image foodIcon;
        [SerializeField] private TextMeshProUGUI foodNameText;
        [SerializeField] private GameObject attributesPanel;
        [SerializeField] private TextMeshProUGUI hungerText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Button selectButton;

        private FoodItemConfig foodConfig;

        public event Action OnClicked;

        private void Awake()
        {
            if (selectButton != null)
            {
                selectButton.onClick.AddListener(() => OnClicked?.Invoke());
            }
        }

        public void Initialize(FoodItemConfig config)
        {
            foodConfig = config;

            // 设置图标
            if (foodIcon != null && config.Icon != null)
            {
                foodIcon.sprite = config.Icon;
            }

            // 设置名称
            if (foodNameText != null)
            {
                foodNameText.text = config.DisplayName;
            }

            // 根据菜单等级显示属性
            if (config.MenuTier == MenuTier.Tier1)
            {
                // 一级菜单：只显示名称和图标
                if (attributesPanel != null)
                    attributesPanel.SetActive(false);
            }
            else
            {
                // 二级菜单：显示属性
                if (attributesPanel != null)
                    attributesPanel.SetActive(true);

                UpdateAttributeDisplay();
            }
        }

        private void UpdateAttributeDisplay()
        {
            // 显示饱腹感效果
            if (hungerText != null)
            {
                float hungerEffect = foodConfig.GetAttributeEffect(AttributeType.Hunger);
                hungerText.text = $"饱腹感: {(hungerEffect >= 0 ? "+" : "")}{hungerEffect}";
            }

            // 显示健康值效果
            if (healthText != null)
            {
                float healthEffect = foodConfig.GetAttributeEffect(AttributeType.Health);
                healthText.text = $"健康值: {(healthEffect >= 0 ? "+" : "")}{healthEffect}";
            }
        }
    }

    /// <summary>
    /// 点餐确认弹窗
    /// </summary>
    public class OrderConfirmDialog : DataDrivenUIPanel<FoodItemConfig>
    {
        [Header("UI组件")]
        [SerializeField] private Image foodIcon;
        [SerializeField] private TextMeshProUGUI foodNameText;
        [SerializeField] private TextMeshProUGUI hungerEffectText;
        [SerializeField] private TextMeshProUGUI healthEffectText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        public event Action<FoodItemConfig> OnConfirmed;
        public event Action OnCancelled;

        protected override void Awake()
        {
            base.Awake();

            if (confirmButton != null)
                confirmButton.onClick.AddListener(() => OnConfirmed?.Invoke(currentData));

            if (cancelButton != null)
                cancelButton.onClick.AddListener(() => OnCancelled?.Invoke());
        }

        protected override void UpdateUI()
        {
            if (currentData == null) return;

            if (foodIcon != null && currentData.Icon != null)
                foodIcon.sprite = currentData.Icon;

            if (foodNameText != null)
                foodNameText.text = currentData.DisplayName;

            if (hungerEffectText != null)
            {
                float hunger = currentData.GetAttributeEffect(AttributeType.Hunger);
                hungerEffectText.text = $"饱腹感 {(hunger >= 0 ? "+" : "")}{hunger}";
            }

            if (healthEffectText != null)
            {
                float health = currentData.GetAttributeEffect(AttributeType.Health);
                healthEffectText.text = $"健康值 {(health >= 0 ? "+" : "")}{health}";
            }

            if (confirmButton != null)
            {
                var buttonText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.text = currentData.ConfirmButtonText;
            }
        }
    }
}
