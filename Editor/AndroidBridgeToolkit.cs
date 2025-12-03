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
        [MenuItem("Tools/Android Bridge Toolkit/🧙 Workflow Wizard (START HERE!)", priority = 0)]
        public static void ShowWorkflowWizard()
        {
            WorkflowWizard.Init();
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Open Main Window", priority = 50)]
        public static void ShowMainWindow()
        {
            MainWindow.Init();
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Advanced/1. Extract APK Classes", priority = 100)]
        public static void ShowDecompiler()
        {
            APKDecompilerWindow.Init();
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Advanced/2. Generate Bridge", priority = 101)]
        public static void ShowBridgeGenerator()
        {
            BridgeGeneratorWindow.Init();
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Advanced/3. Build AAR", priority = 102)]
        public static void ShowAARBuilder()
        {
            AARBuilderWindow.Init();
        }
        
        [MenuItem("Tools/Android Bridge Toolkit/Settings", priority = 200)]
        public static void ShowSettings()
        {
            SettingsWindow.Init();
        }
    }
}
