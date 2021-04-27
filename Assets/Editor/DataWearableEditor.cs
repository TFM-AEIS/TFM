using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataWearable))]
public class DataWearableEditor : Editor
{
    private SerializedProperty displayName;
    private SerializedProperty image;

    void OnEnable()
    {
        displayName = serializedObject.FindProperty("displayName");
        image = serializedObject.FindProperty("image");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Load the real class values into the serialized copy

        EditorGUILayout.PropertyField(displayName);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(image);
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(new GUIContent(((DataWearable)target).Image), GUILayout.Height(250));

        serializedObject.ApplyModifiedProperties(); // Write back changed values, mark as dirty and handle undo/redo
    }
}
