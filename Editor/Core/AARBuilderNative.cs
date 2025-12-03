//
// AARBuilderNative.cs - Build AAR libraries without Gradle using native C#
// Copyright (c) 2024 Instemic
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Instemic.AndroidBridge.Core
{
    /// <summary>
    /// Builds Android AAR libraries using only javac and C# ZIP - no Gradle required!
    /// </summary>
    public static class AARBuilderNative
    {
        public class BuildResult
        {
            public bool Success;
            public string Message;
            public string AARPath;
            public string BuildLog;
        }
        
        public class BuildConfig
        {
            public string ProjectName = "AndroidBridge";
            public string PackageName = "com.unity.androidbridge";
            public string JavaCode;
            public string OutputDirectory;
            public bool CopyToPlugins = true;
        }
        
        /// <summary>
        /// Build an AAR from Java source code - no Gradle required!
        /// </summary>
        public static BuildResult BuildAAR(BuildConfig config, Action<float, string> onProgress = null)
        {
            var result = new BuildResult { BuildLog = "" };
            
            try
            {
                onProgress?.Invoke(0.1f, "Validating configuration...");
                
                // Validate
                if (string.IsNullOrEmpty(config.JavaCode))
                {
                    result.Success = false;
                    result.Message = "No Java source code provided";
                    return result;
                }
                
                if (string.IsNullOrEmpty(config.OutputDirectory))
                {
                    config.OutputDirectory = Path.Combine(Application.dataPath, "..", "Temp", "AARBuildNative");
                }
                
                // Create build directory
                var buildPath = Path.Combine(config.OutputDirectory, config.ProjectName);
                if (Directory.Exists(buildPath))
                {
                    Directory.Delete(buildPath, true);
                }
                Directory.CreateDirectory(buildPath);
                
                onProgress?.Invoke(0.2f, "Writing Java source files...");
                
                // Write Java source file
                var packageName = ExtractPackageName(config.JavaCode) ?? config.PackageName;
                var className = ExtractClassName(config.JavaCode);
                
                var srcPath = Path.Combine(buildPath, "src");
                var packagePath = Path.Combine(srcPath, packageName.Replace('.', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(packagePath);
                
                var javaFile = Path.Combine(packagePath, className + ".java");
                File.WriteAllText(javaFile, config.JavaCode);
                
                result.BuildLog += $"Created: {javaFile}\n";
                
                onProgress?.Invoke(0.3f, "Compiling Java code...");
                
                // Compile with javac
                var classesPath = Path.Combine(buildPath, "classes");
                Directory.CreateDirectory(classesPath);
                
                var compileLog = CompileJava(javaFile, classesPath);
                result.BuildLog += compileLog + "\n";
                
                onProgress?.Invoke(0.6f, "Creating AAR structure...");
                
                // Create AAR structure
                var aarPath = CreateAAR(buildPath, classesPath, packageName, config.ProjectName);
                
                result.BuildLog += $"Created AAR: {aarPath}\n";
                
                onProgress?.Invoke(0.9f, "Copying to Unity project...");
                
                // Copy to Unity if requested
                if (config.CopyToPlugins)
                {
                    var pluginsPath = Path.Combine(Application.dataPath, "Plugins", "Android");
                    Directory.CreateDirectory(pluginsPath);
                    
                    var destPath = Path.Combine(pluginsPath, Path.GetFileName(aarPath));
                    File.Copy(aarPath, destPath, true);
                    
                    result.AARPath = destPath;
                    result.BuildLog += $"Copied to: {destPath}\n";
                }
                else
                {
                    result.AARPath = aarPath;
                }
                
                onProgress?.Invoke(1f, "Build complete!");
                
                result.Success = true;
                result.Message = $"Successfully built AAR: {Path.GetFileName(result.AARPath)}\n\nNo Gradle required!";
                
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Build failed: {ex.Message}";
                result.BuildLog += $"\nERROR: {ex}\n";
                UnityEngine.Debug.LogError($"AAR build failed: {ex}");
                return result;
            }
        }
        
        private static string CompileJava(string javaFile, string outputPath)
        {
            // Find javac
            var javacPath = FindJavaC();
            if (string.IsNullOrEmpty(javacPath))
            {
                throw new Exception("javac not found. Please ensure JDK is installed and JAVA_HOME is set.");
            }
            
            var args = $"-d \"{outputPath}\" \"{javaFile}\"";
            
            var startInfo = new ProcessStartInfo
            {
                FileName = javacPath,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            
            var output = new StringBuilder();
            var process = new Process { StartInfo = startInfo };
            
            process.OutputDataReceived += (sender, e) => {
                if (e.Data != null)
                {
                    output.AppendLine(e.Data);
                    UnityEngine.Debug.Log($"javac: {e.Data}");
                }
            };
            
            process.ErrorDataReceived += (sender, e) => {
                if (e.Data != null)
                {
                    output.AppendLine(e.Data);
                    UnityEngine.Debug.LogWarning($"javac: {e.Data}");
                }
            };
            
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            
            if (process.ExitCode != 0)
            {
                throw new Exception($"Java compilation failed with exit code {process.ExitCode}\n{output}");
            }
            
            return output.ToString();
        }
        
        private static string CreateAAR(string buildPath, string classesPath, string packageName, string projectName)
        {
            var aarPath = Path.Combine(buildPath, projectName + ".aar");
            
            // AAR is a ZIP file with specific structure:
            // - AndroidManifest.xml
            // - classes.jar
            // - R.txt (optional)
            // - res/ (optional)
            
            using (var archive = ZipFile.Open(aarPath, ZipArchiveMode.Create))
            {
                // 1. Create AndroidManifest.xml
                var manifestContent = CreateManifest(packageName);
                var manifestEntry = archive.CreateEntry("AndroidManifest.xml");
                using (var writer = new StreamWriter(manifestEntry.Open()))
                {
                    writer.Write(manifestContent);
                }
                
                // 2. Create classes.jar from compiled .class files
                var jarPath = Path.Combine(buildPath, "classes.jar");
                CreateJar(classesPath, jarPath);
                archive.CreateEntryFromFile(jarPath, "classes.jar");
                
                // 3. Create empty R.txt (required for AAR format)
                var rEntry = archive.CreateEntry("R.txt");
                using (var writer = new StreamWriter(rEntry.Open()))
                {
                    writer.Write("");
                }
            }
            
            return aarPath;
        }
        
        private static void CreateJar(string classesPath, string jarPath)
        {
            // JAR is just a ZIP file with compiled .class files
            using (var archive = ZipFile.Open(jarPath, ZipArchiveMode.Create))
            {
                AddDirectoryToZip(archive, classesPath, "");
            }
        }
        
        private static void AddDirectoryToZip(ZipArchive archive, string sourcePath, string entryPrefix)
        {
            foreach (var file in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
            {
                var relativePath = file.Substring(sourcePath.Length + 1).Replace('\\', '/');
                var entryName = string.IsNullOrEmpty(entryPrefix) ? relativePath : entryPrefix + "/" + relativePath;
                
                archive.CreateEntryFromFile(file, entryName);
            }
        }
        
        private static string CreateManifest(string packageName)
        {
            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android""
    package=""{packageName}"">
    
    <uses-sdk android:minSdkVersion=""24"" />
    
</manifest>";
        }
        
        private static string ExtractPackageName(string javaCode)
        {
            var lines = javaCode.Split('\n');
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("package "))
                {
                    var packageLine = trimmed.Substring(8).TrimEnd(';', ' ');
                    return packageLine;
                }
            }
            return null;
        }
        
        private static string ExtractClassName(string javaCode)
        {
            var lines = javaCode.Split('\n');
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.Contains("class ") && (trimmed.StartsWith("public ") || trimmed.StartsWith("class ")))
                {
                    var parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var classIndex = Array.IndexOf(parts, "class");
                    if (classIndex >= 0 && classIndex < parts.Length - 1)
                    {
                        return parts[classIndex + 1].TrimEnd('{');
                    }
                }
            }
            return "Wrapper";
        }
        
        private static string FindJavaC()
        {
            // Try JAVA_HOME first
            var javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(javaHome))
            {
                var isWindows = Application.platform == RuntimePlatform.WindowsEditor;
                var javacPath = Path.Combine(javaHome, "bin", isWindows ? "javac.exe" : "javac");
                
                if (File.Exists(javacPath))
                {
                    return javacPath;
                }
            }
            
            // Try system PATH
            var isWin = Application.platform == RuntimePlatform.WindowsEditor;
            return isWin ? "javac.exe" : "javac";
        }
    }
}
