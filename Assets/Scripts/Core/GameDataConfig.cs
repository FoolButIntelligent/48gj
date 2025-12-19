using UnityEngine;

namespace Core
{
    /// <summary>
    /// 游戏数据配置基类 - 所有可配置的游戏数据都继承此类
    /// </summary>
    public abstract class GameDataConfig : ScriptableObject
    {
        [SerializeField] protected string id;
        [SerializeField] protected string displayName;
        [SerializeField] protected Sprite icon;

        public string Id => id;
        public string DisplayName => displayName;
        public Sprite Icon => icon;

        public abstract void Validate();
    }
}
