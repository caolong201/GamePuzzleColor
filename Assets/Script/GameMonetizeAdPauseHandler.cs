using UnityEngine;

/// <summary>
/// Bridges GameMonetize ad pause/resume events to Unity timeScale.
/// It preserves the previous timeScale so game state is restored safely after ads.
/// </summary>
public class GameMonetizeAdPauseHandler : MonoBehaviour
{
#if UNITY_WEBGL || UNITY_EDITOR
    private static GameMonetizeAdPauseHandler _instance;
    private int _activeAdPauseCount;
    private float _timeScaleBeforeAd = 1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (_instance != null)
            return;

        GameObject go = new GameObject(nameof(GameMonetizeAdPauseHandler));
        DontDestroyOnLoad(go);
        _instance = go.AddComponent<GameMonetizeAdPauseHandler>();
    }

    private void OnEnable()
    {
        GameMonetize.OnPauseGame += HandlePauseFromAd;
        GameMonetize.OnResumeGame += HandleResumeFromAd;
    }

    private void OnDisable()
    {
        GameMonetize.OnPauseGame -= HandlePauseFromAd;
        GameMonetize.OnResumeGame -= HandleResumeFromAd;
    }

    private void HandlePauseFromAd()
    {
        _activeAdPauseCount++;
        if (_activeAdPauseCount == 1)
        {
            _timeScaleBeforeAd = Time.timeScale;
            Time.timeScale = 0f;
        }
    }

    private void HandleResumeFromAd()
    {
        if (_activeAdPauseCount <= 0)
            return;

        _activeAdPauseCount--;
        if (_activeAdPauseCount == 0)
            Time.timeScale = _timeScaleBeforeAd;
    }
#endif
}

