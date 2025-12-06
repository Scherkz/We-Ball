using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingData))]
public class BuildingBitmaskDisplay : Editor
{
    private SerializedProperty cellCountProp;
    private SerializedProperty bitmaskProp;

    private void OnEnable()
    {
        cellCountProp = serializedObject.FindProperty("cellCount");
        bitmaskProp = serializedObject.FindProperty("bitmask");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        EditorGUILayout.PropertyField(cellCountProp);

        var width  = Mathf.Max(1, cellCountProp.vector2IntValue.x);
        var height = Mathf.Max(1, cellCountProp.vector2IntValue.y);
        var total  = width * height;

        if (bitmaskProp.arraySize != total)
            bitmaskProp.arraySize = total;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Shape Bitmask", EditorStyles.boldLabel);

        for (int y = height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < width; x++)
            {
                var index = x + y * width;
                SerializedProperty cell = bitmaskProp.GetArrayElementAtIndex(index);

                cell.boolValue = GUILayout.Toggle(cell.boolValue, GUIContent.none, GUILayout.Width(20));
            }

            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
