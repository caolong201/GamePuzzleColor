using System.Collections.Generic;
using UnityEngine;

public class ChunkPanl : MonoBehaviour
{
    [SerializeField] GameObject chunkPrefabs;
    [SerializeField] int startingChunksAmount = 12;
    [SerializeField] float chunkLength = 10f;
    [SerializeField] Transform chunkParent;
    [SerializeField] float scrollSpeed = 8f;
    [Tooltip("Player hoặc camera. Để trống sẽ dùng Camera.main, không có thì dùng vị trí object này.")]
    [SerializeField] Transform referencePoint;
   
    [SerializeField] float recycleMargin = 2f;

    private readonly List<Transform> _chunks = new List<Transform>();

    void Start()
    {
        LoadChunk();
    }

    void Update()
    {
        if (_chunks.Count == 0)
            return;

        float dz = -scrollSpeed * Time.deltaTime;
        for (int i = 0; i < _chunks.Count; i++)
        {
            Transform t = _chunks[i];
            Vector3 p = t.position;
            p.z += dz;
            t.position = p;
        }

        RecycleChunks();
    }

    private void LoadChunk()
    {
        for (int i = 0; i < startingChunksAmount; i++)
        {
            float spawnZ = NewMethod(i);
            Vector3 chunkSpawnPos = new Vector3(transform.position.x, transform.position.y, spawnZ);
            GameObject instance = Instantiate(chunkPrefabs, chunkSpawnPos, Quaternion.identity, chunkParent);
            _chunks.Add(instance.transform);
        }
    }

    private float NewMethod(int i)
    {
        if (i == 0)
            return transform.position.z;
        return transform.position.z + (i * chunkLength);
    }

    private void RecycleChunks()
    {
        Transform reference = referencePoint != null ? referencePoint : (Camera.main != null ? Camera.main.transform : transform);
        float refZ = reference.position.z;

        while (_chunks.Count > 0)
        {
            Transform tail = _chunks[0];
            if (tail.position.z + chunkLength < refZ - recycleMargin)
            {
                Transform head = _chunks[_chunks.Count - 1];
                Vector3 p = tail.position;
                p.z = head.position.z + chunkLength;
                tail.position = p;

                _chunks.RemoveAt(0);
                _chunks.Add(tail);
            }
            else
            {
                break;
            }
        }
    }
}
