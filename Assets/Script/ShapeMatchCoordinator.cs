using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// So khớp pattern player với obstacle khi obstacle tới vạch kiểm tra; thua thì bật UI.
/// </summary>
[DefaultExecutionOrder(50)]
public class ShapeMatchCoordinator : MonoBehaviour
{
    /// <summary>
    /// int.MinValue = chưa có lần chơi nào trong session / vừa về Home; dùng startMaterialSetIndex trên coordinator.
    /// Giá trị khác = bộ material hiện tại, giữ khi Replay (load lại scene).
    /// </summary>
    private static int s_persistedMaterialSetIndex = int.MinValue;

    /// <summary>Gọi trước khi load lại scene từ HomeUIController: về Home xóa tiến trình skin; Replay giữ skin.</summary>
    public static void PrepareSceneRestart(bool startGameImmediately)
    {
        if (!startGameImmediately)
            s_persistedMaterialSetIndex = int.MinValue;
    }
    [Serializable]
    private class MaterialSetById
    {
        [Min(0)] public int setId;
        public Material activeMaterial;
        public Material inactiveMaterial;
    }

    [SerializeField] private PlayerCubeShapeController player;
    [SerializeField] private ObstacleGridSpawner obstacleSpawner;
    [SerializeField] private GameObject loseUiRoot;
    [SerializeField] private bool pauseTimeOnLose = true;
    [Header("Lose Explosion Sequence")]
    [SerializeField] private GameObject playerBombEffect; // object Boom con của Player
    [SerializeField] private float loseUiDelayAfterExplosion = 0.6f;

    [Header("Tốc độ theo điểm (đường + obstacle)")]
    [Tooltip("Mỗi N điểm (10, 20, 30…) cộng thêm speedBonusPerTier vào base scroll/move.")]
    [SerializeField] private int scorePerSpeedTier = 10;
    [SerializeField] private float speedBonusPerTier = 3f;
    [Header("Material Sets (Player + Obstacle theo cùng ID)")]
    [SerializeField] private MaterialSetById[] materialSets;
    [SerializeField] private int startMaterialSetIndex;

    public bool IsGameOver { get; private set; }
    public int CurrentScore { get; private set; }
    public event Action<int> OnScoreChanged;

    private int _currentMaterialSetIndex;

    private void Awake()
    {
        CurrentScore = 0;
        int maxSet = Mathf.Max(0, materialSets != null ? materialSets.Length - 1 : 0);
        if (materialSets == null || materialSets.Length == 0)
            _currentMaterialSetIndex = 0;
        else if (s_persistedMaterialSetIndex == int.MinValue)
            _currentMaterialSetIndex = Mathf.Clamp(startMaterialSetIndex, 0, maxSet);
        else
            _currentMaterialSetIndex = Mathf.Clamp(s_persistedMaterialSetIndex, 0, maxSet);

        if (loseUiRoot != null)
            loseUiRoot.SetActive(false);
        if (playerBombEffect != null)
            playerBombEffect.SetActive(false);
    }

    private void Start()
    {
        ApplyCurrentMaterialSet();
    }

    public void TryResolve(ObstaclePatternGrid obstacle)
    {
        if (IsGameOver || obstacle == null || player == null)
            return;

        if (player.PatternMatchesObstacle(obstacle))
        {
            int previousTier = CurrentSpeedTier();
            CurrentScore++;
            int currentTier = CurrentSpeedTier();
            if (currentTier > previousTier)
                AdvanceMaterialSet();

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
        int tiers = CurrentSpeedTier();
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

        ApplyCurrentMaterialSet();
    }

    private int CurrentSpeedTier()
    {
        return Mathf.Max(0, CurrentScore / Mathf.Max(1, scorePerSpeedTier));
    }

    private void AdvanceMaterialSet()
    {
        if (materialSets == null || materialSets.Length == 0)
            return;

        _currentMaterialSetIndex = (_currentMaterialSetIndex + 1) % materialSets.Length;
        s_persistedMaterialSetIndex = _currentMaterialSetIndex;
        ApplyCurrentMaterialSet();
    }

    /// <summary>
    /// Gọi từ UI Lose trước Replay: đổi bộ material cho ván mới (chỉ cập nhật index lưu; scene reload sẽ áp dụng).
    /// Không gọi khi va chạm sai — tránh obstacle còn trên đường đổi màu lộn xộn lúc nổ / chờ UI.
    /// </summary>
    public void AdvanceMaterialForLoseReplayAndPersist()
    {
        if (materialSets == null || materialSets.Length == 0)
            return;

        _currentMaterialSetIndex = (_currentMaterialSetIndex + 1) % materialSets.Length;
        s_persistedMaterialSetIndex = _currentMaterialSetIndex;
    }

    private void ApplyCurrentMaterialSet()
    {
        if (materialSets == null || materialSets.Length == 0)
            return;

        _currentMaterialSetIndex = Mathf.Clamp(_currentMaterialSetIndex, 0, materialSets.Length - 1);
        MaterialSetById set = materialSets[_currentMaterialSetIndex];
        if (set == null || set.activeMaterial == null || set.inactiveMaterial == null)
            return;

        if (player != null)
            player.SetMaterials(set.activeMaterial, set.inactiveMaterial);

        if (obstacleSpawner != null)
            obstacleSpawner.SetMaterials(set.activeMaterial, set.inactiveMaterial);
    }

    private static void ShowLoseAd()
    {
#if UNITY_WEBGL || UNITY_EDITOR
        if (GameMonetize.Instance != null)
            GameMonetize.Instance.ShowAd();
#endif
    }
}
