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
        private string[] tabs = { "Welcome", "Bridge Generator", "AAR Builder", "APK Decompiler", "Settings" };
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
                case 1: DrawBridgeGeneratorTab(); break;
                case 2: DrawAARBuilderTab(); break;
                case 3: DrawDecompilerTab(); break;
                case 4: DrawSettingsTab(); break;
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            GUILayout.Label("🔨 Android Bridge Toolkit", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("📖 Docs", EditorStyles.toolbarButton))
            {
                Application.OpenURL("https://github.com/rcgeorge/unity-android-bridge-toolkit/wiki");
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
                "Create Android bridges for Unity in minutes!\n\n" +
                "Features:\n" +
                "• Generate C# bridges from Java code automatically\n" +
                "• Build AARs without Android Studio\n" +
                "• Decompile APKs to study implementations\n" +
                "• Complete end-to-end workflow",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            GUILayout.Label("Quick Start", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Generate C# Bridge from Java", GUILayout.Height(40)))
            {
                currentTab = 1;
            }
            
            if (GUILayout.Button("Build AAR Library", GUILayout.Height(40)))
            {
                currentTab = 2;
            }
            
            if (GUILayout.Button("Decompile APK", GUILayout.Height(40)))
            {
                currentTab = 3;
            }
            
            EditorGUILayout.Space();
            
            GUILayout.Label("Resources", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL("https://github.com/rcgeorge/unity-android-bridge-toolkit/wiki");
            }
            
            if (GUILayout.Button("Examples"))
            {
                Application.OpenURL("https://github.com/rcgeorge/unity-android-bridge-toolkit/tree/main/Samples~");
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
            
            GUILayout.Label("Made with ❤️ by Instemic", centeredStyle);
            GUILayout.Label("v1.0.0", centeredStyle);
        }
        
        void DrawBridgeGeneratorTab()
        {
            GUILayout.Label("Bridge Generator", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "Generate C# Unity bridge code from Java source automatically.",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Open Bridge Generator Window", GUILayout.Height(40)))
            {
                BridgeGeneratorWindow.Init();
            }
        }
        
        void DrawAARBuilderTab()
        {
            GUILayout.Label("AAR Builder", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "Build Android AAR libraries without Android Studio.\n\n" +
                "Coming soon!",
                MessageType.Info
            );
        }
        
        void DrawDecompilerTab()
        {
            GUILayout.Label("APK Decompiler", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "Decompile APK files to study implementations.\n\n" +
                "Coming soon!",
                MessageType.Info
            );
        }
        
        void DrawSettingsTab()
        {
            GUILayout.Label("Settings", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "Configure Android Bridge Toolkit settings.\n\n" +
                "Coming soon!",
                MessageType.Info
            );
        }
    }
}
