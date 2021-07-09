////////////////////////////////////////////////////////////////////////
// EditorUtility.cs
// Copyright (C) 2018 by Don Hopkins, Ground Up Software.
// Based on TextMesh Pro TMP_EditorUtility.cs.


using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Bridge {

    public class EditorUtility
    {


        /// <summary>
        /// Returns the relative path of the package.
        /// </summary>
        public static string packageRelativePath
        {
            get
            {
                if (string.IsNullOrEmpty(m_PackagePath))
                    m_PackagePath = GetPackageRelativePath();

                return m_PackagePath;
            }
        }

        [SerializeField] private static string m_PackagePath;


        /// <summary>
        /// Returns the fully qualified path of the package.
        /// </summary>
        public static string packageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_PackageFullPath))
                    m_PackageFullPath = GetPackageFullPath();

                return m_PackageFullPath;
            }
        }

        [SerializeField] private static string m_PackageFullPath;


        // Static Fields Related to locating the Bridge Asset
        private static string folderPath = "Not Found";

        private static EditorWindow Gameview;
        private static bool isInitialized = false;

        private static void GetGameview()
        {
            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            System.Type type = assembly.GetType("UnityEditor.GameView");
            Gameview = EditorWindow.GetWindow(type);
        }


        public static void RepaintAll()
        {
            if (isInitialized == false)
            {
                GetGameview();
                isInitialized = true;
            }

            SceneView.RepaintAll();
            Gameview.Repaint();
        }


        /// <summary>
        /// Create and return a new asset in a smart location based on the current selection and then select it.
        /// </summary>
        /// <param name="name">
        /// Name of the new asset. Do not include the .asset extension.
        /// </param>
        /// <returns>
        /// The new asset.
        /// </returns>
        public static T CreateAsset<T>(string name) where T : ScriptableObject
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path.Length == 0)
            {
                // no asset selected, place in asset root
                path = "Assets/" + name + ".asset";
            }
            else if (Directory.Exists(path))
            {
                // place in currently selected directory
                path += "/" + name + ".asset";
            }
            else
            {
                // place in current selection's containing directory
                path = Path.GetDirectoryName(path) + "/" + name + ".asset";
            }

            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(path));
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return asset;
        }


        private static string GetPackageRelativePath(string subDir="StreamingAssets")
        {
            // Check for potential UPM package
            string packagePath = Path.GetFullPath("Packages/com.unity.textmeshpro");
            if (Directory.Exists(packagePath))
            {
                return "Packages/com.groundupsoftware.bridge.unity";
            }

            packagePath = Path.GetFullPath("Assets/..");
            if (Directory.Exists(packagePath))
            {
                // Search default location for development package
                if (Directory.Exists(packagePath +
                                     "/Assets/Packages/com.groundupsoftware.bridge.unity/" + 
                                     subDir))
                {
                    return "Assets/Packages/com.groundupsoftware.bridge.unity";
                }

                // Search for default location of normal TextMesh Pro AssetStore package
                if (Directory.Exists(packagePath + "/Assets/bridge/" + subDir))
                {
                    return "Assets/StreamingAssets";
                }

                // Search for potential alternative locations in the user project
                string[] matchingPaths = Directory.GetDirectories(packagePath, "bridge", SearchOption.AllDirectories);
                packagePath = ValidateLocation(matchingPaths, packagePath);
                if (packagePath != null) return packagePath;
            }

            return null;
        }

        private static string GetPackageFullPath(string subDir="StreamingAssets")
        {
            // Check for potential UPM package
            string packagePath = Path.GetFullPath("Packages/com.groundupsoftware.bridge.unity");
            if (Directory.Exists(packagePath))
            {
                return packagePath;
            }

            packagePath = Path.GetFullPath("Assets/..");
            if (Directory.Exists(packagePath))
            {
                // Search default location for development package
                if (Directory.Exists(packagePath +
                                     "/Assets/Packages/com.groundupsoftware.bridge.unity/" + subDir))
                {
                    return packagePath + "/Assets/Packages/com.groundupsoftware.bridge.unity";
                }

                // Search for default location of normal TextMesh Pro AssetStore package
                if (Directory.Exists(packagePath + "/Assets/bridge/" + subDir))
                {
                    return packagePath + "/Assets/bridge";
                }

                // Search for potential alternative locations in the user project
                string[] matchingPaths = Directory.GetDirectories(packagePath, "bridge", SearchOption.AllDirectories);
                string path = ValidateLocation(matchingPaths, packagePath);
                if (path != null) return packagePath + path;
            }

            return null;
        }


        [MenuItem("Bridge/Create StreamingAssets and WebGLTemplates bridge Directories")]
        public static void CreateDirectories()
        {
            string streamingAssetsPath = Path.GetFullPath("Assets") + "/StreamingAssets";
            Debug.Log("Bridge.EditorUtility.CreateDirectories: streamingAssetsPath: " + streamingAssetsPath);
        }

    }

}