# Unity Android Bridge Toolkit

**🧙 Step-by-step wizard for APK to Unity integration - No Gradle required!**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Unity](https://img.shields.io/badge/Unity-2021.3%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![Platform](https://img.shields.io/badge/Platform-Android-green.svg)](https://developer.android.com/)

## 🚀 What is this?

**The simplest way to integrate Android functionality into Unity!**

A complete Unity editor plugin with a **step-by-step wizard** that:
1. 📦 **Extracts APIs from APKs** - Learn how existing apps work
2. 🔨 **Builds AARs with pure C#** - No Gradle installation needed!
3. 🌉 **Generates C# bridges automatically** - From Java to Unity
4. 📚 **Extracts native libraries** - Complete integration

**Perfect for:**
- Using Viture XR SDK from ghostparty.apk
- Integrating third-party Android SDKs without documentation
- Creating standalone Unity apps with native functionality
- Learning Android APIs by example

## ✨ Key Features

### 🧙 **Unified Workflow Wizard (NEW!)**
- **One window for everything** - No jumping between tools
- **Step-by-step guidance** - Never get lost
- **Visual progress** - See where you are
- **Smart defaults** - Just click Next!

### 🔍 **Native APK Extraction**
- Pure C# DEX parser (no external tools!)
- Extract class metadata and method signatures
- Preview native libraries (.so files)
- Fast and reliable

### 🔨 **AAR Builder - No Gradle Required!**
- Uses javac + C# ZipArchive
- No Gradle installation needed
- Only requirement: JDK (Unity needs this anyway)
- Builds in seconds

### 🌉 **Smart Bridge Generator**
- Automatic Java → C# conversion
- PascalCase naming
- Type-safe wrappers
- XML documentation

## 📦 Installation

### Unity Package Manager (Recommended)

```
1. Window → Package Manager
2. Click '+' → Add package from git URL
3. Enter: https://github.com/rcgeorge/unity-android-bridge-toolkit.git
4. Click 'Add'
```

## 🎬 Quick Start

### The Wizard Way (Easiest!)

```
Tools → Android Bridge Toolkit → 🧙 Workflow Wizard (START HERE!)

Then follow the steps:
1. Load your APK (e.g., ghostparty.apk)
2. Select classes you want to use
3. Review generated Java wrapper
4. Build AAR (one click!)
5. Generate C# bridge (automatic!)
6. Extract native libraries
7. Done! Use in Unity!
```

### Real Example: Viture SDK Integration

**Goal:** Use Viture XR hand tracking without needing ghostparty.apk installed

```
Step 1: Load ghostparty.apk
  → Found 324 classes
  → Found libviture_sdk.so

Step 2: Select IXRHandTracking class
  → enterExclusiveHandTracking()
  → exitExclusiveHandTracking()
  → registerHandTrackingListener()

Step 3: Auto-generated Java wrapper loads the .so

Step 4: Build VitureWrapper.aar (10 seconds!)

Step 5: Generate VitureWrapperBridge.cs

Step 6: Extract libviture_sdk.so to Unity

Step 7: Use in Unity!
```

**Result:**
```csharp
using UnityEngine;

public class HandTracking : MonoBehaviour 
{
    void Start() 
    {
        VitureWrapperBridge.EnterExclusiveHandTracking();
        Debug.Log("Hand tracking enabled!");
    }
}
```

**No ghostparty.apk dependency!** ✅

## 🛠️ Requirements

**Only JDK required!**
- Unity 2021.3 or newer
- JDK 8+ (Unity needs this for Android anyway)
- JAVA_HOME environment variable set

**No Gradle, no Android Studio, no external tools!**

## 📁 What Gets Created

```
Assets/
├── Plugins/Android/
│   ├── VitureWrapper.aar        ← Your wrapper (built in Unity!)
│   └── libs/
│       └── arm64-v8a/
│           └── libviture_sdk.so ← Native library
└── Scripts/
    └── VitureWrapperBridge.cs   ← Auto-generated C# bridge
```

## 🎯 Complete Workflow

```
ghostparty.apk
    ↓ Extract & Learn
Classes + Native Libraries
    ↓ Create Wrapper
Your Java wrapper code
    ↓ Build AAR
VitureWrapper.aar
    ↓ Generate Bridge
VitureWrapperBridge.cs
    ↓ Use in Unity
Standalone app! 🎉
```

## 📖 Documentation

- **[END_TO_END_WORKFLOW.md](END_TO_END_WORKFLOW.md)** - Complete guide
- **[COMPLETE_APK_INTEGRATION.md](COMPLETE_APK_INTEGRATION.md)** - Technical details
- **[NATIVE_APK_EXTRACTOR.md](NATIVE_APK_EXTRACTOR.md)** - Extraction system

## 💡 Advanced Usage

The toolkit also provides individual tools for advanced users:

```
Tools → Android Bridge Toolkit → Advanced/
  ├── 1. Extract APK Classes
  ├── 2. Generate Bridge
  └── 3. Build AAR
```

## ⚖️ Legal & Ethics

**Important:**
- ✅ Decompilation for learning and interoperability is generally legal
- ✅ Creating your own implementations is fine
- ✅ Extracting native libraries for your own wrapper is fine
- ❌ **Do not copy proprietary code verbatim**
- ❌ **Respect licenses and terms of service**
- ❌ **Do not redistribute decompiled code**

This tool is for **educational and interoperability purposes only.**

## 🐛 Troubleshooting

### "javac not found"
```bash
# Set JAVA_HOME
export JAVA_HOME=/path/to/jdk
# or on Windows:
setx JAVA_HOME "C:\Program Files\Java\jdk-xx"
```

### "UnsatisfiedLinkError" at runtime
- Verify .so files are in `Assets/Plugins/Android/libs/[arch]/`
- Check library name matches in Java: `System.loadLibrary("name")`
- Ensure correct architecture for your device

### "Build failed"
- Check build log in wizard
- Verify JDK is installed: `javac -version`
- Ensure JAVA_HOME is set correctly

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details.

## 🙏 Credits

Developed by [Instemic](https://github.com/rcgeorge) for **Viture XR** development

## 🗺️ Roadmap

### v1.0 ✅ (Current)
- [x] Workflow Wizard
- [x] Native APK extraction
- [x] AAR building (no Gradle!)
- [x] Bridge generation
- [x] Native library extraction
- [x] Complete documentation

### v1.1 (Next)
- [ ] Support for more Java types
- [ ] Batch processing
- [ ] Custom templates library
- [ ] UI improvements

### v2.0 (Future)
- [ ] iOS bridge support
- [ ] Visual scripting integration
- [ ] Cloud build support

## 💬 Support

- 🐛 **Issues**: [GitHub Issues](https://github.com/rcgeorge/unity-android-bridge-toolkit/issues)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/rcgeorge/unity-android-bridge-toolkit/discussions)
- 📖 **Docs**: See documentation files in repo

## ⭐ Star Us!

If this toolkit helps you, please ⭐ star the repo!

---

**Made with ❤️ by Instemic for the Unity community**

*"From APK to Unity in 7 easy steps!"* 🧙
