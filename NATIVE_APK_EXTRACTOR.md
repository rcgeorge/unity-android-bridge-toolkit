# Native APK Class Extractor

## 🎉 Self-Contained Solution - No External Tools Required!

The Android Bridge Toolkit now includes a **native C# DEX parser** that extracts class metadata directly from APK files without requiring any external tools like JADX!

## Features

### ✅ What It Extracts
- **Class Names & Packages** - Full class hierarchy
- **Method Signatures** - Names, return types, parameters
- **Access Modifiers** - Public/private, static/instance
- **Complete Metadata** - Everything needed for bridge generation

### ✅ Benefits
- **Self-Contained** - No JADX or external dependencies
- **Fast** - Metadata extraction is much faster than full decompilation
- **Cross-Platform** - Works on Windows, Mac, and Linux
- **Purpose-Built** - Designed specifically for bridge generation
- **Easy to Use** - Just select an APK and click extract

## How It Works

### 1. APK Structure
An APK is just a ZIP file containing:
```
my-app.apk
├── classes.dex          ← Android bytecode (Dalvik)
├── classes2.dex         ← Additional DEX files
├── AndroidManifest.xml
├── res/                 ← Resources
└── lib/                 ← Native libraries
```

### 2. Native Extraction Process
```
 APK File
    ↓
 [Unzip with System.IO.Compression]
    ↓
 Extract classes.dex, classes2.dex, etc.
    ↓
 [Parse DEX Format]
    ↓
 Extract Class Metadata
    ↓
 Filter System Classes
    ↓
 Present in Class Browser
    ↓
 Generate C# Bridge
```

### 3. DEX File Parsing
Our `DexParser.cs` implements the official DEX file format specification:
- **Header Parsing** - Magic number, offsets, sizes
- **String Table** - All string constants
- **Type Table** - Type descriptors
- **Class Definitions** - Class metadata
- **Method Definitions** - Method signatures

Reference: [DEX Format Specification](https://source.android.com/docs/core/runtime/dex-format)

## Usage

### Via Main Menu
```
Tools > Android Bridge Toolkit > 1. Extract APK Classes
```

### Workflow
1. **Select APK** - Choose your APK file
2. **Extract** - Click "Extract Class Metadata"
3. **Browse** - Explore classes in the browser
4. **Generate** - Select a class and generate C# bridge
5. **Save** - Save the bridge to your Unity project

### Example Output
```
📦 Extracted Classes (42)

📁 com.viture.sdk
  📄 VitureSDK (5 methods)
  📄 XRManager (12 methods)
  📄 DisplayController (8 methods)

📁 com.viture.display
  📄 ScreenManager (15 methods)
  📄 BrightnessController (6 methods)
```

## Components

### Core Files
- **`DexParser.cs`** - Native DEX file parser
- **`APKExtractor.cs`** - APK unzip and class extraction
- **`APKDecompilerWindow.cs`** - Main extraction UI
- **`DecompiledFileBrowser.cs`** - Class browser and bridge integration

### Key Classes

#### DexParser
```csharp
public class DexParser
{
    public DexParser(byte[] dexData);
    public List<DexClass> Parse();
}
```

#### APKExtractor
```csharp
public static class APKExtractor
{
    public static List<DexClass> ExtractClasses(
        string apkPath,
        Action<float, string> onProgress
    );
    
    public static JavaClass ConvertToJavaClass(DexClass dexClass);
}
```

#### DexClass
```csharp
public class DexClass
{
    public string ClassName;
    public bool IsPublic;
    public List<DexMethod> Methods;
    
    public string GetPackageName();
    public string GetSimpleClassName();
}
```

## Comparison: Native vs JADX

| Feature | Native Extractor | JADX |
|---------|-----------------|------|
| **Installation** | Built-in ✅ | External tool required ❌ |
| **Speed** | Fast (metadata only) ⚡ | Slower (full decompilation) |
| **Output** | Class signatures | Full Java source |
| **Purpose** | Bridge generation | Code study |
| **Dependencies** | None | Java required |
| **Cross-platform** | Yes ✅ | Yes |
| **Use Case** | Automated workflows | Manual inspection |

## Limitations

### What It Doesn't Do
- ❌ Full Java source code decompilation
- ❌ Method implementations
- ❌ Comments and documentation
- ❌ Original variable names

### Why That's OK
For generating C# bridges, you only need:
- ✅ Class and package names
- ✅ Method signatures
- ✅ Parameter types
- ✅ Return types

**The native extractor gives you exactly this - nothing more, nothing less!**

## Future Enhancements

Potential improvements for future versions:
- [ ] Parse method parameters from type_list
- [ ] Extract field definitions
- [ ] Parse AndroidManifest.xml (binary XML)
- [ ] Support for multi-dex APKs (already handled)
- [ ] Annotation parsing
- [ ] Generic type parameter extraction

## Technical Details

### DEX File Format
The DEX format is well-documented by Android:
- **Magic Number**: `dex\n035\0` or `dex\n037\0`
- **Endianness**: Little-endian
- **Strings**: Modified UTF-8 encoding
- **Integers**: ULEB128 variable-length encoding

### System Class Filtering
We filter out common system packages:
```csharp
string[] systemPackages = {
    "android.",
    "androidx.",
    "com.google.android.",
    "java.",
    "javax.",
    "kotlin.",
    "kotlinx."
};
```

## Credits

**Built for the Unity Android Bridge Toolkit**
- Designed specifically for Viture XR glasses integration
- Self-contained solution requiring no external tools
- Optimized for bridge generation workflows

## Contributing

Want to improve the DEX parser? Areas for contribution:
1. **Parameter Parsing** - Extract parameter names and types
2. **Annotation Support** - Parse Java annotations
3. **Generics** - Better generic type handling
4. **Performance** - Optimize parsing for large APKs

## References

- [Android DEX Format Specification](https://source.android.com/docs/core/runtime/dex-format)
- [Dalvik Bytecode](https://source.android.com/docs/core/runtime/dalvik-bytecode)
- [Unity AndroidJavaClass Documentation](https://docs.unity3d.com/ScriptReference/AndroidJavaClass.html)

---

**Made with ❤️ by Instemic for Viture XR Development**
