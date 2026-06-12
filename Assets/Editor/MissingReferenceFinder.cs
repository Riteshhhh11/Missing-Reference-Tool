using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

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

    private void ScanProject() { //ScanProject has one job find every asset that could contain a missing reference; two assets that can be mainly missing are .prefab and .unity
        string[] allPaths = AssetDatabase.GetAllAssetPaths(); // Get all asset paths in the project
        foreach (string path in allPaths) {
            if (!path.StartsWith("Assets/")) {
                continue; 
            }
            if (path.EndsWith(".prefab")) {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path); // Load the prefab at the given path

                if (prefab != null) {
                    CheckObject(prefab, path);
                }
            }
            else if (path.EndsWith(".unity")) {
                ScanScene(path);
            }
        }
    }

    private void ScanScene(string scenePath) {
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive); // Open the scene additively so we can check it without affecting the currently open scene
        foreach (GameObject root in scene.GetRootGameObjects()) {
            CheckObject(root, scenePath);
        }
        EditorSceneManager.CloseScene(scene, true); // Close the scene after checking it
    }

    private void CheckObject(GameObject gameobject, string assetPath) {
        Component[] components = gameobject.GetComponents<Component>(); // Get all components attached to the gameobject
        foreach (Component component in components) {
            // component itsell can be null if it's script is missing
            if (component == null) {
                results.Add($"[Script is missing]\nAsset: {assetPath}\nGameObject: {gameobject.name}");
                continue;
            }
            // wrap the components in a seralized object so that we can iterate through it's serialized references
            SerializedObject serializedObject = new SerializedObject(component); // Create a serialized object to access the properties of the gameobject
            SerializedProperty property = serializedObject.GetIterator(); // Get an iterator to go through all properties of the gameobject

            while (property.NextVisible(true))
            {
                // Key Check: null value but non-zero instance ID indicates a missing reference
                if (property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    if (property.objectReferenceValue == null && property.objectReferenceInstanceIDValue != 0)
                    {
                        results.Add($"[Missing Reference]\n" +
                                    $"Asset: {assetPath}\n" +
                                    $"GameObject: {gameobject.name}\n" +
                                    $"Component: {component.GetType().Name}\n" +
                                    $"property: {property.displayName}"
                                    );
                    }
                }
            }
        }
        foreach (Transform child in gameobject.transform) {
            CheckObject(child.gameObject, assetPath);
        }
    }
        
}