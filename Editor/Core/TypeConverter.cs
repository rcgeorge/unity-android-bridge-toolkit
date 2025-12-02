//
// TypeConverter.cs - Java to C# type conversion
// Copyright (c) 2024 Instemic
//

using System.Text.RegularExpressions;

namespace Instemic.AndroidBridge.Core
{
    /// <summary>
    /// Handles conversion between Java and C# types
    /// </summary>
    public static class TypeConverter
    {
        /// <summary>
        /// Convert Java type to C# type
        /// </summary>
        public static string ConvertType(string javaType)
        {
            if (string.IsNullOrEmpty(javaType))
                return "void";
            
            // Remove generics for now (TODO: Handle properly)
            javaType = Regex.Replace(javaType, @"<.*?>", "");
            javaType = javaType.Trim();
            
            // Primitive types
            switch (javaType)
            {
                case "int": return "int";
                case "long": return "long";
                case "float": return "float";
                case "double": return "double";
                case "boolean": return "bool";
                case "byte": return "byte";
                case "short": return "short";
                case "char": return "char";
                case "void": return "void";
                
                // Common Java types
                case "String": return "string";
                case "Integer": return "int";
                case "Long": return "long";
                case "Float": return "float";
                case "Double": return "double";
                case "Boolean": return "bool";
                
                // Arrays
                case "byte[]": return "byte[]";
                case "int[]": return "int[]";
                case "float[]": return "float[]";
                case "double[]": return "double[]";
                case "String[]": return "string[]";
                case "boolean[]": return "bool[]";
            }
            
            // Handle arrays
            if (javaType.EndsWith("[]"))
            {
                string baseType = javaType.Substring(0, javaType.Length - 2);
                string convertedBase = ConvertType(baseType);
                
                // For object arrays, use AndroidJavaObject[]
                if (IsObjectType(convertedBase))
                    return "AndroidJavaObject[]";
                    
                return convertedBase + "[]";
            }
            
            // For all other objects, use AndroidJavaObject
            return "AndroidJavaObject";
        }
        
        /// <summary>
        /// Check if a type is an object (not primitive)
        /// </summary>
        private static bool IsObjectType(string csharpType)
        {
            switch (csharpType)
            {
                case "int":
                case "long":
                case "float":
                case "double":
                case "bool":
                case "byte":
                case "short":
                case "char":
                case "void":
                case "string":
                    return false;
                default:
                    return true;
            }
        }
        
        /// <summary>
        /// Get the generic parameter for AndroidJavaClass.Call
        /// </summary>
        public static string GetGenericParameter(string csharpType)
        {
            if (csharpType == "void")
                return "";
            
            return $"<{csharpType}>";
        }
    }
}
