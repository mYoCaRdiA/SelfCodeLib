using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FontApplier : EditorWindow
{
    private Font fontToApply;

    [MenuItem("Tools/Apply Font to Text Components")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FontApplier));
    }

    void OnGUI()
    {
        GUILayout.Label("Apply Font to Text Components", EditorStyles.boldLabel);

        fontToApply = EditorGUILayout.ObjectField("Font to Apply", fontToApply, typeof(Font), true) as Font;

        if (GUILayout.Button("Apply Font"))
        {
            ApplyFontToTextComponents();
        }
    }

    private void ApplyFontToTextComponents()
    {
        if (fontToApply == null)
        {
            Debug.LogWarning("Font not specified.");
            return;
        }

        Text[] allTextComponents = FindObjectsOfType<Text>();

        foreach (Text textComponent in allTextComponents)
        {
            textComponent.font = fontToApply;
        }

        Debug.Log("Font applied to all Text components.");
    }
}
