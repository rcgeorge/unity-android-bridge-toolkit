//
// DexParser.cs - Native DEX file parser for extracting class metadata
// Copyright (c) 2024 Instemic
//
// Reference: https://source.android.com/docs/core/runtime/dex-format
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Instemic.AndroidBridge.Core
{
    /// <summary>
    /// Parses Android DEX files to extract class and method metadata
    /// </summary>
    public class DexParser
    {
        private byte[] dexData;
        private DexHeader header;
        private List<string> stringTable;
        private List<string> typeTable;
        private List<DexClass> classes;
        
        public DexParser(byte[] data)
        {
            dexData = data;
            stringTable = new List<string>();
            typeTable = new List<string>();
            classes = new List<DexClass>();
        }
        
        /// <summary>
        /// Parse the DEX file and extract all class metadata
        /// </summary>
        public List<DexClass> Parse()
        {
            try
            {
                // Parse header
                header = ParseHeader();
                
                // Parse string table
                ParseStringTable();
                
                // Parse type table
                ParseTypeTable();
                
                // Parse class definitions
                ParseClassDefs();
                
                return classes;
            }
            catch (Exception ex)
            {
                Debug.LogError($"DEX parsing error: {ex.Message}");
                throw;
            }
        }
        
        private DexHeader ParseHeader()
        {
            DexHeader h = new DexHeader();
            
            // Check magic number
            string magic = Encoding.ASCII.GetString(dexData, 0, 4);
            if (magic != "dex\n")
            {
                throw new Exception("Not a valid DEX file - invalid magic number");
            }
            
            // Parse header fields
            h.stringIdsSize = ReadUInt32(56);
            h.stringIdsOff = ReadUInt32(60);
            h.typeIdsSize = ReadUInt32(64);
            h.typeIdsOff = ReadUInt32(68);
            h.protoIdsSize = ReadUInt32(72);
            h.protoIdsOff = ReadUInt32(76);
            h.fieldIdsSize = ReadUInt32(80);
            h.fieldIdsOff = ReadUInt32(84);
            h.methodIdsSize = ReadUInt32(88);
            h.methodIdsOff = ReadUInt32(92);
            h.classDefsSize = ReadUInt32(96);
            h.classDefsOff = ReadUInt32(100);
            
            return h;
        }
        
        private void ParseStringTable()
        {
            uint offset = header.stringIdsOff;
            
            for (int i = 0; i < header.stringIdsSize; i++)
            {
                uint stringDataOff = ReadUInt32(offset + (uint)(i * 4));
                string str = ReadString(stringDataOff);
                stringTable.Add(str);
            }
        }
        
        private void ParseTypeTable()
        {
            uint offset = header.typeIdsOff;
            
            for (int i = 0; i < header.typeIdsSize; i++)
            {
                uint descriptorIdx = ReadUInt32(offset + (uint)(i * 4));
                string typeName = stringTable[(int)descriptorIdx];
                typeTable.Add(typeName);
            }
        }
        
        private void ParseClassDefs()
        {
            uint offset = header.classDefsOff;
            
            for (int i = 0; i < header.classDefsSize; i++)
            {
                uint classIdx = ReadUInt32(offset);
                uint accessFlags = ReadUInt32(offset + 4);
                uint classDataOff = ReadUInt32(offset + 24);
                
                string className = typeTable[(int)classIdx];
                
                // Skip array types and primitive types
                if (className.StartsWith("[") || className.Length <= 3)
                {
                    offset += 32;
                    continue;
                }
                
                // Convert from descriptor format (Lcom/example/Class;) to Java format
                className = ConvertDescriptorToClassName(className);
                
                // Parse access flags
                // Reference: https://source.android.com/docs/core/runtime/dex-format#access-flags
                DexClass dexClass = new DexClass
                {
                    ClassName = className,
                    IsPublic = (accessFlags & 0x0001) != 0,
                    IsInterface = (accessFlags & 0x0200) != 0,
                    IsAbstract = (accessFlags & 0x0400) != 0,
                    IsEnum = (accessFlags & 0x4000) != 0
                };
                
                // Parse class data if present
                if (classDataOff != 0)
                {
                    ParseClassData(classDataOff, dexClass);
                }
                
                classes.Add(dexClass);
                offset += 32;
            }
        }
        
        private void ParseClassData(uint offset, DexClass dexClass)
        {
            // Parse class data structure
            // This is a simplified version - full parsing is more complex
            try
            {
                uint staticFieldsSize = ReadULeb128(ref offset);
                uint instanceFieldsSize = ReadULeb128(ref offset);
                uint directMethodsSize = ReadULeb128(ref offset);
                uint virtualMethodsSize = ReadULeb128(ref offset);
                
                // Skip static fields
                for (int i = 0; i < staticFieldsSize; i++)
                {
                    ReadULeb128(ref offset); // field_idx_diff
                    ReadULeb128(ref offset); // access_flags
                }
                
                // Skip instance fields
                for (int i = 0; i < instanceFieldsSize; i++)
                {
                    ReadULeb128(ref offset); // field_idx_diff
                    ReadULeb128(ref offset); // access_flags
                }
                
                // Parse direct methods (includes constructors, static methods, private methods)
                uint methodIdx = 0;
                for (int i = 0; i < directMethodsSize; i++)
                {
                    uint methodIdxDiff = ReadULeb128(ref offset);
                    methodIdx += methodIdxDiff;
                    uint accessFlags = ReadULeb128(ref offset);
                    uint codeOff = ReadULeb128(ref offset);
                    
                    ParseMethod(methodIdx, accessFlags, dexClass, true);
                }
                
                // Parse virtual methods
                methodIdx = 0;
                for (int i = 0; i < virtualMethodsSize; i++)
                {
                    uint methodIdxDiff = ReadULeb128(ref offset);
                    methodIdx += methodIdxDiff;
                    uint accessFlags = ReadULeb128(ref offset);
                    uint codeOff = ReadULeb128(ref offset);
                    
                    ParseMethod(methodIdx, accessFlags, dexClass, false);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error parsing class data for {dexClass.ClassName}: {ex.Message}");
            }
        }
        
        private void ParseMethod(uint methodIdx, uint accessFlags, DexClass dexClass, bool isDirect)
        {
            try
            {
                // Read method_id_item from method_ids table
                uint methodOffset = header.methodIdsOff + (methodIdx * 8);
                uint classIdx = ReadUInt16(methodOffset);
                uint protoIdx = ReadUInt16(methodOffset + 2);
                uint nameIdx = ReadUInt32(methodOffset + 4);
                
                string methodName = stringTable[(int)nameIdx];
                
                // Skip constructors for now
                if (methodName == "<init>" || methodName == "<clinit>")
                    return;
                
                bool isPublic = (accessFlags & 0x0001) != 0;
                bool isStatic = (accessFlags & 0x0008) != 0;
                
                // Parse prototype (return type and parameters)
                string returnType = "void";
                List<string> parameters = new List<string>();
                
                if (protoIdx < header.protoIdsSize)
                {
                    uint protoOffset = header.protoIdsOff + (protoIdx * 12);
                    uint returnTypeIdx = ReadUInt32(protoOffset + 4);
                    
                    if (returnTypeIdx < typeTable.Count)
                    {
                        returnType = ConvertDescriptorToClassName(typeTable[(int)returnTypeIdx]);
                    }
                    
                    // TODO: Parse parameters from type_list
                }
                
                DexMethod method = new DexMethod
                {
                    Name = methodName,
                    ReturnType = returnType,
                    Parameters = parameters,
                    IsPublic = isPublic,
                    IsStatic = isStatic
                };
                
                dexClass.Methods.Add(method);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error parsing method: {ex.Message}");
            }
        }
        
        private string ConvertDescriptorToClassName(string descriptor)
        {
            if (descriptor.StartsWith("L") && descriptor.EndsWith(";"))
            {
                // Lcom/example/MyClass; -> com.example.MyClass
                return descriptor.Substring(1, descriptor.Length - 2).Replace("/", ".");
            }
            
            // Handle primitive types
            switch (descriptor)
            {
                case "V": return "void";
                case "Z": return "boolean";
                case "B": return "byte";
                case "S": return "short";
                case "C": return "char";
                case "I": return "int";
                case "J": return "long";
                case "F": return "float";
                case "D": return "double";
                default: return descriptor;
            }
        }
        
        private string ReadString(uint offset)
        {
            // Skip UTF-16 length
            uint len = ReadULeb128(ref offset);
            
            // Read modified UTF-8 string
            List<byte> bytes = new List<byte>();
            while (dexData[offset] != 0)
            {
                bytes.Add(dexData[offset]);
                offset++;
            }
            
            return Encoding.UTF8.GetString(bytes.ToArray());
        }
        
        private uint ReadUInt32(uint offset)
        {
            return BitConverter.ToUInt32(dexData, (int)offset);
        }
        
        private ushort ReadUInt16(uint offset)
        {
            return BitConverter.ToUInt16(dexData, (int)offset);
        }
        
        private uint ReadULeb128(ref uint offset)
        {
            uint result = 0;
            int shift = 0;
            
            while (true)
            {
                byte b = dexData[offset++];
                result |= (uint)((b & 0x7F) << shift);
                
                if ((b & 0x80) == 0)
                    break;
                    
                shift += 7;
            }
            
            return result;
        }
    }
    
    public class DexHeader
    {
        public uint stringIdsSize;
        public uint stringIdsOff;
        public uint typeIdsSize;
        public uint typeIdsOff;
        public uint protoIdsSize;
        public uint protoIdsOff;
        public uint fieldIdsSize;
        public uint fieldIdsOff;
        public uint methodIdsSize;
        public uint methodIdsOff;
        public uint classDefsSize;
        public uint classDefsOff;
    }
    
    public class DexClass
    {
        public string ClassName;
        public bool IsPublic;
        public bool IsInterface;
        public bool IsAbstract;
        public bool IsEnum;
        public List<DexMethod> Methods = new List<DexMethod>();
        
        public string GetPackageName()
        {
            int lastDot = ClassName.LastIndexOf('.');
            return lastDot > 0 ? ClassName.Substring(0, lastDot) : "";
        }
        
        public string GetSimpleClassName()
        {
            int lastDot = ClassName.LastIndexOf('.');
            return lastDot > 0 ? ClassName.Substring(lastDot + 1) : ClassName;
        }
        
        public string GetClassTypeLabel()
        {
            if (IsInterface) return "interface";
            if (IsEnum) return "enum";
            if (IsAbstract) return "abstract";
            return "class";
        }
    }
    
    public class DexMethod
    {
        public string Name;
        public string ReturnType;
        public List<string> Parameters;
        public bool IsPublic;
        public bool IsStatic;
    }
}
