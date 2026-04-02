using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Xu ly button trong UI Lose:
/// - Replay: reload scene va vao game ngay
/// - Home: reload scene va quay ve man Home
/// </summary>
public class UILoseController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HomeUIController homeUIController;
    [SerializeField] private ShapeMatchCoordinator matchCoordinator;

    [Header("Score UI")]
    [SerializeField] private TMP_Text scoreTextTmp;
    [SerializeField] private Text scoreTextLegacy;
    [SerializeField] private string scorePrefix = "Score: ";

    private void OnEnable()
    {
        RefreshScore();
    }

    public void RefreshScore()
    {
        int score = matchCoordinator != null ? matchCoordinator.CurrentScore : 0;
        string value = scorePrefix + score;
        if (scoreTextTmp != null)
            scoreTextTmp.text = value;
        if (scoreTextLegacy != null)
            scoreTextLegacy.text = value;
    }

    public void OnClickReplay()
    {
        if (homeUIController != null)
        {
            homeUIController.RestartScene(true);
            return;
        }

        HomeUIController.StartGameImmediatelyOnNextLoad = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnClickHome()
    {
        if (homeUIController != null)
        {
            homeUIController.RestartScene(false);
            return;
        }

        HomeUIController.StartGameImmediatelyOnNextLoad = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
