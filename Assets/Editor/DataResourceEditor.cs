using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataResource))]
public class DataResourceEditor : Editor
{
    private SerializedProperty displayName;
    private SerializedProperty image;

    private Texture2D texture;

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
        EditorGUILayout.ObjectField("Image", ((DataResource)target).Image, typeof(Texture2D), false);
        EditorGUILayout.Space(10);

        serializedObject.ApplyModifiedProperties(); // Write back changed values, mark as dirty and handle undo/redo
    }
}
