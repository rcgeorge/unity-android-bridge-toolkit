//
// APKDecompilerWindow.cs - Native APK extraction UI
// Copyright (c) 2024 Instemic
//

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Instemic.AndroidBridge.Core;

namespace Instemic.AndroidBridge
{
    /// <summary>
    /// Window for extracting class metadata and native libraries from APK files
    /// </summary>
    public class APKDecompilerWindow : EditorWindow
    {
        private string apkPath = "";
        private bool isExtracting = false;
        private float progress = 0f;
        private string progressMessage = "";
        private List<DexClass> extractedClasses;
        private List<string> nativeLibraries;
        private bool copyApkToProject = true;
        
        private Vector2 scrollPos;
        
        public static void Init()
        {
            APKDecompilerWindow window = GetWindow<APKDecompilerWindow>("APK Extractor");
            window.minSize = new Vector2(650, 500);
            window.Show();
        }
        
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            DrawHeader();
            EditorGUILayout.Space();
            
            DrawExtractorUI();
            
            EditorGUILayout.EndScrollView();
        }
        
        void DrawHeader()
        {
            GUILayout.Label("📦 APK Extractor - Complete Setup", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "✅ Complete APK Integration Toolkit\n\n" +
                "Extract everything you need from APK files:\n" +
                "• Class metadata (for C# bridges)\n" +
                "• Native libraries (.so files)\n" +
                "• Copy APK to your Unity project\n\n" +
                "All self-contained - no external tools required!",
                MessageType.Info
            );
        }
        
        void DrawExtractorUI()
        {
            EditorGUILayout.Space();
            
            // APK Selection
            GUILayout.Label("1. Select APK File", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            apkPath = EditorGUILayout.TextField("APK Path:", apkPath);
            
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string path = EditorUtility.OpenFilePanel("Select APK File", "", "apk");
                if (!string.IsNullOrEmpty(path))
                {
                    apkPath = path;
                    PreviewNativeLibraries();
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // Show native library preview if available
            if (nativeLibraries != null && nativeLibraries.Count > 0)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField($"📱 Found {nativeLibraries.Count} native libraries in this APK", EditorStyles.miniLabel);
            }
            
            EditorGUILayout.Space();
            
            // Extract Classes Section
            GUILayout.Label("2. Extract Class Metadata", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Extract Java class signatures for bridge generation");
            
            EditorGUI.BeginDisabledGroup(isExtracting || string.IsNullOrEmpty(apkPath));
            if (GUILayout.Button("🔍 Extract Classes", GUILayout.Height(40)))
            {
                StartClassExtraction();
            }
            EditorGUI.EndDisabledGroup();
            
            if (extractedClasses != null && extractedClasses.Count > 0)
            {
                EditorGUILayout.HelpBox($"✅ Extracted {extractedClasses.Count} classes", MessageType.Info);
                
                if (GUILayout.Button("📂 Browse Classes & Generate Bridges", GUILayout.Height(35)))
                {
                    DecompiledFileBrowser.Init(extractedClasses);
                }
            }
            
            EditorGUILayout.Space();
            
            // Extract Native Libraries Section
            GUILayout.Label("3. Extract Native Libraries", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Extract .so files needed for native functionality");
            
            copyApkToProject = EditorGUILayout.ToggleLeft(
                "  Also copy APK to Assets/Plugins/Android/", 
                copyApkToProject
            );
            
            EditorGUI.BeginDisabledGroup(isExtracting || string.IsNullOrEmpty(apkPath));
            if (GUILayout.Button("📲 Extract Native Libraries (.so files)", GUILayout.Height(40)))
            {
                StartNativeLibraryExtraction();
            }
            EditorGUI.EndDisabledGroup();
            
            // Progress
            if (isExtracting)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Progress:", progressMessage);
                EditorGUI.ProgressBar(GUILayoutUtility.GetRect(18, 18), progress, $"{(int)(progress * 100)}%");
            }
            
            // Info Section
            EditorGUILayout.Space(20);
            DrawInfoSection();
        }
        
        void DrawInfoSection()
        {
            GUILayout.Label("ℹ️ Complete Setup Process", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("To use an APK's functionality:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("1. Extract classes → Generate C# bridges");
            EditorGUILayout.LabelField("2. Extract native libraries → Get .so files");
            EditorGUILayout.LabelField("3. Copy APK → Add to Unity project");
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Files will be placed in:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("• APK: Assets/Plugins/Android/[apkname].apk");
            EditorGUILayout.LabelField("• Libraries: Assets/Plugins/Android/libs/[arch]/");
            EditorGUILayout.LabelField("• Bridges: Generate and save manually");
            
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "💡 This gives you everything needed to call the APK's functionality from Unity C#!",
                MessageType.Info
            );
            
            EditorGUILayout.EndVertical();
        }
        
        void PreviewNativeLibraries()
        {
            if (string.IsNullOrEmpty(apkPath) || !File.Exists(apkPath))
            {
                nativeLibraries = null;
                return;
            }
            
            nativeLibraries = NativeLibraryExtractor.PreviewNativeLibraries(apkPath);
        }
        
        void StartClassExtraction()
        {
            if (!File.Exists(apkPath))
            {
                EditorUtility.DisplayDialog("Error", "APK file not found: " + apkPath, "OK");
                return;
            }
            
            isExtracting = true;
            progress = 0f;
            progressMessage = "Extracting classes...";
            extractedClasses = null;
            
            try
            {
                extractedClasses = APKExtractor.ExtractClasses(apkPath, OnProgress);
                OnClassExtractionComplete();
            }
            catch (Exception ex)
            {
                OnError("Class Extraction", ex.Message);
            }
        }
        
        void StartNativeLibraryExtraction()
        {
            if (!File.Exists(apkPath))
            {
                EditorUtility.DisplayDialog("Error", "APK file not found: " + apkPath, "OK");
                return;
            }
            
            isExtracting = true;
            progress = 0f;
            progressMessage = "Extracting native libraries...";
            
            try
            {
                var result = NativeLibraryExtractor.ExtractNativeLibraries(
                    apkPath,
                    null, // Use default path
                    copyApkToProject,
                    OnProgress
                );
                
                OnNativeLibraryExtractionComplete(result);
            }
            catch (Exception ex)
            {
                OnError("Native Library Extraction", ex.Message);
            }
        }
        
        void OnProgress(float p, string message)
        {
            progress = p;
            progressMessage = message;
            Repaint();
        }
        
        void OnClassExtractionComplete()
        {
            isExtracting = false;
            progress = 1f;
            progressMessage = "Complete!";
            
            Debug.Log($"APK class extraction complete: Found {extractedClasses.Count} classes");
            
            EditorUtility.DisplayDialog(
                "Extraction Complete",
                "Successfully extracted " + extractedClasses.Count + " classes!\n\nClick 'Browse Classes' to explore and generate bridges.",
                "OK"
            );
            
            // Refresh to show new files
            AssetDatabase.Refresh();
            
            Repaint();
        }
        
        void OnNativeLibraryExtractionComplete(NativeLibraryExtractor.ExtractionResult result)
        {
            isExtracting = false;
            progress = 1f;
            progressMessage = "Complete!";
            
            if (result.Success)
            {
                Debug.Log($"Native library extraction complete: Extracted {result.ExtractedLibraries.Count} libraries");
                
                EditorUtility.DisplayDialog(
                    "Extraction Complete",
                    result.Message,
                    "OK"
                );
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Extraction Result",
                    result.Message,
                    "OK"
                );
            }
            
            // Refresh to show new files
            AssetDatabase.Refresh();
            
            Repaint();
        }
        
        void OnError(string operation, string error)
        {
            isExtracting = false;
            progress = 0f;
            progressMessage = "";
            
            Debug.LogError($"{operation} failed: {error}");
            
            EditorUtility.DisplayDialog(
                operation + " Failed",
                "Error: " + error + "\n\nMake sure the file is a valid APK.",
                "OK"
            );
            
            Repaint();
        }
    }
}
