//
// DecompiledFileBrowser.cs - Browse extracted APK classes
// Copyright (c) 2024 Instemic
//

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Instemic.AndroidBridge.Core;

namespace Instemic.AndroidBridge
{
    /// <summary>
    /// Browser for navigating extracted APK classes
    /// </summary>
    public class DecompiledFileBrowser : EditorWindow
    {
        private List<DexClass> classes;
        private List<DexClass> filteredClasses;
        private string searchFilter = "";
        private Vector2 scrollPos;
        private DexClass selectedClass;
        
        public static void Init(List<DexClass> extractedClasses)
        {
            DecompiledFileBrowser window = GetWindow<DecompiledFileBrowser>("APK Class Browser");
            window.minSize = new Vector2(700, 500);
            window.classes = extractedClasses;
            window.filteredClasses = extractedClasses;
            window.Show();
        }
        
        void OnGUI()
        {
            DrawHeader();
            EditorGUILayout.Space();
            
            DrawSearchBar();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            
            // Left panel: Class list
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.4f));
            DrawClassList();
            EditorGUILayout.EndVertical();
            
            // Right panel: Class details
            EditorGUILayout.BeginVertical();
            DrawClassDetails();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
        }
        
        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            GUILayout.Label($"📦 Extracted Classes ({filteredClasses.Count})", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
            {
                ApplySearchFilter();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        void DrawSearchBar()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label("🔍 Search:", GUILayout.Width(60));
            
            string newFilter = EditorGUILayout.TextField(searchFilter);
            if (newFilter != searchFilter)
            {
                searchFilter = newFilter;
                ApplySearchFilter();
            }
            
            if (GUILayout.Button("Clear", GUILayout.Width(60)))
            {
                searchFilter = "";
                ApplySearchFilter();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.HelpBox(
                "💡 Tip: Click a class to view details, then click 'Generate Bridge' to create C# code",
                MessageType.Info
            );
        }
        
        void DrawClassList()
        {
            GUILayout.Label("Classes", EditorStyles.boldLabel);
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.helpBox);
            
            // Group by package
            var groupedClasses = filteredClasses
                .GroupBy(c => c.GetPackageName())
                .OrderBy(g => g.Key);
            
            foreach (var group in groupedClasses)
            {
                // Package header
                GUIStyle packageStyle = new GUIStyle(EditorStyles.foldoutHeader);
                GUILayout.Label($"📁 {group.Key}", packageStyle);
                
                // Classes in package
                foreach (DexClass dexClass in group)
                {
                    bool isSelected = selectedClass == dexClass;
                    
                    GUIStyle classStyle = new GUIStyle(GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        normal = { background = isSelected ? MakeTexture(new Color(0.3f, 0.5f, 0.8f, 0.3f)) : null }
                    };
                    
                    string methodCount = dexClass.Methods.Count > 0 ? $" ({dexClass.Methods.Count} methods)" : "";
                    
                    if (GUILayout.Button($"  📄 {dexClass.GetSimpleClassName()}{methodCount}", classStyle))
                    {
                        selectedClass = dexClass;
                    }
                }
                
                EditorGUILayout.Space(5);
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        void DrawClassDetails()
        {
            GUILayout.Label("Class Details", EditorStyles.boldLabel);
            
            if (selectedClass == null)
            {
                EditorGUILayout.HelpBox("Select a class to view details", MessageType.Info);
                return;
            }
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Class info
            GUILayout.Label($"Class: {selectedClass.ClassName}", EditorStyles.largeLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Package:", selectedClass.GetPackageName());
            EditorGUILayout.LabelField("Simple Name:", selectedClass.GetSimpleClassName());
            EditorGUILayout.LabelField("Public Methods:", selectedClass.Methods.Count.ToString());
            
            EditorGUILayout.Space();
            
            // Methods
            GUILayout.Label("Methods:", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.textArea, GUILayout.Height(200));
            
            foreach (DexMethod method in selectedClass.Methods)
            {
                if (!method.IsPublic)
                    continue;
                    
                string modifiers = method.IsStatic ? "static " : "";
                string signature = $"{modifiers}{method.ReturnType} {method.Name}(";
                
                if (method.Parameters.Count > 0)
                {
                    signature += string.Join(", ", method.Parameters);
                }
                
                signature += ")";
                
                EditorGUILayout.LabelField(signature, EditorStyles.wordWrappedLabel);
            }
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            
            // Actions
            if (GUILayout.Button("⚡ Generate C# Bridge", GUILayout.Height(50)))
            {
                GenerateBridge();
            }
        }
        
        void ApplySearchFilter()
        {
            if (string.IsNullOrWhiteSpace(searchFilter))
            {
                filteredClasses = classes;
            }
            else
            {
                string filter = searchFilter.ToLower();
                filteredClasses = classes
                    .Where(c => c.ClassName.ToLower().Contains(filter))
                    .ToList();
            }
        }
        
        void GenerateBridge()
        {
            if (selectedClass == null)
                return;
                
            // Convert to JavaClass
            JavaClass javaClass = APKExtractor.ConvertToJavaClass(selectedClass);
            
            // Generate bridge code
            string bridgeCode = BridgeGenerator.GenerateCSharpBridge(javaClass);
            
            // Open bridge generator with the code
            BridgeGeneratorWindow window = GetWindow<BridgeGeneratorWindow>("Bridge Generator");
            window.SetOutputCode(bridgeCode);
            window.Show();
            window.Focus();
        }
        
        private Texture2D MakeTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}
