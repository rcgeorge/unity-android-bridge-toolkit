//
// Android Bridge Toolkit - Main Entry Point
// Copyright (c) 2024 Instemic
// https://github.com/rcgeorge/unity-android-bridge-toolkit
//

using UnityEngine;
using UnityEditor;

namespace Instemic.AndroidBridge
{
    /// <summary>
    /// Main menu entry for Android Bridge Toolkit
    /// </summary>
    public static class AndroidBridgeToolkit
    {
        [MenuItem("Tools/Android Bridge Toolkit/Open Main Window", priority = 0)]
        public static void ShowMainWindow()
        {
            MainWindow.Init();
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Generate Bridge", priority = 100)]
        public static void ShowBridgeGenerator()
        {
            BridgeGeneratorWindow.Init();
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Build AAR", priority = 200)]
        public static void ShowAARBuilder()
        {
            // AARBuilderWindow.Init();
            Debug.Log("AAR Builder - Coming soon!");
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Decompile APK", priority = 201)]
        public static void ShowDecompiler()
        {
            // APKDecompilerWindow.Init();
            Debug.Log("APK Decompiler - Coming soon!");
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Settings", priority = 300)]
        public static void ShowSettings()
        {
            SettingsWindow.Init();
        }
    }
}
