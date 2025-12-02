# Unity Android Bridge Toolkit

**Create Android bridges for Unity in minutes, not hours!**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Unity](https://img.shields.io/badge/Unity-2021.3%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![Platform](https://img.shields.io/badge/Platform-Android-green.svg)](https://developer.android.com/)

## 🚀 What is this?

A complete Unity editor plugin that lets you:
- 📦 **Decompile APKs** to study Android implementations
- 🔨 **Build AARs** directly in Unity (no Android Studio needed!)
- 🌉 **Generate C# bridges** automatically from Java code
- 🎮 **Use immediately** in your Unity projects

Perfect for:
- Integrating third-party Android SDKs
- Building custom Android plugins
- Learning from existing apps (legally!)
- Creating Unity-Android bridges

## ✨ Features

### 🔍 APK Decompiler
- Pure C# implementation (no JADX required for basic use)
- Optional JADX integration for advanced decompilation
- Browse decompiled classes in Unity
- Search and filter functionality
- Extract resources and assets

### 🏗️ AAR Builder
- Build Android libraries without Android Studio
- Multiple project templates (Unity Bridge, System Service, Native Wrapper)
- Dependency management
- Custom Gradle configuration
- One-click builds

### 🌉 Bridge Generator
- **Automatic C# bridge generation from Java code**
- Smart type conversion (Java ↔ C#)
- Handles static and instance methods
- Supports primitives, arrays, and objects
- Template-based with customization

### 🎯 End-to-End Workflow
```
APK → Decompile → Select Classes → Generate Bridge → Build AAR → Use in Unity!
```

## 📦 Installation

### Method 1: Unity Package Manager (Recommended)

1. Open Unity Package Manager (Window → Package Manager)
2. Click '+' → Add package from git URL
3. Enter: `https://github.com/rcgeorge/unity-android-bridge-toolkit.git`
4. Click 'Add'

### Method 2: Manual Installation

1. Download the latest release
2. Extract to `Assets/AndroidBridgeToolkit`
3. Unity will automatically import

## 🎬 Quick Start

### Example: Complete Workflow

```csharp
// 1. Open the toolkit
Tools → Android Bridge Toolkit

// 2. Decompile APK
- Select your APK file
- Click "Decompile"
- Wait for completion

// 3. Generate Bridge
- Browse decompiled classes
- Select the class you want
- Click "Generate C# Bridge"
- Save the generated code

// 4. Use in Unity!
var bridge = new MySDKBridge();
bridge.Init();
```

## 📖 Use Cases

### Viture XR Integration Example
```csharp
// Study existing implementation, create your own
1. Decompile ghostparty.apk
2. Find VitureUnityXRManager.java
3. Generate C# bridge automatically
4. Build your own AAR
5. Ship your Unity app!
```

### Custom Android Plugin
```csharp
// Build native Android features
1. Create AAR project with template
2. Add your Java code
3. Build AAR
4. Generated bridge ready to use!
```

## 🛠️ Requirements

- **Unity:** 2021.3 or newer
- **Gradle:** 7.0+ (auto-downloaded if needed)
- **Java:** JDK 8+ (for building AARs)
- **Platform:** Windows, macOS, Linux

## 📝 Generated Bridge Example

**Input (Java):**
```java
package com.example.sdk;

public class MySDK {
    public static void init() { }
    public static String getMessage() { return "Hello"; }
    public static int calculate(int a, int b) { return a + b; }
}
```

**Output (C# - Auto-generated):**
```csharp
using UnityEngine;

public class MySDKBridge
{
    private static AndroidJavaClass javaClass;
    
    static MySDKBridge()
    {
        javaClass = new AndroidJavaClass("com.example.sdk.MySDK");
    }
    
    public static void Init()
    {
        javaClass.CallStatic("init");
    }
    
    public static string GetMessage()
    {
        return javaClass.CallStatic<string>("getMessage");
    }
    
    public static int Calculate(int a, int b)
    {
        return javaClass.CallStatic<int>("calculate", a, b);
    }
}
```

## ⚖️ Legal & Ethics

**Important:**
- ✅ Decompilation for learning and interoperability is generally legal
- ✅ Creating your own implementations is fine
- ❌ **Do not copy proprietary code verbatim**
- ❌ **Respect licenses and terms of service**
- ❌ **Do not distribute decompiled code**

This tool is for **educational and interoperability purposes only.**

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details.

## 🙏 Credits

Developed by [Instemic](https://github.com/rcgeorge)

Special thanks to:
- **JADX** - Optional decompilation backend
- **Gradle** - Build system
- **Unity Technologies** - Unity Engine

## 🗺️ Roadmap

### v1.0 (In Development)
- [x] Project structure
- [ ] APK decompilation core
- [ ] AAR building
- [ ] Bridge generation
- [ ] Basic templates
- [ ] Documentation

### v1.1 (Planned)
- [ ] Improved bytecode decompilation
- [ ] More templates (Firebase, AdMob, etc.)
- [ ] Batch processing
- [ ] UI improvements

### v2.0 (Future)
- [ ] Visual scripting integration
- [ ] iOS bridge support
- [ ] Cloud build support

## 💬 Support

- 🐛 Issues: [GitHub Issues](https://github.com/rcgeorge/unity-android-bridge-toolkit/issues)
- 💬 Discussions: [GitHub Discussions](https://github.com/rcgeorge/unity-android-bridge-toolkit/discussions)

---

**Made with ❤️ by Instemic**

*"Stop fighting with Android, start shipping!"*
