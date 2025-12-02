# End-to-End Workflow: APK to Unity

## 🎯 Complete Standalone Implementation

This guide shows you how to create a **completely standalone** implementation that doesn't depend on the original APK being installed on the device.

---

## 📋 Overview

```
ghostparty.apk (Viture SDK sample app)
    ↓ Study & Extract
1. Learn API structure
2. Extract native libraries
    ↓ Implement
3. Write YOUR Java wrapper
4. Build YOUR AAR
    ↓ Integrate
5. Generate C# bridge
6. Use in Unity
    ↓ Result
Standalone app with Viture SDK functionality!
```

---

## 🚀 Step-by-Step Workflow

### **Step 1: Extract APK to Learn the API**

```
Tools > Android Bridge Toolkit > 1. Extract APK Classes
```

1. Select `ghostparty.apk`
2. Click **"Extract Classes"**
3. Browse the extracted classes

**What you learn:**
```
Class: viture.xr.IXRHandTracking
Methods:
  - boolean enterExclusiveHandTracking()
  - boolean exitExclusiveHandTracking()
  - void registerHandTrackingListener()
```

**This tells you the API signatures you need to implement!**

---

### **Step 2: Extract Native Libraries**

Still in APK Extractor:

1. **Uncheck** "Copy APK to project" (we don't need it!)
2. Click **"Extract Native Libraries"**

**Result:**
```
Assets/Plugins/Android/libs/
└── arm64-v8a/
    └── libviture_sdk.so    ← The actual implementation
```

---

### **Step 3: Write Your Java Wrapper**

```
Tools > Android Bridge Toolkit > 3. Build AAR
```

**Java Code Tab:**

```java
package com.yourcompany.viture;

/**
 * Standalone Viture SDK Wrapper
 * 
 * This loads libviture_sdk.so and provides access to
 * hand tracking functionality WITHOUT needing ghostparty.apk!
 */
public class VitureSDKWrapper {
    
    // Load the native library
    static {
        System.loadLibrary("viture_sdk");
    }
    
    // Native methods - signatures match what we learned from ghostparty!
    
    /**
     * Enable exclusive hand tracking mode
     * @return true if successful
     */
    public static native boolean enterExclusiveHandTracking();
    
    /**
     * Disable exclusive hand tracking mode  
     * @return true if successful
     */
    public static native boolean exitExclusiveHandTracking();
    
    /**
     * Register a hand tracking listener
     */
    public static native void registerHandTrackingListener();
    
    /**
     * Unregister hand tracking listener
     */
    public static native void unregisterHandTrackingListener();
}
```

**Key Points:**
- `System.loadLibrary("viture_sdk")` loads `libviture_sdk.so`
- Method signatures **must match exactly** what the .so expects
- You learned these signatures by studying ghostparty.apk!

---

### **Step 4: Build Your AAR**

**Settings Tab:**
- Project Name: `VitureWrapper`
- Package Name: `com.yourcompany.viture`

**Build Tab:**
1. Click **"Build AAR"**
2. Wait for Gradle to compile
3. AAR is automatically copied to `Assets/Plugins/Android/`

**Result:**
```
Assets/Plugins/Android/
├── VitureWrapper.aar        ← YOUR code
└── libs/
    └── arm64-v8a/
        └── libviture_sdk.so ← Native library
```

**You now have a standalone SDK!** 🎉

---

### **Step 5: Generate C# Bridge**

```
Tools > Android Bridge Toolkit > 2. Generate Bridge
```

1. Paste your Java wrapper code (from Step 3)
2. Click **"Generate Bridge"**
3. Save as `VitureSDKWrapperBridge.cs`

**Generated Code:**
```csharp
using UnityEngine;

public class VitureSDKWrapperBridge {
    private static AndroidJavaClass javaClass;
    
    static VitureSDKWrapperBridge() {
        javaClass = new AndroidJavaClass("com.yourcompany.viture.VitureSDKWrapper");
    }
    
    public static bool EnterExclusiveHandTracking() {
        return javaClass.CallStatic<bool>("enterExclusiveHandTracking");
    }
    
    public static bool ExitExclusiveHandTracking() {
        return javaClass.CallStatic<bool>("exitExclusiveHandTracking");
    }
    
    public static void RegisterHandTrackingListener() {
        javaClass.CallStatic("registerHandTrackingListener");
    }
    
    public static void UnregisterHandTrackingListener() {
        javaClass.CallStatic("unregisterHandTrackingListener");
    }
}
```

---

### **Step 6: Use in Unity!**

```csharp
using UnityEngine;

public class VitureHandTracking : MonoBehaviour 
{
    void Start() 
    {
        // YOUR wrapper, YOUR code, no ghostparty.apk needed!
        bool success = VitureSDKWrapperBridge.EnterExclusiveHandTracking();
        
        if (success) 
        {
            Debug.Log("Hand tracking enabled!");
            VitureSDKWrapperBridge.RegisterHandTrackingListener();
        }
    }
    
    void OnDestroy() 
    {
        VitureSDKWrapperBridge.UnregisterHandTrackingListener();
        VitureSDKWrapperBridge.ExitExclusiveHandTracking();
    }
}
```

---

## 🎯 How It Works

```
Your Unity C# Code
  ↓
VitureSDKWrapperBridge.cs (generated)
  ↓
VitureWrapper.aar (YOUR Java code)
  ↓ System.loadLibrary()
libviture_sdk.so (native implementation)
  ↓
Viture Hardware
```

**NO ghostparty.apk dependency!** ✅

---

## 📁 Final Project Structure

```
Assets/
├── Plugins/
│   └── Android/
│       ├── VitureWrapper.aar          ← YOUR wrapper (Java)
│       └── libs/
│           └── arm64-v8a/
│               └── libviture_sdk.so   ← Native library
├── Scripts/
│   └── VitureAPI/
│       └── VitureSDKWrapperBridge.cs  ← Generated bridge (C#)
└── Game/
    └── VitureHandTracking.cs          ← Your game code
```

---

## ⚙️ Requirements

### **For AAR Building:**

1. **Gradle** must be installed
   ```bash
   # Check if installed:
   gradle --version
   
   # Install on Mac:
   brew install gradle
   
   # Install on Windows:
   # Download from gradle.org or use Chocolatey:
   choco install gradle
   ```

2. **JAVA_HOME** must be set
   ```bash
   # Check:
   echo $JAVA_HOME
   
   # Set (Mac/Linux):
   export JAVA_HOME=/Library/Java/JavaVirtualMachines/jdk-xx.jdk/Contents/Home
   
   # Set (Windows):
   setx JAVA_HOME "C:\Program Files\Java\jdk-xx"
   ```

3. **Android SDK** (usually installed with Unity)

---

## 🐛 Troubleshooting

### **"UnsatisfiedLinkError" at runtime**

**Problem:** Native library not loading

**Solutions:**
1. Verify `libviture_sdk.so` is in `Assets/Plugins/Android/libs/arm64-v8a/`
2. Check library name matches: `System.loadLibrary("viture_sdk")` → `libviture_sdk.so`
3. Ensure correct architecture for your device
4. Rebuild Unity project

### **"Gradle build failed"**

**Problem:** AAR build failed

**Solutions:**
1. Check Gradle is installed: `gradle --version`
2. Verify JAVA_HOME is set
3. Check build log in AAR Builder window
4. Ensure Java package name matches in code and settings

### **Method signatures don't match**

**Problem:** `NoSuchMethodError` at runtime

**Solutions:**
1. Verify method signatures exactly match what the .so expects
2. Check parameter types (int vs long, etc.)
3. Re-study the original APK to confirm signatures
4. Use exact same method names (case-sensitive!)

### **"ClassNotFoundException"**

**Problem:** Can't find your wrapper class

**Solutions:**
1. Verify AAR was built successfully
2. Check AAR is in `Assets/Plugins/Android/`
3. Verify package name matches in Java and C# bridge
4. Rebuild Unity project

---

## 💡 Pro Tips

1. **Study First, Build Second** - Fully understand the API before writing your wrapper
2. **Match Signatures Exactly** - The .so expects exact method signatures
3. **Test Incrementally** - Add one method at a time
4. **Keep It Simple** - Start with basic methods, add complexity later
5. **Document Everything** - Future you will thank present you!

---

## 🎊 Advantages of This Approach

✅ **No External Dependencies** - Your app is self-contained  
✅ **Smaller APK Size** - Only includes what you use  
✅ **Full Control** - You own the wrapper code  
✅ **Better Performance** - Direct native calls  
✅ **Easier Debugging** - You wrote the Java layer  
✅ **App Store Safe** - No bundled APKs to worry about  

---

## 📚 Next Steps

- [AAR Builder Documentation](Documentation/AARBuilder.md)
- [Bridge Generator Guide](Documentation/BridgeGenerator.md)
- [Native Library Integration](COMPLETE_APK_INTEGRATION.md)
- [Troubleshooting Guide](Documentation/Troubleshooting.md)

---

## 🎯 Summary

You've learned how to:
1. **Reverse engineer** an APK to learn the API
2. **Extract** native libraries for reuse
3. **Write** your own Java wrapper
4. **Build** a standalone AAR
5. **Generate** C# bridges automatically
6. **Integrate** everything into Unity

**Result:** A completely standalone Unity app with native SDK functionality! 🚀
