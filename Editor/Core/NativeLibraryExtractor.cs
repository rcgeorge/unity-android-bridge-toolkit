//
// NativeLibraryExtractor.cs - Extract .so files from APK for Unity
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
    /// Extracts native libraries (.so files) from APK files and copies them to Unity's Plugins folder
    /// </summary>
    public static class NativeLibraryExtractor
    {
        /// <summary>
        /// Result of library extraction
        /// </summary>
        public class ExtractionResult
        {
            public bool Success;
            public string Message;
            public List<string> ExtractedLibraries = new List<string>();
            public Dictionary<string, int> LibrariesByArchitecture = new Dictionary<string, int>();
        }
        
        /// <summary>
        /// Extract native libraries from APK and copy to Unity project
        /// </summary>
        /// <param name="apkPath">Path to APK file</param>
        /// <param name="outputPath">Output path (default: Assets/Plugins/Android/libs/)</param>
        /// <param name="copyApk">Also copy the APK file to Assets/Plugins/Android/</param>
        /// <param name="onProgress">Progress callback (0-1, message)</param>
        public static ExtractionResult ExtractNativeLibraries(
            string apkPath, 
            string outputPath = null,
            bool copyApk = true,
            Action<float, string> onProgress = null)
        {
            var result = new ExtractionResult();
            
            try
            {
                // Default output path
                if (string.IsNullOrEmpty(outputPath))
                {
                    outputPath = Path.Combine(Application.dataPath, "Plugins", "Android", "libs");
                }
                
                onProgress?.Invoke(0.1f, "Opening APK...");
                
                // Open APK as ZIP
                using (var archive = ZipFile.OpenRead(apkPath))
                {
                    // Find all .so files in lib/ folder
                    var soFiles = new List<ZipArchiveEntry>();
                    
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName.StartsWith("lib/") && entry.FullName.EndsWith(".so"))
                        {
                            soFiles.Add(entry);
                        }
                    }
                    
                    if (soFiles.Count == 0)
                    {
                        result.Success = false;
                        result.Message = "No native libraries (.so files) found in APK.\n\nThis APK may not contain native code.";
                        return result;
                    }
                    
                    onProgress?.Invoke(0.2f, $"Found {soFiles.Count} native libraries...");
                    
                    // Create output directory
                    Directory.CreateDirectory(outputPath);
                    
                    // Extract each .so file
                    for (int i = 0; i < soFiles.Count; i++)
                    {
                        var entry = soFiles[i];
                        
                        // lib/arm64-v8a/libviture.so -> arm64-v8a/libviture.so
                        var relativePath = entry.FullName.Substring(4); // Remove "lib/"
                        var architecture = relativePath.Split('/')[0];
                        var fileName = Path.GetFileName(relativePath);
                        
                        // Track by architecture
                        if (!result.LibrariesByArchitecture.ContainsKey(architecture))
                        {
                            result.LibrariesByArchitecture[architecture] = 0;
                        }
                        result.LibrariesByArchitecture[architecture]++;
                        
                        // Create architecture subfolder
                        var archPath = Path.Combine(outputPath, architecture);
                        Directory.CreateDirectory(archPath);
                        
                        // Extract file
                        var outputFile = Path.Combine(archPath, fileName);
                        
                        onProgress?.Invoke(0.2f + (0.6f * i / soFiles.Count), $"Extracting {fileName}...");
                        
                        using (var entryStream = entry.Open())
                        using (var fileStream = File.Create(outputFile))
                        {
                            entryStream.CopyTo(fileStream);
                        }
                        
                        result.ExtractedLibraries.Add(relativePath);
                        
                        Debug.Log($"Extracted: {relativePath} -> {outputFile}");
                    }
                }
                
                // Copy APK file if requested
                if (copyApk)
                {
                    onProgress?.Invoke(0.9f, "Copying APK to Unity project...");
                    
                    var apkOutputPath = Path.Combine(Application.dataPath, "Plugins", "Android");
                    Directory.CreateDirectory(apkOutputPath);
                    
                    var apkFileName = Path.GetFileName(apkPath);
                    var apkDestination = Path.Combine(apkOutputPath, apkFileName);
                    
                    File.Copy(apkPath, apkDestination, overwrite: true);
                    
                    Debug.Log($"Copied APK: {apkPath} -> {apkDestination}");
                }
                
                onProgress?.Invoke(1f, "Complete!");
                
                result.Success = true;
                result.Message = BuildSuccessMessage(result);
                
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Extraction failed: {ex.Message}";
                Debug.LogError($"Native library extraction failed: {ex}");
                return result;
            }
        }
        
        private static string BuildSuccessMessage(ExtractionResult result)
        {
            var message = $"Successfully extracted {result.ExtractedLibraries.Count} native libraries!\n\n";
            
            message += "Architecture breakdown:\n";
            foreach (var kvp in result.LibrariesByArchitecture)
            {
                message += $"  • {kvp.Key}: {kvp.Value} libraries\n";
            }
            
            message += "\nFiles copied to:\n";
            message += "  Assets/Plugins/Android/libs/\n";
            message += "\nReady to use in your Unity project!";
            
            return message;
        }
        
        /// <summary>
        /// Preview what would be extracted without actually extracting
        /// </summary>
        public static List<string> PreviewNativeLibraries(string apkPath)
        {
            var libraries = new List<string>();
            
            try
            {
                using (var archive = ZipFile.OpenRead(apkPath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName.StartsWith("lib/") && entry.FullName.EndsWith(".so"))
                        {
                            libraries.Add(entry.FullName.Substring(4)); // Remove "lib/"
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to preview native libraries: {ex.Message}");
            }
            
            return libraries;
        }
    }
}
