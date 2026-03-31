using UnityEngine;
using System.Collections;

/// <summary>
/// So khớp pattern player với obstacle khi obstacle tới vạch kiểm tra; thua thì bật UI.
/// </summary>
public class ShapeMatchCoordinator : MonoBehaviour
{
    [SerializeField] private PlayerCubeShapeController player;
    [SerializeField] private GameObject loseUiRoot;
    [SerializeField] private bool pauseTimeOnLose = true;
    [Header("Lose Explosion Sequence")]
    [SerializeField] private GameObject playerBombEffect; // object Boom con của Player
    [SerializeField] private float loseUiDelayAfterExplosion = 0.6f;

    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        if (loseUiRoot != null)
            loseUiRoot.SetActive(false);
        if (playerBombEffect != null)
            playerBombEffect.SetActive(false);
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
        player.SetInteractionEnabled(false);
        StartCoroutine(PlayLoseSequence(obstacle));
    }

    private IEnumerator PlayLoseSequence(ObstaclePatternGrid obstacle)
    {
        if (playerBombEffect != null)
            playerBombEffect.SetActive(true);

        if (player != null)
            player.DestroyAllPlayerCubes();

        if (obstacle != null)
            Destroy(obstacle.gameObject);

        yield return new WaitForSeconds(loseUiDelayAfterExplosion);

        if (loseUiRoot != null)
            loseUiRoot.SetActive(true);
        if (pauseTimeOnLose)
            Time.timeScale = 0f;
    }

    public void ResetMatchState()
    {
        IsGameOver = false;
        Time.timeScale = 1f;
        if (player != null)
            player.SetInteractionEnabled(true);
        if (loseUiRoot != null)
            loseUiRoot.SetActive(false);
    }
}
