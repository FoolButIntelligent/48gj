using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// 菜单等级
    /// </summary>
    public enum MenuTier
    {
        Tier1,  // 一级菜单（只显示名称和图标）
        Tier2   // 二级菜单（显示属性）
    }

    /// <summary>
    /// 食物类别
    /// </summary>
    public enum FoodCategory
    {
        MainDish,   // 主食
        SideDish,   // 快餐
        Beverage    // 饮品
    }

    /// <summary>
    /// 食物配置 - ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "FoodItem", menuName = "TakeoutSimulator/FoodItem")]
    public class FoodItemConfig : GameDataConfig
    {
        [Header("食物分类")]
        [SerializeField] private FoodCategory category;
        [SerializeField] private MenuTier menuTier = MenuTier.Tier1;

        [Header("属性效果")]
        [SerializeField] private List<AttributeEffect> attributeEffects = new List<AttributeEffect>();

        [Header("确认按钮文本")]
        [SerializeField] private string confirmButtonText = "确认";

        public FoodCategory Category => category;
        public MenuTier MenuTier => menuTier;
        public List<AttributeEffect> AttributeEffects => attributeEffects;
        public string ConfirmButtonText => confirmButtonText;

        /// <summary>
        /// 获取指定属性的效果值
        /// </summary>
        public float GetAttributeEffect(AttributeType type)
        {
            var effect = attributeEffects.Find(e => e.AttributeType == type);
            return effect?.Value ?? 0f;
        }

        /// <summary>
        /// 是否影响指定属性
        /// </summary>
        public bool AffectsAttribute(AttributeType type)
        {
            return attributeEffects.Exists(e => e.AttributeType == type);
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(id))
                Debug.LogError($"FoodItem {name} 缺少 ID!");
            if (icon == null)
                Debug.LogWarning($"FoodItem {name} 缺少图标!");
            if (attributeEffects.Count == 0)
                Debug.LogWarning($"FoodItem {name} 没有属性效果!");
        }
    }
}
