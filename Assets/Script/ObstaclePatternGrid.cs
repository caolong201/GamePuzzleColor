using UnityEngine;

/// <summary>
/// Một obstacle 3×3: lưu pattern để so khớp và cập nhật material từng ô.
/// Index: x + y * 3 (x trái→phải, y dưới→trên).
/// </summary>
public class ObstaclePatternGrid : MonoBehaviour
{
    private readonly bool[] _pattern = new bool[9];
    private Renderer[] _cells;

    public bool GetPatternCell(int index)
    {
        if (index < 0 || index >= 9)
            return false;
        return _pattern[index];
    }

    public void BuildFromShape(
        GridShape3x3 shape,
        GameObject cellPrefab,
        Material activeMaterial,
        Material inactiveMaterial,
        Transform reference,
        float cellSpacingX,
        float cellSpacingY)
    {
        if (shape == null || cellPrefab == null || reference == null)
            return;

        shape.EnsureSize();
        bool[] src = shape.Cells;
        for (int i = 0; i < 9; i++)
            _pattern[i] = src[i];

        BuildCells(cellPrefab, activeMaterial, inactiveMaterial, reference, cellSpacingX, cellSpacingY);
    }

    public void BuildFromBools(
        bool[] pattern9,
        GameObject cellPrefab,
        Material activeMaterial,
        Material inactiveMaterial,
        Transform reference,
        float cellSpacingX,
        float cellSpacingY)
    {
        if (pattern9 == null || pattern9.Length != 9)
            return;

        for (int i = 0; i < 9; i++)
            _pattern[i] = pattern9[i];

        BuildCells(cellPrefab, activeMaterial, inactiveMaterial, reference, cellSpacingX, cellSpacingY);
    }

    private void BuildCells(
        GameObject cellPrefab,
        Material activeMaterial,
        Material inactiveMaterial,
        Transform reference,
        float cellSpacingX,
        float cellSpacingY)
    {
        ClearChildren();

        float offX = (3f - 1f) * cellSpacingX * 0.5f;
        float offY = (3f - 1f) * cellSpacingY * 0.5f;

        _cells = new Renderer[9];
        int k = 0;

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                float lx = x * cellSpacingX - offX;
                float ly = y * cellSpacingY - offY;
                Vector3 worldPos = transform.position + reference.right * lx + reference.up * ly;
                Quaternion worldRot = reference.rotation;

                GameObject cell = Instantiate(cellPrefab, worldPos, worldRot, transform);
                Renderer r = cell.GetComponentInChildren<Renderer>();
                if (r == null)
                    r = cell.AddComponent<MeshRenderer>();

                bool on = _pattern[k];
                if (activeMaterial != null && inactiveMaterial != null)
                    r.sharedMaterial = on ? activeMaterial : inactiveMaterial;

                _cells[k] = r;
                k++;
            }
        }
    }

    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        _cells = null;
    }
}
