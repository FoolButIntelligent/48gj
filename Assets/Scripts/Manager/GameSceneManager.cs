using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    private void Awake()
    {
        // 确保管理类在切换场景时不被销毁
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 场景常量，防止拼写错误
    public const string START_SCENE = "StartScene";
    public const string GAME_SCENE = "GameScene";
    public const string SETTLE_SCENE = "SettleScene";

    public void LoadStartScene() => SceneManager.LoadScene(START_SCENE);
    public void LoadGameScene() => SceneManager.LoadScene(GAME_SCENE);
    public void LoadSettleScene() => SceneManager.LoadScene(SETTLE_SCENE);

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
