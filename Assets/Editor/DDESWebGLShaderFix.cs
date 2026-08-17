using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class DDESWebGLShaderFix : EditorWindow
{
    private const string ProblemShaderName =
        "Shader Graphs/S_Vertex Blend";

    private const string ReplacementShaderName =
        "Universal Render Pipeline/Lit";

    private const string BackupFolder =
        "Assets/DDES9903_WebGL_MaterialBackup";

    [MenuItem("Tools/DDES9903/Fix Vertex Blend For WebGL")]
    public static void ShowWindow()
    {
        GetWindow<DDESWebGLShaderFix>(
            "WebGL Shader Fix"
        );
    }

    private void OnGUI()
    {
        GUILayout.Space(12);

        EditorGUILayout.LabelField(
            "DDES9903 WebGL Vertex Blend Fix",
            EditorStyles.boldLabel
        );

        GUILayout.Space(8);

        EditorGUILayout.HelpBox(
            "This tool finds every .mat Material using " +
            ProblemShaderName +
            ", creates a backup, then converts it to URP/Lit.",
            MessageType.Info
        );

        GUILayout.Space(12);

        if (GUILayout.Button(
            "FIX ALL MATERIALS",
            GUILayout.Height(40)
        ))
        {
            FixAllMaterials();
        }
    }

    private void FixAllMaterials()
    {
        Shader problemShader =
            Shader.Find(ProblemShaderName);

        Shader replacementShader =
            Shader.Find(ReplacementShaderName);

        if (problemShader == null)
        {
            Debug.LogError(
                "Problem shader not found: " +
                ProblemShaderName
            );
            return;
        }

        if (replacementShader == null)
        {
            Debug.LogError(
                "URP/Lit shader not found."
            );
            return;
        }

        bool confirmed =
            EditorUtility.DisplayDialog(
                "Fix WebGL Materials?",
                "This will:\n\n" +
                "1. Find all .mat files using S_Vertex Blend\n" +
                "2. Back them up\n" +
                "3. Convert them to URP/Lit\n" +
                "4. Try to preserve Base/Normal textures\n\n" +
                "Continue?",
                "Fix",
                "Cancel"
            );

        if (!confirmed)
            return;

        EnsureBackupFolder();

        string[] materialGuids =
            AssetDatabase.FindAssets("t:Material");

        int fixedCount = 0;

        foreach (string guid in materialGuids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guid);

            // Only touch real .mat assets.
            // Do not touch the .shadergraph itself.
            if (!path.EndsWith(".mat"))
                continue;

            Material mat =
                AssetDatabase.LoadAssetAtPath<Material>(
                    path
                );

            if (mat == null)
                continue;

            if (mat.shader != problemShader)
                continue;

            Debug.Log(
                "FIXING MATERIAL → " +
                mat.name +
                "\n" +
                path,
                mat
            );

            //--------------------------------------------------
            // Remember useful properties BEFORE changing shader
            //--------------------------------------------------

            Texture baseTexture =
                FindBestTexture(
                    mat,
                    mat.shader,
                    new string[]
                    {
                        "base",
                        "albedo",
                        "diffuse",
                        "color1"
                    },
                    new string[]
                    {
                        "normal",
                        "mask",
                        "metal",
                        "rough"
                    }
                );

            Texture normalTexture =
                FindBestTexture(
                    mat,
                    mat.shader,
                    new string[]
                    {
                        "normal"
                    },
                    null
                );

            Color baseColor =
                FindBestColor(
                    mat,
                    mat.shader
                );

            //--------------------------------------------------
            // BACKUP
            //--------------------------------------------------

            string backupName =
                Path.GetFileNameWithoutExtension(path)
                + "_BACKUP.mat";

            string backupPath =
                BackupFolder +
                "/" +
                backupName;

            // Avoid overwriting an existing backup.
            if (!File.Exists(backupPath))
            {
                AssetDatabase.CopyAsset(
                    path,
                    backupPath
                );
            }

            //--------------------------------------------------
            // CHANGE TO URP/LIT
            //--------------------------------------------------

            mat.shader = replacementShader;

            //--------------------------------------------------
            // Restore useful visual properties
            //--------------------------------------------------

            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor(
                    "_BaseColor",
                    baseColor
                );
            }

            if (
                baseTexture != null &&
                mat.HasProperty("_BaseMap")
            )
            {
                mat.SetTexture(
                    "_BaseMap",
                    baseTexture
                );
            }

            if (
                normalTexture != null &&
                mat.HasProperty("_BumpMap")
            )
            {
                mat.SetTexture(
                    "_BumpMap",
                    normalTexture
                );

                mat.EnableKeyword(
                    "_NORMALMAP"
                );
            }

            EditorUtility.SetDirty(mat);

            fixedCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Finished",
            "Fixed " +
            fixedCount +
            " material(s).\n\n" +
            "Backups are stored in:\n" +
            BackupFolder,
            "OK"
        );

        Debug.Log(
            "WEBGL SHADER FIX COMPLETE → " +
            fixedCount +
            " MATERIALS FIXED"
        );
    }

    private void EnsureBackupFolder()
    {
        if (
            AssetDatabase.IsValidFolder(
                BackupFolder
            )
        )
        {
            return;
        }

        AssetDatabase.CreateFolder(
            "Assets",
            "DDES9903_WebGL_MaterialBackup"
        );
    }

    private Texture FindBestTexture(
        Material material,
        Shader shader,
        string[] wantedWords,
        string[] excludedWords)
    {
        if (shader == null)
            return null;

        int propertyCount =
            shader.GetPropertyCount();

        Texture fallback = null;

        for (int i = 0;
             i < propertyCount;
             i++)
        {
            if (
                shader.GetPropertyType(i) !=
                ShaderPropertyType.Texture
            )
            {
                continue;
            }

            string propertyName =
                shader.GetPropertyName(i);

            Texture texture =
                material.GetTexture(propertyName);

            if (texture == null)
                continue;

            if (fallback == null)
                fallback = texture;

            string lowerName =
                propertyName.ToLowerInvariant();

            bool excluded = false;

            if (excludedWords != null)
            {
                foreach (
                    string excludedWord
                    in excludedWords
                )
                {
                    if (
                        lowerName.Contains(
                            excludedWord.ToLowerInvariant()
                        )
                    )
                    {
                        excluded = true;
                        break;
                    }
                }
            }

            if (excluded)
                continue;

            foreach (
                string wantedWord
                in wantedWords
            )
            {
                if (
                    lowerName.Contains(
                        wantedWord.ToLowerInvariant()
                    )
                )
                {
                    return texture;
                }
            }
        }

        return fallback;
    }

    private Color FindBestColor(
        Material material,
        Shader shader)
    {
        Color fallback = Color.white;

        if (shader == null)
            return fallback;

        int propertyCount =
            shader.GetPropertyCount();

        for (int i = 0;
             i < propertyCount;
             i++)
        {
            if (
                shader.GetPropertyType(i) !=
                ShaderPropertyType.Color
            )
            {
                continue;
            }

            string propertyName =
                shader.GetPropertyName(i);

            Color value =
                material.GetColor(
                    propertyName
                );

            string lower =
                propertyName.ToLowerInvariant();

            if (
                lower.Contains("base") ||
                lower.Contains("color1") ||
                lower == "_color"
            )
            {
                return value;
            }

            fallback = value;
        }

        return fallback;
    }
}