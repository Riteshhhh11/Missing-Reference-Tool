using UnityEngine;
using UnityEditor;
using System.Collections.Generic; // Let's you access libraries for editor scripting

public class MissingReferenceEditor : EditorWindow {

    private List<string> results = new List<string>();
    private Vector2 scrollPos;
    [MenuItem("Tools/Missing Reference Finder")] // Adds a menu item to the Unity Editor under "Tools" called "Missing Reference Finder"
    public static void OpenWindow() { // Because it's static unity can run this method directly from the menu
        GetWindow<MissingReferenceEditor>("Missing Reference Finder"); // Opens the editor window with the title "Missing Reference Finder"
    }

    private void OnGUI() // This method is called to draw the GUI of the editor window
    {
        if(GUILayout.Button("Scan Project")) {
            results.Clear();
            ScanProject();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Results: {results.Count}", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var r in results) {
            EditorGUILayout.HelpBox(r, MessageType.Warning);
        }

        EditorGUILayout.EndScrollView();
    }

    private void ScanProject() {
        results.Add("Logic to be added yet");
    }
}