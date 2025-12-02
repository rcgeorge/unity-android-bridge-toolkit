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
    /// Window for extracting class metadata from APK files (no external tools required!)
    /// </summary>
    public class APKDecompilerWindow : EditorWindow
    {
        private string apkPath = "";
        private bool isExtracting = false;
        private float progress = 0f;
        private string progressMessage = "";
        private List<DexClass> extractedClasses;
        
        private Vector2 scrollPos;
        
        public static void Init()
        {
            APKDecompilerWindow window = GetWindow<APKDecompilerWindow>("APK Extractor");
            window.minSize = new Vector2(600, 400);
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
            GUILayout.Label("📦 APK Class Extractor", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "✅ Self-Contained - No external tools required!\n\n" +
                "Extract class metadata from APK files natively. This tool parses DEX files " +
                "to extract class names, methods, and signatures - perfect for generating C# bridges!",
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
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Extract Button
            EditorGUILayout.Space();
            GUILayout.Label("2. Extract Classes", EditorStyles.boldLabel);
            
            EditorGUI.BeginDisabledGroup(isExtracting || string.IsNullOrEmpty(apkPath));
            
            if (GUILayout.Button("🚀 Extract Class Metadata", GUILayout.Height(50)))
            {
                StartExtraction();
            }
            
            EditorGUI.EndDisabledGroup();
            
            // Progress
            if (isExtracting)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Progress:", progressMessage);
                EditorGUI.ProgressBar(GUILayoutUtility.GetRect(18, 18), progress, $"{(int)(progress * 100)}%");
            }
            
            // Browse Results
            if (extractedClasses != null && extractedClasses.Count > 0)
            {
                EditorGUILayout.Space();
                GUILayout.Label("✅ Extraction Complete!", EditorStyles.boldLabel);
                
                EditorGUILayout.HelpBox(
                    $"Found {extractedClasses.Count} classes!\n\n" +
                    "Click 'Browse Classes' to explore and generate bridges.",
                    MessageType.Info
                );
                
                if (GUILayout.Button("📂 Browse Extracted Classes", GUILayout.Height(50)))
                {
                    DecompiledFileBrowser.Init(extractedClasses);
                }
            }
            
            // Info Section
            EditorGUILayout.Space(20);
            DrawInfoSection();
        }
        
        void DrawInfoSection()
        {
            GUILayout.Label("ℹ️ How It Works", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("What this extracts:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("• Class names and packages");
            EditorGUILayout.LabelField("• Public method signatures");
            EditorGUILayout.LabelField("• Return types and parameters");
            EditorGUILayout.LabelField("• Static/instance method flags");
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Perfect for:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("• Generating C# bridges automatically");
            EditorGUILayout.LabelField("• Understanding APK structure");
            EditorGUILayout.LabelField("• Quick class discovery");
            
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "💡 This tool parses DEX bytecode natively - no JADX or external tools needed!\n" +
                "It's fast, self-contained, and gives you exactly what you need for bridge generation.",
                MessageType.Info
            );
            
            EditorGUILayout.EndVertical();
        }
        
        void StartExtraction()
        {
            if (!File.Exists(apkPath))
            {
                EditorUtility.DisplayDialog("Error", "APK file not found: " + apkPath, "OK");
                return;
            }
            
            isExtracting = true;
            progress = 0f;
            progressMessage = "Starting extraction...";
            extractedClasses = null;
            
            try
            {
                extractedClasses = APKExtractor.ExtractClasses(apkPath, OnProgress);
                OnComplete();
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
            }
        }
        
        void OnProgress(float p, string message)
        {
            progress = p;
            progressMessage = message;
            Repaint();
        }
        
        void OnComplete()
        {
            isExtracting = false;
            progress = 1f;
            progressMessage = "Complete!";
            
            Debug.Log($"APK extraction complete: Found {extractedClasses.Count} classes");
            
            EditorUtility.DisplayDialog(
                "Extraction Complete",
                "Successfully extracted " + extractedClasses.Count + " classes!\n\nClick 'Browse Extracted Classes' to explore and generate bridges.",
                "OK"
            );
            
            Repaint();
        }
        
        void OnError(string error)
        {
            isExtracting = false;
            progress = 0f;
            progressMessage = "";
            extractedClasses = null;
            
            Debug.LogError($"APK extraction failed: {error}");
            
            EditorUtility.DisplayDialog(
                "Extraction Failed",
                "Error: " + error + "\n\nMake sure the file is a valid APK.",
                "OK"
            );
            
            Repaint();
        }
    }
}
