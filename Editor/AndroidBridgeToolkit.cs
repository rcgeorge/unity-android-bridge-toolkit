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
        [MenuItem("Tools/Android Bridge Toolkit")]
        public static void ShowMainWindow()
        {
            MainWindow.Init();
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Decompile APK")]
        public static void ShowDecompiler()
        {
            // APKDecompilerWindow.Init();
            Debug.Log("APK Decompiler - Coming soon!");
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Build AAR")]
        public static void ShowAARBuilder()
        {
            // AARBuilderWindow.Init();
            Debug.Log("AAR Builder - Coming soon!");
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Generate Bridge")]
        public static void ShowBridgeGenerator()
        {
            BridgeGeneratorWindow.Init();
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Settings")]
        public static void ShowSettings()
        {
            SettingsWindow.Init();
        }
    }
}
