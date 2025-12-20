namespace Manager
{
    using UnityEngine;
    using TMPro;

    public class GameLoopManager : MonoBehaviour
    {
        public static GameLoopManager Instance;

        [Header("时间设置")]
        public float dayDuration = 60f; // 1分钟一关
        private float currentTimer;
        public int currentDay = 1;
        private bool isGameActive = false;

        [Header("UI 引用")]
        public TextMeshProUGUI timeText; // 显示倒计时
        public GameObject dailyStatusPanel; // “今日状态”UI面板

        void Start()
        {
            Instance = this;
            StartDay();
        }

        public void StartDay()
        {
            currentTimer = dayDuration;
            isGameActive = true;
            Time.timeScale = 1f; // 恢复时间
            dailyStatusPanel.SetActive(false);
            Debug.Log($"第 {currentDay} 天开始");
        }

        void Update()
        {
            if (!isGameActive) return;

            currentTimer -= Time.deltaTime;
        
            // 更新 UI 倒计时 (00:00 格式)
            if(timeText != null)
            {
                int seconds = Mathf.Max(0, Mathf.FloorToInt(currentTimer));
                timeText.text = string.Format("{0:00}:{1:00}", seconds / 60, seconds % 60);
            }

            if (currentTimer <= 0)
            {
                EndDay();
            }
        }

        void EndDay()
        {
            isGameActive = false;
            Time.timeScale = 0f; // 暂停游戏逻辑（属性停止下降）

            // 判定任务结果 (调用你之前的 MissionManager)
            // MissionManager.Instance.OnDayEnd(); 

            if (currentDay < 3)
            {
                // 弹出今日状态
                dailyStatusPanel.SetActive(true); 
                if(AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.popupOpen);
            }
            else
            {
                // 3天结束，跳到结算场景
                //TransitionManager.Instance.TransitionToScene("SettleScene");
            }
        }

        // 当玩家点击“今日状态”上的“进入下一天”按钮时调用
        public void NextDayButton()
        {
            currentDay++;
            StartDay();
        }
    }
}