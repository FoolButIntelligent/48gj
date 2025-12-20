using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    /// <summary>
    /// 游戏主要界面
    /// </summary>
    public class MainUI : MonoBehaviour
    {
        public Button missonCard;
        public Button pulseButton;
        public Image characterImage;

        public HUDController valueHud;
        //public FoodHUD foodHud;
    }
    
    /// <summary>
    /// 暂停界面
    /// </summary>
    public class PulseUI: MonoBehaviour
    {
        public Button pulseButton;
    }
    
    /// <summary>
    /// 结算界面
    /// </summary>
    public class SettlementUI : MonoBehaviour
    {
        public TextMeshProUGUI titleText;
        public Image characterImage;
        public TextMeshProUGUI contentText;
    }
}