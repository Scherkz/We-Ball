using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingData))]
public class BuildingDataCustomEditor : Editor
{
    private SerializedProperty cellCountProperty;
    private SerializedProperty bitmaskProperty;

    private bool defaultBitMask;
    private void OnEnable()
    {
        cellCountProperty = serializedObject.FindProperty("cellCount");
        bitmaskProperty = serializedObject.FindProperty("bitmask");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        var width  = Mathf.Max(1, cellCountProperty.vector2IntValue.x);
        var height = Mathf.Max(1, cellCountProperty.vector2IntValue.y);
        var total  = width * height;

        if (bitmaskProperty.arraySize != total)
        {
            bitmaskProperty.arraySize = total;
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Shape Bitmask", EditorStyles.boldLabel);

        //initializes the bitmask with all true
        if (!defaultBitMask)
        {
            defaultBitMask = true;
            for (int y = height - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();

                for (int x = 0; x < width; x++)
                {
                    var index = x + y * width;
                    bitmaskProperty.GetArrayElementAtIndex(index).boolValue = true;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        for (int y = height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < width; x++)
            {
                var index = x + y * width;
                SerializedProperty cell = bitmaskProperty.GetArrayElementAtIndex(index);
                cell.boolValue = GUILayout.Toggle(cell.boolValue, GUIContent.none, GUILayout.Width(20));
            }

            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
