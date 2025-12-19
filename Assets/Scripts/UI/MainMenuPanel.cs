using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Manager;

namespace UI
{
    /// <summary>
    /// 开始菜单面板
    /// </summary>
    public class MainMenuPanel : UIBase
    {
        [Header("UI组件")]
        [SerializeField] private TextMeshProUGUI gameTitleText;
        [SerializeField] private Button startButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("存档提示")]
        [SerializeField] private GameObject saveIndicator;
        [SerializeField] private TextMeshProUGUI saveInfoText;

        [Header("设置")]
        [SerializeField] private string gameTitle = "今天吃点啥——点外卖模拟器";

        public event Action OnStartNewGame;
        public event Action OnContinueGame;
        public event Action OnOpenSettings;
        public event Action OnQuitGame;

        protected override void Awake()
        {
            base.Awake();

            // 设置游戏标题
            if (gameTitleText != null)
                gameTitleText.text = gameTitle;

            // 绑定按钮事件
            if (startButton != null)
                startButton.onClick.AddListener(() => OnStartNewGame?.Invoke());

            if (continueButton != null)
                continueButton.onClick.AddListener(() => OnContinueGame?.Invoke());

            if (settingsButton != null)
                settingsButton.onClick.AddListener(() => OnOpenSettings?.Invoke());

            if (quitButton != null)
                quitButton.onClick.AddListener(() => OnQuitGame?.Invoke());
        }

        protected override void OnShow()
        {
            base.OnShow();
            UpdateContinueButton();
        }

        /// <summary>
        /// 更新继续游戏按钮状态
        /// </summary>
        private void UpdateContinueButton()
        {
            bool hasSave = SaveManager.Instance.HasSaveFile();

            if (continueButton != null)
            {
                continueButton.interactable = hasSave;
            }

            if (saveIndicator != null)
            {
                saveIndicator.SetActive(hasSave);
            }

            if (hasSave && saveInfoText != null)
            {
                var saveInfo = SaveManager.Instance.GetSaveInfo();
                if (saveInfo != null)
                {
                    saveInfoText.text = $"存档时间: {saveInfo.saveTime}\n第{saveInfo.currentDay}天";
                }
            }
        }

        /// <summary>
        /// 显示新游戏确认对话框（如果有存档）
        /// </summary>
        public void ShowNewGameConfirmation(Action onConfirm)
        {
            if (SaveManager.Instance.HasSaveFile())
            {
                // 这里可以显示一个确认对话框
                Debug.Log("已有存档，开始新游戏将覆盖存档");
                // 暂时直接执行
                onConfirm?.Invoke();
            }
            else
            {
                onConfirm?.Invoke();
            }
        }
    }

    /// <summary>
    /// 设置面板
    /// </summary>
    public class SettingsPanel : UIBase
    {
        [Header("UI组件")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Button backButton;
        [SerializeField] private Button deleteSaveButton;

        [Header("音量文本")]
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI sfxVolumeText;

        public event Action OnBack;
        public event Action OnDeleteSave;

        protected override void Awake()
        {
            base.Awake();

            // 绑定按钮
            if (backButton != null)
                backButton.onClick.AddListener(() => OnBack?.Invoke());

            if (deleteSaveButton != null)
                deleteSaveButton.onClick.AddListener(() => OnDeleteSave?.Invoke());

            // 绑定滑动条
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }

            // 绑定全屏切换
            if (fullscreenToggle != null)
            {
                fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
            }
        }

        protected override void OnShow()
        {
            base.OnShow();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // 加载音量设置
            if (musicVolumeSlider != null)
            {
                float volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
                musicVolumeSlider.value = volume;
            }

            if (sfxVolumeSlider != null)
            {
                float volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
                sfxVolumeSlider.value = volume;
            }

            // 加载全屏设置
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = Screen.fullScreen;
            }
        }

        private void OnMusicVolumeChanged(float value)
        {
            PlayerPrefs.SetFloat("MusicVolume", value);
            if (musicVolumeText != null)
                musicVolumeText.text = $"{(int)(value * 100)}%";
            
            // TODO: 应用音量到音频管理器
        }

        private void OnSFXVolumeChanged(float value)
        {
            PlayerPrefs.SetFloat("SFXVolume", value);
            if (sfxVolumeText != null)
                sfxVolumeText.text = $"{(int)(value * 100)}%";
            
            // TODO: 应用音量到音频管理器
        }

        private void OnFullscreenToggled(bool isOn)
        {
            Screen.fullScreen = isOn;
        }
    }

    /// <summary>
    /// 删除存档确认对话框
    /// </summary>
    public class DeleteSaveConfirmDialog : UIBase
    {
        [Header("UI组件")]
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        public event Action OnConfirmed;
        public event Action OnCancelled;

        protected override void Awake()
        {
            base.Awake();

            if (confirmButton != null)
                confirmButton.onClick.AddListener(() => OnConfirmed?.Invoke());

            if (cancelButton != null)
                cancelButton.onClick.AddListener(() => OnCancelled?.Invoke());

            if (messageText != null)
                messageText.text = "确定要删除存档吗？\n此操作无法撤销！";
        }
    }
}