using System;
using Manager;
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
        private void Start()
        {
            missonCard.onClick.AddListener(() => AudioManager.Instance.PlayBGM(AudioManager.Instance.startMenu));
            
        }
    
    /// <summary>
    /// 暂停界面
    /// </summary>
    
        public Button resumeButton;
    
    /// <summary>
    /// 结算界面
    /// </summary>
    
        public Text titleText;
        //public Image characterImage;
        public Text contentText;
    }
}