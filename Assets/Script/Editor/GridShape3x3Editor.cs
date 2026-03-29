#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridShape3x3))]
public class GridShape3x3Editor : Editor
{
    private SerializedProperty _displayNameProp;
    private SerializedProperty _cellsProp;

    private void OnEnable()
    {
        _displayNameProp = serializedObject.FindProperty("displayName");
        _cellsProp = serializedObject.FindProperty("cells");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_displayNameProp);

        EditorGUILayout.Space(6f);
        EditorGUILayout.LabelField("Lưới 3×3 (hàng trên = y=2, hàng dưới = y=0)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Ô bật = active (hình xanh). Trái→phải, dưới→trên khớp với index x + y·3.", MessageType.None);

        if (_cellsProp == null || !_cellsProp.isArray)
        {
            EditorGUILayout.HelpBox("Mảng cells không hợp lệ.", MessageType.Error);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        while (_cellsProp.arraySize < 9)
            _cellsProp.InsertArrayElementAtIndex(_cellsProp.arraySize);
        while (_cellsProp.arraySize > 9)
            _cellsProp.DeleteArrayElementAtIndex(_cellsProp.arraySize - 1);

        for (int displayRow = 0; displayRow < 3; displayRow++)
        {
            int y = 2 - displayRow;
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < 3; x++)
            {
                int idx = x + y * 3;
                SerializedProperty el = _cellsProp.GetArrayElementAtIndex(idx);
                bool v = el.boolValue;
                GUI.backgroundColor = v ? new Color(0.45f, 0.65f, 1f) : Color.white;
                if (GUILayout.Button(v ? "■" : "□", GUILayout.Height(32f), GUILayout.Width(32f)))
                    el.boolValue = !v;
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(4f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Xóa hết"))
        {
            for (int i = 0; i < 9; i++)
                _cellsProp.GetArrayElementAtIndex(i).boolValue = false;
        }
        if (GUILayout.Button("Bật hết"))
        {
            for (int i = 0; i < 9; i++)
                _cellsProp.GetArrayElementAtIndex(i).boolValue = true;
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
