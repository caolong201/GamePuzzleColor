using UnityEngine;

/// <summary>
/// Di chuyển obstacle về phía player theo -forward của player; tới vạch kiểm tra thì gọi coordinator.
/// </summary>
[RequireComponent(typeof(ObstaclePatternGrid))]
public class MovingObstacleTowardPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 7f;
    [Tooltip("Khi (vị trí - player)·forward <= ngưỡng này thì chấm điểm một lần.")]
    [SerializeField] private float evaluateAlongThreshold = 0.35f;

    private ShapeMatchCoordinator _coordinator;
    private ObstaclePatternGrid _grid;
    private bool _evaluated;

    public void Setup(Transform playerRef, float speed, ShapeMatchCoordinator coordinator)
    {
        player = playerRef;
        moveSpeed = speed;
        _coordinator = coordinator;
        _grid = GetComponent<ObstaclePatternGrid>();
        _evaluated = false;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    private void Update()
    {
        if (_evaluated || player == null)
            return;

        if (_coordinator != null && _coordinator.IsGameOver)
            return;

        transform.position += -player.forward * (moveSpeed * Time.deltaTime);

        float along = Vector3.Dot(transform.position - player.position, player.forward);
        if (along > evaluateAlongThreshold)
            return;

        _evaluated = true;
        if (_coordinator != null && _grid != null)
            _coordinator.TryResolve(_grid);
        else
            Debug.LogWarning($"{nameof(MovingObstacleTowardPlayer)}: Thiếu ShapeMatchCoordinator hoặc ObstaclePatternGrid.", this);
    }
}
