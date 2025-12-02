//
// JadxIntegration.cs - JADX decompiler integration
// Copyright (c) 2024 Instemic
//

using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Instemic.AndroidBridge.Core
{
    /// <summary>
    /// Integrates with JADX APK decompiler
    /// </summary>
    public static class JadxIntegration
    {
        private const string JADX_PATH_KEY = "Instemic.AndroidBridge.JadxPath";
        private const string OUTPUT_DIR_KEY = "Instemic.AndroidBridge.DecompileOutputDir";
        
        /// <summary>
        /// Get configured JADX executable path
        /// </summary>
        public static string GetJadxPath()
        {
            return EditorPrefs.GetString(JADX_PATH_KEY, "");
        }
        
        /// <summary>
        /// Set JADX executable path
        /// </summary>
        public static void SetJadxPath(string path)
        {
            EditorPrefs.SetString(JADX_PATH_KEY, path);
        }
        
        /// <summary>
        /// Get output directory for decompiled files
        /// </summary>
        public static string GetOutputDirectory()
        {
            string defaultPath = Path.Combine(Application.dataPath, "../DecompiledAPKs");
            return EditorPrefs.GetString(OUTPUT_DIR_KEY, defaultPath);
        }
        
        /// <summary>
        /// Set output directory for decompiled files
        /// </summary>
        public static void SetOutputDirectory(string path)
        {
            EditorPrefs.SetString(OUTPUT_DIR_KEY, path);
        }
        
        /// <summary>
        /// Check if JADX is configured and exists
        /// </summary>
        public static bool IsJadxConfigured()
        {
            string jadxPath = GetJadxPath();
            if (string.IsNullOrEmpty(jadxPath))
                return false;
                
            return File.Exists(jadxPath);
        }
        
        /// <summary>
        /// Decompile APK file using JADX
        /// </summary>
        /// <param name="apkPath">Path to APK file</param>
        /// <param name="outputName">Name for output folder</param>
        /// <param name="onProgress">Progress callback (0-1)</param>
        /// <param name="onComplete">Completion callback with output path</param>
        /// <param name="onError">Error callback</param>
        public static void DecompileAPK(
            string apkPath,
            string outputName,
            Action<float, string> onProgress,
            Action<string> onComplete,
            Action<string> onError)
        {
            if (!IsJadxConfigured())
            {
                onError?.Invoke("JADX not configured. Please set JADX path in Settings.");
                return;
            }
            
            if (!File.Exists(apkPath))
            {
                onError?.Invoke($"APK file not found: {apkPath}");
                return;
            }
            
            string jadxPath = GetJadxPath();
            string outputDir = Path.Combine(GetOutputDirectory(), outputName);
            
            // Create output directory
            if (Directory.Exists(outputDir))
            {
                if (!EditorUtility.DisplayDialog(
                    "Output Directory Exists",
                    $"Directory already exists:\n{outputDir}\n\nOverwrite?",
                    "Yes", "Cancel"))
                {
                    onError?.Invoke("Decompilation cancelled by user.");
                    return;
                }
                
                Directory.Delete(outputDir, true);
            }
            
            Directory.CreateDirectory(outputDir);
            
            onProgress?.Invoke(0.1f, "Starting JADX...");
            
            try
            {
                // Build JADX command
                // jadx -d output_dir input.apk
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = jadxPath,
                    Arguments = $"-d \"{outputDir}\" \"{apkPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                
                onProgress?.Invoke(0.2f, "Decompiling APK...");
                
                Process process = new Process { StartInfo = startInfo };
                
                string output = "";
                string error = "";
                
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        output += e.Data + "\n";
                        UnityEngine.Debug.Log($"JADX: {e.Data}");
                    }
                };
                
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        error += e.Data + "\n";
                        UnityEngine.Debug.LogWarning($"JADX: {e.Data}");
                    }
                };
                
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                
                // Wait for completion (with timeout)
                bool completed = process.WaitForExit(300000); // 5 minutes timeout
                
                if (!completed)
                {
                    process.Kill();
                    onError?.Invoke("JADX decompilation timed out (5 minutes).");
                    return;
                }
                
                if (process.ExitCode != 0)
                {
                    onError?.Invoke($"JADX failed with exit code {process.ExitCode}:\n{error}");
                    return;
                }
                
                onProgress?.Invoke(1.0f, "Decompilation complete!");
                
                // Verify output exists
                if (Directory.Exists(outputDir))
                {
                    onComplete?.Invoke(outputDir);
                }
                else
                {
                    onError?.Invoke("JADX completed but output directory not found.");
                }
            }
            catch (Exception ex)
            {
                onError?.Invoke($"Error running JADX: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Get JADX download URL
        /// </summary>
        public static string GetJadxDownloadUrl()
        {
            return "https://github.com/skylot/jadx/releases";
        }
    }
}
