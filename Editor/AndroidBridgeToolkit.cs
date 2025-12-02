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
        
        [MenuItem("Tools/Android Bridge Toolkit/1. Decompile APK", priority = 100)]
        public static void ShowDecompiler()
        {
            // APKDecompilerWindow.Init();
            Debug.Log("APK Decompiler - Coming in v1.1!");
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/2. Generate Bridge", priority = 101)]
        public static void ShowBridgeGenerator()
        {
            BridgeGeneratorWindow.Init();
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/3. Build AAR", priority = 102)]
        public static void ShowAARBuilder()
        {
            // AARBuilderWindow.Init();
            Debug.Log("AAR Builder - Coming in v1.1!");
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Settings", priority = 200)]
        public static void ShowSettings()
        {
            SettingsWindow.Init();
        }
    }
}
