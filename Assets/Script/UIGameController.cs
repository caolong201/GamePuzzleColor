using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Quan ly UIGame:
/// - Button Stop: pause game va mo popup UIStop
/// - Score text: cap nhat moi khi player qua obstacle (+1)
/// </summary>
public class UIGameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShapeMatchCoordinator matchCoordinator;
    [SerializeField] private HomeUIController homeUIController;
    [SerializeField] private GameObject stopPopupRoot;

    [Header("Score UI")]
    [SerializeField] private TMP_Text scoreTextTmp;
    [SerializeField] private Text scoreTextLegacy;
    [SerializeField] private string scorePrefix = "Score: ";

    private void Awake()
    {
        if (stopPopupRoot != null)
            stopPopupRoot.SetActive(false);
    }

    private void OnEnable()
    {
        if (matchCoordinator != null)
            matchCoordinator.OnScoreChanged += HandleScoreChanged;

        RefreshScoreText();
    }

    private void OnDisable()
    {
        if (matchCoordinator != null)
            matchCoordinator.OnScoreChanged -= HandleScoreChanged;
    }

    // Goi cho button Stop trong UIGame
    public void OnClickStop()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUiClick();

        Time.timeScale = 0f;
        if (stopPopupRoot != null)
            stopPopupRoot.SetActive(true);
    }

    // Goi cho button Playgame/Tiep tuc trong UIStop
    public void OnClickPlaygame()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUiClick();

        if (stopPopupRoot != null)
            stopPopupRoot.SetActive(false);

        if (matchCoordinator == null || !matchCoordinator.IsGameOver)
            Time.timeScale = 1f;
    }

    // Goi cho button Replay/Choi lai trong UIStop
    public void OnClickReplay()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUiClick();

        if (stopPopupRoot != null)
            stopPopupRoot.SetActive(false);

        if (homeUIController != null)
        {
            homeUIController.RestartScene(true);
            return;
        }

        HomeUIController.StartGameImmediatelyOnNextLoad = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Goi cho button Home/Ve Home trong UIStop
    public void OnClickHome()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUiClick();

        if (stopPopupRoot != null)
            stopPopupRoot.SetActive(false);

        if (homeUIController != null)
        {
            homeUIController.RestartScene(false);
            return;
        }

        HomeUIController.StartGameImmediatelyOnNextLoad = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void HandleScoreChanged(int score)
    {
        SetScoreText(score);
    }

    private void RefreshScoreText()
    {
        int score = matchCoordinator != null ? matchCoordinator.CurrentScore : 0;
        SetScoreText(score);
    }

    private void SetScoreText(int score)
    {
        string value = scorePrefix + score;
        if (scoreTextTmp != null)
            scoreTextTmp.text = value;
        if (scoreTextLegacy != null)
            scoreTextLegacy.text = value;
    }
}
