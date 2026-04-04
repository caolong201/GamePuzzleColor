using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawn obstacle 3×3 liên tục: mỗi obstacle một pattern (ScriptableObject), có material active/inactive,
/// di chuyển về phía player; so khớp do ShapeMatchCoordinator + PlayerCubeShapeController.
/// </summary>
public class ObstacleGridSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform obstacleParent;
    [SerializeField] private ShapeMatchCoordinator matchCoordinator;
    [Tooltip("Gán Home UI (Tap to Play). Trước khi tap: obstacle spawn nhưng toàn ô activeMaterial để không lộ hình. Để trống = luôn hiện pattern như cũ.")]
    [SerializeField] private HomeUIController homeUi;

    [Header("Materials (giống player)")]
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material inactiveMaterial;

    [Header("Patterns (L, Z, U, H, … — tạo asset Grid Shape 3x3)")]
    [SerializeField] private List<GridShape3x3> shapePatterns = new List<GridShape3x3>();

    [Header("Spawn & di chuyển")]
    [SerializeField] private float spawnDistance = 14f;
    [Tooltip("Càng lớn thì các obstacle spawn cách nhau xa hơn dọc hướng tiến (≈ spawnInterval × moveSpeed).")]
    [SerializeField] private float spawnInterval = 3.4f;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private int maxAliveObstacles = 4;
    [Tooltip("Chọn pattern ngẫu nhiên trong list; tắt thì lần lượt từ đầu list.")]
    [SerializeField] private bool randomizePattern = true;

    [Header("Lưới 3×3 (local X–Y của player)")]
    [SerializeField] private float cellSpacingX = 1f;
    [SerializeField] private float cellSpacingY = 1f;
    [SerializeField] private float heightOffset = 0f;

    [Header("Thời điểm spawn")]
    [SerializeField] private bool spawnOnStart = true;

    private float _nextSpawnTime;
    private int _patternSequentialIndex;
    private bool _obstacleVisualsRevealed;

    private void Start()
    {
        _nextSpawnTime = Time.time;
        if (spawnOnStart && shapePatterns != null && shapePatterns.Count > 0)
            TrySpawnInitial();
    }

    private void Update()
    {
        if (matchCoordinator != null && matchCoordinator.IsGameOver)
            return;

        if (homeUi != null && homeUi.HasGameStarted && !_obstacleVisualsRevealed)
        {
            RevealAllObstacleVisuals();
            _obstacleVisualsRevealed = true;
        }

        if (shapePatterns == null || shapePatterns.Count == 0 || cellPrefab == null)
            return;

        if (player == null)
            return;

        if (CountAliveObstacles() >= maxAliveObstacles)
            return;

        if (Time.time < _nextSpawnTime)
            return;

        SpawnOneObstacle();
        _nextSpawnTime = Time.time + spawnInterval;
    }

    private void TrySpawnInitial()
    {
        if (matchCoordinator != null && matchCoordinator.IsGameOver)
            return;
        SpawnOneObstacle();
        _nextSpawnTime = Time.time + spawnInterval;
    }

    private int CountAliveObstacles()
    {
        Transform root = obstacleParent != null ? obstacleParent : transform;
        int n = 0;
        var movers = root.GetComponentsInChildren<MovingObstacleTowardPlayer>(false);
        for (int i = 0; i < movers.Length; i++)
        {
            if (movers[i] != null && movers[i].enabled && movers[i].gameObject.activeInHierarchy)
                n++;
        }
        return n;
    }

    private GridShape3x3 PickPattern()
    {
        if (shapePatterns == null || shapePatterns.Count == 0)
            return null;

        if (randomizePattern)
            return shapePatterns[Random.Range(0, shapePatterns.Count)];

        GridShape3x3 s = shapePatterns[_patternSequentialIndex % shapePatterns.Count];
        _patternSequentialIndex++;
        return s;
    }

    public void SpawnOneObstacle()
    {
        GridShape3x3 shape = PickPattern();
        if (shape == null)
            return;

        Transform reference = player;
        Transform parent = obstacleParent != null ? obstacleParent : transform;

        Vector3 spawnPos = reference.position
            + reference.forward * spawnDistance
            + reference.up * heightOffset;

        GameObject root = new GameObject($"Obstacle_{shape.name}");
        root.transform.SetPositionAndRotation(spawnPos, reference.rotation);
        root.transform.SetParent(parent, worldPositionStays: true);

        var patternGrid = root.AddComponent<ObstaclePatternGrid>();
        if (homeUi != null && !homeUi.HasGameStarted)
            patternGrid.BuildFromShapeHidden(shape, cellPrefab, activeMaterial, inactiveMaterial, reference, cellSpacingX, cellSpacingY);
        else
            patternGrid.BuildFromShape(shape, cellPrefab, activeMaterial, inactiveMaterial, reference, cellSpacingX, cellSpacingY);

        var mover = root.AddComponent<MovingObstacleTowardPlayer>();
        mover.Setup(reference, moveSpeed, matchCoordinator);
    }

    public void ClearAllObstacles()
    {
        Transform root = obstacleParent != null ? obstacleParent : transform;
        var grids = root.GetComponentsInChildren<ObstaclePatternGrid>(false);
        for (int i = 0; i < grids.Length; i++)
        {
            if (grids[i] != null)
                Destroy(grids[i].gameObject);
        }
    }

    private void RevealAllObstacleVisuals()
    {
        Transform root = obstacleParent != null ? obstacleParent : transform;
        var grids = root.GetComponentsInChildren<ObstaclePatternGrid>(false);
        for (int i = 0; i < grids.Length; i++)
        {
            if (grids[i] != null)
                grids[i].RevealPatternVisuals(activeMaterial, inactiveMaterial);
        }
    }
}
