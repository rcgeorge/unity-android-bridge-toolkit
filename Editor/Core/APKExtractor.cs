//
// APKExtractor.cs - Native APK extraction and DEX parsing
// Copyright (c) 2024 Instemic
//

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace Instemic.AndroidBridge.Core
{
    /// <summary>
    /// Extracts and analyzes APK files natively without external tools
    /// </summary>
    public static class APKExtractor
    {
        /// <summary>
        /// Extract classes from APK file
        /// </summary>
        /// <param name="apkPath">Path to APK file</param>
        /// <param name="onProgress">Progress callback (0-1, message)</param>
        /// <returns>List of extracted classes with metadata</returns>
        public static List<DexClass> ExtractClasses(string apkPath, Action<float, string> onProgress = null)
        {
            if (!File.Exists(apkPath))
            {
                throw new FileNotFoundException($"APK file not found: {apkPath}");
            }
            
            onProgress?.Invoke(0.1f, "Opening APK file...");
            
            List<DexClass> allClasses = new List<DexClass>();
            
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(apkPath))
                {
                    // Find all DEX files (classes.dex, classes2.dex, etc.)
                    List<ZipArchiveEntry> dexEntries = new List<ZipArchiveEntry>();
                    
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.Name.StartsWith("classes") && entry.Name.EndsWith(".dex"))
                        {
                            dexEntries.Add(entry);
                        }
                    }
                    
                    if (dexEntries.Count == 0)
                    {
                        throw new Exception("No DEX files found in APK");
                    }
                    
                    onProgress?.Invoke(0.2f, $"Found {dexEntries.Count} DEX file(s)");
                    
                    // Parse each DEX file
                    for (int i = 0; i < dexEntries.Count; i++)
                    {
                        ZipArchiveEntry entry = dexEntries[i];
                        float progress = 0.2f + (0.7f * (i / (float)dexEntries.Count));
                        onProgress?.Invoke(progress, $"Parsing {entry.Name}...");
                        
                        // Read DEX data
                        using (Stream stream = entry.Open())
                        using (MemoryStream ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            byte[] dexData = ms.ToArray();
                            
                            // Parse DEX file
                            DexParser parser = new DexParser(dexData);
                            List<DexClass> classes = parser.Parse();
                            
                            Debug.Log($"Parsed {entry.Name}: Found {classes.Count} classes");
                            allClasses.AddRange(classes);
                        }
                    }
                }
                
                onProgress?.Invoke(0.9f, "Filtering classes...");
                
                // Filter out system classes and duplicates
                allClasses = FilterClasses(allClasses);
                
                onProgress?.Invoke(1.0f, $"Complete! Found {allClasses.Count} classes");
                
                return allClasses;
            }
            catch (Exception ex)
            {
                Debug.LogError($"APK extraction failed: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
        
        /// <summary>
        /// Filter out system classes and keep only app classes
        /// </summary>
        private static List<DexClass> FilterClasses(List<DexClass> classes)
        {
            List<DexClass> filtered = new List<DexClass>();
            HashSet<string> seen = new HashSet<string>();
            
            // System packages to filter out
            string[] systemPackages = new string[]
            {
                "android.",
                "androidx.",
                "com.google.android.",
                "java.",
                "javax.",
                "kotlin.",
                "kotlinx.",
                "dalvik.",
                "org.xml.",
                "org.w3c.",
                "org.json."
            };
            
            foreach (DexClass dexClass in classes)
            {
                // Skip if already seen
                if (seen.Contains(dexClass.ClassName))
                    continue;
                    
                // Skip system classes
                bool isSystemClass = false;
                foreach (string pkg in systemPackages)
                {
                    if (dexClass.ClassName.StartsWith(pkg))
                    {
                        isSystemClass = true;
                        break;
                    }
                }
                
                if (isSystemClass)
                    continue;
                    
                // Skip classes with no public methods
                if (dexClass.Methods.Count == 0)
                    continue;
                    
                seen.Add(dexClass.ClassName);
                filtered.Add(dexClass);
            }
            
            // Sort by package and class name
            filtered.Sort((a, b) => string.Compare(a.ClassName, b.ClassName, StringComparison.Ordinal));
            
            return filtered;
        }
        
        /// <summary>
        /// Get APK package name from AndroidManifest.xml
        /// </summary>
        public static string GetPackageName(string apkPath)
        {
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(apkPath))
                {
                    ZipArchiveEntry manifestEntry = archive.GetEntry("AndroidManifest.xml");
                    
                    if (manifestEntry != null)
                    {
                        // TODO: Parse binary XML format
                        // For now, extract package name from classes
                        return "com.unknown.package";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to get package name: {ex.Message}");
            }
            
            return "com.unknown.package";
        }
        
        /// <summary>
        /// Convert DexClass to JavaClass for bridge generation
        /// </summary>
        public static JavaClass ConvertToJavaClass(DexClass dexClass)
        {
            JavaClass javaClass = new JavaClass
            {
                PackageName = dexClass.GetPackageName(),
                ClassName = dexClass.GetSimpleClassName()
            };
            
            foreach (DexMethod dexMethod in dexClass.Methods)
            {
                if (!dexMethod.IsPublic)
                    continue;
                    
                JavaMethod javaMethod = new JavaMethod
                {
                    Name = dexMethod.Name,
                    ReturnType = dexMethod.ReturnType,
                    IsStatic = dexMethod.IsStatic,
                    IsPublic = dexMethod.IsPublic
                };
                
                // Add parameters
                for (int i = 0; i < dexMethod.Parameters.Count; i++)
                {
                    javaMethod.Parameters.Add(new JavaParameter
                    {
                        Type = dexMethod.Parameters[i],
                        Name = $"param{i}"
                    });
                }
                
                javaClass.Methods.Add(javaMethod);
            }
            
            return javaClass;
        }
    }
}
