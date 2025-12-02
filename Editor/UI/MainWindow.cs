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
        private string[] tabs = { "Welcome", "1. Extract APK", "2. Generate Bridge", "3. Build AAR", "Settings" };
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
                case 1: DrawExtractorTab(); break;
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
                "✅ Complete End-to-End Workflow - Now Available!\n\n" +
                "From APK to Unity - completely self-contained:\n\n" +
                "1. Extract APK → Learn API + Get Native Libraries ✅\n" +
                "2. Build AAR → Create your own wrapper ✅ NEW!\n" +
                "3. Generate Bridge → Auto-generate C# code ✅\n" +
                "4. Use in Unity → Call native functionality ✅\n\n" +
                "💡 No dependency on original APK - create standalone wrappers!",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            GUILayout.Label("✅ Complete Workflow Available", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Step 1: Extract from APK", EditorStyles.boldLabel);
            if (GUILayout.Button("Extract APK Classes & Libraries", GUILayout.Height(40)))
            {
                currentTab = 1;
            }
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Step 2: Create Your Wrapper", EditorStyles.boldLabel);
            if (GUILayout.Button("Build AAR from Java Code", GUILayout.Height(40)))
            {
                currentTab = 3;
            }
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Step 3: Generate C# Bridge", EditorStyles.boldLabel);
            if (GUILayout.Button("Generate Unity Bridge", GUILayout.Height(40)))
            {
                currentTab = 2;
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(20);
            
            GUILayout.Label("Resources", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Complete Workflow Guide"))
            {
                Application.OpenURL("https://github.com/rcgeorge/unity-android-bridge-toolkit/blob/main/COMPLETE_APK_INTEGRATION.md");
            }
            
            if (GUILayout.Button("Quick Start"))
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
        
        void DrawExtractorTab()
        {
            GUILayout.Label("1. Extract APK - Learn API & Get Libraries", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "✅ Extract Everything from APK\n\n" +
                "Class Metadata:\n" +
                "• Learn the SDK's API structure\n" +
                "• Method signatures and types\n" +
                "• Use as reference for your wrapper\n\n" +
                "Native Libraries:\n" +
                "• Extract .so files for all architectures\n" +
                "• Place in Assets/Plugins/Android/libs/\n" +
                "• Link in your AAR wrapper\n\n" +
                "💡 Study the APK, then create your own implementation!",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Open APK Extractor", GUILayout.Height(50)))
            {
                APKDecompilerWindow.Init();
            }
        }
        
        void DrawBridgeGeneratorTab()
        {
            GUILayout.Label("2. Generate Bridge", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "✅ Auto-Generate C# Bridges\n\n" +
                "Use AFTER building your AAR:\n\n" +
                "1. Paste your Java wrapper code\n" +
                "2. Generate C# bridge automatically\n" +
                "3. Save to your Unity project\n" +
                "4. Call native functionality from Unity!\n\n" +
                "Features:\n" +
                "• AndroidJavaClass/Object wrappers\n" +
                "• PascalCase naming\n" +
                "• XML documentation",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Open Bridge Generator", GUILayout.Height(50)))
            {
                BridgeGeneratorWindow.Init();
            }
        }
        
        void DrawAARBuilderTab()
        {
            GUILayout.Label("3. Build AAR - Create Your Wrapper", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "✅ AAR Builder - Now Available!\n\n" +
                "Create standalone wrapper AARs:\n\n" +
                "Workflow:\n" +
                "1. Write Java wrapper code\n" +
                "   • Loads your .so library\n" +
                "   • Declares native methods\n" +
                "   • Based on API learned from APK\n\n" +
                "2. Build AAR with Gradle\n" +
                "   • Compiles Java code\n" +
                "   • Packages into AAR\n" +
                "   • Copies to Unity project\n\n" +
                "3. No dependency on original APK!\n\n" +
                "Requirements:\n" +
                "• Gradle must be installed\n" +
                "• JAVA_HOME must be set",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Open AAR Builder", GUILayout.Height(50)))
            {
                AARBuilderWindow.Init();
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Example workflow:", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "// Step 1: Extract ghostparty.apk\n" +
                "// Learn: IXRHandTracking.enterExclusiveHandTracking()\n" +
                "// Extract: libviture_sdk.so\n\n" +
                "// Step 2: Write VitureSDKWrapper.java\n" +
                "static { System.loadLibrary(\"viture_sdk\"); }\n" +
                "public static native boolean enterExclusiveHandTracking();\n\n" +
                "// Step 3: Build VitureSDKWrapper.aar\n\n" +
                "// Step 4: Generate VitureSDKWrapperBridge.cs\n\n" +
                "// Step 5: Use in Unity!\n" +
                "VitureSDKWrapperBridge.EnterExclusiveHandTracking();",
                MessageType.None
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
