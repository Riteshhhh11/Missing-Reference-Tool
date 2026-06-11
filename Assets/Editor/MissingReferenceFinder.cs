using UnityEngine;
using UnityEditor; // Let's you access libraries for editor scripting

public class MissingReferenceEditor : EditorWindow {
    [MenuItem("Tools/Missing Reference Finder")] // Adds a menu item to the Unity Editor under "Tools" called "Missing Reference Finder"
    public static void OpenWindow() { // Because it's static unity can run this method directly from the menu
        GetWindow<MissingReferenceEditor>("Missing Reference Finder"); // Opens the editor window with the title "Missing Reference Finder"
    }

    private void OnGUI() // This method is called to draw the GUI of the editor window
    {
        if(GUILayout.Button("Scan Project")) {
            Debug.Log("Button Clickeds");
        }
    }
}