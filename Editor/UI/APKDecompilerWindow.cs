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
                "✅ Self-Contained - No external tools required!\\n\\n" +
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
            
            if (GUILayout.Button("Browse", GUILayout.Width(80)))\n            {\n                string path = EditorUtility.OpenFilePanel("Select APK File", "", "apk");\n                if (!string.IsNullOrEmpty(path))\n                {\n                    apkPath = path;\n                }\n            }\n            \n            EditorGUILayout.EndHorizontal();\n            \n            // Extract Button\n            EditorGUILayout.Space();\n            GUILayout.Label("2. Extract Classes", EditorStyles.boldLabel);\n            \n            EditorGUI.BeginDisabledGroup(isExtracting || string.IsNullOrEmpty(apkPath));\n            \n            if (GUILayout.Button("🚀 Extract Class Metadata", GUILayout.Height(50)))\n            {\n                StartExtraction();\n            }\n            \n            EditorGUI.EndDisabledGroup();\n            \n            // Progress\n            if (isExtracting)\n            {\n                EditorGUILayout.Space();\n                EditorGUILayout.LabelField("Progress:", progressMessage);\n                EditorGUI.ProgressBar(GUILayoutUtility.GetRect(18, 18), progress, $"{(int)(progress * 100)}%");\n            }\n            \n            // Browse Results\n            if (extractedClasses != null && extractedClasses.Count > 0)\n            {\n                EditorGUILayout.Space();\n                GUILayout.Label("✅ Extraction Complete!", EditorStyles.boldLabel);\n                \n                EditorGUILayout.HelpBox(\n                    $"Found {extractedClasses.Count} classes!\\n\\n" +\n                    "Click 'Browse Classes' to explore and generate bridges.\",\n                    MessageType.Info\n                );\n                \n                if (GUILayout.Button("📂 Browse Extracted Classes", GUILayout.Height(50)))\n                {\n                    DecompiledFileBrowser.Init(extractedClasses);\n                }\n            }\n            \n            // Info Section\n            EditorGUILayout.Space(20);\n            DrawInfoSection();\n        }\n        \n        void DrawInfoSection()\n        {\n            GUILayout.Label("ℹ️ How It Works\", EditorStyles.boldLabel);\n            \n            EditorGUILayout.BeginVertical(EditorStyles.helpBox);\n            \n            EditorGUILayout.LabelField(\"What this extracts:\", EditorStyles.boldLabel);\n            EditorGUILayout.LabelField(\"• Class names and packages\");\n            EditorGUILayout.LabelField(\"• Public method signatures\");\n            EditorGUILayout.LabelField(\"• Return types and parameters\");\n            EditorGUILayout.LabelField(\"• Static/instance method flags\");\n            \n            EditorGUILayout.Space();\n            \n            EditorGUILayout.LabelField(\"Perfect for:\", EditorStyles.boldLabel);\n            EditorGUILayout.LabelField(\"• Generating C# bridges automatically\");\n            EditorGUILayout.LabelField(\"• Understanding APK structure\");\n            EditorGUILayout.LabelField(\"• Quick class discovery\");\n            \n            EditorGUILayout.Space();\n            \n            EditorGUILayout.HelpBox(\n                \"💡 This tool parses DEX bytecode natively - no JADX or external tools needed!\\n\" +\n                \"It's fast, self-contained, and gives you exactly what you need for bridge generation.\",\n                MessageType.Info\n            );\n            \n            EditorGUILayout.EndVertical();\n        }\n        \n        void StartExtraction()\n        {\n            if (!File.Exists(apkPath))\n            {\n                EditorUtility.DisplayDialog(\"Error\", $\"APK file not found:\\n{apkPath}\", \"OK\");\n                return;\n            }\n            \n            isExtracting = true;\n            progress = 0f;\n            progressMessage = \"Starting extraction...\";\n            extractedClasses = null;\n            \n            try\n            {\n                extractedClasses = APKExtractor.ExtractClasses(apkPath, OnProgress);\n                OnComplete();\n            }\n            catch (Exception ex)\n            {\n                OnError(ex.Message);\n            }\n        }\n        \n        void OnProgress(float p, string message)\n        {\n            progress = p;\n            progressMessage = message;\n            Repaint();\n        }\n        \n        void OnComplete()\n        {\n            isExtracting = false;\n            progress = 1f;\n            progressMessage = \"Complete!\";\n            \n            Debug.Log($\"APK extraction complete: Found {extractedClasses.Count} classes\");\n            \n            EditorUtility.DisplayDialog(\n                \"Extraction Complete\",\n                $\"Successfully extracted {extractedClasses.Count} classes!\\n\\n\" +\n                \"Click 'Browse Extracted Classes' to explore and generate bridges.\",\n                \"OK\"\n            );\n            \n            Repaint();\n        }\n        \n        void OnError(string error)\n        {\n            isExtracting = false;\n            progress = 0f;\n            progressMessage = \"\";\n            extractedClasses = null;\n            \n            Debug.LogError($\"APK extraction failed: {error}\");\n            \n            EditorUtility.DisplayDialog(\n                \"Extraction Failed\",\n                $\"Error:\\n{error}\\n\\nMake sure the file is a valid APK.\",\n                \"OK\"\n            );\n            \n            Repaint();\n        }\n    }\n}\n"