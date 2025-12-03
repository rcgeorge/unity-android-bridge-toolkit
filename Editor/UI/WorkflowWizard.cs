//
// WorkflowWizard.cs - Unified wizard for complete APK to Unity workflow
// Copyright (c) 2024 Instemic
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Instemic.AndroidBridge.Core;

namespace Instemic.AndroidBridge
{
    /// <summary>
    /// Unified wizard that guides users through the complete workflow
    /// </summary>
    public class WorkflowWizard : EditorWindow
    {
        // Wizard state
        private int currentStep = 0;
        private string[] stepTitles = { 
            "1. Load APK", 
            "2. Select Classes", 
            "3. Review & Generate Wrapper",
            "4. Build AAR",
            "5. Generate C# Bridge",
            "6. Extract Libraries",
            "7. Complete!"
        };
        
        // Step 1: Load APK
        private string apkPath = "";
        private bool apkLoaded = false;
        private int totalClasses = 0;
        private List<string> nativeLibraries = new List<string>();
        
        // Step 2: Select Classes
        private List<DexClass> allClasses = new List<DexClass>();
        private List<DexClass> selectedClasses = new List<DexClass>();
        private Dictionary<DexClass, bool> expandedClasses = new Dictionary<DexClass, bool>();
        private Dictionary<string, bool> expandedPackages = new Dictionary<string, bool>();
        private Vector2 classScrollPos;
        private string classSearchFilter = "";
        private bool groupByPackage = true;
        private int searchMode = 0; // 0 = class name, 1 = method name
        
        // Step 3: Review Wrapper
        private string generatedJavaCode = "";
        private string wrapperClassName = "SDKWrapper";
        private string packageName = "com.unity.wrapper";
        private Vector2 javaScrollPos;
        
        // Step 4: Build AAR
        private string aarPath = "";
        private bool aarBuilt = false;
        
        // Step 5: C# Bridge
        private string csharpBridgeCode = "";
        private string bridgeSavePath = "";
        private bool bridgeGenerated = false;
        private Vector2 csharpScrollPos;
        
        // Step 6: Extract Libraries
        private bool librariesExtracted = false;
        
        // UI
        private Vector2 scrollPos;
        private bool isProcessing = false;
        private float progress = 0f;
        private string statusMessage = "";
        
        public static void Init()
        {
            WorkflowWizard window = GetWindow<WorkflowWizard>("APK to Unity Wizard");
            window.minSize = new Vector2(900, 700);
            window.Show();
        }
        
        void OnGUI()
        {
            DrawHeader();
            DrawProgressBar();
            
            EditorGUILayout.Space();
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            switch (currentStep)
            {
                case 0: DrawStep1_LoadAPK(); break;
                case 1: DrawStep2_SelectClasses(); break;
                case 2: DrawStep3_ReviewWrapper(); break;
                case 3: DrawStep4_BuildAAR(); break;
                case 4: DrawStep5_GenerateBridge(); break;
                case 5: DrawStep6_ExtractLibraries(); break;
                case 6: DrawStep7_Complete(); break;
            }
            
            EditorGUILayout.EndScrollView();
            
            DrawNavigationButtons();
        }
        
        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("🧙 APK to Unity Wizard", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Step {currentStep + 1} of {stepTitles.Length}", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
        }
        
        void DrawProgressBar()
        {
            var rect = GUILayoutUtility.GetRect(18, 30);
            
            // Background
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, rect.height), new Color(0.2f, 0.2f, 0.2f));
            
            // Progress
            var progressWidth = rect.width * ((currentStep + 1) / (float)stepTitles.Length);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, progressWidth, rect.height), new Color(0.3f, 0.7f, 0.3f));
            
            // Step labels
            var labelStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            
            GUI.Label(rect, stepTitles[currentStep], labelStyle);
        }
        
        void DrawStep1_LoadAPK()
        {
            GUILayout.Label("📦 Step 1: Load APK File", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "Select an APK file to reverse engineer.\n\n" +
                "We'll extract:\n" +
                "• Class metadata (to learn the API)\n" +
                "• Native libraries (.so files)\n\n" +
                "Example: ghostparty.apk (Viture SDK)",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("APK File:", GUILayout.Width(70));
            EditorGUILayout.LabelField(string.IsNullOrEmpty(apkPath) ? "No file selected" : Path.GetFileName(apkPath));
            
            if (GUILayout.Button("Browse...", GUILayout.Width(100)))
            {
                var path = EditorUtility.OpenFilePanel("Select APK", "", "apk");
                if (!string.IsNullOrEmpty(path))
                {
                    apkPath = path;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            if (!string.IsNullOrEmpty(apkPath) && !apkLoaded)
            {
                EditorGUILayout.Space();
                
                if (GUILayout.Button("🔍 Analyze APK", GUILayout.Height(50)))
                {
                    AnalyzeAPK();
                }
            }
            
            if (apkLoaded)
            {
                EditorGUILayout.Space();
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("✅ APK Analyzed", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Classes found: {totalClasses}");
                EditorGUILayout.LabelField($"Native libraries: {nativeLibraries.Count}");
                
                if (nativeLibraries.Count > 0)
                {
                    EditorGUILayout.LabelField("Libraries:", EditorStyles.miniLabel);
                    foreach (var lib in nativeLibraries.Take(5))
                    {
                        EditorGUILayout.LabelField("  • " + lib, EditorStyles.miniLabel);
                    }
                    if (nativeLibraries.Count > 5)
                    {
                        EditorGUILayout.LabelField($"  ... and {nativeLibraries.Count - 5} more", EditorStyles.miniLabel);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            
            if (isProcessing)
            {
                EditorGUILayout.Space();
                EditorGUI.ProgressBar(GUILayoutUtility.GetRect(18, 18), progress, statusMessage);
            }
        }
        
        void DrawStep2_SelectClasses()
        {
            GUILayout.Label("📋 Step 2: Select Classes to Wrap", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "Select the classes you want to use in Unity.\n\n" +
                "Tips:\n" +
                "• [interface] tags show interfaces - usually skip these, use the implementation\n" +
                "• [class] tags show concrete classes - these are what you typically want\n" +
                "• Search by METHOD NAME to find specific functionality (getPose, 6DOF, listener)",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            // Search mode selector
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search by:", GUILayout.Width(70));
            string[] searchModes = { "Class Name", "Method Name" };
            searchMode = GUILayout.Toolbar(searchMode, searchModes, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // Search field
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Filter:", GUILayout.Width(60));
            classSearchFilter = EditorGUILayout.TextField(classSearchFilter);
            
            if (!string.IsNullOrEmpty(classSearchFilter) && GUILayout.Button("Clear", GUILayout.Width(60)))
            {
                classSearchFilter = "";
            }
            EditorGUILayout.EndHorizontal();
            
            // Search hints
            if (searchMode == 1 && string.IsNullOrEmpty(classSearchFilter))
            {
                EditorGUILayout.LabelField("💡 Try searching: getPose, 6DOF, listener, tracking, position", EditorStyles.miniLabel);
            }
            
            EditorGUILayout.Space();
            
            // View options
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Selected: {selectedClasses.Count} classes", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            groupByPackage = EditorGUILayout.ToggleLeft("Group by Package", groupByPackage, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Class list
            classScrollPos = EditorGUILayout.BeginScrollView(classScrollPos, GUILayout.Height(400));
            
            var filteredClasses = FilterClasses();
            
            if (groupByPackage && searchMode == 0)
            {
                DrawClassesByPackage(filteredClasses);
            }
            else
            {
                DrawClassesList(filteredClasses);
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        List<DexClass> FilterClasses()
        {
            if (string.IsNullOrEmpty(classSearchFilter))
            {
                return allClasses;
            }
            
            var filter = classSearchFilter.ToLower();
            
            if (searchMode == 0)
            {
                // Search by class name
                return allClasses.Where(c => c.ClassName.ToLower().Contains(filter)).ToList();
            }
            else
            {
                // Search by method name - show classes that have matching methods
                return allClasses.Where(c => 
                    c.Methods.Any(m => m.Name.ToLower().Contains(filter))
                ).ToList();
            }
        }
        
        void DrawClassesByPackage(List<DexClass> classes)
        {
            // Group classes by package
            var packageGroups = classes
                .GroupBy(c => c.GetPackageName())
                .OrderBy(g => g.Key)
                .Take(50); // Limit packages shown
            
            foreach (var packageGroup in packageGroups)
            {
                var packageName = packageGroup.Key;
                var classesInPackage = packageGroup.ToList();
                
                // Initialize expanded state
                if (!expandedPackages.ContainsKey(packageName))
                {
                    expandedPackages[packageName] = false;
                }
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                
                // Package foldout
                bool wasExpanded = expandedPackages[packageName];
                bool isExpanded = EditorGUILayout.Foldout(wasExpanded, $"📦 {packageName}", true, EditorStyles.boldLabel);
                expandedPackages[packageName] = isExpanded;
                
                // Class count and Select All button
                EditorGUILayout.LabelField($"{classesInPackage.Count} classes", EditorStyles.miniLabel, GUILayout.Width(100));
                
                if (GUILayout.Button("Select All", GUILayout.Width(80)))
                {
                    foreach (var cls in classesInPackage)
                    {
                        if (!selectedClasses.Contains(cls))
                        {
                            selectedClasses.Add(cls);
                        }
                    }
                }
                
                if (GUILayout.Button("Deselect All", GUILayout.Width(90)))
                {
                    foreach (var cls in classesInPackage)
                    {
                        selectedClasses.Remove(cls);
                    }
                }
                
                EditorGUILayout.EndHorizontal();
                
                // Show classes when expanded
                if (isExpanded)
                {
                    EditorGUI.indentLevel++;
                    
                    foreach (var classInfo in classesInPackage.Take(30))
                    {
                        DrawSingleClass(classInfo);
                    }
                    
                    if (classesInPackage.Count > 30)
                    {
                        EditorGUILayout.LabelField($"  ... and {classesInPackage.Count - 30} more classes", EditorStyles.miniLabel);
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }
        }
        
        void DrawClassesList(List<DexClass> classes)
        {
            foreach (var classInfo in classes.Take(100))
            {
                DrawSingleClass(classInfo);
            }
            
            if (classes.Count > 100)
            {
                EditorGUILayout.LabelField($"... and {classes.Count - 100} more. Use search to narrow down.", EditorStyles.miniLabel);
            }
        }
        
        void DrawSingleClass(DexClass classInfo)
        {
            // If searching by method name, count matching methods
            int matchingMethodCount = 0;
            if (searchMode == 1 && !string.IsNullOrEmpty(classSearchFilter))
            {
                var filter = classSearchFilter.ToLower();
                matchingMethodCount = classInfo.Methods.Count(m => m.Name.ToLower().Contains(filter));
            }
            
            // Initialize expanded state if needed
            if (!expandedClasses.ContainsKey(classInfo))
            {
                expandedClasses[classInfo] = false;
            }
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            
            // Checkbox for selection
            bool isSelected = selectedClasses.Contains(classInfo);
            bool newSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));
            
            if (newSelected != isSelected)
            {
                if (newSelected)
                    selectedClasses.Add(classInfo);
                else
                    selectedClasses.Remove(classInfo);
            }
            
            // Foldout arrow + class name (show simple name in grouped view, full name otherwise)
            bool wasExpanded = expandedClasses[classInfo];
            string displayName = (groupByPackage && searchMode == 0) ? classInfo.GetSimpleClassName() : classInfo.ClassName;
            bool isExpanded = EditorGUILayout.Foldout(wasExpanded, displayName, true);
            expandedClasses[classInfo] = isExpanded;
            
            // Class type badge
            string classTypeLabel = classInfo.GetClassTypeLabel();
            Color badgeColor = GetClassTypeBadgeColor(classInfo);
            var badgeStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = badgeColor },
                fontStyle = FontStyle.Bold
            };
            EditorGUILayout.LabelField($"[{classTypeLabel}]", badgeStyle, GUILayout.Width(70));
            
            // Method count (show matching count if searching by method)
            if (matchingMethodCount > 0)
            {
                EditorGUILayout.LabelField($"{matchingMethodCount} matching", EditorStyles.boldLabel, GUILayout.Width(100));
            }
            else
            {
                EditorGUILayout.LabelField($"{classInfo.Methods.Count} methods", EditorStyles.miniLabel, GUILayout.Width(100));
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Show full path as subtitle when grouped
            if (groupByPackage && searchMode == 0)
            {
                EditorGUILayout.LabelField("  " + classInfo.ClassName, EditorStyles.miniLabel);
            }
            
            // Show methods when expanded
            if (isExpanded && classInfo.Methods.Count > 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                // Filter methods if searching by method name
                var methodsToShow = classInfo.Methods;
                if (searchMode == 1 && !string.IsNullOrEmpty(classSearchFilter))
                {
                    var filter = classSearchFilter.ToLower();
                    methodsToShow = classInfo.Methods.Where(m => m.Name.ToLower().Contains(filter)).ToList();
                }
                
                int displayCount = Math.Min(methodsToShow.Count, 20);
                for (int i = 0; i < displayCount; i++)
                {
                    var method = methodsToShow[i];
                    string methodSignature = $"{method.ReturnType} {method.Name}()";
                    if (method.IsStatic)
                        methodSignature = "static " + methodSignature;
                    
                    // Highlight matching methods
                    bool isMatch = searchMode == 1 && !string.IsNullOrEmpty(classSearchFilter) && 
                                   method.Name.ToLower().Contains(classSearchFilter.ToLower());
                    
                    if (isMatch)
                    {
                        EditorGUILayout.LabelField("  ⭐ " + methodSignature, EditorStyles.boldLabel);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("  • " + methodSignature, EditorStyles.miniLabel);
                    }
                }
                
                if (methodsToShow.Count > 20)
                {
                    EditorGUILayout.LabelField($"  ... and {methodsToShow.Count - 20} more methods", EditorStyles.miniLabel);
                }
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }
        
        Color GetClassTypeBadgeColor(DexClass classInfo)
        {
            if (classInfo.IsInterface) return new Color(0.4f, 0.7f, 1.0f); // Light blue
            if (classInfo.IsAbstract) return new Color(1.0f, 0.8f, 0.4f); // Orange
            if (classInfo.IsEnum) return new Color(0.8f, 0.5f, 1.0f); // Purple
            return new Color(0.6f, 0.9f, 0.6f); // Green (concrete class)
        }
        
        void DrawStep3_ReviewWrapper()
        {
            GUILayout.Label("✏️ Step 3: Review & Customize Java Wrapper", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "Generated Java wrapper based on your selections.\n\n" +
                "This wrapper:\n" +
                "• Loads the native .so library\n" +
                "• Declares native methods matching the original API\n" +
                "• Can be customized before building",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wrapper Class Name:", GUILayout.Width(150));
            wrapperClassName = EditorGUILayout.TextField(wrapperClassName);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Package Name:", GUILayout.Width(150));
            packageName = EditorGUILayout.TextField(packageName);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            if (string.IsNullOrEmpty(generatedJavaCode))
            {
                if (GUILayout.Button("📝 Generate Java Wrapper Code", GUILayout.Height(40)))
                {
                    GenerateJavaWrapper();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Java Wrapper Code:", EditorStyles.boldLabel);
                
                javaScrollPos = EditorGUILayout.BeginScrollView(javaScrollPos, GUILayout.Height(350));
                generatedJavaCode = EditorGUILayout.TextArea(generatedJavaCode, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
                
                EditorGUILayout.Space();
                
                if (GUILayout.Button("🔄 Regenerate", GUILayout.Width(120)))
                {
                    GenerateJavaWrapper();
                }
            }
        }
        
        void DrawStep4_BuildAAR()
        {
            GUILayout.Label("🔨 Step 4: Build AAR Library", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "Compile your Java wrapper into an AAR library.\n\n" +
                "This uses:\n" +
                "• javac to compile Java code\n" +
                "• C# to package into AAR (no Gradle!)\n" +
                "• Automatically copies to Unity project",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            if (!aarBuilt)
            {
                if (GUILayout.Button("🚀 Build AAR", GUILayout.Height(50)))
                {
                    BuildAAR();
                }
            }
            else
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("✅ AAR Built Successfully!", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Location: {aarPath}");
                EditorGUILayout.EndVertical();
            }
            
            if (isProcessing)
            {
                EditorGUILayout.Space();
                EditorGUI.ProgressBar(GUILayoutUtility.GetRect(18, 18), progress, statusMessage);
            }
        }
        
        void DrawStep5_GenerateBridge()
        {
            GUILayout.Label("🌉 Step 5: Generate C# Bridge", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "Generate Unity C# code to call your AAR.\n\n" +
                "This creates:\n" +
                "• AndroidJavaClass wrapper\n" +
                "• C#-style method names (PascalCase)\n" +
                "• Type conversions between Java and C#",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            if (!bridgeGenerated)
            {
                if (GUILayout.Button("🎯 Generate C# Bridge", GUILayout.Height(50)))
                {
                    GenerateCSharpBridge();
                }
            }
            else
            {
                EditorGUILayout.LabelField("C# Bridge Code:", EditorStyles.boldLabel);
                
                csharpScrollPos = EditorGUILayout.BeginScrollView(csharpScrollPos, GUILayout.Height(300));
                EditorGUILayout.TextArea(csharpBridgeCode, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
                
                EditorGUILayout.Space();
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Save to:", GUILayout.Width(60));
                EditorGUILayout.LabelField(string.IsNullOrEmpty(bridgeSavePath) ? "Not saved" : bridgeSavePath);
                
                if (GUILayout.Button("💾 Save", GUILayout.Width(100)))
                {
                    SaveCSharpBridge();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        
        void DrawStep6_ExtractLibraries()
        {
            GUILayout.Label("📚 Step 6: Extract Native Libraries", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "Extract .so files from the APK.\n\n" +
                "These native libraries:\n" +
                "• Contain the actual implementation\n" +
                "• Will be loaded by your AAR wrapper\n" +
                "• Support multiple architectures (arm64, x86, etc.)",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            if (!librariesExtracted)
            {
                if (GUILayout.Button("📲 Extract Native Libraries", GUILayout.Height(50)))
                {
                    ExtractLibraries();
                }
            }
            else
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("✅ Libraries Extracted!", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Extracted {nativeLibraries.Count} libraries");
                EditorGUILayout.LabelField("Location: Assets/Plugins/Android/libs/");
                EditorGUILayout.EndVertical();
            }
            
            if (isProcessing)
            {
                EditorGUILayout.Space();
                EditorGUI.ProgressBar(GUILayoutUtility.GetRect(18, 18), progress, statusMessage);
            }
        }
        
        void DrawStep7_Complete()
        {
            GUILayout.Label("🎉 Workflow Complete!", EditorStyles.largeLabel);
            
            EditorGUILayout.HelpBox(
                "✅ Your APK integration is complete!\n\n" +
                "What you have:\n" +
                "• AAR library with your wrapper code\n" +
                "• C# bridge for Unity integration\n" +
                "• Native .so libraries\n\n" +
                "You can now use the SDK functionality in your Unity project!",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("📁 Files Created:", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"• {aarPath}");
            if (!string.IsNullOrEmpty(bridgeSavePath))
            {
                EditorGUILayout.LabelField($"• {bridgeSavePath}");
            }
            EditorGUILayout.LabelField($"• Assets/Plugins/Android/libs/ ({nativeLibraries.Count} libraries)");
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(20);
            
            EditorGUILayout.LabelField("🚀 Next Steps:", EditorStyles.boldLabel);
            
            EditorGUILayout.HelpBox(
                "1. Use the C# bridge in your scripts\n" +
                "2. Build for Android\n" +
                "3. Test on device\n\n" +
                "Example usage:\n" +
                wrapperClassName + "Bridge.YourMethod();",
                MessageType.None
            );
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("🔄 Start New Workflow", GUILayout.Height(40)))
            {
                ResetWizard();
            }
        }
        
        void DrawNavigationButtons()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = currentStep > 0;
            if (GUILayout.Button("◀ Back", GUILayout.Height(30), GUILayout.Width(100)))
            {
                currentStep--;
            }
            GUI.enabled = true;
            
            GUILayout.FlexibleSpace();
            
            bool canProceed = CanProceedToNextStep();
            GUI.enabled = canProceed;
            
            if (currentStep < stepTitles.Length - 1)
            {
                if (GUILayout.Button("Next ▶", GUILayout.Height(30), GUILayout.Width(100)))
                {
                    currentStep++;
                }
            }
            
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
        }
        
        bool CanProceedToNextStep()
        {
            switch (currentStep)
            {
                case 0: return apkLoaded;
                case 1: return selectedClasses.Count > 0;
                case 2: return !string.IsNullOrEmpty(generatedJavaCode);
                case 3: return aarBuilt;
                case 4: return bridgeGenerated;
                case 5: return librariesExtracted;
                default: return true;
            }
        }
        
        // Implementation methods
        
        void AnalyzeAPK()
        {
            isProcessing = true;
            statusMessage = "Analyzing APK...";
            progress = 0.5f;
            
            try
            {
                // Extract classes
                allClasses = APKExtractor.ExtractClasses(apkPath, (p, msg) => {
                    progress = p;
                    statusMessage = msg;
                    Repaint();
                });
                
                totalClasses = allClasses.Count;
                
                // Preview native libraries
                nativeLibraries = NativeLibraryExtractor.PreviewNativeLibraries(apkPath);
                
                apkLoaded = true;
                statusMessage = "APK analyzed successfully!";
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error", "Failed to analyze APK: " + ex.Message, "OK");
            }
            finally
            {
                isProcessing = false;
                Repaint();
            }
        }
        
        void GenerateJavaWrapper()
        {
            // Auto-detect library name from native libraries
            var libraryName = "native_lib";
            if (nativeLibraries.Count > 0)
            {
                var firstLib = Path.GetFileNameWithoutExtension(nativeLibraries[0]);
                if (firstLib.StartsWith("lib"))
                {
                    libraryName = firstLib.Substring(3); // Remove "lib" prefix
                }
            }
            
            var code = $@"package {packageName};

/**
 * Wrapper for native SDK functionality
 * Auto-generated by Android Bridge Toolkit
 */
public class {wrapperClassName} {{
    
    static {{
        System.loadLibrary(""{libraryName}"");
    }}
    
";
            
            // Add methods from selected classes
            foreach (var classInfo in selectedClasses)
            {
                code += $"    // Methods from {classInfo.ClassName}\n";
                
                foreach (var method in classInfo.Methods.Take(10)) // Limit for demo
                {
                    var returnType = method.ReturnType ?? "void";
                    var methodName = method.Name;
                    
                    code += $"    public static native {returnType} {methodName}();\n";
                }
                
                code += "\n";
            }
            
            code += "}\n";
            
            generatedJavaCode = code;
        }
        
        void BuildAAR()
        {
            isProcessing = true;
            statusMessage = "Building AAR...";
            
            try
            {
                var config = new AARBuilderNative.BuildConfig
                {
                    ProjectName = wrapperClassName,
                    PackageName = packageName,
                    JavaCode = generatedJavaCode,
                    CopyToPlugins = true
                };
                
                var result = AARBuilderNative.BuildAAR(config, (p, msg) => {
                    progress = p;
                    statusMessage = msg;
                    Repaint();
                });
                
                if (result.Success)
                {
                    aarPath = result.AARPath;
                    aarBuilt = true;
                    AssetDatabase.Refresh();
                }
                else
                {
                    EditorUtility.DisplayDialog("Build Failed", result.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error", "Build failed: " + ex.Message, "OK");
            }
            finally
            {
                isProcessing = false;
                Repaint();
            }
        }
        
        void GenerateCSharpBridge()
        {
            // BridgeGenerator.Generate() is a static method
            csharpBridgeCode = BridgeGenerator.Generate(generatedJavaCode);
            
            bridgeGenerated = true;
        }
        
        void SaveCSharpBridge()
        {
            var path = EditorUtility.SaveFilePanel("Save C# Bridge", Application.dataPath, wrapperClassName + "Bridge.cs", "cs");
            
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, csharpBridgeCode);
                bridgeSavePath = path;
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Saved", "C# bridge saved successfully!", "OK");
            }
        }
        
        void ExtractLibraries()
        {
            isProcessing = true;
            statusMessage = "Extracting libraries...";
            
            try
            {
                var result = NativeLibraryExtractor.ExtractNativeLibraries(apkPath, null, false, (p, msg) => {
                    progress = p;
                    statusMessage = msg;
                    Repaint();
                });
                
                if (result.Success)
                {
                    librariesExtracted = true;
                    AssetDatabase.Refresh();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Failed to extract libraries: " + result.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error", "Extraction failed: " + ex.Message, "OK");
            }
            finally
            {
                isProcessing = false;
                Repaint();
            }
        }
        
        void ResetWizard()
        {
            currentStep = 0;
            apkPath = "";
            apkLoaded = false;
            allClasses.Clear();
            selectedClasses.Clear();
            expandedClasses.Clear();
            expandedPackages.Clear();
            generatedJavaCode = "";
            aarBuilt = false;
            bridgeGenerated = false;
            librariesExtracted = false;
            Repaint();
        }
    }
}
