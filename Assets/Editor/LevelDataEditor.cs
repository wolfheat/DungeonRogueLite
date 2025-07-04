using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseLevelData),true)]
public class LevelDataEditor : Editor
{
    private GUIStyle _monospaceStyle;

    private void OnEnable()
    {
        _monospaceStyle = new GUIStyle(EditorStyles.textArea)
        {
            font = Font.CreateDynamicFontFromOSFont("Courier New", 10),
            fontSize = 10,
            wordWrap = false
        };
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //LevelData data = (LevelData)target;
        BaseLevelData data = (BaseLevelData)target;

        EditorGUILayout.LabelField("Level Data", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        string newText = EditorGUILayout.TextArea(data.LevelString, _monospaceStyle, GUILayout.Height(550));
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(data, "Edit Level Data");
            data.LevelString = newText;
            EditorUtility.SetDirty(data);
        }
    }
}