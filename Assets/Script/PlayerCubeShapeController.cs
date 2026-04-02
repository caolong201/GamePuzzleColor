using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCubeShapeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cubesRoot;
    [SerializeField] private Camera targetCamera;
    [Tooltip("9 Renderer theo thứ tự index x+y*3 (y từ dưới lên). Để trống sẽ tự sort theo local Y rồi X.")]
    [SerializeField] private Renderer[] orderedGridCells;

    [Header("Materials")]
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material inactiveMaterial;

    [Header("Click Settings")]
    [SerializeField] private LayerMask cubeLayerMask = ~0;
    [SerializeField] private bool allowInteraction = true;

    private readonly Dictionary<Renderer, bool> _cubeStates = new Dictionary<Renderer, bool>();
    private Renderer[] _resolvedOrder;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (cubesRoot == null)
            cubesRoot = transform;
    }

    private void Start()
    {
        InitializeAllCubesAsActive();
        ResolveOrderedCells();
    }

    private void Update()
    {
        if (!allowInteraction)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        if (IsPointerOverUi())
            return;

        if (targetCamera == null)
            return;

        Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 500f, cubeLayerMask))
            return;

        Renderer renderer = hit.collider.GetComponent<Renderer>();
        if (renderer == null)
            return;

        if (!IsInCubesRoot(renderer.transform))
            return;

        ToggleCube(renderer);
    }

    private static bool IsPointerOverUi()
    {
        if (EventSystem.current == null)
            return false;

        // Mouse / standalone
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        // Touch devices
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                if (EventSystem.current.IsPointerOverGameObject(t.fingerId))
                    return true;
            }
        }

        return false;
    }

    public void SetInteractionEnabled(bool enabled)
    {
        allowInteraction = enabled;
    }

    public void DestroyAllPlayerCubes()
    {
        if (cubesRoot == null)
            return;

        for (int i = cubesRoot.childCount - 1; i >= 0; i--)
            Destroy(cubesRoot.GetChild(i).gameObject);
    }

    private void InitializeAllCubesAsActive()
    {
        if (activeMaterial == null || inactiveMaterial == null)
        {
            Debug.LogError($"{nameof(PlayerCubeShapeController)}: Please assign both activeMaterial and inactiveMaterial.", this);
            enabled = false;
            return;
        }

        _cubeStates.Clear();
        Renderer[] renderers = cubesRoot.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            _cubeStates[r] = true;
            r.material = activeMaterial;
        }
    }

    private void ToggleCube(Renderer renderer)
    {
        if (!_cubeStates.ContainsKey(renderer))
        {
            _cubeStates[renderer] = true;
            renderer.material = activeMaterial;
        }

        bool currentState = _cubeStates[renderer];
        bool nextState = !currentState;
        _cubeStates[renderer] = nextState;

        renderer.material = nextState ? activeMaterial : inactiveMaterial;
    }

    private bool IsInCubesRoot(Transform target)
    {
        return target == cubesRoot || target.IsChildOf(cubesRoot);
    }

    private void ResolveOrderedCells()
    {
        if (orderedGridCells != null && orderedGridCells.Length == 9)
        {
            _resolvedOrder = orderedGridCells;
            return;
        }

        Renderer[] renderers = cubesRoot.GetComponentsInChildren<Renderer>(true);
        _resolvedOrder = renderers
            .OrderBy(r => r.transform.localPosition.y)
            .ThenBy(r => r.transform.localPosition.x)
            .ToArray();

        if (_resolvedOrder.Length != 9)
        {
            Debug.LogWarning(
                $"{nameof(PlayerCubeShapeController)}: Cần đúng 9 ô 3×3. Hiện có {_resolvedOrder.Length}. Gán orderedGridCells thủ công nếu sort tự động sai.",
                this);
        }
    }

    /// <summary>
    /// So khớp trạng thái active/inactive của player với pattern obstacle (cùng quy ước index).
    /// </summary>
    public bool PatternMatchesObstacle(ObstaclePatternGrid obstacle)
    {
        if (obstacle == null)
            return false;

        if (_resolvedOrder == null || _resolvedOrder.Length != 9)
            return false;

        for (int i = 0; i < 9; i++)
        {
            Renderer r = _resolvedOrder[i];
            if (r == null)
                return false;

            bool playerActive = _cubeStates.TryGetValue(r, out bool st) && st;
            bool obstacleActive = obstacle.GetPatternCell(i);
            if (playerActive != obstacleActive)
                return false;
        }

        return true;
    }
}

