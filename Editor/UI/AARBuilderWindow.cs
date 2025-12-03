//
// AARBuilderWindow.cs - UI for building AAR libraries
// Copyright (c) 2024 Instemic
//

using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Instemic.AndroidBridge.Core;

namespace Instemic.AndroidBridge
{
    /// <summary>
    /// Window for building Android AAR libraries from Java code - NO GRADLE REQUIRED!
    /// </summary>
    public class AARBuilderWindow : EditorWindow
    {
        private string projectName = "VitureWrapper";
        private string packageName = "com.yourcompany.viture";
        private string javaCode = "";
        
        private bool isBuilding = false;
        private float progress = 0f;
        private string progressMessage = "";
        private string buildLog = "";
        
        private Vector2 scrollPos;
        private Vector2 codeScrollPos;
        
        private int selectedTab = 0;
        private string[] tabs = { "Java Code", "Settings", "Build" };
        
        public static void Init()
        {
            AARBuilderWindow window = GetWindow<AARBuilderWindow>("AAR Builder");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }
        
        void OnEnable()
        {
            // Load default template
            if (string.IsNullOrEmpty(javaCode))
            {
                LoadTemplate();
            }
        }
        
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            DrawHeader();
            EditorGUILayout.Space();
            
            selectedTab = GUILayout.Toolbar(selectedTab, tabs, GUILayout.Height(30));
            EditorGUILayout.Space();
            
            switch (selectedTab)
            {
                case 0: DrawJavaCodeTab(); break;
                case 1: DrawSettingsTab(); break;
                case 2: DrawBuildTab(); break;
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        void DrawHeader()
        {
            GUILayout.Label("🔨 AAR Builder - No Gradle Required!", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "✅ Build AAR libraries using only javac + C#\n\n" +
                "Workflow:\n" +
                "1. Write Java wrapper code (loads native .so library)\n" +
                "2. Configure project settings\n" +
                "3. Build AAR (uses javac, not Gradle!)\n" +
                "4. Use with C# bridges in Unity!\n\n" +
                "Requirements: Only JDK (Unity already needs this for Android)",
                MessageType.Info
            );
        }
        
        void DrawJavaCodeTab()
        {
            GUILayout.Label("Java Wrapper Code", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("📋 Load Template", GUILayout.Width(120)))
            {
                LoadTemplate();
            }
            
            if (GUILayout.Button("💾 Save Code", GUILayout.Width(120)))
            {
                SaveJavaCode();
            }
            
            if (GUILayout.Button("📂 Load File", GUILayout.Width(120)))
            {
                LoadJavaFile();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Edit your Java wrapper code below:");
            
            codeScrollPos = EditorGUILayout.BeginScrollView(codeScrollPos, GUILayout.Height(400));
            javaCode = EditorGUILayout.TextArea(javaCode, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "💡 Template Guide:\n" +
                "• System.loadLibrary(\"library_name\") - Loads your .so file\n" +
                "• native methods - Declare methods that call the .so\n" +
                "• Method signatures must match the native library!",
                MessageType.Info
            );
        }
        
        void DrawSettingsTab()
        {
            GUILayout.Label("Project Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            
            projectName = EditorGUILayout.TextField("Project Name:", projectName);
            packageName = EditorGUILayout.TextField("Package Name:", packageName);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "Project Name: Name of the AAR file (e.g., 'VitureWrapper')\n" +
                "Package Name: Java package (e.g., 'com.yourcompany.viture')\n\n" +
                "The package name should match the package declaration in your Java code!",
                MessageType.None
            );
            
            EditorGUILayout.Space(20);
            
            GUILayout.Label("Output Configuration", EditorStyles.boldLabel);
            
            EditorGUILayout.LabelField("AAR will be output to:", EditorStyles.miniLabel);
            EditorGUILayout.LabelField("Assets/Plugins/Android/" + projectName + ".aar", EditorStyles.boldLabel);
        }
        
        void DrawBuildTab()
        {
            GUILayout.Label("Build AAR Library", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            
            if (!isBuilding)
            {
                EditorGUILayout.HelpBox(
                    "✅ Ready to build - No Gradle required!\n\n" +
                    "This will:\n" +
                    "1. Compile Java code with javac\n" +
                    "2. Package classes into JAR\n" +
                    "3. Create AAR structure (ZIP format)\n" +
                    "4. Copy to Assets/Plugins/Android/\n\n" +
                    "Requirements:\n" +
                    "• JDK installed (Unity needs this anyway)\n" +
                    "• JAVA_HOME environment variable set",
                    MessageType.Info
                );
                
                EditorGUILayout.Space();
                
                if (GUILayout.Button("🚀 Build AAR (Native - No Gradle!)", GUILayout.Height(50)))
                {
                    StartBuild();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Building...", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Progress:", progressMessage);
                EditorGUI.ProgressBar(GUILayoutUtility.GetRect(18, 18), progress, $"{(int)(progress * 100)}%");
            }
            
            if (!string.IsNullOrEmpty(buildLog))
            {
                EditorGUILayout.Space();
                GUILayout.Label("Build Log:", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.TextArea(buildLog, GUILayout.Height(200));
                EditorGUILayout.EndVertical();
            }
        }
        
        void LoadTemplate()
        {
            javaCode = @"package " + packageName + @";

/**
 * Native wrapper for Viture SDK
 * 
 * This class loads the native library and declares methods
 * that call into the .so file.
 * 
 * IMPORTANT: Method signatures must match exactly what the
 * native library expects!
 */
public class VitureSDKWrapper {
    
    // Load the native library
    // Make sure libviture_sdk.so is in Assets/Plugins/Android/libs/[arch]/
    static {
        System.loadLibrary(""viture_sdk"");
    }
    
    // Declare native methods - these call the .so directly
    // You learned these signatures by extracting ghostparty.apk!
    
    /**
     * Enable exclusive hand tracking mode
     * @return true if successful
     */
    public static native boolean enterExclusiveHandTracking();
    
    /**
     * Disable exclusive hand tracking mode
     * @return true if successful
     */
    public static native boolean exitExclusiveHandTracking();
    
    /**
     * Register a hand tracking listener
     */
    public static native void registerHandTrackingListener();
    
    /**
     * Unregister hand tracking listener
     */
    public static native void unregisterHandTrackingListener();
    
    // Add more native methods as needed...
    // Study the APK to learn the correct signatures!
}
";
        }
        
        void SaveJavaCode()
        {
            var path = EditorUtility.SaveFilePanel("Save Java File", "", "VitureSDKWrapper.java", "java");
            
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, javaCode);
                Debug.Log($"Java code saved to: {path}");
                EditorUtility.DisplayDialog("Saved", "Java code saved successfully!", "OK");
            }
        }
        
        void LoadJavaFile()
        {
            var path = EditorUtility.OpenFilePanel("Load Java File", "", "java");
            
            if (!string.IsNullOrEmpty(path))
            {
                javaCode = File.ReadAllText(path);
                Debug.Log($"Java code loaded from: {path}");
            }
        }
        
        void StartBuild()
        {
            var config = new AARBuilderNative.BuildConfig
            {
                ProjectName = projectName,
                PackageName = packageName,
                JavaCode = javaCode,
                CopyToPlugins = true
            };
            
            isBuilding = true;
            progress = 0f;
            progressMessage = "Starting build...";
            buildLog = "";
            
            try
            {
                var result = AARBuilderNative.BuildAAR(config, OnProgress);
                OnBuildComplete(result);
            }
            catch (Exception ex)
            {
                OnBuildError(ex.Message);
            }
        }
        
        void OnProgress(float p, string message)
        {
            progress = p;
            progressMessage = message;
            Repaint();
        }
        
        void OnBuildComplete(AARBuilderNative.BuildResult result)
        {
            isBuilding = false;
            progress = 1f;
            progressMessage = "Complete!";
            buildLog = result.BuildLog;
            
            if (result.Success)
            {
                Debug.Log($"AAR build complete: {result.AARPath}");
                
                EditorUtility.DisplayDialog(
                    "Build Complete!",
                    result.Message + "\n\nAAR Location:\n" + result.AARPath + "\n\nNow generate a C# bridge to use it!",
                    "OK"
                );
                
                // Refresh assets
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError($"AAR build failed: {result.Message}");
                
                EditorUtility.DisplayDialog(
                    "Build Failed",
                    result.Message,
                    "OK"
                );
            }
            
            Repaint();
        }
        
        void OnBuildError(string error)
        {
            isBuilding = false;
            progress = 0f;
            progressMessage = "";
            buildLog = error;
            
            Debug.LogError($"AAR build error: {error}");
            
            EditorUtility.DisplayDialog(
                "Build Error",
                "Build failed: " + error + "\n\nCheck Console for details.",
                "OK"
            );
            
            Repaint();
        }
    }
}
