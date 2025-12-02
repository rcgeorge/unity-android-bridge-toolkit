//
// Java Source Code Parser for Android Bridge Toolkit
// Copyright (c) 2024 Instemic
//

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Instemic.AndroidBridge.Models;

namespace Instemic.AndroidBridge
{
    /// <summary>
    /// Parses Java source code to extract class structure.
    /// Uses regex-based parsing for simplicity (v1.0).
    /// </summary>
    public class JavaParser
    {
        private string sourceCode;

        public JavaParser(string javaSource)
        {
            sourceCode = javaSource;
        }

        /// <summary>
        /// Parses the Java source code and returns a JavaClass object.
        /// </summary>
        public JavaClass Parse()
        {
            var javaClass = new JavaClass();
            javaClass.PackageName = ParsePackage();
            javaClass.ClassName = ParseClassName();
            javaClass.Methods = ParseMethods();
            return javaClass;
        }

        /// <summary>
        /// Extracts the package name from Java source.
        /// </summary>
        private string ParsePackage()
        {
            var match = Regex.Match(sourceCode, @"package\s+([\w\.]+);");
            return match.Success ? match.Groups[1].Value : "";
        }

        /// <summary>
        /// Extracts the class name from Java source.
        /// </summary>
        private string ParseClassName()
        {
            var match = Regex.Match(sourceCode, @"(?:public\s+)?class\s+(\w+)");
            return match.Success ? match.Groups[1].Value : "UnknownClass";
        }

        /// <summary>
        /// Extracts all methods from Java source.
        /// Pattern matches: [modifiers] returnType methodName(params)
        /// </summary>
        private List<JavaMethod> ParseMethods()
        {
            var methods = new List<JavaMethod>();

            // Match method signatures
            // Pattern: [public/private/protected] [static] [final] returnType methodName(params)
            var pattern = @"(?:public|private|protected)?\s*" +
                         @"(static)?\s*" +
                         @"(?:final)?\s*" +
                         @"([\w<>\[\]]+)\s+" +  // Return type
                         @"(\w+)\s*" +           // Method name
                         @"\(([^\)]*)\)";

            var matches = Regex.Matches(sourceCode, pattern);

            foreach (Match match in matches)
            {
                // Skip constructors and common false positives
                string methodName = match.Groups[3].Value;
                if (methodName == "class" || methodName == "interface") continue;

                var method = new JavaMethod
                {
                    IsStatic = !string.IsNullOrWhiteSpace(match.Groups[1].Value),
                    ReturnType = match.Groups[2].Value,
                    Name = methodName,
                    Parameters = ParseParameters(match.Groups[4].Value)
                };

                methods.Add(method);
            }

            return methods;
        }

        /// <summary>
        /// Parses method parameters from parameter string.
        /// Handles: "int a, String b, List<String> c"
        /// </summary>
        private List<JavaParameter> ParseParameters(string paramString)
        {
            var parameters = new List<JavaParameter>();

            if (string.IsNullOrWhiteSpace(paramString))
                return parameters;

            // Split parameters while respecting generic brackets
            var paramParts = SplitParameters(paramString);

            foreach (var part in paramParts)
            {
                var trimmed = part.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                // Split on last space to separate type from name
                var lastSpace = trimmed.LastIndexOf(' ');
                if (lastSpace > 0)
                {
                    string type = trimmed.Substring(0, lastSpace).Trim();
                    string name = trimmed.Substring(lastSpace + 1).Trim();

                    // Remove 'final' modifier if present
                    type = Regex.Replace(type, @"^final\s+", "");

                    parameters.Add(new JavaParameter
                    {
                        Type = type,
                        Name = name
                    });
                }
            }

            return parameters;
        }

        /// <summary>
        /// Splits parameter string by commas, respecting generic brackets.
        /// Example: "List<String> a, Map<String, Integer> b" -> ["List<String> a", "Map<String, Integer> b"]
        /// </summary>
        private List<string> SplitParameters(string paramString)
        {
            var result = new List<string>();
            var current = "";
            int bracketDepth = 0;

            foreach (char c in paramString)
            {
                if (c == '<')
                {
                    bracketDepth++;
                    current += c;
                }
                else if (c == '>')
                {
                    bracketDepth--;
                    current += c;
                }
                else if (c == ',' && bracketDepth == 0)
                {
                    // Split here
                    if (!string.IsNullOrWhiteSpace(current))
                        result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }

            // Add last parameter
            if (!string.IsNullOrWhiteSpace(current))
                result.Add(current);

            return result;
        }
    }
}
