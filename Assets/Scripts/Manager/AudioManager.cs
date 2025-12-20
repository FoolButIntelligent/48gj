namespace Manager
{
    using UnityEngine;

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("UI 音效")]
        public AudioSource uiSource;
        public AudioClip clickSound;      // 对应：轻快清脆
        public AudioClip hoverSound;      // 对应：细小音色
        public AudioClip popupOpen;       // 对应：啪/咔哒
        public AudioClip popupClose;      // 对应：咻

        [Header("结算与结局")]
        public AudioSource musicSource;   // 用于播放结局长音效或背景音乐
        public AudioClip settleMusic;//场景音乐
        public AudioClip settleTransition; // 对应：过渡音
        public AudioClip ending1;         // 养生砖家：平稳
        public AudioClip ending2;         // 大卫戴：作死音色
        public AudioClip ending3;         // 营养均衡：欢呼

        void Awake()
        {
            if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
            else { Destroy(gameObject); }
        }

        // 播放短促音效
        public void PlaySFX(AudioClip clip)
        {
            if (clip != null) uiSource.PlayOneShot(clip);
        }

        // 播放结局音频
        public void PlayEnding(int index)
        {
            AudioClip target = index switch {
                1 => ending1,
                2 => ending2,
                3 => ending3,
                _ => null
            };
            musicSource.clip = target;
            musicSource.Play();
        }
    }
}