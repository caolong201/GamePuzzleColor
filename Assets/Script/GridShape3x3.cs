using UnityEngine;

/// <summary>
/// Pattern 3x3: index = x + y * 3 với x = 0 trái → phải, y = 0 hàng dưới → y = 2 hàng trên.
/// true = ô active (xanh), false = inactive (xám/lavender).
/// </summary>
[CreateAssetMenu(fileName = "GridShape3x3", menuName = "GamePuzzle/Grid Shape 3x3", order = 0)]
public class GridShape3x3 : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private bool[] cells = new bool[9];

    public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;

    public bool[] Cells
    {
        get
        {
            EnsureSize();
            return cells;
        }
    }

    public bool GetCell(int flatIndex)
    {
        EnsureSize();
        if (flatIndex < 0 || flatIndex >= 9)
            return false;
        return cells[flatIndex];
    }

    public void EnsureSize()
    {
        if (cells == null || cells.Length != 9)
            cells = new bool[9];
    }

    private void OnValidate()
    {
        EnsureSize();
    }
}
