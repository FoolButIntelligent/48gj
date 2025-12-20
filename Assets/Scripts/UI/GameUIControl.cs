using Manager;
using UnityEngine;

namespace UI
{
    public class GameUIControl : MonoBehaviour
    {
        [Header("UI 面板引用")]
        public GameObject pausePanel;       // 暂停页面
        public GameObject dailyStatusPanel; // 今日状态/任务卡片页面

        private bool isPaused = false;

        void Update()
        {
            // 按下 ESC 键切换暂停
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }

        // 暂停游戏
        public void PauseGame()
        {
            isPaused = true;
            pausePanel.SetActive(true);
            Time.timeScale = 0f; // 停止物理和计时逻辑
        
            // 音频不受 timeScale 影响，如果你想让音乐变小，可以加这一行：
            AudioManager.Instance.bgmSource.volume = 0.1f;
        }

        // 继续游戏
        public void ResumeGame()
        {
            isPaused = false;
            pausePanel.SetActive(false);
           // dailyStatusPanel.SetActive(false); // 关闭今日状态
            Time.timeScale = 1f; // 恢复正常时间
        
            // 恢复音量
            AudioManager.Instance.bgmSource.volume = 1f;
        }

        // 显示今日状态 (弹出时通常也要暂停游戏)
        public void ShowDailyStatus()
        {
            dailyStatusPanel.SetActive(true);
            Time.timeScale = 0f; 
            // 播放弹出音效
            if(AudioManager.Instance != null) 
                AudioManager.Instance.PlaySFX(AudioManager.Instance.popupOpen);
        }
    }
}
