# Complete APK Integration Guide

## 🎯 Complete Workflow: From APK to Unity

This guide shows you how to extract **everything** from an APK and use it in Unity.

---

## 📦 What You'll Extract

### 1. **Java Classes** (DEX Bytecode)
- Class names and packages
- Method signatures
- Used for: Generating C# bridges

### 2. **Native Libraries** (.so files)
- ARM, ARM64, x86, x86_64 libraries
- Contains actual implementation code
- Used for: Runtime functionality

### 3. **APK File** (Optional)
- The complete APK
- Used for: Accessing resources and classes at runtime

---

## 🚀 Step-by-Step Guide

### Step 1: Open APK Extractor

```
Tools > Android Bridge Toolkit > 1. Extract APK Classes
```

### Step 2: Select Your APK

1. Click **Browse**
2. Select your APK file (e.g., `viture-sdk.apk`, `ghostparty.apk`)
3. The tool will preview how many native libraries are in the APK

### Step 3: Extract Class Metadata

1. Click **🔍 Extract Classes**
2. Wait for extraction to complete
3. You'll see: "✅ Extracted XXX classes"

**What this does:**
- Parses DEX files from APK
- Extracts class names and method signatures
- Filters out system classes (android.*, java.*, etc.)

### Step 4: Extract Native Libraries

1. ✅ Check **"Also copy APK to Assets/Plugins/Android/"** (recommended)
2. Click **📲 Extract Native Libraries (.so files)**
3. Wait for extraction to complete

**What this does:**
- Extracts all `.so` files from APK's `lib/` folder
- Copies them to: `Assets/Plugins/Android/libs/[architecture]/`
- Optionally copies APK to: `Assets/Plugins/Android/[apkname].apk`
- Automatically refreshes Unity's asset database

### Step 5: Browse Classes & Generate Bridges

1. Click **📂 Browse Classes & Generate Bridges**
2. Search/filter for the class you want
3. Click on a class to see its methods
4. Click **⚡ Generate C# Bridge**
5. Save the C# file to your project

---

## 📁 Final Project Structure

After extraction, your Unity project will look like this:

```
Assets/
├── Plugins/
│   └── Android/
│       ├── viture-sdk.apk              ← The APK (Java classes)
│       └── libs/
│           ├── arm64-v8a/
│           │   ├── libviture_sdk.so    ← Native library (ARM64)
│           │   └── libunity.so
│           └── armeabi-v7a/
│               └── libviture_sdk.so    ← Native library (ARM32)
│
├── Scripts/
│   └── VitureAPI/
│       ├── IXRHandTrackingBridge.cs    ← Generated C# bridge
│       ├── IXRGlassesCommandBridge.cs
│       └── ...
│
└── YourGame/
    └── HandTrackingManager.cs          ← Your code using the bridges
```

---

## 🎮 Using the Generated Bridges

### Example: Viture Hand Tracking

```csharp
using UnityEngine;
using VitureAPI;

public class HandTrackingManager : MonoBehaviour 
{
    private IXRHandTrackingBridge handTracking;
    
    void Start() 
    {
        // Create bridge (loads from APK)
        handTracking = new IXRHandTrackingBridge();
        
        // Call Java method (uses native .so)
        bool success = handTracking.EnterExclusiveHandTracking();
        Debug.Log($"Hand tracking enabled: {success}");
    }
    
    void OnDestroy() 
    {
        // Clean up
        if (handTracking != null)
        {
            handTracking.ExitExclusiveHandTracking();
        }
    }
}
```

---

## 🔍 How It Works at Runtime

```
Your C# Code
    ↓
IXRHandTrackingBridge.cs
    ↓ AndroidJavaClass.CallStatic()
Java Class (from viture-sdk.apk)
    ↓ JNI Call
Native Library (libviture_sdk.so)
    ↓
Hardware / Android System
```

---

## ✅ Architecture Support

The toolkit automatically handles multiple architectures:

| Architecture | Directory | Used For |
|-------------|-----------|----------|
| `arm64-v8a` | `Assets/Plugins/Android/libs/arm64-v8a/` | 64-bit ARM devices (most modern Android) |
| `armeabi-v7a` | `Assets/Plugins/Android/libs/armeabi-v7a/` | 32-bit ARM devices (older Android) |
| `x86_64` | `Assets/Plugins/Android/libs/x86_64/` | 64-bit x86 emulators |
| `x86` | `Assets/Plugins/Android/libs/x86/` | 32-bit x86 emulators |

Unity will automatically pick the correct architecture when building your APK!

---

## 🔧 Build Settings

Make sure your Unity project is configured for Android:

1. **File > Build Settings**
2. Select **Android** platform
3. Click **Switch Platform**
4. In **Player Settings**:
   - Set **Minimum API Level**: Android 7.0 (API 24) or higher
   - Enable **ARM64** in **Target Architectures**

---

## 🎯 Use Cases

### ✅ Perfect For:
- Using proprietary SDKs (like Viture XR)
- Wrapping Android libraries without source code
- Reverse engineering APK functionality
- Quick prototyping with Android features

### ❌ Not Needed For:
- Official SDKs with Unity packages
- Open-source Android libraries (use source directly)
- Simple Android APIs (use Unity's built-in AndroidJavaClass)

---

## 💡 Pro Tips

1. **Always extract both** classes and native libraries - you need both!
2. **Check the preview** - if it shows 0 native libraries, the APK is pure Java
3. **Copy APK to project** - Unity needs it to load classes at runtime
4. **Test on device** - Emulators may behave differently with native code
5. **Keep APK updated** - If the APK changes, re-extract everything

---

## 🐛 Troubleshooting

### "No native libraries found"
- The APK is pure Java (no native code)
- You only need the APK file in `Assets/Plugins/Android/`

### "UnsatisfiedLinkError" at runtime
- Native library not extracted correctly
- Check that .so files are in the correct architecture folders
- Rebuild your Unity project

### "ClassNotFoundException" at runtime
- APK not copied to `Assets/Plugins/Android/`
- Enable "Copy APK to project" when extracting native libraries

### Bridge code doesn't compile
- Make sure to add `using UnityEngine;`
- AndroidJavaClass requires Unity's Android support

---

## 🎊 You're Done!

You now have:
- ✅ Java classes accessible from C#
- ✅ Native libraries in your project
- ✅ C# bridges for easy calling
- ✅ Complete APK integration

**Next**: Build your app and test on device! 🚀

---

## 📚 Additional Resources

- [Native APK Extractor Documentation](NATIVE_APK_EXTRACTOR.md)
- [Bridge Generator Guide](Documentation/BridgeGenerator.md)
- [Quick Start Guide](Documentation/QuickStart.md)
- [GitHub Issues](https://github.com/rcgeorge/unity-android-bridge-toolkit/issues)
