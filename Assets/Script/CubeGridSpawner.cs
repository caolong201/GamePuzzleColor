using System.Collections.Generic;
using UnityEngine;

public class CubeGridSpawner : MonoBehaviour
{
    public enum GridPlane
    {
        // X ngang, Z dọc: giữ Y cố định (giống bản gốc)
        XZ,
        // X ngang, Y dọc: giữ Z cố định
        XY
    }

    [Header("Prefabs")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Transform cubeParent;

    [Header("Grid Plane & Size (3x3)")]
    [SerializeField] private GridPlane gridPlane = GridPlane.XY;
    [SerializeField] private int sizeX = 3;
    [SerializeField] private int sizeZ = 3;

    [Header("Spacing")]
    [SerializeField] private float spacingX = 1f;
    [SerializeField] private float spacingZ = 1f;
    [SerializeField] private float yOffset = 0f;

    [Header("Placement")]
    [SerializeField] private bool centerGrid = true;

    private readonly List<GameObject> _instances = new List<GameObject>();
    private bool _spawned;

    private void Start()
    {
        if (_spawned)
            return;

        if (cubePrefab == null)
        {
            Debug.LogError($"{nameof(CubeGridSpawner)}: cubePrefab is not assigned.", this);
            enabled = false;
            return;
        }

        if (sizeX <= 0 || sizeZ <= 0)
        {
            Debug.LogError($"{nameof(CubeGridSpawner)}: sizeX and sizeZ must be > 0.", this);
            enabled = false;
            return;
        }

        SpawnGrid();
        _spawned = true;
    }

    private void SpawnGrid()
    {
        Transform parent = cubeParent != null ? cubeParent : transform;

        float offsetX = centerGrid ? (sizeX - 1) * spacingX * 0.5f : 0f;
        float offsetZorY = centerGrid ? (sizeZ - 1) * spacingZ * 0.5f : 0f;

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                // gridPlane determines the axes:
                // - XZ: use (X, Z), keep Y fixed.
                // - XY: use (X, Y), keep Z fixed.
                float localX = x * spacingX - offsetX;
                float localSecond = z * spacingZ - offsetZorY; // Z when XZ plane, Y when XY plane

                Vector3 worldPos = transform.position + transform.right * localX;
                if (gridPlane == GridPlane.XZ)
                {
                    // Keep Y fixed.
                    float localZ = localSecond;
                    worldPos = worldPos + transform.up * yOffset + transform.forward * localZ;
                }
                else
                {
                    // Keep Z fixed, build along Y.
                    float localY = localSecond + yOffset;
                    worldPos = worldPos + transform.up * localY;
                }

                GameObject instance = Instantiate(cubePrefab, worldPos, Quaternion.identity, parent);
                _instances.Add(instance);
            }
        }
    }
}

