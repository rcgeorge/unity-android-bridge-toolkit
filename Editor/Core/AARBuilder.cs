//
// AARBuilder.cs - Build Android AAR libraries from Java source
// Copyright (c) 2024 Instemic
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Instemic.AndroidBridge.Core
{
    /// <summary>
    /// Builds Android AAR libraries from Java source files using Gradle
    /// </summary>
    public static class AARBuilder
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
            public List<string> JavaSourceFiles = new List<string>();
            public string OutputDirectory;
            public bool CopyToPlugins = true;
        }
        
        /// <summary>
        /// Build an AAR from Java source files
        /// </summary>
        public static BuildResult BuildAAR(BuildConfig config, Action<float, string> onProgress = null)
        {
            var result = new BuildResult();
            
            try
            {
                onProgress?.Invoke(0.1f, "Validating configuration...");
                
                // Validate
                if (config.JavaSourceFiles.Count == 0)
                {
                    result.Success = false;
                    result.Message = "No Java source files specified";
                    return result;
                }
                
                if (string.IsNullOrEmpty(config.OutputDirectory))
                {
                    config.OutputDirectory = Path.Combine(Application.dataPath, "..", "Temp", "AARBuild");
                }
                
                onProgress?.Invoke(0.2f, "Creating Gradle project...");
                
                // Create temporary Gradle project
                var projectPath = CreateGradleProject(config);
                
                onProgress?.Invoke(0.3f, "Copying Java source files...");
                
                // Copy Java files
                CopyJavaFiles(config, projectPath);
                
                onProgress?.Invoke(0.4f, "Running Gradle build...");
                
                // Build with Gradle
                var buildLog = RunGradleBuild(projectPath);
                result.BuildLog = buildLog;
                
                onProgress?.Invoke(0.8f, "Copying AAR output...");
                
                // Find output AAR
                var aarPath = FindOutputAAR(projectPath);
                
                if (string.IsNullOrEmpty(aarPath) || !File.Exists(aarPath))
                {
                    result.Success = false;
                    result.Message = "AAR file not found after build\n\nBuild log:\n" + buildLog;
                    return result;
                }
                
                // Copy to Unity project if requested
                if (config.CopyToPlugins)
                {
                    var pluginsPath = Path.Combine(Application.dataPath, "Plugins", "Android");
                    Directory.CreateDirectory(pluginsPath);
                    
                    var destPath = Path.Combine(pluginsPath, Path.GetFileName(aarPath));
                    File.Copy(aarPath, destPath, true);
                    
                    result.AARPath = destPath;
                    
                    UnityEngine.Debug.Log($"AAR copied to: {destPath}");
                }
                else
                {
                    result.AARPath = aarPath;
                }
                
                onProgress?.Invoke(1f, "Build complete!");
                
                result.Success = true;
                result.Message = $"Successfully built AAR: {Path.GetFileName(result.AARPath)}";
                
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Build failed: {ex.Message}\n\n{result.BuildLog}";
                UnityEngine.Debug.LogError($"AAR build failed: {ex}");
                return result;
            }
        }
        
        private static string CreateGradleProject(BuildConfig config)
        {
            var projectPath = Path.Combine(config.OutputDirectory, config.ProjectName);
            
            // Clean and create directory
            if (Directory.Exists(projectPath))
            {
                Directory.Delete(projectPath, true);
            }
            Directory.CreateDirectory(projectPath);
            
            // Create build.gradle
            var buildGradleTemplate = LoadTemplate("build.gradle.template");
            buildGradleTemplate = buildGradleTemplate.Replace("{{PACKAGE_NAME}}", config.PackageName);
            File.WriteAllText(Path.Combine(projectPath, "build.gradle"), buildGradleTemplate);
            
            // Create settings.gradle
            var settingsGradleTemplate = LoadTemplate("settings.gradle.template");
            settingsGradleTemplate = settingsGradleTemplate.Replace("{{PROJECT_NAME}}", config.ProjectName);
            File.WriteAllText(Path.Combine(projectPath, "settings.gradle"), settingsGradleTemplate);
            
            // Create gradle.properties
            File.WriteAllText(Path.Combine(projectPath, "gradle.properties"), 
                "android.useAndroidX=true\n" +
                "android.enableJetifier=true\n" +
                "org.gradle.jvmargs=-Xmx2048m\n");
            
            // Create src/main directory structure
            var srcMainPath = Path.Combine(projectPath, "src", "main");
            var javaPath = Path.Combine(srcMainPath, "java");
            Directory.CreateDirectory(javaPath);
            
            // Create AndroidManifest.xml
            var manifestPath = Path.Combine(srcMainPath, "AndroidManifest.xml");
            File.WriteAllText(manifestPath, 
                $"<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                $"<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\">\n" +
                $"</manifest>\n");
            
            return projectPath;
        }
        
        private static void CopyJavaFiles(BuildConfig config, string projectPath)
        {
            var javaPath = Path.Combine(projectPath, "src", "main", "java");
            
            foreach (var sourceFile in config.JavaSourceFiles)
            {
                if (!File.Exists(sourceFile))
                {
                    UnityEngine.Debug.LogWarning($"Source file not found: {sourceFile}");
                    continue;
                }
                
                // Read Java file to extract package
                var javaContent = File.ReadAllText(sourceFile);
                var packageName = ExtractPackageName(javaContent);
                
                if (string.IsNullOrEmpty(packageName))
                {
                    packageName = config.PackageName;
                }
                
                // Create package directory
                var packagePath = Path.Combine(javaPath, packageName.Replace('.', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(packagePath);
                
                // Copy file
                var fileName = Path.GetFileName(sourceFile);
                var destPath = Path.Combine(packagePath, fileName);
                File.Copy(sourceFile, destPath, true);
                
                UnityEngine.Debug.Log($"Copied: {fileName} -> {packagePath}");
            }
        }
        
        private static string ExtractPackageName(string javaContent)
        {
            var lines = javaContent.Split('\n');
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
        
        private static string RunGradleBuild(string projectPath)
        {
            var isWindows = Application.platform == RuntimePlatform.WindowsEditor;
            var gradleCommand = isWindows ? "gradlew.bat" : "./gradlew";
            
            // Check if gradlew exists, if not use system gradle
            var gradlewPath = Path.Combine(projectPath, isWindows ? "gradlew.bat" : "gradlew");
            
            if (!File.Exists(gradlewPath))
            {
                // Create gradle wrapper
                CreateGradleWrapper(projectPath);
            }
            
            var startInfo = new ProcessStartInfo
            {
                FileName = isWindows ? "cmd.exe" : "/bin/bash",
                Arguments = isWindows 
                    ? $"/c cd /d \"{projectPath}\" && gradlew.bat assembleRelease" 
                    : $"-c \"cd '{projectPath}' && ./gradlew assembleRelease\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = projectPath
            };
            
            var output = "";
            var process = new Process { StartInfo = startInfo };
            
            process.OutputDataReceived += (sender, args) => {
                if (args.Data != null)
                {
                    output += args.Data + "\n";
                    UnityEngine.Debug.Log($"Gradle: {args.Data}");
                }
            };
            
            process.ErrorDataReceived += (sender, args) => {
                if (args.Data != null)
                {
                    output += args.Data + "\n";
                    UnityEngine.Debug.LogWarning($"Gradle: {args.Data}");
                }
            };
            
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            
            if (process.ExitCode != 0)
            {
                throw new Exception($"Gradle build failed with exit code {process.ExitCode}");
            }
            
            return output;
        }
        
        private static void CreateGradleWrapper(string projectPath)
        {
            // Create minimal gradle wrapper
            var isWindows = Application.platform == RuntimePlatform.WindowsEditor;
            
            if (isWindows)
            {
                var gradlewBat = Path.Combine(projectPath, "gradlew.bat");
                File.WriteAllText(gradlewBat, "@echo off\ngradle %*\n");
            }
            else
            {
                var gradlewSh = Path.Combine(projectPath, "gradlew");
                File.WriteAllText(gradlewSh, "#!/bin/sh\ngradle \"$@\"\n");
                
                // Make executable
                var chmod = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "chmod",
                        Arguments = $"+x \"{gradlewSh}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                chmod.Start();
                chmod.WaitForExit();
            }
        }
        
        private static string FindOutputAAR(string projectPath)
        {
            var buildOutputPath = Path.Combine(projectPath, "build", "outputs", "aar");
            
            if (!Directory.Exists(buildOutputPath))
            {
                return null;
            }
            
            var aarFiles = Directory.GetFiles(buildOutputPath, "*.aar");
            return aarFiles.FirstOrDefault();
        }
        
        private static string LoadTemplate(string templateName)
        {
            var templatePath = Path.Combine(
                Path.GetDirectoryName(typeof(AARBuilder).Assembly.Location),
                "..", "..", "Templates", templateName
            );
            
            // Try alternative path for package
            if (!File.Exists(templatePath))
            {
                // Search in package
                var guids = AssetDatabase.FindAssets($"t:TextAsset {templateName}");
                if (guids.Length > 0)
                {
                    templatePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    return File.ReadAllText(templatePath);
                }
            }
            
            if (File.Exists(templatePath))
            {
                return File.ReadAllText(templatePath);
            }
            
            throw new FileNotFoundException($"Template not found: {templateName}");
        }
    }
}
