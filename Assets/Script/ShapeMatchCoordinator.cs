using UnityEngine;

/// <summary>
/// So khớp pattern player với obstacle khi obstacle tới vạch kiểm tra; thua thì bật UI.
/// </summary>
public class ShapeMatchCoordinator : MonoBehaviour
{
    [SerializeField] private PlayerCubeShapeController player;
    [SerializeField] private GameObject loseUiRoot;
    [SerializeField] private bool pauseTimeOnLose = true;

    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        if (loseUiRoot != null)
            loseUiRoot.SetActive(false);
    }

    public void TryResolve(ObstaclePatternGrid obstacle)
    {
        if (IsGameOver || obstacle == null || player == null)
            return;

        if (player.PatternMatchesObstacle(obstacle))
        {
            Destroy(obstacle.gameObject);
            return;
        }

        IsGameOver = true;
        if (loseUiRoot != null)
            loseUiRoot.SetActive(true);
        if (pauseTimeOnLose)
            Time.timeScale = 0f;
    }

    public void ResetMatchState()
    {
        IsGameOver = false;
        Time.timeScale = 1f;
        if (loseUiRoot != null)
            loseUiRoot.SetActive(false);
    }
}
