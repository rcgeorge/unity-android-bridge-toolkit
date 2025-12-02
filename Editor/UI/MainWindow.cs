//
// MainWindow.cs - Main toolkit window with workflow tabs
// Copyright (c) 2024 Instemic
//

using UnityEngine;
using UnityEditor;

namespace Instemic.AndroidBridge
{
    /// <summary>
    /// Main window for Android Bridge Toolkit
    /// </summary>
    public class MainWindow : EditorWindow
    {
        private int currentTab = 0;
        private string[] tabs = { "Welcome", "1. Decompile APK", "2. Generate Bridge", "3. Build AAR", "Settings" };
        private Vector2 scrollPos;
        
        public static void Init()
        {
            MainWindow window = GetWindow<MainWindow>("Android Bridge Toolkit");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }
        
        void OnGUI()
        {
            DrawHeader();
            
            EditorGUILayout.Space();
            
            currentTab = GUILayout.Toolbar(currentTab, tabs, GUILayout.Height(30));
            
            EditorGUILayout.Space();
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            switch (currentTab)
            {
                case 0: DrawWelcomeTab(); break;
                case 1: DrawDecompilerTab(); break;
                case 2: DrawBridgeGeneratorTab(); break;
                case 3: DrawAARBuilderTab(); break;
                case 4: DrawSettingsTab(); break;
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            GUILayout.Label("🔨 Android Bridge Toolkit v1.0.0", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("📖 Docs", EditorStyles.toolbarButton))
            {
                Application.OpenURL("https://github.com/rcgeorge/unity-android-bridge-toolkit");
            }
            
            if (GUILayout.Button("⭐ Star on GitHub", EditorStyles.toolbarButton))
            {
                Application.OpenURL("https://github.com/rcgeorge/unity-android-bridge-toolkit");
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        void DrawWelcomeTab()
        {
            GUILayout.Label("Welcome to Android Bridge Toolkit", EditorStyles.largeLabel);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "Complete workflow for Unity Android development!\n\n" +
                "Workflow:\n" +
                "1. Decompile APK → Study existing implementations (Coming v1.1)\n" +
                "2. Generate Bridge → Convert Java to C# automatically (✅ Available Now!)\n" +
                "3. Build AAR → Package without Android Studio (Coming v1.1)\n\n" +
                "Start with the Bridge Generator to create C# bridges from Java code!",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            GUILayout.Label("✅ Available Now", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Generate C# Bridge from Java Code", GUILayout.Height(50)))
            {
                currentTab = 2; // Bridge Generator tab
            }
            
            EditorGUILayout.Space();
            
            GUILayout.Label("🚧 Coming in v1.1", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.Button("Decompile APK", GUILayout.Height(40));
            GUILayout.Button("Build AAR Library", GUILayout.Height(40));
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(20);
            
            GUILayout.Label("Resources", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Quick Start Guide"))
            {
                Application.OpenURL("https://github.com/rcgeorge/unity-android-bridge-toolkit/blob/main/Documentation/QuickStart.md");
            }
            
            if (GUILayout.Button("Report Issue"))
            {
                Application.OpenURL("https://github.com/rcgeorge/unity-android-bridge-toolkit/issues/new");
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(20);
            
            GUIStyle centeredStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic
            };
            
            GUILayout.Label("Made with ❤️ by Instemic for Viture XR", centeredStyle);
        }
        
        void DrawDecompilerTab()
        {
            GUILayout.Label("1. Decompile APK", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "📦 APK Decompiler\n\n" +
                "Decompile Android APK files to extract Java source code.\n\n" +
                "Use this to:\n" +
                "• Study existing Android implementations\n" +
                "• Extract classes from third-party apps\n" +
                "• Generate bridges from decompiled code\n\n" +
                "🚧 Coming in v1.1!\n\n" +
                "For now, use JADX manually:\n" +
                "Download: https://github.com/skylot/jadx",
                MessageType.Warning
            );
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Download JADX", GUILayout.Height(40)))
            {
                Application.OpenURL("https://github.com/skylot/jadx/releases");
            }
        }
        
        void DrawBridgeGeneratorTab()
        {
            GUILayout.Label("2. Generate Bridge", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "✅ Bridge Generator - Available Now!\n\n" +
                "Automatically generate C# Unity bridge code from Java source.\n\n" +
                "Features:\n" +
                "• Parse Java classes and methods\n" +
                "• Generate C# AndroidJavaClass/Object bridges\n" +
                "• Handle static and instance methods\n" +
                "• XML documentation comments\n" +
                "• PascalCase method naming",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Open Bridge Generator", GUILayout.Height(50)))
            {
                BridgeGeneratorWindow.Init();
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Example Input (Java):", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(
                "package com.example;\n" +
                "public class SDK {\n" +
                "    public static void init() { }\n" +
                "    public static String getMessage() { return \"Hello\"; }\n" +
                "}",
                GUILayout.Height(80)
            );
            
            EditorGUILayout.LabelField("Example Output (C#):", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(
                "public class SDKBridge {\n" +
                "    private static AndroidJavaClass javaClass;\n" +
                "    static SDKBridge() {\n" +
                "        javaClass = new AndroidJavaClass(\"com.example.SDK\");\n" +
                "    }\n" +
                "    public static void Init() {\n" +
                "        javaClass.CallStatic(\"init\");\n" +
                "    }\n" +
                "    public static string GetMessage() {\n" +
                "        return javaClass.CallStatic<string>(\"getMessage\");\n" +
                "    }\n" +
                "}",
                GUILayout.Height(150)
            );
        }
        
        void DrawAARBuilderTab()
        {
            GUILayout.Label("3. Build AAR", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "🔨 AAR Builder\n\n" +
                "Build Android AAR libraries without Android Studio.\n\n" +
                "Use this to:\n" +
                "• Package your Java bridge implementations\n" +
                "• Create plugins from generated code\n" +
                "• Build AARs with Gradle directly\n\n" +
                "🚧 Coming in v1.1!",
                MessageType.Warning
            );
        }
        
        void DrawSettingsTab()
        {
            GUILayout.Label("Settings", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "⚙️ Configure Android Bridge Toolkit\n\n" +
                "Settings available:\n" +
                "• Code generation preferences\n" +
                "• Output directories\n" +
                "• Java/Gradle paths\n\n" +
                "Click below to open full settings.",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Open Settings Window", GUILayout.Height(40)))
            {
                SettingsWindow.Init();
            }
        }
    }
}
