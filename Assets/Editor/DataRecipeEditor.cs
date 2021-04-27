using UnityEditor;
using UnityEditorInternal; // Para usar ReorderableList
using UnityEngine;

[CustomEditor(typeof(DataRecipe))]
public class DataRecipeEditor : Editor
{
    private SerializedProperty displayName;
    private SerializedProperty requisites;
    private SerializedProperty reward;

    private ReorderableList list;

    void OnEnable()
    {
        displayName = serializedObject.FindProperty("displayName");
        requisites = serializedObject.FindProperty("requisites");
        reward = serializedObject.FindProperty("reward");

        list = new ReorderableList(serializedObject, requisites, true, true, true, true);
        list.drawHeaderCallback = DrawHeader;
        list.drawElementCallback = DrawListItems;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Load the real class values into the serialized copy

        EditorGUILayout.PropertyField(displayName);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(reward);
        EditorGUILayout.Space(10);
        list.DoLayoutList();

        serializedObject.ApplyModifiedProperties(); // Write back changed values, mark as dirty and handle undo/redo
    }

    private void DrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Requisites");
    }

    private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); //The element in the list

        EditorGUI.PropertyField
        (
            new Rect(rect.x, rect.y, 150, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("type"),
            GUIContent.none
        );

        EditorGUI.PropertyField
        (
            new Rect(rect.x + 160, rect.y, 30, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("quantity"),
            GUIContent.none
        );
    }
}
