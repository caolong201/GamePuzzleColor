using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class HomeUIController : MonoBehaviour
{
    public static bool StartGameImmediatelyOnNextLoad { get; set; }

    [Header("Home UI")]
    [SerializeField] private GameObject homeRoot;
    [SerializeField] private GameObject gameUiRoot;
    [SerializeField] private SkyManager skyManager;

    [Header("Setting Popup")]
    [SerializeField] private GameObject settingPopupRoot;
    [SerializeField] private RectTransform settingPopupPanel;
    [SerializeField] private float popupDuration = 0.25f;
    [SerializeField] private float popupStartScale = 0.8f;

    private Coroutine popupRoutine;
    private bool hasStartedGame;

    /// <summary>Đã Tap to Play — obstacle có thể hiện đúng pattern.</summary>
    public bool HasGameStarted => hasStartedGame;

    private void Awake()
    {
        bool startImmediately = StartGameImmediatelyOnNextLoad;
        StartGameImmediatelyOnNextLoad = false;
        hasStartedGame = false;

        if (homeRoot != null)
            homeRoot.SetActive(true);

        if (gameUiRoot != null)
            gameUiRoot.SetActive(false);

        if (settingPopupRoot != null)
            settingPopupRoot.SetActive(false);

        if (startImmediately)
            OnTapToPlay();
        else
            Time.timeScale = 0f;
    }

    private void OnDestroy()
    {
        StopPopupRoutine();
    }

    // Goi trong OnClick cua button Tap To Play
    public void OnTapToPlay()
    {
        if (hasStartedGame)
            return;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUiClick();

        ShowStartAd();

        hasStartedGame = true;
        Time.timeScale = 1f;

        if (skyManager != null)
            skyManager.ApplyRandomSky();

        if (homeRoot != null)
            homeRoot.SetActive(false);

        if (gameUiRoot != null)
            gameUiRoot.SetActive(true);
    }

    private static void ShowStartAd()
    {
#if UNITY_WEBGL || UNITY_EDITOR
        if (GameMonetize.Instance != null)
            GameMonetize.Instance.ShowAd();
#endif
    }

    public void RestartScene(bool startGameImmediately)
    {
        ShapeMatchCoordinator.PrepareSceneRestart(startGameImmediately);
        StartGameImmediatelyOnNextLoad = startGameImmediately;
        Time.timeScale = 1f;
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    // Goi trong OnClick cua button Setting
    public void OnOpenSetting()
    {
        if (settingPopupRoot == null)
            return;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUiClick();

        StopPopupRoutine();
        settingPopupRoot.SetActive(true);

        if (settingPopupPanel != null)
        {
            popupRoutine = StartCoroutine(AnimatePopupScale(settingPopupPanel, popupStartScale, 1f, popupDuration, false));
        }
    }

    // Goi trong OnClick cua button Close tren popup setting
    public void OnCloseSetting()
    {
        if (settingPopupRoot == null)
            return;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUiClick();

        StopPopupRoutine();

        if (settingPopupPanel == null)
        {
            settingPopupRoot.SetActive(false);
            return;
        }

        popupRoutine = StartCoroutine(AnimatePopupScale(settingPopupPanel, 1f, popupStartScale, popupDuration * 0.8f, true));
    }

    private void StopPopupRoutine()
    {
        if (popupRoutine == null)
            return;

        StopCoroutine(popupRoutine);
        popupRoutine = null;
    }

    private IEnumerator AnimatePopupScale(RectTransform target, float from, float to, float duration, bool hideAfter)
    {
        if (target == null)
            yield break;

        duration = Mathf.Max(0.01f, duration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = EaseOutBack(t);
            if (hideAfter)
                eased = 1f - EaseOutBack(1f - t);

            float scale = Mathf.LerpUnclamped(from, to, eased);
            target.localScale = Vector3.one * scale;
            yield return null;
        }

        target.localScale = Vector3.one * to;

        if (hideAfter && settingPopupRoot != null)
            settingPopupRoot.SetActive(false);

        popupRoutine = null;
    }

    private static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
