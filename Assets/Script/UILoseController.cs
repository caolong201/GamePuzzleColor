using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Xu ly button trong UI Lose:
/// - Replay: reload scene va vao game ngay
/// - Home: reload scene va quay ve man Home
/// </summary>
public class UILoseController : MonoBehaviour
{
    [SerializeField] private HomeUIController homeUIController;

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
