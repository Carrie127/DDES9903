using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Profile;
using UnityEngine.SceneManagement;

public class DDESMaterialReplacer : EditorWindow
{
    private string shaderName = "Shader Graphs/S_Vertex Blend";

    private Vector2 scroll;

    private class FoundItem
    {
        public string type;
        public string scene;
        public string objectPath;
        public string materialName;
        public string materialPath;
    }

    private readonly List<FoundItem> results =
        new List<FoundItem>();

    private string status = "Not scanned yet.";

    [MenuItem("Tools/DDES9903/Find Problem Shader")]
    public static void ShowWindow()
    {
        GetWindow<DDESMaterialReplacer>(
            "Problem Shader Finder"
        );
    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        EditorGUILayout.LabelField(
            "DDES9903 WebGL Shader Finder",
            EditorStyles.boldLabel
        );

        GUILayout.Space(8);

        shaderName = EditorGUILayout.TextField(
            "Shader Name",
            shaderName
        );

        GUILayout.Space(10);

        if (GUILayout.Button(
            "FIND EVERYTHING USING THIS SHADER",
            GUILayout.Height(36)
        ))
        {
            FindEverything();
        }

        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            status,
            MessageType.Info
        );

        if (results.Count == 0)
            return;

        scroll = EditorGUILayout.BeginScrollView(scroll);

        for (int i = 0; i < results.Count; i++)
        {
            FoundItem item = results[i];

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField(
                (i + 1) + ". " + item.type,
                EditorStyles.boldLabel
            );

            if (!string.IsNullOrEmpty(item.scene))
            {
                EditorGUILayout.LabelField(
                    "Scene:",
                    item.scene
                );
            }

            if (!string.IsNullOrEmpty(item.objectPath))
            {
                EditorGUILayout.LabelField(
                    "Object:",
                    item.objectPath
                );
            }

            EditorGUILayout.LabelField(
                "Material:",
                item.materialName
            );

            EditorGUILayout.LabelField(
                "Material Path:",
                item.materialPath
            );

            if (GUILayout.Button("PING MATERIAL"))
            {
                Material mat =
                    AssetDatabase.LoadAssetAtPath<Material>(
                        item.materialPath
                    );

                if (mat != null)
                {
                    Selection.activeObject = mat;
                    EditorGUIUtility.PingObject(mat);
                }
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }

    private void FindEverything()
    {
        results.Clear();

        Shader targetShader = Shader.Find(shaderName);

        if (targetShader == null)
        {
            status =
                "Shader not found: " +
                shaderName;

            Debug.LogError(status);
            Repaint();
            return;
        }

        Debug.Log(
            "FOUND SHADER → " +
            targetShader.name
        );

        // =================================================
        // 1. FIND ALL MATERIAL ASSETS USING THIS SHADER
        // =================================================

        string[] materialGuids =
            AssetDatabase.FindAssets("t:Material");

        HashSet<Material> problemMaterials =
            new HashSet<Material>();

        foreach (string guid in materialGuids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guid);

            Material mat =
                AssetDatabase.LoadAssetAtPath<Material>(
                    path
                );

            if (mat == null)
                continue;

            if (mat.shader != targetShader)
                continue;

            problemMaterials.Add(mat);

            results.Add(
                new FoundItem
                {
                    type = "MATERIAL ASSET",
                    materialName = mat.name,
                    materialPath = path
                }
            );

            Debug.Log(
                "PROBLEM MATERIAL → " +
                mat.name +
                "\n" +
                path,
                mat
            );
        }

        // =================================================
        // 2. FIND USES IN ACTIVE BUILD PROFILE SCENES
        // =================================================

        if (!EditorSceneManager
            .SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        string originalScenePath =
            SceneManager.GetActiveScene().path;

        EditorBuildSettingsScene[] buildScenes =
            GetBuildScenes();

        foreach (
            EditorBuildSettingsScene buildScene
            in buildScenes
        )
        {
            if (buildScene == null ||
                !buildScene.enabled ||
                string.IsNullOrEmpty(buildScene.path))
            {
                continue;
            }

            Scene scene =
                EditorSceneManager.OpenScene(
                    buildScene.path,
                    OpenSceneMode.Single
                );

            Renderer[] renderers =
                Object.FindObjectsByType<Renderer>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None
                );

            foreach (Renderer renderer in renderers)
            {
                Material[] mats =
                    renderer.sharedMaterials;

                foreach (Material mat in mats)
                {
                    if (mat == null)
                        continue;

                    // IMPORTANT:
                    // Compare shader directly,
                    // not one specific material asset.
                    if (mat.shader != targetShader)
                        continue;

                    string matPath =
                        AssetDatabase.GetAssetPath(mat);

                    results.Add(
                        new FoundItem
                        {
                            type = "SCENE OBJECT",
                            scene = scene.name,
                            objectPath =
                                GetHierarchyPath(
                                    renderer.transform
                                ),
                            materialName = mat.name,
                            materialPath = matPath
                        }
                    );

                    Debug.Log(
                        "SCENE USE → " +
                        scene.name +
                        "\nObject: " +
                        GetHierarchyPath(
                            renderer.transform
                        ) +
                        "\nMaterial: " +
                        mat.name,
                        renderer.gameObject
                    );
                }
            }
        }

        // =================================================
        // RESTORE ORIGINAL SCENE
        // =================================================

        if (!string.IsNullOrEmpty(originalScenePath))
        {
            EditorSceneManager.OpenScene(
                originalScenePath,
                OpenSceneMode.Single
            );
        }

        status =
            "Found " +
            problemMaterials.Count +
            " material(s) using the shader, " +
            "and " +
            CountSceneUses() +
            " scene object use(s).";

        Debug.Log(
            "========== SHADER SEARCH COMPLETE ==========\n" +
            status
        );

        Repaint();
    }

    private int CountSceneUses()
    {
        int count = 0;

        foreach (FoundItem item in results)
        {
            if (item.type == "SCENE OBJECT")
                count++;
        }

        return count;
    }

    private EditorBuildSettingsScene[] GetBuildScenes()
    {
        BuildProfile profile =
            BuildProfile.GetActiveBuildProfile();

        if (profile != null)
            return profile.GetScenesForBuild();

        return EditorBuildSettings.scenes;
    }

    private string GetHierarchyPath(
        Transform transform)
    {
        string path = transform.name;

        while (transform.parent != null)
        {
            transform = transform.parent;

            path =
                transform.name +
                "/" +
                path;
        }

        return path;
    }
}