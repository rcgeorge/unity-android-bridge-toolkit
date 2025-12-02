//
// SettingsWindow.cs - Toolkit settings
// Copyright (c) 2024 Instemic
//

using UnityEngine;
using UnityEditor;

namespace Instemic.AndroidBridge
{
    /// <summary>
    /// Settings window for Android Bridge Toolkit
    /// </summary>
    public class SettingsWindow : EditorWindow
    {
        public static void Init()
        {
            SettingsWindow window = GetWindow<SettingsWindow>("Settings");
            window.minSize = new Vector2(500, 400);
            window.Show();
        }
        
        void OnGUI()
        {
            GUILayout.Label("Android Bridge Toolkit Settings", EditorStyles.largeLabel);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "Configure your toolkit preferences here.",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            // General Settings
            GUILayout.Label("General", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            ToolkitSettings.autoRefreshOnGenerate = EditorGUILayout.Toggle(
                "Auto-refresh after generation",
                ToolkitSettings.autoRefreshOnGenerate
            );
            
            ToolkitSettings.showNotifications = EditorGUILayout.Toggle(
                "Show notifications",
                ToolkitSettings.showNotifications
            );
            
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space();
            
            // Bridge Generator Settings
            GUILayout.Label("Bridge Generator", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            ToolkitSettings.generateXmlComments = EditorGUILayout.Toggle(
                "Generate XML comments",
                ToolkitSettings.generateXmlComments
            );
            
            ToolkitSettings.usePascalCase = EditorGUILayout.Toggle(
                "Use PascalCase for methods",
                ToolkitSettings.usePascalCase
            );
            
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space();
            
            // Paths
            GUILayout.Label("Paths", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.LabelField("Default output directory:");
            ToolkitSettings.defaultOutputPath = EditorGUILayout.TextField(
                ToolkitSettings.defaultOutputPath
            );
            
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space(20);
            
            // Action buttons
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Save Settings", GUILayout.Height(30)))
            {
                ToolkitSettings.Save();
                ShowNotification(new GUIContent("✅ Settings saved!"));
            }
            
            if (GUILayout.Button("Reset to Defaults", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog(
                    "Reset Settings",
                    "Are you sure you want to reset all settings to default values?",
                    "Yes", "Cancel"))
                {
                    ToolkitSettings.ResetToDefaults();
                    ShowNotification(new GUIContent("✅ Settings reset!"));
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
    
    /// <summary>
    /// Stores toolkit settings using EditorPrefs
    /// </summary>
    public static class ToolkitSettings
    {
        private const string PREFIX = "Instemic.AndroidBridge.";
        
        // General
        public static bool autoRefreshOnGenerate
        {
            get => EditorPrefs.GetBool(PREFIX + "AutoRefresh", true);
            set => EditorPrefs.SetBool(PREFIX + "AutoRefresh", value);
        }
        
        public static bool showNotifications
        {
            get => EditorPrefs.GetBool(PREFIX + "ShowNotifications", true);
            set => EditorPrefs.SetBool(PREFIX + "ShowNotifications", value);
        }
        
        // Bridge Generator
        public static bool generateXmlComments
        {
            get => EditorPrefs.GetBool(PREFIX + "GenerateXmlComments", true);
            set => EditorPrefs.SetBool(PREFIX + "GenerateXmlComments", value);
        }
        
        public static bool usePascalCase
        {
            get => EditorPrefs.GetBool(PREFIX + "UsePascalCase", true);
            set => EditorPrefs.SetBool(PREFIX + "UsePascalCase", value);
        }
        
        // Paths
        public static string defaultOutputPath
        {
            get => EditorPrefs.GetString(PREFIX + "DefaultOutputPath", "Assets/Scripts");
            set => EditorPrefs.SetString(PREFIX + "DefaultOutputPath", value);
        }
        
        public static void Save()
        {
            EditorPrefs.SetBool(PREFIX + "SettingsSaved", true);
        }
        
        public static void ResetToDefaults()
        {
            EditorPrefs.DeleteKey(PREFIX + "AutoRefresh");
            EditorPrefs.DeleteKey(PREFIX + "ShowNotifications");
            EditorPrefs.DeleteKey(PREFIX + "GenerateXmlComments");
            EditorPrefs.DeleteKey(PREFIX + "UsePascalCase");
            EditorPrefs.DeleteKey(PREFIX + "DefaultOutputPath");
        }
    }
}
