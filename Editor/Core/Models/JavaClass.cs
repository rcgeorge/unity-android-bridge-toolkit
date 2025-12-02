//
// JavaClass.cs - Java class representation
// Copyright (c) 2024 Instemic
//

using System.Collections.Generic;

namespace Instemic.AndroidBridge.Core
{
    /// <summary>
    /// Represents a parsed Java class
    /// </summary>
    [System.Serializable]
    public class JavaClass
    {
        public string PackageName;
        public string ClassName;
        public List<JavaMethod> Methods = new List<JavaMethod>();
        public List<JavaField> Fields = new List<JavaField>();
        
        public string FullName => string.IsNullOrEmpty(PackageName) 
            ? ClassName 
            : PackageName + "." + ClassName;
    }
    
    /// <summary>
    /// Represents a Java method
    /// </summary>
    [System.Serializable]
    public class JavaMethod
    {
        public bool IsStatic;
        public bool IsPublic = true;
        public string ReturnType;
        public string Name;
        public List<JavaParameter> Parameters = new List<JavaParameter>();
    }
    
    /// <summary>
    /// Represents a Java method parameter
    /// </summary>
    [System.Serializable]
    public class JavaParameter
    {
        public string Type;
        public string Name;
    }
    
    /// <summary>
    /// Represents a Java field
    /// </summary>
    [System.Serializable]
    public class JavaField
    {
        public bool IsStatic;
        public bool IsPublic = true;
        public string Type;
        public string Name;
    }
}
