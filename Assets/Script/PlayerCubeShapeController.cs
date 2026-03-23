using System.Collections.Generic;
using UnityEngine;

public class PlayerCubeShapeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cubesRoot;
    [SerializeField] private Camera targetCamera;

    [Header("Materials")]
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material inactiveMaterial;

    [Header("Click Settings")]
    [SerializeField] private LayerMask cubeLayerMask = ~0;

    private readonly Dictionary<Renderer, bool> _cubeStates = new Dictionary<Renderer, bool>();

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
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
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
}

