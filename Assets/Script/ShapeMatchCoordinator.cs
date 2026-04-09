using UnityEngine;
using System.Collections;
using System;

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

    [Header("Tốc độ theo điểm (đường + obstacle)")]
    [Tooltip("Mỗi N điểm (10, 20, 30…) cộng thêm speedBonusPerTier vào base scroll/move.")]
    [SerializeField] private int scorePerSpeedTier = 10;
    [SerializeField] private float speedBonusPerTier = 3f;

    public bool IsGameOver { get; private set; }
    public int CurrentScore { get; private set; }
    public event Action<int> OnScoreChanged;

    private void Awake()
    {
        CurrentScore = 0;
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
            CurrentScore++;
            OnScoreChanged?.Invoke(CurrentScore);
            Destroy(obstacle.gameObject);
            return;
        }

        IsGameOver = true;
        player.SetInteractionEnabled(false);
        ShowLoseAd();
        StartCoroutine(PlayLoseSequence(obstacle));
    }

    private IEnumerator PlayLoseSequence(ObstaclePatternGrid obstacle)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBomb();

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

    /// <summary>Base scrollSpeed / moveSpeed + bonus mỗi tier điểm (mặc định +3 mỗi 10 điểm).</summary>
    public float GetScaledSpeed(float baseSpeed)
    {
        if (baseSpeed < 0f)
            return baseSpeed;
        int tiers = Mathf.Max(0, CurrentScore / Mathf.Max(1, scorePerSpeedTier));
        return baseSpeed + tiers * speedBonusPerTier;
    }

    public void ResetMatchState()
    {
        IsGameOver = false;
        CurrentScore = 0;
        OnScoreChanged?.Invoke(CurrentScore);
        Time.timeScale = 1f;
        if (player != null)
            player.SetInteractionEnabled(true);
        if (loseUiRoot != null)
            loseUiRoot.SetActive(false);
    }

    private static void ShowLoseAd()
    {
#if UNITY_WEBGL || UNITY_EDITOR
        if (GameMonetize.Instance != null)
            GameMonetize.Instance.ShowAd();
#endif
    }
}
