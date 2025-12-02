//
// BridgeGeneratorWindow.cs - Bridge code generation interface
// Copyright (c) 2024 Instemic
//

using UnityEngine;
using UnityEditor;
using System.IO;
using Instemic.AndroidBridge.Core;

namespace Instemic.AndroidBridge
{
    /// <summary>
    /// Window for generating C# bridges from Java code
    /// </summary>
    public class BridgeGeneratorWindow : EditorWindow
    {
        private string javaSource = "";
        private string generatedCSharp = "";
        private Vector2 leftScrollPos;
        private Vector2 rightScrollPos;
        
        public static void Init()
        {
            BridgeGeneratorWindow window = GetWindow<BridgeGeneratorWindow>("Bridge Generator");
            window.minSize = new Vector2(1000, 600);
            window.Show();
        }
        
        void OnGUI()
        {
            DrawHeader();
            
            EditorGUILayout.Space();
            
            // Split view: Java input on left, C# output on right
            EditorGUILayout.BeginHorizontal();
            
            // Left panel - Java input
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 2 - 10));
            DrawJavaInputPanel();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            
            // Right panel - C# output
            EditorGUILayout.BeginVertical();
            DrawCSharpOutputPanel();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            DrawActionButtons();
        }
        
        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("🌉 Bridge Generator", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Load Java File", EditorStyles.toolbarButton))
            {
                LoadJavaFile();
            }
            
            if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
            {
                javaSource = "";
                generatedCSharp = "";
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        void DrawJavaInputPanel()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            GUILayout.Label("☕ Java Source", EditorStyles.boldLabel);
            
            EditorGUILayout.HelpBox(
                "Paste your Java class code here or load from file.",
                MessageType.Info
            );
            
            leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos);
            
            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = false,
                font = Font.CreateDynamicFontFromOSFont("Courier New", 12)
            };
            
            javaSource = EditorGUILayout.TextArea(javaSource, textAreaStyle, GUILayout.ExpandHeight(true));
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        
        void DrawCSharpOutputPanel()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            GUILayout.Label("🎨 Generated C# Bridge", EditorStyles.boldLabel);
            
            if (string.IsNullOrEmpty(generatedCSharp))
            {
                EditorGUILayout.HelpBox(
                    "Generated C# code will appear here.\n\n" +
                    "Click 'Generate Bridge' to create the code.",
                    MessageType.Info
                );
            }
            else
            {
                rightScrollPos = EditorGUILayout.BeginScrollView(rightScrollPos);
                
                GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea)
                {
                    wordWrap = false,
                    font = Font.CreateDynamicFontFromOSFont("Courier New", 12)
                };
                
                EditorGUILayout.TextArea(generatedCSharp, textAreaStyle, GUILayout.ExpandHeight(true));
                
                EditorGUILayout.EndScrollView();
            }
            
            EditorGUILayout.EndVertical();
        }
        
        void DrawActionButtons()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = !string.IsNullOrWhiteSpace(javaSource);
            
            if (GUILayout.Button("⚡ Generate Bridge", GUILayout.Height(40), GUILayout.Width(200)))
            {
                GenerateBridge();
            }
            
            GUI.enabled = !string.IsNullOrEmpty(generatedCSharp);
            
            if (GUILayout.Button("📋 Copy to Clipboard", GUILayout.Height(40), GUILayout.Width(200)))
            {
                EditorGUIUtility.systemCopyBuffer = generatedCSharp;
                ShowNotification(new GUIContent("✅ Copied to clipboard!"));
            }
            
            if (GUILayout.Button("💾 Save as C# File", GUILayout.Height(40), GUILayout.Width(200)))
            {
                SaveCSharpFile();
            }
            
            GUI.enabled = true;
            
            GUILayout.FlexibleSpace();
            
            EditorGUILayout.EndHorizontal();
        }
        
        void LoadJavaFile()
        {
            string path = EditorUtility.OpenFilePanel("Load Java File", "", "java");
            
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    javaSource = File.ReadAllText(path);
                    ShowNotification(new GUIContent("✅ Java file loaded!"));
                }
                catch (System.Exception e)
                {
                    EditorUtility.DisplayDialog("Error", "Failed to load file:\n" + e.Message, "OK");
                }
            }
        }
        
        void GenerateBridge()
        {
            try
            {
                generatedCSharp = BridgeGenerator.Generate(javaSource);
                
                ShowNotification(new GUIContent("✅ Bridge generated successfully!"));
                
                Debug.Log("Bridge generated successfully!");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog(
                    "Generation Failed",
                    "Failed to generate bridge:\n\n" + e.Message,
                    "OK"
                );
                
                Debug.LogError("Bridge generation failed: " + e.Message);
            }
        }
        
        void SaveCSharpFile()
        {
            string path = EditorUtility.SaveFilePanel(
                "Save C# Bridge",
                "Assets/Scripts",
                "Bridge.cs",
                "cs"
            );
            
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    File.WriteAllText(path, generatedCSharp);
                    
                    // Refresh Unity to import the new file
                    AssetDatabase.Refresh();
                    
                    ShowNotification(new GUIContent("✅ File saved!"));
                    
                    EditorUtility.DisplayDialog(
                        "Success",
                        "Bridge saved to:\n" + path,
                        "OK"
                    );
                }
                catch (System.Exception e)
                {
                    EditorUtility.DisplayDialog(
                        "Save Failed",
                        "Failed to save file:\n" + e.Message,
                        "OK"
                    );
                }
            }
        }
    }
}
