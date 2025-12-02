//
// BridgeGenerator.cs - Automatic C# bridge code generation
// Copyright (c) 2024 Instemic
//

using System;
using System.Linq;
using System.Text;

namespace Instemic.AndroidBridge.Core
{
    /// <summary>
    /// Generates C# Unity bridge code from Java classes
    /// </summary>
    public static class BridgeGenerator
    {
        /// <summary>
        /// Generate complete C# bridge from Java source code
        /// </summary>
        public static string Generate(string javaSource)
        {
            // Parse Java
            JavaClass javaClass = JavaParser.Parse(javaSource);
            
            // Generate C# bridge
            return GenerateCSharpBridge(javaClass);
        }
        
        /// <summary>
        /// Generate C# bridge code from parsed Java class
        /// </summary>
        public static string GenerateCSharpBridge(JavaClass javaClass)
        {
            StringBuilder cs = new StringBuilder();
            
            // Header comment
            cs.AppendLine("//");
            cs.AppendLine($"// Auto-generated Unity bridge for {javaClass.FullName}");
            cs.AppendLine($"// Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            cs.AppendLine("// Generator: Android Bridge Toolkit by Instemic");
            cs.AppendLine("// https://github.com/rcgeorge/unity-android-bridge-toolkit");
            cs.AppendLine("//");
            cs.AppendLine("// DO NOT EDIT MANUALLY - Regenerate using Android Bridge Toolkit");
            cs.AppendLine("//");
            cs.AppendLine();
            
            // Using statements
            cs.AppendLine("using UnityEngine;");
            cs.AppendLine("using System;");
            cs.AppendLine();
            
            // XML documentation
            cs.AppendLine("/// <summary>");
            cs.AppendLine($"/// Unity bridge for {javaClass.ClassName}");
            cs.AppendLine($"/// Java class: {javaClass.FullName}");
            cs.AppendLine("/// </summary>");
            
            // Class declaration
            cs.AppendLine($"public class {javaClass.ClassName}Bridge");
            cs.AppendLine("{");
            
            // Determine what references we need
            bool hasInstanceMethods = javaClass.Methods.Any(m => !m.IsStatic);
            bool hasStaticMethods = javaClass.Methods.Any(m => m.IsStatic);
            
            // Static class reference for static methods
            if (hasStaticMethods)
            {
                cs.AppendLine("    private static AndroidJavaClass javaClass;");
                cs.AppendLine();
            }
            
            // Instance object reference for instance methods
            if (hasInstanceMethods)
            {
                cs.AppendLine("    private AndroidJavaObject javaObject;");
                cs.AppendLine();
            }
            
            // Static constructor
            if (hasStaticMethods)
            {
                cs.AppendLine($"    static {javaClass.ClassName}Bridge()");
                cs.AppendLine("    {");
                cs.AppendLine($"        javaClass = new AndroidJavaClass(\"{javaClass.FullName}\");");
                cs.AppendLine("    }");
                cs.AppendLine();
            }
            
            // Instance constructor
            if (hasInstanceMethods)
            {
                cs.AppendLine($"    public {javaClass.ClassName}Bridge()");
                cs.AppendLine("    {");
                cs.AppendLine($"        AndroidJavaClass cls = new AndroidJavaClass(\"{javaClass.FullName}\");");
                cs.AppendLine("        // TODO: Adjust this based on how the Java class is instantiated");
                cs.AppendLine("        javaObject = cls.CallStatic<AndroidJavaObject>(\"getInstance\");");
                cs.AppendLine("    }");
                cs.AppendLine();
            }
            
            // Generate methods
            foreach (var method in javaClass.Methods)
            {
                cs.Append(GenerateMethod(method));
                cs.AppendLine();
            }
            
            cs.AppendLine("}");
            
            return cs.ToString();
        }
        
        /// <summary>
        /// Generate a single method
        /// </summary>
        private static string GenerateMethod(JavaMethod method)
        {
            StringBuilder sb = new StringBuilder();
            
            string returnType = TypeConverter.ConvertType(method.ReturnType);
            string methodName = ToPascalCase(method.Name);
            
            // Build parameter list
            var paramList = method.Parameters
                .Select(p => $"{TypeConverter.ConvertType(p.Type)} {p.Name}")
                .ToList();
            
            // Build argument list for Java call
            var argList = method.Parameters.Select(p => p.Name).ToList();
            
            // XML documentation
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    /// Calls {method.Name} in Java");
            
            if (method.Parameters.Count > 0)
            {
                sb.AppendLine("    /// </summary>");
                foreach (var param in method.Parameters)
                {
                    sb.AppendLine($"    /// <param name=\"{param.Name}\">{param.Type}</param>");
                }
            }
            else
            {
                sb.AppendLine("    /// </summary>");
            }
            
            if (returnType != "void")
            {
                sb.AppendLine($"    /// <returns>{returnType}</returns>");
            }
            
            // Method signature
            string modifiers = method.IsStatic ? "public static" : "public";
            sb.AppendLine($"    {modifiers} {returnType} {methodName}({string.Join(", ", paramList)})");
            sb.AppendLine("    {");
            
            // Method body
            string javaCall = method.IsStatic ? "javaClass.CallStatic" : "javaObject.Call";
            string args = argList.Count > 0 ? $", {string.Join(", ", argList)}" : "";
            
            if (returnType == "void")
            {
                sb.AppendLine($"        {javaCall}(\"{method.Name}\"{args});");
            }
            else
            {
                string genericParam = TypeConverter.GetGenericParameter(returnType);
                sb.AppendLine($"        return {javaCall}{genericParam}(\"{method.Name}\"{args});");
            }
            
            sb.AppendLine("    }");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// Convert method name to PascalCase for C# conventions
        /// </summary>
        private static string ToPascalCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
